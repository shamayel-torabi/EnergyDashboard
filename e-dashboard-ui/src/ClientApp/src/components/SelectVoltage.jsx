import React, { Component } from 'react';
import PropTypes from 'prop-types';

export class SelectVoltage extends Component {
    constructor(props) {
        super(props);
        let op = props.options[0];
        let style = {
            color: op.color,
        };

        this.state = {
            style: style,
        }
    }

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
        let value = event.target.value;
        let op = this.props.options.find(x => x.value == value);
        let style = {
            color: op.color,
        };

        this.setState({
            style
        });

        this.props.onChange(op);
    }

    render() {
        let opt = this.props.options.map((d) => {
            let st = {
                color: d.color,
            }
            return (
                <option style={st} key={d.value} value={d.value}>
                    {d.label}
                </option>)
        });
        const {
            options,
            selectText,
            onChange,
            ...attributes
        } = this.props;

        return (
            <select class="form-select" style={this.state.style} onChange={this.handleChange} {...attributes}>
                {this.props.selectText ? <option key="0" value="">{this.props.selectText}</option> : null}
                {opt}
            </select>
        );
    }
}
