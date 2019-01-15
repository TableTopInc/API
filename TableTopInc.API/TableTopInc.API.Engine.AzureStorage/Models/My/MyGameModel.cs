using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.My;

namespace TableTopInc.API.Engine.AzureStorage.Models.My
{
    public class MyGameModel : MyEntityBase, IMyGameModel
    {
        public string Title { get; set; }
        
        public int? PlayersFrom { get; set; }
        public int? PlayersTo { get; set; }
        public int? SessionMinutesFrom { get; set; }
        public int? SessionMinutesTo { get; set; }
        public int? AgeFrom { get; set; }
        public int? YearReleased { get; set; }
        
        public string Description { get; set; }
        
        public string CoverUrl { get; set; }
    }
}