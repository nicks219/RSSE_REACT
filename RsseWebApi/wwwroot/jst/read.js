import { Loader } from "/jst/loader.js";
export class HomeView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null
        };
        this.url = '/api/read';
        this.formId;
        this.mounted = true;
    }

    componentDidMount() {
        this.formId = ReactDOM.findDOMNode(this.refs.mainForm);
        Loader.getData(this, this.url);
    }

    componentDidUpdate() {
        ReactDOM.render( /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement(SubmitButton, {
            listener: this,
            formId: this.formId
        })), document.querySelector("#searchButton1")); //внешняя зависимость

        document.getElementById("header").style.backgroundColor = "#e9ecee";
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    render() {
        var checkboxes = [];

        if (this.state.data != null) {
            for (var i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push( /*#__PURE__*/React.createElement(Checkbox, {
                    key: "checkbox " + i,
                    id: i,
                    jsonStorage: this.state.data
                }));
            }
        }

        return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("form", {
            ref: "mainForm",
            id: "dizzy"
        }, checkboxes), /*#__PURE__*/React.createElement("div", {
            id: "messageBox"
        }, this.state.data != null && this.state.data.textCS != null && /*#__PURE__*/React.createElement(Message, {
            formId: this.formId,
            jsonStorage: this.state.data
        })));
    }

}

class Checkbox extends React.Component {
    render() {
        var getGenreName = i => {
            return this.props.jsonStorage.genresNamesCS[i];
        };

        return /*#__PURE__*/React.createElement("div", {
            id: "checkboxStyle"
        }, /*#__PURE__*/React.createElement("input", {
            name: "chkButton",
            value: this.props.id,
            type: "checkbox",
            id: this.props.id,
            className: "regular-checkbox",
            defaultChecked: ""
        }), /*#__PURE__*/React.createElement("label", {
            htmlFor: this.props.id
        }, getGenreName(this.props.id)));
    }

}

class Message extends React.Component {
    constructor(props) {
        super(props);
        this.hideMenu = this.hideMenu.bind(this);
    }

    hideMenu(e) {
        if (this.props.formId.style.display != "none") {
            this.props.formId.style.display = "none"; //внешняя зависимость

            document.getElementById("login").style.display = "none";
            return;
        }

        this.props.formId.style.display = "block";
    }

    render() {
        if (this.props.jsonStorage && Number(this.props.jsonStorage.savedTextId) != 0) window.textId = Number(this.props.jsonStorage.savedTextId);
        return /*#__PURE__*/React.createElement("span", null, this.props.jsonStorage != null ? this.props.jsonStorage.textCS != null ? /*#__PURE__*/React.createElement("span", null, /*#__PURE__*/React.createElement("div", {
            id: "songTitle",
            onClick: this.hideMenu
        }, this.props.jsonStorage.titleCS), /*#__PURE__*/React.createElement("div", {
            id: "songBody"
        }, this.props.jsonStorage.textCS)) : "выберите жанр" : "");
    }

}

class SubmitButton extends React.Component {
    constructor(props) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = '/api/read';
    }

    submit(e) {
        e.preventDefault(); //внешняя зависимость

        document.getElementById("login").style.display = "none";
        var formData = new FormData(this.props.formId);
        var checkboxesArray = formData.getAll('chkButton').map(a => Number(a) + 1);
        const item = {
            CheckedCheckboxesJS: checkboxesArray
        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this.props.listener, requestBody, this.url); //внешняя зависимость

        document.getElementById("header").style.backgroundColor = "slategrey"; //#4cff00
    }

    componentWillUnmount() {//отменяй подписки и асинхронную загрузку
    }

    render() {
        return /*#__PURE__*/React.createElement("div", {
            id: "submitStyle"
        }, /*#__PURE__*/React.createElement("input", {
            form: "dizzy",
            type: "checkbox",
            id: "submitButton",
            className: "regular-checkbox",
            onClick: this.submit
        }), /*#__PURE__*/React.createElement("label", {
            htmlFor: "submitButton"
        }, "\u041F\u043E\u0438\u0441\u043A"));
    }

}