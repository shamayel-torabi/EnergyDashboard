import { LoaderFunctionArgs, createBrowserRouter, defer } from 'react-router-dom'

import DefaultLayout from './layout/DefaultLayout'
import ErrorPage from './ErrorPage'
import ApiAuthorzationRoutes from './api-authorization/ApiAuthorizationRoutes'

import Dashboard from './views/Dashboard/Dashboard'
import Network from './views/Diagram/Network'
import NetworkDiagram from './views/Diagram/NetworkDiagram'

import ReadMeter from './views/Meter/ReadMeter'
import MeterDataTable from './views/Meter/MeterDataTable'
import MeterEnergyList from './views/Meter/MeterEnergyList'
import TransformersMeterEnergyList from './views/Meter/TransformersMeterEnergyList'
import SubstationMeterEnergyList from './views/Meter/SubstationMeterEnergyList'
import UploadMeterEnergy from './views/Meter/UploadMeterEnergy'
import UpdateEnergyProfile from './views/Meter/UpdateEnergyProfile'
import UpdateEquipmentEnergyProfile from './views/Meter/UpdateEquipmentEnergyProfile'
import UpdateMeterEnergy from './views/Meter/UpdateMeterEnergy'
import UpdateEquipmentMeterEnergy from './views/Meter/UpdateEquipmentMeterEnergy'
import UpdateSubstationEquipmentsEnergy from './views/Meter/UpdateSubstationEquipmentsEnergy'

import TotalEnergyChart from './views/Charts/TotalEnergyChart'
import PowerPlantYear from './views/Charts/PowerPlant/PowerPlantYear'
import UnitGenerationHourly from './views/Charts/PowerPlant/UnitGenerationHourly'
import UnitGenerationMonthly from './views/Charts/PowerPlant/UnitGenerationMonthly'
import UnitGenerationYear from './views/Charts/PowerPlant/UnitGenerationYear'
import PowerPlantDaily from './views/Charts/PowerPlant/PowerPlantDaily'
import PowerPlantMonthly from './views/Charts/PowerPlant/PowerPlantMonthly'

import IndustrialYear from './views/Charts/Industrial/IndustrialYear'
import IndustrialMonthly from './views/Charts/Industrial/IndustrialMonthly'
import IndustrialDaily from './views/Charts/Industrial/IndustrialDaily'

import MeterFailure from './views/Reports/MeterFailure/MeterFailure'
import EnergyBalance from './views/Reports/DailyReport/EnergyBalance'
import SubstationEnergyBalance from './views/Reports/DailyReport/SubstationEnergyBalance'
import StationEnergy from './views/Reports/Substation/StationEnergy'
import SubstationEnergy from './views/Reports/Substation/SubstationEnergy'
import SubstationEnergyDaily from './views/Reports/Substation/SubstationEnergyDaily'
import SubstationEnergyMonthly from './views/Reports/Substation/SubstationEnergyMonthly'
import EnergyTable from './views/Reports/PowerPlant/EnergyTable'
import ReportMonthly from './views/Reports/PowerPlant/ReportMonthly'
import LoadFeederTypes from './views/Setting/LoadFeederTypes'
import PowerplantOperators from './views/Setting/PowerplantOperators'
import EnergyTariff from './views/Setting/EnergyTariff'
import AddEnergyTariffForm from './views/Setting/AddEnergyTariffForm'

import HealthUI from './views/Health/HealthUI'
import SendMessage from './views/Message/SendMessage'
import About from './views/Pages/About/About'
import App from './App'
import RequireAuth from './api-authorization/RequireAuth'

import { WebStorageStateStore } from 'oidc-client-ts';
import { ApplicationPaths, ApplicationName } from './api-authorization';


async function loadUserManager(arg: LoaderFunctionArgs) {
  try {
    const request: Request = new Request(ApplicationPaths.ApiAuthorizationClientConfigurationUrl, { signal: arg.request.signal })
    const response = await fetch(request);

    if (!response.ok) {
      throw new Error(`Could not load settings for '${ApplicationName}'`);
    }

    let settings = await response.json();
    settings.automaticSilentRenew = true;
    settings.includeIdTokenInSilentRenew = true;
    settings.loadUserInfo = true;
    settings.userStore = new WebStorageStateStore({
      prefix: ApplicationName
    });

    return defer(settings)
  }
  catch (e) {
    throw new Error(`Could connect to '${ApplicationPaths.ApiAuthorizationClientConfigurationUrl}'`);
  }
}

