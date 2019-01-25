using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class GameDesignerAzureService : AzureTableStorageServiceBase<GameDesignerTableStorageEntity, IGameDesignerModel>, IGameDesignerService
    {
        public const string TableName = "GameDesigners";
        
        public GameDesignerAzureService(CloudTable table)
            : base(table)
        {
        }
    }
}