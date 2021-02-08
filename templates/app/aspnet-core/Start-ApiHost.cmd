@echo off
set ProjectNamespace=MyCompanyName.MyProjectName
set ASPNETCORE_ENVIRONMENT=Development

pushd .\src\%ProjectNamespace%.HttpApi.Host

start .\bin\Debug\netcoreapp3.1\%ProjectNamespace%.HttpApi.Host.exe

popd
