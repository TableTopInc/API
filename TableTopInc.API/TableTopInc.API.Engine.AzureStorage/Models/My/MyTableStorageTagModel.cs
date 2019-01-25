using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.My;

namespace TableTopInc.API.Engine.AzureStorage.Models.My
{
    public class MyTableStorageTagModel : MyTableStorageEntityBase, IMyTagModel
    {
        public string Title { get; set; }
    }
}