using ABB.SrcML.Data;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.Swum.WordData;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

public class SUnitExtractor
{
    string methodName = "GetInt";
    string folderName = "Sample Methods";
    string fullFilePath = "..//..//..//projects//Sample Methods";

    private static ConservativeIdSplitter splitter;
    private static UnigramTagger tagger;
    private static PCKimmoPartOfSpeechData posData;

    public SUnitExtractor ()
	{

	}

    public void SetMethod (XElement method) { }
    public String GetCurrentMethodName() { return ""; }

    public IEnumerable<XElement> GetSameAction() { return new List<XElement>(); }

    public IEnumerable<XElement> GetVoidReturn() { return new List<XElement>(); }

    public IEnumerable<XElement> GetEnding() {

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

        return new List<XElement>();
    }

}
