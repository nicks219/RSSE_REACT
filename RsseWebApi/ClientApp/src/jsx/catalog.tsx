import * as React from 'react';
import { Loader } from "./loader.jsx";

interface IState {
    data: any;
}
interface IProps {
    listener: any;
}

class CatalogView extends React.Component<IProps, IState> {
    url: string;
    mounted: boolean;

    public state: IState = {
    data: null
}

    constructor(props: any) {
        super(props);
        this.url = "/api/catalog";
        this.mounted = true;
    }

    componentWillUnmount() {
        this.mounted = false;
    }

    componentDidMount() {
        Loader.getDataById(this, 1, this.url);
    }

    click = (e: any) => {
        e.preventDefault();
        var target = Number(e.target.id.slice(7));
        const item = {
            pageNumber: this.state.data.pageNumber,
            navigationButtons: [target]
        };
        var requestBody = JSON.stringify(item);
        Loader.postData(this, requestBody, this.url);
    }

    redirect = (e: any) => {
        e.preventDefault();
        var id = Number(e.target.id);
        this.props.listener.setState({ id: id });
    }

    delete = (e: any) => {
        e.preventDefault();
        var id = Number(e.target.id);
        console.log('You want to delete song with id: ' + id);
        Loader.deleteDataById(this, id, this.url, this.state.data.pageNumber);
    }

    render() {
        if (!this.state.data) return null;
        var songs = [];
        for (let i = 0; i < this.state.data.titlesAndIds.length; i++) {
            songs.push(
                <tr key={"song " + i} className="bg-warning">
                    <td></td>
                    <td>
                        <button className="btn btn-outline-light" id={this.state.data.titlesAndIds[i].item2}
                            onClick={this.redirect}>{this.state.data.titlesAndIds[i].item1}
                        </button>
                    </td>
                    <td>
                        <button className="btn btn-outline-light" id={this.state.data.titlesAndIds[i].item2} onClick={this.delete}>
                            &#10060;
                        </button>
                    </td>
                </tr>);
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
                            <th ></th>
                            <th >
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
                            <th ></th>
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