import React, { Component } from 'react';
import { routes, roles } from "../../constants";
import { strings } from '../../localization.js'
import { UserService, RedirectService } from '../../services';

export class Header extends Component {
    static displayName = Header.name;

    componentDidMount() {
        var x, i, j, l, ll, selElmnt, a, b, c, language;

        x = document.getElementsByClassName("language-select");
        l = x.length;
        var select = document.querySelector('#select-lang')
        language = select.value
        for (i = 0; i < l; i++) {
            selElmnt = x[i].getElementsByTagName("select")[0]
            ll = selElmnt.length;

            a = document.createElement("DIV");
            a.setAttribute("class", "select-selected");
            a.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
            x[i].appendChild(a);

            b = document.createElement("DIV");
            b.setAttribute("class", "select-items select-hide");
            for (j = 0; j < ll; j++) {

                c = document.createElement("DIV");
                c.innerHTML = selElmnt.options[j].innerHTML;
                c.addEventListener("click",
                    function (e) {

                        var y, i, k, s, h, sl, yl;
                        s = this.parentNode.parentNode.getElementsByTagName("select")[0];
                        sl = s.length;
                        h = this.parentNode.previousSibling;
                        for (i = 0; i < sl; i++) {
                            if (s.options[i].innerHTML == this.innerHTML) {
                                s.selectedIndex = i;
                                h.innerHTML = this.innerHTML;
                                y = this.parentNode.getElementsByClassName("same-as-selected");
                                yl = y.length;
                                for (k = 0; k < yl; k++) {
                                    y[k].removeAttribute("class");
                                }
                                this.setAttribute("class", "same-as-selected");
                                break;
                            }
                        }
                        h.click();
                    });
                b.appendChild(c);
            }
            x[i].appendChild(b);
            a.addEventListener("click",
                function (e) {

                    e.stopPropagation();
                    closeAllSelect(this);
                    this.nextSibling.classList.toggle("select-hide");
                    this.classList.toggle("select-arrow-active");
                });
        }

        function closeAllSelect(elmnt) {
            var select = document.querySelector('#select-lang')
            if (select && language != select.value) {
                language = select.value;
                localStorage.setItem('lang', language);
                let search = window.location.search;
                let params = new URLSearchParams(search);
                if (params.get('lang')) {
                    params.set('lang', language)
                    window.location.href = `${window.location.origin}${window.location.pathname}?${params}`
                }
                window.location.href = window.location.origin + window.location.pathname;
            }

            var x, y, i, xl, yl, arrNo = [];
            x = document.getElementsByClassName("select-items");
            y = document.getElementsByClassName("select-selected");
            xl = x.length;
            yl = y.length;
            for (i = 0; i < yl; i++) {
                if (elmnt == y[i]) {
                    arrNo.push(i)
                } else {
                    y[i].classList.remove("select-arrow-active");
                }
            }
            for (i = 0; i < xl; i++) {
                if (arrNo.indexOf(i)) {
                    x[i].classList.add("select-hide");
                }
            }
        }

        document.addEventListener("click", closeAllSelect);
    }
    render () {
    return (
        <header>
            <div className="header">
                <div className="main-container">
                    <div className="grid-item">
                            <div className="language-select">
                                <select name="lang" id="select-lang">
                                {localStorage.getItem("lang") === "en" ? < option selected value="en">English</option> : < option value="en">English</option>}
                                {localStorage.getItem("lang") === "ru" ? < option selected value="ru">русский</option> : < option value="ru">русский</option>}
                                {localStorage.getItem("lang") === "be" ? < option selected value="be">беларуская</option> : < option value="be">беларуская</option>}
                                </select>
                            </div>
                    </div>
                    <div className="grid-item">
                        <div className="logo-box">
                            <a href={routes.home.href}><img src="/logo.png" alt="" height="30" width="110" /></a>
                        </div>
                    </div>
                    <div className="grid-item">
                        <ul className="mainbar">
                            <li className="nav-li"><a href={routes.home.href}>{strings.Home}</a></li>
                            <li className="nav-li"><a href={RedirectService.prepareLink(routes.venues.href)}>{strings.Venues}</a></li>
                            <li className="nav-li"><a href={routes.events.href}>{strings.Events}</a></li>
                        </ul>
                    </div>
                    <div className="grid-item nav-sub">
                        {UserService.isInRole(roles.VenueManager) || UserService.isInRole(roles.EventManager) ?
                            <ul className="topmenu ul-sub">
                                <li className="li-sub nav-li">
                                    <a href="">{strings.Tools}{'>'}</a>
                                    <ul className="submenu ul-sub">
                                        {UserService.isInRole(roles.VenueManager) ?
                                            <>
                                                <li className="li-sub"><a className="a-sub" href={RedirectService.prepareLink(routes.manageVenues.href)}>{strings.ManageVenues}</a></li>
                                                <li className="li-sub"><a className="a-sub" href={routes.manageUsers.href}>{strings.ManageUsers}</a></li>
                                            </> : null
                                        }
                                        {UserService.isInRole(roles.EventManager) ?
                                            <li className="li-sub"><a className="a-sub" href={routes.manageEvents.href}>{strings.ManageEvents}</a></li>
                                            : null}
                                    </ul>
                                </li>
                            </ul> : null
                        }
                    </div>
                    <div className="grid-item">
                        <ul className="mainbar">
                            {UserService.isAuthorized() ? <li className="nav-li"><a href={routes.editAccount.href}>{strings.Account}</a></li> : <li className="nav-li"><a href={routes.login.href}>{strings.SignIn}</a></li>}
                            {UserService.isInRole(roles.User) ? <li className="nav-li"><a href={`${routes.cart.href}?lang=${localStorage.getItem("lang")}`}>{strings.Cart}</a></li> : null}
                        </ul>
                    </div>
                </div>
            </div>
        </header>
    );
  }
}
