import * as React from 'react';
import * as ReactDOM from 'react-dom';

import { Loader } from "./loader";

interface IState {
    style: any;
}

declare global {
    interface Window { textId: number, temp: any }
}

interface IProps {
    listener: any;
    formId: any;
    jsonStorage: any;
    id: any;
}

export class LoginRequired {
    // восстанавливаем данные (но не последнее действие), не полученные из-за ошибки авторизации
    static ContinueLoading() {
        let component = window.temp;
        if (component) {
            if (component.url === "/api/update") {
                // Loader в случае ошибки вызовет MessageOn()
                Loader.getDataById(component, window.textId, component.url);
            } else if (component.url === "/api/catalog"){
                Loader.getDataById(component, component.state.data.pageNumber, component.url);
            }
            else {
                Loader.getData(component, component.url);
            }
        }
        
        this.MessageOff();
    }

    static MessageOff() {
        (document.getElementById("loginMessage")as HTMLElement).style.display = "none";
    }

    static MessageOn(component: any) {
        window.temp = component;
        (document.getElementById("loginMessage")as HTMLElement).style.display = "block";
        (document.getElementById("login")as HTMLElement).style.display = "block";
        ReactDOM.render(
            <h1>
                LOGIN PLEASE
            </h1>
            , document.querySelector("#loginMessage")
        );
    }
}

export class Login extends React.Component<IProps, IState> {
    url: string;
    corsAddress: string;

    public state: IState = {
        style: "submitStyle"
    }
    
    private credos: "omit" | "same-origin" | "include";

    constructor(props: any) {
        super(props);
        this.submit = this.submit.bind(this);
        this.url = "/account/login";

        // [TODO]: вынеси в Loader fetch submit: get([query], credos, callback)
        Loader.ifDevelopment(); // по идее этот метод уже должен быть вызван минимум один раз при загрузке главной
        this.credos = Loader.credos;
        this.corsAddress = Loader.corsAddress;
        this.url = this.corsAddress + this.url;
        
        (document.getElementById("login")as HTMLElement).style.display = "block";
    }

    submit(e: any) {
        e.preventDefault();
        let email = "test_e";
        let password = "test_p";
        let emailElement = document.getElementById('email') as HTMLInputElement;
        let passwordElement = document.getElementById('password') as HTMLInputElement;
        if (emailElement) email = emailElement.value;
        if (passwordElement) password = passwordElement.value;

        window.fetch(this.url + "?email=" + String(email) + "&password=" + String(password),
            {credentials: this.credos})
            .then(response => response.ok ? this.loginOk() : this.loginErr());
    }
    
    loginErr = () => {
        // установим локальные куки
        document.cookie = 'rsse_auth = false';
        console.log("Login error");
    }

    loginOk = () => {
        // установим локальные куки
        document.cookie = 'rsse_auth = true';
        console.log("Login ok");
        this.setState({ style: "submitStyleGreen" });
        LoginRequired.ContinueLoading();
        setTimeout(() => {
            (document.getElementById("login")as HTMLElement).style.display = "none";
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

//export default Login;
// загрузка без обработки ошибок
                //fetch(component.url + "?id=" + String(window.textId), { credentials: 'same-origin' })
                //    .then(response => response.json())
                //    .then(data => { if (component.mounted) component.setState({ data }) });

//fetch(component.url, { credentials: 'same-origin' })
                //    .then(response => response.json())
                //    .then(data => { if (component.mounted) component.setState({ data }) });