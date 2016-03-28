using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ABB.SrcML;
using ABB.SrcML.Data;
using System.IO;
using Swummary;
using System.Reflection;

namespace Swummary.Test
{
    [TestFixture]
    class TestMethodExtractor
    {
        [TestCase]
        public void TestMethodExtractorSingleFile()
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var filepath = Path.Combine(baseDir, @"..\..\..\", "testdata", "Sample Methods", "sampleMethods.cpp");

            Console.WriteLine(filepath);

            var methodList = MethodExtractor.ExtractAllMethodsFromFile(filepath).ToList();
            
            Assert.IsNotEmpty(methodList);
            Console.WriteLine(methodList.ToString());
            
        }

        [TestCase("Sample Methods")]
        [TestCase("OpenRA")]
        public void TestMethodExtractorDirectory(string directoryName)
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var targetDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\testdata\", directoryName));

            var methodList = MethodExtractor.ExtractAllMethodsFromDirectory(targetDir).ToList();

            Assert.That(methodList is List<MethodDefinition>);
            Assert.IsNotEmpty(methodList);

            // Generate report on methods extracted
            foreach(var m in methodList)
            {
                var methodName = m.GetFullName();
                var methodDef = m.ToString();
                var parentClass = m.GetAncestors<TypeDefinition>();
                
                Console.WriteLine(String.Format("{0}\t{1}\t{2}", parentClass, methodName, methodDef));
            }

        }
    }
}
