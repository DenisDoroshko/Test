import { routes, claims } from "../constants";
import moment from 'moment'
import { UserService } from '.'

class TimeConverter {

    convertToUTC(localTime) {
        const userTimeZone = this.getUserTimeZone();
        let formatedTime = moment(localTime).format("YYYY-MM-DD hh:mm");
        return moment.tz(formatedTime, userTimeZone).utc().format();
    }

    convertFromUTC(utcTime, asDate = false) {
        const userTimeZone = this.getUserTimeZone();
        let formatedTime = moment(utcTime).format("YYYY-MM-DD hh:mm");
        let localTime = moment.utc(formatedTime).tz(userTimeZone);
        var lang = localStorage.getItem("lang") ?? "en";
        if (asDate === true) {
            return localTime;
        }
        return lang === "en" ? localTime.format("MM/DD/YYYY h:mmA") : localTime.format("DD.MM.YYYY hh:mm");
    }

    getUserTimeZone() {
        return UserService.currentUser()?.timeZone ?? moment.tz.guess();
    }
}

export default new TimeConverter();