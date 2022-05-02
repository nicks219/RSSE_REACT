# Random Song Search Engine 
Ветка [DOTNET-1]
# Технологии
* .NET6 + TypeScript React + тесты     
* БД - MSSQL (Windows), MySQL(Ubuntu)
* сбилдите фронт (npm install + npm run build), скопируйте его в бэк (из FrontRepository/ClientApp/build в RandomSongSearchEngine/ClientApp/build)
* выполните dotnet restore && dotnet publish -c Release и поднимите компоуз с бд и проектом
* либо сбилдите образ с помощью Dockerfile-2 (надо отредактировать, NodeJs уже не требуется), не забудьте присоединить контейнер к mysql-сети при запуске
* в appsettings.Production.json в качестве сервера бд указано имя сервиса из docker-compose (mysql) - требуется только для работы в контейнере