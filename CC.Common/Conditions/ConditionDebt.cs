using UnityEngine;

namespace CC.Common.Conditions
{
    [CreateAssetMenu(menuName = "DVCustomCargo/Cargo Spawn Condition (Player Debt)")]
    public class ConditionDebt : CargoCondition
    {
        public override string ConditionType => "Debt";

        public float Debt;
        public Interval When;
    }
}
