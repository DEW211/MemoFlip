import {
	EDIT_CHANGE,
	DELETE_PICTURE,
	FETCH_ALBUMS_REQUEST,
	FETCH_ALBUMS_FAILURE,
	FETCH_ALBUMS_SUCCESS,
	FETCH_ALBUM_REQUEST,
	FETCH_ALBUM_FAILURE,
	FETCH_ALBUM_SUCCESS,
	BACK_TO_ALBUMS_VIEW,
	SIGN_IN,
	UPLOAD_FORM_VISIBLE,
	FETCH_UPLOAD_SUCCESS
} from '../actions/ActionTypes';

export const initialState = {
	userName: '',
	albums: [
		{
			name: 'Album34',
			thumbnail:
				'https://picturestoragernalbi.blob.core.windows.net/albums/2.jpg?sv=2019-10-10&ss=bfqt&srt=sco&sp=rwdlacup&se=2020-05-09T19:58:41Z&st=2020-05-03T11:58:41Z&spr=https&sig=0aQ2VlJzfH4ZBhCizuD6HrYCuxLT6A6PyaIvhlKnii0%3D',
		},
	],
	isFetching: false,
	isEditing: false,
	isSignedIn: false,
	isUpload: false,
	error: '',
	currentAlbum: {
		name: 'Album2',
		owner: 'DEW211',
		pictures: [
			{
				id: 'id1',
				link:
					'https://picturestoragernalbi.blob.core.windows.net/albums/1.jpg?sv=2019-10-10&ss=bfqt&srt=sco&sp=rwdlacup&se=2020-05-09T19:58:41Z&st=2020-05-03T11:58:41Z&spr=https&sig=0aQ2VlJzfH4ZBhCizuD6HrYCuxLT6A6PyaIvhlKnii0%3D',
			},
			{
				id: 'id2',
				link:
					'https://picturestoragernalbi.blob.core.windows.net/albums/2.jpg?sv=2019-10-10&ss=bfqt&srt=sco&sp=rwdlacup&se=2020-05-09T19:58:41Z&st=2020-05-03T11:58:41Z&spr=https&sig=0aQ2VlJzfH4ZBhCizuD6HrYCuxLT6A6PyaIvhlKnii0%3D',
			},
			{
				id: 'id3',
				link:
					'https://picturestoragernalbi.blob.core.windows.net/albums/3.jpg?sv=2019-10-10&ss=bfqt&srt=sco&sp=rwdlacup&se=2020-05-09T19:58:41Z&st=2020-05-03T11:58:41Z&spr=https&sig=0aQ2VlJzfH4ZBhCizuD6HrYCuxLT6A6PyaIvhlKnii0%3D',
			},
			{
				id: 'id4',
				link:
					'https://picturestoragernalbi.blob.core.windows.net/albums/1.jpg?sv=2019-10-10&ss=bfqt&srt=sco&sp=rwdlacup&se=2020-05-09T19:58:41Z&st=2020-05-03T11:58:41Z&spr=https&sig=0aQ2VlJzfH4ZBhCizuD6HrYCuxLT6A6PyaIvhlKnii0%3D',
			},
			{
				id: 'id5',
				link:
					'https://picturestoragernalbi.blob.core.windows.net/albums/2.jpg?sv=2019-10-10&ss=bfqt&srt=sco&sp=rwdlacup&se=2020-05-09T19:58:41Z&st=2020-05-03T11:58:41Z&spr=https&sig=0aQ2VlJzfH4ZBhCizuD6HrYCuxLT6A6PyaIvhlKnii0%3D',
			},
			{
				id: 'id6',
				link:
					'https://picturestoragernalbi.blob.core.windows.net/albums/3.jpg?sv=2019-10-10&ss=bfqt&srt=sco&sp=rwdlacup&se=2020-05-09T19:58:41Z&st=2020-05-03T11:58:41Z&spr=https&sig=0aQ2VlJzfH4ZBhCizuD6HrYCuxLT6A6PyaIvhlKnii0%3D',
			},
		],
	},
};

export function editReducer(state = initialState, action) {
	switch (action.type) {
		case EDIT_CHANGE:
			return Object.assign({}, state, {
				isEditing: !state.isEditing,
			});
		case DELETE_PICTURE:
			let newState = Object.assign({}, state);
			newState.currentAlbum.Pictures = newState.currentAlbum.pictures.filter(
				(value, index, arr) => {
					return value.id !== action.id;
				}
			);
			return newState;
		case FETCH_ALBUMS_REQUEST:
			return Object.assign({}, state, { isFetching: true });
		case FETCH_ALBUMS_SUCCESS:
			return Object.assign({}, state, {
				albums: action.data,
				isFetching: false,
			});
		case FETCH_ALBUMS_FAILURE:
			return Object.assign({}, state, {
				error: action.error,
				isFetching: false,
			});
		case FETCH_ALBUM_REQUEST:
			console.log(action);
			return Object.assign({}, state, { isFetching: true });
		case FETCH_ALBUM_SUCCESS:
			return Object.assign({}, state, {
				currentAlbum: action.data,
				isFetching: false,
				isEditing: true,
			});
		case FETCH_ALBUM_FAILURE:
			return Object.assign({}, state, {
				error: action.error,
				isFetching: false,
			});
		case BACK_TO_ALBUMS_VIEW:
			return Object.assign({}, state, { isEditing: false, isUpload: false });
		case SIGN_IN:
			return Object.assign({}, state, {userName: action.username, isSignedIn: true})
		case UPLOAD_FORM_VISIBLE:
			return Object.assign({}, state, {isUpload: true})
		case FETCH_UPLOAD_SUCCESS:
			return Object.assign({}, state, {isUpload: false})
		default:
			return state;
	}
}
