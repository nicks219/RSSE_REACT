// данный проект в итоге должен быть "библиотекой классов"
// MS Template .NET Core 3.1 SPA using: Microsoft.AspNetCore.SpaServices.Extensions 3.1.16
// в папке ClientApp - нода и актуальные файлы с typescript
// билд выполняется из папок public и src (+ конфиги в корне) в папку build:
// npm install && npm run build && npm start (если хочешь запустить фронт на NodeJs)
// node -v v16.14.2
// npm -v 8.5.0

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();