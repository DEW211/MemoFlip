import {
	UPLOAD_FORM_VISIBLE,
	FETCH_UPLOAD_SUCCESS,
	FETCH_UPLOAD_FAILURE,
	FETCH_UPLOAD_REQUEST,
} from './ActionTypes';

export function uploadFormAction() {
	return {
		type: UPLOAD_FORM_VISIBLE,
	};
}

export function fetchUploadSuccessAction() {
	return {
		type: FETCH_UPLOAD_SUCCESS
	}
}

export function fetchUploadOnFailureAction(error) {
	return {
		type: FETCH_UPLOAD_FAILURE,
		error
	}
}

export function fetchUploadOnAction(file, owner, title) {


	return function (dispatch) {


		let formData = new FormData();
		formData.append('files', file);
		formData.append('albumTitle', title);
		formData.append('albumOwner', owner);

		return fetch(`http://localhost:8082/api/video`, {
			method: 'POST',
			mode: 'cors',
			body: formData,
		}).then((response) => dispatch(fetchUploadSuccessAction()),(error) => dispatch(fetchUploadOnFailureAction(error))

		);
	};
}
