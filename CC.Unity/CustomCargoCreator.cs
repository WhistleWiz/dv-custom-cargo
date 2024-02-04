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
        public Sprite? Icon = null;
        public Sprite? ResourceIcon = null;
        public List<ModelsForVanillaCar> Models = new List<ModelsForVanillaCar>();

        private bool _requireConfirm = false;

        internal bool DisplayWarning => _requireConfirm;
        internal bool ShouldMakeBundle => Models.Count > 0 ||
            Icon != null ||
            ResourceIcon != null;

        private void OnValidate()
        {
            _requireConfirm = false;

            // Make sure this cargo is always in any group.
            foreach (var group in Cargo.CargoGroups)
            {
                group.AddIdIfMissing(Cargo.Identifier);
            }
        }

        public string GetFullPath()
        {
            string path = Application.dataPath;
            string assetPath = AssetDatabase.GetAssetPath(this);
            path = path + "/" + assetPath.Substring(7);
            return Path.GetDirectoryName(path);
        }

        public void CreateModelSet(CarParentType parentType)
        {
            var set = CreateInstance<ModelsForVanillaCar>();
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

            Models.Add(set);
            set.CarType = parentType;

            EditorUtility.SetDirty(this);
            AssetDatabase.CreateAsset(set, $"{path}/{Cargo.Identifier.Replace(" ", "_")}_{parentType}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = set;
        }

        public string? ExportBundle()
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
                return null!;
            }

            Debug.Log("Exporting cargo...");

            string path = GetFullPath();

            List<string> extraFiles = new List<string>();
            string bundlePath = string.Empty;

            // Only make a bundle if there's need for it.
            if (ShouldMakeBundle)
            {
                Debug.Log("Building asset bundle...");
                bundlePath = CreateBundle(path);
                extraFiles.Add(bundlePath);
            }

            // Add icon file if it exists.
            string extraPath = Path.Combine(path, Constants.IconFile);

            if (Icon == null && File.Exists(extraPath))
            {
                Debug.Log("Adding icon...");
                extraFiles.Add(extraPath);
            }

            // Same for the resource icon.
            extraPath = Path.Combine(path, Constants.ResourceIconFile);

            if (ResourceIcon == null && File.Exists(extraPath))
            {
                Debug.Log("Adding resource icon...");
                extraFiles.Add(extraPath);
            }

            Debug.Log("Zipping cargo mod...");
            var output = ZipUtility.WriteToZip(Cargo, path, extraFiles.ToArray());

            if (!string.IsNullOrEmpty(bundlePath))
            {
                Debug.Log("Cleaning up...");
                DeleteBundle(path, bundlePath);
            }

            Debug.Log("Zip created!");
            AssetDatabase.Refresh();

            return output;
        }

        public string CreateBundle(string path)
        {
            List<(Object Asset, string? Name)> objs = new List<(Object, string?)>();

            // Add the models, no need to assign names to them.
            foreach (var model in Models)
            {
                objs.Add((model, null));
            }

            // Add the icons
            if (Icon != null)
            {
                objs.Add((Icon, Constants.Icon));
            }

            if (ResourceIcon != null)
            {
                objs.Add((ResourceIcon, Constants.ResourceIcon));
            }

            BuildPipeline.BuildAssetBundles(Path.GetDirectoryName(AssetDatabase.GetAssetPath(this)),
                GetAssetBuilds(objs),
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);

            return Directory.EnumerateFiles(path, Constants.ModelBundle, SearchOption.TopDirectoryOnly).First();
        }

        public void DeleteBundle(string path, string bundlePath)
        {
            File.Delete(bundlePath);
            File.Delete(bundlePath + ".manifest");

            // Delete the 2nd bundle too.
            bundlePath = Path.GetFileName(path);
            bundlePath = Path.Combine(path, bundlePath);

            File.Delete(bundlePath);
            File.Delete(bundlePath + ".manifest");
        }

        private static AssetBundleBuild[] GetAssetBuilds(IEnumerable<(Object Asset, string? Name)> assets)
        {
            List<string> paths = new List<string>();

            foreach (var (asset, _) in assets)
            {
                paths.Add(AssetDatabase.GetAssetPath(asset));
            }

            var build = new AssetBundleBuild
            {
                assetBundleName = Constants.ModelBundle,
                assetNames = paths.ToArray(),
                addressableNames = assets.Select(x => x.Name).ToArray(),
            };

            return new[] { build };
        }
    }
}
