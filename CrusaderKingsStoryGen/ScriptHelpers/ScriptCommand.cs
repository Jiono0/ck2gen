using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen.ScriptHelpers
{
    public class ScriptCommand
    {
        public ScriptScope Parent;

        public string Name { get; set; }

        public object Value { get; set; }

        public object Replaced { get; set; }

        public bool AlwaysQuote { get; set; }

        public string Op { get; set; }

        public ScriptCommand()
        {
        }

        public ScriptCommand(string name, object val, ScriptScope parent)
        {
            this.Parent = parent;
            this.Name = name;
            this.Value = val;
            if (val == null || val.ToString().Length == 0)
            {
                this.Value = "\"\"";
            }
        }

        public override string ToString()
        {
            return this.Name + " : " + this.Value;
        }

        public ScriptCommand Copy()
        {
            var s = new ScriptCommand(this.Name, this.Value, null);
            s.AlwaysQuote = this.AlwaysQuote;
            return s;
        }

        public int GetInt()
        {
            var v = this.Value as ScriptReference;


            if (v != null)
            {
                return Convert.ToInt32(v.Referenced);
            }

            if (this.Value.ToString() == "-")
            {
                return 0;
            }

            if (this.Value is string)
            {
                return Convert.ToInt32(this.Value);
            }

            return (int)this.Value;
        }
    }
}
