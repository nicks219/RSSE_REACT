import { Loader } from "./loader.jsx";
//document.location.href = "/";

export class LoginRequired {
    // восстанавливаем данные (но не последнее действие), не полученные из-за ошибки авторизации
    static ContinueLoading() {
        var component = window.temp;
        if (component) {
            if (component.url === "/api/update") {
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
        ReactDOM.render(
            <h1>
                LOGIN PLEASE
            </h1>
            , document.querySelector("#loginMessage")
        );
    }
}

export class Login extends React.Component {
    constructor(props) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = "/account/login";
        this.state = { style: "submitStyle" };
        document.getElementById("login").style.display = "block";
    }

    submit(e) {
        e.preventDefault();
        let email = document.getElementById("email").value;
        let password = document.getElementById("password").value;
        window.fetch(this.url + "?email=" + String(email) + "&password=" + String(password), { credentials: "same-origin" })
            .then(response => response.ok ? this.loginOk() : console.log("Login error"));
    }

    loginOk = () => {
        console.log("Login ok");
        this.setState({ style: "submitStyleGreen" });
        LoginRequired.ContinueLoading();
        setTimeout(() => {
            document.getElementById("login").style.display = "none";
        }, 1500);
    }

    componentWillUnmount() {
        // отменяй подписки и асинхронную загрузку
    }

    render() {
        return (
            <div>
                <div id={this.state.style}>
                    <input type="checkbox" id="loginButton" className="regular-checkbox" onClick={this.submit} />
                    <label htmlFor="loginButton">Войти</label>
                </div>
                {/*<div id={this.state.style}>*/}
                &nbsp;&nbsp;&nbsp;&nbsp;
                <span>
                    <input type="text" id="email" name="email" />
                </span>
                {/*<div id={this.state.style}>*/}
                &nbsp;&nbsp;&nbsp;&nbsp;
                <span>
                    <input type="text" id="password" name="password" />
                </span>
            </div>
        );
    }
}

// загрузка без обработки ошибок
                //fetch(component.url + "?id=" + String(window.textId), { credentials: 'same-origin' })
                //    .then(response => response.json())
                //    .then(data => { if (component.mounted) component.setState({ data }) });

//fetch(component.url, { credentials: 'same-origin' })
                //    .then(response => response.json())
                //    .then(data => { if (component.mounted) component.setState({ data }) });