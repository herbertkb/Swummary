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
        string methodName = "GetInt";
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

            foreach (var line in sampleMethod.GetDescendants()) //goes through lines (Statement class) in method 
            {
                var sampleMethod_MethodCalls = line.FindExpressions<MethodCall>(); //represents all of the method calls within the method

                foreach (var methodCall in sampleMethod_MethodCalls) //goes through each method call
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
            MethodDeclarationNode expMDN = null;
            if (expResult is ReturnStatement)
            {
                Console.WriteLine("return");
            }
            else
            {
                var mCall = expResult.FindExpressions<MethodCall>().First();

                expMDN = new MethodDeclarationNode(mCall.Name, mc);
            }
            //MethodDeclarationNode mdn2 = new MethodDeclarationNode(expResult.ToString(), mc);

            //BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);
            //Console.WriteLine("InClass = " + rule.InClass(mdn)); //REQUIRED in order for the ConstructSwum method to work
            //rule.ConstructSwum(mdn); //rewrites mdn.ToString to a SWUM breakdown
            //Console.WriteLine(mdn.Action.ToString());

            BaseVerbRule rule2 = new BaseVerbRule(posData, tagger, splitter);
            Console.WriteLine("InClass = " + rule2.InClass(expMDN)); //REQUIRED in order for the ConstructSwum method to work
            rule2.ConstructSwum(expMDN); //rewrites mdn.ToString to a SWUM breakdown
            Console.WriteLine(expMDN.Action.ToString());
            //Console.WriteLine(mdn.Action.ToString());


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


            //var dataProject = new DataProject<CompleteWorkingSet>("CodeAnalysisToolkit",
            //    Path.GetFullPath("..//..//..//samples"),
            //    "..//..//..//SrcML");

            var dataProject = new DataProject<CompleteWorkingSet>(folderName,
                Path.GetFullPath(fullFilePath),
                "..//..//..//SrcML");


            List<String> success = new List<String>();
            Dictionary<SwumRule, bool> inClasses = null;

            string methodName = arg;
            bool debug = false; ///////////////// DEBUGGING

            // Get SrcML stuff in order
            dataProject.UpdateAsync().Wait();
            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(1000, out globalNamespace));


            // Initialize Swum
            splitter = new ConservativeIdSplitter();
            tagger = new UnigramTagger();
            posData = new PCKimmoPartOfSpeechData();



            var methodList = globalNamespace.GetDescendants<MethodDefinition>().Where(m => m.Name == methodName);
            MethodDefinition topMethod = null;

            // Check if the method was found
            try
            {
                topMethod = methodList.First();
            }
            catch (System.InvalidOperationException)
            {
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


            // Get the action verb from the SWUM
            String methodVerb = GetMethodVerb(methodName, mc);
            Console.WriteLine("Verb = \t\t" + methodVerb);
            Console.WriteLine("============================");

            // Get all of the lines of code that contains the verb in any form
            var expr = topMethod.GetDescendants().Where(t => t.ToString().Contains(methodVerb)).ToArray();

            // Iterate each line
            foreach (Statement t in expr)
            {
                if (debug) Console.WriteLine("Line: " + t.ToString());
                // This finds any method calls in this line and finds the verb in that method - from GetCallsTo()
                var methods = t.FindExpressions<MethodCall>(true).Where(c => c.ToString().ToLower().Contains(methodVerb));//c.Name.Contains(methodVerb));

                // Iterate through a list of the method calls in a line and find ones that contain the verb
                foreach (MethodCall i in methods)
                {

                    if (debug) Console.WriteLine("===\n" + i.Name);

                    MethodDeclarationNode mdnNEW = new MethodDeclarationNode(i.Name, mc);

                    inClasses = InClassChecker(i.Name, mc);


                    String foundVerb = GetMethodVerb(i.Name, mc);
                    if (foundVerb.Equals("!NONE!"))
                    {
                        if (debug) Console.WriteLine("   Method does not contain verb");
                    }
                    else
                    {
                        if (debug) Console.WriteLine("GetMethodVerb= " + GetMethodVerb(i.Name, mc));

                        success.Add(i.Name);
                    }


                    if (debug) Console.WriteLine("CompareSwums= " + CompareSwums(i.Name, mc));

                    // Debugging
                    if (debug)
                    {
                        int numbtrue = 0;
                        foreach (KeyValuePair<SwumRule, bool> entry in inClasses)
                        {
                            if (entry.Value)
                            {
                                numbtrue++;
                                mdnNEW = new MethodDeclarationNode(i.Name, mc);
                                entry.Key.InClass(mdnNEW);
                                entry.Key.ConstructSwum(mdnNEW);

                                Console.WriteLine("\t" + entry.Key.GetType().ToString() + new String(' ', 30 - entry.Key.GetType().ToString().Length) + " = " + mdnNEW.ToString());

                                if (mdnNEW.Action.ToPlainString().Equals(methodVerb, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Console.WriteLine("\tMethod contains the verb");
                                }
                                else
                                {
                                    Console.WriteLine("\tMethod does not contain the verb" + "\n");
                                }

                            }

                        }
                        Console.WriteLine(" Inclasses  = " + numbtrue);

                    } //end if debug



                } // End Method Iteration
            } // End Line Iteration

            if (success.Count == 0)
            {
                Console.WriteLine("===== No Same-Action Methods Found =====");
                //Assert.Fail("No Same-Action Methods found");
            }
            else
            {
                Console.WriteLine("\n============= SUCCESSES ===============");
                foreach (String i in success)
                {
                    Console.WriteLine(i);
                }
                //Assert.Pass("Same-Action methods found, check Output");

            }



            dataProject.WorkingSet.ReleaseReadLock();
        } // End Same-Action main method

        /// <summary>
        /// Checks if each SwumRule (13) returns (InClass = true) for the given method name.  returns a dictionary of the SwumRule and boolean.  
        /// Note that to build the swum you still have to run InClass for that method name.
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="mc">The MethodContext object generated from the top-level method.  This is required to generate a swum for any method</param>
        /// <returns>Dictionary of SwumRules and boolean thats true if InClass = true</returns>
        Dictionary<SwumRule, bool> InClassChecker(String name, MethodContext mc)
        {

            MethodDeclarationNode mdn = new MethodDeclarationNode(name, mc);

            Dictionary<SwumRule, bool> rules = new Dictionary<SwumRule, bool>
            {
                {new BaseVerbRule(posData, tagger, splitter), false},
                {new CheckerRule (posData, tagger, splitter), false},
                {new ConstructorRule(posData, tagger, splitter), false},
                {new DefaultBaseVerbRule(posData, tagger, splitter), false},
                {new DestructorRule(posData, tagger, splitter), false},
                {new EmptyNameRule(posData, tagger, splitter), false},
                {new EventHandlerRule(posData, tagger, splitter), false},
                {new FieldRule(posData, tagger, splitter), false},
                {new LeadingPrepositionRule(posData, tagger, splitter), false},
                {new NonBaseVerbRule(posData, tagger, splitter), false},
                {new NounPhraseRule(posData, tagger, splitter), false},
                {new ReactiveRule(posData, tagger, splitter), false},
                {new SpecialCaseRule(posData, tagger, splitter), false}
                //{new SwumRule(posData, tagger, splitter),""},     these are abstract
                //{new UnigramMethodRule(posData, tagger, splitter),""},
                //{new UnigramRule(posData, tagger, splitter),""}
            };

            var listing = rules.Keys.ToList();

            foreach (SwumRule ent in listing)
            {
                // Console.WriteLine(ent.GetType() + "   InClass = " + ent.InClass(mdn));
                if (ent.InClass(mdn))
                {
                    rules[ent] = true;
                }
            }


            return rules;

        } //end InClassChecker




        Dictionary<SwumRule, MethodDeclarationNode> BuildValidSwums(String name, MethodContext mc)
        {
            Dictionary<SwumRule, bool> inClasses = InClassChecker(name, mc);
            MethodDeclarationNode mdn = null;
            Dictionary<SwumRule, MethodDeclarationNode> validSwums = new Dictionary<SwumRule, MethodDeclarationNode>();

            foreach (KeyValuePair<SwumRule, bool> entry in inClasses)
            {
                if (entry.Value)
                {
                    mdn = new MethodDeclarationNode(name, mc);
                    entry.Key.InClass(mdn);
                    entry.Key.ConstructSwum(mdn);

                    validSwums.Add(entry.Key, mdn);

                }
            }

            return validSwums;

        }



        /// <summary>
        /// Returns the verb given a method name using all of the SwumRules.
        /// This does not make sure all of the rules returned the same verb, it returns the first one found.
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="mc">The MethodContext object generated from the top-level method.  This is required to generate a swum for any method</param>
        String GetMethodVerb(String name, MethodContext mc)
        {
            Dictionary<SwumRule, bool> inClasses = InClassChecker(name, mc);
            bool found = false;
            Dictionary<SwumRule, MethodDeclarationNode> validSwums = BuildValidSwums(name, mc);

            foreach (KeyValuePair<SwumRule, MethodDeclarationNode> entry in validSwums)
            {
                if (entry.Value.Action.ToPlainString().Length > 0)
                {
                    found = true;
                    return entry.Value.Action.ToPlainString();
                }
            }

            if (!found)
            {
                return "!!NONE FOUND!!"; //TODO
            }
            else
            {  //should never get to this code but its needed to make the compiler happy
                return null;
            }

        }// end GetMethodVerb



        /// <summary>
        /// Given a method name, check if the generated swums are all the same.
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="mc">The MethodContext object generated from the top-level method.  This is required to generate a swum for any method</param>
        /// <returns>True - if they are all the same
        /// False - if any are different</returns>
        bool CompareSwums(string name, MethodContext mc)
        {

            Dictionary<SwumRule, bool> inClasses = InClassChecker(name, mc);
            List<MethodDeclarationNode> mdns = BuildValidSwums(name, mc).Values.ToList<MethodDeclarationNode>();
            List<string> swums = new List<string>();

            foreach (MethodDeclarationNode m in mdns)
            {
                swums.Add(m.ToString());
            }

            // Compare

            //returns true if something different is found
            if (swums.Any(o => o != swums[0]))
            {
                return false;
            }
            else return true;

        }// end compareSwums

    }
    
}
