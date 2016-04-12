using System;
using NUnit.Framework;
using ABB.SrcML.Data;
using ABB.SrcML.Test.Utilities;
using ABB.SrcML;
using System.Linq;
using System.Text.RegularExpressions;

[TestFixture]
public class TestSUnitTranslator
{

    /******************************************************************************
     * John:	findInOne, setFindInFilesDirFilter
     * Keith:	findInOne, gethurrry, findFilesInOut
     * Dylan: 	return true

            bool findInFiles()
            {
                const TCHAR* dir2Search = _findReplaceDlg.getDir2Search();

                findFilesInOut();
                if (!dir2Search[0] || !::PathFileExists(dir2Search))
                {
                    return false;
                }
                string findString = "";

                gethurry();

                findInOne(int a, findString);

                bool isRecursive = _findReplaceDlg.isRecursive();
                bool isInHiddenDir = _findReplaceDlg.isInHiddenDir();

                if (a.size() == 0)
                {
                    a.setFindInFilesDirFilter("dddd", TEXT("*.*"));
                    a.getPatterns(findString);
                }

                return true;
            }

    ******************************************************************************/

    // Create a dummy method defintion object from sample method code
    // by taking the raw XML output from srcml on a method from our samplemethods.cpp file
    // and converting the string to a srcml method definition within the [Setup] method
    string srcmlOutput = @"<function><type><name> bool </name></type> <name> findInFiles </name><parameter_list> () </parameter_list>
                        <block>{
	                        <decl_stmt><decl><type><specifier>const</specifier> <name>TCHAR</name> <modifier>*</modifier></type><name>dir2Search</name> <init>= <expr><call><name><name>_findReplaceDlg</name><operator>.</operator><name>getDir2Search</name></name><argument_list>()</argument_list></call></expr></init></decl>;</decl_stmt>

	                        <expr_stmt><expr><call><name>findFilesInOut</name><argument_list>()</argument_list></call></expr>;</expr_stmt>
	                        <if>if <condition>(<expr><operator>!</operator><name><name>dir2Search</name><index>[<expr><literal type=""number"">0</literal></expr>]</index></name> <operator>||</operator> <operator>!</operator><call><name><operator>::</operator><name>PathFileExists</name></name><argument_list>(<argument><expr><name>dir2Search</name></expr></argument>)</argument_list></call></expr>)</condition><then>
	                        <block>{
		                        <return>return <expr><literal type = ""boolean"" > false </literal></expr>;</return>
	                        }</block></then></if>
	                        <decl_stmt><decl><type><name>string</name></type> <name>findString</name> <init>= <expr><literal type = ""string"" > """" </literal ></expr ></init ></decl >;</decl_stmt>

	                        <expr_stmt><expr><call><name>getHurry</name><argument_list>()</argument_list></call></expr>;</expr_stmt>
	
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
    MethodDefinition methodDef;

    [SetUp]
    public void PrepareMethodDefinition()
    {
        // SrcML objects which help generate the SrcML objects we need to test
        var fileSetup = new SrcMLFileUnitSetup(Language.CPlusPlus);
        var parser = new CPlusPlusCodeParser();

        var fileUnit = fileSetup.GetFileUnitForXmlSnippet(srcmlOutput, "sampletestmethods.cpp");
        var scope = parser.ParseFileUnit(fileUnit);

        // Create the method srcml object in which to search for s-units.
        methodDef = scope.GetDescendants<MethodDefinition>().First();
    }


    [TestCase]
    public void TranslateSameActionSUnit()
    {
        var sameAction = methodDef.GetDescendants<Statement>()
                .First(s => Regex.IsMatch(s.ToString(), "setFindInFilesDirFilter"));

        var translated = SUnitTranslator.Translate(sameAction);

        Console.WriteLine("SUnit Type: {0}\nAction: {1}\nTheme: {2}",
                    translated.type, translated.action, translated.theme);

        Assert.AreEqual("set", translated.action, "correct action");
        Assert.AreEqual("filter", translated.theme, "exact theme");
        Assert.IsTrue(Regex.IsMatch(translated.theme, "filter"), "vague theme");
    }

    [TestCase]
    public void TranslateVoidReturnSUnit()
    {
        var voidReturn = methodDef.GetDescendants<Statement>()
                .First(s => Regex.IsMatch(s.ToString(), "getHurry"));

        var translated = SUnitTranslator.Translate(voidReturn);

        Console.WriteLine("SUnit Type: {0}\nAction: {1}\nTheme: {2}", 
                            translated.type, translated.action, translated.theme);

        Assert.AreEqual("get", translated.action, "correct action");
        Assert.AreEqual("hurry", translated.theme, "exact theme");
    }

    [TestCase]
    public void TranslateEndingSUnit() {
        var ending = methodDef.GetDescendants<Statement>()
                .First(s => Regex.IsMatch(s.ToString(), "return"));

        ending = methodDef.GetDescendants<ReturnStatement>().First();

        Console.WriteLine("Ending statement: {0}", ending);

        var translated = SUnitTranslator.Translate(ending);

        Assert.AreEqual("Return", translated.action, "correct action");
        Assert.AreEqual("true", translated.theme, "exact theme");
        Assert.IsTrue(Regex.IsMatch(translated.theme, "true"), "vague theme");
    }

}