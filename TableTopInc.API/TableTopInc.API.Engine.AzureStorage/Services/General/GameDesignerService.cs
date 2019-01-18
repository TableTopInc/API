using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class GameDesignerService : EntityServiceBase<GameDesignerTableEntity>
    {
        public GameDesignerService(CloudTable table)
            : base(table)
        {
        }
    }
}