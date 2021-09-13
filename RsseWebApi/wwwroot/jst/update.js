import { Loader } from "/jst/loader.js";

class UpdateView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null
        };
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
                checkboxes.push( /*#__PURE__*/React.createElement(Checkbox, {
                    key: "checkbox " + i + this.state.time,
                    id: i,
                    jsonStorage: this.state.data
                })); //
            } //

        }

        return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("form", {
            ref: "mainForm",
            id: "dizzy"
        }, checkboxes, this.state.data != null && /*#__PURE__*/React.createElement(SubmitButton, {
            listener: this,
            formId: this.formId,
            jsonStorage: this.state.data
        })), this.state.data != null && this.state.data.textCS != null && /*#__PURE__*/React.createElement(Message, {
            formId: this.formId,
            jsonStorage: this.state.data
        }));
    }

}

class Checkbox extends React.Component {
    render() {
        var checked = this.props.jsonStorage.isGenreCheckedCS[this.props.id] == "checked" ? "checked" : "";

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
            defaultChecked: checked
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
            this.props.formId.style.display = "none";
            return;
        }

        this.props.formId.style.display = "block";
    }

    inputText = e => {
        const newText = e.target.value;
        this.props.jsonStorage.textCS = newText;
        this.forceUpdate();
    };

    render() {
        return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("p", null), this.props.jsonStorage != null ? this.props.jsonStorage.textCS != null ? /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("h1", {
            onClick: this.hideMenu
        }, this.props.jsonStorage.titleCS), /*#__PURE__*/React.createElement("h5", null, /*#__PURE__*/React.createElement("textarea", {
            name: "msg",
            cols: "66",
            rows: "30",
            form: "dizzy",
            value: this.props.jsonStorage.textCS,
            onChange: this.inputText
        }))) : "выберите песню" : "loading..");
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
        var checkboxesArray = formData.getAll('chkButton').map(a => Number(a) + 1);
        var formMessage = formData.get('msg');
        const item = {
            CheckedCheckboxesJS: checkboxesArray,
            TextJS: formMessage,
            TitleJS: this.props.jsonStorage.titleCS,
            SavedTextId: window.textId //InitialCheckboxes: this.props.jsonStorage.initialCheckboxes.map(a => Number(a))

        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this.props.listener, requestBody, this.url);
    }

    componentWillUnmount() {//отменяй подписки и асинхронную загрузку
    }

    render() {
        return /*#__PURE__*/React.createElement("div", {
            id: "submitStyle"
        }, /*#__PURE__*/React.createElement("input", {
            type: "checkbox",
            id: "submitButton",
            className: "regular-checkbox",
            onClick: this.submit
        }), /*#__PURE__*/React.createElement("label", {
            htmlFor: "submitButton"
        }, "\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C"));
    }

}

export default UpdateView;