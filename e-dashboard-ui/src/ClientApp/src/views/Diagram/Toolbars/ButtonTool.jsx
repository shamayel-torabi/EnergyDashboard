import React from 'react';
import PropTypes from 'prop-types';

const ButtonTool = (props) => {
    const { children, label, icon, onClick } = props;

    const handleChange = (event) => {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        onClick(value);
    };

    return (
        <div className="tool" title={label}>
            <label className="btn btn-secondary btn-sm" onClick={handleChange}>
                <span className={icon}>
                    {children}
                </span>
            </label>
        </div>
    );
};

ButtonTool.defaultProps = {
    onClick: (e) => { }
};

ButtonTool.propTypes = {
    children: PropTypes.element,
    label: PropTypes.string.isRequired,
    icon: PropTypes.string.isRequired,
    onClick: PropTypes.func
};

export { ButtonTool }