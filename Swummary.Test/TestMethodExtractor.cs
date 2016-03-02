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
            var filepath = Path.Combine(baseDir, "..", ".." , "..", "testdata", "Sample Methods", "sampleMethods.cpp");

            var methodList = MethodExtractor.ExtractAllMethodsFromFile(filepath).ToList();
            
            Assert.IsNotEmpty(methodList);
            Console.WriteLine(methodList.ToString());
            
        }

        [TestCase]
        public void TestMethodExtractorDirectory()
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dirPath = Path.Combine(baseDir, "..", "..", "..", "testdata", "Sample Methods");

            var methodList = MethodExtractor.ExtractAllMethodsFromDirectory( dirPath ).ToList();

            Assert.IsNotEmpty(methodList);
            Console.WriteLine(methodList.ToString());

        }
    }
}
