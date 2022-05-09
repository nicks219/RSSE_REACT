﻿import * as React from 'react';
import { Loader } from "./loader";

interface IState {
    data: any;
    time: any;
}

interface IProps {
    listener: any;
    formId: any;
    jsonStorage: any;
    id: any;
}

class UpdateView extends React.Component<IProps, IState> {
    url: string;
    formId: any;
    mounted: boolean;

    public state: IState = {
        data: null,
        time: null
    }

    mainForm: React.RefObject<HTMLFormElement>;

    constructor(props: any) {
        super(props);
        this.url = '/api/update';
        this.formId = null;
        this.mounted = true;

        this.mainForm = React.createRef();
    }

    componentDidMount() {
        this.formId = this.mainForm.current;
        Loader.getDataById(this, window.textId, this.url);
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    render() {
        var checkboxes = [];
        if (this.state != null && this.state.data != null) {
            for (var i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push(<Checkbox key={"checkbox " + i + this.state.time} id={i} jsonStorage={this.state.data} listener formId/>);
            }
        }

        return (
            <div>
                <form ref={this.mainForm}
                    id="dizzy">
                    {checkboxes}
                    {this.state.data != null &&
                        <SubmitButton listener={this} formId={this.formId} jsonStorage={this.state.data} id/>
                    }
                </form>
                {this.state.data != null && this.state.data.textCS != null &&
                    <Message formId={this.formId} jsonStorage={this.state.data} listener id/>
                }
            </div>
        );
    }
}

class Checkbox extends React.Component<IProps> {
    
    render() {
        var checked = this.props.jsonStorage.isGenreCheckedCS[this.props.id] === "checked" ? true : false;
        var getGenreName = (i: number) => { return this.props.jsonStorage.genresNamesCS[i]; };
        return (
            <div id="checkboxStyle">
                <input name="chkButton" value={this.props.id} type="checkbox" id={this.props.id} className="regular-checkbox"
                    defaultChecked={checked} />
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

    componentDidMount() {
        this.getCookie();
    }

    // name: .AspNetCore.Cookies
    getCookie = () => {
        // выставляются в компоненте Login
        const name = "rsse_auth";
        var matches = document.cookie.match(new RegExp(
            "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
        ));

        // return matches ? decodeURIComponent(matches[1]) : undefined;
        if (matches == null || decodeURIComponent(matches[1]) === 'false')
        {
            this.hideMenu('something_stuff');
        }
    }
    
    // hideMenu() сидит в каждом компоненте !
    hideMenu(e: any) {
        if (this.props.formId.style.display !== "none") {
            this.props.formId.style.display = "none";
            return;
        }
        this.props.formId.style.display = "block";
    }

    inputText = (e: any) => {
        const newText = e.target.value;
        this.props.jsonStorage.textCS = newText;
        this.forceUpdate();
    }

    render() {
        return (
            <div >
                <p />
                {this.props.jsonStorage != null ? (this.props.jsonStorage.textCS != null ?
                    <div>
                        <h1 onClick={this.hideMenu}>
                            {this.props.jsonStorage.titleCS}
                        </h1>
                        <h5>
                            <textarea name="msg" cols={66} rows={30} form="dizzy"
                                value={this.props.jsonStorage.textCS} onChange={this.inputText} />
                        </h5>
                    </div>
                    : "выберите песню")
                    : "loading.."}
            </div>
        );
    }
}

class SubmitButton extends React.Component<IProps> {
    url: string;

    constructor(props: any) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = '/api/update';
    }

    submit(e: any) {
        e.preventDefault();
        var formData = new FormData(this.props.formId);
        var checkboxesArray = (formData.getAll("chkButton")).map(a => Number(a) + 1);
        var formMessage = formData.get("msg");
        const item = {
            CheckedCheckboxesJS: checkboxesArray,
            TextJS: formMessage,
            TitleJS: this.props.jsonStorage.titleCS,
            SavedTextId: window.textId
            //InitialCheckboxes: this.props.jsonStorage.initialCheckboxes.map(a => Number(a))
        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this.props.listener, requestBody, this.url);
    }

    componentWillUnmount() {
        //отменяй подписки и асинхронную загрузку
    }

    render() {
        return (
            <div id="submitStyle">
                <input type="checkbox" id="submitButton" className="regular-checkbox" onClick={this.submit} />
                <label htmlFor="submitButton">Сохранить</label>
            </div>
        );
    }
}

export default UpdateView;