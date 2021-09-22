﻿import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Loader } from "./loader.jsx";

interface IState {
    data: any;
}
interface IProps {
    listener: any;
    formId: any;
    jsonStorage: any;
    id: any;
}

export class HomeView extends React.Component<IState> {
    url: string;
    formId: any;
    mounted: boolean;

    public state: IState = {
        data: null
    }

    mainForm: React.RefObject<HTMLFormElement>;

    constructor(props: any) {
        super(props);
        this.url = '/api/read';
        this.formId = null;
        this.mounted = true;

        this.mainForm = React.createRef();
    }

    componentDidMount() {
        //[Obsolete] this.formId = ReactDOM.findDOMNode(this.refs.mainForm);
        this.formId = this.mainForm.current;
        Loader.getData(this, this.url);
    }

    componentDidUpdate() {
        ReactDOM.render(
            <div>
                <SubmitButton listener={this} formId={this.formId} jsonStorage id/>
            </div>,
            document.querySelector("#searchButton1")
        );
        //внешняя зависимость
        (document.getElementById("header")as HTMLElement).style.backgroundColor = "#e9ecee";//???
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    render() {
        var checkboxes = [];
        if (this.state.data != null) {
            for (var i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push(<Checkbox key={`checkbox ${i}`} id={i} jsonStorage={this.state.data} listener formId/>);
            }
        }

        return (
            <div>
                
                <form ref={this.mainForm}//[Obsolete] ref="mainForm"
                    id="dizzy">
                    {checkboxes}
                </form>
                <div id="messageBox">
                    {this.state.data != null && this.state.data.textCS != null &&
                        <Message formId={this.formId} jsonStorage={this.state.data} listener id/>
                    }
                </div>
            </div>
        );
    }
}

class Checkbox extends React.Component<IProps> {
    render() {
        var getGenreName = (i: number) => { return this.props.jsonStorage.genresNamesCS[i]; };
        return (
            <div id="checkboxStyle">
                <input name="chkButton" value={this.props.id} type="checkbox" id={this.props.id} className="regular-checkbox" defaultChecked={false} />
                <label htmlFor={this.props.id}>{getGenreName(this.props.id)}</label>
            </div>
        );
    }
}

class Message extends React.Component<IProps> {
    constructor(props: any) {
        super(props);
        this.hideMenu = this.hideMenu.bind(this);
    }

    hideMenu(e: any) {
        if (this.props.formId.style.display !== "none") {
            this.props.formId.style.display = "none";
            //внешняя зависимость
            (document.getElementById("login")as HTMLElement).style.display = "none";/////////////
            return;
        }
        this.props.formId.style.display = "block";
    }

    render() {
        if (this.props.jsonStorage && Number(this.props.jsonStorage.savedTextId) !== 0) window.textId = Number(this.props.jsonStorage.savedTextId);

        return (
            <span>
                {this.props.jsonStorage != null ? (this.props.jsonStorage.textCS != null ?
                    <span>
                        <div id="songTitle" onClick={this.hideMenu}>
                            {this.props.jsonStorage.titleCS}
                        </div>
                        <div id="songBody">
                                {this.props.jsonStorage.textCS}
                        </div>
                    </span>
                    : "выберите жанр")
                    : ""}
            </span>
        );
    }
}

class SubmitButton extends React.Component<IProps> {
    url: string;

    constructor(props: any) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = '/api/read';
    }

    submit(e: any) {
        e.preventDefault();
        //внешняя зависимость
        (document.getElementById("login") as HTMLElement).style.display = "none";
        var formData = new FormData(this.props.formId);
        var checkboxesArray = (formData.getAll("chkButton")).map(a => Number(a) + 1);
        const item = {
            CheckedCheckboxesJS: checkboxesArray
        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this.props.listener, requestBody, this.url);
        //внешняя зависимость
        (document.getElementById("header") as HTMLElement).style.backgroundColor = "slategrey";//#4cff00
    }

    componentWillUnmount() {
        //отменяй подписки и асинхронную загрузку
    }

    render() {
        return (
            <div id="submitStyle">
                <input form="dizzy" type="checkbox" id="submitButton" className="regular-checkbox" onClick={this.submit} />
                <label htmlFor="submitButton">Поиск</label>
            </div>
        );
    }
}