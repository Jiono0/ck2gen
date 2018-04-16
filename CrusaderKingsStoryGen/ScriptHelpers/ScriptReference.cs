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
