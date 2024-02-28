using CC.Common;
using System.Collections.Generic;
using System.IO;
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
                bundlePath = AssetBundleHelper.CreateBundle(path, AssetBundleHelper.GetAssetPath(this), GetBundleAssets());
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
                AssetBundleHelper.DeleteBundle(path, bundlePath);
            }

            Debug.Log("Zip created!");
            AssetDatabase.Refresh();

            return output;
        }

        public List<(Object Asset, string? Name)> GetBundleAssets()
        {
            List<(Object Asset, string? Name)> objs = new List<(Object, string?)>();

            // Add the models, no need to assign names to them.
            foreach (var model in Models)
            {
                objs.Add((model, model.GetAssetAdressable(Cargo.Identifier)));
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

            return objs;
        }

        public CommonCargoObject ToCommon()
        {
            var common = CreateInstance<CommonCargoObject>();

            common.Identifier = Cargo.Identifier;
            common.Icon = Icon;
            common.ResourceIcon = ResourceIcon;
            common.Models = Models;

            return common;
        }
    }
}
