# Random Song Search Engine 
* Ветка [DOTNET-1] : рефакторинг   
* Ветка [DOTNET-2] : добавлен нечеткий тестковый поиск  
  ```bash
  /api/find/{string} вернет json {res: [id, weight]}
  ```  
  Цель: gRpc сервис и CI/CD конвейер
# Технологии
* ```bash
  .NET 6 | TypeScript React | тесты | докеры
  ```    
* ```bash
  MSSQL | MySQL
  ```
# Запуск
* Сбилдите фронт: ```npm install && npm run build```  
  Для локального запуска скопируйте папку ```Rsse.Front/ClientApp/build``` в ```Rsse.Base/ClientApp/build```
* Поднимите компоуз с бд и приложением
* Либо сбилдите только приложение с помощью ```Dockerfile```  
  Фронт будет скопирован в процессе, не забудьте присоединить контейнер к ```mysql-сети``` при запуске
* В ```appsettings.Production.json``` в качестве сервера бд указано имя сервиса из docker-compose ```mysql```  
  Это работает только в контейнере и сделано для запуска в контейнере
* Фронт на данный момент "пойдёт" на ```localhost:5000```  
  Cделано для отдельного запуска фронта на ```NodeJs```  
  Для деплоя необходмо убрать префикс ```corsAddress``` в ```loader.jsx``` и ```login.tsx```