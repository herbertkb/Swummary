using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ABB.SrcML.Data;
using ABB.SrcML;

namespace Swummary
{
    class Swummary
    {
        public static IEnumerable<Tuple<string, string>> Swummarize(string directoryPath)
        {
            var swummaries = new List<Tuple<string, string>>();

            var srcmlMethods = MethodExtractor.ExtractAllMethodsFromDirectory(directoryPath);

            foreach (var methodDef in srcmlMethods)
            {
                // Extract SUnit Statements from MethodDefinition
                var statements = SUnitExtractor.ExtractAll(methodDef).ToList();

                // Translate Statements into SUnits
                List<SUnit> sunits = statements.ConvertAll(
                            new Converter<Statement, SUnit>(SUnitTranslator.Translate));

                // Generate text from SUnits
                List<string> sentences = sunits.ConvertAll(
                            new Converter<SUnit, string>(TextGenerator.GenerateText));

                // Collect text and summarize
                var methodDocument = String.Join<string>("\n", sentences);

                // TEMP: Just use full set of sentences for now.
                //       Maybe set a flag for this.
                //var summary = Summarizer.Summarize(methodDocument);
                var summary = methodDocument;
        
                // Add swummary to collection with its full method name
                var methodName = methodDef.GetFullName();
                swummaries.Add(new Tuple<string, string>(methodName, summary));
            }

            return swummaries.AsEnumerable();
        }
    }
}
