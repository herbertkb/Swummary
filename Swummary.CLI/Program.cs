using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;			// to run srcml as external process
using System.Linq;
using System.Xml.Linq;

namespace Swummary.CLI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// load source files from command line arguments
			var files = new List<string>();
			foreach (string filename in args) 
			{
				/* 
				 * put code here to check that files can be opened and quit on error
				*/

				// only add file after confirming it's legit
				files.Add (filename);
			}


			foreach (string filename in files) 
			{
				// parse them with srcML
				string srcmlOutput = RunSrcml (filename);
				//Console.WriteLine (srcmlOutput);

				// convert output to xml
				var srcmlXml = XDocument.Parse (srcmlOutput);

				// extract classes with all names that appear in that class with LINQ
				// classname => methods => namedentities
				// derived from: http://www.dotnetcurry.com/ShowArticle.aspx?ID=564
				var classes = srcmlXml.Descendants ("namespace");
				foreach (XElement classe in classes) {
					//Console.WriteLine (classe);
				}



				// datastruct that maps class names to comments, methods, attributes


			}
			// pretty print the datastruct to console

		}

		/// <summary>
		/// Runs srcml against a source code file and returns the output as a string. 
		/// </summary>
		/// <returns>A string for the output of srcml against the given sourcefile</returns>
		/// <param name="sourcefile">Sourcefile to be analyzed</param>
		private static string RunSrcml (string sourcefile) 
		{
			// derived from: 
			// https://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput.aspx
			var srcml = new Process ();
			srcml.StartInfo.FileName = "srcml";
			srcml.StartInfo.Arguments = sourcefile;
			srcml.StartInfo.UseShellExecute = false;
			srcml.StartInfo.RedirectStandardOutput = true;
			srcml.Start ();

			StreamReader readStdOut = srcml.StandardOutput;
			string output = readStdOut.ReadToEnd ();
			srcml.WaitForExit ();
			srcml.Close ();

			//Console.WriteLine (output);

			return output;
		}
	}
}