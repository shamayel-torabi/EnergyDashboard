import React, { Component } from 'react';
import PropTypes from 'prop-types';
import validator from 'validator'

export class IPAddress extends Component {
    static defaultProps = {
        className: "form-control",
        onChange: (e) => { }
    }

    static propTypes = {
        name: PropTypes.string.isRequired,
        onChange: PropTypes.func
    };


    constructor(props) {
        super(props);
        this.inputRef = React.createRef();
    }

    getInputRef() {
        return this.inputRef.current;
    }

    handleChange = (event) => {
        this.props.onChange(event);
        this.checkError();
    }

    handleBlur = (e) => {
        this.checkError();
    }

    checkError = (e) => {
        const inputRef = this.getInputRef();
        const value = inputRef.value;

        if (validator.isIP(value)) {
            inputRef.classList.remove("is-invalid")
            inputRef.classList.add("is-valid");
            inputRef.setCustomValidity("");
        }
        else {
            inputRef.classList.add("is-invalid")
            inputRef.classList.remove("is-valid");
            inputRef.setCustomValidity("IP Address Error");
        }
    }

    render() {
        const {
            onChange,
            ...attributes
        } = this.props;

        return (
            <React.Fragment>
                <input className={this.props.className}  {...attributes} ref={this.inputRef} onChange={this.handleChange} onBlur={this.handleBlur}/>
            </React.Fragment>
        );
    }
}
