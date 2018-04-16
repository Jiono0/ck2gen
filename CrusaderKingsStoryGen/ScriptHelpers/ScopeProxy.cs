// <copyright file="ScopeProxy.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.ScriptHelpers
{
    using System;

    class ScopeProxy
    {
        ScriptScope scope;

        public virtual void Init(ScriptScope scope)
        {
            this.scope = scope;
        }

        public ScopeProxy()
        {
        }

        public string GetString(string name)
        {
            return (this.scope.ChildrenMap[name] as ScriptCommand).Value.ToString();
        }

        public bool GetBool(string name)
        {
            return ((bool)(this.scope.ChildrenMap[name] as ScriptCommand).Value);
        }

        public int GetInt(string name)
        {
            return ((int)(this.scope.ChildrenMap[name] as ScriptCommand).Value);
        }

        public void SetValue(string name, object value)
        {
            if (!this.scope.ChildrenMap.ContainsKey(name))
            {
                var c = new ScriptCommand(name, value, this.scope);
                this.scope.Add(c);
            }
            else
            {
                (this.scope.ChildrenMap[name] as ScriptCommand).Value = value;
            }
        }
    }
}
