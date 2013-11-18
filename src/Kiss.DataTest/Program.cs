using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using Kiss.Data;
using Kiss.DataTest.Expression;
using Kiss.DataTest.Driver;

namespace Kiss.DataTest
{
    class Program
    {
        static void WriteTrace(TraceData d)
        {
            Console.WriteLine(d);
        }

        static void Main(string[] args)
        {
            
            RunAll();
            Console.WriteLine("press ENTER to exit");
            Console.ReadLine();
        }

        static void BenchAll()
        {
            int count = 1000 * 1000;
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                BenchExpression b = new BenchExpression();
                b.Bench(writer, count);
                writer.Flush();
                string log = sb.ToString();
                Console.WriteLine(log);
            }
        }

        static void RunTest(object obj)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                AUtils.RunAllMethods(writer, obj);
                writer.Flush();
                string log = sb.ToString();
                Console.WriteLine(log);
            }
        }

        static void RunAll()
        {
            StringBuilder sb = new StringBuilder();
            using(StringWriter writer = new StringWriter(sb))
            {
                AUtils.RunAllTypes(writer);
                writer.Flush();
                string log = sb.ToString();
                Console.WriteLine(log);
            }
        }

    }
}
