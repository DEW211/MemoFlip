import {
	FETCH_ALBUMS_REQUEST,
	FETCH_ALBUMS_FAILURE,
	FETCH_ALBUMS_SUCCESS,
} from './ActionTypes';

export function fetchAlbumsAction(owner) {
	return {
		type: FETCH_ALBUMS_REQUEST,
		owner,
	};
}

export function fetchAlbumsOnSuccessAction(data) {
	return {
		type: FETCH_ALBUMS_SUCCESS,
		data,
	};
}

export function fetchAlbumsOnFailureAction(error) {
	return {
		type: FETCH_ALBUMS_FAILURE,
		error,
	};
}

export function fetchAlbums(owner) {
	return function (dispatch) {
		dispatch(fetchAlbumsAction(owner));
		console.log(`fetching `)
		return fetch(`http://localhost:8081/api/album/${owner}`)
			.then(
				(response) => response.json(),
				(error) => dispatch(fetchAlbumsOnFailureAction(error))
			)
			.then((json) => dispatch(fetchAlbumsOnSuccessAction(json)));
	};
}
