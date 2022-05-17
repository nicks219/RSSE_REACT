using RandomSongSearchEngine;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseWebRoot("ClientApp/build");
        webBuilder.UseKestrel(options =>
        {
            // MinRequest ?
            // Minresponse ?
            // MaxConcurrent ?
            var l = options.Limits;
        });
    });

var app = builder.Build();

app.Run();