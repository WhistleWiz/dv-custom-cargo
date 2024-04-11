using CC.Common.Conditions;
using DV.ThingTypes;
using System.Collections.Generic;
using static Oculus.Avatar.CAPI;

namespace CC.Game
{
    internal class CCCargoGroup : CargoGroup
    {
        public CargoCondition[] Conditions { get; }
        public CargoCondition.ConditionMode ConditionMode { get; }

        public CCCargoGroup(List<CargoType> cargoTypes, List<StationController> stations, CargoCondition[] conditions, CargoCondition.ConditionMode mode) :
            base(cargoTypes, stations)
        {
            Conditions = conditions;
            ConditionMode = mode;
        }

        public bool ConditionsMet()
        {
            bool flag = true;

            foreach (var c in Conditions)
            {
                flag = ConditionChecker.CheckCondition(c);

                if (flag)
                {
                    switch (ConditionMode)
                    {
                        case CargoCondition.ConditionMode.Any:
                            return true;
                        default:
                            continue;
                    }
                }
                else
                {
                    switch (ConditionMode)
                    {
                        case CargoCondition.ConditionMode.All:
                            return false;
                        default:
                            continue;
                    }
                }
            }

            return flag;
        }
    }
}
