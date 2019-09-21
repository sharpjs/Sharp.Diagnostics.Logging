/*
    Copyright (C) 2019 Jeffrey Sharp

    Permission to use, copy, modify, and distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

SET NOCOUNT ON;

PRINT N'= opts: database';
GO

ALTER DATABASE CURRENT SET
    COMPATIBILITY_LEVEL =         140 -- until SQL 2019 is generally available
  , ANSI_NULL_DEFAULT             ON
  , ANSI_NULLS                    ON
  , ANSI_PADDING                  ON
  , ANSI_WARNINGS                 ON
  , QUOTED_IDENTIFIER             ON
  , ARITHABORT                    ON
  , NUMERIC_ROUNDABORT            OFF
  , CONCAT_NULL_YIELDS_NULL       ON
  , RECURSIVE_TRIGGERS            ON
  , CURSOR_DEFAULT                LOCAL
  , CURSOR_CLOSE_ON_COMMIT        OFF
  , PARAMETERIZATION              SIMPLE
  , DATE_CORRELATION_OPTIMIZATION ON
  , AUTO_CLOSE                    OFF
  , AUTO_SHRINK                   OFF
  , AUTO_CREATE_STATISTICS        ON
  , AUTO_UPDATE_STATISTICS        ON
  , AUTO_UPDATE_STATISTICS_ASYNC  ON
  , DELAYED_DURABILITY =          DISABLED
    WITH ROLLBACK IMMEDIATE
;

IF CONVERT(sysname, SERVERPROPERTY('Edition')) = N'SQL Azure'
EXEC('
    -- Options specific to Azure SQL Database

    ALTER DATABASE CURRENT SET
        COMPATIBILITY_LEVEL = 150
        WITH ROLLBACK IMMEDIATE
    ;

    ALTER DATABASE CURRENT
        COLLATE Latin1_General_100_CI_AS_SC_UTF8
    ;
');
ELSE
EXEC('
    -- Options specific to SQL Server

    ALTER DATABASE CURRENT
        COLLATE Latin1_General_100_CI_AS_SC
    ;

    ALTER DATABASE CURRENT SET
        CONTAINMENT = PARTIAL
        WITH ROLLBACK IMMEDIATE
    ;

    IF DB_NAME() LIKE N''%Test''
    ALTER DATABASE CURRENT
        SET RECOVERY SIMPLE
    ;
');

ALTER DATABASE CURRENT SET
    ALLOW_SNAPSHOT_ISOLATION ON
;

ALTER DATABASE CURRENT SET
    READ_COMMITTED_SNAPSHOT ON
    WITH ROLLBACK IMMEDIATE
;

GO
PRINT N'+ role: Writer';
GO

CREATE ROLE Writer AUTHORIZATION dbo;

IF DB_NAME() LIKE N'%Test'
BEGIN
    CREATE USER Tests WITH
        PASSWORD       = N'2GUaRTafGLN7fnib'
      , DEFAULT_SCHEMA = dbo
    ;

    ALTER ROLE Writer ADD MEMBER Tests;
END;

GO
PRINT N'+ tbl:  dbo.Log';
GO

CREATE TABLE dbo.Log
(
    Id              int IDENTITY    NOT NULL
  , Application     varchar (32)    NOT NULL
  , Environment     varchar (32)    NOT NULL
  , Component       varchar (32)    NOT NULL

  , CONSTRAINT Log_PK
        PRIMARY KEY (Id)

  , CONSTRAINT Log_UQ_Application_Environment_Component
        UNIQUE (Application, Environment, Component)
);

GO
PRINT N'+ tbl:  dbo.LogEntryType';
GO

CREATE TABLE dbo.LogEntryType
(
    Code            char    (  1)   NOT NULL
  , Name            varchar ( 20)   NOT NULL
  , Description     varchar (100)   NOT NULL

  , CONSTRAINT LogEntryType_PK
        PRIMARY KEY (Code)

  , CONSTRAINT LogEntryType_UQ_Name
        UNIQUE (Name)
);

INSERT dbo.LogEntryType
VALUES
    ('C', 'Critical',    'Fatal error or application crash' )
  , ('E', 'Error',       'Recoverable error'                )
  , ('W', 'Warning',     'Noncritical problem'              )
  , ('I', 'Information', 'Informational message'            )
  , ('V', 'Verbose',     'Debugging information'            )
  , ('<', 'Start',       'Starting of a logical operation'  )
  , ('>', 'Stop',        'Stopping of a logical operation'  )
  , ('-', 'Suspend',     'Suspension of a logical operation')
  , ('+', 'Resume',      'Resumption of a logical operation')
  , ('=', 'Transfer',    'Change of correlation identity'   )
;

GO
PRINT N'+ tbl:  dbo.LogEntry';
GO

CREATE TABLE dbo.LogEntry
(
    Id          bigint IDENTITY     NOT NULL

  , Date        datetime2(3)        NOT NULL
        CONSTRAINT LogEntry_DF_Date
            DEFAULT SYSUTCDATETIME()

  , TypeCode    char(1)             NOT NULL
        CONSTRAINT LogEntry_FK_Type
            REFERENCES dbo.LogEntryType (Code)

  , LogId       int                 NOT NULL
        CONSTRAINT LogEntry_FK_Log
            REFERENCES dbo.Log (Id)

  , Machine     varchar(128)        NOT NULL -- Name of machine where entry originated
  , Source      varchar(128)            NULL -- .NET trace source name
  , ProcessId   int                     NULL -- Id of process within OS
  , ThreadId    int                     NULL -- Id of thread  within OS/process
  , ActivityId  uniqueidentifier        NULL -- Id of logical activity within app
  , MessageId   int                 NOT NULL -- Id of kind of message
  , Message     varchar(1024)           NULL -- Entry data, serialized to string

  , CONSTRAINT LogEntry_PK
        PRIMARY KEY NONCLUSTERED (Id)

  , CONSTRAINT LogEntry_UQ_Date_Id
        UNIQUE CLUSTERED (Date, Id)
);

CREATE INDEX LogEntry_IX_LogId
    ON dbo.LogEntry (LogId)
    INCLUDE (Machine, ProcessId, ThreadId, TypeCode)
    WITH (
        ONLINE           = ON 
      , DATA_COMPRESSION = PAGE
    )
;

GO
PRINT N'+ tbl:  dbo.LogDataType';
GO

CREATE TABLE dbo.LogDataType
(
    Code        char    (  1) NOT NULL
  , Name        varchar ( 20) NOT NULL
  , Description varchar (100) NOT NULL

  , CONSTRAINT LogDataType_PK
        PRIMARY KEY (Code)

  , CONSTRAINT LogDataType_UQ_Name
        UNIQUE (Name)
);

INSERT dbo.LogDataType
VALUES
    ('C', 'Call Stack',       'Call stack')
  , ('L', 'Logical Op Stack', 'Logical operation stack')
  , ('T', 'Text',             'Miscellaneous text')
  , ('J', 'JSON',             'Data in JSON format')
;

GO
PRINT N'+ tbl:  dbo.LogData';
GO

CREATE TABLE dbo.LogData
(
    EntryId     bigint              NOT NULL
        CONSTRAINT LogData_FK_Entry
            REFERENCES dbo.LogEntry (Id)
            ON DELETE CASCADE

  , TypeCode    char(1)             NOT NULL
        CONSTRAINT LogData_FK_Type
            REFERENCES dbo.LogDataType (Code)
            ON DELETE CASCADE

  , Data        varchar(max)        NOT NULL

  , CONSTRAINT LogData_PK
        PRIMARY KEY (EntryId, TypeCode)
);

GO
PRINT N'+ type: dbo.LogEntryRow';
GO

CREATE TYPE dbo.LogEntryRow AS TABLE
(
    Id          int                 NOT NULL
  , Date        datetime2(3)        NOT NULL
  , TypeCode    char(1)             NOT NULL
  , Application varchar( 32)        NOT NULL
  , Environment varchar( 32)        NOT NULL
  , Component   varchar( 32)        NOT NULL
  , Machine     varchar(128)        NOT NULL
  , Source      varchar(128)        NOT NULL
  , ProcessId   int                     NULL -- Unique id of the process on the OS
  , ThreadId    int                     NULL -- Unique id of the thread in the process
  , ActivityId  uniqueidentifier        NULL -- Unique id of the logical activity within app
  , MessageId   int                 NOT NULL -- Unique id of the type of event, or 0
  , Message     varchar(1024)       NOT NULL -- Entry data serialized to string
);

GO
PRINT N'+ type: dbo.LogDataRow';
GO

CREATE TYPE dbo.LogDataRow AS TABLE
(
    EntryId     int                 NOT NULL
  , TypeCode    char(1)             NOT NULL
  , Data        varchar(max)        NOT NULL
);

GO
PRINT N'+ proc: dbo.WriteLog';
GO

CREATE PROCEDURE dbo.WriteLog
    @EntryRows dbo.LogEntryRow READONLY
  , @DataRows  dbo.LogDataRow  READONLY
AS BEGIN
    -- Log
    WITH _Log AS
    (
        SELECT DISTINCT Application, Environment, Component
        FROM @EntryRows
    )
    MERGE dbo.Log s
    USING _Log x
        ON  s.Application = x.Application
        AND s.Environment = x.Environment
        AND s.Component   = x.Component
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Application, Environment, Component)
        VALUES (Application, Environment, Component)
    ;

    -- LogEntry
    DECLARE @Ids TABLE
    (
        OldId   int     NOT NULL PRIMARY KEY
      , NewId   bigint  NOT NULL
    );
    WITH _Entry AS
    (
        SELECT
            e.Id, e.Date, e.TypeCode, LogId=l.Id, e.Machine, e.Source
          , e.ProcessId, e.ThreadId, e.ActivityId, e.MessageId, e.Message
        FROM @EntryRows e
        LEFT JOIN dbo.Log l
            ON  l.Application = e.Application
            AND l.Environment = e.Environment
            AND l.Component   = e.Component
    )
    MERGE dbo.LogEntry
    USING _Entry e
        ON 0=1 -- Always insert
    WHEN NOT MATCHED BY TARGET THEN
        INSERT 
            ( Date, TypeCode, LogId, Machine, Source
            , ProcessId, ThreadId, ActivityId, MessageId, Message )
        VALUES
            ( Date, TypeCode, LogId, Machine, Source
            , ProcessId, ThreadId, ActivityId, MessageId, Message )
    OUTPUT
        e.Id, inserted.Id INTO @Ids
    ;

    -- LogData
    INSERT dbo.LogData (EntryId, TypeCode, Data)
    SELECT EntryId = x.NewId, d.TypeCode, d.Data
    FROM @DataRows d
    LEFT JOIN @Ids x ON x.OldId = d.EntryId
    ;
END;
GO

GRANT REFERENCES, EXECUTE ON TYPE::dbo.LogEntryRow TO Writer;
GRANT REFERENCES, EXECUTE ON TYPE::dbo.LogDataRow  TO Writer;
GRANT REFERENCES, EXECUTE ON dbo.WriteLog          TO Writer;
