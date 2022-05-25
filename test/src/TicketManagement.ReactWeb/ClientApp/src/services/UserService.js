import jwt_decode from "jwt-decode";
import { routes, claims, roles } from "./../constants";

class UserService {

    async login(username, password) {
        let result = true;
        try {
            await fetch(routes.loginApi.href, {
                method: "POST",
                headers: this.getHeaders(),
                body: JSON.stringify({ Username: username, Password: password })
            })
                .then(async resp => {
                    const data = await resp.text();
                    if (resp.ok && data) {
                        localStorage.setItem("token", data);
                        const userData = jwt_decode(data);
                        const user = {
                            id : userData[`${claims.nameIdentifier}`],
                            role : userData[`${claims.role}`]
                        }
                        localStorage.setItem("user", JSON.stringify(user));
                    } else {
                        result = false;
                    }
                })
                .then((error) => {
                    if (error) {
                        result = false;
                    }
                })
        } catch {
            result = false;
        }
        return result;
    }

    async changePassword(oldPassword, newPassword) {
        let result = false;
        try {
            const user = JSON.parse(localStorage.getItem("user"));
            await fetch(routes.changePasswordApi.href, {
                method: "POST",
                headers: this.getHeaders(),
                body: JSON.stringify({id: user.id, oldPassword: oldPassword, newPassword: newPassword })
            })
                .then(resp => {
                    if (resp.ok) {
                        result = true;
                    }
                })
        } catch {
        }
        return result;
    }

    async register(registration) {
        let errors = [];
        try {
            await fetch(routes.registrationApi.href, {
                method: "POST",
                headers: this.getHeaders(),
                body: JSON.stringify(registration)
            })
                .then(async resp => {
                    const data = JSON.parse(await resp.text());
                    if (resp.ok && data) {
                        localStorage.setItem("token", data.result);
                        const userData = jwt_decode(data.result);
                        const user = {
                            id: userData[`${claims.nameIdentifier}`],
                            role: userData[`${claims.role}`]
                        }
                        localStorage.setItem("user", JSON.stringify(user));
                    } else {
                        errors = data.errors
                    }
                })
        } catch {
        }
        return errors;
    }

    async createUser(user) {
        let errors = [];
        try {
            await fetch(routes.createUserApi.href, {
                method: "POST",
                headers: this.getHeaders(),
                body: JSON.stringify(user)
            })
                .then(async resp => {
                    if (!resp.ok) {
                        const data = JSON.parse(await resp.text());
                        errors = data.errors
                    }
                })
        } catch {
        }
        return errors;
    }

    logout() {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
    }

    isAuthorized() {
        const user = JSON.parse(localStorage.getItem("user"));
        if (user) {
            return true;
        }

        return false;
    }

    isInRole(role) {
        const user = JSON.parse(localStorage.getItem("user"));
        if (user) {
            if (user.role && user.role.includes(role)) {
                return true;
            }
        }

        return false;
    }

    currentUser() {
        return JSON.parse(localStorage.getItem("user"));
    }

    async getUser(id) {
        try {
            const resp = await fetch(routes.userApi.href+`/${id}`, {
                method: "GET",
                headers: this.getHeaders()
            });
            const data = await resp.json();
            if (resp.ok && data) {
                return data;
            }
        } catch {
        }
        return null;
    }

    async getUsers() {
        try {
            const resp = await fetch(routes.usersApi.href, {
                method: "GET",
                headers: this.getHeaders()
            });
            const data = await resp.json();
            if (resp.ok && data) {
                return data;
            }
        } catch {
        }
        return [];
    }

    async edit(user) {
        let result = false;
        try {
            await fetch(routes.editAccountApi.href, {
                method: "PUT",
                headers: this.getHeaders(),
                body: JSON.stringify(user)
            })
                .then(async resp => {
                    if (resp.ok) {
                        result = true;
                    }
                })
        } catch {
        }
        return result;
    }

    async addRole(id, role) {
        let result = false;
        try {
            await fetch(routes.addRoleApi.href + `/${role}/${id}`, {
                method: "POST",
                headers: this.getHeaders()
            })
                .then(async resp => {
                    if (resp.ok) {
                        const user = JSON.parse(localStorage.getItem("user"));
                        if (user.id === id) {
                            await fetch(routes.refreshTokenApi.href + `?id=${id}`, {
                                method: "GET",
                                headers: this.getHeaders()
                            }).then(async refresh => {
                                if (refresh.ok) {
                                    const token = await refresh.text();
                                    localStorage.setItem("token", token);
                                }
                            });
                        }
                        result = true;
                    }
                })
        } catch {
        }
        return result;
    }

    async deleteRole(id, role) {
        let result = false;
        try {
            const user = JSON.parse(localStorage.getItem("user"));
            if (user.id === id && role === roles.VenueManager) {
                return result;
            }
            await fetch(routes.deleteRoleApi.href + `/${role}/${id}`, {
                method: "DELETE",
                headers: this.getHeaders()
            })
                .then(async resp => {
                    if (resp.ok) {
                        if (user.id === id) {
                            await fetch(routes.refreshTokenApi.href + `?id=${id}`, {
                                method: "GET",
                                headers: this.getHeaders()
                            }).then(async refresh => {
                                if (refresh.ok) {
                                    const token = await refresh.text();
                                    localStorage.setItem("token", token);
                                }
                            });
                        }
                        result = true;
                    }
                })
        } catch {
        }
        return result;
    }

    async deleteUser(id) {
        let result = false;
        try {
            const user = JSON.parse(localStorage.getItem("user"));
            if (user.id !== id) {
                await fetch(routes.deleteUserApi.href + `/${id}`, {
                    method: "DELETE",
                    headers: this.getHeaders()
                })
                    .then(async resp => {
                        if (resp.ok) {
                            result = true;
                        }
                    })
            }
        } catch {
        }
        return result;
    }

    async validateToken() {
        let validationResult = false;
        await fetch(routes.validateApi.href + `?token=${localStorage.getItem("token")}`, {
            method: "GET",
            headers: this.getHeaders()
        }).then(result => {
            if (result.ok) {
                validationResult = true;
            }
        });
        return validationResult;
    }

    getHeaders() {
        var token = localStorage.getItem("token");
        var headers =  {
            'Content-Type': 'application/json',
            'Accept-Language': localStorage.getItem("lang") ?? "en"
        };
        if (token) {
            headers['Authorization'] = `Bearer ${token}`
        }
        return headers;
    }
}

export default new UserService();
