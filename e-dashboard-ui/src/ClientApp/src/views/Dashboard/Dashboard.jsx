import { useRef, useEffect, useState } from 'react';
import { CRow, CCol } from '@coreui/react';
import { TotalEnergyChart } from '../Charts/PowerPlant/TotalEnergyChart';
import { TotalIndustrialEnergyChart } from '../Charts/Industrial/TotalIndustrialEnergyChart';
import { Welcome } from './Welcome';
import { PowerPlants } from './PowerPlants';
import { Generations } from './Generations';
import { Industrial } from './Industrial';
import { Distribution } from './Distribution';
import { Tabadol } from './Tabadol';
import { PersianCalendarUtil } from '../../components/PersianCalendarUtil';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useToast } from '../../components';
import { useAuthorize } from '../../api-authorization';
import { apiUrl, notificationUrl } from '../../ApiConfig';

const Dashboard = (props) => {
    const hYear = PersianCalendarUtil.hijriYear(new Date());

    const [loading, setLoading] = useState(false);
    const [total, setTotal] = useState({
        generation: 0,
        distribution: 0,
        industrial: 0,
        tabadol: 0,
    });
    const [powerPlants, setPowerPlants] = useState([]);
    const toast = useToast();
    const authService = useAuthorize();
    const hubConnection = useRef(null);

    useEffect(() => {
        const url = `${notificationUrl}/message`;

        const getTotalInstantPower = async () => {
            const url = `${apiUrl}/MeterInstants/GetTotalInstantPowers`;
    
            try {
                let response = await fetch(url);
                if (response.ok) {
                    let t = await response.json();
                    setTotal(t);
                }
                else {
                    const problem = await response.json();
                    console.error(problem);
                    toast.showToast("error", "خطا در دریافت مقادیر لحظه ای توان .")
                }
            }
            catch (e) {
                toast.showToast("error", "خطا اتصال به وب سرور .");
                console.error(e)
            }
        }
    
        const getPowerPlantInstantPowers = async () => {
            const url = `${apiUrl}/MeterInstants/GetPowerPlantInstantPowers`;
    
            try {
                let response = await fetch(url);
                if (response.ok) {
                    let pps = await response.json();
                    setPowerPlants(pps);
                }
                else {
                    const problem = await response.json();
                    console.error(problem);
                    toast.showToast("error", "خطا در دریافت توان لحظه ای نیروگاه .");
                }
            }
            catch (e) {
                toast.showToast("error", "خطا اتصال به وب سرور .");
                console.error(e);
            }
        }
    
        const getData = async () => {
            setLoading(true);
            await getPowerPlantInstantPowers();
            await getTotalInstantPower();
            setLoading(false);
        }
   

        hubConnection.current = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Error)
            .build();

        hubConnection.current.on('refreshMeterInstant', async () => {
            await getData();
        });

        hubConnection.current.start()
            .catch(err => console.error('Error while establishing connection :('));

        getData();

        return () => {
            if (hubConnection.current)
                hubConnection.current.stop();
        };
    }, [toast]);

    const spanLoading = loading ? <span className="k-icon k-i-loading"></span> : null;

    if (authService.isLoading)
        return (<span className="k-icon k-i-loading"></span>);

    if (authService.isAuthenticated) {
        return (
            <div className="animated fadeIn">
                <CRow className="mb-2">
                    <CCol sm="12" md="6" lg="3">
                        <Generations total={total.generation} />
                    </CCol>
                    <CCol sm="12" md="6" lg="3">
                        <Industrial total={total.industrial} />
                    </CCol>
                    <CCol sm="12" md="6" lg="3">
                        <Distribution total={total.distribution} />
                    </CCol>
                    <CCol sm="12" md="6" lg="3">
                        <Tabadol total={total.tabadol} />
                    </CCol>
                </CRow>

                <TotalEnergyChart year={hYear} />
                <TotalIndustrialEnergyChart year={hYear} />
                <PowerPlants powerPlants={powerPlants} />
                {spanLoading}
            </div>
        );
    }
    else {
        return (<Welcome />);
    }
}

export default Dashboard;
