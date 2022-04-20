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

export class CreateEvent extends Component {
    static displayName = CreateEvent.name;

    constructor(props) {
        super(props);
        this.state = {
            name: "",
            description: "",
            start: new Date(),
            finish: new Date(),
            startUTC: "",
            finishUTC: "",
            layoutId: "",
            image: ""
        };
    }

    renderEvent() {
        return (
            <Layout>
                <div className="all-container">
                    <div className="header-item">
                        <h1>{strings.EventCreating}</h1>
                    </div>
                    <div className="header-item">
                        <div className="error-box" id="create-event-errors"></div>
                        <form onSubmit={this.formHandler} className="form">
                            <label>{strings.Name}</label><br />
                            <input value={this.state.name} onChange={this.nameHandler} className="form-box" maxLength="120" /><br />
                            <label>{strings.Description}</label><br />
                            <textarea value={this.state.description} onChange={this.descriptionHandler} className="form-box" /><br />
                            <label>{strings.Start}</label><br />
                            <DateTimePicker locale={lang} value={new Date(this.state.start)} onChange={this.startHandler} /><br />
                            <label>{strings.Finish}</label><br />
                            <DateTimePicker locale={lang} value={new Date(this.state.finish)} onChange={(date) => this.finishHandler(date)} /><br />
                            <div className="create-button-box">
                                <label className="btn btn-default btn-file crud-button choose-button">
                                    {strings.Choose} <input onChange={this.fileHandler} type="file" style={{ display: 'none' }} required name="image" className="file-input" accept="image/png, image/jpeg" id="file" />
                                </label>
                            </div>
                            <div className="create-button-box"><button id="submit-form-button" type="submit" className="crud-button submit-button">{strings.Submit}</button></div>
                        </form>
                        <Venues selectedLayoutId="0" setLayoutId={this.setLayoutId.bind(this)} />
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

    handleReaderLoaded = (readerEvt) => {
        let binaryString = readerEvt.target.result;
        this.setState({ image: encode(binaryString)})
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
            const result = await EventService.createEvent({ name: name, description: description, start: start, finish: finish, layoutId: layoutId, image: image });
            if (result.length === 0) {
                this.props.history.push(routes.manageEvents.href);
                return;
            }
            else {
                errors = errors.concat(result)
            }
        }
        var errorBox = document.getElementById("create-event-errors");
        errorBox.innerHTML = '';
        errors.forEach(function (item, i, errors) {
            errorBox.appendChild(document.createTextNode(item));
            errorBox.appendChild(document.createElement("br"));
        });
    }

    render() {
        let contents = this.state.loading
            ? <p><em>{strings.Loading}...</em></p>
            : this.renderEvent();

        return (
            <>
                {contents}
            </>
        );
    }
}