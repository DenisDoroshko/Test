import React, { Component } from 'react';
import { AccountCenter } from '../Shared/AccountCenter';
import { strings } from '../../localization.js'
import { UserService } from '../../services'
import momentTZ from 'moment-timezone';
import { validate } from 'email-validator'

const timeZonesList = momentTZ.tz.names();
export class EditAccount extends Component {
    static displayName = EditAccount.name;

    constructor(props) {
        super(props);
        this.state = {
            id: "",
            username: "",
            name: "",
            surname: "",
            email: "",
            timeZoneId: "",
            loading: true
        };
    }

    async componentDidMount() {
        const currentUser = UserService.currentUser();
        const data = await UserService.getUser(currentUser.id);
        if (data) {
            this.setState({
                id: data.id,
                username: data.username,
                name: data.name,
                surname: data.surname,
                email: data.email,
                timeZoneId: data.timeZoneId,
                loading: false
            });
        }
    }
    renderUser() {
        return (
            <AccountCenter>
                <p id="success-change-message" className="account-success-message"></p>
                <div id="edit-account-errors" className="error-box"></div>
                <form onSubmit={this.formHandler} className="form">
                    <label className="account-label">{strings.Username}</label><br />
                    <input className="form-box" disabled value={this.state.username}  />
                    <label className="account-label">{strings.Name}</label><br />
                    <input className="form-box" type="text" value={this.state.name} onChange={this.nameHandler} />
                    <label className="account-label">{strings.Surname}</label><br />
                    <input className="form-box" type="text" value={this.state.surname} onChange={this.surnameHandler} />
                    <label className="account-label">{strings.Email}</label><br />
                    <input className="form-box" type="text" value={this.state.email} onChange={this.emailHandler} />
                    <select className="form-box" onChange={this.timeZoneIdHandler}>
                        {timeZonesList.map(zone => zone == this.state.timeZoneId ? < option selected value={zone} > {zone}</option> : < option value={zone} > {zone}</option>)}
                    </select>
                    <div className="submit-box"><button type="submit" className="crud-button submit-button">{strings.Submit}</button></div>
                </form>
            </AccountCenter>
        );}
    nameHandler = (event) => {
        this.setState({ name: event.target.value });
    }

    surnameHandler = (event) => {
        this.setState({ surname: event.target.value });
    }
    emailHandler = (event) => {
        this.setState({ email: event.target.value });
    }
    timeZoneIdHandler = (event) => {
        this.setState({ timeZoneId: event.target.value });
    }
    formHandler = async (event) => {
        event.preventDefault();
        const id = this.state.id;
        const username = this.state.username;
        const name = this.state.name;
        const surname = this.state.surname;
        const email = this.state.email;
        const timeZoneId = this.state.timeZoneId;
        let errors = []
        if (!validate(email)) {
            errors.push(strings.IncorrectEmail);
        }
        if (!id || !username || !name || !surname || !email || !timeZoneId) {
            errors.push(strings.AllFieldsMustBeFilled);
        }
        if (errors.length == 0) {
            const result = await UserService.edit({ id: id, username: username, name: name, surname: surname, email: email, timeZoneId: timeZoneId })
            if (result) {
                var successBox = document.getElementById("success-change-message");
                var content = document.createTextNode(strings.ContactInformationChanged);
                successBox.appendChild(content);
            }
            else {
                errors = errors.push(strings.SomethingWrong);
            }
        }
        var errorBox = document.getElementById("edit-account-errors");
        errorBox.innerHTML = '';
        errors.forEach(function (item, i, errors) {
            errorBox.appendChild(document.createTextNode(item));
            errorBox.appendChild(document.createElement("br"));
        });
    }

    render() {
            let contents = this.state.loading
                ? <p><em>Loading...</em></p>
                : this.renderUser();

        return (
            <>
                {contents}
            </>
            );
        }
}