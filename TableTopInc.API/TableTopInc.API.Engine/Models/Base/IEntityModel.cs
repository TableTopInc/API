using System;

namespace TableTopInc.API.Engine.Models.Base
{
    public interface IEntityModel
    {
        Guid Id { get; set; }
    }
}