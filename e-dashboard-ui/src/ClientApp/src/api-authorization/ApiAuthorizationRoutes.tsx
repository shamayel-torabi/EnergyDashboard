import Login from './Login'
import Logout from './Logout'
import { AuthLayout } from './AuthLayout'
import { ApplicationPaths, LoginActions, LogoutActions } from './ApiAuthorizationConstants';
import { RouteObject } from 'react-router';


const ApiAuthorizationRoutes: RouteObject[] = [
  {
    path: ApplicationPaths.ApiAuthorizationPrefix,
    element: <AuthLayout />,
    children: [
      {
        path: ApplicationPaths.Login,
        //element: loginAction(LoginActions.Login)
        element: <Login action={LoginActions.Login} />
      },
      {
        path: ApplicationPaths.LoginCallback,
        //element: loginAction(LoginActions.LoginCallback)
        element: <Login action={LoginActions.LoginCallback} />
      },
      {
        path: ApplicationPaths.Profile,
        //element: loginAction(LoginActions.Profile)
        element: <Login action={LoginActions.Profile} />
      },
      {
        path: ApplicationPaths.Register,
        //element: loginAction(LoginActions.Register)
        element: <Login action={LoginActions.Profile} />
      },
      {
        path: ApplicationPaths.LogOut,
        //element: logoutAction(LogoutActions.Logout)
        element: <Logout action={LogoutActions.Logout} />
      },
      {
        path: ApplicationPaths.LogOutCallback,
        //element: logoutAction(LogoutActions.LogoutCallback)
        element: <Logout action={LogoutActions.LogoutCallback} />
      },
    ]
  }
];



export default ApiAuthorizationRoutes;
