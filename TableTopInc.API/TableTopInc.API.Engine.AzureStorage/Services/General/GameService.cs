using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class GameService : AzureTableService<GameTableEntity, IGameModel>, IGameService
    {
        public const string TableName = "Games";
        
        public GameService(CloudTable table)
            : base(table)
        {
        }

        public async Task<IEnumerable<IGameModel>> GetGamesByTagsAsync(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IGameModel>> GetGamesByGameDesignersAsync(params string[] authors)
        {
            throw new NotImplementedException();
        }
    }
}