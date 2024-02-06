using DV.ThingTypes;
using System.Collections.Generic;
using System.Linq;

namespace CC.Game
{
    internal class OriginalCargoDefinitions
    {
        private HashSet<CargoType> _liquids;
        private HashSet<CargoType> _oils;
        private HashSet<CargoType> _flammableLiquids;
        private HashSet<CargoType> _corosiveLiquids;
        private HashSet<CargoType> _gases;
        private HashSet<CargoType> _flammableGases;
        private HashSet<CargoType> _flammableSolids;
        private HashSet<CargoType> _radioactiveCargo;
        private HashSet<CargoType> _explosiveCargo;
        private HashSet<CargoType> _extinguishingGases;
        private HashSet<CargoType> _oxidizers;

        public OriginalCargoDefinitions()
        {
            _liquids = TrainCarAndCargoDamageProperties.Liquids.ToHashSet();
            _oils = TrainCarAndCargoDamageProperties.Oils.ToHashSet();
            _flammableLiquids = TrainCarAndCargoDamageProperties.FlammableLiquids.ToHashSet();
            _corosiveLiquids = TrainCarAndCargoDamageProperties.CorosiveLiquids.ToHashSet();
            _gases = TrainCarAndCargoDamageProperties.Gases.ToHashSet();
            _flammableGases = TrainCarAndCargoDamageProperties.FlammableGases.ToHashSet();
            _flammableSolids = TrainCarAndCargoDamageProperties.FlammableSolids.ToHashSet();
            _radioactiveCargo = TrainCarAndCargoDamageProperties.RadioactiveCargo.ToHashSet();
            _explosiveCargo = TrainCarAndCargoDamageProperties.ExplosiveCargo.ToHashSet();
            _extinguishingGases = TrainCarAndCargoDamageProperties.ExtinguishingGases.ToHashSet();
            _oxidizers = TrainCarAndCargoDamageProperties.Oxidizers.ToHashSet();
        }

        public void ResetToThis()
        {
            TrainCarAndCargoDamageProperties.Liquids = _liquids.ToHashSet();
            TrainCarAndCargoDamageProperties.Oils = _oils.ToHashSet();
            TrainCarAndCargoDamageProperties.FlammableLiquids = _flammableLiquids.ToHashSet();
            TrainCarAndCargoDamageProperties.CorosiveLiquids = _corosiveLiquids.ToHashSet();
            TrainCarAndCargoDamageProperties.Gases = _gases.ToHashSet();
            TrainCarAndCargoDamageProperties.FlammableGases = _flammableGases.ToHashSet();
            TrainCarAndCargoDamageProperties.FlammableSolids = _flammableSolids.ToHashSet();
            TrainCarAndCargoDamageProperties.RadioactiveCargo = _radioactiveCargo.ToHashSet();
            TrainCarAndCargoDamageProperties.ExplosiveCargo = _explosiveCargo.ToHashSet();
            TrainCarAndCargoDamageProperties.ExtinguishingGases = _extinguishingGases.ToHashSet();
            TrainCarAndCargoDamageProperties.Oxidizers = _oxidizers.ToHashSet();
        }
    }
}
