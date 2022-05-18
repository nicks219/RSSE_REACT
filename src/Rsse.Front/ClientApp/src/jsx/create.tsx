﻿import * as React from 'react';
import {Loader} from "./loader";

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

class CreateView extends React.Component<IState> {
    url: string;
    formId: any;
    mounted: boolean;

    public state: IState = {
        data: null,
        time: null // получи реальное время иначе key отвалятся
    }

    mainForm: React.RefObject<HTMLFormElement>;

    constructor(props: any) {
        super(props);
        this.url = "/api/create";
        this.formId = null;
        this.mounted = true;

        this.mainForm = React.createRef();
    }

    componentDidMount() {
        this.formId = this.mainForm.current;
        Loader.getData(this, this.url);
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    componentDidUpdate() {
        let id = 0;
        if (this.state.data) id = Number(this.state.data.savedTextId);
        if (id !== 0) window.textId = id;
    }

    render() {
        let checkboxes = [];
        if (this.state.data != null && this.state.data.genresNamesCS != null) {
            for (let i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push(<Checkbox key={`checkbox ${i}${this.state.time}`} id={i} jsonStorage={this.state.data} listener={null} formId={null}/>);
            }
        }

        var jsonStorage = this.state.data;
        if (jsonStorage) {
            if (!jsonStorage.textCS) jsonStorage.textCS = "";
            if (!jsonStorage.titleCS) jsonStorage.titleCS = "";
        }

        return (
            <div>
                <form ref={this.mainForm}
                    id="dizzy">
                    {checkboxes}
                    {this.state.data != null &&
                        <SubmitButton listener={this} formId={this.formId} id={null} jsonStorage={null}/>
                    }
                </form>
                {this.state.data != null &&
                    <Message formId={this.formId} jsonStorage={jsonStorage} listener={null} id={null}/>
                }
            </div>
        );
    }
}

class Checkbox extends React.Component<IProps> {

    render() {
        let checked = this.props.jsonStorage.isGenreCheckedCS[this.props.id] === "checked";
        let getGenreName = (i: number) => {
            return this.props.jsonStorage.genresNamesCS[i];
        };
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
    inputText = (e: any) => {
        this.props.jsonStorage.textCS = e.target.value;
        this.forceUpdate();
    }

    inputTitle = (e: any) => {
        this.props.jsonStorage.titleCS = e.target.value;
        this.forceUpdate();
    }

    render() {
        return (
            <div >
                <p />
                {this.props.jsonStorage != null ?
                    <div>
                        <h5>
                            <textarea name="ttl" cols={66} rows={1} form="dizzy"
                                value={this.props.jsonStorage.titleCS} onChange={this.inputTitle} />
                        </h5>
                        <h5>
                            <textarea name="msg" cols={66} rows={30} form="dizzy"
                                value={this.props.jsonStorage.textCS} onChange={this.inputText} />
                        </h5>
                    </div>
                    : "loading.."}
            </div>
        );
    }
}

class SubmitButton extends React.Component<IProps> {
    url: string;
    //
    corsAddress: string;
    findUrl: string;
    readUrl: string;
    requestBody: any;
    storage: string[] = [];
    //
    state: any;
    btn: any;

    private credos: "omit" | "same-origin" | "include";
    
    constructor(props: any) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = "/api/create";
        this.state = 0;

        // [TODO]: вынеси в Loader: finder и checkScanResult([query], credos, callback) (await на promise)
        this.credos = Loader.credos;
        this.corsAddress = Loader.corsAddress;
        
        this.findUrl = this.corsAddress + "/api/find";
        this.readUrl = this.corsAddress + "/api/read/title";
    }

    cancel = (e: any) => {
        e.preventDefault();
        this.btn.style.display = "none";
        
        // TODO: костыль чтоб получить [Already Exists] - нужна существующая песня, поправь
        this.requestBody = JSON.stringify({
            "CheckedCheckboxesJS":[1],
            "TextJS":"Stub",
            "TitleJS":"Nirvana - In Bloom" // 
            });
        // "Nirvana - In Bloom";
        Loader.postData(this.props.listener, this.requestBody, this.url);
    }
    
