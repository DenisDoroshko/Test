import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { Layout } from '../Shared/Layout';
import { EventService, TimeConverter, RedirectService } from '../../services';
import { routes} from '../../constants';

export class ManageEvents extends Component {
    static displayName = ManageEvents.name;

    constructor(props) {
        super(props);
        this.state = { events: [], loading: true };
    }

    async componentDidMount() {
        const data = await EventService.getEvents();
        this.setState({ events: data, loading: false });
    }

    async deleteHandler(event, id) {
        event.preventDefault();
        const result = await EventService.deleteEvent(id);
        if (result.length > 0) {
            var errorBox = document.getElementById("event-errors");
            errorBox.innerHTML = '';
            result.forEach(function (item, i, result) {
                errorBox.appendChild(document.createTextNode(item));
                errorBox.appendChild(document.createElement("br"));
            });
        }
        else {
            window.location.reload();
        }
    }

    fileHandler = (event) => {
        let file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = this.handleReaderLoaded
            reader.readAsText(file)
        }
    }

    handleReaderLoaded = (readerEvt) => {
        let data = readerEvt.target.result;
        var result = EventService.importEvents(JSON.parse(data));
        if (!result) {
            var errorBox = document.getElementById("event-errors");
            errorBox.innerHTML = '';
            errorBox.appendChild(document.createTextNode(strings.SomethingWrongDuringImport));
        } else {
            window.location.reload();
        }
    }

    renderEvents(events) {
        return (
            <Layout>
                <div className="all-container">
                    <div className="container">
                        <div className="container-item">
                            <h1>{strings.EventsManagement}</h1>
                        </div>
                        <div id="event-errors" className="text-danger"></div>
                        <div className="container-item">
                            <a href={routes.createEvent.href} className="crud-button">{strings.Create}</a>
                                <label class="btn btn-default btn-file crud-button">
                                    {strings.Import} <input type="file" onChange={this.fileHandler} style={{ display: 'none' }} required name="jsonFile" className="file-input" accept="application/JSON" />
                                </label>
                        </div>
                            <div className="container-item">
                                {events.length > 0 ? 
                                    <table className="table">
                                        <thead>
                                            <tr>
                                                <th>{strings.Name}</th>
                                                <th>{strings.Start}</th>
                                                <td />
                                            </tr>
                                        </thead>
                                        <tbody>

                                        {events.map(item =>
                                            <tr>
                                                    <td>{item.name}</td>
                                                    <td>{TimeConverter.convertFromUTC(item.start).toString()}</td>
                                                    <td>
                                                    <div className="button-container">
                                                        <div><a className="crud-button" href={RedirectService.prepareLink(routes.detailsEvent.href, [{ name: "eventId", value: item.id }])}>{strings.Details}</a></div>
                                                            <div><a className="crud-button" href={routes.editEvent.href + `?id=${item.id}`}>{strings.Edit}</a></div>
                                                            <div><button className="crud-button" onClick={(e) => this.deleteHandler(e, item.id)}>{strings.Delete}</button></div>
                                                        </div>
                                                    </td>
                                                </tr>
                                            )}
                                        </tbody>
                                    </table>
                                    : <h3>{strings.ThereNoEvents}</h3>
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
            : this.renderEvents(this.state.events);

        return (
                <>
                {contents}
                </>
        );
    }
}