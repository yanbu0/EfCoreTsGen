﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace EfCoreTsGen
{
    class Program
    {
        static void Main(string[] args)
        {
            const string SETTINGS_FILE_NAME = "efcoretsgen.settings.json";
            string settingsPath = args.FirstOrDefault() ?? ".\\";
            string settingsFilePath = $"{settingsPath}\\{SETTINGS_FILE_NAME}";

            Console.WriteLine($"Looking for {SETTINGS_FILE_NAME} at {settingsPath}...");

            if (File.Exists(settingsFilePath)){
                Console.WriteLine("Found settings file...");

                JObject settingsObj = JObject.Parse(File.ReadAllText(settingsFilePath));

                string modelsPath = Path.GetFullPath(settingsPath + settingsObj["modelPath"].Value<string>());
                List<string> excludeList= settingsObj["excludeStrings"].ToObject<List<string>>();

                Console.WriteLine($"Reading EF Models from {modelsPath}, excluding '{String.Join("','",excludeList)}'");

                var modelFileNames = Directory.EnumerateFiles(modelsPath, "*.cs").Where(m => !excludeList.Any(e => m.Contains(e)));
                
                //Parse out the class name and the lines internal to the class
                List<ClassAndLines> classAndLines = new List<ClassAndLines>();
                foreach (string csFilePath in modelFileNames)
                {
                    classAndLines.Add(csFilePath.GetClassAndLines());
                }

                foreach (ClassAndLines cl in classAndLines)
                {
                    //Write ts files into specified dir
                    string outputDir = Path.GetFullPath(settingsObj["outputPath"].Value<string>());
                    string filePath = outputDir + "\\" + cl.ClassName + ".ts";
                    File.WriteAllText(filePath, cl.BuildTsStr(classAndLines.Select(cl => cl.ClassName).ToList()));

                }

                
                Console.WriteLine($"Complete.  {classAndLines.Count()} files created or updated!");

            }
            else {
                Console.WriteLine("No settings file found, exiting!");
            }
        }
    }
}
