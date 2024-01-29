using CC.Common;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;

namespace CC.Game
{
    internal static class Extensions
    {
        public static TrainCarType_v2 ToV2(this CarParentType car)
        {
            switch (car)
            {
                case CarParentType.None:
                    return TrainCarType.NotSet.ToV2().parentType;
                case CarParentType.Flatbed:
                    return TrainCarType.FlatbedEmpty.ToV2().parentType;
                case CarParentType.FlatbedStakes:
                    return TrainCarType.FlatbedStakes.ToV2().parentType;
                case CarParentType.FlatbedMilitary:
                    return TrainCarType.FlatbedMilitary.ToV2().parentType;
                case CarParentType.Autorack:
                    return TrainCarType.AutorackBlue.ToV2().parentType;
                case CarParentType.TankOil:
                    return TrainCarType.TankChrome.ToV2().parentType;
                case CarParentType.TankGas:
                    return TrainCarType.TankBlue.ToV2().parentType;
                case CarParentType.TankChem:
                    return TrainCarType.TankBlack.ToV2().parentType;
                case CarParentType.TankFood:
                    return TrainCarType.TankShortMilk.ToV2().parentType;
                case CarParentType.Stock:
                    return TrainCarType.StockBrown.ToV2().parentType;
                case CarParentType.Boxcar:
                    return TrainCarType.BoxcarBrown.ToV2().parentType;
                case CarParentType.BoxcarMilitary:
                    return TrainCarType.BoxcarMilitary.ToV2().parentType;
                case CarParentType.Refrigerator:
                    return TrainCarType.RefrigeratorWhite.ToV2().parentType;
                case CarParentType.Hopper:
                    return TrainCarType.HopperBrown.ToV2().parentType;
                case CarParentType.Gondola:
                    return TrainCarType.GondolaGray.ToV2().parentType;
                case CarParentType.Passenger:
                    return TrainCarType.PassengerBlue.ToV2().parentType;
                case CarParentType.NuclearFlask:
                    return TrainCarType.NuclearFlask.ToV2().parentType;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(car));
            }
        }
    }
}
