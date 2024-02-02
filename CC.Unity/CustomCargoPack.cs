using CC.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

namespace CC.Unity
{
    [CreateAssetMenu(menuName = "DVCustomCargo/Custom Cargo Pack")]
    internal class CustomCargoPack : ScriptableObject
    {
        public string PackName = "MyCargoPack";
        [Tooltip("Your name, probably")]
        public string Author = string.Empty;
        public string Version = "1.0.0";
        [Tooltip("Add a link to your website here (Optional)")]
        public string? HomePage;
        [Tooltip("Add a repository link here to be able to automatically update your cargo (Optional)")]
        public string? Repository;
        [Tooltip("The cargos you want to include in this pack")]
        public List<CustomCargoCreator> Cargos = new List<CustomCargoCreator>();
        
        private bool _requireConfirm = false;

        internal bool DisplayWarning => _requireConfirm;

        public string PackId =>  $"{Constants.ModIdPrefix}{PackName.Replace(" ", "")}";

        public string GetFullPath()
        {
            string path = Application.dataPath;
            string assetPath = AssetDatabase.GetAssetPath(this);
            path = path + "/" + assetPath.Substring(7);
            return Path.GetDirectoryName(path);
        }

        public string? Export()
        {
            if (_requireConfirm)
            {
                _requireConfirm = false;
            }
            else
            {
                Debug.Log("Validating all cargo...");

                foreach (var cargo in Cargos)
                {
                    if (!Validation.CargoValidator.ValidateCargo(cargo))
                    {
                        _requireConfirm = true;
                        break;
                    }
                }
            }

            if (_requireConfirm)
            {
                Debug.LogWarning("Cargo failed validation! You can click export again to force it to export, " +
                    "but it's recommended to fix errors first!");
                return null!;
            }


            Debug.Log("Exporting cargo...");

            // Almost the same as ZipUtility.
            using var memoryStream = new MemoryStream();

            var fileName = PackId;

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;

                var file = archive.CreateEntry(Path.Combine(fileName, Constants.ModInfo));

                using (var entryStream = file.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                using (var jsonWr = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonWr, GetModInfo(this));
                }

                foreach (var c in Cargos)
                {
                    // Create the cargo file, and serialize the CustomCargo into it.
                    file = archive.CreateEntry(Path.Combine(fileName, c.Cargo.Identifier, Constants.CargoFile));

                    using (var entryStream = file.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    using (var jsonWr = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonWr, c.Cargo);
                    }

                    List<string> extraFiles = new List<string>();
                    string cargoPath = c.GetFullPath();
                    string bundlePath = string.Empty;

                    // Only make a bundle if there's need for it.
                    if (c.ShouldMakeBundle)
                    {
                        Debug.Log($"Building asset bundle for {c.name}...");
                        bundlePath = c.CreateBundle(cargoPath);
                        extraFiles.Add(bundlePath);
                    }

                    // Add icon file if it exists.
                    string extraPath = Path.Combine(cargoPath, Constants.IconFile);

                    if (c.Icon == null && File.Exists(extraPath))
                    {
                        Debug.Log($"Adding icon for {c.name}...");
                        extraFiles.Add(extraPath);
                    }

                    // Same for the resource icon.
                    extraPath = Path.Combine(cargoPath, Constants.ResourceIconFile);

                    if (c.ResourceIcon == null && File.Exists(extraPath))
                    {
                        Debug.Log($"Adding resource icon for {c.name}...");
                        extraFiles.Add(extraPath);
                    }

                    // Include extra files into the zip.
                    foreach (var item in extraFiles)
                    {
                        file = archive.CreateEntryFromFile(item, Path.Combine(fileName, c.Cargo.Identifier, Path.GetFileName(item)));
                    }

                    if (!string.IsNullOrEmpty(bundlePath))
                    {
                        Debug.Log($"Cleaning up {c.name}...");
                        c.DeleteBundle(cargoPath, bundlePath);
                    }
                }
            }

            // Finally get the final output path and write the memory stream into it.
            var outputPath = Path.Combine(GetFullPath(),
                $"{fileName}.zip");

            Debug.Log("Zipping cargo mod...");

            using (var fileStream = new FileStream(outputPath, FileMode.Create))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(fileStream);
            }

            Debug.Log("Zip created!");
            AssetDatabase.Refresh();

            // The final file.
            return outputPath;
        }

        private static JObject GetModInfo(CustomCargoPack c)
        {
            var modInfo = new JObject
            {
                { "Id", c.PackId },
                { "DisplayName", $"Custom Cargo {c.PackName}" },
                { "Version", c.Version },
                { "Author", c.Author },
                { "ManagerVersion", "0.27.3" },
                { "Requirements", JToken.FromObject(new[] { Constants.MainModId }) },
            };

            // If a homepage was defined, also add the link.
            if (!string.IsNullOrEmpty(c.HomePage))
            {
                modInfo.Add("HomePage", c.HomePage);
            }

            // If a repository was defined, also add the link.
            if (!string.IsNullOrEmpty(c.Repository))
            {
                modInfo.Add("Repository", c.Repository);
            }

            return modInfo;
        }
    }
}
