cd ../

Write-Host "==============================================="
Write-Host "==========      Building   ===================="

& ".\build.ps1"

Write-Host "==============================================="
Write-Host "==========      Packaging   ==================="

Copy-Item ".\deployment\Sharpenter.BootstrapperLoader.nuspec" -Destination ".\Sharpenter.BootstrapperLoader"

cd ./Sharpenter.BootstrapperLoader

Invoke-Expression "..\tools\nuget.exe pack Sharpenter.BootstrapperLoader.csproj -Prop Configuration=Release"

Remove-Item ".\Sharpenter.BootstrapperLoader.nuspec"

cd ../deployment