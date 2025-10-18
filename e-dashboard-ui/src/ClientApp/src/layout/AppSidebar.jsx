import React from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { CSidebar, CSidebarBrand, CSidebarNav, CSidebarToggler } from '@coreui/react'
import SimpleBar from 'simplebar-react'
import CIcon from '@coreui/icons-react'
import { useAuthorize } from '../api-authorization/AuthorizeProvider'
import { AppSidebarNav } from './AppSidebarNav'
import { logoNegative } from '../assets/brand/logo-negative'
import { sygnet } from '../assets/brand/sygnet'
import 'simplebar-react/dist/simplebar.min.css'
import navigation from '../_nav';

const AppSidebar = () => {
    const dispatch = useDispatch()
    const unfoldable = useSelector((state) => state.sidebarUnfoldable)
    const sidebarShow = useSelector((state) => state.sidebarShow)
    const authService = useAuthorize();

    if (authService.isLoading)
        return (<div></div>)

    const role = authService.user?.profile.role;
    
    return (
        <CSidebar
            position="fixed"
            unfoldable={unfoldable}
            visible={sidebarShow}
            onVisibleChange={(visible) => {
                dispatch({ type: 'set', sidebarShow: visible })
            }}
        >
            <CSidebarBrand className="d-none d-md-flex" to="/">
                <CIcon className="sidebar-brand-full" icon={logoNegative} height={55} />
                <CIcon className="sidebar-brand-narrow" icon={sygnet} height={55} />
            </CSidebarBrand>
            <CSidebarNav>
                <SimpleBar>
                    <AppSidebarNav items={navigation} role={role} />
                </SimpleBar>
            </CSidebarNav>
            <CSidebarToggler
                className="d-none d-lg-flex"
                onClick={() => dispatch({ type: 'set', sidebarUnfoldable: !unfoldable })}
            />
        </CSidebar>
    )
}

export default React.memo(AppSidebar)
