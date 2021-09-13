using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Models;

namespace RandomSongSearchEngine.Services.Logger
{
    public static class FullLog
    {
        /// <summary>
        /// Логгирует Id модели, треда и http-контекста
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="model">Модель, из которой вызывается метод</param>
        public static void LogId (this ILogger logger, SongModel model)
        {
            var modelId = model.GetHashCode();
            //костыли - у модели сейчас нет контекста
            var httpContextId = "MockContext";// model.HttpContext.GetHashCode();
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            logger.LogInformation("[Model ID: {0} Thread ID: {1} HttpContextID: {2}]", modelId, threadId, httpContextId);
        }
    }
}
