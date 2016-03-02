using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABB.SrcML;
using ABB.SrcML.Data;
using ABB.Swum;

using NUnit.Framework;
using ABB.SrcML.Test.Utilities;

namespace Swummary.Test
{
    /// <summary>
    /// Unit test that shows source to summary usage of the Swummary API.
    /// </summary>
    class TestPipeline
    {
        [Test]
        public void TestPipelineXMLSnippet ()
        {
            // SrcML sample method
            string srcmlOutput = @"<function><type><name> bool </name></type> <name> findInFiles </name><parameter_list> () </parameter_list>
                            <block>{
	                            <decl_stmt><decl><type><specifier>const</specifier> <name>TCHAR</name> <modifier>*</modifier></type><name>dir2Search</name> <init>= <expr><call><name><name>_findReplaceDlg</name><operator>.</operator><name>getDir2Search</name></name><argument_list>()</argument_list></call></expr></init></decl>;</decl_stmt>

	                            <expr_stmt><expr><call><name>findFilesInOut</name><argument_list>()</argument_list></call></expr>;</expr_stmt>
	                            <if>if <condition>(<expr><operator>!</operator><name><name>dir2Search</name><index>[<expr><literal type=""number"">0</literal></expr>]</index></name> <operator>||</operator> <operator>!</operator><call><name><operator>::</operator><name>PathFileExists</name></name><argument_list>(<argument><expr><name>dir2Search</name></expr></argument>)</argument_list></call></expr>)</condition><then>
	                            <block>{
		                            <return>return <expr><literal type = ""boolean"" > false </literal></expr>;</return>
	                            }</block></then></if>
	                            <decl_stmt><decl><type><name>string</name></type> <name>findString</name> <init>= <expr><literal type = ""string"" > """" </literal ></expr ></init ></decl >;</decl_stmt>

	                            <expr_stmt><expr><call><name>gethurry</name><argument_list>()</argument_list></call></expr>;</expr_stmt>
	
	                            <macro><name>findInOne</name><argument_list>(<argument>int a</argument>, <argument>findString</argument>)</argument_list></macro><empty_stmt>;</empty_stmt>

	                            <decl_stmt><decl><type><name>bool</name></type> <name>isRecursive</name> <init>= <expr><call><name><name>_findReplaceDlg</name><operator >.</operator><name>isRecursive</name></name><argument_list>()</argument_list></call></expr></init></decl>;</decl_stmt>
	                            <decl_stmt><decl><type><name>bool</name></type> <name>isInHiddenDir</name> <init>= <expr><call><name><name>_findReplaceDlg</name><operator >.</operator><name>isInHiddenDir</name></name><argument_list>()</argument_list></call></expr></init></decl>;</decl_stmt>

	                            <if>if <condition>(<expr><call><name><name>a</name><operator >.</operator><name>size</name></name><argument_list>()</argument_list></call> <operator >==</operator> <literal type = ""number"" > 0 </literal></expr>)</condition><then>
	                            <block>{
		                            <expr_stmt><expr><call><name><name>a</name><operator >.</operator><name>setFindInFilesDirFilter</name></name><argument_list>(<argument><expr><literal type = ""string""> ""dddd"" </literal ></expr ></argument>, <argument><expr><call><name>TEXT</name><argument_list>(<argument><expr><literal type = ""string"" > ""*.*"" </literal ></expr></argument>)</argument_list></call></expr></argument>)</argument_list></call></expr>;</expr_stmt>
		                            <expr_stmt><expr><call><name><name>a</name><operator >.</operator><name>getPatterns</name></name><argument_list>(<argument><expr><name>findString</name></expr></argument>)</argument_list></call></expr>;</expr_stmt>
	                            }</block></then></if>
	                            <return>return <expr><literal type = ""boolean"" > true </literal ></expr>;</return>
                            }</block></function>";

            // Convert raw string to MethodDefinition
            var fileSetup = new SrcMLFileUnitSetup(Language.CPlusPlus);
            var parser = new CPlusPlusCodeParser();

            var fileUnit = fileSetup.GetFileUnitForXmlSnippet(srcmlOutput, "sampletestmethods.cpp");
            var scope = parser.ParseFileUnit(fileUnit);

            var srcmlMethod = scope.GetDescendants<MethodDefinition>().First();

            // Verify the method definition
            Assert.IsInstanceOf<MethodDefinition>(srcmlMethod, "MethodDefinition found.");
            Console.WriteLine(srcmlMethod.ToString());

            // Extract SUnit Statements from MethodDefinition
            var statements = new List<Statement>();
            statements.AddRange(SUnitExtractor.ExtractEnding(srcmlMethod));
            statements.AddRange(SUnitExtractor.ExtractSameAction(srcmlMethod));
            statements.AddRange(SUnitExtractor.ExtractVoidReturn(srcmlMethod));

            // verify the statements selected
            Assert.IsNotEmpty(statements, "statements selected from method definition");
            Console.WriteLine(statements.ToString());

            // Translate Statements into SUnits
            List<SUnit> sunits = statements.ConvertAll(
                        new Converter<Statement, SUnit> (SUnitTranslator.Translate) );

            // verify sunits have been translated
            Assert.That(sunits.TrueForAll(s => s.action != null), "All SUnits initialized.");
            Console.WriteLine(sunits.ToString());
            

            // Generate text from SUnits
            List<string> sentences = sunits.ConvertAll(
                        new Converter<SUnit, string> (TextGenerator.GenerateText) );

            // verify string generated
            Assert.That(sentences.TrueForAll(s => s.Length > 0));
            Console.WriteLine(sentences);

            // Collect text and summarize
            var methodDocument = String.Join<string>(" ", sentences);
            var summary = Summarizer.Summarize(methodDocument);


            // verify summary
            Assert.That(! summary.Equals(""));
            Console.WriteLine(summary);

        }

        [Test]
        public void TestPipelineWithSourceFile()
        {
            // TODO: rewrite unit test to handle all the methods in a file
            // For now, just pull the first method from the file and proceed as TestPipelineXMLSnipper().
            var srcmlMethod = MethodExtractor.ExtractAllMethodsFromFile("../Sample Methods/sample methods.cpp").First().Item3;

            // Verify the method definition
            Assert.IsInstanceOf<MethodDefinition>(srcmlMethod, "MethodDefinition found.");
            Console.WriteLine(srcmlMethod.ToString());

            // Extract SUnit Statements from MethodDefinition
            var statements = SUnitExtractor.ExtractAll(srcmlMethod).ToList();

            // verify the statements selected
            Assert.IsNotEmpty(statements, "statements selected from method definition");
            Console.WriteLine(statements.ToString());

            // Translate Statements into SUnits
            List<SUnit> sunits = statements.ConvertAll(
                        new Converter<Statement, SUnit>(SUnitTranslator.Translate));

            // verify sunits have been translated
            Assert.That(sunits.TrueForAll(s => s.action != null), "All SUnits initialized.");
            Console.WriteLine(sunits.ToString());


            // Generate text from SUnits
            List<string> sentences = sunits.ConvertAll(
                        new Converter<SUnit, string>(TextGenerator.GenerateText));

            // verify string generated
            Assert.That(sentences.TrueForAll(s => s.Length > 0));
            Console.WriteLine(sentences);

            // Collect text and summarize
            var methodDocument = String.Join<string>(" ", sentences);
            var summary = Summarizer.Summarize(methodDocument);


            // verify summary
            Assert.That(!summary.Equals(""));
            Console.WriteLine(summary);

        }
    }
}
