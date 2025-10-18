import React, { useState, useRef, useEffect, useReducer, useCallback } from 'react';
import { User, UserManager, UserManagerSettingsStore } from 'oidc-client-ts';


interface AuthState {
    user: User | null,
    isAuthenticated: boolean,
    accessToken: string | undefined,
    isLoading: boolean,
    error: any
}

interface AuthorizeContextType extends AuthState {
    signIn: (url: string) => Promise<void>,
    completeSignIn: (url: string) => Promise<{state: unknown}>,
    signOut: (url: string) => Promise<void>,
    completeSignOut: (url: string) => Promise<void>,
}

export const AuthorizeContext = React.createContext<AuthorizeContextType>({} as AuthorizeContextType);

const initialAuthState: AuthState = {
    user: null,
    isAuthenticated: false,
    accessToken: undefined,
    isLoading: true,
    error: undefined
};

enum ActionType {
    USER_LOADED = 'USER_LOADED',
    USER_UNLOADED = 'USER_UNLOADED',
    NAVIGATOR_INIT = 'NAVIGATOR_INIT',
    NAVIGATOR_CLOSE = 'NAVIGATOR_CLOSE',
    ERROR = 'ERROR'
}

type Action =
    | { type: ActionType.USER_LOADED, payload: User | null }
    | { type: ActionType.USER_UNLOADED, }
    | { type: ActionType.NAVIGATOR_INIT }
    | { type: ActionType.NAVIGATOR_CLOSE }
    | { type: ActionType.ERROR, payload: any }

const reducer = (state: AuthState, action: Action): AuthState => {
    switch (action.type) {
        case ActionType.USER_LOADED:
            return {
                ...state,
                user: action.payload,
                isLoading: false,
                isAuthenticated: action.payload ? !action.payload.expired : false,
                accessToken: action.payload ? action.payload.access_token : undefined,
                error: undefined,
            };
        case ActionType.USER_UNLOADED:
            return {
                ...state,
                user: null,
                isAuthenticated: false,
                accessToken: undefined,
            };
        case ActionType.NAVIGATOR_INIT:
            return {
                ...state,
                isLoading: true,
            };
        case ActionType.NAVIGATOR_CLOSE:
            return {
                ...state,
                isLoading: false,
            };
        case ActionType.ERROR:
            return {
                ...state,
                isLoading: false,
                error: action.payload,
            };
        default:
            return {
                ...state,
                isLoading: false,
                error: new Error(`unknown type ${action["type"]}`),
            };
    }
};

interface AuthorizeProviderProps {
    children: React.ReactNode,
    settings: UserManagerSettingsStore
}



export const AuthorizeProvider: React.FC<AuthorizeProviderProps> = ({ children, settings }) => {
    const [authState, dispatch] = useReducer(reducer, initialAuthState);

    const [userManager] = useState(() => {
        let userManager = new UserManager(settings);
        return userManager;
    });

    const didInitialize = useRef(false);

    useEffect(() => {
        if (!userManager || didInitialize.current) {
            return;
        }

        didInitialize.current = true;

        const fetchUser = async () => {
            try {
                const user = await userManager.getUser();
                dispatch({ type: ActionType.USER_LOADED, payload: user });
            } catch (err) {
                dispatch({ type: ActionType.ERROR, payload: new Error(`getUser error:${err}`) });
            }
        }

        fetchUser();
    }, [userManager]);

    useEffect(() => {
        if (!userManager) return undefined;
        // event UserLoaded (e.g. initial load, silent renew success)
        const handleUserLoaded = (user: User) => {
            dispatch({ type: ActionType.USER_LOADED, payload: user });
        };
        userManager.events.addUserLoaded(handleUserLoaded);

        // event UserUnloaded (e.g. userManager.removeUser)
        const handleUserUnloaded = () => {
            dispatch({ type: ActionType.USER_UNLOADED });
        };
        userManager.events.addUserUnloaded(handleUserUnloaded);

        // event SilentRenewError (silent renew error)
        const handleSilentRenewError = (err: any) => {
            dispatch({ type: ActionType.ERROR, payload: err });
        };
        userManager.events.addSilentRenewError(handleSilentRenewError);

        return () => {
            userManager.events.removeUserLoaded(handleUserLoaded);
            userManager.events.removeUserUnloaded(handleUserUnloaded);
            userManager.events.removeSilentRenewError(handleSilentRenewError);
        };
    }, [userManager]);

    const signIn = useCallback(async (returnUrl: string) => {
        try {
            dispatch({ type: ActionType.NAVIGATOR_INIT });
            const user = await userManager.signinSilent();
            dispatch({ type: ActionType.USER_LOADED, payload: user });
            dispatch({ type: ActionType.NAVIGATOR_CLOSE });
        } catch (e) {
            try {
                const arg = { state: { returnUrl } };
                await userManager.signinRedirect(arg);
            }
            catch (err) {
                const errMessage = `signinRedirect authentication error: ${err}`;
                console.error(errMessage);
                dispatch({ type: ActionType.ERROR, payload: new Error(errMessage) });
            }
        }
    }, [userManager]);

    const completeSignIn = useCallback(async (url: string) => {
        try {
            const user = await userManager.signinCallback(url);
            dispatch({ type: ActionType.NAVIGATOR_CLOSE });
            const state = user && user.state;
            return { state };
        } catch (err) {
            const errMessage = `signinCallback authentication error: ${err}`;
            console.error(errMessage);
            dispatch({ type: ActionType.ERROR, payload: new Error(errMessage) });
            return { state: undefined };
        }
    }, [userManager]);

    const signOut = useCallback(async (returnUrl: string) => {
        const arg = { state: { returnUrl } };

        try {
            await userManager.signoutRedirect(arg);
        } catch (err) {
            const errMessage = `signoutRedirect authentication error: ${err}`;
            console.error(errMessage);
            dispatch({ type: ActionType.ERROR, payload: new Error(errMessage) });
        }
    }, [userManager]);

    const completeSignOut = useCallback(async (url: string) => {
        try {
            //const response = await userManager.signoutCallback(url);
            await userManager.signoutCallback(url);
            dispatch({ type: ActionType.USER_UNLOADED });
            //const state = response && response.state;
            //const state = undefined;
            //return state;
        } catch (err) {
            const errMessage = `signoutCallback authentication error: ${err}`;
            console.error(errMessage);
            dispatch({ type: ActionType.ERROR, payload: new Error(errMessage) });
            //return { state: undefined };
        }
    }, [userManager]);

    const authContext = {
        ...authState,
        signIn,
        completeSignIn,
        signOut,
        completeSignOut,
    };

    return (
        <AuthorizeContext.Provider value={{ ...authContext }}>
            {children}
        </AuthorizeContext.Provider >
    );
}

export const useAuthorize = () => React.useContext(AuthorizeContext);

export function withAuthorize(Component: any) {
    function ComponentWithAuthorizeProp(props: any) {
        const context = useAuthorize();
        return (
            <Component  {...props} {...context} />
        );
    }

    return ComponentWithAuthorizeProp;
}