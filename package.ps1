Push-Location
Set-Location ./Sharpenter.BootstrapperLoader

Remove-Item bin\Release\*.nupkg
dotnet pack -c Release

Pop-Location