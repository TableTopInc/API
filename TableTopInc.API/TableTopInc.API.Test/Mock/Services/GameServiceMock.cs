using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;
using TableTopInc.API.Test.Mock.Services.Base;

namespace TableTopInc.API.Test.Mock.Services
{
    internal class GameServiceMock : ServiceBaseMock<IGameModel>, IGameService
    {
        public Task<IEnumerable<IGameModel>> GetGamesByTagsAsync(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IGameModel>> GetGamesByGameDesignersAsync(params string[] authors)
        {
            throw new NotImplementedException();
        }
    }
}