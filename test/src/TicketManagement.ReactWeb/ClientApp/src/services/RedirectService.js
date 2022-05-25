import { routes, claims } from "./../constants";

class RedirectService {

    prepareLink(path, params = []) {
        const lang = localStorage.getItem("lang") ?? "en";
        const token = localStorage.getItem("token");
        path += `?lang=${lang}`;
        if (token) {
            path += `&token=${token}`;
        }
        params.forEach(function (item, index, array) {
            path += `&${item.name}=${item.value}`
        });
        return path;
    }
}

export default new RedirectService();
