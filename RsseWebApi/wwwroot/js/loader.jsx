import { LoginRequired } from "./login.jsx";

export class Loader {
    //GET request: /api/controller
    static getData(component, url) {
        LoginRequired.MessageOff();
        try {
            window.fetch(url, {
                //устанавливаем куки
                credentials: "same-origin"
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => { if (component.mounted) component.setState({ data }) })
                .catch((e) => LoginRequired.MessageOn(component));//
        } catch (err) {
            console.log("Loader try-catch: 1");
        }
    }

    //GET request: /api/controller?id=
    static getDataById(component, requestId, url) {
        LoginRequired.MessageOff();
        try {
            window.fetch(url + "?id=" + String(requestId), {
                credentials: "same-origin"
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => { if (component.mounted) component.setState({ data }) })
                .catch((e) => LoginRequired.MessageOn(component));//LogForm(component)
        } catch (err) {
            console.log("Loader try-catch: 2");
        }
    }

    //POST request: /api/controller
    static postData(component, requestBody, url) {
        // ПРОБЛЕМА: при пустых areChecked чекбоксах внешний вид компонента <Сheckboxes> не менялся (после "ошибки" POST)
        // при этом все данные были  правильные и рендеринг/обновление проходили успешно (в компоненте <UpdateView>)
        // РЕШЕНИЕ: уникальный key <Checkbox key={"checkbox " + i + this.state.time} ...>
        var time = String(Date.now());
        LoginRequired.MessageOff();
        try {
            window.fetch(url, {
                method: "POST",
                headers: { 'Content-Type': "application/json;charset=utf-8" },
                body: requestBody,
                credentials: "same-origin"
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => component.setState({ data, time }))
                .catch((e) => LoginRequired.MessageOn(component));
        } catch (err) {
            console.log("Loader try-catch: 3");
        }
    }

    //DELETE request: /api/controller?id=
    static deleteDataById(component, requestId, url, pageNumber) {
        LoginRequired.MessageOff();
        try {
            window.fetch(url + "?id=" + String(requestId) + "&pg=" + String(pageNumber), {
                method: "DELETE",
                credentials: "same-origin"
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(data => { if (component.mounted) component.setState({ data }) })
                .catch((e) => LoginRequired.MessageOn(component));//LogForm(component)
        } catch (err) {
            console.log("Loader try-catch: 2");
        }
    }
}