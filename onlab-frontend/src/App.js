import React from 'react';
import Albums from './components/Albums';
import Album from './components/Album';
import Header from './components/Header';
import CssBaseline from '@material-ui/core/CssBaseline';
import SignIn from './components/SignIn';
import UploadForm from './components/UploadForm';

import { connect } from 'react-redux';

function App(props) {
	return (
		<div>
			<CssBaseline />
			{props.state.isSignedIn && <Header />}
			{props.state.isUpload ? (
				<UploadForm />
			) : props.state.isSignedIn ? (
				props.state.isEditing ? (
					<Album />
				) : (
					<Albums />
				)
			) : (
				<SignIn />
			)}
		</div>
	);
}

const mapStateToProps = (state) => {
	return {
		state,
	};
};

export default connect(mapStateToProps)(App);
