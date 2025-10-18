import { CRUDService } from '../../../../services'
import {apiUrl} from '../../../../ApiConfig';

class UtilityServiceClass {
    constructor() {
        this.powerPlantTypes = null;
        this.powerplantOperators = null;
        this.loadFeederTypes = null;
        this.substations = null;
        this.igmcSubstations = null;

        this.getPowerPlantTypes();
        this.getPowerplantOperators();
        this.getLoadFeederTypes();
        this.getSubstations();
        this.getIGMCSubstations();
    }


    get PowerPlantTypes() {
        return this.powerPlantTypes;
    }

    get PowerplantOperators() {
        return this.powerplantOperators;
    }

    get LoadFeederTypes() {
        return this.loadFeederTypes;
    }

    get Substations() {
        return this.substations;
    }

    get IGMCSubstations() {
        return this.igmcSubstations;
    }


    getPowerPlantTypes = async () => {
        let service = new CRUDService(`${apiUrl}/Utility/GetPowerplantTypes`);
        try {
            this.powerPlantTypes = await service.readAll();
        } catch (e) {
            console.error(e);
        }
    }

    getPowerplantOperators = async () => {
        let service = new CRUDService(`${apiUrl}/Utility/GetPowerplantOperators`);
        try {
            this.powerplantOperators = await service.readAll();
        } catch (e) {
            console.error(e);
        }
    }

    getLoadFeederTypes = async () => {
        let service = new CRUDService(`${apiUrl}/Utility/GetLoadFeederTypes`);
        try {
            let m = await service.readAll();
            let mm = m.map(a => {
                let items = a.items.map((item) => {
                    return { value: item.id, title: item.name };
                })
                return { value: a.value, title: a.title, items: items }
            });

            this.loadFeederTypes = mm;
        } catch (e) {
            console.error(e);
        }
    }

    getSubstations = async () => {
        let service = new CRUDService(`${apiUrl}/Utility/GetSubstations`);
        try {
            this.substations = await service.readAll();
        } catch (e) {
            console.error(e);
        }
    }

    getIGMCSubstations = async () => {
        let service = new CRUDService(`${apiUrl}/Utility/GetIGMCSubstations`);
        try {
            this.igmcSubstations = await service.readAll();
        } catch (e) {
            console.error(e);
        }
    }

    getSubstationMeters = async (substationId) => {
        let service = new CRUDService(`${apiUrl}/Utility/GetSubstationMeters/${substationId}`);
        try {
            return await service.readAll();
        } catch (e) {
            console.error(e);
        }
    }
}

const UtilityService = new UtilityServiceClass();

export { UtilityService}