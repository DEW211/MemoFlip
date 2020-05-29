import React from 'react';
import Button from '@material-ui/core/Button';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardMedia from '@material-ui/core/CardMedia';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import Container from '@material-ui/core/Container';
import {connect} from 'react-redux';
import{deletePictureAction} from '../actions/deletePictureAction'



const useStyles = makeStyles((theme) => ({
  heroContent: {
    backgroundColor: theme.palette.background.paper,
    padding: theme.spacing(8, 0, 6),
  },
  heroButtons: {
    marginTop: theme.spacing(4),
  },
  cardGrid: {
    paddingTop: theme.spacing(8),
    paddingBottom: theme.spacing(8),
  },
  card: {
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
  },
  cardMedia: {
    paddingTop: '56.25%', // 16:9
  },
  cardContent: {
    flexGrow: 1,
  },
}));


function Album(props) {
  const classes = useStyles();

  return (
    <React.Fragment>
      
      
      <main>
        {/* Hero unit */}
        
        <Container className={classes.cardGrid} maxWidth="xl">
          {/* End hero unit */}
          <Grid container spacing={4}>
            {props.state.currentAlbum.pictures.map((card, i) => (
              <Grid item key={i} xs={12} sm={6} md={4}>
                <Card className={classes.card}>
                  <CardMedia
                    className={classes.cardMedia}
                    image={card.link + "?sv=2019-10-10&ss=bfqt&srt=co&sp=rwdlacupx&se=2020-05-31T20:23:47Z&st=2020-05-27T12:23:47Z&spr=https&sig=Ujjl2VWgYXigi1JnrwAcY4xzpDzvS90EeLYB5N4WBs4%3D"}
                    title="Image title"
                  />
                  <CardActions>
                    <Button size="small" color="primary" onClick={() => props.dispatch(deletePictureAction(card.id, props.state.userName, props.state.currentAlbum.name))}>
                     DELETE
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Container>
      </main>
    </React.Fragment>
  );
}

const mapStateToProps = (state) => {
  return {
    state
  }
}



export default connect(
  mapStateToProps,
  null
)(Album);
