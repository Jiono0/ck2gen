using CrusaderKingsStoryGen.Helpers;
using CrusaderKingsStoryGen.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CrusaderKingsStoryGen.ScriptHelpers
{
    public class ScriptLoader
    {
        public static ScriptLoader instance = new ScriptLoader();

        private int lineNum = 0;
        Stack<ScriptScope> scopeStack = new Stack<ScriptScope>();

        public List<string> TokenizeLine(string line)
        {
            if (line.Contains("b_arbil"))
            {
            }

            line = line.Split('#')[0];
            List<string> tokens = new List<string>();
            while (line.Trim().Length > 0)
            {
                if (line.Trim().StartsWith("#"))
                {
                    return tokens;
                }

                line = line.Trim();
                string token = line.GetNextToken(out line);
                line = line.Trim();
                tokens.Add(token);
            }

            return tokens;
        }

        public ScriptScope LoadString(string line, ScriptScope target)
        {
            List<string> tokens = new List<string>();

            Script script = new Script();
            script.Root = target;
            string tokeniseLine = "";
            if (line.Contains("\n"))
            {
                var lines = line.Split('\n');
                this.current = target;
                tokeniseLine = "";
                foreach (var s in lines)
                {
                    if (s.Contains("society_rank = {"))
                    {
                    }

                    string ss = s.Split('#')[0];
                    tokeniseLine += ss.Trim();
                    if (tokeniseLine.Contains("#"))
                    {
                        tokeniseLine = tokeniseLine.Substring(0, tokeniseLine.IndexOf("#"));
                    }

                    if (s.Trim().EndsWith("="))
                    {
                        continue;
                    }

                    var list = this.TokenizeLine(tokeniseLine);
                    tokens.AddRange(list);
                    tokeniseLine = "";
                }
            }
            else
            {
                this.current = script.Root;

                var list = this.TokenizeLine(line);
                tokens.AddRange(list);
            }

            this.DoLinesFromTokens(tokens, script);
            return script.Root;
        }

        public Script Load(string filename)
        {
            if (!File.Exists(filename))
            {
                Script ss = new Script();
                ss.Name = filename;
                ss.Root = new ScriptScope();
                return ss;
            }

            System.IO.StreamReader file =
               new System.IO.StreamReader(filename, Encoding.GetEncoding(1252));
            string line = "";
            Script script = new Script();
            script.Name = filename;
            script.Root = new ScriptScope();
            this.current = script.Root;
            List<string> tokens = new List<string>();
            string tokeniseLine = "";
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("log ="))
                {
                    continue;
                }

                if (line.Trim().StartsWith("#"))
                {
                    continue;
                }

                tokeniseLine += line.Trim();

                if (line.Trim().EndsWith("="))
                {
                    continue;
                }

                var list = this.TokenizeLine(tokeniseLine);
                tokens.AddRange(list);

                tokeniseLine = "";
            }

            this.DoLinesFromTokens(tokens, script);
            file.Close();

            return script;
        }

        private void DoLinesFromTokens(List<string> tokens, Script script)
        {
            string lineForm = "";
            for (int index = 0; index < tokens.Count; index++)
            {
                var token = tokens[index];
                if (index == 40700)
                {
                }

                if (token.Contains("e_wendish_empire"))
                {
                }

                if (token == "c_merv")
                {
                    int gfd = 0;
                }

                var next = "";
                var last = "";

                if (index > 0)
                {
                    last = tokens[index - 1];
                }

                if (index < tokens.Count - 1)
                {
                    next = tokens[index + 1];
                }

                lineForm += token + " ";
                if (token == "color" || token == "color2")
                {
                    if (tokens[index + 2].Trim() == "0")
                    {
                        try
                        {
                            Convert.ToInt32(tokens[index + 3]);
                        }
                        catch (Exception e)
                        {
                            lineForm += tokens[index + 1] + " ";
                            lineForm += tokens[index + 2] + " ";
                            index += 2;
                            this.DoLine(script, lineForm);
                            lineForm = "";
                            continue;
                        }
                    }

                    lineForm += tokens[index + 1] + " ";
                    lineForm += tokens[index + 2] + " ";
                    lineForm += tokens[index + 3] + " ";
                    lineForm += tokens[index + 4] + " ";
                    lineForm += tokens[index + 5] + " ";
                    index += 5;
                    this.DoLine(script, lineForm);
                    lineForm = "";
                    continue;
                }

                if (token == "= {")
                {
                    this.DoLine(script, lineForm);
                    lineForm = "";
                }

                if (token == "}")
                {
                    this.DoLine(script, token);
                    lineForm = "";
                }

                if (last == "=")
                {
                    this.DoLine(script, lineForm);
                    lineForm = "";
                }

                if (last == ">=" || last == "<=" || last == "==")
                {
                    this.DoLine(script, lineForm);
                    lineForm = "";
                }

                if (next == "}")
                {
                    this.DoLine(script, lineForm);
                    lineForm = "";
                }
            }
        }

        public Script LoadKey(string key)
        {
            string filename = ModManager.instance.FileMap[key];
            if (!File.Exists(filename))
            {
                //  var f = File.CreateText(filename);
                //  f.Close();
                Script ss = new Script();
                ss.filenameIsKey = true;
                ss.Name = key;
                ss.Root = new ScriptScope();
                return ss;
            }

            System.IO.StreamReader file =
               new System.IO.StreamReader(filename, Encoding.GetEncoding(1252));
            string line = "";
            Script script = new Script();
            script.filenameIsKey = true;
            script.Name = key;
            script.Root = new ScriptScope();
            this.current = script.Root;
            int l = 0;
            List<string> tokens = new List<string>();
            string tokeniseLine = "";
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("log = "))
                {
                    continue;
                }

                if (line.Trim().Contains("#"))
                {
                    var split = line.Split('#');
                    if (split.Length > 1)
                    {
                        line = split[0].Trim();
                    }
                }

                tokeniseLine += line.Trim();

                if (line.Trim().EndsWith("="))
                {
                    continue;
                }

                var list = this.TokenizeLine(tokeniseLine);
                tokens.AddRange(list);

                tokeniseLine = "";
            }

            this.DoLinesFromTokens(tokens, script);

            file.Close();

            return script;
        }

        private ScriptScope current = null;
        public string deferString = "";

        private void DoLine(Script script, string line)
        {
            if (line.Contains("b_vakash"))
            {
            }

            line = line.Trim();

            if (this.deferString.Length > 0)
            {
                line = this.deferString + line;
                this.deferString = "";
            }

            if (line.Contains("{") && line.Contains("}"))
            {
                int open = line.Count("{");
                int closed = line.Count("}");

                if (closed < open)
                {
                    this.deferString += line.Trim();
                    return;
                }

                string t = line.Trim();
                if (t.StartsWith("#"))
                {
                    return;
                }

                int st = t.IndexOf('{');
                int en = t.LastIndexOf('}');
                string stripbe = t.Substring(0, st + 1);
                string strip = t.Substring(st + 1, (en) - (st + 1));
                string stripend = t.Substring(en);
                if (!stripbe.StartsWith("color"))
                {
                    this.DoLine(script, stripbe);
                    this.DoLine(script, strip);
                    this.DoLine(script, stripend);
                    return;
                }
            }

            if (line.Contains("{"))
            {
                if (line.Split('{').Length > 2 && line.Trim().EndsWith("{"))
                {
                    string t = line.Trim();
                    string[] spl = t.Split('{');
                    foreach (var s in spl)
                    {
                        if (s.Trim().Length > 0)
                        {
                            this.DoLine(script, s + "{");
                        }
                    }

                    return;
                }
            }

            bool skipping = false;

            string origline = line;
            this.lineNum++;
            if (line.Trim().Length == 0)
            {
                return;
            }

            if (line.Trim().StartsWith("#"))
            {
                return;
            }

            if (line.Trim().StartsWith("{"))
            {
                return;
            }

            if (line.Contains("#"))
            {
                line = line.Split('#')[0].Trim();
            }

            if (line.Contains('='))
            {
                string orig = line;
                string op = "=";
                if (line.Contains("=="))
                {
                    line = line.Replace("==", "=");
                    op = "==";
                }

                if (line.Contains(">="))
                {
                    line = line.Replace(">=", "=");
                    op = ">=";
                }

                if (line.Contains("<="))
                {
                    line = line.Replace("<=", "=");
                    op = "<=";
                }

                var sp = line.Split('=');
                if (sp.Length > 2)
                {
                    string newStart = sp[0] + op;
                    string newStart2 = line.Substring(line.IndexOf(op) + op.Length);
                    string newStart3 = null;
                    if (!newStart2.Trim().StartsWith("{"))
                    {
                        newStart += newStart2.Trim().Substring(0, newStart2.Trim().IndexOf(' '));
                        newStart2 = newStart2.Trim().Substring(newStart2.Trim().IndexOf(' '));
                        this.DoLine(script, newStart);
                        this.DoLine(script, newStart2);

                        return;
                    }
                }

                line = orig;
            }

            if (line.Contains("="))
            {
                string op = "=";
                if (line.Contains("=="))
                {
                    line = line.Replace("==", "=");
                    op = "==";
                }

                if (line.Contains(">="))
                {
                    line = line.Replace(">=", "=");
                    op = ">=";
                }

                if (line.Contains("<="))
                {
                    line = line.Replace("<=", "=");
                    op = "<=";
                }

                string[] sp = line.Split('=');
                string name = sp[0].Trim();

                if (!(line.Contains("{") || sp[1].Trim().Length == 0))
                {
                    string value = sp[1].Trim();
                    while (value.EndsWith("}"))
                    {
                        value = value.Substring(0, value.Length - 1).Trim();
                    }

                    if (line.EndsWith("}"))
                    {
                        this.DoLine(script, line.Substring(0, line.Length - 1));
                        this.DoLine(script, "}");
                        return;
                    }

                    if (sp.Length > 2)
                    {
                    }

                    if (value.Contains("["))
                    {
                        if (sp.Count() > 2)
                        {
                        }

                        value = sp[1].Replace("[", "");
                        object val = GetValueFromString(value);
                        this.current.Add(new ScriptCommand() { Name = name, Value = val, AlwaysQuote = true, Op = op });
                    }
                    else
                    {
                        object val = GetValueFromString(value);
                        this.current.Add(new ScriptCommand() { Name = name, Value = val, Op = op });
                    }
                }

                else if (!line.StartsWith("}"))
                {
                    if (line.Contains("{") && line.Contains("}"))
                    {
                        string sname = line.Split('=')[0].Trim();
                        if (sname == "color" || sname == "color2")
                        {
                            object val = GetValueFromString(line.Split('{', '}')[1].Trim());

                            this.current.Add(new ScriptCommand() { Name = name, Value = val });
                            return;
                        }

                        var s = new ScriptScope() { Name = name };
                        s.Parent = this.current;
                        this.scopeStack.Push(this.current);
                        this.current.Add(s);
                        this.current = s;

                        int st = line.IndexOf('{');
                        int en = line.LastIndexOf('}');
                        string strip = line.Substring(st + 1, en - (st + 1));
                        bool bDone = false;

                        if (strip.Contains("{") && strip.Contains("="))
                        {
                            int brace = strip.IndexOf("{");
                            int eq = strip.IndexOf("=");
                            string between = strip.Substring(eq + 1, brace - eq - 1);
                            if (between.Trim().Length > 0)
                            {
                                string[] sp2 = strip.Split(new[] { ' ', '\t' });
                                List<string> lines = new List<string>();

                                foreach (var s1 in sp2)
                                {
                                    if (s1.Trim().Length == 0)
                                    {
                                        continue;
                                    }

                                    bool hasSomethingElse = false;
                                    if (s1.Contains("}"))
                                    {
                                        string s2 = s1.Trim();
                                        string str = "";
                                        for (int index = 0; index < s2.Length; index++)
                                        {
                                            var c = s2[index];
                                            if (c == '}')
                                            {
                                                if (str.Trim().Length > 0)
                                                {
                                                    lines.Add(str.Trim());
                                                }

                                                lines.Add("}");

                                                str = "";
                                            }
                                            else
                                            {
                                                hasSomethingElse = true;
                                                str += c.ToString();
                                            }
                                        }
                                    }
                                }

                                List<string> comp = new List<string>();
                                for (int index = 0; index < lines.Count - 1; index++)
                                {
                                    var line1 = lines[index];
                                    if (lines[index + 1] == "{")
                                    {
                                        string tot = "";
                                        for (int ii = index + 2; ii < lines.Count; ii++)
                                        {
                                            tot += lines[ii];
                                        }

                                        if (tot.Contains("6032"))
                                        {
                                        }

                                        this.DoLine(script, tot.Substring(0, tot.Length));
                                    }

                                    if (line1 == "=")
                                    {
                                        comp.Add(lines[index - 1] + " " + lines[index] + " " + lines[index + 1]);
                                    }
                                }

                                foreach (var l in comp)
                                {
                                    this.DoLine(script, l.Trim());
                                }
                            }
                            else
                            {
                                this.DoLine(script, strip.Trim());
                            }
                        }
                        else
                        {
                            {
                                string[] sp2 = strip.Split(new[] { ' ', '\t' });
                                List<string> lines = new List<string>();

                                foreach (var s1 in sp2)
                                {
                                    if (s1.Trim().Length == 0)
                                    {
                                        continue;
                                    }

                                    lines.Add(s1.Trim());
                                }

                                List<string> comp = new List<string>();

                                for (int index = 0; index < lines.Count - 1; index++)
                                {
                                    var line1 = lines[index];
                                    if (line1 == "=")
                                    {
                                        comp.Add(lines[index - 1] + " " + lines[index] + " " + lines[index + 1]);
                                    }
                                }

                                if (comp.Count == 0 && strip.Trim().Length > 0)
                                {
                                    this.DoLine(script, strip);
                                }

                                foreach (var l in comp)
                                {
                                    this.DoLine(script, l.Trim());
                                }
                            }

                            // DoLine(script, strip.Trim());
                        }


                        this.current = this.scopeStack.Pop();

                        return;
                    }

                    string sname2 = line.Split('=')[0].Trim();
                    if (sname2 == "allow")
                    {
                    }

                    ScriptScope scope = new ScriptScope();
                    scope.Parent = this.current;
                    scope.Name = sname2;

                    this.scopeStack.Push(this.current);
                    this.current.Add(scope);


                    this.current = scope;
                    this.current.Name = name;
                }
            }
            else if (line.Trim().StartsWith("}"))
            {
                if (this.scopeStack.Count == 0)
                {
                    line = line.Trim().Substring(1);
                    this.DoLine(script, line);
                    return;
                }

                this.current = this.scopeStack.Pop();
                line = line.Trim().Substring(1);
                this.DoLine(script, line);
            }
            else
            {
                this.current.Data += origline + "\n";
            }
        }

        public static object GetValueFromString(string value)
        {
            if (value.ToString().Trim() == "\"\"")
            {
                return "\"\"";
            }

            if (value.ToString().Trim().Length == 0)
            {
                return "\"\"";
            }

            value = value.Trim();
            if (value == "yes" || value == "no")
            {
                return value == "yes";
            }

            if (value.Contains("{"))
            {
                value = value.Replace("{", "");
                value = value.Replace("}", "");
                value = value.Trim();
            }

            if (value.Split(' ').Count() >= 3 && !value.StartsWith("\""))
            {
                bool isFloat = value.Contains(".");
                var sp = value.Split(' ');
                try
                {
                    if (isFloat)
                    {
                        return Color.FromArgb(255, (int)(255.0f * Convert.ToSingle(sp[0])), (int)(255.0f * Convert.ToSingle(sp[1])), (int)(255.0f * Convert.ToSingle(sp[2])));
                    }
                    else
                    {
                        return Color.FromArgb(255, Convert.ToInt32(sp[0]), Convert.ToInt32(sp[1]), Convert.ToInt32(sp[2]));
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        int[] rgb = new int[3];
                        int c = 0;

                        foreach (var s in sp)
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

                        return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (value.Contains("\""))
            {
                return value.Replace("\"", "");
            }

            return value;
        }
    }
}
