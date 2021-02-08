@echo off
set ProjectNamespace=MyCompanyName.MyProjectName
set MigrationName=%1
if '%MigrationName%' EQU '' set MigrationName=Initial

pushd .\src\%ProjectNamespace%.DbMigrator

dotnet ef migrations add "Audit_%MigrationName%" ^
	--context AuditMigrationsDbContext ^
	--output-dir Migrations/AuditDb ^
	--startup-project .\%ProjectNamespace%.DbMigrator.csproj ^
	--project ..\%ProjectNamespace%.EntityFrameworkCore.DbMigrations\%ProjectNamespace%.EntityFrameworkCore.DbMigrations.csproj

popd
