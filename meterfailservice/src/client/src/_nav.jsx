import React from 'react'
import CIcon from '@coreui/icons-react'
import {
  cilBell,
  cilCalculator,
  cilChartPie,
  cilCursor,
  cilDescription,
  cilDrop,
  cilNotes,
  cilPencil,
  cilPuzzle,
  cilSpeedometer,
  cilStar,
} from '@coreui/icons'
import { CNavGroup, CNavItem, CNavTitle } from '@coreui/react'

const _nav = [
  {
    component: CNavItem,
    name: 'داشبورد',
    to: '/',
    icon: <CIcon icon={cilSpeedometer} customClassName="nav-icon" />,
    badge: {
      color: 'info',
      text: 'NEW',
    },
  },
  {
    component: CNavItem,
    name: 'به روزرسانی',
    to: '/updatemeterenergy',
    icon: <CIcon icon={cilChartPie} customClassName="nav-icon" />
  },
  {
    component: CNavItem,
    name: 'انرژی',
    to: '/meterenergy',
    icon: <CIcon icon={cilCalculator} customClassName="nav-icon" />
  },
  {
    component: CNavItem,
    name: 'سلامت وبگاه',
    to: '/health-ui',
    icon: <CIcon icon={cilBell} customClassName="nav-icon" />,
  },
]

export default _nav
