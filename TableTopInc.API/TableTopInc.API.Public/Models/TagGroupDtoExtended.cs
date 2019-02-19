using System.Collections.Generic;

namespace TableTopInc.API.Public.Models
{
    public class TagGroupDtoExtended : TagGroupDto
    {
        public List<TagDto> Tags { get; set; }
    }
}