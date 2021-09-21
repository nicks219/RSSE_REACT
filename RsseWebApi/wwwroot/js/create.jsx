import { Loader } from "./loader.jsx?v=1";

class CreateView extends React.Component {
    constructor(props) {
        super(props);
        this.state = { data: null };
        this.url = "/api/create";
        this.formId = null;
        this.mounted = true;
    }

    componentDidMount() {
        this.formId = ReactDOM.findDOMNode(this.refs.mainForm);
        Loader.getData(this, this.url);
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    componentDidUpdate() {
        if (this.state.data) var id = Number(this.state.data.savedTextId);
        if (id !== 0) window.textId = id;
    }

    render() {
        var checkboxes = [];
        if (this.state.data != null) {
            for (var i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push(<Checkbox key={`checkbox ${i}${this.state.time}`} id={i} jsonStorage={this.state.data} />);
            }
        }

        var jsonStorage = this.state.data;
        if (jsonStorage) {
            if (!jsonStorage.textCS) jsonStorage.textCS = "";
            if (!jsonStorage.titleCS) jsonStorage.titleCS = "";
        }

        return (
            <div>
                <form ref="mainForm" id="dizzy">
                    {checkboxes}
                    {this.state.data != null &&
                        <SubmitButton listener={this} formId={this.formId} />
                    }
                </form>
                {this.state.data != null &&
                    <Message formId={this.formId} jsonStorage={jsonStorage} />
                }
            </div>
        );
    }
}

class Checkbox extends React.Component {

    render() {
        var checked = this.props.jsonStorage.isGenreCheckedCS[this.props.id] === "checked" ? "checked" : "";
        var getGenreName = (i) => { return this.props.jsonStorage.genresNamesCS[i]; };
        return (
            <div id="checkboxStyle">
                <input name="chkButton" value={this.props.id} type="checkbox" id={this.props.id} className="regular-checkbox"
                    defaultChecked={checked} />
                <label htmlFor={this.props.id}>{getGenreName(this.props.id)}</label>
            </div>
        );
    }
}

class CheckboxBts extends React.Component {

    render() {
        var checked = this.props.jsonStorage.isGenreCheckedCS[this.props.id] === "checked" ? "checked" : "";
        var getGenreName = (i) => { return this.props.jsonStorage.genresNamesCS[i]; };
        return (
            //нужен site.css
            <label className="checkbox-btn">
                    <input name="chkButton" value={this.props.id} type="checkbox" id={this.props.id}
                    defaultChecked = { checked } />
                    <span style={{ lineHeight: 30 + 'px' }}>{getGenreName(this.props.id)}</span>
            </label>
        );
    }
}

class Message extends React.Component {
    // ESlint пометил бесполезным
    //constructor(props) {
    //    super(props);
    //}

    inputText = (e) => {
        const newText = e.target.value;
        this.props.jsonStorage.textCS = newText;
        this.forceUpdate();
    }

    inputTitle = (e) => {
        const newText = e.target.value;
        this.props.jsonStorage.titleCS = newText;
        this.forceUpdate();
    }

    render() {
        return (
            <div >
                <p />
                {this.props.jsonStorage != null ?
                    <div>
                        <h5>
                            <textarea name="ttl" cols="66" rows="1" form="dizzy"
                                value={this.props.jsonStorage.titleCS} onChange={this.inputTitle} />
                        </h5>
                        <h5>
                            <textarea name="msg" cols="66" rows="30" form="dizzy"
                                value={this.props.jsonStorage.textCS} onChange={this.inputText} />
                        </h5>
                    </div>
                    : "loading.."}
            </div>
        );
    }
}

class SubmitButton extends React.Component {
    constructor(props) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = "/api/create";
    }

    submit(e) {
        e.preventDefault();
        var formData = new FormData(this.props.formId);
        var checkboxesArray = (formData.getAll('chkButton')).map(a => Number(a) + 1);
        var formMessage = formData.get('msg');
        var formTitle = formData.get('ttl');
        const item = {
            CheckedCheckboxesJS: checkboxesArray,
            TextJS: formMessage,
            TitleJS: formTitle
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
                <label htmlFor="submitButton">Создать</label>
            </div>
        );
    }
}

export default CreateView;