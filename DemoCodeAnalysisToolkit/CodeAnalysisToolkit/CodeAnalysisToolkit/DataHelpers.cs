using ABB.SrcML;
using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CodeAnalysisToolkit
{
    public class DataHelpers
    {
        public static XElement GetElement(SrcMLArchive archive, SrcMLLocation location)
        {
            string fileName = location.SourceFileName;
            string query = location.XPath;

            var unit = archive.GetXElementForSourceFile(fileName);
            var element = unit.XPathSelectElement(query, SrcMLNamespaces.Manager);

            return element;
        }

    }
}
