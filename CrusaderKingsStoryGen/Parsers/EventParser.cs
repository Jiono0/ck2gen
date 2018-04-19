// <copyright file="EventParser.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Parsers
{
    using System.Collections.Generic;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    class EventParser : Parser
    {
        public List<EventParser> LinkedEvents = new List<EventParser>();
        public List<string> LinkedEventIDs = new List<string>();

        public EventParser(ScriptScope scope)
            : base(scope)
        {
            this.Trigger = scope.Find("trigger") as ScriptScope;
            this.MeanTime = scope.Find("mean_time_to_happen") as ScriptScope;
        }

        public void FindLinks()
        {
            this.ExamineEvent(this.Scope);
        }

        private void ExamineEvent(ScriptScope node)
        {
            for (int index = 0; index < node.Children.Count; index++)
            {
                var child = node.Children[index];
                if (child is ScriptScope)
                {
                    this.ExamineEvent(child as ScriptScope);
                }

                if (child is ScriptCommand)
                {
                    ScriptCommand c = child as ScriptCommand;
                    if (c.Name == "id" && node.Name == "character_event")
                    {
                        if (!this.LinkedEventIDs.Contains(c.Value.ToString()))
                        {
                            var e = (EventManager.instance.GetEvent(c.Value.ToString()));
                            if (e != null)
                            {
                                this.LinkedEvents.Add(e);
                                this.LinkedEventIDs.Add(c.Value.ToString());
                            }
                        }
                    }
                }
            }
        }

        public ScriptScope Trigger { get; set; }

        public ScriptScope MeanTime { get; set; }
    }
}
