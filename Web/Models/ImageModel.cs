
using System.Collections.Generic;

namespace Web.Models
{
    public class ImagesModel
    {
        public IEnumerable<ImageModel> data { get; set; }
    }

    public class ImageModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}