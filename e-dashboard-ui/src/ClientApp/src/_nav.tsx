import CIcon from '@coreui/icons-react'
import {
    // cilBell,
    // cilCalculator,
    cilChartPie,
    //cilCursor,
    cilDrop,
    //cilNotes,
    //cilPencil,
    cilPuzzle,
    cilSpeedometer,
    //cilStar,
    cilBalanceScale
} from '@coreui/icons'
import { CNavGroup, CNavItem, CNavTitle } from '@coreui/react'

const nav = [
    {
        component: CNavItem,
        name: 'داشبورد',
        to: '/',
        icon: <CIcon icon={cilSpeedometer} customClassName="nav-icon" />,
        badge: {
            color: 'info',
            text: 'جدید',
        },
    },
    {
        component: CNavItem,
        name: 'شبکه',
        to: '/network',
        icon: <CIcon icon={cilDrop} customClassName="nav-icon" />,
    },
    {
        component: CNavTitle,
        name: 'سنجش و پایش',
        roles: ["Admins", "Editors"],
    },
    {
        component: CNavGroup,
        name: 'میتر',
        to: '/meter',
        icon: <CIcon icon={cilPuzzle} customClassName="nav-icon" />,
        roles: ["Admins", "Editors"],
        items: [
            {
                component: CNavItem,
                name: 'فهرست میتر',
                roles: ["Admins"],
                to: '/meter/meterdatatable',
            },
            {
                component: CNavItem,
                name: 'فهرست انرژی میتر',
                to: '/meter/meterenegylist',
            },
            {
                component: CNavItem,
                name: 'فهرست انرژی ترانس',
                to: '/meter/transmeterenegylist',
            },
            {
                component: CNavItem,
                name: 'فهرست انرژی ایستگاه',
                to: '/meter/substationenegylist',
            },
            {
                component: CNavItem,
                name: 'بروز رسانی انرژی تجهیزات',
                to: '/meter/updateequipmentsenergy',
            },
            {
                component: CNavItem,
                name: 'بروز رسانی انرژی ایستگاه',
                to: '/meter/updatesubstation',
            },
            {
                component: CNavItem,
                name: 'بروز رسانی انرژی تجهیز',
                to: '/meter/updateequipmentenergy',
            },
            {
                component: CNavItem,
                name: 'بارگیری انرژی تجهیزات',
                to: '/meter/updatemeterenergy',
            },
            {
                component: CNavItem,
                name: 'بارگیری انرژی تجهیز',
                to: '/meter/updateequipmentmeterenergy',
            },
            {
                component: CNavItem,
                name: 'بارگیری انرژی از فایل',
                to: '/meter/uploadmeter',
            },
            {
                component: CNavItem,
                name: 'بارگیری میتر',
                to: '/meter/igmcread',
            },
        ],
    },
    {
        component: CNavTitle,
        name: 'گزارش نموداری',
        roles: ["Admins", "Editors"],
    },
    {
        component: CNavGroup,
        name: 'نمودار نیروگاهها',
        to: '/charts/powerplant/',
        icon: <CIcon icon={cilChartPie} customClassName="nav-icon" />,
        roles: ["Admins", "Editors"],
        items: [
            {
                component: CNavItem,
                name: 'تولید ساعتی واحد',
                to: '/charts/powerplant/unitgenerationhourly',
            },
            {
                component: CNavItem,
                name: 'تولید روزانه نیروگاه',
                to: '/charts/powerplant/daily',
            },
            {
                component: CNavItem,
                name: 'تولید ماهیانه واحد',
                to: '/charts/powerplant/unitgenerationmonthly',
            },
            {
                component: CNavItem,
                name: 'تولید ماهیانه نیروگاه',
                to: '/charts/powerplant/monthly',
            },
            {
                component: CNavItem,
                name: 'تولید سالیانه نیروگاه',
                to: '/charts/powerplant/totalenergy',
            },
            {
                component: CNavItem,
                name: 'تولید سالیانه واحد',
                to: '/charts/powerplant/unitgenerationyear',
            },
        ],
    },
    {
        component: CNavGroup,
        name: 'نمودار بارهای صنعتی',
        to: '/charts/industrial/',
        icon: <CIcon icon={cilChartPie} customClassName="nav-icon" />,
        roles: ["Admins", "Editors"],
        items: [
            {
                component: CNavItem,
                name: 'سالیانه',
                to: '/charts/industrial/yearly',
            },
            {
                component: CNavItem,
                name: 'ماهیانه',
                to: '/charts/industrial/monthly',
            },
            {
                component: CNavItem,
                name: 'روزانه',
                to: '/charts/industrial/daily',
            },

        ],

    },
    {
        component: CNavTitle,
        name: 'گزارش جدولی',
        roles: ["Admins", "Editors"],
    },
    {
        component: CNavGroup,
        name: 'گزارش',
        to: '/reports',
        icon: <CIcon icon={cilChartPie} customClassName="nav-icon" />,
        roles: ["Admins", "Editors"],
        items: [
            {
                component: CNavItem,
                name: 'نواقص قرائت',
                to: '/reports/meterfailure',
            },
            {
                icon: <CIcon icon={cilBalanceScale} customClassName="nav-icon" />,
                component: CNavItem,
                name: 'تراز انرژی',
                to: '/reports/energybalance',
            },
            {
                component: CNavItem,
                name: 'تراز انرژی ایستگاه',
                to: '/reports/substationenergybalance',
            },
            {
                component: CNavItem,
                name: 'انرژی ساعتی ایستگاه',
                to: '/reports/stationenergy',
            },
            {
                component: CNavItem,
                name: 'انرژی روزانه ایستگاه',
                to: '/reports/substation/energydaily',
            },
            {
                component: CNavItem,
                name: 'انرژی ماهیانه ایستگاه',
                to: '/reports/substation/energymonthly',
            },
            {
                component: CNavItem,
                name: 'انرژی ساعتی نیروگاه',
                to: '/reports/powerplant/energy',
            },
            {
                component: CNavItem,
                name: 'انرژی ماهیانه نیروگاه',
                to: '/reports/powerplant/energymonthly',
            },
        ],

    },
    {
        component: CNavTitle,
        name: 'تنظیمات',
        roles: ["Admins"],
    },
    {
        component: CNavGroup,
        name: 'تنظیمات',
        to: '/setting',
        icon: <CIcon icon={cilChartPie} customClassName="nav-icon" />,
        roles: ["Admins"],
        items: [
            {
                component: CNavItem,
                name: 'فهرست مشترکین',
                to: '/setting/loadfeeder',
                roles: ["Admins"],
            },
            {
                component: CNavItem,
                name: 'فهرست بهره بردار نیروگاه',
                to: '/setting/powerplants',
                roles: ["Admins"],
            },
            {
                component: CNavItem,
                name: 'تعرفه انرژی',
                to: '/setting/energytariff',
                roles: ["Admins"],
            },
        ],
    },
    {
        component: CNavItem,
        name: 'سلامت وبگاه',
        to: '/health-ui',
        icon: <CIcon icon={cilDrop} customClassName="nav-icon" />,
    },
    {
        component: CNavItem,
        name: 'ارسال پیام',
        to: '/sendmessage',
        icon: <CIcon icon={cilDrop} customClassName="nav-icon" />,
    },
    {
        component: CNavItem,
        name: 'درباره',
        to: '/about',
        icon: <CIcon icon={cilDrop} customClassName="nav-icon" />,
    },

]

export default nav;

