import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { Layout } from '../Shared/Layout';
import { EventService, RedirectService } from '../../services';
import { routes } from "../../constants";

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = { events: [], loading: true };
    }

    async componentDidMount() {
        let data = await EventService.getEvents();
        data.sort(function (a, b) {
            return new Date(a.start) - new Date(b.start);
        });
        if (data.length > 9) {
            data = data.slice(0, 9);
        }
        this.setState({ events: data, loading: false });
    }

    static renderEvents(events) {
        return (
            <Layout>
                <div className="all-container">
                    <div className="container-item">
                        <h1>{strings.UpcomingEvents}</h1>
                    </div>
                    <div className="container-item">
                        <div className="events-container">
                            {
                                events.map(item =>
                                    <div className="event-item">
                                        <a className="title-link" href={RedirectService.prepareLink(routes.detailsEvent.href, [{ name: "eventId", value: item.id }])}>
                                            <img className="scale" src={item.image ?? "/default.jpg"} alt="not found" width="245" height="356"></img><div className="img-text"><p>{item.name}</p></div>
                                        </a>
                                    </div>)
                            }
                        </div>
                    </div>
                </div>
            </Layout>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>{strings.Loading}...</em></p>
            : Home.renderEvents(this.state.events);

        return (
            <div>
                {contents}
            </div>
        );
    }
}