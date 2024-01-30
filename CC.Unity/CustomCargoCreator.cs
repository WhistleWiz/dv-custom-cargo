using CC.Common;
using UnityEditor;
using UnityEngine;

namespace CC.Unity
{
    [CreateAssetMenu(menuName = "DVCustomCargo/Custom Cargo")]
    internal class CustomCargoCreator : ScriptableObject
    {
        public CustomCargo Cargo = new CustomCargo();
        public ModelsForVanillaCar[] Models = new ModelsForVanillaCar[0];

        public void ExportModels()
        {
            Debug.Log("Exporting cargo...");

            string path = Application.dataPath;
            path = path + "/" + AssetDatabase.GetAssetPath(this).Substring(7);
            path = System.IO.Path.GetDirectoryName(path);

            if (Models.Length == 0)
            {
                ZipUtility.WriteToZip(Cargo, path);
                Debug.Log("Simple zip created (no model bundle)");
                return;
            }

            Debug.LogWarning("Exporting models does not work yet!");
        }
    }
}
