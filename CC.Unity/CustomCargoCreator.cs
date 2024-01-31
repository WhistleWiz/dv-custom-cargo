using CC.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CC.Unity
{
    [CreateAssetMenu(menuName = "DVCustomCargo/Custom Cargo")]
    internal class CustomCargoCreator : ScriptableObject
    {
        public CustomCargo Cargo = new CustomCargo();
        public List<ModelsForVanillaCar> Models = new List<ModelsForVanillaCar>();

        private bool _requireConfirm = false;

        internal bool DisplayWarning => _requireConfirm;

        private void OnValidate()
        {
            _requireConfirm = false;
        }

        public void CreateModelSet(CarParentType parentType)
        {
            var set = CreateInstance<ModelsForVanillaCar>();
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

            Models.Add(set);
            set.CarType = parentType;

            EditorUtility.SetDirty(this);
            AssetDatabase.CreateAsset(set, $"{path}/{Cargo.Name.Replace(" ", "_")}_{parentType}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = set;
        }

        public void ExportModels()
        {
            if (_requireConfirm)
            {
                _requireConfirm = false;
            }
            else
            {
                _requireConfirm = !Validation.CargoValidator.ValidateCargo(this);
            }

            if (_requireConfirm)
            {
                Debug.LogWarning("Cargo failed validation! You can click export again to force it to export, " +
                    "but it's recommended to fix errors first!");
                return;
            }

            Debug.Log("Exporting cargo...");

            string path = Application.dataPath;
            string assetPath = AssetDatabase.GetAssetPath(this);
            path = path + "/" + assetPath.Substring(7);
            path = Path.GetDirectoryName(path);

            if (Models.Count == 0)
            {
                Debug.Log("Zipping cargo mod...");
                ZipUtility.WriteToZip(Cargo, path);
                Debug.Log("Simple zip created (no model bundle)!");
                return;
            }

            Debug.Log("Building asset bundle...");

            BuildPipeline.BuildAssetBundles(Path.GetDirectoryName(assetPath),
                GetAssetBuilds(Models.ToArray()),
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);

            string bundlePath = Directory.EnumerateFiles(path, NameConstants.ModelBundle, SearchOption.TopDirectoryOnly).First();

            Debug.Log("Zipping cargo mod...");
            ZipUtility.WriteToZip(Cargo, path, bundlePath);

            Debug.Log("Cleaning up...");
            File.Delete(bundlePath);
            File.Delete(bundlePath + ".manifest");

            Debug.Log("Zip created!");
        }

        private static AssetBundleBuild[] GetAssetBuilds(params Object[] assets)
        {
            List<string> names = new List<string>();

            foreach (Object asset in assets)
            {
                names.Add(AssetDatabase.GetAssetPath(asset));
            }

            var build = new AssetBundleBuild
            {
                assetBundleName = NameConstants.ModelBundle,
                assetNames = names.ToArray()
            };

            return new[] { build };
        }
    }
}
