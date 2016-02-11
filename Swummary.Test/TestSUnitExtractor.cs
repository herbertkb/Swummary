using NUnit.Framework;
using System.Xml.Linq;

/**
 * John:	findInOne, setFindInFilesDirFilter
 * Keith:	findInOne, gethurrry, findFilesInOut
 * Dylan: 	return true
*/

[TestFixture]
public class TestSUnitExtractor
{

    // Create a dummy XElement object from sample method code
    // by taking the raw XML output from srcml on a method from our samplemethods.cpp file
    // and converting the string to a srcml method XML element.
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
    
    [TestCase]
    public void LoadMethodIntoSUnitExtractor() {

        var srcmlMethod = XElement.Parse(srcmlOutput);

        var extractor = new SUnitExtractor();
        extractor.SetMethod(srcmlMethod);

        Assert.AreEqual("findInFiles", extractor.GetCurrentMethodName() );
    }




    [TestCase]
    public void GetSameActionSUnits() {

        var srcmlMethod = XElement.Parse(srcmlOutput);
        
        var extractor = new SUnitExtractor();
        extractor.SetMethod(srcmlMethod);

        var sameAction = XElement.Parse(@"<expr_stmt><expr><call><name>findFilesInOut</name><argument_list>()</argument_list></call></expr>;</expr_stmt>");
        Assert.Contains(sameAction, (System.Collections.IList)extractor.GetSameAction() );

    }

    [TestCase]
    public void GetVoidReturnSUnits() { Assert.AreEqual(1, 2, "Not implemented."); }

    [TestCase]
    public void GetEndingSUnits() { Assert.AreEqual(1,2, "Not implemented."); }
}
