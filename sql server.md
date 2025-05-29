# Session Activity Overview

```sql
SELECT 
    s.session_id,
    s.login_name,
    s.database_id,
    d.name AS database_name,
    s.status,
    s.host_name,
    s.program_name,
    s.last_request_start_time
FROM 
    sys.dm_exec_sessions s
JOIN 
    sys.databases d ON s.database_id = d.database_id
WHERE 
    s.session_id > 50;  -- Exclude system sessions
```

# Currently Running Queries

```sql
SELECT 
    r.session_id,
    r.status,
    r.start_time,
    r.command,
    r.cpu_time,
    r.total_elapsed_time,
    r.reads,
    r.writes,
    r.logical_reads,
    r.blocking_session_id,
    t.text AS sql_text
FROM 
    sys.dm_exec_requests r
CROSS APPLY 
    sys.dm_exec_sql_text(r.sql_handle) AS t
WHERE 
    r.session_id > 50;  -- Exclude system sessions
```

# Blocking or Blocked Sessions

```sql
SELECT 
    blocking_session_id AS BlockingSessionID,
    session_id AS BlockedSessionID,
    wait_type,
    wait_time,
    wait_resource,
    (SELECT text FROM sys.dm_exec_sql_text(sql_handle)) AS BlockedSQLText
FROM 
    sys.dm_exec_requests
WHERE 
    blocking_session_id <> 0;
```

# Resource Usage by Session

```sql
SELECT 
    r.session_id,
    r.cpu_time,
    r.total_elapsed_time,
    r.reads,
    r.writes,
    r.logical_reads,
    t.text AS sql_text
FROM 
    sys.dm_exec_requests r
CROSS APPLY 
    sys.dm_exec_sql_text(r.sql_handle) AS t
WHERE 
    r.session_id > 50;  -- Exclude system sessions
```

# Long-Running Queries

This query retrieves queries that have been running for longer than a specified threshold (e.g., 5 seconds).

```sql
SELECT 
    r.session_id,
    r.status,
    r.start_time,
    r.total_elapsed_time,
    t.text AS sql_text
FROM 
    sys.dm_exec_requests r
CROSS APPLY 
    sys.dm_exec_sql_text(r.sql_handle) AS t
WHERE 
    r.total_elapsed_time > 5000  -- Time in milliseconds
    AND r.session_id > 50;  -- Exclude system sessions
```

# Top Resource-Consuming Queries

This query retrieves the top 10 queries based on total CPU time.

```sql
SELECT TOP 10
    t.text AS sql_text,
    r.cpu_time,
    r.total_elapsed_time,
    r.execution_count
FROM 
    sys.dm_exec_query_stats qs
CROSS APPLY 
    sys.dm_exec_sql_text(qs.sql_handle) AS t
JOIN 
    sys.dm_exec_requests r ON r.plan_handle = qs.plan_handle
ORDER BY 
    r.cpu_time DESC;
```

# All Tables and Their Statistics

```sql
SELECT 
    t.NAME AS TableName,
    s.Name AS SchemaName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 AS TotalSpaceKB,
    SUM(a.used_pages) * 8 AS UsedSpaceKB,
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 AS UnusedSpaceKB
FROM 
    sys.tables AS t
INNER JOIN 
    sys.schemas AS s ON t.schema_id = s.schema_id
INNER JOIN 
    sys.indexes AS i ON t.object_id = i.object_id
INNER JOIN 
    sys.partitions AS p ON i.object_id = p.object_id AND i.index_id = p.index_id
INNER JOIN 
    sys.allocation_units AS a ON p.partition_id = a.container_id
WHERE 
    i.index_id <= 1
GROUP BY 
    t.Name, s.Name, p.rows
ORDER BY 
    TotalSpaceKB DESC;
```

# Most Queried Tables

```sql
SELECT 
    t.name AS TableName,
    s.name AS SchemaName,
    qs.execution_count AS ExecutionCount,
    qs.total_worker_time AS TotalWorkerTime,
    qs.total_elapsed_time AS TotalElapsedTime,
    qs.total_logical_reads AS TotalLogicalReads,
    qs.total_logical_writes AS TotalLogicalWrites
FROM 
    sys.dm_exec_query_stats AS qs
CROSS APPLY 
    sys.dm_exec_sql_text(qs.sql_handle) AS st
JOIN 
    sys.objects AS t ON st.objectid = t.object_id
JOIN 
    sys.schemas AS s ON t.schema_id = s.schema_id
WHERE 
    t.type = 'U'  -- Only user tables
ORDER BY 
    qs.execution_count DESC;
```

# All Procs that Begin With "sp_"

```sql
SELECT 
    SCHEMA_NAME(schema_id) AS SchemaName,
    name AS ProcedureName
FROM 
    sys.procedures
WHERE 
    name LIKE 'sp_%'
ORDER BY 
    SchemaName, ProcedureName;
```
# Most Executed Procs

```sql
SELECT 
    o.name AS ProcedureName,
    s.name AS SchemaName,
    qs.execution_count AS ExecutionCount,
    qs.total_worker_time AS TotalWorkerTime,
    qs.total_elapsed_time AS TotalElapsedTime,
    qs.total_logical_reads AS TotalLogicalReads,
    qs.total_logical_writes AS TotalLogicalWrites
FROM 
    sys.dm_exec_query_stats AS qs
CROSS APPLY 
    sys.dm_exec_sql_text(qs.sql_handle) AS st
JOIN 
    sys.objects AS o ON st.objectid = o.object_id
JOIN 
    sys.schemas AS s ON o.schema_id = s.schema_id
WHERE 
    o.type = 'P'  -- Only stored procedures
ORDER BY 
    qs.execution_count DESC;
```

# All Funcs

```sql
SELECT 
    SCHEMA_NAME(schema_id) AS SchemaName,
    name AS FunctionName,
    type_desc AS FunctionType
FROM 
    sys.objects
WHERE 
    type IN ('FN', 'IF', 'TF')  -- FN: Scalar Function, IF: Inline Table-valued Function, TF: Table-valued Function
ORDER BY 
    SchemaName, FunctionName;
```
