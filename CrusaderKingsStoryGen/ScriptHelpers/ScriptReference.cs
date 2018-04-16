using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen.ScriptHelpers
{
    public class ScriptReference
    {
        public string Referenced { get; set; }

        public ScriptReference(string r)
        {
            this.Referenced = r;
        }

        public override string ToString()
        {
            return this.Referenced;
        }
    }
}
