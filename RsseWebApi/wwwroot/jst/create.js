import { Loader } from "/jst/loader.js";

class CreateView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null
        };
        this.url = '/api/create';
        this.formId;
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
        if (id != 0) window.textId = id;
    }

    render() {
        var checkboxes = [];

        if (this.state.data != null) {
            for (var i = 0; i < this.state.data.genresNamesCS.length; i++) {
                checkboxes.push( /*#__PURE__*/React.createElement(Checkbox, {
                    key: "checkbox " + i + this.state.time,
                    id: i,
                    jsonStorage: this.state.data
                }));
            }
        }

        var jsonStorage = this.state.data;

        if (jsonStorage) {
            if (!jsonStorage.textCS) jsonStorage.textCS = "";
            if (!jsonStorage.titleCS) jsonStorage.titleCS = "";
        }

        return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("form", {
            ref: "mainForm",
            id: "dizzy"
        }, checkboxes, this.state.data != null && /*#__PURE__*/React.createElement(SubmitButton, {
            listener: this,
            formId: this.formId
        })), this.state.data != null && /*#__PURE__*/React.createElement(Message, {
            formId: this.formId,
            jsonStorage: jsonStorage
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

class CheckboxBts extends React.Component {
    render() {
        var checked = this.props.jsonStorage.isGenreCheckedCS[this.props.id] == "checked" ? "checked" : "";

        var getGenreName = i => {
            return this.props.jsonStorage.genresNamesCS[i];
        };

        return (
            /*#__PURE__*/
            //нужен site.css
            React.createElement("label", {
                className: "checkbox-btn"
            }, /*#__PURE__*/React.createElement("input", {
                name: "chkButton",
                value: this.props.id,
                type: "checkbox",
                id: this.props.id,
                defaultChecked: checked
            }), /*#__PURE__*/React.createElement("span", {
                style: {
                    lineHeight: 30 + 'px'
                }
            }, getGenreName(this.props.id)))
        );
    }

}

class Message extends React.Component {
    constructor(props) {
        super(props);
    }

    inputText = e => {
        const newText = e.target.value;
        this.props.jsonStorage.textCS = newText;
        this.forceUpdate();
    };
    inputTitle = e => {
        const newText = e.target.value;
        this.props.jsonStorage.titleCS = newText;
        this.forceUpdate();
    };

    render() {
        return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("p", null), this.props.jsonStorage != null ? /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("h5", null, /*#__PURE__*/React.createElement("textarea", {
            name: "ttl",
            cols: "66",
            rows: "1",
            form: "dizzy",
            value: this.props.jsonStorage.titleCS,
            onChange: this.inputTitle
        })), /*#__PURE__*/React.createElement("h5", null, /*#__PURE__*/React.createElement("textarea", {
            name: "msg",
            cols: "66",
            rows: "30",
            form: "dizzy",
            value: this.props.jsonStorage.textCS,
            onChange: this.inputText
        }))) : "loading..");
    }

}

class SubmitButton extends React.Component {
    constructor(props) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = '/api/create';
    }

    submit(e) {
        e.preventDefault();
        var formData = new FormData(this.props.formId);
        var checkboxesArray = formData.getAll('chkButton').map(a => Number(a) + 1);
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
        }, "\u0421\u043E\u0437\u0434\u0430\u0442\u044C"));
    }

}

export default CreateView;