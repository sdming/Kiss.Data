
CREATE TABLE [dbo].[ttable](
	[pk] [bigint] IDENTITY(1,1) NOT NULL,
	[cbool] [bit] NULL,
	[cint] [int] NULL,
	[cfloat] [float] NULL,
	[cnumeric] [numeric](10, 4) NULL,
	[cstring] [nvarchar](100) NULL,
	[cdatetime] [datetime] NULL,
	[cguid] [uniqueidentifier] NULL,
	[cbytes] [binary](100) NULL,
 CONSTRAINT [PK_ttable] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)
)

GO

create procedure [usp_query](@cint int)
as
begin
        select * from ttable where cint > @cint;
end;
GO


create procedure [usp_exec](@cint int)
as
begin
         delete from ttable where cint = @cint;  
end;
GO


create procedure [usp_inout](@x int, @y int output, @sum int output)
as
begin
        set @sum = @x + @y;
        set @y = 2 * @y
end;
GO