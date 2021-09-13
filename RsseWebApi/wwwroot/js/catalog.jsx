import { Loader } from "./loader.jsx";

class CatalogView extends React.Component {
    constructor(props) {
        super(props);
        this.state = { data: null };
        this.url = "/api/catalog";
        this.mounted = true;
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    componentDidMount() {
        Loader.getDataById(this, 1, this.url);
    }

    click = (e) => {
        e.preventDefault();
        var target = Number(e.target.id.slice(7));
        const item = {
            pageNumber: this.state.data.pageNumber,
            navigationButtons: [target]
        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this, requestBody, this.url);
    }

    redirect = (e) => {
        e.preventDefault();
        var id = Number(e.target.id);
        this.props.listener.setState({ id: id });
    }

    render() {
        if (!this.state.data) return null;
        var songs = [];
        for (let i = 0; i < this.state.data.titlesAndIds.length; i++) {
            songs.push(<tr key={"song " + i} className="d-sm-table-row "><td></td><td><a id={this.state.data.titlesAndIds[i].item2}
                onClick={this.redirect}>{this.state.data.titlesAndIds[i].item1}</a></td></tr>);
        }

        return (
            <div className="row">
                <p style={{ marginLeft: 12 + '%' }}>
                    Всего песен: {this.state.data.songsCount} &nbsp;
                    Страница: {this.state.data.pageNumber} &nbsp;
                </p>
                <p></p>
                <p></p>
                <table className="table" id="catalogTable">
                    <thead className="thead-dark ">
                        <tr>
                            <th width="10%"></th>
                            <th width="80%">
                                <form>
                                    <button id="js-nav-1" className="btn btn-info" onClick={this.click}>
                                         &lt;Назад
                                    </button>
                                    &nbsp;
                                    <button id="js-nav-2" className="btn btn-info" onClick={this.click}>
                                          Вперёд&gt;
                                    </button>
                                </form>
                            </th>
                            <th width="10%"></th>
                        </tr>
                    </thead>
                    <tbody>
                        {songs}
                    </tbody>
                </table>
            </div>
        );
    }
}

export default CatalogView;