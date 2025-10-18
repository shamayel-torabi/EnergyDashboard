import React, { useState } from "react";

const PasswordInput = (props) => {
    const [ password, setPassword] = useState('');
    const [ errors, setErrors ] = useState([]);

    const inputPassword = useRef(null);

    const {
        onChange,
        ...attributes
    } = props;

    const handleChange = (event) => {
        const value = event.target.value;

        setPassword(value);
        passwordCheck(value);

        if (onChange)
            onChange(value);
    }

    const passwordCheck = (password) => {
        let errors = [];
        let checkPassword = {
            containsUL: false,
            containsLL: false,
            containsN: false,
            contains6C: false,
        };
        let validPassword;

        // Validate lowercase letters
        const lowerCaseLetters = /[a-z]/g;
        if (password.match(lowerCaseLetters)) {
            checkPassword.containsLL = true;
        } else {
            checkPassword.containsLL = false;
            errors.push(" حروف کوچک ");
        }

        // Validate capital letters
        const upperCaseLetters = /[A-Z]/g;
        if (password.match(upperCaseLetters)) {
            checkPassword.containsUL = true;
        } else {
            checkPassword.containsUL = true;
            errors.push(" حروف بزرگ ");
        }

        // Validate numbers
        const numbers = /[0-9]/g;
        if (password.match(numbers)) {
            checkPassword.containsN = true;
        } else {
            checkPassword.containsN = false;
            errors.push(" عدد (0-9) ");
        }

        // has 8 characters
        if (password.length >= 6) {
            checkPassword.contains6C = true;
        }
        else {
            checkPassword.contains6C = false;
            errors.push(" طول بیشتر از 6 حرف ");
        }

        // all validations passed
        if (checkPassword.containsUL && checkPassword.containsLL && checkPassword.containsN && checkPassword.contains6C)
            validPassword = true;
        else
            validPassword = false;

        if (validPassword) {
            inputPassword.current.classList.remove("is-invalid")
            inputPassword.current.classList.add("is-valid");
            inputPassword.current.setCustomValidity("");
        }
        else {
            inputPassword.current.classList.add("is-invalid")
            inputPassword.current.classList.remove("is-valid");
            inputPassword.current.setCustomValidity("Password Error");
        }

        setErrors(errors);
    }

    const renderErrors = () => {
        return errors.map((v, k) => {
            return <span key={k}>{v + ','}</span>
        })
    }

    return (
        <div className="form-group row">
            <label htmlFor="password" className="col-4 col-form-label">گذرواژه</label>
            <div className="col-8">
                <div className="mb-4 input-group">
                    <div className="input-group-prepend">
                        <span className="input-group-text">
                            <i className="fas fa-key"></i>
                        </span>
                    </div>
                    <input
                        ref={this.inputPassword}
                        type="password"
                        name="password"
                        id="password"
                        autoComplete="off"
                        className="form-control"
                        value={password}                        
                        onChange={handleChange}
                        {...attributes}/>
                    <div className="invalid-feedback"><span> گذرواژه باید از</span>{renderErrors()}<span> تشکیل شده باشد</span></div>
                </div>
            </div>
        </div>
    );
}

export { PasswordInput}