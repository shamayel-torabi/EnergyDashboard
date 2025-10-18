
import {identityServerUrl, appName} from '../ApiConfig'
export const ApplicationName = appName;
export const IdentityServerUrl = identityServerUrl;

export const QueryParameterNames = {
    ReturnUrl: 'returnUrl',
    Message: 'message'
};

export const LogoutActions = {
    Logout: 'logout',
    LogoutCallback: 'logout-callback'
};

export const LoginActions = {
    Login: 'login',
    LoginCallback: 'login-callback',
    LoginFailed: 'login-failed',
    Profile: 'profile',
    Register: 'register'
};

const prefix = '/authentication';

export const ApplicationPaths = {
    DefaultLoginRedirectPath: '/',
    ApiAuthorizationClientConfigurationUrl: `${IdentityServerUrl}/_configuration/${ApplicationName}`,
    ApiAuthorizationPrefix: prefix,
    Login: `${LoginActions.Login}`,
    LoginCallback: `${LoginActions.LoginCallback}`,
    LoginFailed: `${LoginActions.LoginFailed}`,
    Register: `${LoginActions.Register}`,
    Profile: `${LoginActions.Profile}`,
    LogOut: `${LogoutActions.Logout}`,
    LogOutCallback: `${LogoutActions.LogoutCallback}`,
    IdentityRegisterPath: `${IdentityServerUrl}/Account/Register`,
    IdentityManagePath: `${IdentityServerUrl}/Account/Manage`
};