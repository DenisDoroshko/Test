import React, { Component } from 'react';
import { routes } from "../../constants";
import { UserService } from '../../services'

export class Logout extends Component {
    static displayName = Logout.name;

    componentWillMount() {
        UserService.logout();
        this.props.history.push(routes.home.href);
    }
  render() {
    return ( null);
  }
}
