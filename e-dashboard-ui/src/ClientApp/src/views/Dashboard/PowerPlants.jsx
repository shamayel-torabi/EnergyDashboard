import { RadialGauge } from '@progress/kendo-react-gauges';
import { CCard, CCardBody, CCardHeader, CRow, CCol } from '@coreui/react';
import PropTypes from 'prop-types'

const UnitInstantPowerGauge = ({ instantPower }) => {
    let minorUnit, majorUnit, capacity;
    if (instantPower.capacity <= 25) {
        minorUnit = 1;
        majorUnit = 5;
    }
    else if (instantPower.capacity > 25 && instantPower.capacity <= 50) {
        minorUnit = 1;
        majorUnit = 10;
    }
    else {
        minorUnit = 5;
        majorUnit = 25;
    }

    if (instantPower.capacity === 0)
        capacity = 100;
    else
        capacity = instantPower.capacity;

    const radialOptions = {
        pointer: {
            value: instantPower.activePower,
        },
        scale: {
            minorUnit: minorUnit,
            majorUnit: majorUnit,
            max: capacity,
        }
    };

    return (
        <RadialGauge {...radialOptions} />
    );
}

UnitInstantPowerGauge.propTypes = {
    instantPower: PropTypes.object.isRequired
}

const PowerPlantGauges = ({ unitInstantPowers }) => {
    const renderpowerPlantInstant = () => {
        return unitInstantPowers.map((v, k) => {
            return (
                <CCol key={k} sm="12" md="6" lg="3">
                    <CCard>
                        <CCardBody className="pb-3">
                            <div className="text-center">
                                <UnitInstantPowerGauge instantPower={v} />
                                <strong >{`${v.name} : ${v.activePower.toLocaleString('fa-IR')} Mw`}</strong>
                            </div>
                        </CCardBody>
                    </CCard>
                </CCol>
            )
        });
    }

    return (
        <CRow>
            {renderpowerPlantInstant()}
        </CRow>
    )
}

PowerPlantGauges.propTypes = {
    unitInstantPowers: PropTypes.array.isRequired
}

const PowerPlants = ({ powerPlants }) => {
    return powerPlants.map((v, k) => {
        return (
            <CRow key={k} className="mb-2">
                <CCol lg="12">
                    <CCard>
                        <CCardHeader>
                            <strong>{`تولید نیروگاه ${v.powerPlantName}`}</strong>
                        </CCardHeader>
                        <CCardBody className="pb-3">
                            <PowerPlantGauges unitInstantPowers={v.unitInstantPowers} />
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        )
    });
}

PowerPlantGauges.propTypes = {
    powerPlants: PropTypes.array
}


export { PowerPlants }
