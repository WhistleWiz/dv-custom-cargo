using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;

namespace CC.Common
{
    public class ZipUtility
    {
        public static void WriteToZip(CustomCargo c, string path, params string[] extraFilePaths)
        {
            // Use a memory stream so we can write multiple files into
            // a single zip without writing to disk before.
            using (var memoryStream = new MemoryStream())
            {
                // Used most often as the folder name, but also the file.
                var fileName = GetFullModId(c);

                // Create the archive.
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Use a JsonSerializer for a cleaner output.
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;

                    // Create the mod info file, and write the usual stuff in it.
                    var file = archive.CreateEntry(Path.Combine(fileName, NameConstants.ModInfo));

                    using (var entryStream = file.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    using (var jsonWr = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonWr, GetModInfo(c));
                    }

                    // Create the cargo file, and serialize the CustomCargo into it.
                    file = archive.CreateEntry(Path.Combine(fileName, NameConstants.CargoFile));

                    using (var entryStream = file.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    using (var jsonWr = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonWr, c);
                    }

                    // Include extra files into the zip.
                    foreach (var item in extraFilePaths)
                    {
                        file = archive.CreateEntryFromFile(item, Path.Combine(fileName, Path.GetFileName(item)));
                    }
                }

                // Finally get the final output path and write the memory stream into it.
                var outputPath = Path.Combine(path,
                    $"{fileName}.zip");

                using (var fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        public static string GetFullModId(CustomCargo c)
        {
            return $"{NameConstants.ModIdPrefix}{c.Identifier.Replace(" ", "")}";
        }

        public static JObject GetModInfo(CustomCargo c)
        {
            var modInfo = new JObject
            {
                { "Id", GetFullModId(c) },
                { "DisplayName", $"Custom Cargo {c.Identifier}" },
                { "Version", c.Version },
                { "Author", c.Author },
                { "ManagerVersion", "0.27.3" },
                { "Requirements", JToken.FromObject(new[] { NameConstants.MainModId }) },
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
