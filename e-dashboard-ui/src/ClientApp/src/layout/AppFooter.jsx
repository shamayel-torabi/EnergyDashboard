import React from 'react'
import { CFooter } from '@coreui/react'

const AppFooter = () => {
    const options = { year: 'numeric'};
    const dateTimeFormat = new Intl.DateTimeFormat('fa-IR', options);
    const now = new Date();
    const parts = dateTimeFormat.formatToParts(now);
    const year = parts.map(p => p.value)[0];

    return (
        <CFooter>
            <div>
                <a href="http://www.hrec.co.ir" target="_blank" rel="noopener noreferrer">شرکت سهامی برق منطقه ای هرمزگان</a>
                <span className="ms-1">&copy; {year}</span>
            </div>
            <div className="ms-auto">
                <span className="me-1">کلیه حقوق محفوظ برای بازار برق</span>
            </div>
        </CFooter>
    )
}

export default React.memo(AppFooter)
