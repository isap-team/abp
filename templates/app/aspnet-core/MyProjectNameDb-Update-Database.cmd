@echo off
set ProjectNamespace=MyCompanyName.MyProjectName

pushd .\src\%ProjectNamespace%.DbMigrator\bin\Debug\netcoreapp3.1

%ProjectNamespace%.DbMigrator.exe -p=PostgreSql -e=Development

popd
