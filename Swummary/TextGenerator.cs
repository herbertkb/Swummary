using ABB.Swum.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using Swummary;


public static class TextGenerator
{
  
    public static String GenerateText(Swummary.SUnit sunit) {

        String sentence = "";

        if (sunit.type == SUnitType.SingleMethodCall)
        {
            sentence += sunit.action + " " + sunit.theme;

            if (sunit.args != null && sunit.args.Count() > 0)
            {
                sentence += " given " + string.Join(", ", sunit.args);
            }
            
            if (sunit.hasReturnType)
            {
                sentence += " and get " + sunit.returnType;
            }

        }

        else if (sunit.type == SUnitType.Assignment)
        {
            sentence += sunit.action + " " + sunit.theme + " given ";

            foreach (String arg in sunit.args)
            {
                sentence += " " + arg;
            }

            if (sunit.hasReturnType)
            {
                sentence += " and get " + sunit.returnType;
            }

            sentence += " assign to " + sunit.lhs;

        }

        else {
            sentence += sunit.action + " " + sunit.theme + " ";

            foreach (String arg in sunit.args)
            {
                sentence += " " + arg;
            }

            if (sunit.hasReturnType)
            {
                sentence += " and get " + sunit.returnType;
            }

        }

        // Begin with uppercase and end with period.
        sentence = char.ToUpper(sentence[0]) + sentence.Substring(1);
        sentence += ".";


        return sentence;
    }
}
