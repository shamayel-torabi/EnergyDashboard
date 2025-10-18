import { createBrowserRouter } from "react-router-dom";
import App from './App'
import Home from "./pages/Home";
import MeterEnergyList from './pages/MeterEnergyList';
import UpdateEnergy  from "./pages/UpdateEnergy";
import HealthUI from "./pages/HealthUI";
const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            { index: true, element: <Home /> },
            { path: 'meterenergy', element: <MeterEnergyList /> },
            { path: 'updatemeterenergy', element: <UpdateEnergy /> },
            { path: 'health-ui', element: <HealthUI /> },
        ]
    },
]);

export default router;