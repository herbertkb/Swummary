using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ABB.SrcML;
using ABB.SrcML.Data;
using System.IO;
using Swummary;

namespace Swummary.Test
{
    [TestFixture]
    class TestMethodExtractor
    {
        [TestCase]
        public void TestMethodExtractorSingleFile()
        {
            var methodList = MethodExtractor.ExtractAllMethodsFromFile("../testdata/Sample Methods/sampleMethods.cpp").ToList();

            Assert.IsNotEmpty(methodList);
            Console.WriteLine(methodList.ToString());
            
        }
        [TestCase]
        public void TestMethodExtractorDirectory()
        {
            var methodList = MethodExtractor.ExtractAllMethodsFromFile("../testdata/OpenRA").ToList();

            Assert.IsNotEmpty(methodList);
            Console.WriteLine(methodList.ToString());

        }
    }
}
