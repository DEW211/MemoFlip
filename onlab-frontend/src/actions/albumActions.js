import {
	FETCH_ALBUM_REQUEST,
	FETCH_ALBUM_FAILURE,
	FETCH_ALBUM_SUCCESS,
} from './ActionTypes';

export function fetchAlbumAction(owner) {
	return {
		type: FETCH_ALBUM_REQUEST,
		owner,
	};
}

export function fetchAlbumOnSuccessAction(data) {
	return {
		type: FETCH_ALBUM_SUCCESS,
		data,
	};
}

export function fetchAlbumOnFailureAction(error) {
	return {
		type: FETCH_ALBUM_FAILURE,
		error,
	};
}

export function fetchAlbum(owner, title) {
	console.log(owner);
		console.log(title);
	return function (dispatch) {
		dispatch(fetchAlbumAction(owner));
		
		return fetch(`http://localhost:8081/api/album/${owner}/${title}/details`)
			.then(
				(response) => response.json(),
				(error) => dispatch(fetchAlbumOnFailureAction(error))
			)
			.then((json) => {
				dispatch(fetchAlbumOnSuccessAction(json))
			});
	};
}
