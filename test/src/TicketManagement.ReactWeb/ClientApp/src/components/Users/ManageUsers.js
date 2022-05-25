import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { Layout } from '../Shared/Layout';
import { UserService } from '../../services';
import { routes, roles } from '../../constants';

export class ManageUsers extends Component {
    static displayName = ManageUsers.name;

    constructor(props) {
        super(props);
        this.state = { users: [], loading: true };
    }

    async componentDidMount() {
        const data = await UserService.getUsers();
        this.setState({ users: data, loading: false });
    }

    async deleteHandler(event, id) {
        event.preventDefault();
        const result = await UserService.deleteUser(id);
        if (!result) {
            var errorBox = document.getElementById("delete-errors");
            errorBox.innerHTML = '';
            errorBox.appendChild(document.createTextNode(strings.CouldNotDeleteUser));
        }
        else {
            window.location.reload();
        }
    }

    async deleteRoleHandler(event, id, role) {
        event.preventDefault();
        await UserService.deleteRole(id, role);
        window.location.reload();
    }

    async addRoleHandler(event, id, role) {
        event.preventDefault();
        await UserService.addRole(id, role);
        window.location.reload();
    }

    renderUsers(users) {
        return (
            <Layout>
                <div className="all-container">
                    <div className="container">
                        <div className="container-item">
                            <h1>{strings.UsersManagement}</h1>
                        </div>
                        <div className="container-item">
                            <a href={routes.createUser.href} className="crud-button">{strings.Create}</a>
                        </div>
                        <div id="delete-errors" className="text-danger"></div>
                        <div className="container-item">
                            {users.length > 0 ?
                                <table className="table">
                                    <thead>
                                        <tr>
                                            <th>{strings.Username}</th>
                                            <th>{strings.Name}</th>
                                            <th>{strings.Surname}</th>
                                            <th>{strings.Email}</th>
                                            <th>{strings.Roles}</th>
                                            <th>{strings.TimeZone}</th>
                                            <td />
                                        </tr>
                                    </thead>
                                    <tbody>

                                        {users.map(item =>
                                        
                                            <tr>
                                                <td>{item.username}</td>
                                                <td>{item.name}</td>
                                                <td>{item.surname}</td>
                                                <td>{item.email}</td>
                                                <td>
                                                    <div className="role-button-container">
                                                        {item.roles.includes(roles.VenueManager) ? 

                                                            <button className="role-button" onClick={(e) => this.deleteRoleHandler(e, item.id, roles.VenueManager)}>{strings.DeleteVenueManager}</button>
                                                            :
                                                            <button className="role-button not-exist-role" onClick={(e) => this.addRoleHandler(e, item.id, roles.VenueManager)}>{strings.AddVenueManager}</button>
                                                                
                                                        }
                                                        {item.roles.includes(roles.User) ?
                                                            <button className="role-button" onClick={(e) => this.deleteRoleHandler(e, item.id, roles.User)}>{strings.DeleteUser}</button>
                                                            :
                                                            <button className="role-button not-exist-role" onClick={(e) => this.addRoleHandler(e, item.id, roles.User)}>{strings.AddUser}</button>
                                                        }
                                                        {item.roles.includes(roles.EventManager) ?
                                                            <button className="role-button" onClick={(e) => this.deleteRoleHandler(e, item.id, roles.EventManager)}>{strings.DeleteEventManager}</button>
                                                            :
                                                            <button className="role-button not-exist-role" onClick={(e) => this.addRoleHandler(e, item.id, roles.EventManager)}>{strings.AddEventManager}</button>
                                                        }
                                                    </div>
                                                </td>
                                                <td>{item.timeZoneId}</td>
                                                <td>
                                                    <button className="crud-button" onClick={(e) => this.deleteHandler(e, item.id)}>{strings.Delete}</button>
                                                </td>
                                            </tr>
                                        )}
                                    </tbody>
                                </table>
                                :
                                <h3>{strings.NoUsers}</h3>}
                        </div>
                    </div>
                </div>
            </Layout>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>{strings.Loading}...</em></p>
            : this.renderUsers(this.state.users);

        return (
                <>
                {contents}
                </>
        );
    }
}