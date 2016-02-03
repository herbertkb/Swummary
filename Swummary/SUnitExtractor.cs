using System;
using System.Collections.Generic;
using System.Linq;


public class SUnitExtractor
{
    public SUnitExtractor ()
	{

	}

    public void SetMethod (XElement method) { }
    public String GetCurrentMethodName() { }

    public IEnumerable<XElement> GetSameAction() { }

    public IEnumerable<XElement> GetVoidReturn() { }

    public IEnumerable<XElement> GetEnding() { }

}
