// <copyright file="Parser.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Xml;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class Parser
    {
        public string Name
        {
            get
            {
                if (this._name == null)
                {
                    this._name = this.Scope.Name;
                }

                return this._name;
            }

            set { this._name = value; }
        }

        public Parser(ScriptScope scope)
        {
            this.Scope = scope;
            int line = 0;
            foreach (var child in scope.Children)
            {
                if (child is ScriptCommand)
                {
                    string name = (child as ScriptCommand).Name;
                    this.RegisterProperty(line, name, child);
                }

                line++;
            }
        }

        public ScriptScope Scope { get; set; }

        public Dictionary<string, int> PropertyMap = new Dictionary<string, int>();
        private string _name;

        public void SavePropertyXml(string var, XmlWriter writer)
        {
            var p = this.GetType().GetProperty(var);
            writer.WriteElementString(p.Name, p.GetValue(this).ToString());
        }

        public void SaveFieldXml(string var, XmlWriter writer)
        {
            var p = this.GetType().GetField(var);
            writer.WriteElementString(p.Name, p.GetValue(this).ToString());
        }

        public void RegisterProperty(int line, string property, object child)
        {
            ScriptCommand command = child as ScriptCommand;
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                if (propertyInfo.Name == command.Name)
                {
                    if (command.Value.GetType() == propertyInfo.PropertyType)
                    {
                        propertyInfo.SetValue(this, command.Value);
                    }
                    else
                    {
                        if (command.Value is ScriptReference)
                        {
                            var scriptReference = command.Value as ScriptReference;
                            if (propertyInfo.PropertyType == typeof(int))
                            {
                                propertyInfo.SetValue(this, Convert.ToInt32(scriptReference.Referenced));
                            }
                            else if (propertyInfo.PropertyType == typeof(float))
                            {
                                propertyInfo.SetValue(this, Convert.ToSingle(scriptReference.Referenced));
                            }
                            else if (propertyInfo.PropertyType == typeof(Color))
                            {
                                int r = 0;
                                int g = 0;
                                int b = 0;

                                string[] str = scriptReference.Referenced.Split(' ');
                                int[] rgb = new int[3];
                                int c = 0;

                                foreach (var s in str)
                                {
                                    if (s.Trim().Length > 0)
                                    {
                                        rgb[c] = Convert.ToInt32(s.Trim());
                                        c++;
                                        if (c >= 3)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (c == 0)
                                    propertyInfo.SetValue(this, Color.Black);
                                else if (c < 3)
                                {
                                    for (int n = c; n < 3; n++)
                                    {
                                        rgb[n] = rgb[n - 1];
                                    }
                                }

                                for (int n = 0; n < 3; n++)
                                {
                                    if (rgb[n] > 255)
                                    {
                                        rgb[n] = 255;
                                    }

                                    if (rgb[n] < 0)
                                    {
                                        rgb[n] = 0;
                                    }
                                }

                                propertyInfo.SetValue(this, Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
                            }
                            else propertyInfo.SetValue(this, scriptReference.Referenced);
                        }
                    }
                }
            }

            this.PropertyMap[property] = line;
        }


        public ScriptCommand GetProperty(string property)
        {
            if (!this.PropertyMap.ContainsKey(property))
            {
                return null;
            }

            return this.Scope.Children[this.PropertyMap[property]] as ScriptCommand;
        }

        public void DeleteProperty(string property)
        {
            if (!this.PropertyMap.ContainsKey(property))
            {
                return;
            }

            this.Scope.RemoveAt(this.PropertyMap[property]);
        }

        public void SetProperty(string property, object value)
        {
            if (!this.PropertyMap.ContainsKey(property))
            {
                this.Scope.Delete(property);
                this.Scope.Add(new ScriptCommand() { Name = property, Value = value });
                return;
            }

            this.Scope.Children[this.PropertyMap[property]] = new ScriptCommand() {Name = property, Value = value};
            this.Scope.ChildrenMap[property] = value;
        }
    }
}