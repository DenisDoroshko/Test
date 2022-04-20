import React, { Component } from 'react';
import { routes } from "../../constants";
import { strings } from '../../localization.js'
import { UserService } from '../../services'

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }
    usernameHandler = (event) => {
        this.setState({ username: event.target.value });
    }

    passwordHandler = (event) => {
        this.setState({ password: event.target.value });
    }
    formHandler = (event) => {
        event.preventDefault();
        const username = this.state.username;
        const password = this.state.password;
        if (username && password) {
            UserService.login(username, password).then(result => {
                if (result) {
                    this.props.history.push(routes.home.href);
                }
                else {
                    var errorBox = document.getElementById("login-errors");
                    errorBox.innerHTML = '';
                    var content = document.createTextNode(strings.SomethingWrong);
                    errorBox.appendChild(content);
                }

                this.setState({ username: "", password: "" });
            });
        }
    }

  render() {
      return (
        <div className="fullscreen">
        <div className="login">
            <div className="outer">
                <div className="middle">
                    <div className="inner">
                        <div className="login-wr">
                                  <h2 className="sign-title">{strings.SignInLabel}</h2>
                            <div id="login-errors" className="text-danger"></div>
                                  <form onSubmit={this.formHandler} className="sign-form">
                                      <input className="sign-input" value={this.state.username} onChange={this.usernameHandler} placeholder={strings.username} />
                                      <input className="sign-input" type="password" value={this.state.password} onChange={this.passwordHandler} placeholder={strings.password} />
                                      <a className="register-link" href={routes.registration.href}><p className="register-label">{strings.DontHaveAccount}</p></a>
                                      <button className="sign-button" type="submit">{strings.SignIn}</button>
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
