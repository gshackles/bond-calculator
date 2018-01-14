Invoke-Expression "msbuild /p:VisualStudioVersion=14.0 /p:Configuration=Release"

Copy-Item C:\bonds.csv bin\Release\bonds.csv

Invoke-Expression "docker build -t bond-calculator ."