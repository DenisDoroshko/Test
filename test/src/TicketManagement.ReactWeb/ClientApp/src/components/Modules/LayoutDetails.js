import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { EventService } from '../../services';

export class LayoutDetails extends Component {
    static displayName = LayoutDetails.name;

    constructor(props) {
        super(props);
        this.state = { layout: null, loading: true };
    }

    async componentDidMount() {
        const layout = await EventService.getLayout(this.props.id);
        this.setState({ layout: layout, loading: false });
    }

    renderLayout(layout) {
        return (
            <div className="all-container">
                <div className="layout-container">
                    <div className="container-item">
                        <div className="main-info">
                            <h4 className="text-center">{layout.name}</h4>
                            <h5 className="text-center">{layout.description}</h5>
                        </div>
                    </div>
                </div>
                <div className="header-item">
                    <div className="layout-details-block">
                        <svg className="svgClass" width={layout.width < 65 ? 300 : layout.width * 13} height={layout.height < 20 ? 250 : layout.height * 13}>
                            {layout.areas.map(area =>
                                <g fill={area.color}>
                                    {area.seats.map(seat =>
                                        <circle className="circle" cx={(seat.number + area.coordX - 1) * 10} cy={(seat.row + area.coordY - 1) * 10} r="4" />
                                        )}
                                </g>
                            )}
                        </svg>
                    </div>
                </div>
            </div>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>{strings.Loading}...</em></p>
            : this.renderLayout(this.state.layout);

        return (
            <div>
                {contents}
            </div>
        );
    }
}