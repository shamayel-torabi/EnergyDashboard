import React from 'react'
import { useSelector, useDispatch } from 'react-redux'
import {
    CContainer,
    CHeader,
    CHeaderBrand,
    CHeaderNav,
    CHeaderToggler,
} from '@coreui/react'
import CIcon from '@coreui/icons-react'
import { cilMenu } from '@coreui/icons'
import { AppHeaderDropdown } from './header/AppHeaderDropdown'
import { logo } from '../assets/brand/logo'

const AppHeader = () => {
    const dispatch = useDispatch()
    const sidebarShow = useSelector((state) => state.sidebarShow)

    return (
        <CHeader position="sticky">
            <CContainer fluid>
                <CHeaderToggler
                    className="ps-1"
                    onClick={() => dispatch({ type: 'set', sidebarShow: !sidebarShow })}
                >
                    <CIcon icon={cilMenu} size="lg" />
                </CHeaderToggler>
                <CHeaderBrand className="mx-auto d-md-none" to="/">
                    <CIcon icon={logo} height={48} alt="Logo" />
                </CHeaderBrand>
                <CHeaderNav className="ms-3">
                    <AppHeaderDropdown />
                </CHeaderNav>
            </CContainer>
        </CHeader>
    )
}

export default React.memo(AppHeader)
