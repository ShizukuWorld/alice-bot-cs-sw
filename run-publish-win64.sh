rm -rf ./esubot/bin 
echo Delete //bin successful.
rm -rf ./esubot/obj
echo Delete //obj successful.
echo Now dotnet will publish new version on win-x64.
dotnet publish -r win-x64 -c Release