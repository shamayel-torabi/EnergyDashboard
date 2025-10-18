import { useState } from 'react';
import PropTypes from 'prop-types'
import {
    CToast,
    CToastBody,
    CToastHeader,
    CToastClose
} from '@coreui/react';

const Message = ({ message, onDelete }) => {
    const [visible, setVisible] = useState(true);

    const onDismiss = (id) => {
        setVisible(false);
        onDelete(id);
    }

    const Icon = () => {
        let icon;
        switch (message.messageType) {
            //Success
            case 0:
                icon = '#00FF00';
                break;
            //Error
            case 1:
                icon = '#FF0000';
                break;
            //Warning
            case 2:
                icon = '#FFFF00';
                break;
            //Info
            case 3:
            default:
                icon = '#0000FF';
                break;
        }

        return icon;
    }

    return (
        <CToast className="m-1 rounded" visible={visible} autohide={false}>
            <CToastHeader>
                <svg
                    className="rounded me-2"
                    width="20"
                    height="20"
                    xmlns="http://www.w3.org/2000/svg"
                    preserveAspectRatio="xMidYMid slice"
                    focusable="false"
                    role="img"
                >
                    <rect width="100%" height="100%" fill={Icon()}></rect>
                </svg>
                <small className="mr-1">{message.time}</small>
                <CToastClose title="حذف پیام" className="me-2 m-auto" onClick={e => onDismiss(message.id)} />
            </CToastHeader>
            <CToastBody>
                {message.text}
            </CToastBody>
        </CToast>
    )
}

Message.propTypes = {
    onDelete: PropTypes.func.isRequired,
    message: PropTypes.object.isRequired
}


export { Message };