    componentDidMount() {
        this.btn  = (document.getElementById('cancelButton') as HTMLInputElement);
        this.btn.style.display = "none";
    }

    async submit(e: any) {
        e.preventDefault();
        
        this.btn.style.display = "none";
        
        if (this.state === 1)
        {
            // подтверждение
            this.state = 0;
            Loader.postData(this.props.listener, this.requestBody, this.url);
            return;
        }

        let formData = new FormData(this.props.formId);
        let checkboxesArray = (formData.getAll('chkButton')).map(a => Number(a) + 1);
        let formMessage = formData.get('msg');
        let formTitle = formData.get('ttl');
        const item = {
            CheckedCheckboxesJS: checkboxesArray,
            TextJS: formMessage,
            TitleJS: formTitle
        };
        this.requestBody = JSON.stringify(item);
        
        this.storage = [];
        let promise = this.finder(formMessage, formTitle);
        await promise;
        
        if (this.storage.length > 0)
        {
            // переключение в "подтверждение или отбой сохранения"
            this.btn.style.display = "block";
            this.state = 1;
            return;
        }

        // совпадения не обнаружены
        Loader.postData(this.props.listener, this.requestBody, this.url);
    }
    
    finder = async (formMessage: string | File | null, formTitle: string | File | null) => {
        let promise;

        if (typeof formMessage === "string") {
            formMessage = formMessage.replace(/\r\n|\r|\n/g, " ");
        }
        
        try {
            
            promise = window.fetch(this.findUrl + "?text=" + formMessage + " " + formTitle
                , {credentials: this.credos})
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => this.checkScanResult(data));
        } catch (err) {
            console.log("Find when create: try-catch err");
        }

        await promise;
    }
    
    checkScanResult = async (res: any) => {
        let result = [];
        let response = res['res'];
        if (response === undefined) {
            return;
        }

        let array = Object.keys(response).map((key) => [Number(key), response[key]]);
        array.sort(function (a, b) {
            return b[1] - a[1]
        });

        for (let index in array) {
            result.push(array[index]);
        }

        if (result.length === 0) {
            return;
        }

        for (let ind = 0; ind < result.length; ind++) {
            // лучше reject
            if (this.storage.length >= 10) {
                continue;
            }

            let i = String(result[ind][0]);
            
            //  получаем имена возможных совпадений
            let promise;
            try {
                
                promise = window.fetch(this.readUrl + "?id=" + i,
                    {credentials: this.credos})
                    .then(response => response.ok ? response.json() : Promise.reject(response))
                    .then(data => this.getTitle(data));
            } catch (err) {
                console.log("Find when create: try-catch err");
            }

            await promise;
        }
    }
    
    getTitle = (data: any) => {
        this.storage.push((data.res + '\r\n'));

        let time = String(Date.now());
        // stub
        data = {
            "genresNamesCS": this.storage, // this.props.listener.state.data.genresNamesCS, 
            "isGenreCheckedCS": [], 
            "textCS": JSON.parse(this.requestBody).TextJS
        };
        this.props.listener.setState({ data , time });
    }

    componentWillUnmount() {
        //отменяй подписки и асинхронную загрузку
    }

    render() {
        return (
            <div id="submitStyle">
                <input type="checkbox" id="submitButton" className="regular-checkbox" />
                <label htmlFor="submitButton" onClick={this.submit}>Создать</label>
                  <div id="cancelButton">
                    <input type="checkbox" id="submitButton" className="regular-checkbox" />
                    <label htmlFor="submitButton" onClick={this.cancel}>Отменить</label>
                  </div>
            </div>
        );
    }
}

export default CreateView;

/*
class CheckboxBts extends React.Component<IProps> {

    render() {
        let checked = this.props.jsonStorage.isGenreCheckedCS[this.props.id] === "checked";
        let getGenreName = (i: number) => {
            return this.props.jsonStorage.genresNamesCS[i];
        };
        return (
            // нужен site.css
            <label className="checkbox-btn">
                <input name="chkButton" value={this.props.id} type="checkbox" id={this.props.id}
                       defaultChecked = { checked } />
                <span style={{ lineHeight: 30 + 'px' }}>{getGenreName(this.props.id)}</span>
            </label>
        );
    }
}
*/