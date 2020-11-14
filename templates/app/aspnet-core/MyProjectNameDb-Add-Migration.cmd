@echo off
set ProjectNamespace=MyCompanyName.MyProjectName
set MigrationName=%1
if '%MigrationName%' EQU '' set MigrationName=Initial

pushd .\src\%ProjectNamespace%.DbMigrator

dotnet ef migrations add "MyProjectName_%MigrationName%" ^
	--context MyProjectNameMigrationsDbContext ^
	--output-dir Migrations/MyProjectNameDb ^
	--startup-project .\%ProjectNamespace%.DbMigrator.csproj ^
	--project ..\%ProjectNamespace%.EntityFrameworkCore.DbMigrations\%ProjectNamespace%.EntityFrameworkCore.DbMigrations.csproj

popd
