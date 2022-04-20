import React, { Component } from 'react';
import { strings } from '../../localization';
import { UserService, RedirectService } from '../../services';
import { routes, roles } from "../../constants";
import { Layout } from './Layout';

export class AccountCenter extends Component {
    static displayName = AccountCenter.name;
    render() {
        return (
            <Layout>
                <div className="all-container">
                    <div className="header-item">
                        <h1>{strings.AccountManagement}</h1>
                    </div>
                    <div className="account-info-container">
                        <div className="container-item">
                            <ul id="account-navbar">
                            {UserService.isInRole(roles.User) ?
                                <>
                                   <li><a href={RedirectService.prepareLink(routes.balance.href)}>{strings.Balance}</a></li>
                                   <li><a href={RedirectService.prepareLink(routes.purchaseHistory.href)}>{strings.MyTickets}</a></li>
                                </> : null
                                }
                                <li><a href={routes.editAccount.href}>{strings.EditContactInformation}</a></li>
                                <li><a href={routes.changePassword.href}>{strings.ChangePassword}</a></li>
                                <li><a href={routes.logout.href}>{strings.Exit}</a></li>
                            </ul>
                        </div>
                        <div className="container-item">
                            {this.props.children}
                        </div>
                    </div>
                </div>
            </Layout>
        );
    }
}