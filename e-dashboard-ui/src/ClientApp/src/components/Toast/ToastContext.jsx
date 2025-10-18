import React from 'react';

import { ToastContainer,  toast } from 'react-toastify';

const ToastContext = React.createContext({})

const ToastProvider = ({children}) =>  {
    const getToastContext = () => {
        return {
            showToast: showToast,
        }
    }

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

    <ToastContext.Provider value={{ ...getToastContext() }}>
        {children}
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
    </ToastContext.Provider >
}

const useToast = () => React.useContext(ToastContext);

const withToast = (WrappedComponent) => (props) => {
    return (
        <ToastContext.Consumer>
            {context =>
                <WrappedComponent {...props} {...context} />
            }
        </ToastContext.Consumer>
    )
}

export { ToastContext, ToastProvider, useToast, withToast}
