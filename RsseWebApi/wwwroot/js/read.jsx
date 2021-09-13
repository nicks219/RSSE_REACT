import { Loader } from "./loader.jsx";

export class HomeView extends React.Component {
    constructor(props) {
        super(props);
        this.state = { data: null };
        this.url = '/api/read';
        this.formId;
        this.mounted = true;
    }

    componentDidMount() {
        this.formId = ReactDOM.findDOMNode(this.refs.mainForm);
        Loader.getData(this, this.url);
    }

    componentDidUpdate() {
        ReactDOM.render(
            <div>
                <SubmitButton listener={this} formId={this.formId} />
            </div>,
            document.querySelector("#searchButton1")
        );
        //внешняя зависимость
        document.getElementById("header").style.backgroundColor = "#e9ecee";
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    render() {
        var checkboxes = [];
        if (this.state.data != null) {
            for (var i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push(<Checkbox key={`checkbox ${i}`} id={i} jsonStorage={this.state.data} />);
            }
        }

        return (
            <div>
                
                <form ref="mainForm" id="dizzy">
                    {checkboxes}
                </form>
                <div id="messageBox">
                    {this.state.data != null && this.state.data.textCS != null &&
                        <Message formId={this.formId} jsonStorage={this.state.data} />
                    }
                </div>
            </div>
        );
    }
}

class Checkbox extends React.Component {
    render() {
        var getGenreName = (i) => { return this.props.jsonStorage.genresNamesCS[i]; };
        return (
            <div id="checkboxStyle">
                <input name="chkButton" value={this.props.id} type="checkbox" id={this.props.id} className="regular-checkbox" defaultChecked="" />
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
            //внешняя зависимость
            document.getElementById("login").style.display = "none";
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

class SubmitButton extends React.Component {
    constructor(props) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = '/api/read';
    }

    submit(e) {
        e.preventDefault();
        //внешняя зависимость
        document.getElementById("login").style.display = "none";
        var formData = new FormData(this.props.formId);
        var checkboxesArray = (formData.getAll("chkButton")).map(a => Number(a) + 1);
        const item = {
            CheckedCheckboxesJS: checkboxesArray
        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this.props.listener, requestBody, this.url);
        //внешняя зависимость
        document.getElementById("header").style.backgroundColor = "slategrey";//#4cff00
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

//export default HomeView;