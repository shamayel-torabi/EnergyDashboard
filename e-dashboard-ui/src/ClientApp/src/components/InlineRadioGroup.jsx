import React from 'react';
import { CFormCheck } from '@coreui/react'

const InlineRadioGroup = (props) => {

    const handleChange = (event) => {
        if (props.onChange)
            props.onChange(event);
    }

    const renderOptions = ()=> {
        const {
            options,
            onChange,
            name,
            ...attributes
        } = props;

        return options.map((option, key) => {
            return (
                <CFormCheck key={key}
                    inline
                    type="radio"
                    name={name}
                    id={option.value}
                    label={option.label}
                    onChange={handleChange}
                    defaultChecked={props.value === option.value}
                    {...attributes}>
                </CFormCheck>
            )
        });
    }

    return (
        <>
            {renderOptions()}
        </>
    );
}

export { InlineRadioGroup }

//export class InlineRadioGroup extends Component {
//    constructor(props) {
//        super(props);
//    }

//    static defaultProps = {
//        options: [],
//        onChange: (e) => { }
//    }

//    static propTypes = {
//        name: PropTypes.string.isRequired,
//        options: PropTypes.array.isRequired,
//        onChange: PropTypes.func
//    };

//    handleChange = (event) => {
//        if (this.props.onChange)
//            this.props.onChange(event);
//    }

//    renderOptions() {
//        const {
//            options,
//            onChange,
//            name,            
//            ...attributes
//        } = this.props;

//        return this.props.options.map((option, key) => {
//            return (
//                <CFormCheck key={key}
//                    inline
//                    type="radio"
//                    name={name}
//                    id={option.value}
//                    label={option.label}
//                    onChange={this.handleChange}
//                    defaultChecked = {this.props.value === option.value}
//                    {...attributes}>
//                </CFormCheck>
//            )
//        });
//    }
//    render() {
//        let options = this.renderOptions();
//        return (
//            <React.Fragment>
//                {options}
//            </React.Fragment>
//        );
//    }
//}
