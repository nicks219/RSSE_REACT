import { Loader } from "./loader.jsx";

class UpdateView extends React.Component {
    constructor(props) {
        super(props);
        this.state = { data: null };
        this.url = '/api/update';
        this.formId;
        this.mounted = true;
    }

    componentDidMount() {
        this.formId = ReactDOM.findDOMNode(this.refs.mainForm);
        Loader.getDataById(this, window.textId, this.url);
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    render() {
        var checkboxes = [];
        if (this.state.data != null) {
            for (var i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push(<Checkbox key={"checkbox " + i + this.state.time} id={i} jsonStorage={this.state.data} />);
            }
        }

        return (
            <div>
                <form ref="mainForm" id="dizzy">
                    {checkboxes}
                    {this.state.data != null &&
                        <SubmitButton listener={this} formId={this.formId} jsonStorage={this.state.data} />
                    }
                </form>
                {this.state.data != null && this.state.data.textCS != null &&
                    <Message formId={this.formId} jsonStorage={this.state.data} />
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

class Message extends React.Component {
    constructor(props) {
        super(props);
        this.hideMenu = this.hideMenu.bind(this);
    }

    hideMenu(e) {
        if (this.props.formId.style.display !== "none") {
            this.props.formId.style.display = "none";
            return;
        }
        this.props.formId.style.display = "block";
    }

    inputText = (e) => {
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
                            <textarea name="msg" cols="66" rows="30" form="dizzy"
                                value={this.props.jsonStorage.textCS} onChange={this.inputText} />
                        </h5>
                    </div>
                    : "выберите песню")
                    : "loading.."}
            </div>
        );
    }
}

class SubmitButton extends React.Component {
    constructor(props) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = '/api/update';
    }

    submit(e) {
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