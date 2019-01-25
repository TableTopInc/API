using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Models.General
{
    public class GameDesignerTableStorageEntity : TableStorageEntityBase, IGameDesignerModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string Bio { get; set; }
        
        public string PhotoUrl { get; set; }
    }
}
