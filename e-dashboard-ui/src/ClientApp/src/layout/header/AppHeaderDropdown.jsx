import {
    CDropdown,
    CDropdownHeader,
    CDropdownItem,
    CDropdownMenu,
    CDropdownToggle,
} from '@coreui/react'
import { useAuthorize } from '../../api-authorization/AuthorizeProvider'
import { ApplicationPaths } from '../../api-authorization/ApiAuthorizationConstants';
import CIcon from '@coreui/icons-react';
import { cilUser } from '@coreui/icons'

const AppHeaderDropdown = (props) => {
    const authService = useAuthorize();


    const userName = authService.user?.profile.name;

    const renderUserMenu = () => {
        const profilePath = `${ApplicationPaths.ApiAuthorizationPrefix}/${ApplicationPaths.Profile}`;
        const registerPath = `${ApplicationPaths.ApiAuthorizationPrefix}/${ApplicationPaths.Register}`;
        const loginPath = `${ApplicationPaths.ApiAuthorizationPrefix}/${ApplicationPaths.Login}`;
        const logoutPath = `${ApplicationPaths.ApiAuthorizationPrefix}/${ApplicationPaths.LogOut}`;

        if (authService.isAuthenticated) {
            return (
                <CDropdownMenu className="pt-0" placement="bottom-end">
                    <CDropdownHeader className="bg-light fw-semibold py-2"><strong><span>{userName}</span></strong></CDropdownHeader>
                    <CDropdownItem href={profilePath}><strong><span>مشخصات کاربر</span></strong></CDropdownItem>
                    <CDropdownItem href={logoutPath}>خروج از وبگاه</CDropdownItem>
                </CDropdownMenu>
            );
        }
        else {
            return (
                <CDropdownMenu className="pt-0" placement="bottom-end">
                    <CDropdownHeader className="bg-light fw-semibold py-2"><strong><span>کاربر ناشناس</span></strong></CDropdownHeader>
                    <CDropdownItem href={registerPath}>ثبت نام</CDropdownItem>
                    <CDropdownItem href={loginPath}>ورود به وبگاه</CDropdownItem>
                </CDropdownMenu>

            );
        }
    }

    if (authService.isLoading)
        return (<div></div>)

    return (
        <CDropdown variant="nav-item">
            <CDropdownToggle placement="bottom-end" className="py-0" caret={false}>
                <CIcon icon={cilUser} size="xl" />
            </CDropdownToggle>
            {renderUserMenu()}
        </CDropdown>
    )
}

export { AppHeaderDropdown }
