import { routes } from "../constants";

class EventService {

    async getEvents() {
        try {
            const resp = await fetch(routes.eventsApi.href, {
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

    async getEvent(eventId) {
        try {
            const resp = await fetch(routes.eventApi.href + `/${eventId}`, {
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

    async setPrice(areaId, price) {
        try {
            const resp = await fetch(routes.setPriceApi.href + `/${areaId}/price`, {
                method: "PUT",
                headers: this.getHeaders(),
                body: JSON.stringify({price:price})
            });
            if (resp.ok) {
                return true;
            }
        } catch {
        }
        return false;
    }

    async attachImage(eventId, image) {
        try {
            await fetch(routes.attachImageApi.href + `/${eventId}/image`, {
                method: "PUT",
                headers: this.getHeaders(),
                body: JSON.stringify({image: image})
            })
                .then(async resp => {
                    if (!resp.ok) {
                        return true;
                    }
                })
        } catch {
        }
        return false;
    }

    async getVenues() {
        try {
            const resp = await fetch(routes.venuesApi.href, {
                method: "GET",
                headers: this.getHeaders()
            });
            const data = await resp.json();
            if (resp.status == 200 && data) {
                return data;
            }
        } catch {
        }
        return [];
    }

    async getLayout(id) {
        try {
            const resp = await fetch(routes.layoutApi.href + `/${id}`, {
                method: "GET",
                headers: this.getHeaders()
            });
            const data = await resp.json();
            if (resp.status == 200 && data) {
                return data;
            }
        } catch {
        }
        return null;
    }

    async createEvent(event) {
        let errors = [];
        try {
            await fetch(routes.createEventApi.href, {
                method: "POST",
                headers: this.getHeaders(),
                body: JSON.stringify(event)
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

    async importEvents(events) {
        let result = false;
        try {
            await fetch(routes.importEventsApi.href, {
                method: "POST",
                headers: this.getHeaders(),
                body: JSON.stringify(events)
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

    async deleteEvent(id) {
        let errors = [];
        alert(id);
        try {
            await fetch(routes.deleteEventApi.href + `/${id}`, {
                method: "DELETE",
                headers: this.getHeaders()
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

    async editEvent(event) {
        let errors = [];
        try {
            await fetch(routes.editEventApi.href, {
                method: "PUT",
                headers: this.getHeaders(),
                body: JSON.stringify(event)
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

export default new EventService();
