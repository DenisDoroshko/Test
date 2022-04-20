import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { UserService } from '../../services'
import { validate } from 'email-validator'
import { routes, roles } from "../../constants";
import { Layout } from '../Shared/Layout';

export class CreateUser extends Component {
    static displayName = CreateUser.name;

    constructor(props) {
        super(props);
        this.state = {
            username: "",
            name: "",
            surname: "",
            password: "",
            repeatPassword: "",
            email: "",
            role: roles.User
        };
    }

    renderUser() {
        return (
            <Layout>
                <div className="all-container">
                    <div className="header-item">
                        <h1>{strings.UserCreating}</h1>
                    </div>
                    <div className="header-item">
                        <div id="create-errors" className="text-danger"></div>
                        <form onSubmit={this.formHandler} className="form">
                            <input className="form-box" value={this.state.username} onChange={this.usernameHandler} placeholder={strings.username} />
                            <input className="form-box" value={this.state.name} onChange={this.nameHandler} placeholder={strings.name} />
                            <input className="form-box" value={this.state.surname} onChange={this.surnameHandler} placeholder={strings.surname} />
                            <input className="form-box" type="password" value={this.state.password} onChange={this.passwordHandler} placeholder={strings.password} />
                            <input className="form-box" type="password" value={this.state.repeatPassword} onChange={this.repeatPasswordHandler} placeholder={strings.repeatPassword} />
                            <input className="form-box" value={this.state.email} onChange={this.emailHandler} placeholder={strings.email} />
                            <select className="form-box" onChange={this.roleHandler}>
                                < option value={roles.User}>{roles.User}</option>
                                < option value={roles.VenueManager}>{roles.VenueManager}</option>
                                < option value={roles.EventManager}>{roles.EventManager}</option>
                            </select>
                            <div className="submit-box"><button type="submit" className="crud-button submit-button">{strings.Submit}</button></div>
                        </form>
                    </div>
                </div>
            </Layout>
        );}
    usernameHandler = (event) => {
        this.setState({ username: event.target.value });
    }
    nameHandler = (event) => {
        this.setState({ name: event.target.value });
    }
    surnameHandler = (event) => {
        this.setState({ surname: event.target.value });
    }
    passwordHandler = (event) => {
        this.setState({ password: event.target.value });
    }
    repeatPasswordHandler = (event) => {
        this.setState({ repeatPassword: event.target.value });
    }
    emailHandler = (event) => {
        this.setState({ email: event.target.value });
    }
    roleHandler = (event) => {
        this.setState({ role: event.target.value });
    }
    formHandler = async (event) => {
        event.preventDefault();
        const username = this.state.username;
        const name = this.state.name;
        const surname = this.state.surname;
        const password = this.state.password;
        const repeatPassword = this.state.repeatPassword;
        const email = this.state.email;
        const role = this.state.role;
        let errors = []
        if (!validate(email)) {
            errors.push(strings.IncorrectEmail);
        }
        if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$/.test(password)) {
            errors.push(strings.PasswordIsTooEasy);
        }
        if (password != repeatPassword) {
            errors.push(strings.PasswordsAreNotEqual);
        }
        if (!username || !name || !surname || !password || !repeatPassword || !email || !role) {
            errors.push(strings.AllFieldsMustBeFilled);
        }

        if (errors.length === 0) {
            const result = await UserService.createUser({ username: username, name: name, surname: surname, password: password, repeatPassword: repeatPassword, email: email, role: role });
            if (result.length === 0) {
                this.props.history.push(routes.manageUsers.href);
                return;
            }
            else {
                errors = errors.concat(result)
            }
        }

        var errorBox = document.getElementById("create-errors");
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