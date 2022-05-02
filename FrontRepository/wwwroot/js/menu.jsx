import { HomeView } from "./read.jsx";
import UpdateView from "./update.jsx";
import CreateView from "./create.jsx";
import CatalogView from "./catalog.jsx";
import { Login } from "./login.jsx";

//import React from 'react';
//import ReactDOM from 'react-dom';
//import { HomeView } from "/jst/read.js";
//import UpdateView from "/jst/update.js";
//import CreateView from "/jst/create.js";
//import CatalogView from "/jst/catalog.js";
//import { Login } from "/jst/login.js";

window.textId = 0;
window.temp = null;

class Menu extends React.Component {
    constructor(props) {
        super(props);
        this.select = this.select.bind(this);
        this.visibilityCss = ["act", "pas", "pas", "pas"];
        this.visibilityFlag = [true, false, false, false];
        this.menu = ["Посмотреть", "Поменять", "Создать", "Каталог"];
        this.state = { id: null };
    }

    select(e) {
        var target = Number(e.target.id.slice(5));
        //если "update" до выбора песни
        if (target === 1 && window.textId === 0) return;

        this.visibilityFlag.forEach((value, index) => this.visibilityFlag[index] = false);
        this.visibilityFlag[target] = !this.visibilityFlag[target];
        //this.setState(this.state)
        this.forceUpdate();
    }

    componentDidUpdate() {
        if (this.state.id) this.setState({ id: null });
        if (!this.visibilityFlag[0]) {
            // костыль, убирает кнопку "Поиск"
            ReactDOM.render(
                <div>
                </div>,
                document.querySelector("#searchButton1")
            );
        }
    }

    render() {
        // костыль - переключаемся из catalog на changeText
        if (this.state.id) {
            this.visibilityFlag = [false, true, false, false];
            window.textId = this.state.id;
        }

        this.visibilityFlag.forEach((value, index) => this.visibilityFlag[index] ?
            this.visibilityCss[index] = "act" : this.visibilityCss[index] = "pas");
        var buttons = [];
        for (let i = 0; i < 4; i++) {
            buttons.push(<button key={"menu " + i} onClick={this.select} id={"menu " + String(i)}
                className={this.visibilityCss[i]}
                //className="btn btn-info" style={{ margin: 10 + 'px' }}
            >
                {this.menu[i]}</button>);
        }

        //+TODO: не отображать кнопку пока не загрузилось
        //+TODO: менять цвет кнопки при выполнении POST запроса
        //+TODO: зачем view={this.visibilityFlag[0]}
        //TODO: список жанров точно один на всю сессию (кол-во песен в них будет отличаться) - зачем его грузить каждый раз?
        //TODO: экран некрасиво мигает при переключении пунктов меню

        return (
            <div>
                <div id="header">
                    {buttons}
                </div>
                {/*первый пункт меню*/}
                {this.visibilityFlag[0] === true &&
                    <div id="renderContainer1">
                        <HomeView />
                    </div>
                }
                {/*второй пункт меню*/}
                {this.visibilityFlag[1] === true &&
                    <div id="renderContainer">
                    <UpdateView />
                    </div>
                }
                {/*третий пункт меню*/}
                {this.visibilityFlag[2] === true &&
                    <div id="renderContainer">
                    <CreateView />
                    </div>
                }
                {/*четвертый пункт меню*/}
                {this.visibilityFlag[3] === true &&
                    <div id="renderContainer">
                    <CatalogView listener={this}/>
                    </div>
                }
            </div>
        );
    }
}

ReactDOM.render(
    <div>
        <Menu />
    </div>,
    document.querySelector("#renderMenu")
);

ReactDOM.render(
    <Login listener={this} />
    , document.querySelector("#renderLoginForm")
);

export default Menu;