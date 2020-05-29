using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumAPI.DAL.Entities
{
    public class Picture
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Owner { get; set; }
        public string Link { get; set; }

    }
}
