import React from 'react';
import './styles/main.css';
import './styles/login.css';
import { Route, Switch, withRouter } from "react-router-dom";
import { Home } from './components/Home/Home';
import { Events } from './components/Events/Events';
import { Login } from './components/Users/Login';
import { Registration } from './components/Users/Registration';
import { EditAccount } from './components/Users/EditAccount';
import { Logout } from './components/Users/Logout';
import { PrivateRoute } from './components/Modules/PrivateRoute';
import { routes, roles } from "./constants";
import { ChangePassword } from './components/Users/ChangePassword';
import { ManageUsers } from './components/Users/ManageUsers';
import { CreateUser } from './components/Users/CreateUser';
import { CreateEvent } from './components/Events/CreateEvent';
import { EditEvent } from './components/Events/EditEvent';
import { ManageEvents } from './components/Events/ManageEvents';

let search = window.location.search;
let params = new URLSearchParams(search);
let lang = params.get('lang');
if (lang) {
    localStorage.setItem('lang', lang);
}
let token = params.get('token');
if (token) {
    localStorage.setItem('token', token);
}
const App = ({ history }) => {

    return (
        <Switch>
            <Route exact history={history} path={routes.home.href} component={Home} />
            <Route history={history} path={routes.events.href} component={Events} />
            <PrivateRoute history={history} path={routes.manageUsers.href} roles={[roles.VenueManager]} component={ManageUsers} />
            <PrivateRoute history={history} path={routes.editAccount.href} roles={[]} component={EditAccount} />
            <PrivateRoute history={history} path={routes.changePassword.href} roles={[]} component={ChangePassword} />
            <Route history={history} path={routes.login.href} component={Login} />
            <PrivateRoute history={history} path={routes.createUser.href} roles={[roles.VenueManager]} component={CreateUser} />
            <PrivateRoute history={history} path={routes.createEvent.href} roles={[roles.EventManager]} component={CreateEvent} />
            <PrivateRoute history={history} path={routes.editEvent.href} roles={[roles.EventManager]} component={EditEvent} />
            <PrivateRoute history={history} path={routes.manageEvents.href} roles={[roles.EventManager]} component={ManageEvents} />
            <PrivateRoute history={history} path={routes.logout.href} roles={[]} component={Logout} />
            <Route history={history} path={routes.registration.href} component={Registration} />
        </Switch>
    );
}

export default withRouter(App);
