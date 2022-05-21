import * as React from 'react';
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
    formId: any;
    mounted: boolean;

    public state: IState = {
        data: null,
        // используй реальное время, более корректно для key
        time: null
    }

    mainForm: React.RefObject<HTMLFormElement>;

    constructor(props: any) {
        super(props);
        this.formId = null;
        this.mounted = true;

        this.mainForm = React.createRef();
    }

    componentDidMount() {
        this.formId = this.mainForm.current;
        Loader.getData(this, Loader.createUrl);
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

        let jsonStorage = this.state.data;
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
    requestBody: any;
    storage: string[] = [];
    state: any;
    btn: any;
    
    constructor(props: any) {
        super(props);
        this.submit = this.submit.bind(this);
        this.state = 0;
    }

    cancel = (e: any) => {
        e.preventDefault();
        this.btn.style.display = "none";
        
        this.requestBody = JSON.stringify({
            "CheckedCheckboxesJS":[1],
            "TextJS":"",
            "TitleJS":""
            });
        Loader.postData(this.props.listener, this.requestBody, Loader.createUrl);
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
            Loader.postData(this.props.listener, this.requestBody, Loader.createUrl);
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
        let promise = this.findSongMatches(formMessage, formTitle);
        await promise;
        
        if (this.storage.length > 0)
        {
            // переключение в "подтверждение или отмена"
            this.btn.style.display = "block";
            this.state = 1;
            return;
        }

        // совпадения не обнаружены
        Loader.postData(this.props.listener, this.requestBody, Loader.createUrl);
    }
    
    findSongMatches = async (formMessage: string | File | null, formTitle: string | File | null) => {
        let promise;

        if (typeof formMessage === "string") {
            formMessage = formMessage.replace(/\r\n|\r|\n/g, " ");
        }
        
        let callback = (data: any) => this.getFoundNames(data);
        let query = "?text=" + formMessage + " " + formTitle;
        
        try {
            promise = Loader.getWithPromise(Loader.findUrl, query, callback);
        } catch (err) {
            console.log("Find when create: try-catch err");
        }

        if (promise !== undefined) {
            await promise;}
    }
    
    getFoundNames = async (res: any) => {
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
            // лучше сделать reject
            if (this.storage.length >= 10) {
                continue;
            }

            let i = String(result[ind][0]);
            
            //  получаем имена возможных совпадений
            let promise;
            
            let callback = (data: any) => this.getTitle(data);
            
            let query = "?id=" + i;
            
            try {
                promise = Loader.getWithPromise(Loader.readTitleUrl, query, callback);
            } catch (err) {
                console.log("Find when create: try-catch err");
            }

            if (promise !== undefined) {
                await promise;
            }
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
        // отменяй подписки и асинхронную загрузку
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