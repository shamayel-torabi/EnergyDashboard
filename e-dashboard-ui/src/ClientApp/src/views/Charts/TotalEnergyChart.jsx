import IndustrialYear from "./Industrial/IndustrialYear"
import PowerPlantYear from "./PowerPlant/PowerPlantYear"

const TotalEnergyChart = () =>{
    return(
        <div>
            <PowerPlantYear />
            <br/>
            <IndustrialYear />
        </div>
    )
}

export default TotalEnergyChart