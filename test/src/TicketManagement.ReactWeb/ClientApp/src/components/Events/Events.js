import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { Layout } from '../Shared/Layout';
import { EventService, RedirectService } from '../../services';
import { routes } from "../../constants";

export class Events extends Component {
    static displayName = Events.name;

  constructor(props) {
    super(props);
    this.state = { events: [], loading: true };
  }

    async componentDidMount() {
        const data = await EventService.getEvents();
        this.setState({ events: data, loading: false });
  }

  static renderEvents(events) {
      return (
        <Layout>
        <div className="all-container">
              <div className="container-item">
                  <h1>{strings.Events}</h1>
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
        : Events.renderEvents(this.state.events);

    return (
      <div>
        {contents}
      </div>
    );
  }
}
