import React, { Component } from 'react';
import PropTypes from 'prop-types';

export class DataListInput extends Component {
    static defaultProps = {
        options: [],
        selectText: null,
        onChange: (e) => { }
    }

    static propTypes = {
        options: PropTypes.array,
        selectText: PropTypes.string,
        onChange: PropTypes.func
    };

    handleChange = (event) => {
        this.props.onChange(event);
    }

    render() {
        let opt = this.props.options.map((d) => <option key={d.value} value={d.value}>{d.label}</option>)
        const {
            options,
            selectText,
            onChange,
            ...attributes
        } = this.props;

        return (
            <>
                <input list="data-options" className="form-control" onChange={this.handleChange} />
                <datalist id="data-options" >
                    {opt}
                </datalist>
            </>
        );
    }
}
