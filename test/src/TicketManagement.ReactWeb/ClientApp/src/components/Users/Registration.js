import React, { Component } from 'react';
import { routes } from "../../constants";
import { strings } from '../../localization.js'
import { UserService } from '../../services'
import { validate } from 'email-validator'

export class Registration extends Component {
    static displayName = Registration.name;

    constructor(props) {
        super(props);

        this.state = {
            username: "",
            name: "",
            surname: "",
            password: "",
            repeatPassword: "",
            email: ""
        };
    }
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
    formHandler = async (event) => {
        event.preventDefault();
        const username = this.state.username;
        const name = this.state.name;
        const surname = this.state.surname;
        const password = this.state.password;
        const repeatPassword = this.state.repeatPassword;
        const email = this.state.email;
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
        if (!username || !name || !surname || !password || !repeatPassword || !email) {
            errors.push(strings.AllFieldsMustBeFilled);
        }

        if (errors.length == 0) {
            const result = await UserService.register({ username: username, name: name, surname: surname, password: password, repeatPassword: repeatPassword, email: email })
                if (result.length == 0) {
                    this.props.history.push(routes.home.href);
                }
                else {
                    errors = errors.concat(result)
                }
        }
        var errorBox = document.getElementById("registration-errors");
        errorBox.innerHTML = '';
        errors.forEach(function (item, i, errors) {
            errorBox.appendChild(document.createTextNode(item));
            errorBox.appendChild(document.createElement("br"));
        });
    }

  render() {
      return (
        <div className="fullscreen">
        <div className="login">
            <div className="outer">
                <div className="middle">
                    <div className="inner">
                        <div className="login-wr">
                                  <h2 className="sign-title">{strings.SignUpLabel}</h2>
                            <div id="registration-errors" className="text-danger"></div>
                                  <form onSubmit={this.formHandler} className="sign-form">
                                      <input className="sign-input" value={this.state.username} onChange={this.usernameHandler} placeholder={strings.username} />
                                      <input className="sign-input" value={this.state.name} onChange={this.nameHandler} placeholder={strings.name} />
                                      <input className="sign-input" value={this.state.surname} onChange={this.surnameHandler} placeholder={strings.surname} />
                                      <input className="sign-input" type="password" value={this.state.password} onChange={this.passwordHandler} placeholder={strings.password} />
                                      <input className="sign-input" type="password" value={this.state.repeatPassword} onChange={this.repeatPasswordHandler} placeholder={strings.repeatPassword} />
                                      <input className="sign-input" value={this.state.email} onChange={this.emailHandler} placeholder={strings.email} />
                                      <button className="sign-button" type="submit">{strings.signUp}</button>
                                      <a className="register-link" href={routes.home.href}><p>{strings.BackToHome}</p></a>
                            </form>
                        </div>
                    </div>

                </div>
                  </div>
              </div>
          </div>
    );
  }
}
