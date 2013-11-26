kiss.data的简单记录
==

[github地址](https://github.com/sdming/Kiss.Data)  

kiss.data是golang的数据库访问类库[kdb](https://github.com/sdming/kdb)的c#版本， 最初是先有c#版本的，后来根据golang的设计重构了。  

刚写完主干部分，还在测试中。     

特性
==

* 支持主流数据库  
* 支持常见ORM操作  
* 智能数据转换  
* ...     

基本类型
==

`DbContent`类

封装对db的一些基本操作。  

`IDataObjectAdapter`接口

对象字段访问的接口， 包含四个方法

```
public interface IDataObjectAdapter
{
    void Set(string field, object value); //设置字段的值
    object Get(string field); //获取字段的值
    IEnumerable<string> Fields(); //获取所有字段
    bool Contains(string field); //判断字段是否存在, 可能多余，因为根据Fields的结果也可以判断字段是否存在
}

```

`ISqlExpression`接口

Sql表达式的接口，定义如下     

```

public interface ISqlExpression
{
    NodeType NodeType();
}

```

环境
==

假设数据库连接字符串配置如下

```
<add name="mssql" connectionString="Server=localhost;Database=kiss;Trusted_Connection=True;" providerName="System.Data.SqlClient"/>

```
表结构如下  

```
CREATE TABLE [ttable](
	[pk] 		[bigint] IDENTITY(1,1) NOT NULL,
	[cbool] 	[bit] NULL,
	[cint] 		[int] NULL,
	[cfloat] 	[float] NULL,
	[cnumeric] 	[numeric](10, 4) NULL,
	[cstring] 	[nvarchar](100) NULL,
	[cdatetime] [datetime] NULL,
	[cguid] 	[uniqueidentifier] NULL,
	[cbytes] 	[binary](100) NULL,

	CONSTRAINT [PK_ttable] PRIMARY KEY CLUSTERED 
	(
		[pk] ASC
	)
)

```

代码中的对象定义如下

```

[DbTable(Name = "ttable")]
public class CEntity
{
    [DbColumn(IsKey = true, UpdateAble = false, InsertAble = false)]
    public int PK { get; set; }

    public bool CBool { get; set; }

    public int CInt { get; set; }

    public float CFloat { get; set; }

    [DbColumn(Name = "CNumeric")]
    public decimal ColNumeric { get; set; }

    public string CString { get; set; }

    public DateTime CDateTime;

    public Guid CGuid;

}
```        

直接执行sql 脚本
==

用IDataObjectAdapter传参数

```
using (DbContent db = new DbContent("mssql"))
{
    var data = Kiss.Core.Adapter.Dictionary();
    data.Set("cint", 101);
    data.Set("pk", 11606);
    db.TextNonQuery("update TTABLE set cint = @cint where pk = @pk", data);
}

```

用params object[]传参数

```
using (DbContent db = new DbContent("mssql"))
{
    var i = db.TextNonQuery("update TTABLE set cint = @cint where pk = @pk", 102, 11606);
}

```

创建Text对象

```
using (DbContent db = new DbContent("mssql"))
{
    var exp = new Kiss.Data.Expression.Text("update TTABLE set cint = @cint where pk = @pk")
        .Set("cint", 103)
        .Set("pk", 11606);

    db.ExecuteNonQuery(exp);
}
```


执行存储过程
==

测试存储过程定义如下

```

create procedure [usp_query](@cint int)
as
begin
        select * from ttable where cint > @cint;
end;

create procedure [usp_exec](@cint int)
as
begin
         delete from ttable where cint = @cint;  
end;

create procedure [usp_inout](@x int, @y int output, @sum int output)
as
begin
        set @sum = @x + @y;
        set @y = 2 * @y
end;

```

用IDataObjectAdapter传参数

```
using (DbContent db = new DbContent("mssql"))
{
    Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    data["cint"] = 101;
    var table = db.ProcedureReader("usp_query", Kiss.Core.Adapter.Dictionary(data)).ToTable();
}          
```

用params object[]传参数

```
using (DbContent db = new DbContent("mssql"))
{
    db.ProcedureNonQuery("usp_exec", 11606);
}
```

返回output参数

```
using (DbContent db = new DbContent("mssql"))
{
    var data = Kiss.Core.Adapter.Dictionary();
    data.Set("x", 2);
    data.Set("y", 7);
    IExecuteResult r;
    db.ProcedureNonQuery("usp_inout", data, out r);
    var output = r.Output();
    Console.WriteLine("y:{0},sum:{1}", output["y"], output["sum"]);
}
```

通过Procedure对象执行

```
using (DbContent db = new DbContent("mssql"))
{
    var exp = new Kiss.Data.Expression.Procedure("usp_exec")
        .Set("cint", 64);

    db.ExecuteNonQuery(exp);               
}
```

如果定义了存储过程对应的接口，可以通过动态代理执行，比如接口定义如下  


```
public interface IPorxyTest
{
    IDataReader usp_query(int cint);

    [DbProcedure(Name = "usp_exec")]
    IDataReader Exec([DbParameter(Name = "cint")] int c);

    IDataReader usp_inout(int x, ref int y, out int sum);
}

```

可以这么执行
```
using (DbContent db = new DbContent("mssql"))
{
    IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
    var reader = proxy.usp_query(101);
    reader.Dispose();
}

using (DbContent db = new DbContent("mssql"))
{
    IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
    var reader = proxy.Exec(101);
    Console.WriteLine(reader.RecordsAffected);
    reader.Dispose();
}

using (DbContent db = new DbContent("mssql"))
{
    int x = 3;
    int y = 7;
    int sum;

    IPorxyTest proxy = FunctionProxy<IPorxyTest>.Create(db);
    proxy.usp_inout(x, ref y, out sum);
    Console.WriteLine("y:{0},sum:{1}", y, sum);
} 

```

Insert
==

通过insert对象执行

```
using (DbContent db = new DbContent("mssql"))
{
    var insert = new Kiss.Data.Expression.Insert("ttable")
        .Set("cbool", true)
        .Set("cint", 42)
        .Set("cfloat", 3.14)
        .Set("cnumeric", 1.1)
        .Set("cstring", "string")
        .Set("cdatetime", "2004-07-24");

    db.ExecuteNonQuery(insert);
}

```

通过IDataObjectAdapter执行

```
using (DbContent db = new DbContent("mssql"))
{
    var data = Kiss.Core.Adapter.Dictionary();
    data.Set("A_cbool", true);
    data.Set("A_cint", 42);
    data.Set("A_cfloat", 3.14);
    data.Set("A_cnumeric", 1.1);
    data.Set("A_cstring", "string");
    data.Set("A_cdatetime", "2004-07-24");
    
    db.Table("ttable").Insert(data, (x)=> "A_" + x, null, new string[]{"A_cint"});
}

```

通过ActiveEntity执行


```
using (DbContent db = new DbContent("mssql"))
{
    var data = CEntity.NewEntity();
    ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
    var pk = ae.Add(data);
    Console.WriteLine(pk);
}
```

Update
==

通过Update对象执行

```
using (DbContent db = new DbContent("mssql"))
{
    var update = new Kiss.Data.Expression.Update("ttable");
    update
        .Set("cstring", "new string")
        .Set("cdatetime", DateTime.Now)
    .Limit(10)
    .Where
        .EqualsTo("cint", 101);

    db.ExecuteNonQuery(update);
}
```

通过IDataObjectAdapter执行

```
using (DbContent db = new DbContent("mssql"))
{
    var data = Kiss.Core.Adapter.Dictionary();
    data.Set("cint", 420);
    data.Set("cfloat", 3.141);
    data.Set("cnumeric", 1.12);

    db.Table("ttable").Update(data, "cint", 101);
}

```

更新字段

```

using (DbContent db = new DbContent("mssql"))
{
    var where = new Where()
        .EqualsTo("cint", 101);

    db.Table("ttable").UpdateColumn("cstring", "a string", where);
}

```
通过主键更新

```

using (DbContent db = new DbContent("mssql"))
{
    CEntity entity = CEntity.NewEntity();
    ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
    ae.UpdateByKey(entity, 11606);
}

```

根据字段更新

```
using (DbContent db = new DbContent("mssql"))
{
    CEntity entity = CEntity.NewEntity();
    ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
    ae.UpdateByFields(entity, (x) => x.CInt, 101);
}

```

根据lamda条件更新

```

using (DbContent db = new DbContent("mssql"))
{
    var data = new Dictionary<string, object>();
    data["cstring"] = "some string";
    data["cfloat"] = 3.14 * 3.14;
    ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
    ae.UpdateFields((x) => x.CInt > 101 && x.CInt < 202, data);
}

```

Query
==

通过Query对象执行  


```
using (DbContent db = new DbContent("mssql"))
{
    var query= new Data.Expression.Query("ttable");
    query.Where
        .EqualsTo("cint" , 10100)
        .EqualsTo("cint", 20200);

    var reader = db.ExecuteReader(query);
    reader.Dispose();
}

```

根据列查询  

```
using (DbContent db = new DbContent("mssql"))
{
    var reader = db.Table("ttable").Read("cint", 10100, "cint", 20200);
    reader.Dispose();
}

```            

读取某个字段的值


```

using (DbContent db = new DbContent("mssql"))
{
    var i = db.Table("ttable").ReadCell("cint", "pk", "11606");
}

```

查询某一列

```

using (DbContent db = new DbContent("mssql"))
{
    var list = db.Table("ttable").ReadColumn<string>("cstring", false, "cint", SqlOperator.GreaterThan, 202);
}

```

通过ActiveEntity执行


```

using (DbContent db = new DbContent("mssql"))
{                
    ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
    var list = ae.QueryByFields((x)=>x.PK, 11606);
}

```

通过lamda执行

```
using (DbContent db = new DbContent("mssql"))
{
    ActiveEntity<CEntity> ae = new ActiveEntity<CEntity>(db);
    var list = ae.Query((x) => x.CInt > 101 && x.CInt < 20200);
}

```

注意
==

要确保关闭DbConnection，特别是返回IDataReader时(此时DbContent不会自动关闭DbConnection)  


FAQ
==

默认实体类型名字段名要和表名列名一致:
故意这么设计的，取不一样的名字有什么好处？ 当然也可以自定义映射关系  

不追求性能: 
不追求某一项的性能指标，加班到半夜和凌晨起来改bug和性能基本上没什么关系 

不支持多表查询:
故意这么设计的， 如果有复杂的多表关联还是改掉为宜，或者用view  

不支持缓存:
缓存不应该在数据层处理  

不支持代码生成:
如果可以代码生成，就基本上可以统一处理  




