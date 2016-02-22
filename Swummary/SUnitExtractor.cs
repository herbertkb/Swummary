﻿using ABB.SrcML.Data;
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
    /// Extracts same-action s_units from a method.
    /// Same-actions s_units feature a method call which shares a symantic action with their method's name.  
    /// For example, suppose a method named "LoadConfigFile" had a statement calling some "OpenFile()" method. 
    /// </summary>
    /// <param name="methodDef">The SrcML MethodDefinition from which to extract same action s_units.</param>
    /// <returns>An IEnumerable collection containing the same action s_units found in methodDef</returns>
    public static IEnumerable<Statement> GetSameAction( MethodDefinition methodDef ) { return new List<Statement>(); }

    /// <summary>
    /// Extracts void return s_units from a method.
    ///  Void return s_units do not return a value or do not assign any value to a variable.
    /// </summary>
    /// <param name="methodDef">The SrcML MethodDefinition from which to extract void return s_units.</param>
    /// <returns>An IEnumerable collection containing the void return s_units found in methodDef</returns>
    public static IEnumerable<Statement> GetVoidReturn(MethodDefinition methodDef ) {
        var statements = methodDef.GetDescendants<Statement>();
        

        return new List<Statement>();
    }

    /// <summary>
    /// Extracts ending s_units from method. 
    /// Ending s_units are statements that exit from the control of a method. 
    /// These include explicit return statements and/or the last statement in a method. 
    /// </summary>
    /// <param name="methodDef">The SrcML MethodDefinition from which to extract ending s_units.</param>
    /// <returns>An IEnumerable collection containing the ending s_units found in methodDef</returns>
    public static IEnumerable<Statement> GetEnding( MethodDefinition methodDef ) {
        
        var statements = methodDef.GetDescendants<Statement>();
        var lastStatement = statements.ElementAt(statements.Count() - 1);

        return new List<Statement>() { lastStatement };
    }

}
