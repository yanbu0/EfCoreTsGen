using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

namespace EfCoreTsGen
{
    public static class Extensions
    {

        public static ClassAndLines GetClassAndLines(this string csFilePath)
        {
            List<string> allLines = File.ReadAllLines(csFilePath).ToList<string>();

            int classNameIndex = allLines.FindIndex(l => l.Contains(" class "));

            string className = allLines.Where(l => l.Contains(" class ")).Select(l => l.Substring(l.LastIndexOf(' ') + 1)).Single();

            //Get lines inside class
            List<string> filteredLines = allLines.GetRange(classNameIndex + 2, allLines.Count() - (classNameIndex + 3));
            filteredLines = filteredLines.Where(l => l.Length > 0 && !l.Contains("//")).ToList(); //Remove blank lines and comments


            return new ClassAndLines() { ClassName = className, Lines = filteredLines };
        }

        public static string BuildTsStr(this ClassAndLines classAndLines, List<string> allClassNames)
        {
            string tsClassStr = string.Empty;

            tsClassStr += "export class " + classAndLines.ClassName + " {\r\n";

            List<string> modifiedLines = new List<string>();
            //Remove get and set
            //Remove public
            //Remove virtual
            foreach (string l in classAndLines.Lines)
            {
                string lineToAdd = l;
                List<string> removeMeList = new List<string>() { " { get; set; }", "virtual", "public", "{ get; }", " = new List<*" };
                foreach (string removeMe in removeMeList)
                {
                    lineToAdd = lineToAdd.RemoveThis(removeMe);
                }
                modifiedLines.Add(lineToAdd.Trim());
            }

            //Remove constructor if exists
            int endOfConstructorIndex = modifiedLines.FindLastIndex(l => l.Contains("}"));
            if (endOfConstructorIndex != -1)
            {
                modifiedLines = modifiedLines.GetRange(endOfConstructorIndex + 1, modifiedLines.Count() - (endOfConstructorIndex + 1));
            }

            List<string> importList = new List<string>();
            foreach (string line in modifiedLines)
            {
                string typeStr = line.Substring(0, line.IndexOf(" "));

                string propName = line.Substring(line.LastIndexOf(' ') + 1);

                //Convert to camelcase
                var cc = new CamelCasePropertyNamesContractResolver();
                propName = cc.GetResolvedPropertyName(propName);

                tsClassStr += $"   public {propName}: {typeStr.TypeConversion(allClassNames, importList)};\r\n";
            }

            tsClassStr += "}";

            //add imports
            string importStr = "\r\n";
            //Converting to hashset removes duplicates
            foreach (string import in importList.ToHashSet<string>())
            {
                //don't import self
                if (!(classAndLines.ClassName==import))
                {
                    importStr += "import { " + import + " } from './index';\r\n";
                }
            }

            tsClassStr = importStr + "\r\n" + tsClassStr;

            return tsClassStr;
        }

        private static string TypeConversion(this string csTypeStr, List<string> allClassNames, List<string> importList)
        {
            //Default to number since lots of C# stuff rolls up to this in ts
            string tsTypeStr = string.Empty;

            switch (csTypeStr)
            {
                case "bool":
                case "boolean":
                    tsTypeStr = "boolean = false";
                    break;
                case "boolean?":
                case "bool?":
                    tsTypeStr = "boolean = null";
                    break;
                case "string":
                    tsTypeStr = "string = null";
                    break;
                case "DateOnly":
                case "DateTime":
                case "DateTimeOffset":
                    tsTypeStr = "Date = new Date(0)";
                    break;
                case "DateOnly?":
                case "DateTime?":
                case "DateTimeOffset?":
                    tsTypeStr = "Date = null";
                    break;
                case "int?":
                case "decimal?":
                    tsTypeStr = "number = null";
                    break;
                case "byte[]":
                    tsTypeStr = "Array<any> = []";
                    break;
                case "Guid":
                    tsTypeStr = "string = '00000000-0000-0000-0000-000000000000'";
                    break;
                case "Guid?":
                    tsTypeStr = "string = null";
                    break;
                default:
                    tsTypeStr = "number = 0";
                    if (csTypeStr.Contains("ICollection"))
                    {
                        int startIndex = csTypeStr.IndexOf('<')+1;
                        int lengthToGet = csTypeStr.IndexOf('>') - startIndex;
                        tsTypeStr = csTypeStr.Substring(startIndex,lengthToGet);
                        importList.Add(tsTypeStr);
                        tsTypeStr = tsTypeStr + "[] = []";
                    }
                    else if (allClassNames.Contains(csTypeStr))
                    {
                        importList.Add(csTypeStr);
                        tsTypeStr = csTypeStr + " = null";
                    }
                    break;
            }

            return tsTypeStr;
        }

        private static string RemoveThis(this string fullStr, string removeMe)
        {
            //if last char is "*" remove rest of line
            string test = removeMe.EndsWith('*') ? removeMe.Remove(removeMe.Length-1) : removeMe;
            int index = fullStr.IndexOf(test);
            string retStr = fullStr;
            if (index > 0)
            {
                if (removeMe.EndsWith('*'))
                {
                    retStr = fullStr.Remove(index);
                }
                else
                {
                    retStr = fullStr.Remove(index, removeMe.Length);
                }
            }
            return retStr;
        }

        public static string GetStringBetweenCharacters(this string input, char charFrom, char charTo)
        {
            int posFrom = input.IndexOf(charFrom);
            if (posFrom != -1) //if found char
            {
                int posTo = input.IndexOf(charTo, posFrom + 1);
                if (posTo != -1) //if found char
                {
                    return input.Substring(posFrom + 1, posTo - posFrom - 1);
                }
            }

            return string.Empty;
        }
    }

}


public class ClassAndLines
{
    public string ClassName { get; set; }

    public List<string> Lines { get; set; }
}
