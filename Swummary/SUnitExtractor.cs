using ABB.SrcML.Data;
using ABB.Swum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Static class which provides methods to parse a SrcML MethodDefinition for s_units.   
/// </summary>
public static class SUnitExtractor
{
    /// <summary>
    /// Extracts all selected s_units from a method: ending, void-return, and same-action.
    /// TODO: add data-facilitating s_unit selection.
    /// </summary>
    /// <param name="methodDef">The SrcML MethodDefinition from which to extract same action s_units.</param>
    /// <returns>An IEnumerable collection containing the same action s_units found in methodDef</returns>
    public static IEnumerable<Statement> ExtractAll(MethodDefinition methodDef) {
        var summarySet = new List<Statement>();

        summarySet.AddRange(SUnitExtractor.ExtractSameAction(methodDef));
        summarySet.AddRange(SUnitExtractor.ExtractVoidReturn(methodDef));
        summarySet.AddRange(SUnitExtractor.ExtractEnding(methodDef));

        return summarySet.Distinct<Statement>().OrderBy(s => s.PrimaryLocation.StartingLineNumber);
    }


    /// <summary>
    /// Extracts same-action s_units from a method.
    /// Same-actions s_units feature a method call which shares a symantic action with their method's name.  
    /// For example, suppose a method named "LoadConfigFile" had a statement calling some "OpenFile()" method. 
    /// </summary>
    /// <param name="methodDef">The SrcML MethodDefinition from which to extract same action s_units.</param>
    /// <returns>An IEnumerable collection containing the same action s_units found in methodDef</returns>
    public static IEnumerable<Statement> ExtractSameAction( MethodDefinition methodDef ) {

        var sameActions = new List<Statement>();

        // Break each program element down into words, add them to a bag, and collect if any words repeat.

        // Build Method name word list.



        // Build and compare statement word list for each statement. 

        return sameActions;
    }

    /// <summary>
    /// Extracts void return s_units from a method.
    ///  Void return s_units do not return a value or do not assign any value to a variable.
    /// </summary>
    /// <param name="methodDef">The SrcML MethodDefinition from which to extract void return s_units.</param>
    /// <returns>An IEnumerable collection containing the void return s_units found in methodDef</returns>
    public static IEnumerable<Statement> ExtractVoidReturn(MethodDefinition methodDef ) {
        var statements = methodDef.GetDescendants<Statement>();

        var voidReturns = new List<Statement>();

        foreach(var s in statements)
        {
            var assignments = s.GetDescendants<OperatorUse>().Where(o => o.Text.Equals("="));

            if (! assignments.Any()) {
                voidReturns.Add(s);
            }
            
        }

        return voidReturns;
    }

    /// <summary>
    /// Extracts ending s_units from method. 
    /// Ending s_units are statements that exit from the control of a method. 
    /// These include explicit return statements and/or the last statement in a method. 
    /// </summary>
    /// <param name="methodDef">The SrcML MethodDefinition from which to extract ending s_units.</param>
    /// <returns>An IEnumerable collection containing the ending s_units found in methodDef</returns>
    public static IEnumerable<Statement> ExtractEnding( MethodDefinition methodDef ) {
        
        var statements = methodDef.GetDescendants<Statement>();
        if (statements.Count() == 0) { return new List<Statement>().AsEnumerable(); }

        var returns = statements.Where(s => s is ReturnStatement).ToList();
        var last = statements.Last();

        var endings = returns;
        if(! returns.Contains(last))
        {
            endings.Add(last);
        }

        return endings;
    }

}
