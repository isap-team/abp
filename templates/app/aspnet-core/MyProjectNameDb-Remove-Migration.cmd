@echo off
set ProjectNamespace=MyCompanyName.MyProjectName

pushd .\src\%ProjectNamespace%.DbMigrator

dotnet ef migrations remove --force ^
	--context MyProjectNameMigrationsDbContext ^
	--startup-project .\%ProjectNamespace%.DbMigrator.csproj ^
	--project ..\%ProjectNamespace%.EntityFrameworkCore.DbMigrations\%ProjectNamespace%.EntityFrameworkCore.DbMigrations.csproj

popd
