using RandomSongSearchEngine;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseWebRoot("ClientApp/build");
    });

var app = builder.Build();

app.Run();