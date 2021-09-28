# Random Song Search Engine 
Разработано и задеплоено по заказу частного лица   
Сервис выводит в окне браузера текст случайной песни по заданным категориям из базы данных  
Функционал: аутентификация, просмотр, добавление, изменение, удаление песен, а также просмотр каталога с песнями    
# Apache Bench
Код проекта асинхронный     
* Нагрузочное тестирование (локалхост, Windows, IIS с пулом 128Mb, MSSQL):     
100.000 запросов (параллельно 15.000) за 57 секунд (ab -n 100000 -c 15000)     
* Нагрузочное тестрирование (локалхост, Ubuntu Server из-под Hyper-V 512Mb ram 1vCPU, Kestrel, MySQL):    
100.000 запросов (параллельно 15.000) за 79 секунд (ab -n 100000 -c 15000)  
* Нагрузочное тестрирование в продакшне:  
В процессе поиска адекватного хостинга.        
# Технологии
Что-то надо же рассказать
* Среда выполнения - .NET Core 3.1   
* Реализован REST - запросы Create, Read, Update, Delete    
* Фронт - React (TypeScript на данный момент, также оставил JSX как legacy)     
* Транспилятор JSX - NuGet пакет REACT.ASPNET    
# Комментарии   
* Рефакторинг проекта https://github.com/nicks219/Random-Song-Search-Engine        
* При запуске при необходимости создастся бд с заполненной таблицей с категориями текстов (T-SQL)  
Таблица для авторизации пользователя останется пустой    
* При запуске в dev. или prod. менеджер npm создаст все необходимое при наличии Node.js        
* Проект в стадии доработки, код постепенно обкладываю MSUnit тестами   
