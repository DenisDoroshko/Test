import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { EventService } from '../../services';
import { LayoutDetails } from './LayoutDetails';

export class Venues extends Component {
    static displayName = Venues.name;

    constructor(props) {
        super(props);
        this.state = { venues: [], loading: true };
    }

    async componentDidMount() {
        const venues = await EventService.getVenues();
        for (var i = 0; i < venues.length; i++) {
            for (var j = 0; j < venues[i].layouts.length; j++) {
                if (this.props.selectedLayoutId != venues[i].layouts[j].id) {
                    venues[i].layouts[j]["selectStatus"] = "Select";
                }
                else {
                    venues[i].layouts[j]["selectStatus"] = "Selected";
                }
            }
        }
        this.setState({ venues: venues, loading: false });
    }

    renderVenues(venues) {
        return (
            <div className="select-block">
                {venues.length > 0 ?
                    <ul className="venues-list">
                        {venues.map(venue => 
                            <li>
                                <p><a href="/" onClick={(e) => this.flipflop(e, venue.name)}>{venue.name}</a></p>

                                <ul id={venue.name} style={{ display: 'none' }}>
                                    {venue.layouts.map(layout =>
                                        <li className="item">
                                            {layout.name}<br />
                                            <input id={"select" + layout.id} onClick={(e) => this.setLayout(e, layout.id)} className="place-button crud-button" type="button" value={layout.selectStatus} />
                                            <input onClick={(e) => this.flipflop(e, layout.name + layout.id)} className="place-button crud-button" type="button" value="Details" />
                                            <div className="details" id={layout.name + layout.id} style={{ display: 'none' }}><LayoutDetails id={layout.id} /></div>
                                        </li>
                                        )}
                                </ul>
                            </li>
                        )}
                    </ul>
                :
                    <h3>{strings.ThereAreNoVenues}</h3>
                }
            </div>
        );
    }

    setLayout(e, id) {
        const oldValue = this.props.setLayoutId(id);
        let venues = this.state.venues;
        for (var i = 0; i < venues.length; i++) {
            for (var j = 0; j < venues[i].layouts.length; j++) {
                if (id == venues[i].layouts[j].id) {
                    venues[i].layouts[j]["selectStatus"] = "Selected";
                }
                if (oldValue == venues[i].layouts[j].id) {
                    venues[i].layouts[j]["selectStatus"] = "Select";
                }
            }
        }
        this.setState({ venues: venues });
    }

    flipflop(e, id) {
        e.preventDefault();
        let element = document.getElementById(id);
        if (element)
            element.style.display = element.style.display == "none" ? "" : "none";
    }

    render() {
        let contents = this.state.loading
            ? <p><em>{strings.Loading}...</em></p>
            : this.renderVenues(this.state.venues);

        return (
            <div>
                {contents}
            </div>
        );
    }
}
