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
    private static ConservativeIdSplitter splitter;
    private static UnigramTagger tagger;
    private static PCKimmoPartOfSpeechData posData;


    static Swummary.SUnit findType(Statement statement)
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


    //this goes to findType and pases thru to each case
    public static Swummary.SUnit TranslateAny(Statement statement)
    {
        return findType(statement);

    }



    public static Swummary.SUnit TranslateVoid(Statement statement)
    {
        //TODO this should be the same
        return new Swummary.SUnit();
    }




    public static Swummary.SUnit TranslateReturn(Statement statement)
    {

        //TODO: TEMPORARY STATEMENT -- where do i find the normal one
        MethodDefinition TEMP = new MethodDefinition();

        var statements = SUnitExtractor.ExtractEnding(TEMP);

        foreach (Statement stat in statements)
        {


        }

        //
        /*  
         new SUnit object
         return = verb
         
         if var then get type
         if methodcall then generate swum
         
        */

        return new Swummary.SUnit();
    }



    public static Swummary.SUnit TranslateMethodCall(Statement statement)
    {

        //TODO: TEMPORARY STATEMENT -- where do i find the normal one
        MethodDefinition TEMP = new MethodDefinition();

        var statements = SUnitExtractor.ExtractEnding(TEMP); /////// im confused why im sending something to this, shouldnt i be passing it nothing and getting the statements


        foreach (Statement stat in statements)
        {
            SUnit sunit = new SUnit();

            //
            var mdn = BuildMethodCallSwum(stat.GetDescendants<MethodCall>().First().ToString());

            sunit.action = GetAction(mdn);
            sunit.theme = GetTheme(mdn);
            sunit.args = GetArgs(mdn);

        }

        return new Swummary.SUnit();
    }


    //generate action from MDN
    static String GetAction(MethodDeclarationNode mdn)
    {
        return mdn.Action.ToPlainString();
    }


    //generate theme from MDN
    static String GetTheme(MethodDeclarationNode mdn)
    {
        return mdn.Theme.ToPlainString();
    }


    //generate args from MDN
    static IEnumerable<String> GetArgs(MethodDeclarationNode mdn)
    {
        var list = new List<String>();
        List<ArgumentNode> gg = mdn.SecondaryArguments;
        foreach (ArgumentNode i in gg)
        {
            list.Add(i.ToPlainString());
        }

        return list;

    }



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
}
