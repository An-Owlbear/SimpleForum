if (-Not (Test-Path "build")) 
{
    mkdir "build";
}
else
{
    rm -r -fo "build/**"
}

dotnet build SimpleForum.API/SimpleForum.API.csproj -c Release -o build/SimpleForum.API
dotnet build SimpleForum.CrossConnection/SimpleForum.CrossConnection.csproj -c Release -o build/SimpleForum.CrossConnection
dotnet build SimpleForum.Web/SimpleForum.Web.csproj -c Release -o build/SimpleForum.Web
cp SimpleForumConfig.json build/
cp -r UploadedImages build/
cp run.sh build/