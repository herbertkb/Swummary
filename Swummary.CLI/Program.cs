using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;			// to run srcml as external process
using System.ComponentModel;

namespace Swummy.CLI
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

				Console.WriteLine (srcmlOutput);

				// extract classes with all names that appear in that class with LINQ

				// datastruct that maps class names to comments, methods, attributes


			}
			// pretty print the datastruct to console

		}

		/// <summary>
		/// Runs srcml against a source code file. 
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
			srcml.Start ();

			StreamReader readStdOut = srcml.StandardOutput;
			string output = readStdOut.ReadToEnd ();

			return output;
		}
	}
}