dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true 
pushd bin\Debug\netcoreapp3.1\linux-arm\publish
pscp -P 22 -pw <sshPassword> -v -r .\* <IPaddressToPi>:/home/pi/IoTButton
popd