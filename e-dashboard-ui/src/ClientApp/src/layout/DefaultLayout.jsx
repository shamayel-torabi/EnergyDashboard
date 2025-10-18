import React, { useRef, useEffect } from 'react'
import { Provider } from 'react-redux'
import { AppContent, AppSidebar, AppFooter, AppHeader } from './index'
import { ToastContainer, toast } from 'react-toastify';
import { ToastContext } from '../components';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import store from './store'
import { notificationUrl } from '../ApiConfig';

const DefaultLayout = () => {
    const hubConnection = useRef(null);

    useEffect(() => {
        const url = `${notificationUrl}/message`;

        hubConnection.current = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Error)
            .build();

        hubConnection.current.on('messageAdded', (receivedMessage) => {
            let type;
            switch (receivedMessage.messageType) {
                case 0:
                    type = 'success';
                    break;
                case 1:
                    type = 'error';
                    break;
                case 2:
                    type = 'warn';
                    break;
                case 3:
                default:
                    type = 'info';
                    break;
            }

            showToast(type, receivedMessage.text, 5000);
        });

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection'));

        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        };
    }, []);

    const showToast = (type, body, delay) => {
        switch (type) {
            case 'success':
                toast.success(body, { autoClose: delay });
                break;
            case 'error':
                toast.error(body, { autoClose: delay });
                break;
            case 'warn':
                toast.warn(body, { autoClose: delay });
                break;
            case 'info':
                toast.info(body, { autoClose: delay });
                break;
            default:
                toast(body, { autoClose: delay });
                break;
        }
    }

    const getToastContext = () => {
        return {
            showToast: showToast,
        }
    }

    return (
        <Provider store={store}>
            <ToastContext.Provider value={{ ...getToastContext() }}>
                <AppSidebar />
                <div className="wrapper d-flex flex-column vh-100 bg-light">
                    <AppHeader />
                    <div className="body flex-grow-1 px-1">
                        <AppContent />
                    </div>
                    <AppFooter />
                </div>
                <ToastContainer
                    position="bottom-left"
                    autoClose={10000}
                    hideProgressBar={false}
                    newestOnTop={false}
                    closeOnClick
                    rtl
                    pauseOnVisibilityChange
                    draggable
                    pauseOnHover
                />
            </ToastContext.Provider>
        </Provider>
    )
}

export default React.memo(DefaultLayout)
