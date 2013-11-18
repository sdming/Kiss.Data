using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using Kiss.Data.Expression;
using System.IO;

namespace Kiss.DataTest.Expression
{
    public class BenchExpression
    {
        public int DefaultCount = 1000 * 1000;

        public ISqlExpression Query()
        {
            var exp = new ExpressionData().BuildSimpileQuery();
            var driver = Kiss.Data.Driver.SqlDriverFactory.SqlClient();
            var command = driver.Compile(exp, null);
            var expected = @"
SELECT [cbool], [cint], [cfloat], [cdatetime], [cnumeric] AS [a_cnumeric], [cstring] AS [a_cstring], [cdatetime], [cguid] 
FROM [ttable]
WHERE
[cbool] = @P_0
AND
[cint] <= @P_1
AND
[cfloat] > @P_2
AND
[cnumeric] >= @P_3
AND
[cstring] LIKE @P_4
AND
[cdatetime] > @P_5
AND
[cbytes] IS NULL 
ORDER BY [cint] ASC, [cfloat] ASC, [cnumeric] DESC, [cstring] DESC  ;
";
            //Console.WriteLine(command.CommandText);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(command.CommandText));

            return exp;
        }

        public void Bench(TextWriter writer, int count)
        {
            Dictionary<string, ISqlExpression> data =  new Dictionary<string,ISqlExpression>();
            data["query"] = Query();
            data["insert"] = Insert();
            data["update"] = Update();

            var driver = Kiss.Data.Driver.SqlDriverFactory.SqlClient();
            foreach (var item in data)
            {
                //Console.WriteLine("bench: {0}", item.Key);
                writer.WriteLine("\r\n{0} start: {1}", item.Key, DateTime.Now.ToString("hh:mm:ss:fff"));
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (var i = 0; i < count; i++)
                {
                    var c = driver.Compile(item.Value, null);
                }
                sw.Stop();
                writer.WriteLine("{0} end: count:{1}, ms:{2}, avg:{3}, {4}", 
                    item.Key, count, sw.ElapsedMilliseconds, 
                    sw.ElapsedMilliseconds * 1000.0 / count,
                    DateTime.Now.ToString("hh:mm:ss:fff"));
            }
        }

        public ISqlExpression Insert()
        {
            var exp = new ExpressionData().BuildInsert();
            var driver = Kiss.Data.Driver.SqlDriverFactory.SqlClient();
            var command = driver.Compile(exp, null);
            var expected = @"
INSERT INTO [ttable]([cbool], [cint], [cfloat], [cnumeric], [cstring], [cdatetime], [cguid])
VALUES(@P_0, @P_1, @P_2, @P_3, @P_4, @P_5, @P_6) ;
";
            //Console.WriteLine(command.CommandText);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(command.CommandText));

            return exp;

        }
    
        public ISqlExpression Update()
        {
            var exp = new ExpressionData().BuildUpdate();
            var driver = Kiss.Data.Driver.SqlDriverFactory.SqlClient();
            var command = driver.Compile(exp, null);
            var expected = @"
UPDATE TOP( 101 ) [ttable] SET
[cbool] = @P_0, [cint] = @P_1, [cfloat] = @P_2, [cnumeric] = @P_3, [cstring] = @P_4, [cdatetime] = @P_5, [cguid] = @P_6
WHERE
[cint] = @P_7  ;
";
            //Console.WriteLine(command.CommandText);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(command.CommandText));

            return exp;

        }

    }
}
