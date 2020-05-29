import thunkMiddleware from 'redux-thunk';
//import { createLogger } from 'redux-logger';
import { createStore, applyMiddleware, compose } from 'redux';
import { editReducer } from '../reducers/editReducer';

//const loggerMiddleware = createLogger();
const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
export const store = createStore(
	editReducer,
	/* preloadedState, */ composeEnhancers(applyMiddleware(thunkMiddleware))
);
/*export const store = createStore(
	editReducer,
	applyMiddleware(thunkMiddleware),
	window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
);*/
