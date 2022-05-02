FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY bin/Release/net6.0/publish/ App/
COPY ClientApp/build/ App/ClientApp/build/
WORKDIR /App
EXPOSE 5000
ENTRYPOINT ["dotnet", "RandomSongSearchEngine.dll"]