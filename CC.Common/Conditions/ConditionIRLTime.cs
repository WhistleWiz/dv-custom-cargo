using UnityEngine;

namespace CC.Common.Conditions
{
    [CreateAssetMenu(menuName = "DVCustomCargo/Cargo Spawn Condition (IRL Time)")]
    public class ConditionIRLTime : CargoCondition
    {
        public override string ConditionType => "IRLTime";

        public bool RepeatMonthly = false;
        public bool RepeatDaily = false;

        public Month StartMonth = Month.January;
        public int StartDay = 1;
        [Range(0, 23)]
        public int StartHour = 0;
        [Range(0, 59)]
        public int StartMinute = 0;

        public Month EndMonth = Month.January;
        public int EndDay = 1;
        [Range(0, 23)]
        public int EndHour = 0;
        [Range(0, 59)]
        public int EndMinute = 0;

        public enum Month
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December,
        }
    }
}
