using Microsoft.Azure.CosmosDB.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class GameDesignerRoleService : EntityServiceBase<GameDesignerRole>
    {
        public GameDesignerRoleService(CloudTable table)
            : base(table)
        {
        }
    }
}