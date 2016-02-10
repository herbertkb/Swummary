using System;
using System.Xml;
using System.Collections.Generic;
using NUnit.Framework;
using ABB.SrcML;

//using Swummary;
using System.Xml.Linq;

[TestFixture]
public class TestSUnitExtractor
{

    // Create a dummy XElement object from sample method code
    // by taking the raw XML output from srcml on a method from our samplemethods.cpp file
    // and converting the string to a srcml method XML element.
    string srcmlOutput = @"<macro><name>breakEverything</name><argument_list>(<argument>string foo</argument>, <argument>int bar</argument>)</argument_list></macro><block>{
	                        <decl_stmt><decl><type><name>string</name></type> <name>foob</name> <init>= <expr><call><name>FindAndBreakTwoVerbs</name><argument_list>(<argument><expr><literal type = ""string"" > ""blahblahbreak"" </ literal ></ expr ></ argument >)</argument_list></call></expr></init></decl>;</decl_stmt>
	                        <decl_stmt><decl><type><name>int</name></type> <name>breakDont</name> <init>= <expr><literal type = ""number"" > 3892 </ literal ></ expr ></ init ></ decl >;</decl_stmt>
	                        <expr_stmt><expr><call><name>NoVerbreak</name><argument_list>()</argument_list></call></expr>;</expr_stmt>
	                        <expr_stmt><expr><call><name>voidReturn</name><argument_list>(<argument><expr><literal type = ""string"" > ""EqualsSign = true"" </ literal ></ expr ></ argument >, <argument><expr><literal type = ""number"" > 890 </ literal ></ expr ></ argument >)</argument_list></call></expr>;</expr_stmt>
	                         <for>for <control>(<init><decl><type><name>x</name> <name>in</name></type> <name>foo</name></decl></init>)</control><block>{
		                         <if>if <condition>(<expr><name>True</name></expr>)</condition><then><block>{
			                         <return>return <expr><name>False</name>
			                         <name>lastRealLine</name> <operator>=</operator> <name>True</name></expr>;</return>
		                         }</block></then></if>
	                         }</block></for>
                         }</block>";
    
    [TestCase]
    public void LoadMethodIntoSUnitExtractor() {

        var srcmlMethod = XElement.Parse(srcmlOutput);

        var extractor = new SUnitExtractor();
        extractor.SetMethod(srcmlMethod);

        Assert.AreEqual( "breakEverything", extractor.GetCurrentMethodName() );
    }

 /**
 * John		FindAndBreakTwoVerbs
 * Keith	NoVerbreak, voidReturn
 * Dylan	lastRealLine = True;
 */


    [TestCase]
    public void GetSameActionSUnits() {

        var srcmlMethod = XElement.Parse(srcmlOutput);

        var extractor = new SUnitExtractor();
        extractor.SetMethod(srcmlMethod);

        var sameAction = XElement.Parse(@"< decl_stmt >< decl >< type >< name > string </ name ></ type > < name > foob </ name > < init >= < expr >< call >< name > FindAndBreakTwoVerbs </ name >< argument_list > (< argument >< expr >< literal type = ""string"" > ""blahblahbreak"" </ literal ></ expr ></ argument >) </ argument_list ></ call ></ expr ></ init ></ decl >;</ decl_stmt >");

        Assert.AreEqual(sameAction.ToString(), extractor.GetSameAction().ToString() );

    }

    [TestCase]
    public void GetVoidReturnSUnits() { Assert.AreEqual(1, 2, "Not implemented."); }

    [TestCase]
    public void GetEndingSUnits() { Assert.AreEqual(1,2, "Not implemented."); }
}
