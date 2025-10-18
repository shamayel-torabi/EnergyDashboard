import { useRef, useEffect, useState } from 'react';
import PropTypes from 'prop-types'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { CCloseButton } from '@coreui/react';
import { Message } from './Message';
import { useToast } from '../../components';
import { notificationUrl, apiUrl } from '../../ApiConfig';


const MessageList = ({ scope }) => {
    const [loading, setLoading] = useState(false);
    const [messages, setMessages] = useState([]);

    const hubConnection = useRef(null);
    const toast = useToast();

    useEffect(() => {
        const url = `${notificationUrl}/message`;

        hubConnection.current = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Error)
            .build();

        hubConnection.current.on('messageAdded', (receivedMessage) => {
            const d = new Date(receivedMessage.date);
            const dd = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 12, 0, 0, 0);
            const message = {
                id: receivedMessage.id,
                text: receivedMessage.text,
                date: dd.toJSON(),
                time: d.toLocaleTimeString('fa-IR'),
                messageScope: receivedMessage.messageScope,
                messageType: receivedMessage.messageType
            }

            if (scope) {
                if (message.messageScope === scope) {
                    setMessages(pre => [...pre, message]);
                }
            }
            else {
                setMessages(pre => [...pre, message]);
            }
        });

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection :('));

        getMessagesInitially();

        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        }

    }, []);


    const formatDate = (date) => {
        const { year, literal, month, day, weekday } = Object.fromEntries(
            new Intl.DateTimeFormat("fa", {
                weekday: "long",
                year: "numeric",
                month: "long",
                day: "numeric"
            })
                .formatToParts(date)
                .map(item => [item.type, item.value])
        );

        const faDate = `${weekday}${literal}${day} ${month} ${year}`;
        return faDate;
    }

    const getMessagesInitially = async () => {
        const url = `${apiUrl}/Messages`;

        setLoading(true)

        try {
            let response = await fetch(url);
            if (response.ok) {
                const data = await response.json();

                let filterMessages = data;
                if (scope) {
                    filterMessages = data.filter(x => x.messageScope === scope);
                }

                let msgs = filterMessages.map((m, i) => {
                    let d = new Date(m.date);
                    let dd = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 12, 0, 0, 0);
                    return {
                        id: m.id,
                        text: m.text,
                        date: dd.toJSON(),
                        time: d.toLocaleTimeString('fa-IR'),
                        messageScope: m.messageScope,
                        messageType: m.messageType
                    };
                });

                setMessages(msgs);
            }
            else {
                setMessages([]);
                toast.showToast("error", "خطا در دریافت لیست پیام ها.");
                const err = await response.json();
                console.error(err);
            }
        }
        catch (e) {
            setMessages([]);
            toast.showToast("error", "خطا در اتصال به سرور پیام رسان.");
            console.error(e);
        }
        setLoading(false);
    }

    const deleteMessage = async (id) => {
        const url = `${apiUrl}/Messages/${id}`;

        let messageId = null;

        try {
            let response = await fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                },
            });

            if (response.ok) {
                messageId = await response.json();

                const msg = messages.find(x => x.id === messageId);
                const index = messages.indexOf(msg);
                let msgs = messages;
                if (index !== -1) {
                    msgs.splice(index, 1);
                }

                setMessages(msgs);
            }
            else {
                const err = await response.json();
                console.log(err);
                toast.showToast("error", "خطا در حذف پیام.");
            }
        }
        catch (e) {
            toast.showToast("error", "خطا در اتصال به سرور پیام رسان.");
            console.error('deleteMessage error', e);
        }

        return messageId;
    }

    const deleteMessages = async (date) => {
        const d = new Date(date).toJSON();
        const url = `${apiUrl}/Messages/DeleteAllAtDate/${d}`;

        setLoading(true)

        try {
            let response = await fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                },
            });

            if (response.ok) {
                await getMessagesInitially();
            }
            else {
                const err = await response.json();
                toast.showToast("error", "خطا در حذف پیام ها.");
                console.error(err);
            }
        }
        catch (e) {
            toast.showToast("error", "خطا در اتصال به سرور پیام رسان.");
            console.error("خطا در اتصال به سرور پیام رسان.", e);
        }

        setLoading(false);
    }

    const renderMessages = () => {
        if (loading)
            return (<span className="k-icon k-i-loading"></span>);

        if (messages && messages.length === 0)
            return null;

        const messageByDate = messages.reduce((ubc, u) => ({
            ...ubc,
            [u.date]: [...(ubc[u.date] || []), u],
        }), {});

        let a = Object.keys(messageByDate).map((date, i) => {
            const items = messageByDate[date];

            const pdate = formatDate(new Date(date));

            let msgs = items.map((message, index) => {
                return <Message key={index} message={message} onDelete={deleteMessage} />
            })

            return (
                <div key={i}>
                    <div className="shadow-sm p-3 mb-3 bg-light rounded">
                        <span className="text-center" color="link">{pdate}</span>
                        <CCloseButton title="حذف همه پیامها" className="float-end" onClick={(e) => deleteMessages(date)}></CCloseButton>
                    </div>
                    <div>
                        {msgs}
                    </div>
                </div>
            );
        })

        return a;
    }

    return (
        <div>
            {renderMessages()}
        </div>
    );
}

MessageList.propTypes = {
    scope: PropTypes.number
}

export { MessageList };
