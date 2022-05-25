import React, { Component } from 'react';
import { strings } from '../../localization.js'
import { EventService, TimeConverter } from '../../services'
import { routes } from "../../constants";
import { Layout } from '../Shared/Layout';
import { Venues } from '../Modules/Venues'
import DateTimePicker from 'react-datetime-picker';
import { encode } from 'base-64'
let lang = localStorage.getItem("lang") ?? "en";
lang = lang !== "be" ? lang : "ru";

export class EditEvent extends Component {
    static displayName = EditEvent.name;

    constructor(props) {
        super(props);
        this.state = {
            event: null,
            name: "",
            description: "",
            start: new Date(),
            finish: new Date(),
            startUTC: "",
            finishUTC: "",
            layoutId: "",
            image: "/default.jpg",
            loading: true
        };
    }
    async componentDidMount() {
        let search = window.location.search;
        let params = new URLSearchParams(search);
        let eventId = params.get('id');
        const event = await EventService.getEvent(eventId);
        if (event) {
            this.setState({
                event: event,
                id: event.id,
                name: event.name,
                description: event.description,
                start: TimeConverter.convertFromUTC(event.start, true),
                finish: TimeConverter.convertFromUTC(event.finish, true),
                startUTC: event.start,
                finishUTC: event.finish,
                layoutId: event.layoutId,
                image: event.image ?? "/default.jpg",
                loading: false
            });
        }
    }

    renderEvent(event) {
        return (
            <Layout>
                <div className="all-container">
                    <div className="header-item">
                        <h1>{strings.EventEditing}</h1>
                    </div>
                    <div className="event-info-container">
                        <div className="container-item">
                            <div className="image-container">
                                <img src={this.state.image} width="245" height="356" />
                                <label className="btn btn-default btn-file crud-button">
                                {strings.Browse} <input onChange={this.fileHandler} type="file" style={{ display: 'none' }} required name="image" className="file-input" accept="image/png, image/jpeg" id="file" />
                                </label>
                            </div>
                        </div>
                        <div className="container-item">
                                    <p className="success-message"></p>
                            <div className="error-box" id="edit-event-errors"></div>
                            <form className="form" onSubmit={this.formHandler}>
                                <label>{strings.Name}</label><br />
                                <input value={this.state.name} onChange={this.nameHandler} className="form-box" maxLength="120" /><br />
                                <label>{strings.Description}</label><br />
                                <textarea value={this.state.description} onChange={this.descriptionHandler} className="form-box" /><br />
                                <label>{strings.Start}</label><br />
                                <DateTimePicker locale={lang} value={new Date(this.state.start)} onChange={this.startHandler} /><br />
                                <label>{strings.Finish}</label><br />
                                <DateTimePicker locale={lang} value={new Date(this.state.finish)} onChange={(date) => this.finishHandler(date)} /><br />
                                <div className="submit-box"><button type="submit" className="crud-button edit-button">{strings.Submit}</button></div>
                            </form>
                            {event.eventAreas.every((area) => area.eventSeats.every((seat) => seat.state === 0)) ?

                                <>
                                    <h3>{strings.ChangeLayout}</h3>
                                    <Venues selectedLayoutId={event.layoutId} setLayoutId={this.setLayoutId.bind(this)} />
                                </> : null}
                        </div>
                    </div>
                            <div className="header-item">
                                        <div className="event-details-block">
                            <svg className="svgClass" width={event.width < 65 ? 900 : event.width * 13} height={event.height < 20 ? 600 : event.height * 13}>
                                {event.eventAreas.map(area =>
                                    <g fill={area.color}>
                                        {area.eventSeats.map(seat =>
                                                        seat.state === 0 ?
                                                        <circle className="circle available-circle " cx={(seat.number + area.coordX - 1) * 18} cy={(seat.number + area.coordX - 1) * 18} r="8" />
                                                        :
                                                        <circle className="circle booked-circle" fill="#cdd5d4" cx={(seat.number + area.coordX - 1) * 18} cy={(seat.number + area.coordX - 1) * 18} r="8" />
                                                        )}
                                    </g>
                                )}
                                            </svg>
                                        </div>
                                    </div>
                    <div className="header-item">
                        <h2>{strings.EventAreas}</h2>
                        {event.eventAreas.map(area =>
                            <>
            <h3>{area.description}</h3>
            <div className="event-block">
                <svg className="svgClass" width={area.width < 65 ? 900 : area.width * 13} height={area.height < 40 ? 600 : area.height * 13}>
                    {area.coordX != 0 && area.coordY != 0 ?
                        <g fill={area.color}>
                            {area.eventSeats.map(seat =>
                                <circle className="circle" cx={(seat.number + area.coordX - 1) * 10} cy={(seat.row + area.coordX - 1) * 10} r="4" />
                            )}
                        </g> : null
                    }
                </svg>
            </div>
            <div className="set-form-block">
                <form onSubmit={(e) => this.setPriceHandler(e, area.id)} className="price-form">
                    <input name="price" onChange={(e) => this.priceHandler(e, area.id)} type="text" placeholder={strings.Amount} className="edit-form-box number-input" maxLength="16" value={area.price} />
                    <div className="submit-price"><button type="submit" className="crud-button edit-button">{strings.SetPrice}</button></div>
                </form>
            </div>
                       </> )}
            </div>
            </div>
        </Layout>
        );
    }

