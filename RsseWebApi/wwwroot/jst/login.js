import { Loader } from "/jst/loader.js"; //document.location.href = "/";

export class LoginRequired {
    // восстанавливаем данные (но не последнее действие), не полученные из-за ошибки авторизации
    static ContinueLoading() {
        var component = window.temp;

        if (component) {
            if (component.url == '/api/update') {
                // Loader в случае ошибки вызовет MessageOn()
                Loader.getDataById(component, window.textId, component.url);
            } else {
                Loader.getData(component, component.url);
            }
        }

        this.MessageOff();
    }

    static MessageOff() {
        document.getElementById("loginMessage").style.display = "none";
    }

    static MessageOn(component) {
        window.temp = component;
        document.getElementById("loginMessage").style.display = "block";
        document.getElementById("login").style.display = "block";
        ReactDOM.render( /*#__PURE__*/React.createElement("h1", null, "LOGIN PLEASE"), document.querySelector("#loginMessage"));
    }

}
export class Login extends React.Component {
    constructor(props) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = '/account/login';
        this.state = {
            style: "submitStyle"
        };
        document.getElementById("login").style.display = "block";
    }

    submit(e) {
        e.preventDefault();
        let email = document.getElementById("email").value;
        let password = document.getElementById("password").value;
        fetch(this.url + "?email=" + String(email) + "&password=" + String(password), {
            credentials: 'same-origin'
        }).then(response => response.ok ? this.loginOk() : console.log('Login error'));
    }

    loginOk = () => {
        console.log('Login ok');
        this.setState({
            style: "submitStyleGreen"
        });
        LoginRequired.ContinueLoading();
        setTimeout(() => {
            document.getElementById("login").style.display = "none";
        }, 1500);
    };

    componentWillUnmount() {// отменяй подписки и асинхронную загрузку
    }

    render() {
        return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
            id: this.state.style
        }, /*#__PURE__*/React.createElement("input", {
            type: "checkbox",
            id: "loginButton",
            className: "regular-checkbox",
            onClick: this.submit
        }), /*#__PURE__*/React.createElement("label", {
            htmlFor: "loginButton"
        }, "\u0412\u043E\u0439\u0442\u0438")), "\xA0\xA0\xA0\xA0", /*#__PURE__*/React.createElement("span", null, /*#__PURE__*/React.createElement("input", {
            type: "text",
            id: "email",
            name: "email"
        })), "\xA0\xA0\xA0\xA0", /*#__PURE__*/React.createElement("span", null, /*#__PURE__*/React.createElement("input", {
            type: "text",
            id: "password",
            name: "password"
        })));
    }

}