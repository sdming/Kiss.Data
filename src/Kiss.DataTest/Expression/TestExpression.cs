using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.DataTest;

using NUnit.Framework;

namespace Kiss.DataTest.Expression
{
    [TestFixture]
    public class TestExpression
    {
        [Test]
        public void TestQuery()
        {
            var query = new ExpressionData().BuildQuery();
            var actual = query.ToString();

            var expected = @"
SELECT distinct cbool, cint, cfloat, cdatetime, cnumeric AS a_cnumeric, cstring AS a_cstring, COUNT(cint)  AS avg_cint, COUNT(cint)  AS count_cint, SUM(cint)  AS sum_cint, MIN(cint)  AS min_cint, MAX(cint)  AS max_cint, sql(cint - 1) AS exp_cint 
FROM ttable 
WHERE 
cbool = True
AND
cbool <> False
AND
cstring < LessThan
AND
cstring <= LessOrEquals
AND
cstring > GreaterThan
AND
cstring >= GreaterOrEquals
AND
cstring = Equals
AND
cstring <> NotEquals
AND
cstring IN [a, b, c]
AND
cstring NOT IN [h, i, j]
AND
cstring LIKE %like%
AND
cstring NOT LIKE %NotLike%
AND
cint < 100
AND
cint <= 101
AND
cint > 200
AND
cint >= 201
AND
cint = 300
AND
cint <> 301
AND
cint IN [0, 1, 2, 3, 4]
AND
cint NOT IN [5, 6, 7, 8, 9]
AND
cfloat < 1.01
AND
cfloat <= 1.02
AND
cfloat > 2.01
AND
cfloat >= 2.02
AND
cfloat = 3.01
AND
cfloat <> 3.02
AND
cfloat IN [10.01, 11.01, 12.01, 13.01, 14.01]
AND
cfloat NOT IN [15.01, 16.01, 17.01, 18.01, 19.01]
AND
cnumeric < 1.1
AND
cnumeric <= 1.2
AND
cnumeric > 2.1
AND
cnumeric >= 2.2
AND
cnumeric = 3.1
AND
cnumeric <> 3.2
AND
cnumeric IN [10.1, 11.1, 12.1, 13.1, 14.1]
AND
cnumeric NOT IN [15.1, 16.1, 17.1, 18.1, 19.1]
AND
cdatetime < 2001-01-01
AND
cdatetime <= 2001-01-02
AND
cdatetime > 2001-02-01
AND
cdatetime >= 2001-02-02
AND
cdatetime = 2001-03-01 00:00:00
AND
cdatetime <> 2001-03-02 00:00:00
AND
cdatetime IN [2001-04-01 01:01:01, 2001-04-02 02:02:02]
AND
cdatetime NOT IN [2001-04-01 01:01:01, 2001-04-02 02:02:02]
AND
cguid = 550e8400-e29b-41d4-a716-446655440000
AND
cguid <> 550e8400-e29b-41d4-a716-446655440000
AND
(
	(
		cbytes IS NULL
		OR
		cbytes IS NOT NULL
	)
	OR
	(
		sql( 1!=2 )
		AND
		EXISTS(sql(select count(*) from ttable where cint > 1))
		AND
		NOT EXISTS(sql(select count(*) from ttable where cint > 10000))
		AND
		cint IN sql(select cint from ttable)
		AND
		cint NOT IN sql(select cint from ttable)
	)
) 
GROUP BY cbool, cint, cnumeric, cstring, cfloat, cdatetime, sql(cint - 1) 
HAVING 
cstring LIKE %like%
AND
cint NOT IN [1, 2, 3, 4, 5]
AND
(
	cint < 12345
	OR
	cint >= 101
)
AND
cnumeric = 1.1
AND
cbool IS NOT NULL
AND
COUNT(cint)  < 201
AND
COUNT(cint)  > 301
AND
SUM(cint)  <> 401
AND
MIN(cint)  <= sql(501)
AND
MAX(cint)  >= 601 
ORDER BY cint ASC, cfloat ASC, cnumeric DESC, cstring DESC, cdatetime ASC 
LIMIT 3, 101 
";
            //Console.WriteLine(actual);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(actual));

        }

        [Test]
        public void TestInsert()
        {
            var insert = new ExpressionData().BuildInsert();
            var actual = insert.ToString();

            var expected = @"
insert ttable 
[
    cbool = True , 
    cint = 42 , 
    cfloat = 3.14 , 
    cnumeric = 1.1 , 
    cstring = string , 
    cdatetime = 2004-07-24 00:00:00 , 
    cguid = 550e8400-e29b-41d4-a716-446655440000 
]
";
            //Console.WriteLine(actual);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(actual));

        }

        [Test]
        public void TestUpdate()
        {
            var update = new ExpressionData().BuildUpdate();
            var actual = update.ToString();

            var expected = @"
UPDATE ttable SET 
[
	cbool = True , 
	cint = 42 , 
	cfloat = 3.14 , 
	cnumeric = 1.1 , 
	cstring = string , 
	cdatetime = 2004-07-24 00:00:00 , 
	cguid = 550e8400-e29b-41d4-a716-446655440000 
] 
WHERE 
cint = 101 
ORDER BY cint ASC 
LIMIT 101
";
            //Console.WriteLine(actual);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(actual));

        }

        [Test]
        public void TestDelete()
        {
            var delete = new ExpressionData().BuildDelete();
            var actual = delete.ToString();

            var expected = @"
DELETE FROM ttable 
WHERE 
cint = 101 
ORDER BY cint ASC 
LIMIT 101 
";
            //Console.WriteLine(actual);

            Assert.AreEqual(AUtils.FS(expected), AUtils.FS(actual));

        }
    }
}
