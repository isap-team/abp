@echo off
set ProjectNamespace=MyCompanyName.MyProjectName
set ASPNETCORE_ENVIRONMENT=Development

pushd .\src\%ProjectNamespace%.IdentityServer

start .\bin\Debug\netcoreapp3.1\%ProjectNamespace%.IdentityServer.exe

popd
