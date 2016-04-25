using System;
using System.Collections.Generic;
using System.Linq;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.SrcML.Data;
using System.Xml.Linq;
using ABB.Swum.WordData;
using Swummary;
using ABB.SrcML.Test.Utilities;
using System.Text;
using System.Xml;
using ABB.SrcML;
//using ABB.SrcML;
using System.IO;
 
 
 
/// <summary>
/// Static class providing methods to translate s_units into SWUM structures
/// </summary>
public static class SUnitTranslator
{

    private static BaseVerbRule SetupBaseVerbRule()
    {
        var splitter = new ConservativeIdSplitter();
        var tagger = new UnigramTagger();              
        var posData = new PCKimmoPartOfSpeechData();

        return new BaseVerbRule(posData, tagger, splitter);
    }
    private static FieldRule SetupFieldRule()
    {
        var splitter = new ConservativeIdSplitter();
        var tagger = new UnigramTagger();
        var posData = new PCKimmoPartOfSpeechData();

        return new FieldRule(posData, tagger, splitter);
    }

    static SUnit findType(Statement statement)
    {

        /*
         * == MethodCall
         * use code from sunitextractor to parse this and find a MethodCall
         * return TranslateMethodCall(statement);
         *
         * == Assignment
         * else??
         * or find equals sign? -- no because methodcall might have an equals sign
         *
         * == Return
         * find "return" in statement
         * return TranslateReturn(statement);
         *
        */

        //or pass thru to each type
        return new SUnit();
    }


    public static SUnit Translate(Statement statement)
    {
        if(statement is ReturnStatement)
        {
            return TranslateReturn(statement);
        }

        // Search statement for use of an assignment operator
        if (statement.GetDescendants<OperatorUse>()
                    .Where(o => o.Text.Equals("="))
                    .Count() > 0)
        {
            return TranslateAssignment(statement);

        }

        else
        {
            return TranslateMethodCall(statement);
        }
    }

    private static SUnit TranslateAssignment(Statement statement)
    {
        // action = "Assign"
        // define left-hand-side (lhs)
        // theme = right hand side

        var fieldRule = SetupFieldRule();

        var equalsSign = statement.GetDescendants<OperatorUse>()
                                .Where(o => o.Text.Equals("=")).First();

        var lhs = equalsSign.GetSiblingsBeforeSelf<VariableUse>().First();
        
        var lhsFieldContext = new FieldContext(lhs.ResolveType().First().ToString(), false, "");
        var lhsDecNode = new FieldDeclarationNode(lhs.ToString(), lhsFieldContext);
        fieldRule.InClass(lhsDecNode);
        fieldRule.ConstructSwum(lhsDecNode);

        var rhsString = "";
        var rhs = equalsSign.GetSiblingsAfterSelf<Expression>().First();
        if (rhs is VariableUse)
        {
            var rhsFieldContext = new FieldContext(rhs.ResolveType().First().ToString(), false, "");
            var rhsDecNode = new FieldDeclarationNode(rhs.ToString(), lhsFieldContext);
            fieldRule.InClass(rhsDecNode);
            fieldRule.ConstructSwum(rhsDecNode);

            rhsString = rhsDecNode.ToPlainString();

        }
        else if (rhs is MethodCall)
        {
            
            string type = rhs.ResolveType().ToString();

            MethodContext mc = new MethodContext(type);
            MethodDeclarationNode mdn = new MethodDeclarationNode(rhs.ToString(), mc);

            var swumRule = SetupBaseVerbRule();
            swumRule.InClass(mdn);
            swumRule.ConstructSwum(mdn);

            rhsString = mdn.ToPlainString();
        }


        var sunit = new SUnit();
        sunit.action = "Assign";
        sunit.lhs = lhsDecNode.ToPlainString();
        sunit.theme = rhsString;
       
        return sunit;
    }

    public static SUnit TranslateReturn(Statement statement)
    {
        var action = "return";
        var expressions = statement.GetExpressions();

        var theme = String.Join(" ", expressions);
        return new SUnit(SUnitType.Return, action, theme, "", new List<string>(), "");
    }

    public static SUnit TranslateMethodCall(Statement statement)
    {
        var expressions = statement.GetExpressions();
        
        // Give an empty SUnit if statement has no expressions.  
        if (expressions.Count() == 0)
        {
            return new SUnit(SUnitType.SingleMethodCall, "", "", "", new List<string>(), "void");
        }

        // Build a minimal method context and declaration node required by SWUM. 
        var exp = expressions.First();
        string type = exp.ResolveType().ToString();
        MethodContext mc = new MethodContext(type);
        MethodDeclarationNode mdn = new MethodDeclarationNode(exp.ToString(), mc);

        // Apply the SWUM to our statement
        var swumRule = SetupBaseVerbRule();
        swumRule.InClass(mdn);
        swumRule.ConstructSwum(mdn);

        // Build and return SUnit from the SWUM
        SUnit sunit = new SUnit();
        sunit.action = GetAction(mdn);
        sunit.theme = GetTheme(mdn);
        sunit.args = GetArgs(mdn);

        return sunit;
    }


    //generate action from MDN
    static String GetAction(MethodDeclarationNode mdn)
    {
        return mdn.Action.ToPlainString().ToLower();
    }


    //generate theme from MDN
    static String GetTheme(MethodDeclarationNode mdn)
    {
        // If mdn sucks, then theme is empty string.
        if (mdn.Theme == null){ return "";  }
        
        return mdn.Theme.ToPlainString().ToLower();
    }


    //generate args from MDN
    static IEnumerable<String> GetArgs(MethodDeclarationNode mdn)
    {
        var list = new List<String>();
        List<ArgumentNode> gg = mdn.SecondaryArguments;
        
        if (gg != null)
        {
            foreach (ArgumentNode i in gg)
            {
                list.Add(i.ToPlainString().ToLower());
            }
        }

        return list;
    }


/*******************
    // generate swum given name
    //I decided to give up on the string swum builder and just go with this

    static MethodDeclarationNode BuildMethodCallSwum(String name)
    {
        string methodName = "GetInt";
        string folderName = "Sample Methods";
        string fullFilePath = "..//..//..//projects//Sample Methods";

        var dataProject = new DataProject<CompleteWorkingSet>(folderName,
            Path.GetFullPath(fullFilePath),
            "..//..//..//SrcML");


        List<String> success = new List<String>();
        Dictionary<SwumRule, bool> inClasses = null;


        // Get SrcML stuff in order
        dataProject.UpdateAsync().Wait();
        NamespaceDefinition globalNamespace;
        dataProject.WorkingSet.TryObtainReadLock(1000, out globalNamespace);

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
        }

        var guiMethodXElement = DataHelpers.GetElement(dataProject.SourceArchive, topMethod.PrimaryLocation);

        //generate swum for method declaration
        MethodContext mc = ContextBuilder.BuildMethodContext(guiMethodXElement);
        MethodDeclarationNode mdn = new MethodDeclarationNode(methodName, mc);
        BaseVerbRule rule = new BaseVerbRule(posData, tagger, splitter);

        mdn = new MethodDeclarationNode(name, mc);
        rule.InClass(mdn);
        rule.ConstructSwum(mdn);

        return mdn;
    }
*****************************************************************************/

}
