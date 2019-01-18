using TableTopInc.API.Engine.AzureStorage.Helpers;
using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Models.General
{
    public class GameTableEntity : EntityBase, IGameModel
    {
        public static GameTableEntity Create(IGameModel model)
        {
            return model.ToTableEntity();
        }
        
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
