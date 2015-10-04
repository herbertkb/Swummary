using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.SrcML;
using ABB.SrcML.Data;
using NUnit.Framework;

namespace CodeAnalysisToolkit
{
    [TestFixture]
    public class SimpleAnalyticsCalculator
    {
        [TestCase]
        public void CalculateSimpleProjectStats()
        {
            var dataProject = new DataProject<CompleteWorkingSet>("npp_6.2.3",
                Path.GetFullPath("..//..//..//projects//npp_6.2.3"),
                "..//..//..//SrcML");

            dataProject.UpdateAsync().Wait();

            NamespaceDefinition globalNamespace;
            Assert.That(dataProject.WorkingSet.TryObtainReadLock(5000, out globalNamespace));

            Debug.WriteLine(dataProject.Data.GetFiles().Count() + " files");
            Debug.WriteLine(globalNamespace.GetDescendants<NamespaceDefinition>().Count() + " namespaces");
            Debug.WriteLine(globalNamespace.GetDescendants<TypeDefinition>().Count() + " types");
            Debug.WriteLine(globalNamespace.GetDescendants<MethodDefinition>().Count() + " methods");
            Debug.WriteLine("");

            var orderedMethodList = from method in globalNamespace.GetDescendants<MethodDefinition>()
                let loc = method.GetDescendants().Count()
                orderby loc descending
                select method;

            var top10 = orderedMethodList.Take(10);
            foreach (var m in top10)
            {
                Debug.WriteLine(m.GetFullName() + " - " + m.GetDescendants().Count());        
            }

        }

    }
}
