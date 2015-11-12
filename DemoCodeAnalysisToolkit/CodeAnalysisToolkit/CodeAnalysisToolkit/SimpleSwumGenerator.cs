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
        //change this to change referenced method name across test cases
        string methodName = "moveFiles";
        string folderName = "Sample Methods";
        string fullFilePath = "..//..//..//projects//Sample Methods";

        private static ConservativeIdSplitter splitter;
        private static UnigramTagger tagger;
        private static PCKimmoPartOfSpeechData posData;

        [SetUp]
        public void initalizeSwum()
        {
            //initialize swum stuff
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();
        }


        [TestCase]
        public void GenerateSimpleSwum2()
        {
            var dataProject = new DataProject<CompleteWorkingSet>("npp_6.2.3",
                Path.GetFullPath("..//..//..//projects//npp_6.2.3"),
                "..//..//..//SrcML");

            dataProject.UpdateAsync().Wait();

            //get srcml stuff in order
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));

            //find an example method
            var guiMethod = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == "saveGUIParams").First();
            var guiMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, guiMethod.PrimaryLocation);

            //generate swum for method declaration
            MethodContext mc = ContextBuilder.BuildMethodContext(guiMethodXElement);
            MethodDeclarationNode mdn = new MethodDeclarationNode("saveGUIParams", mc);
            BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
            Console.WriteLine("InClass = " + rule.InClass(mdn));
            rule.ConstructSwum(mdn);
            Console.WriteLine(mdn.ToString());
        }
        
        [TestCase]
        public void GenerateSwumForAnyOccassion()
        {
            var dataProject = new DataProject<CompleteWorkingSet>(folderName,
                    Path.GetFullPath(fullFilePath),
                    "..//..//..//SrcML");

            dataProject.UpdateAsync().Wait();

            //get srcml stuff in order
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));

            //find an example method
            var sampleMethod = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == methodName).First();

            foreach (var line in sampleMethod.GetDescendants())
            {
                var sampleMethod_MethodCalls = line.FindExpressions<MethodCall>();

                foreach (var methodCall in sampleMethod_MethodCalls)
                {
                    var swummedMdn = FromMethodCallToSWUM(methodCall, globalNamespace, dataProject);
                    if (swummedMdn != null)
                    {
                        Console.WriteLine("VOID RETURN");
                        Console.WriteLine(swummedMdn.ToString());
                    }
                }

                //return statement
                if (line is ReturnStatement)
                {
                    var returnMCall = line.FindExpressions<MethodCall>().First();
                    var swummedMdn = FromMethodCallToSWUM(returnMCall, globalNamespace, dataProject);
                    if (swummedMdn != null)
                    {
                        Console.WriteLine("RETURN STMT");
                        Console.WriteLine(swummedMdn.ToString());
                    }
                }
            }
        }


        private MethodDeclarationNode FromMethodCallToSWUM(MethodCall mcall, NamespaceDefinition globalNamespace, DataProject<CompleteWorkingSet> dataProject)
        {
            var mdef = globalNamespace.GetDescendants<MethodDefinition>().Where(decl => mcall.SignatureMatches(decl));
            if (mdef.Count() > 0)
            {
                var mdefXml = DataHelpers.GetElement(dataProject.SourceArchive, mdef.First().PrimaryLocation);
                MethodContext mc = ContextBuilder.BuildMethodContext(mdefXml);
                MethodDeclarationNode mdn = new MethodDeclarationNode(mcall.Name, mc);
                BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
                if (rule.InClass(mdn))
                {
                    rule.ConstructSwum(mdn);
                    return mdn;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Console.WriteLine(mcall.Name + " could not be found");
                return null;
            }
        }


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

            var dataProject = new DataProject<CompleteWorkingSet>(folderName,
                Path.GetFullPath(fullFilePath),
                "..//..//..//SrcML");

            dataProject.UpdateAsync().Wait();

            //get srcml stuff in order
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));

            //initialize swum stuff
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();

            //find an example method, uses global methodName variable
            var testMethod = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == methodName).First();
            var testMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, testMethod.PrimaryLocation);

            //generate swum for method declaration
            MethodContext mc = ContextBuilder.BuildMethodContext(testMethodXElement);
            MethodDeclarationNode mdn = new MethodDeclarationNode(methodName, mc);

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
        [TestCase]
        public void GenerateVoidReturnSUnit(){
            var dataProject = new DataProject<CompleteWorkingSet>(folderName,
                Path.GetFullPath(fullFilePath),
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
            var guiMethod = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == methodName).First();
            var guiMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, guiMethod.PrimaryLocation);

            // forget that, find ALL the methods
            var methods = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == methodName);

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
    //}
    
    /*[TestFixture]
    public class SwumGeneratorJohn
    {
        private static ConservativeIdSplitter splitter;
        private static UnigramTagger tagger;
        private static PCKimmoPartOfSpeechData posData;
        */

        [TestCase]
        public void GenerateSameActionSUnit()
        {
         //   var dataProject = new DataProject<CompleteWorkingSet>("npp_6.2.3",
           //     Path.GetFullPath("..//..//..//projects//npp_6.2.3"),
             //   "..//..//..//SrcML");  
            

            //TODO: <<<<<<<<<<<<< Assert statement to make sure this 

            var dataProject = new DataProject<CompleteWorkingSet>("CodeAnalysisToolkit",
                Path.GetFullPath("..//..//..//"),
                "..//..//..//SrcML");

            int countSuccess = 0;
            List<String> success = new List<String>();

            String methodName = "saveLaptop";   // <<<<<<<<<<<<<<<<<<<<<<<<<===========================

            dataProject.UpdateAsync().Wait();

            //get srcml stuff in order
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));

            //initialize swum stuff
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();

         

            var methodList = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == methodName);
            MethodDefinition topMethod = null;

            // Check if the method was found
            try {       
                topMethod = methodList.First();
            }
            catch (System.InvalidOperationException) {
                Console.WriteLine("--ERROR: Method '" + methodName + "' Not Found--");
                Assert.Fail("Method '" + methodName + "' Not Found");
            }

            var guiMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, topMethod.PrimaryLocation);

            //generate swum for method declaration
            MethodContext mc = ContextBuilder.BuildMethodContext(guiMethodXElement);
            MethodDeclarationNode mdn = new MethodDeclarationNode(methodName, mc);
            BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
            Console.WriteLine("Method = \t" + methodName);
            Console.WriteLine("InClass = \t" + rule.InClass(mdn));
            Console.WriteLine("Rule = \t\t" + rule.GetType().ToString().Substring(9));

            rule.ConstructSwum(mdn);
            //Console.WriteLine(mdn.ToString());

            // Get the action verb from the SWUM
            String methodVerb = mdn.Action.ToPlainString().ToLower();
            Console.WriteLine("Verb = \t\t" + methodVerb);


            // Get all of the lines of code that contains the verb in any form
            var expr = topMethod.GetDescendants().Where(t => t.ToString().Contains(methodVerb)).ToArray();

            // Iterate each line
            foreach (Statement t in expr)
            {
                Console.WriteLine("Line: " + t.ToString());
                // This finds any method calls in this line and finds the verb in that method - from GetCallsTo()
                var methods = t.FindExpressions<MethodCall>(true).Where(c => c.ToString().ToLower().Contains(methodVerb));//c.Name.Contains(methodVerb));


                // Iterate through a list of the method calls in a line and find ones that contain the verb
                foreach (MethodCall i in methods)
                {
                    Console.WriteLine("   MethodCall: " + i.ToString());
                    //Console.WriteLine("Method contains verb: \t" + i.Name);


                    // Build SWUM for found method to get the verb out.  This uses the mc from the original swum method build 
                    //   because this MethodDeclaration node takes in a string that is then used to parse the swum rather than the mc name
                    MethodDeclarationNode mdnNEW = new MethodDeclarationNode(i.Name, mc);


                    if (rule.InClass(mdnNEW))
                    {
                        Console.WriteLine("\tMethod: " +  i.Name + "\t\tInClass = true" );

                        rule.ConstructSwum(mdnNEW);
                        //Console.WriteLine(mdnNEW.ToString());


                        // if the verbs are the same in each method
                        if (mdnNEW.Action.ToPlainString().Equals(methodVerb, StringComparison.InvariantCultureIgnoreCase))
                        {
                            countSuccess++;
                            success.Add(i.Name);
                        }
                        else
                        {
                            Console.WriteLine("\tMethod does not contain the verb" + "\n");
                        }
                    }
                    else {
                        Console.WriteLine("   MethodCall: i.Name  not InClass");
                    }


                } // End Method Iteration
            } // End Line Iteration

            if (countSuccess == 0) {
                Console.WriteLine("===== No Same-Action Methods Found =====");
                Assert.Fail("No Same-Action Methods found");
            }
            else {
                Console.WriteLine("\n============= SUCCESSES ===============");
                foreach (String i in success) {
                    Console.WriteLine(i);
                }

            }


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
