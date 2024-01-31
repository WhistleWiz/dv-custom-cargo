using CC.Common;
using System;
using System.IO;

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
            c.Identifier = Console.ReadLine();

            Console.WriteLine("Cargo mass per unit:");
            c.MassPerUnit = ForceReadFloat(f => f > 0, "Must be a number greater than 0!");

            Console.WriteLine("Price per unit:");
            c.FullDamagePrice = ForceReadFloat(f => f > 0, "Must be a number greater than 0!");

            Console.WriteLine("Environmental damage cost:");
            c.EnvironmentDamagePrice = ForceReadFloat();

            Console.WriteLine("And finally, the author's name (yours):");
            c.Author = Console.ReadLine();

            Console.WriteLine("\n\n\nPreparing zip...");
            ZipUtility.WriteToZip(c, s_exportPath);
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
    }
}
