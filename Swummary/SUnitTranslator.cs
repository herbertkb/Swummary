using System;
using System.Collections.Generic;
using System.Linq;
using ABB.Swum;
using ABB.Swum.Nodes;
using ABB.SrcML.Data;
using System.Xml.Linq;


/// <summary>
/// Static class providing methods to translate s_units into SWUM structures
/// </summary>
public static class SUnitTranslator
{

    public static Swummary.SUnit Translate (Statement sunit) {
        return new Swummary.SUnit();
    }


}
