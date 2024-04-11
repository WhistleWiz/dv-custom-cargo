using CC.Common.Conditions;
using DV.ServicePenalty;
using System;

namespace CC.Game
{
    internal static class ConditionChecker
    {
        public static bool CheckCondition<T>(T condition)
            where T : CargoCondition
        {
            return condition switch
            {
                ConditionIRLTime irlTime => IRLTimeChecker(irlTime),
                ConditionDebt debt => DebtChecker(debt),
                _ => Fail(condition)
            };
        }

        private static bool Fail(CargoCondition condition)
        {
            CCMod.Log($"FAILED (${condition.GetType()})");
            return true;
        }

        private static bool IRLTimeChecker(ConditionIRLTime c)
        {
            var time = DateTime.Now;
            bool loopYear = c.EndMonth < c.StartMonth;

            if (!c.RepeatMonthly)
            {
                if (loopYear)
                {
                    if (time.Month < (int)c.StartMonth && time.Month > (int)c.EndMonth)
                    {
                        return false;
                    }
                }
                else
                {
                    if (time.Month < (int)c.StartMonth || time.Month > (int)c.EndMonth)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool DebtChecker(ConditionDebt c)
        {
            float debt = CareerManagerDebtController.Instance.GetAllDebtsPrice();

            switch (c.When)
            {
                case Common.Interval.MoreThan:
                    return debt > c.Debt;
                case Common.Interval.EqualOrMore:
                    return debt >= c.Debt;
                case Common.Interval.Equal:
                    return debt == c.Debt;
                case Common.Interval.EqualOrLess:
                    return debt <= c.Debt;
                case Common.Interval.LessThan:
                    return debt < c.Debt;
                default:
                    break;
            }

            return true;
        }
    }
}
