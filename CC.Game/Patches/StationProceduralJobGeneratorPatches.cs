using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace CC.Game.Patches
{
    [HarmonyPatch(typeof(StationProceduralJobGenerator))]
    internal class StationProceduralJobGeneratorPatches
    {
        public class GroupState
        {
            public StationProceduralJobsRuleset Rules;
            public List<CCCargoGroup> RemovedInputGroups;
            public List<CCCargoGroup> RemovedOutputGroups;

            public GroupState(StationController station)
            {
                Rules = station.proceduralJobsRuleset;
                RemovedInputGroups = new List<CCCargoGroup>();
                RemovedOutputGroups = new List<CCCargoGroup>();
            }
        }

        private static Dictionary<StationController, GroupState> s_states = new Dictionary<StationController, GroupState>();

        [HarmonyPrefix, HarmonyPatch(nameof(StationProceduralJobGenerator.GenerateJobChain))]
        public static void GenerateJobChainPrefix(StationController ___stationController)
        {
            if (s_states.ContainsKey(___stationController))
            {
                CCMod.Error("StationController was already in group state dictionary!");
                return;
            }

            var state = new GroupState(___stationController);

            // Remove input groups that do not check conditions.
            for (int i = 0; i < state.Rules.inputCargoGroups.Count; i++)
            {
                if (state.Rules.inputCargoGroups[i] is CCCargoGroup group)
                {
                    if (!group.ConditionsMet())
                    {
                        state.RemovedInputGroups.Add(group);
                        state.Rules.inputCargoGroups.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Ditto for output groups.
            for (int i = 0; i < state.Rules.outputCargoGroups.Count; i++)
            {
                if (state.Rules.outputCargoGroups[i] is CCCargoGroup group)
                {
                    if (!group.ConditionsMet())
                    {
                        state.RemovedOutputGroups.Add(group);
                        state.Rules.outputCargoGroups.RemoveAt(i);
                        i--;
                    }
                }
            }

            s_states.Add(___stationController, state);
        }

        [HarmonyPostfix, HarmonyPatch(nameof(StationProceduralJobGenerator.GenerateJobChain))]
        public static void GenerateJobChainPostfix(StationController ___stationController)
        {
            if (s_states.TryGetValue(___stationController, out GroupState state))
            {
                // Just add the groups back in after the jobs are generated.
                state.Rules.inputCargoGroups.AddRange(state.RemovedInputGroups);
                state.Rules.outputCargoGroups.AddRange(state.RemovedOutputGroups);

                s_states.Remove(___stationController);
            }
        }
    }
}
