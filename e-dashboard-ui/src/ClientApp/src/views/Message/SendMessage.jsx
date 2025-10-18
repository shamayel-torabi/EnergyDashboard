import { useRef, useEffect, useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import {
    CCard, CCardHeader, CCardBody,
    CInputGroup,
    CForm, CButton, CFormInput
} from '@coreui/react';
import { v4 as uuidv4 } from 'uuid';
import { MessageList } from './MessageList';
import { notificationUrl } from '../../ApiConfig';
import './MessageList.scss';

const SendMessage = (props) => {
    const hubConnection = useRef(null);
    const [message, setMessage] = useState({
        text: '',
        date: new Date().toISOString(),
        save: true,
        messageType: 0,
        messageScope: 0
    })

    useEffect(() => {
        const url = `${notificationUrl}/message`;

        hubConnection.current = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Error)
            .build();

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection :('));

        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        }

    }, []);

    const inputChange = (e) => {
        const value = e.target.value;

        setMessage(prevState => ({
            ...prevState,
            text: value
        }));
    }


    const sendMessage = (e) => {
        e.preventDefault();

        message.id = uuidv4();
        message.date = new Date().toISOString();

        if (message.text.length > 0) {
            hubConnection.current.invoke('sendMessage', message)
                .catch(err => console.error(err));

            setMessage(prevState => ({
                ...prevState,
                text: ''
            }));
        }
    }

    return (
        <CCard className="animated fadeIn">
            <CCardHeader>
                <strong>ارسال پیام</strong>
            </CCardHeader>
            <CCardBody>
                <CCard>
                    <div className="message-list">
                        <MessageList />
                    </div>
                </CCard>
                <CForm onSubmit={sendMessage}>
                    <CInputGroup className="mt-1">
                        <CFormInput value={message.text} onChange={inputChange} />
                        <CButton type="submit">ارسال</CButton>
                    </CInputGroup>
                </CForm>
            </CCardBody>
        </CCard>
    );
}

export default SendMessage;
