import React, { useState } from 'react'
import PropTypes from 'prop-types';
import { CDropdown, CDropdownToggle, CDropdownMenu } from '@coreui/react';

const ColorSwatch = ({ color, colors, onColorChange }) => {
    const handleClick = (color) => {
        onColorChange && onColorChange(color);
    }

    const renderColors = () => {
        return colors.map((c, i) => {

            const nonSelectedStyle = {
                backgroundColor: c,
                width: "100%",
                height: "calc(1.0em + 0.75rem + 2px)",
                padding: "0.375rem 0.75rem",
                margin: "0.2rem 0.0rem",
                borderRadius: "0.25rem",
            };
            const selectedStyle = {
                backgroundColor: c,
                width: "100%",
                height: "calc(1.0em + 0.75rem + 2px)",
                padding: "0.375rem 0.75rem",
                margin: "0.2rem 0.0rem",
                borderRadius: "0.25rem",
                boxShadow: "0 0 10px rgba(81, 203, 238, 1)",
                border: "1px solid rgba(81, 203, 238, 1)"
            }

            const style = c === color ? selectedStyle : nonSelectedStyle;
            return <div className="ml-2" key={i} style={style} onClick={(e) => handleClick(c)}></div>
        })
    }

    const style = {
        width: '16.0rem',
        padding: '0px 10px',
    }

    return (
        <div style={style}>
            {renderColors()}
        </div>
    )
}

ColorSwatch.defaultProps = {
    colors: ['#000000', '#EB9694', '#FFFF00', '#0000FF', '#00FF00', '#FF0000', '#FF00FF'],
}

ColorSwatch.propTypes = {
    color: PropTypes.string,
    onColorChange: PropTypes.func,
    colors: PropTypes.arrayOf(PropTypes.string),
}

const ColorPicker = ({ value, name, onChange, ...rest }) => {
    const [color, setcolor] = useState(value);

    const handleColorChange = (color) => {
        setcolor(color)
        onChange && onChange(
            {
                value: color,
                name: name
            }
        );
    }

    const style = {
        backgroundColor: color,
    };

    return (
        <CDropdown style={{ width: '100%' }} variant='dropdown'>
            <CDropdownToggle custom>
                <input className='form-select' name={name} style={style} {...rest} />
            </CDropdownToggle>
            <CDropdownMenu component='div'>
                <ColorSwatch color={color} onColorChange={handleColorChange} />
            </CDropdownMenu>
        </CDropdown>
    )
}

ColorPicker.prototype = {
    value: PropTypes.string.isRequired,
    name: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
}

export { ColorPicker };