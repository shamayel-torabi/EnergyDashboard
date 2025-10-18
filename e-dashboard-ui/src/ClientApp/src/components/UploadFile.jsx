import React, { Component } from 'react';
import PropTypes from 'prop-types';

export class UploadFile extends Component {
    constructor(props) {
        super(props);

        this.state = {
            files:null,
        }
    }

    static defaultProps = {
        label: 'انتخاب فایل',
        onChange: (e) => { }
    }

    static propTypes = {
        label: PropTypes.string,
        onChange: PropTypes.func
    };

    fileChange = (event) => {
        let files = event.target.files;
        this.setState({
            files: files
        })
    }

    renderFiles() {
        let f = this.state.files;
        if (f === null)
            return null;
        else {
            return Object.keys(f).map(k => {
                return <span style={{ display: 'block' }} key={k}>{f[k].name}</span>
            })
        }
    }

    render() {
        const {
            label,
            onChange,
            ...attributes
        } = this.props;

        let f = this.renderFiles();

        return (
            <>
                <div className="mb-3">
                    <label htmlFor="formFile" className="form-label">{this.props.label}</label>
                    <input className="form-control" type="file" id="formFile" {...attributes} onChange={this.fileChange} lang="fa" />
                </div>
                {f}
            </>
        )
    }
}
