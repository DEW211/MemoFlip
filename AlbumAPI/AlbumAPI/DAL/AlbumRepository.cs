using AlbumAPI.DAL.Entities;
using AlbumAPI.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace AlbumAPI.DAL
{

    public class AlbumRepository : IAlbumRepository
    {
        private readonly IMongoCollection<Entities.Album> albumCollection;
        private readonly IMongoCollection<Entities.Picture> pictureCollection;
        public AlbumRepository(IMongoDatabase database)
        {
            albumCollection = database.GetCollection<Entities.Album>("albums");
            pictureCollection = database.GetCollection<Entities.Picture>("pictures");
        }

        public Model.Album Create(Model.RawAlbum value)
        {
            var album = FindByNameAndOwner(value.Name, value.Owner);

            if (album == null)
            {
                var toInsert = toEntityFromRaw(value);
                albumCollection.InsertOne(toInsert);
                return toModel(FindByNameAndOwner(toInsert.AlbumName, toInsert.Owner));
            }
            else
            {
                throw new ArgumentException("This user already has an album under that name");
            }
        }

        private Entities.Album toEntityFromRaw(RawAlbum value)
        {
            return new Entities.Album
            {
                Owner = value.Owner,
                AlbumName = value.Name,
                Pictures = new List<ObjectId>() { new ObjectId("5eb01dda5cb63f469c3bc968") }
            };
        }

        public Model.Album Delete(string owner, string name)
        {
            var album = FindByNameAndOwner(name, owner);
            if (album == null)
            {
                return null;
            }
            else
            {
                albumCollection.DeleteOne(a => a.Id == album.Id);
                return toModel(album);

            }
        }

        public Entities.Album FindByNameAndOwner(string name, string owner)
        {
            var result = albumCollection.Find(a => a.AlbumName == name && a.Owner == owner)
                .SingleOrDefault();
            return result;
        }

        public Model.AlbumDetails FindDetailsByNameAndOwner(string name, string owner)
        {
            var result = albumCollection.Find(a => a.AlbumName == name && a.Owner == owner)
                .SingleOrDefault();
            if (result == null)
            {
                return null;
            }
            else
            {
                return toModelDetails(result);
            }
        }


        public IList<Model.Album> ListAlbums()
        {
            var albums = albumCollection
                .Find(_ => true)
                .Project(a => toModel(a))
                .ToList();
            return albums;
        }



        private Entities.Album toEntity(Model.Album album)
        {
            List<ObjectId> pictures = new List<ObjectId>();
            //foreach(string id in album.Pictures)
            //{
            //    pictures.Add(new ObjectId(id));
            //}
            return new Entities.Album
            {
                AlbumName = album.Name,
                Owner = album.Owner,
                Pictures = pictures
            };
        }

        private Model.Album toModel(Entities.Album album)
        {
            List<string> pictures = new List<string>();
            foreach (ObjectId id in album.Pictures)
            {
                pictures.Add(id.ToString());
            }
            return new Model.Album
            {
                Owner = album.Owner,
                Name = album.AlbumName,
                Thumbnail = (pictures.Count == 0) ? null : new Model.Picture { Id = pictures[0] }
            };
        }

        private Model.AlbumDetails toModelDetails(Entities.Album album)
        {
            var pictures = album.Pictures.Select(p =>
            {
                var picture = pictureCollection.Find(p2 => p2.Id == p)
                   .Project(p3 => new Model.Picture
                   {
                       Id = p3.Id.ToString(),
                       Link = p3.Link
                   })
                   .SingleOrDefault();
                return picture;
            })
            .ToList();
            return new Model.AlbumDetails
            {
                Name = album.AlbumName,
                Owner = album.Owner,
                Pictures = pictures
            };
        }

        Model.Album IAlbumRepository.FindByNameAndOwner(string name, string owner)
        {
            throw new NotImplementedException();
        }



        public bool AddPicture(string owner, string name, string link)
        {
            Entities.Picture picture = new Entities.Picture { Link = link};
            pictureCollection.InsertOne(picture);
            var inserted = pictureCollection.Find(p => p.Link == link).First();

            var placeHolderID = new ObjectId("5eb01dda5cb63f469c3bc968");
            var album = albumCollection.Find(a => a.AlbumName == name && a.Owner == owner).SingleOrDefault();
            if (album != null)
            {
                if (album.Pictures[0] == placeHolderID)
                {
                    var update = albumCollection.UpdateOne(
                        filter: p => p.AlbumName == name && p.Owner == owner,
                        update: Builders<Entities.Album>.Update.Pull(a => a.Pictures, placeHolderID),
                        options: new UpdateOptions { IsUpsert = false });
                }
            }


            var result = albumCollection.UpdateOne(
                filter: p => p.AlbumName == name && p.Owner == owner,
                update: Builders<Entities.Album>.Update.Push(a => a.Pictures, inserted.Id),
                options: new UpdateOptions { IsUpsert = false }
                );
            if (result.MatchedCount <= 0)
            {
                pictureCollection.DeleteOne(p => p.Id == inserted.Id);
                return false;
            }
            return true;
        }

        public bool DeletePictureFromAlbum(string owner, string name, string id)
        {
            //delete from pictures collection
            pictureCollection.DeleteOne(p => p.Id == new ObjectId(id));
            //delete from album
            var result = albumCollection.UpdateOne(
                filter: a => a.AlbumName == name && a.Owner == owner,
                update: Builders<Entities.Album>.Update.Pull(a => a.Pictures, new ObjectId(id)),
                options: new UpdateOptions { IsUpsert = false }
                );
            return result.MatchedCount > 0;
        }

        public IEnumerable<Model.Album> ListAlbumsOfOwner(string owner)
        {
            var list = albumCollection.Find(a => a.Owner == owner).Project(a => toModel(a)).ToList();
            var picture = pictureCollection.Find(p2 => p2.Id == new ObjectId(list[0].Thumbnail.Id))
                .Project(p3 => new Model.Picture
                {
                    Id = p3.Id.ToString(),
                    Link = p3.Link
                })
                .SingleOrDefault();
            var joinedList = list.Select(a =>
            {
                var picture = pictureCollection.Find(p2 => p2.Id == new ObjectId(a.Thumbnail.Id))
                .Project(p3 => new Model.Picture
                {
                    Id = p3.Id.ToString(),
                    Link = p3.Link
                })
                .SingleOrDefault();
                var res = a;
                res.Thumbnail = picture;
                return res;
            });
            return joinedList;
        }
    }
}
