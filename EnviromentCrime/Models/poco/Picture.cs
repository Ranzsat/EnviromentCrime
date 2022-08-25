using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Models
{
    public class Picture
    {
        public int PictureId { set; get; }
        public string PictureName { set; get; }
        public int ErrandId { set; get; }
    }
}
