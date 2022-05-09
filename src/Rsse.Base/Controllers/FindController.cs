using Microsoft.AspNetCore.Mvc;
using RandomSongSearchEngine.Service.Models;

namespace RandomSongSearchEngine.Controllers;

[Route("api/find")]
public class FindController : ControllerBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FindController> _logger;

    public FindController(IServiceScopeFactory scopeFactory, ILogger<FindController> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    // TODO: необходимо логгировать запросы на поиск
    // TODO: сделать индекс, зависящий от количества слов в источнике и сортировку ответов по индексу
    // иначе в случае "мусорного поиска" точные совпадения окажутся в конце, если они были добавлены в финал бд
    [HttpGet]
    public ActionResult Find(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Ok(new{});
        }
        
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var model = new FindModel(scope);
            var result = model.Find(text);

            if (result.Count == 0)
            {
                return Ok(new{});
            }

            // нулевой вес не стоит учитывать если результатов много
            if (result.Count > 10)
            {
                result = result
                    .Where(kv => kv.Value > 0)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            
            result = result
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            var resp = new OkObjectResult(new{Res = result});

            return resp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FindController: OnGet Error - Search Indices May Failed !]");
            return new BadRequestObjectResult("[FindController: OnGet Error - Search Indices May Failed !]");
        }
    }
}