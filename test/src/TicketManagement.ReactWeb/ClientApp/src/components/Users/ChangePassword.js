import React, { Component } from 'react';
import { AccountCenter } from '../Shared/AccountCenter';
import { strings } from '../../localization.js'
import { UserService } from '../../services'

export class ChangePassword extends Component {
    static displayName = ChangePassword.name;

    constructor(props) {
        super(props);
        this.state = {
            currentPassword: "",
            newPassword: ""
        };
    }
    currentPasswordHandler = (event) => {
        this.setState({ currentPassword: event.target.value });
    }

    newPasswordHandler = (event) => {
        this.setState({ newPassword: event.target.value });
    }
    formHandler = (event) => {
        event.preventDefault();
        const currentPassword = this.state.currentPassword;
        const newPassword = this.state.newPassword;
        if (currentPassword && newPassword) {
            UserService.changePassword(currentPassword, newPassword).then(result => {
                if (result) {
                    var successBox = document.getElementById("success-change-message");
                    successBox.innerHTML = '';
                    var content = document.createTextNode("Password successfully changed");
                    successBox.appendChild(content);
                }
                else {
                    var errorBox = document.getElementById("changePassword-errors");
                    errorBox.innerHTML = '';
                    var content = document.createTextNode("Something wrong");
                    errorBox.appendChild(content);
                }

                this.setState({ currentPassword: "", newPassword: "" });
            });
    }
    }

    render() {
        return (
            <AccountCenter>
               <p id="success-change-message" className="account-success-message"></p>
               <div id="changePassword-errors" className="error-box"></div>
                <form onSubmit={this.formHandler} className="form">
                    <label className="account-label">{strings.CurrentPasswordLabel}</label><br />
                     <input className="form-box" type="password" maxlength="120" value={this.state.currentPassword} onChange={this.currentPasswordHandler} />
                    <label className="account-label">{strings.NewPasswordLabel}</label><br />
                     <input className="form-box" type="password" value={this.state.newPassword} onChange={this.newPasswordHandler} />
                     <div className="submit-box"><button type="submit" className="crud-button submit-button">{strings.Submit}</button></div>
                </form>
            </AccountCenter>

        );
    }
}