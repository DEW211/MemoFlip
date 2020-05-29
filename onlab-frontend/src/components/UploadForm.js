import React, {useState} from 'react';
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import Link from '@material-ui/core/Link';
import Grid from '@material-ui/core/Grid';
import Box from '@material-ui/core/Box';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import Container from '@material-ui/core/Container';
import {connect} from 'react-redux'
import {fetchUploadOnAction} from '../actions/uploadFormAction'

const useStyles = makeStyles((theme) => ({
	paper: {
		marginTop: theme.spacing(8),
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'center',
	},
	avatar: {
		margin: theme.spacing(1),
		backgroundColor: theme.palette.secondary.main,
	},
	form: {
		width: '100%', // Fix IE 11 issue.
		marginTop: theme.spacing(3),
	},
	submit: {
		margin: theme.spacing(3, 0, 2),
	},
}));

function UploadForm(props) {
	const classes = useStyles();

    const [file, setFile] = useState();
    const [title, setTitle] = useState();

	const onFileChangeHandler = (event) => {
        setFile(event.target.files[0])
		console.log(event.target.files[0]);
	};

    const onTitleChangeHandler = event =>{
        setTitle(event.target.value)
        console.log(event.target.value)
    }

    

	const handleSubmit = (event) => {
        event.preventDefault();
        props.dispatch(fetchUploadOnAction(file, props.state.userName, title))
		console.log(event);
	};

	return (
		<Container component="main" maxWidth="xs">
			<CssBaseline />
			<div className={classes.paper}>
				<Typography component="h1" variant="h5">
					Create new album
				</Typography>
				<form className={classes.form} noValidate onSubmit={handleSubmit}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<TextField
								onChange={onTitleChangeHandler}
								name="Title"
								variant="outlined"
								required
								fullWidth
								id="title"
								label="Album title"
								autoFocus
							/>
						</Grid>
						<Grid item xs={12}>
							<input type="file" name="file" onChange={onFileChangeHandler} />
						</Grid>
					</Grid>
					<Button
						type="submit"
						fullWidth
						variant="contained"
						color="primary"
						className={classes.submit}
					>
						Create album
					</Button>
				</form>
			</div>
		</Container>
	);
}

const mapStateToProps = state =>{
    return {state}
}

export default connect(mapStateToProps, null)(UploadForm);

