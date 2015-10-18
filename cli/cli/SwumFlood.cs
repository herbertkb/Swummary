using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ABB.SrcML.Data;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.Swum.WordData;

namespace Swummary
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("SWUM Flood\nUnleashes a flood of Srcml and Swum data");

			//String pathToData = "/home/kh/VCU/2015 Fall/CMSC 451/Swummary/testdata/";


			//TODO: Find a simpler way to setup the testing data
			var testProject = new DataProject<CompleteWorkingSet> 
					("testdata", 
					Path.GetFullPath ("/home/kh/VCU/2015\\ Fall/CMSC\\ 451/Swummary/testdata/"),
				    "/usr/bin/srcml");

			testProject.UpdateAsync ().Wait ();
			NamespaceDefinition globalNamespace;
			testProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace);

			// SWUM STUFF
//			idSplitter = new ConservativeIdSplitter();
//			tagger = new UnigramTagger();
//			posData = newPCKimmoPartOfSpeechData ();	// PC Kimmo is an ancient POS tagger.
//
			foreach (var expression in globalNamespace.GetExpressions()) {
				expression.ToString ();
			}









		}
	}
}
