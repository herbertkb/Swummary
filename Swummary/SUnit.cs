using System.Collections.Generic;

namespace Swummary
{
    public struct SUnit
    {
        SUnitType type;
        string action;
        string theme;
        IEnumerable<string> args;
        bool hasReturnType;
        string returnType;

        // Constructor for a single method call with no return type
        public SUnit(string action, string theme, IEnumerable<string> args)
        {
            type = SUnitType.SingleMethodCall;
            this.action = action;
            this.theme = theme;
            this.args = args;

            hasReturnType = false;
            returnType = null;
        }

        // Constructor for a single method call with a return type
        public SUnit(string action, string theme, IEnumerable<string> args, string returnType)
        {
            type = SUnitType.SingleMethodCall;
            this.action = action;
            this.theme = theme;
            this.args = args;

            hasReturnType = true;
            this.returnType = returnType; 
        } 

    }
}
