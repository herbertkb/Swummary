using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swummary;

namespace Swummary.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = args[0];
            var swummaries = Swummary.Swummarize(directory);

            // top header
            Console.WriteLine("# Swummary for " + directory);

            foreach (var s in swummaries)
            {
                string methodName = s.Item1;
                string methodSummary = s.Item2;

                // Subheaders with each method name followed by swummary in normal text.
                Console.WriteLine("## {0}", methodName);
                Console.WriteLine(methodSummary);

                //Console.WriteLine("{0}\n{1}\n", methodName, methodSummary);
            }
        }
    }
}
