using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace Kiss.Data.XForce
{
    public abstract class BuckInsert<T> : IDisposable where T : class
    {
        #region properties
        protected Queue<T> buffer;  //ConcurrentQueue<T>      
        protected Semaphore maxBufferSizeSemaphore;
        protected int bufferThresholdSize = 1000;
        protected int maxBufferSize = 10000 * 10000;
        protected int tableMaxSize = 10000 * 1;
        protected DataTable dataTableSchema = new DataTable();
        public virtual string ConnectionString { get; set; }
        public virtual string TableName { get; set; }

        protected int status;
        protected const int statusWaiting = 0;
        protected const int statusEexcuting = 1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BuckInsert()
        {
            buffer = new Queue<T>();
            maxBufferSizeSemaphore = new Semaphore(maxBufferSize, maxBufferSize);
            dataTableSchema = BuildDataSchema();
            CreateTimer();
            //AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler;
        }

        #endregion

        #region public methods

        public void Append(T item)
        {
            if (item == null)
            {
                return;
            }

            maxBufferSizeSemaphore.WaitOne();
            int size = Enqueue(item);
            if (size >= bufferThresholdSize)
            {
                ReadyToFlush(size);
            }
        }

        public void Flush()
        {
            FlushAll();
        }

        #endregion

        #region log
        public static Action<string> Logger;
        public static bool IsDebugEnable = false;

        protected void Log(string text)
        {
            if (Logger == null)
            {
                EventLog.WriteEntry("Application", text);
                return;
            }
            Logger(text);
        }
        #endregion

        #region methods

        protected object lockQuene = new object();
        protected int Enqueue(T item)
        {
            int count;
            lock (lockQuene)
            {
                buffer.Enqueue(item); //TryEnqueue
                count = buffer.Count;
            }
            return count;
        }

        protected void FlushAll()
        {
            FlushBuffer(Int32.MaxValue);
        }

        //ProcessExit event,
        //void Application_End(object sender, EventArgs e)
        protected void ProcessExitHandler(object sender, EventArgs e)
        {
            FlushAll();
        }

        protected void ReadyToFlush(int size)
        {
            int s = Interlocked.CompareExchange(ref status, statusEexcuting, statusWaiting);
            if (s == statusWaiting)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Flush), size);
            }
        }

        protected void Flush(object state)
        {
            try
            {
                Thread.VolatileWrite(ref status, statusEexcuting);
                int size = Convert.ToInt32(state);
                if (size <= 0)
                {
                    return;
                }

                //Console.WriteLine("flush:{0}", size); //:debug:
                FlushBuffer(size);

                while (buffer.Count >= bufferThresholdSize)
                {
                    //Console.WriteLine("continue flush :{0}", buffer.Count); //:debug:
                    FlushBuffer(buffer.Count);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
                Thread.VolatileWrite(ref status, statusWaiting);
            }
        }


        protected T[] Dequeue(int batchSize)
        {
            if (batchSize <= 0)
            {
                return null;
            }

            T[] data = null;
            lock (lockQuene)
            {
                if (buffer.Count > 0)
                {
                    int size = buffer.Count < batchSize ? buffer.Count : batchSize;
                    data = new T[size];
                    for (var i = 0; i < size; i++)
                    {
                        data[i] = buffer.Dequeue();
                    }
                }
            }

            return data;
        }

        protected void FlushBuffer(int batchSize)
        {
            var data = Dequeue(batchSize);

            if (data == null || data.Length == 0)
            {
                return;
            }

            //Console.WriteLine("FlushBuffer:{0}", data.Length); //:debug:
            int start = 0;
            int end = 0;
            int last = data.Length - 1;

            try
            {
                for (start = 0; start <= last; )
                {
                    end = start + tableMaxSize - 1 > last ? last : start + tableMaxSize - 1;
                    DataTable table = dataTableSchema.Clone();
                    table.BeginLoadData();
                    AddRowValues(table, data, start, end);
                    table.EndLoadData();
                    WriteToDb(table);

                    start = end + 1;
                    end = start;
                }
            }
            catch (Exception ex)
            {
                Dump(data, start, last);
                Log(ex.ToString());
            }
            finally
            {
                maxBufferSizeSemaphore.Release(data.Length);
            }
        }

        protected void WriteToDb(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return;
            }

            int batchSize = 500; //config
            int batchTimeout = 60 * 5;

            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var tx = cn.BeginTransaction();

                try
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cn, SqlBulkCopyOptions.Default, tx))
                    {
                        bulkCopy.BatchSize = batchSize;
                        bulkCopy.BulkCopyTimeout = batchTimeout;
                        bulkCopy.DestinationTableName = TableName;
                        bulkCopy.WriteToServer(table);
                        tx.Commit();
                    }

                    if (IsDebugEnable)
                    {
                        Log(string.Concat("write database:", table.Rows.Count));
                    }
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    Log(ex.ToString());
                    throw;
                }
            }

        }

        protected abstract DataTable BuildDataSchema();

        protected abstract void AddRowValues(DataTable table, T[] data, int start, int end);

        #endregion

        #region dump

        protected object lockDump = new object();
        protected byte[] lineBreak = Encoding.UTF8.GetBytes("\r\n");

        protected void Dump(T[] data, int start, int end)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }
            string fileName = GetDumpFileName();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }


            lock (lockDump)
            {
                try
                {
                    string basePath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
                    if (string.IsNullOrEmpty(basePath))
                    {
                        basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    }
                    string path = Path.Combine(basePath, fileName);
                    using (var stream = File.Open(path, FileMode.Append))
                    {
                        for (var i = start; i < data.Length; i++)
                        {
                            if (i > end)
                            {
                                break;
                            }
                            string text = DumpToText(data[i]);
                            if (string.IsNullOrEmpty(text))
                            {
                                continue;
                            }
                            byte[] buffer = Encoding.UTF8.GetBytes(text);
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Write(lineBreak, 0, lineBreak.Length);
                        }
                        stream.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }

        protected abstract string DumpToText(T item);

        protected virtual string GetDumpFileName()
        {
            return string.Concat("dump_", typeof(T).Name, "_", DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".txt");
        }

        #endregion

        #region Timer handle
        protected int timerInterval = 30 * 1 * 1000; //ms
        protected Timer timer = null;

        private void CreateTimer()
        {
            TimerCallback callback = new TimerCallback(TimerTick);
            int interval = timerInterval;
            timer = new Timer(callback, null, interval, interval);
        }

        protected void DestroyTimer()
        {
            if (timer == null)
            {
                return;
            }
            timer.Dispose();
            timer = null;
        }

        private void TimerTick(Object state)
        {
            //Console.WriteLine("timer tick :{0}", DateTime.Now); //:debug:
            FlushAll();
        }

        #endregion

        #region IDisposable
        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    DestroyTimer();
                    FlushAll();
                    //AppDomain.CurrentDomain.ProcessExit -= ProcessExitHandler;
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine(ex.ToString());
                }

                disposed = true;
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~BuckInsert()
        {
            Dispose(false);
        }
        #endregion
    }


}
