﻿import { LoginRequired } from "./login";//

export class Loader {
    // "same-origin" (100% работает на хостинге) или "include" для запуска на nodejs
    static /*readonly*/ credos: "omit" | "same-origin" | "include" = "same-origin"; 
    static /*readonly*/ corsAddress: string = ""; // для запуска на проде
    // static /*readonly*/ corsAddress: string = "http://localhost:5000"; // для запуска на nodejs
    
    static isEnv() {
        console.log(process.env.NODE_ENV);
        // работает
        if (process.env.NODE_ENV === "development") 
        {
            this.credos = "include";
            // ОПЯТЬ КОСТЫЛИ:
            // 1. креды и адрес не меняются сами по себе, не забывай вызвать метод
            // 2. адрес NodeJs можно указать явно: "scripts": { "set PORT=3000 && set HOST=localhost&& react-scripts start",
            // 3. куки чувствительны к Origin ('localhost' и '127.0.0.1' это разные значения), но это только для разработки
            // т.к. на проде будет "same-origin"
            this.corsAddress = "http://localhost:5000";
            // this.corsAddress = "http://127.0.0.1:5000";
        }
    }


    //GET request: /api/controller
    static getData(component: any, url: any) {
        Loader.isEnv();
        LoginRequired.MessageOff();

        url = this.corsAddress + url;

        try {
            window.fetch(url, {
                credentials: this.credos
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => {
                    if (component.mounted) component.setState({data})
                })
                .catch((e) => LoginRequired.MessageOn(component));//
        } catch (err) {
            console.log("Loader try-catch: 1");
        }
    }

    //GET request: /api/controller?id=
    static getDataById(component: any, requestId: any, url: any) {
        Loader.isEnv();
        LoginRequired.MessageOff();
        
        url = this.corsAddress + url;
        
        try {
            window.fetch(url + "?id=" + String(requestId), {
                //credentials: "same-origin"
                credentials: this.credos
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => { if (component.mounted) component.setState({ data }) })
                .catch((e) => LoginRequired.MessageOn(component));//LogForm(component)
        } catch (err) {
            console.log("Loader try-catch: 2");
        }
    }

    //POST request: /api/controller
    static postData(component: any, requestBody: any, url: any) {
        // ПРОБЛЕМА: при пустых areChecked чекбоксах внешний вид компонента <Сheckboxes> не менялся (после "ошибки" POST)
        // при этом все данные были  правильные и рендеринг/обновление проходили успешно (в компоненте <UpdateView>)
        // РЕШЕНИЕ: уникальный key <Checkbox key={"checkbox " + i + this.state.time} ...>
        Loader.isEnv();
        var time = String(Date.now());
        LoginRequired.MessageOff();
        
        url = this.corsAddress + url;
        
        try {
            window.fetch(url, {
                method: "POST",
                headers: { 'Content-Type': "application/json;charset=utf-8" },
                body: requestBody,
                credentials: this.credos
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => component.setState({ data, time }))
                .catch((e) => LoginRequired.MessageOn(component));
        } catch (err) {
            console.log("Loader try-catch: 3");
        }
    }

    //DELETE request: /api/controller?id=
    static deleteDataById(component: any, requestId: any, url: any, pageNumber: any) {
        Loader.isEnv();
        LoginRequired.MessageOff();
        
        url = this.corsAddress + url;
        
        try {
            window.fetch(url + "?id=" + String(requestId) + "&pg=" + String(pageNumber), {
                method: "DELETE",
                credentials: this.credos
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => { if (component.mounted) component.setState({ data }) })
                .catch((e) => LoginRequired.MessageOn(component));//LogForm(component)
        } catch (err) {
            console.log("Loader try-catch: 2");
        }
    }
}