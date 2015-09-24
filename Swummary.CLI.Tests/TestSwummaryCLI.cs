/*
 * TestSwummaryCLI.cs
 * A collection of unit tests for Swummy's command line interface (CLI). 
 * 
 * Uses the NUnit Unit Testing framework (based on JUnit).
 * Design inspired from QNUnit quickstart guide: http://nunit.com/index.php?p=quickStart&r=2.6.4
 * And Chapter 1 of 'Programming C# 5.0' from Oreily Publishing (available on Safari)
 * 
 * 
 * Last Edit     | Author               | Comment
 * --------------+----------------------+----------------------
 * 23 Sep 2015   | Keith Herbert        | Getting started. Very rough. Probably doesn't work. Yay TDD.
*/

using System;
using NUnit.Framework;

namespace Swummary.CLI.Tests
{
    [TextClass]
    public class SwummyCLITest
    {
        [TestInitialize]
        public void Initialize ()
        {
            var output = new System.IO.StringWriter();
            Console.SetOut (output);

            Program.Main (new string[0]);

            output = output.GetStringBuilder ().ToString ().Trim ();
        }

        [Test]
        public void PrintsOutput()
        {
            Assert.IsNotNull (string output); 
        }
    }
}