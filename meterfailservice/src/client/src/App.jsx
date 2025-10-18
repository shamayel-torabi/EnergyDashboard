import { Provider } from 'react-redux'
import store from './store'
import { enableRipple, enableRtl, loadCldr, L10n, registerLicense } from '@syncfusion/ej2-base';
import { numberingSystems, persian, numbers, timeZoneNames } from './global';
import { fa } from './locale';
import DefaultLayout from './layout/DefaultLayout';
import "./scss/style.scss"

enableRipple(true);
loadCldr(numberingSystems, persian, numbers, timeZoneNames);
enableRtl(true);
L10n.load(fa);
registerLicense('ORg4AjUWIQA/Gnt2VVhiQlFadVlJXmJWf1FpTGpQdk5yd19DaVZUTX1dQl9hSXlTckVmXHtfcHNVRGM=');

function App() {
  return (
      <Provider store={store}>
          <DefaultLayout />
      </Provider>
  )
}

export default App
