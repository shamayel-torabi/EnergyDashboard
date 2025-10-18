import { useEffect, useState } from 'react'
import {
    QueryParameterNames,
    LogoutActions,
    ApplicationPaths,
} from './ApiAuthorizationConstants';
import { useAuthorize } from './AuthorizeProvider'

interface LogoutProps {
    action: string
}
const Logout = ({ action }: LogoutProps) => {
    const [message, setMessage] = useState(undefined);
    const authService = useAuthorize()

    useEffect(() => {
        switch (action) {
            case LogoutActions.Logout:
                logout(getReturnUrl(null))
                break;
            case LogoutActions.LogoutCallback:
                processLogoutCallback();
                break;
            default:
                throw new Error(`Invalid action '${action}'`);
        }
    }, []);

    const logout = async (returnUrl: string) => {
        await authService.signOut(returnUrl);

        if (authService.error) {
            setMessage(authService.error.message)
        }
    }

    const processLogoutCallback = async () => {
        const url = window.location.href;
        await authService.completeSignOut(url);

        if (authService.error) {
            setMessage(authService.error.message)
        }
        else {
            navigateToReturnUrl(getReturnUrl(null));
        }
    }

    const getReturnUrl = (state: any) => {
        const params = new URLSearchParams(window.location.search);
        const fromQuery = params.get(QueryParameterNames.ReturnUrl);
        if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
            throw new Error("Invalid return url. The return url needs to have the same origin as the current page.")
        }
        return (state && state.returnUrl) || fromQuery || `${window.location.origin}${ApplicationPaths.DefaultLoginRedirectPath}`;
    }

    const navigateToReturnUrl = (returnUrl: string) => {
        return window.location.replace(returnUrl);
    }

    if (!!message) {
        return (<div>{message}</div>);
    }
}

export default Logout