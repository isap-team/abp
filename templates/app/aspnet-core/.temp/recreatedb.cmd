@echo off
set databaseName=MyProjectName
set serverHost=localhost
set userName=postgres
set PGPASSWORD=postgres
set PgsqlBin=C:\Program Files\pgsql\bin
set psql=%PgsqlBin%\psql.exe
set createdb=%PgsqlBin%\createdb.exe
set dropdb=%PgsqlBin%\dropdb.exe
set pg_restore=%PgsqlBin%\pg_restore.exe

echo Disconnecting database users...
"%psql%" --quiet --host=%serverHost% --username=%userName% --dbname=%databaseName% ^
	--command="SELECT pg_terminate_backend(pid) FROM pg_catalog.pg_stat_activity WHERE datname = '%databaseName%' AND query NOT LIKE '%%pg_terminate_backend(pid)%%'"

echo Dropping database "%databaseName%"...
"%dropdb%" -e --username=%userName% --host=%serverHost% %databaseName%
echo Creating database "%databaseName%"...
"%createdb%" -e --username=%userName% --host=%serverHost% %databaseName%
echo Complete!
