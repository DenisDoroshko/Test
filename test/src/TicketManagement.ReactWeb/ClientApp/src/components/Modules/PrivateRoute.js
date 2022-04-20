import React from 'react';
import { Redirect } from 'react-router-dom';
import { Route } from "@simple-contacts/react-router-async-routes";
import { UserService } from '../../services';

export const PrivateRoute = ({ component: Component, roles, ...rest }) => (
    <Route async {...rest} render={async props => {
        const currentUser = UserService.currentUser();
        const isValid = await UserService.validateToken();
        if (currentUser && isValid && roles && (roles.length === 0 || roles.some((role) => currentUser.role.includes(role)))) {
            return <Component {...props} />
        }

        return <Redirect to={{ pathname: '/Account/Login', state: { from: props.location } }} />
    }} />
)