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

        else if (sunit.type == SUnitType.Assignment)
        {
            sentence += sunit.action + " " + sunit.theme + " ";

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

        return sentence;
    }
}
