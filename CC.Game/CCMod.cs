using System.Reflection;
using DVLangHelper.Runtime;
using HarmonyLib;
using UnityModManagerNet;

namespace CC.Game
{
	public static class CCMod
    {
        public const string Guid = "wiz.customcargo";
        public static UnityModManager.ModEntry Instance { get; private set; } = null!;
        public static TranslationInjector Translations { get; private set; } = null!;

        // Unity Mod Manager Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = modEntry;
            Translations = new TranslationInjector(Guid);

            ScanMods();
            UnityModManager.toggleModsListen += HandleModToggled;

            var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			return true;
		}

		private static void ScanMods()
		{
            foreach (var mod in UnityModManager.modEntries)
            {
                if (mod.Active)
                {
                    CargoManager.LoadCargos(mod);
                }
            }
        }

        private static void HandleModToggled(UnityModManager.ModEntry modEntry, bool newState)
        {
            if (newState)
            {
                CargoManager.LoadCargos(modEntry);
            }
        }

        public static void Log(string message)
		{
			Instance.Logger.Log(message);
		}

        public static void Warning(string message)
        {
            Instance.Logger.Warning(message);
        }

        public static void Error(string message)
        {
            Instance.Logger.Error(message);
        }
    }
}
