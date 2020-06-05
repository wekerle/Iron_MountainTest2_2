if not exists(select * from sys.databases where name = 'Test2_2DB')
    create database Test2_2DB
GO
use Test2_2DB;
GO
CREATE TABLE MetaInfos (
    ID int NOT NULL PRIMARY KEY,
    CreationDate datetime NOT NULL,
    ImageRoute varchar(255) NOT NULL,
	ImageExists bit not null
);
GO

CREATE TYPE [dbo].MetaInfos AS TABLE(  
    ID int NULL,  
    CreationDate DATETIME NULL,  
    ImageRoute varchar(250) NULL,  
    ImageExists bit NULL    
) 
GO
CREATE PROCEDURE dbo.spInsertMetainfo(@tableMetaInfos MetaInfos READONLY)  
AS  
BEGIN  
   INSERT INTO MetaInfos SELECT ID,CreationDate,ImageRoute,ImageExists FROM @tableMetaInfos  
END 
GO
