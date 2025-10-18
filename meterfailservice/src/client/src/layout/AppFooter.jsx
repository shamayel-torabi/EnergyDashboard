import React from 'react'
import { CFooter } from '@coreui/react'

const AppFooter = () => {
  const dateTimeFormat = new Intl.DateTimeFormat('fa-IR', { year: 'numeric' });
  const parts = dateTimeFormat.formatToParts(new Date());
  const partValues = parts.map(p => p.value);
  const year = partValues[0]

  return (
    <CFooter>
      <div>
        <a href="https://www.hrec.co.ir" target="_blank" rel="noopener noreferrer">
          برق منطقه ای هرمزگان
        </a>
        <span className="ms-1">&copy;  {year} </span>
      </div>
    </CFooter>
  )
}

export default React.memo(AppFooter)
