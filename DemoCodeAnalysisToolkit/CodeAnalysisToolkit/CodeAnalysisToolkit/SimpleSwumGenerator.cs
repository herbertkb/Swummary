using ABB.SrcML.Data;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.Swum.WordData;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysisToolkit
{
    [TestFixture]
    public class SimpleSwumGenerator
    {
        private static ConservativeIdSplitter splitter;
        private static UnigramTagger tagger;
        private static PCKimmoPartOfSpeechData posData;

        [TestCase]
        public void GenerateSimpleSwum()
        {
            var dataProject = new DataProject<CompleteWorkingSet>("npp_6.2.3",
                Path.GetFullPath("..//..//..//projects//npp_6.2.3"),
                "..//..//..//SrcML");

            dataProject.UpdateAsync().Wait();

            //get srcml stuff in order
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));

            //initialize swum stuff
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();

            //find an example method
            var guiMethod = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == "saveGUIParams").First();
            var guiMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, guiMethod.PrimaryLocation);

            //generate swum for method declaration
            MethodContext mc = ContextBuilder.BuildMethodContext(guiMethodXElement);
            MethodDeclarationNode mdn = new MethodDeclarationNode("saveGUIParams", mc);
            BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
            rule.ConstructSwum(mdn);
            Console.WriteLine(mdn.ToString());
        }
        [TestCase]
        public void GenerateEndingSUnit()
        {
            /*var dataProject = new DataProject<CompleteWorkingSet>("npp_6.2.3",
                Path.GetFullPath("..//..//..//projects//npp_6.2.3"),
                "..//..//..//SrcML");*/

            var dataProject = new DataProject<CompleteWorkingSet>("test",
                Path.GetFullPath("..//..//..//projects//test"),
                "..//..//..//SrcML");

            dataProject.UpdateAsync().Wait();

            //get srcml stuff in order
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));

            //initialize swum stuff
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();

            //find an example method
            var testMethod = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == "IncrementNum").First();
            var testMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, testMethod.PrimaryLocation);

            //generate swum for method declaration
            MethodContext mc = ContextBuilder.BuildMethodContext(testMethodXElement);
            MethodDeclarationNode mdn = new MethodDeclarationNode("IncrementNum", mc);

            //Console.WriteLine(mdn.ToString()); //returns nothing since it hasn't been written

            var exp = testMethod.GetDescendants();
            //var verb = mdn.Action.ToString();

            var expResult = exp.ElementAt(exp.Count() - 1);
            Console.WriteLine(expResult);

            //MethodDeclarationNode mdn2 = new MethodDeclarationNode(expResult.ToString(), mc);

            BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
            Console.WriteLine("InClass = " + rule.InClass(mdn)); //REQUIRED in order for the ConstructSwum method to work
            rule.ConstructSwum(mdn); //rewrites mdn.ToString to a SWUM breakdown
            //rule.ConstructSwum(mdn2);
            //Console.WriteLine(mdn.ToString());

        }
    }
}
