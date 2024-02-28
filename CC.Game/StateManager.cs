namespace CC.Game
{
    internal class StateManager
    {
        private static OriginalCargoDefinitions? s_og = null;

        public static bool IsLoaded { get; private set; } = false;
        public static bool IsInjected { get; set; }

        static StateManager()
        {
            // So every time a save is loaded we can make sure
            // the patch is run again.
            UnloadWatcher.UnloadRequested += OnUnload;
        }

        public static void RequestLoad()
        {
            if (!IsLoaded)
            {
                OnLoad();
            }
        }

        public static void RequestUnload()
        {
            if (IsLoaded)
            {
                OnUnload();
            }
        }

        private static void OnLoad()
        {
            if (s_og == null)
            {
                CCMod.Log("Adding cargo effects to caches...");
                s_og = new OriginalCargoDefinitions();
            }

            IsLoaded = false;
        }

        private static void OnUnload()
        {
            CCMod.Log("Unloading custom cargo...");

            IsLoaded = false;
            IsInjected = false;
            s_og?.ResetToThis();
            s_og = null;
        }
    }
}
