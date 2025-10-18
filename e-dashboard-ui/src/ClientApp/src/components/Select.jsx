import React, { Component } from 'react';
import PropTypes from 'prop-types';

export class Select extends Component {
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
            <select className="form-select" onChange={this.handleChange} {...attributes}>
                {this.props.selectText ? <option key="0" value="">{this.props.selectText}</option> : null}
                {opt}
            </select>
        );
    }
}
