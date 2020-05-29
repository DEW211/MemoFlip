import { SIGN_IN } from './ActionTypes';
import { fetchAlbums } from './albumsActions';

export function signInAction(username) {
	return {
		type: SIGN_IN,
		username,
	};
}

export function fetchAlbumsOnSignIn(username) {
	return function (dispatch) {
		fetchAlbums(username)(dispatch).then(()=>
		dispatch(signInAction(username)));
	};
}
