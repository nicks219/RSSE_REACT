# Random Song Search Engine 
* [DOTNET-1] : небольшой рефакторинг   
* [DOTNET-2] : добавлен нечеткий тестковый поиск  
  ```bash
  /api/find/{string} вернет json {res: [id, weight]}
  ```  
* [DOTNET-3] : актуальные поисковые индексы для N > 1 реплик (в разработке)

Бизнес-требования заказчика: сервис рассчитан на высокую нагрузку по чтению, 
нагрузка по изменению данных незначительна (этим обусловлена *простая* работа с синхронизацией)

# Технологии
* ```bash
  .NET 6 | TypeScript React | MSTest | Docker
  ```    
* ```bash
  [MSSQL] | MySQL
  ```
# Запуск
* Сбилдите фронт: ```npm install && npm run build```  
* Для локального разработки запустите:  
  ```npm start``` из папки ```Rsse.Front/ClientApp/build```  
* В каталоге есть ```docker-compose.yml``` и ```Dockerfile```
* В папке Rsse.Data/Dump есть MySql-дамп на 389 песен

# Деплой
* Пример скрипта доставки (компоуз должен быть поднят), не забудьте указать свой registry:
```bash
cd /mnt/f/rsse/RSSE_REACT/src && \
docker-compose down && \
docker image rm nick219nick/rsse:v3 && \
docker-compose build && \
docker-compose up -d && \
R_C_HASH=`docker ps | grep rsse | grep -E -o "^\S+"` && \
docker commit -a Nick219 -m v3 ${R_C_HASH} nick219nick/rsse:v3 && \
docker push nick219nick/rsse:v3;
```

# Описание API

В development зайдите на /swagger/v1/swagger.json, также в Rsse.Base/Controller закоммитан api.v1.json
