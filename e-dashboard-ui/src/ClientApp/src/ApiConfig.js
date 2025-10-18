const ApiConfig = {
    host: process.env.REACT_APP_API_HOST,
    identity: process.env.REACT_APP_IDENTIY_HOST,
    notification: process.env.REACT_APP_NOTIFICATION,
    appName: process.env.REACT_APP_NAME
}

export const apiUrl = ApiConfig.host;
export const notificationUrl = ApiConfig.notification;
export const identityServerUrl = ApiConfig.identity;
export const appName = ApiConfig.appName;

