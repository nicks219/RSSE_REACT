import { LoginRequired } from "./login";//

export class Loader {
    // настройки для прода:
    static credos: "omit" | "same-origin" | "include" = "same-origin"; 
    static corsAddress: string = ""; 
    
    static ifDevelopment() {
        // console.log(process.env.NODE_ENV);
        if (process.env.NODE_ENV === "development") 
        {
            this.credos = "include";
            // куки чувствительны к Origin ('localhost' и '127.0.0.1' это разные значения), только для разработки:
            this.corsAddress = "http://localhost:5000";
        }
    }


    //GET request: /api/controller
    static getData(component: any, url: any) {
        Loader.ifDevelopment();
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

    // GET request: /api/controller?id=
    static getDataById(component: any, requestId: any, url: any) {
        Loader.ifDevelopment();
        LoginRequired.MessageOff();
        
        url = this.corsAddress + url;
        
        try {
            window.fetch(url + "?id=" + String(requestId), {
                credentials: this.credos
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => { if (component.mounted) component.setState({ data }) })
                .catch((e) => LoginRequired.MessageOn(component));
        } catch (err) {
            console.log("Loader try-catch: 2");
        }
    }

    // POST request: /api/controller
    static postData(component: any, requestBody: any, url: any) {
        // ПРОБЛЕМА: при пустых areChecked чекбоксах внешний вид компонента <Сheckboxes> не менялся (после "ошибки" POST)
        // при этом все данные были  правильные и рендеринг/обновление проходили успешно (в компоненте <UpdateView>)
        // РЕШЕНИЕ: уникальный key <Checkbox key={"checkbox " + i + this.state.time} ...>
        Loader.ifDevelopment();
        let time = String(Date.now());
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

    // DELETE request: /api/controller?id=
    static deleteDataById(component: any, requestId: any, url: any, pageNumber: any) {
        Loader.ifDevelopment();
        LoginRequired.MessageOff();
        
        url = this.corsAddress + url;
        
        try {
            window.fetch(url + "?id=" + String(requestId) + "&pg=" + String(pageNumber), {
                method: "DELETE",
                credentials: this.credos
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => { if (component.mounted) component.setState({ data }) })
                .catch((e) => LoginRequired.MessageOn(component));
        } catch (err) {
            console.log("Loader try-catch: 2");
        }
    }
}