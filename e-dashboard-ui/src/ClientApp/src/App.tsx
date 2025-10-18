import React from "react";
import { Outlet, useLoaderData, Await, } from "react-router-dom";
import { enableRipple, enableRtl, loadCldr, L10n, registerLicense } from '@syncfusion/ej2-base'
import { numberingSystems, persian, numbers, timeZoneNames } from './global'
import { AuthorizeProvider } from "./api-authorization";
import { UserManagerSettingsStore } from "oidc-client-ts";
import { RouterProvider } from 'react-router-dom'
import router from './router'

import { fa } from './locale';
import './scss/style.scss'

enableRipple(true);
loadCldr(numberingSystems, persian, numbers, timeZoneNames);
enableRtl(true);
L10n.load(fa);
registerLicense('ORg4AjUWIQA/Gnt2VVhiQlFadVlJXmJWf1FpTGpQdk5yd19DaVZUTX1dQl9hSXlTckVmXHtfcHNVRGM=');

const App = () => {
    const settings = useLoaderData() as UserManagerSettingsStore;

    return (
        <React.Suspense fallback={<p>بارگذاری سایت ...</p>}>
            <Await resolve={settings} errorElement={<p>ارتباط با سرور احراز هویت برقرار نیست.</p>}>
                <AuthorizeProvider settings={settings}>
                    <Outlet />
                </AuthorizeProvider>
            </Await>
        </React.Suspense>
    )
}
export default App
