using Microsoft.Extensions.Logging;

namespace RandomSongSearchEngine.Services.Logger;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _path;

    public FileLoggerProvider(string path)
    {
        this._path = path;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_path, categoryName);
    }

    public void Dispose()
    {
    }
}