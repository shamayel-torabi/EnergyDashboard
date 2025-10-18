import { Navigate, useLocation } from "react-router-dom";
import { ApplicationPaths, QueryParameterNames } from './ApiAuthorizationConstants';
import { useAuthorize } from './AuthorizeProvider'

interface RequireAuthProps {
    children: React.ReactNode
}

const RequireAuth = ({ children }: RequireAuthProps) => {
    const authService = useAuthorize()
    const location = useLocation();

    var link = document.createElement("a");
    link.href = location.pathname;
    const returnUrl = `${link.protocol}//${link.host}${link.pathname}${link.search}${link.hash}`;
    const redirectUrl = `${ApplicationPaths.ApiAuthorizationPrefix}/${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURIComponent(returnUrl)}`;

    if (authService.isLoading) {
        return (<div></div>)
    }

    if (!authService.isAuthenticated) {
        return <Navigate to={redirectUrl} replace />;
    }

    return children;
}

export default RequireAuth