const router = createBrowserRouter([
  {
    element: <App />,
    errorElement: <ErrorPage />,
    loader: loadUserManager,
    children: [
      {
        path: "/",
        element: <DefaultLayout />,
        children: [
          {
            index: true,
            element: <Dashboard />,
          },
          {
            path: "network",
            children: [
              {
                index: true,
                element: <Network />
              },
              {
                path: ":diagramId",
                element: <NetworkDiagram />
              },
            ]
          },
          {
            path: "meter",
            children: [
              {
                index: true,
                element: <RequireAuth><ReadMeter /></RequireAuth>
              },
              {
                path: "meterdatatable",
                element: <MeterDataTable />
              },
              {
                path: "meterenegylist",
                element: <MeterEnergyList />
              },
              {
                path: "transmeterenegylist",
                element: <TransformersMeterEnergyList />
              },
              {
                path: "substationenegylist",
                element: <SubstationMeterEnergyList />
              },
              {
                path: "igmcread",
                element: <RequireAuth><ReadMeter /></RequireAuth>
              },
              {
                path: "uploadmeter",
                element: <RequireAuth><UploadMeterEnergy /></RequireAuth>
              },
              {
                path: "updateequipmentsenergy",
                element: <RequireAuth><UpdateEnergyProfile /></RequireAuth>
              },
              {
                path: "updateequipmentenergy",
                element: <RequireAuth><UpdateEquipmentEnergyProfile /></RequireAuth>
              },
              {
                path: "updatemeterenergy",
                element: <RequireAuth><UpdateMeterEnergy /></RequireAuth>
              },
              {
                path: "updateequipmentmeterenergy",
                element: <RequireAuth><UpdateEquipmentMeterEnergy /></RequireAuth>
              },
              {
                path: "updatesubstation",
                element: <RequireAuth><UpdateSubstationEquipmentsEnergy /></RequireAuth>
              },
            ]
          },
          {
            path: "charts",
            children: [
              {
                index: true,
                element: <TotalEnergyChart />
              },
              {
                path: "powerplant",
                children: [
                  {
                    index: true,
                    element: <PowerPlantYear />
                  },
                  {
                    path: "totalenergy",
                    element: <PowerPlantYear />
                  },
                  {
                    path: "daily",
                    element: <PowerPlantDaily />
                  },
                  {
                    path: "monthly",
                    element: <PowerPlantMonthly />
                  },
                  {
                    path: "unitgenerationhourly",
                    element: <UnitGenerationHourly />
                  },
                  {
                    path: "unitgenerationmonthly",
                    element: <UnitGenerationMonthly />
                  },
                  {
                    path: "unitgenerationyear",
                    element: <UnitGenerationYear />
                  },
                ]
              },
              {
                path: "industrial",
                children: [
                  {
                    index: true,
                    element: <IndustrialYear />
                  },
                  {
                    path: "yearly",
                    element: <IndustrialYear />
                  },
                  {
                    path: "monthly",
                    element: <IndustrialMonthly />
                  },
                  {
                    path: "daily",
                    element: <IndustrialDaily />
                  },
                ]
              },
            ]
          },
          {
            path: "reports",
            children: [
              {
                index: true,
                element: <MeterFailure />
              },
              {
                path: "meterfailure",
                element: <MeterFailure />
              },
              {
                path: "energybalance",
                element: <EnergyBalance />
              },
              {
                path: "substationenergybalance",
                element: <SubstationEnergyBalance />
              },
              {
                path: "stationenergy",
                element: <StationEnergy />,
              },
              {
                path: "substationenergy/:substationId",
                element: <SubstationEnergy />,
              },
              {
                path: "substation",
                children: [
                  {
                    index: true,
                    element: <SubstationEnergyDaily />
                  },
                  {
                    path: "energydaily",
                    element: <SubstationEnergyDaily />
                  },
                  {
                    path: "energymonthly",
                    element: <SubstationEnergyMonthly />
                  }
                ]
              },
              {
                path: "powerplant",
                children: [
                  {
                    index: true,
                    element: <EnergyTable />
                  },
                  {
                    path: "energy",
                    element: <EnergyTable />
                  },
                  {
                    path: "energymonthly",
                    element: <ReportMonthly />
                  }
                ]
              }
            ]
          },
          {
            path: "setting",
            children: [
              {
                index: true,
                element: <RequireAuth><LoadFeederTypes /></RequireAuth>
              },
              {
                path: "loadfeeder",
                element: <RequireAuth><LoadFeederTypes /></RequireAuth>
              },
              {
                path: "powerplants",
                element: <RequireAuth><PowerplantOperators /></RequireAuth>
              },
              {
                path: "energytariff",
                element: <RequireAuth><EnergyTariff /></RequireAuth>
              },
              {
                path: "addenergytariff",
                element: <EnergyTariff><AddEnergyTariffForm /></EnergyTariff>
              },
            ]
          },
          {
            path: "health-ui",
            element: <HealthUI />
          },
          {
            path: "sendmessage",
            element: <SendMessage />
          },
          {
            path: "about",
            element: <About />
          }
        ],
      },
      ...ApiAuthorzationRoutes
    ]
  },
]);

export default router
