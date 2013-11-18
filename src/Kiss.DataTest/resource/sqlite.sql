CREATE TABLE ttable (
        pk INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
        cbool BOOLEAN, 
        cint INTEGER, 
        cfloat REAL, 
        cnumeric NUMERIC (10, 4), 
        cstring TEXT, 
        cdatetime DATETIME, 
		cguid UNIQUEIDENTIFIER, 
        cbytes BLOB
);

