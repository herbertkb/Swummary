using System.Collections.Generic;

namespace Swummary
{
    public struct SUnit
    {
        public SUnitType type;
        public string action;
        public string theme;

        //lhs is for variable name,
        //but Damevski recommended we just use variable type for now
        public bool hasLHS;
        public string lhs;

        public bool hasArgs;
        public IEnumerable<string> args;

        public bool hasReturnType;
        public string returnType;

        // Constructor for a SUnit of user-specified SUnitType with a return type
        public SUnit(SUnitType type, string action, string theme, string lhs, IEnumerable<string> args, string returnType)
        {
            this.type = type;
            this.action = action;
            this.theme = theme;
            this.lhs = lhs;
            this.args = args;

            if (lhs != null)
            {
                this.lhs = lhs;
                hasLHS = true;
            }
            else
            {
                this.lhs = lhs;
                hasLHS = false;
            }

            if (args != null)
            {
                this.args = args;
                hasArgs = true;
            }
            else
            {
                this.args = null;
                hasArgs = false;
            }

            if (returnType != null && !returnType.Equals("void"))
            {
                hasReturnType = true;
                this.returnType = returnType;
            }
            else
            {
                hasReturnType = false;
                this.returnType = null;
            }

        }

    }
}