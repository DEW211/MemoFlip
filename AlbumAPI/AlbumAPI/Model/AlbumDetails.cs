using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumAPI.Model
{
    public class AlbumDetails
    {
        public string Name { get; set; }

        public string Owner { get; set; }

        public List<Picture> Pictures { get; set; }
    }
}
