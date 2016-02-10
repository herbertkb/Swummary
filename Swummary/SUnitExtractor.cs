using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ABB.SrcML;

public class SUnitExtractor
{
    public SUnitExtractor ()
	{

	}

    public void SetMethod (XElement method) { }
    public String GetCurrentMethodName() { return ""; }

    public IEnumerable<XElement> GetSameAction() { return new List<XElement>(); }

    public IEnumerable<XElement> GetVoidReturn() { return new List<XElement>(); }

    public IEnumerable<XElement> GetEnding() { return new List<XElement>(); }

}
