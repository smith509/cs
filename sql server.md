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

