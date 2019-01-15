using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.Models.General
{
    public interface IGameModel : IEntityModel
    {
        string Title { get; set; }
        string Description { get; set; }

        string CoverUrl { get; set; }

        int? PlayersFrom { get; set; }
        int? PlayersTo { get; set; }
        int? SessionMinutesFrom { get; set; }
        int? SessionMinutesTo { get; set; }
        int? AgeFrom { get; set; }
        int? YearReleased { get; set; }
    }
}