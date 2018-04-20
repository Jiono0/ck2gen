namespace CrusaderKingsStoryGen.ScriptHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Xml;
    using CrusaderKingsStoryGen.Helpers;

    public class ScriptScope
    {
        public bool FromVanilla { get; set; } = false;

        public ScriptScope Parent { get; set; }

        public List<object> Children = new List<object>();
        public HashSet<object> ChildrenHas = new HashSet<object>();
        public List<ScriptScope> Scopes = new List<ScriptScope>();

        public string Name { get; set; }

        public object Tag1;

        public string Data
        {
            get { return this._data; }
            set { this._data = "\t\t\t" + value.Trim(); }
        }

        public delegate void CopyDelegate(object o);

        public void FillFrom(ScriptScope from, CopyDelegate copy)
        {
            this.Name = from.Name;
            this.Parent = this.Parent;
            foreach (var child in from.Children)
            {
                if (child is ScriptScope)
                {
                    ScriptScope sc = new ScriptScope((child as ScriptScope).Name);

                    sc.FillFrom(child as ScriptScope, copy);
                    copy.Invoke(child);
                    this.Add(sc);
                }
                else
                {
                    ScriptCommand c = new ScriptCommand();
                    ScriptCommand src = child as ScriptCommand;

                    c.Name = src.Name;
                    c.AlwaysQuote = src.AlwaysQuote;
                    c.Op = src.Op;
                    c.Parent = this;
                    copy.Invoke(c);
                    this.Add(c);
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Save(StreamWriter file, int depth)
        {
            string tab = "";
            for (int n = 1; n < depth; n++)
            {
                tab += "\t";
            }

            string ret = "\n";

            if (depth > 0)
            {
                file.Write(tab + this.Name + " = {" + ret);
            }

            string rett = "\t";
            tab = "";
            for (int n = 1; n < depth; n++)
            {
                tab += rett;
            }

            foreach (var child in this.Children)
            {
                if (child is ScriptCommand)
                {
                    ScriptCommand c = child as ScriptCommand;
                    if (c.AlwaysQuote)
                    {
                        string str = this.GetExportStringFromObject(c.Value);
                        if (!str.Trim().StartsWith("\""))
                        {
                            str = '"'.ToString() + str + '"'.ToString();
                        }

                        if (!string.IsNullOrEmpty(c.Op))
                        {
                            file.Write(tab + rett + c.Name + " " + c.Op + " " + str + ret);
                        }
                        else file.Write(tab + rett + c.Name + " = " + str + ret);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(c.Op))
                        {
                            file.Write(tab + rett + c.Name + " " + c.Op + " " + this.GetExportStringFromObject(c.Value) + ret);
                        }
                        else
                            file.Write(tab + rett + c.Name + " = " + this.GetExportStringFromObject(c.Value) + ret);
                    }
                }

                if (child is ScriptScope)
                {
                    (child as ScriptScope).Save(file, depth + 1);
                }

                if (child is string)
                {
                    file.WriteLine(child); //child as ScriptScope);.Save(file, depth + 1);
                }
            }

            if (this.Data.Trim().Length > 0)
            {
                file.Write(this.Data);
            }

            if (depth > 0)
            {
                if (depth == 1)
                {
                    file.Write(tab + "}\n\n");
                }
                else
                {
                    file.Write(tab + "}\n");
                }
            }
        }

        private string GetExportStringFromObject(object value)
        {
            if (value == null)
            {
                return "";
            }

            if (value is bool)
            {
                if (((bool)value) == true)
                {
                    return "yes";
                }
                else
                {
                    return "no";
                }
            }

            if (value is Color)
            {
                try
                {
                    Color col = (Color)value;
                    return "{ " + col.R + " " + col.G + " " + col.B + " }";
                }
                catch (Exception ex)
                {
                    System.Console.Out.WriteLine(ex.StackTrace);
                }
            }

            if (value is ScriptReference)
            {
                return ((ScriptReference)value).Referenced;
            }

            if (value is int || value is float)
            {
                return value.ToString();
            }

            if (value is List<string>)
            {
                return (value as List<string>).ToSpaceDelimited();
            }

            if (value.ToString().Contains(" "))
            {
                return "\"" + value.ToString() + "\"";
            }
            else
            {
                return value.ToString();
            }
        }

        public bool AllowDuplicates { get; set; }

        public string NameSearchReplaced { get; set; }

        public Dictionary<string, object> UnsavedData = new Dictionary<string, object>();

        public void SetChild(ScriptScope scope)
        {
            if (scope == null)
            {
                return;
            }

            if (!this.ChildrenHas.Contains(scope))
            {
                if (scope.Parent != null)
                {
                    scope.Parent.Remove(scope);
                }

                this.Add(scope);
                scope.Parent = this;
            }
        }

        public void SetChildPre(ScriptScope scope)
        {
            if (!this.Children.Contains(scope))
            {
                if (scope.Parent != null)
                {
                    scope.Parent.Remove(scope);
                }

                this.Insert(0, scope);
                scope.Parent = this;
            }
        }

        public bool HasNamed(string s)
        {
            foreach (var child in this.Children)
            {
                if (child is ScriptScope)
                    if (((ScriptScope)child).Name == s)
                    {
                        return true;
                    }

                if (child is ScriptCommand)
                    if (((ScriptCommand)child).Name == s)
                    {
                        return true;
                    }
            }

            return false;
        }

        public void Delete(string s)
        {
            var a = this.Children.ToArray();
            foreach (var child in a)
            {
                if (child is ScriptScope)
                    if (((ScriptScope)child).Name == s)
                    {
                        this.Remove(child);
                    }

                if (child is ScriptCommand)
                    if (((ScriptCommand)child).Name == s)
                    {
                        this.Remove(child);
                    }
            }
        }

        public void Add(string property, object value)
        {
            this.Add(new ScriptCommand() { Name = property, Value = value });
        }

        public ScriptScope AddScope(string name)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            scope.Parent = this;
            this.Add(scope);
            return scope;
        }

        public ScriptScope AddScope()
        {
            ScriptScope scope = new ScriptScope();
            scope.Parent = this;
            this.Add(scope);
            return scope;
        }

        public ScriptScope()
        {
            this.AllowDuplicates = true;
        }

        public ScriptScope(string name)
        {
            this.AllowDuplicates = true;
            this.Name = name;
        }

        public void Remove(object scriptScope)
        {
            this.ChildrenHas.Remove(scriptScope);
            this.Children.Remove(scriptScope);
            if (scriptScope is ScriptScope)
            {
                this.ChildrenMap.Remove(((ScriptScope)scriptScope).Name);
                this.Scopes.Remove(((ScriptScope)scriptScope));
                ((ScriptScope)scriptScope).Parent = null;
            }

            if (scriptScope is ScriptCommand)
            {
                this.ChildrenMap.Remove(((ScriptCommand)scriptScope).Name);
                ((ScriptCommand)scriptScope).Parent = null;
            }
        }

        public void Insert(int index, ScriptScope scriptScope)
        {
            if (!this.AllowDuplicates && this.ChildrenMap.ContainsKey(scriptScope.Name))
            {
                return;
            }

            this.ChildrenHas.Add(scriptScope);
            this.Children.Insert(index, scriptScope);
            if (scriptScope.Name == null)
            {
                scriptScope.Name = this.ChildrenMap.Count.ToString();
            }

            this.ChildrenMap[scriptScope.Name] = scriptScope;
            this.Scopes.Add(scriptScope);
            scriptScope.Parent = this;
        }

        public void Add(ScriptScope scriptScope)
        {
            if (!this.AllowDuplicates && this.ChildrenMap.ContainsKey(scriptScope.Name))
            {
                return;
            }

            this.ChildrenHas.Add(scriptScope);
            this.Children.Add(scriptScope);
            if (scriptScope.Name == null)
            {
                scriptScope.Name = this.ChildrenMap.Count.ToString();
            }

            this.ChildrenMap[scriptScope.Name] = scriptScope;
            this.Scopes.Add(scriptScope);
            scriptScope.Parent = this;
        }

        public void Add(ScriptCommand scriptScope)
        {
            if (!this.AllowDuplicates && this.ChildrenMap.ContainsKey(scriptScope.Name))
            {
                return;
            }

            this.ChildrenHas.Add(scriptScope);
            this.Children.Add(scriptScope);
            this.ChildrenMap[scriptScope.Name] = scriptScope;
            scriptScope.Parent = this;
        }

        public void Add(string str)
        {
            this.Children.Add(str);
            this.ChildrenMap[str] = str;
        }

        internal Dictionary<string, object> ChildrenMap = new Dictionary<string, object>();
        private string _data = "";

        public void Clear()
        {
            this.ChildrenHas.Clear();
            this.Children.Clear();
            this.ChildrenMap.Clear();
            this.Scopes.Clear();
        }

        public void RemoveAt(int property)
        {
            this.Remove(this.Children[property]);
        }

        public void Do(string line)
        {
            ScriptLoader.instance.LoadString(line, this);
        }

        public void Strip(string[] strings)
        {
            foreach (var s in strings)
            {
                if (this.ChildrenMap.ContainsKey(s))
                {
                    for (int index = 0; index < this.Children.Count; index++)
                    {
                        var child = this.Children[index];
                        if (child is ScriptScope)
                        {
                            if (((ScriptScope)child).Name == s)
                            {
                                this.Remove(child);
                            }
                        }

                        if (child is ScriptCommand)
                        {
                            if (((ScriptCommand)child).Name == s)
                            {
                                this.Remove(child);
                            }
                        }
                    }
                }
            }
        }


        public object Find(string name)
        {
            foreach (var child in this.Children)
            {
                if (child is ScriptCommand)
                    if (((ScriptCommand)child).Name == name)
                    {
                        return child;
                    }

                if (child is ScriptScope)
                    if (((ScriptScope)child).Name == name)
                    {
                        return child;
                    }
            }

            return null;
        }

        public void Command(string path)
        {
            string[] split = path.Trim().Split(' ');

            if (split[0] == "delete")
            {
                var o = this.PathTo(split[1]);
                this.Delete(o);
            }

            if (split[0] == "copy")
            {
                var o = this.PathTo(split[1]);
                var o2 = this.PathTo(split[2]);

                //      if()
            }

            if (split[0] == "overwrite")
            {
                var o = this.PathTo(split[1]);
                var o2 = this.PathTo(split[2]);

                if (o2 is ScriptScope)
                {
                    var dest = ((ScriptScope)o2);

                    dest.Clear();

                    if (o is ScriptScope)
                    {
                        var src = ((ScriptScope)o);

                        foreach (var child in src.Children)
                        {
                            dest.AddCopy(child);
                        }
                    }
                }

                //      if()
            }
        }

        public void AddCopy(object child)
        {
            if (child is ScriptScope)
            {
                var newScope = ((ScriptScope)child).Copy();
                this.Add(newScope);
            }
            else
            {
                var newparam = ((ScriptCommand)child).Copy();
                this.Add(newparam);
            }
        }

        private ScriptScope Copy()
        {
            ScriptScope newS = new ScriptScope();
            newS.Name = this.Name;
            foreach (var child in this.Children)
            {
                if (child is ScriptScope)
                {
                    newS.Add(((ScriptScope)child).Copy());
                }

                if (child is ScriptCommand)
                {
                    var c = ((ScriptCommand)child).Copy();

                    newS.Add(c);
                }
            }

            return newS;
        }

        private void Delete(object o)
        {
            if (o is ScriptScope)
            {
                ((ScriptScope)o).Parent.Remove(o);
            }

            if (o is ScriptCommand)
            {
                ((ScriptCommand)o).Parent.Remove(o);
            }
        }

        public object PathTo(string path)
        {
            string[] split = path.Trim().Split('.');
            int index = 0;
            if (split[0].Contains("["))
            {
                index = Convert.ToInt32(split[0].Split(new[] { '[', ']' })[1]);
                split[0] = split[0].Split(new[] { '[', ']' })[0];
            }

            if (this.ChildrenMap.ContainsKey(split[0]))
            {
                int found = 0;
                foreach (var child in this.Children)
                {
                    string name = this.GetName(child);
                    if (name == split[0])
                    {
                        if (found == index)
                        {
                            if (child is ScriptScope)
                            {
                                string s = "";
                                for (int x = 1; x < split.Length; x++)
                                {
                                    s += split[x] + ".";
                                }

                                if (s.Trim().Length == 0)
                                {
                                    return child;
                                }

                                s = s.Substring(0, s.Length - 1);

                                return ((ScriptScope)child).PathTo(s);
                            }
                            else
                            {
                                string s = "";
                                for (int x = 1; x < split.Length; x++)
                                {
                                    s += split[x] + ".";
                                }

                                s = s.Substring(0, s.Length - 1);

                                return ((ScriptCommand)child);
                            }
                        }

                        found++;
                    }
                }
            }

            return null;
        }

        private string GetName(object child)
        {
            if (child is ScriptScope)
            {
                return ((ScriptScope)child).Name;
            }

            if (child is ScriptCommand)
            {
                return ((ScriptCommand)child).Name;
            }

            return "";
        }

        public void WriteHierarchy(XmlWriter writer)
        {
            foreach (var rootChild in this.Children)
            {
                var scope = rootChild as ScriptScope;

                if (scope != null)
                {
                    writer.WriteStartElement(scope.Name);

                    scope.WriteHierarchy(writer);

                    writer.WriteEndElement();
                }
            }
        }

        public void SaveXml(XmlWriter writer)
        {
            writer.WriteStartElement("script");
            writer.WriteElementString("name", this.Name);
            foreach (var rootChild in this.Children)
            {
                var scope = rootChild as ScriptScope;

                if (scope != null)
                {
                    writer.WriteStartElement(scope.Name);

                    scope.SaveXml(writer);

                    writer.WriteEndElement();
                }

                var command = rootChild as ScriptCommand;

                if (command != null)
                {
                    writer.WriteElementString(command.Name, command.Value.ToString());
                }
            }

            writer.WriteEndElement();
        }

        public bool TestBoolean(string name)
        {
            if (this.HasNamed(name))
            {
                foreach (var child in this.Children)
                {
                    if (child is ScriptCommand)
                        if (((ScriptCommand)child).Name == name)
                        {
                            return (bool)(((ScriptCommand)child).Value);
                        }
                }
            }

            return false;
        }

        public string GetString(string name)
        {
            if (this.HasNamed(name))
            {
                foreach (var child in this.Children)
                {
                    if (child is ScriptCommand)
                        if (((ScriptCommand)child).Name == name)
                        {
                            return (((ScriptCommand)child).Value).ToString();
                        }
                }
            }

            return "";
        }

        public object GetValue(string name)
        {
            if (this.HasNamed(name))
            {
                foreach (var child in this.Children)
                {
                    if (child is ScriptCommand)
                        if (((ScriptCommand)child).Name == name)
                        {
                            return (((ScriptCommand)child).Value);
                        }
                }
            }

            return "";
        }

        public int GetInt(string name)
        {
            if (this.HasNamed(name))
            {
                foreach (var child in this.Children)
                {
                    if (child is ScriptCommand)
                        if (((ScriptCommand)child).Name == name)
                        {
                            if (((ScriptCommand)child).Value is ScriptReference)
                            {
                                return Convert.ToInt32((((ScriptCommand)child).Value as ScriptReference).Referenced);
                            }
                            else
                            {
                                return Convert.ToInt32(((ScriptCommand)child).Value);
                            }
                        }
                }
            }


            return 0;
        }

        public Color GetColor(string name)
        {
            if (this.HasNamed(name))
            {
                foreach (var child in this.Children)
                {
                    if (child is ScriptCommand)
                        if (((ScriptCommand)child).Name == name)
                        {
                            {
                                return (Color)(((ScriptCommand)child).Value);
                            }
                        }
                }
            }


            return Color.Black;
        }
    }
}
