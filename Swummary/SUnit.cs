using System.Collections.Generic;

namespace Swummary
{
    public struct SUnit
    {
        public SUnitType type;
        public string action;
        public string theme;

        public bool hasArgs;
        public IEnumerable<string> args;

        public bool hasReturnType;
        public string returnType;

        // Constructor for a SUnit of user-specified SUnitType with a return type
        public SUnit(SUnitType type, string action, string theme, IEnumerable<string> args, string returnType)
        {
            this.type = type;
            this.action = action;
            this.theme = theme;
            this.args = args;

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

            if (returnType != null)
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