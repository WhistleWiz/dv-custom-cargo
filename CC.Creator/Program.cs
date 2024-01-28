using CC.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;

namespace CC.Creator
{
    internal class Program
    {
        private const string Splash = @"

         ██████╗ █████╗ ██████╗  ██████╗  ██████╗    
        ██╔════╝██╔══██╗██╔══██╗██╔════╝ ██╔═══██╗   
        ██║     ███████║██████╔╝██║  ███╗██║   ██║   
        ██║     ██╔══██║██╔══██╗██║   ██║██║   ██║   
        ╚██████╗██║  ██║██║  ██║╚██████╔╝╚██████╔╝   
         ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝    
                                             
        ██╗    ██╗██╗███████╗ █████╗ ██████╗ ██████╗ 
        ██║    ██║██║╚══███╔╝██╔══██╗██╔══██╗██╔══██╗
        ██║ █╗ ██║██║  ███╔╝ ███████║██████╔╝██║  ██║
        ██║███╗██║██║ ███╔╝  ██╔══██║██╔══██╗██║  ██║
        ╚███╔███╔╝██║███████╗██║  ██║██║  ██║██████╔╝
         ╚══╝╚══╝ ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ 
                                             

";

        private static string s_exportPath = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            Console.WriteLine(Splash);
            CustomCargo c = new CustomCargo();

            Console.WriteLine("Welcome to the custom cargo wizard!\n" +
                "Just fill in the fields for your custom cargo!\n");

            Console.WriteLine("Custom cargo ID (name):");
            c.Name = Console.ReadLine();

            Console.WriteLine("Cargo mass per unit:");
            c.MassPerUnit = ForceReadFloat(f => f > 0, "Must be a number greater than 0!");

            Console.WriteLine("Price per unit:");
            c.FullDamagePrice = ForceReadFloat(f => f > 0, "Must be a number greater than 0!");

            Console.WriteLine("Environmental damage cost:");
            c.EnvironmentDamagePrice = ForceReadFloat();

            Console.WriteLine("And finally, the author's name (yours):");
            c.Author = Console.ReadLine();

            Console.WriteLine("\n\n\nPreparing zip...");
            WriteToZip(c);
            Console.WriteLine($"Success! Your mod has been created at {s_exportPath}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static float ForceReadFloat()
        {
            return ForceReadFloat(_ => true, "");
        }

        private static float ForceReadFloat(Predicate<float> match, string reason)
        {
            float f;

            while (!float.TryParse(Console.ReadLine(), out f) || !match(f))
            {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {reason}");
                Console.ForegroundColor = c;
            }

            return f;
        }

        private static void WriteToZip(CustomCargo c)
        {
            using (var memoryStream = new MemoryStream())
            {
                var fileName = GetFullModId(c);

                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;

                    var file = archive.CreateEntry(Path.Combine(fileName, NameConstants.ModInfo));

                    using (var entryStream = file.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    using (var jsonWr = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonWr, GetModInfo(c));
                    }

                    file = archive.CreateEntry(Path.Combine(fileName, NameConstants.CargoFile));

                    using (var entryStream = file.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    using (var jsonWr = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonWr, c);
                    }
                }

                var outputPath = Path.Combine(s_exportPath,
                    $"{fileName}.zip");

                using (var fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        private static string GetFullModId(CustomCargo c)
        {
            return $"{NameConstants.ModIdPrefix}{c.Name.Replace(" ", "")}";
        }

        private static JObject GetModInfo(CustomCargo c)
        {
            return new JObject
            {
                { "Id", GetFullModId(c) },
                { "DisplayName", $"Custom Cargo {c.Name}" },
                { "Version", "1.0.0" },
                { "Author", c.Author },
                { "ManagerVersion", "0.27.3" },
                { "Requirements", JToken.FromObject(new[] { NameConstants.MainModId }) },
            };
        }
    }
}
