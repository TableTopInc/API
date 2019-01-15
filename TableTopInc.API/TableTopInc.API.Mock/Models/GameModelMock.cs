using System;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Mock.Models
{
    public class GameModelMock : IGameModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverUrl { get; set; }
        public int? PlayersFrom { get; set; }
        public int? PlayersTo { get; set; }
        public int? SessionMinutesFrom { get; set; }
        public int? SessionMinutesTo { get; set; }
        public int? AgeFrom { get; set; }
        public int? YearReleased { get; set; }
    }
}