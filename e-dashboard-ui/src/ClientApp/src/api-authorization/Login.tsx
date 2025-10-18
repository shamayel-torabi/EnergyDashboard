import { useEffect, useState } from 'react'
import {
    LoginActions,
    QueryParameterNames,
    ApplicationPaths,
} from './ApiAuthorizationConstants';
import { useAuthorize } from './AuthorizeProvider'

interface LoginProps {
    action: string
}

const Login = ({ action }:LoginProps)  => {
    const [message, setMessage] = useState<string | null>(null);
    const authService = useAuthorize()

    useEffect(() => {
        switch (action) {
            case LoginActions.Login:
                login(getReturnUrl(null));
                break;
            case LoginActions.LoginCallback:
                processLoginCallback();
                break;
            case LoginActions.LoginFailed:
                const params = new URLSearchParams(window.location.search);
                const err = params.get(QueryParameterNames.Message);
                setMessage(err)
                break;
            case LoginActions.Profile:
                redirectToProfile();
                break;
            case LoginActions.Register:
                redirectToRegister();
                break;
            default:
                throw new Error(`Invalid action '${action}'`);
        }
    }, []);

    const login = async (returnUrl: string) => {
        await authService.signIn(returnUrl);

        if (authService.error) {
            setMessage(authService.error.message)
        }
    }

    const processLoginCallback = async () => {
        const url = window.location.href;
        const result = await authService.completeSignIn(url);

        if (authService.error) {
            setMessage(authService.error.message)
        }
        else {
            navigateToReturnUrl(getReturnUrl(result.state));
        }
    }

    const getReturnUrl = (state: any) => {
        const params = new URLSearchParams(window.location.search);
        const fromQuery = params.get(QueryParameterNames.ReturnUrl);
        if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
            throw new Error("Invalid return url. The return url needs to have the same origin as the current page.")
        }
        return (state && state.returnUrl) || fromQuery || `${window.location.origin}/`;
    }

    const redirectToRegister = () => {
        redirectToApiAuthorizationPath(`${ApplicationPaths.IdentityRegisterPath}?${QueryParameterNames.ReturnUrl}=${encodeURI(ApplicationPaths.Login)}`);
    }

    const redirectToProfile = () => {
        redirectToApiAuthorizationPath(`${ApplicationPaths.IdentityManagePath}`);
    }

    const redirectToApiAuthorizationPath = (apiAuthorizationPath: string) => {
        const redirectUrl = `${apiAuthorizationPath}`;
        // It's important that we do a replace here so that when the user hits the back arrow on the
        // browser they get sent back to where it was on the app instead of to an endpoint on this
        // component.
        window.location.replace(redirectUrl);
    }

    const navigateToReturnUrl = (returnUrl: string): void => {
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        window.location.replace(returnUrl);
    }

    if (authService.isLoading)
        return (<div></div>)

    if (!!message) {
        return (<div>{message}</div>)
    }
}

export default Login
