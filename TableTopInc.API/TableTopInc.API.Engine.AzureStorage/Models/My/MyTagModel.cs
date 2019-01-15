using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.My;

namespace TableTopInc.API.Engine.AzureStorage.Models.My
{
    public class MyTagModel : MyEntityBase, IMyTagModel
    {
        public string Title { get; set; }
    }
}