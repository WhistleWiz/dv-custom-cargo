using UnityEngine;

namespace CC.Common
{
    public enum CarParentType
    {
        None            = 0,
        Flatbed         = 200,
        FlatbedStakes   = 201,
        FlatbedMilitary = 202,
        UtilityFlatbed  = 220,
        Autorack        = 250,
        TankOil         = 301,
        TankGas         = 300,
        TankChem        = 305,
        TankFood        = 325,
        Stock           = 350,
        Boxcar          = 400,
        BoxcarMilitary  = 404,
        Refrigerator    = 450,
        Hopper          = 500,
        CoveredHopper   = 510,
        Gondola         = 550,
        Passenger       = 600,
        NuclearFlask    = 800,
        [InspectorName("DM1U-150 (Utility Rail Vehicle)")]
        DM1U150         = 35,
    }
}
