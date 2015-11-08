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
        
        public void GenerateVoidReturnSUnit(){
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

            // forget that, find ALL the methods
            var methods = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == "saveGUIParams");

            foreach (MethodDefinition method in methods)
            {
                //Console.WriteLine(method.ToString());
                var statements = method.ChildStatements;
                foreach (Statement statement in statements)
                {
                    var expressions = statement.GetExpressions();
                    foreach (Expression expression in expressions)
                    {   
                        // Skip any expression that contains an assignment
                       if (expression.ToString().Contains(" =") || expression.ToString().Contains(" ->")) { continue; }

                        // Print whatever's left. It should be a void return.
                        Console.WriteLine(expression.ToString());

                        // *** PoS tag it ***
                        // convert the string to 'PhraseNode' objects so we can feed them to SWUM
                        var pn = PhraseNode.Parse(new WordNode( expression.ToString() ).ToString() ); 

                        Console.WriteLine(pn.ToString());

                        // construct the "rule" to break up method names into sentences
                        BaseVerbRule thisrule = new BaseVerbRule(posData, tagger, splitter);

                        var methodNode = new MethodDeclarationNode(expression.ToString());

                        thisrule.ConstructSwum(methodNode);

                        Console.WriteLine(methodNode.ToString());
                    }
                }
            }
        }
    }
    
    [TestFixture]
    public class SwumGeneratorJohn
    {
        private static ConservativeIdSplitter splitter;
        private static UnigramTagger tagger;
        private static PCKimmoPartOfSpeechData posData;

        [TestCase]
        public void GenerateSimpleSwumJohn()
        {
         //   var dataProject = new DataProject<CompleteWorkingSet>("npp_6.2.3",
           //     Path.GetFullPath("..//..//..//projects//npp_6.2.3"),
             //   "..//..//..//SrcML");  
            
            var dataProject = new DataProject<CompleteWorkingSet>("CodeAnalysisToolkit",
                Path.GetFullPath("..//..//..//"),
                "..//..//..//SrcML");


            


            dataProject.UpdateAsync().Wait();

            //get srcml stuff in order
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));



            //initialize swum stuff
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();       // a unigram is a single word
            posData = new PCKimmoPartOfSpeechData();



            //find an example method
            var guiMethod = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == "breakEverything").First();
            var guiMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, guiMethod.PrimaryLocation);

            //generate swum for method declaration
            MethodContext mc = ContextBuilder.BuildMethodContext(guiMethodXElement);
            MethodDeclarationNode mdn = new MethodDeclarationNode("breakEverything", mc);
            BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
            Console.WriteLine("InClass = " + rule.InClass(mdn));
            

            rule.ConstructSwum(mdn);
            Console.WriteLine(mdn.ToString());

            //========START CUSTOM
            Console.WriteLine("=====================");
            Console.WriteLine("=====================");

            // Get the action verb from the SWUM
            String methodVerb = mdn.Action.ToPlainString().ToLower();
            Console.WriteLine("\tVerb from Method is: " + methodVerb);


            // Get all of the lines of code that contains the verb in any form
            var expr = guiMethod.GetDescendants().Where(t => t.ToString().Contains(methodVerb)).ToArray();     //***THIS IS BREAKING EVERYTHING

            // Iterate each line
            foreach (Statement t in expr) {

                // This finds any method calls in this line and finds the verb in that method - from GetCallsTo()
                var methods = t.FindExpressions<MethodCall>(true).Where(c => c.ToString().ToLower().Contains(methodVerb));//c.Name.Contains(methodVerb));
                

                // Iterate through a list of the method calls in a line and find ones that contain the verb
                foreach (MethodCall i in methods)
                {
                    Console.WriteLine("\tMethod contains verb: \t\t" + i.Name);
                    
                    
                    // Build SWUM for found method to get the verb out.  This uses the mc from the original swum method build 
                    //   because this MethodDeclaration node takes in a string that is then used to parse the swum rather than the mc name
                    MethodDeclarationNode mdnNEW = new MethodDeclarationNode(i.Name, mc);


                    if (rule.InClass(mdnNEW))
                    {
                        Console.WriteLine("\t** InClass = " + rule.InClass(mdnNEW) + "  for  " + i.Name);

                        rule.ConstructSwum(mdnNEW);
                        //Console.WriteLine(mdnNEW.ToString());


                        // if the verbs are the same in each method
                        if (mdnNEW.Action.ToPlainString().Equals(methodVerb, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine("Found Verb in " + i.Name + "\n");

                        }
                        else
                        {
                            Console.WriteLine("\tMethod does not contain the verb" + "\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: NOT IN CLASS" + "  for  " + i.Name);  

                    }

                    
                    
                    
                } // End Method Iteration
                
                
            } //End Line Iteration


            /*
             * 
             * TODO: 
             * 
             *  This might break when there are two verbs in the method name
             * 
             * 
             * 
             * 
            */


        }








        /*
        //Important statement: This searches through the whole code and finds a method with a matching name and signature
        var matches = i.FindMatches().ToList();

        Console.WriteLine("FindMatches ==== ");
        foreach (MethodDefinition k in matches)
        {
            Console.WriteLine("ToString: " + k.ToString() + "Type: " +  k.GetType());

            MethodDeclarationNode mdnNEW = new MethodDeclarationNode(i.Name, mc);



            // Build SWUM for found method to get the verb out
            //var NEWMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, k.PrimaryLocation);
            //MethodContext mcNEW = ContextBuilder.BuildMethodContext(NEWMethodXElement);
            //MethodDeclarationNode mdnNEW = new MethodDeclarationNode(i.Name, mcNEW);
            //Console.WriteLine("InClass = " + rule.InClass(mdnNEW));
            rule.ConstructSwum(mdnNEW);
            //Console.WriteLine(mdnNEW.ToString());

            // if the verbs are the same in each method
            if (mdnNEW.Action.ToPlainString().Equals(methodVerb, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("FoundVerb = " + mdnNEW.Action.ToPlainString() + "\n" +
                                    "MethodVerb = " + methodVerb);


            }
            else
            {
                Console.WriteLine("\tMethod does not contain the verb");
            }

        }
         */











        /*bool DoesVerbEqualMethod(String verb, MethodCall call, BaseVerbRule rule, DataProject<CompleteWorkingSet> dataProject)
        {


            var NEWMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, k.PrimaryLocation);
            MethodContext mcNEW = ContextBuilder.BuildMethodContext(NEWMethodXElement);
            MethodDeclarationNode mdnNEW = new MethodDeclarationNode(i.Name, mcNEW);


            Console.WriteLine("InClass = " + rule.InClass(mdnNEW));
            rule.ConstructSwum(mdnNEW);
            //Console.WriteLine(mdnNEW.ToString());

            // if the verbs are the same in each method
            if (mdnNEW.Action.ToPlainString().Equals(verb, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("FoundVerb = " + mdnNEW.Action.ToPlainString() + "\n" +
                                    "MethodVerb = " + verb);


            }
            else
            {
                Console.WriteLine("\tMethod does not contain the verb");
            }
            return true;
        }
        */
    }
    
}