    setLayoutId(id) {
        const oldValue = this.state.layoutId;
        this.setState({ layoutId: id });
        return oldValue;
    }

    fileHandler = (event) => {
        let file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = this.handleReaderLoaded
            reader.readAsBinaryString(file)
        }
    }

    handleReaderLoaded = async (readerEvt) => {
        let image = encode(readerEvt.target.result);
        await EventService.attachImage(this.state.id, image);
        this.setState({ image: "data:image/png;base64," + image})
    }

    priceHandler = async (event, areaId) =>  {
        var charCode = (event.which) ? event.which : event.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        let eventItem = this.state.event;
        eventItem.eventAreas.forEach(function (item, index, array) {
            if (item.id === areaId) {
                item.price = event.target.value;
            }
        });
        this.setState({ event: eventItem})
    }

    setPriceHandler = async (event, areaId) => {
        event.preventDefault();
        let price = 0;
        this.state.event.eventAreas.forEach(function (item, index, array) {
            if (item.id === areaId) {
                price = item.price;
            }
        });
        let result = await EventService.setPrice(areaId, price);
    }

    nameHandler = (event) => {
        this.setState({ name: event.target.value });
    }
    descriptionHandler = (event) => {
        this.setState({ description: event.target.value });
    }
    startHandler = (date) => {
        this.setState({ start: date })
        this.setState({ startUTC: TimeConverter.convertToUTC(date)});
    }
    finishHandler = (date) => {
        this.setState({ finish: date})
        this.setState({ finishUTC: TimeConverter.convertToUTC(date) });
    }
    
    formHandler = async (event) => {
        event.preventDefault();
        const id = this.state.id;
        const name = this.state.name;
        const description = this.state.description;
        const start = this.state.startUTC;
        const finish = this.state.finishUTC;
        const layoutId = this.state.layoutId;
        const image = this.state.image;
        let errors = []
        if (!layoutId) {
            errors.push(strings.LayoutForEventMustBeSelected);
        }

        if (!name || !description || !start || !finish) {
            errors.push(strings.AllFieldsMustBeFilled);
        }
        if (errors.length === 0) {
            const result = await EventService.editEvent({id:id, name: name, description: description, start: start, finish: finish, layoutId: layoutId, image: image });
            if (result.length === 0) {
                this.props.history.push(routes.manageEvents.href);
                return;
            }
            else {
                errors = errors.concat(result)
            }
        }
        var errorBox = document.getElementById("edit-event-errors");
        errorBox.innerHTML = '';
        errors.forEach(function (item, i, errors) {
            errorBox.appendChild(document.createTextNode(item));
            errorBox.appendChild(document.createElement("br"));
        });
    }

    render() {
        let contents = this.state.loading
            ? <p><em>{strings.Loading}...</em></p>
            : this.renderEvent(this.state.event);

        return (
            <>
                {contents}
            </>
        );
    }
}