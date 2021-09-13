import { Loader } from "/jst/loader.js";

class CatalogView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: null
        };
        this.url = '/api/catalog';
        this.mounted = true;
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    componentDidMount() {
        Loader.getDataById(this, 1, this.url);
    }

    click = e => {
        e.preventDefault();
        var target = Number(e.target.id.slice(7));
        const item = {
            pageNumber: this.state.data.pageNumber,
            navigationButtons: [target]
        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this, requestBody, this.url);
    };
    redirect = e => {
        e.preventDefault();
        var id = Number(e.target.id);
        this.props.listener.setState({
            id: id
        });
    };

    render() {
        if (!this.state.data) return null;
        var songs = [];

        for (let i = 0; i < this.state.data.titlesAndIds.length; i++) {
            songs.push( /*#__PURE__*/React.createElement("tr", {
                key: "song " + i,
                className: "d-sm-table-row "
            }, /*#__PURE__*/React.createElement("td", null), /*#__PURE__*/React.createElement("td", null, /*#__PURE__*/React.createElement("a", {
                id: this.state.data.titlesAndIds[i].item2,
                onClick: this.redirect
            }, this.state.data.titlesAndIds[i].item1))));
        }

        return /*#__PURE__*/React.createElement("div", {
            className: "row"
        }, /*#__PURE__*/React.createElement("p", {
            style: {
                marginLeft: 12 + '%'
            }
        }, "\u0412\u0441\u0435\u0433\u043E \u043F\u0435\u0441\u0435\u043D: ", this.state.data.songsCount, " \xA0 \u0421\u0442\u0440\u0430\u043D\u0438\u0446\u0430: ", this.state.data.pageNumber, " \xA0"), /*#__PURE__*/React.createElement("p", null), /*#__PURE__*/React.createElement("p", null), /*#__PURE__*/React.createElement("table", {
            className: "table",
            id: "catalogTable"
        }, /*#__PURE__*/React.createElement("thead", {
            className: "thead-dark "
        }, /*#__PURE__*/React.createElement("tr", null, /*#__PURE__*/React.createElement("th", {
            width: "10%"
        }), /*#__PURE__*/React.createElement("th", {
            width: "80%"
        }, /*#__PURE__*/React.createElement("form", null, /*#__PURE__*/React.createElement("button", {
            id: "js-nav-1",
            className: "btn btn-info",
            onClick: this.click
        }, "<\u041D\u0430\u0437\u0430\u0434"), "\xA0", /*#__PURE__*/React.createElement("button", {
            id: "js-nav-2",
            className: "btn btn-info",
            onClick: this.click
        }, "\u0412\u043F\u0435\u0440\u0451\u0434>"))), /*#__PURE__*/React.createElement("th", {
            width: "10%"
        }))), /*#__PURE__*/React.createElement("tbody", null, songs)));
    }

}

export default CatalogView;