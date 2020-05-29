using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumAPI.DAL.Entities
{
    public class Album
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("AlbumName")]
        public string AlbumName { get; set; }

        [BsonElement("Owner")]
        public string Owner { get; set; }

        [BsonElement("Pictures")]
        public List<ObjectId> Pictures { get; set; }


    }
}
