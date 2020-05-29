import { fetchAlbum } from './albumActions';
export function deletePictureAction(pictureId, owner, title) {
	return function (dispatch) {
		return fetch(
			`/albumapi/api/album/${owner}/${title}/${pictureId}`,
			{ method: 'DELETE', mode: 'cors' }
		).then((data) => {
			dispatch(fetchAlbum(owner, title));
		});
	};
}

//delete hívás a végpontra, album újratöltése
