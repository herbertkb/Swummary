using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABB.SrcML.Data;
using ABB.SrcML;

namespace Swummary
{
    public static class MethodExtractor
    {
        public static IEnumerable<Tuple<string, string, MethodDefinition>> 
                ExtractAllMethodsFromFile(string filePath)
        {
            // open the damn file and convert to srcml
            var srcml = new SrcMLFile( filePath );

            // get a grip on the parts we care about
            var fileUnits = srcml.FileUnits;
            var parser = new CPlusPlusCodeParser();

            // iterate through the mess to build a list of what we want
            var methodList = new List<Tuple<string, string, MethodDefinition>>();

            foreach (var file in fileUnits)
            {
                var fileScope = parser.ParseFileUnit(file);
                var methods = fileScope.GetDescendants<MethodDefinition>();

                string filename = file.Name.ToString();
                foreach (var method in methods)
                {
                    string methodName = method.Name.ToString();
                    var definition = method.GetDescendants<MethodDefinition>().Single();

                    var element = new Tuple<string, string, MethodDefinition>(filename, methodName, definition);
                    methodList.Add( element);
                }                 
            }

            // done!
            return methodList.AsEnumerable();
        }

        public static IEnumerable<Tuple<string, string, MethodDefinition>> 
                ExtractAllMethodsFromDirectory(string directoryPath)
        {
            return new List<Tuple<string, string, MethodDefinition>>().AsEnumerable();
        }
    }
}
