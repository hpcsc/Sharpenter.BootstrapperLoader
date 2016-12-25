cd ../

$configurations = @("Release 4.5")
Copy-Item ".\deployment\Sharpenter.BootstrapperLoader.nuspec" -Destination ".\Sharpenter.BootstrapperLoader"

foreach ($conf in $configurations) {	
    Write-Host "======================================================="
    Write-Host "==========      Building $($conf)  ===================="

    Invoke-Expression "& .\build.ps1 -Configuration `"$($conf)`""    
}

Write-Host "======================================================="
Write-Host "===================      Packaging  ==================="

cd ./Sharpenter.BootstrapperLoader
md -Force NuGet
Invoke-Expression "..\tools\nuget.exe pack Sharpenter.BootstrapperLoader.csproj -OutputDir NuGet -Prop Configuration=`"Release 4.5`""

Remove-Item ".\Sharpenter.BootstrapperLoader.nuspec"

cd ../deployment