using AlbumAPI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AlbumAPI.DAL
{
    public interface IAlbumRepository
    {
        IList<Model.Album> ListAlbums();
        Model.Album Create(Model.RawAlbum value);

        Model.Album FindByNameAndOwner(string name, string owner);
        Model.Album Delete(string owner, string name);
        Model.AlbumDetails FindDetailsByNameAndOwner(string name, string owner);
        bool AddPicture(string owner, string name, string link);
        bool DeletePictureFromAlbum(string owner, string name, string id);
        IEnumerable<Model.Album> ListAlbumsOfOwner(string owner);
    }
}
