using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Kiss.Data;

namespace Kiss.DataTest
{
    [TestFixture]    
    public class TestTemplate
    {

        public void TestCompile()
        {
            string sql = @"test {{p1}} and2 {{p2 out string}} and3 {{p3 inout date}} and4 {{p4 refcursor}}";
            TextTemplate text = TextTemplate.Compile(sql);
            Assert.AreEqual("test {{p1}} and2 {{p2}} and3 {{p3}} and4 {{p4}}", text.Text);
            Assert.AreEqual("Name:p1", text.Parameters[0].ToString());
            Assert.AreEqual("Name:p2;DbType:String;Direction:Output", text.Parameters[1].ToString());
            Assert.AreEqual("Name:p3;DbType:Date;Direction:InputOutput", text.Parameters[2].ToString());
            Assert.AreEqual("Name:p4;ProviderDbType:refcursor", text.Parameters[3].ToString());

            sql = @"test p1 p2";
            text = TextTemplate.Compile(sql);
            Assert.AreEqual("test p1 p2", text.Text);
            Assert.AreEqual(0, text.Parameters.Count);

            sql = @"test {{p1}} test";
            text = TextTemplate.Compile(sql);
            Assert.AreEqual("test {{p1}} test", text.Text);
            Assert.AreEqual("Name:p1", text.Parameters[0].ToString());

        }
    }
}
