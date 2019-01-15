using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using TableTopInc.API.Engine.Services.General;
using TableTopInc.API.Mock.Models;
using TableTopInc.API.Mock.Services;

namespace TableTopInc.API.Public.Functions.General
{
    public static class Games
    {
        private static IGameService _gameService;
        
        static Games()
        {
            _gameService = new GameServiceMock();
            
            _gameService.SaveAsync(new GameModelMock
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Title = "Ticket to ride", 
                PlayersFrom = 2,
                PlayersTo = 5
            }).Wait();
            _gameService.SaveAsync(new GameModelMock
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Title = "NewYork 1901", 
                PlayersFrom = 2,
                PlayersTo = 4
            }).Wait();
            _gameService.SaveAsync(new GameModelMock
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Title = "Fallout", 
                PlayersFrom = 1,
                PlayersTo = 4
            }).Wait();
        }
        
        [FunctionName("Games")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games")]HttpRequest req,
            ILogger log)
        {
            var games = await _gameService.GetAllAsync();
            
            return new OkObjectResult(games);
        }
    }
}
