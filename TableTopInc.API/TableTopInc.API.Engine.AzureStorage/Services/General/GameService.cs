using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class GameService : EntityServiceBase<GameTableEntity>
    {
        public const string TableName = "Games";
        
        public GameService(CloudTable table)
            : base(table)
        {
        }
    }
}