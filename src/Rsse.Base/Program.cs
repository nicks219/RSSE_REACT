//namespace RandomSongSearchEngine;

//public class Program

using RandomSongSearchEngine;

//{
    //public static void Main(string[] args)
    //{
    //CreateHostBuilder(args).Build().Run();
    //}

    //private static IHostBuilder CreateHostBuilder(string[] args) =>

    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            // [DOTNET-1] - папка со статикой
            webBuilder.UseWebRoot("ClientApp/build");
        });

    var app = builder.Build();

    app.Run();
//}