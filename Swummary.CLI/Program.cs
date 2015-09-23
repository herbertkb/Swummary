using System;
using System.Collections.Generic;

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


			// loop over files
			foreach (string filename in files) 
			{
				// parse them with srcML

				// extract class names 

				// datastruct that maps class names to comments, methods, attributes


			}
			// pretty print the datastruct to console

		}
	}
}