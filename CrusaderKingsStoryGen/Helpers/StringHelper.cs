// <copyright file="StringExtensions.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using System;
    using System.Collections.Generic;
    using CrusaderKingsStoryGen.Managers;

    public static class StringHelper
    {
        public static bool IsDate(this string text)
        {
            var S = text.Split('.');
            if (S.Length == 3)
            {
                try
                {
                    foreach (var s in S)
                    {
                        Convert.ToInt32(s.Trim());
                    }
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static int YearFromDate(this string text)
        {
            var S = text.Split('.');


            return Convert.ToInt32(S[0].Trim());
        }

        public static string ToSpaceDelimited(this List<string> list)
        {
            string ret = "";
            foreach (var VARIABLE in list)
            {
                ret += VARIABLE + " ";
            }

            return ret.Trim();
        }

        public static string Lang(this string str)
        {
            if (str == null)
            {
                return "";
            }

            return LanguageManager.instance.Get(str);
        }

        public static void LangSet(this string str, string val)
        {
            LanguageManager.instance.Add(str, val);
        }

        public static string AddSafe(this string str)
        {
            return LanguageManager.instance.AddSafe(str);
        }

        public static int Count(this string str, string toCount)
        {
            int c = 0;
            int lIndex = 0;
            int nIndex = 0;
            do
            {
                lIndex = nIndex+1;
                nIndex = str.IndexOf(toCount, lIndex);
                if (nIndex != -1)
                {
                    c++;
                }
            } while (nIndex != -1);

            return c;
        }

        public static string ReplaceMinusEscape(this string str, string from, string to)
        {
            int fromIdx = 0;

            do
            {
                fromIdx = str.IndexOf(from, fromIdx+1);
                if (fromIdx == -1)
                {
                    break;
                }

                int fromCloseIdx = str.IndexOf("]", fromIdx);
                int fromOpenIdx = str.IndexOf("[", fromIdx);
                if (fromIdx != -1)
                {
                    int f = fromIdx;
                    int t = fromIdx + from.Length;

                    if (fromCloseIdx != -1)
                    {
                        if (fromOpenIdx > fromCloseIdx || fromOpenIdx == -1)
                        {
                            continue;
                        }
                    }

                    string start = str.Substring(0, fromIdx);
                    string end = str.Substring(t);

                    str = start + to + end;
                }
            } while (fromIdx != -1);

            return str;
        }

        public static string GetNextToken(this string str, out string remain)
        {
            str = str.Trim();
            remain = str;
            if (str == "= {}")
            {
                remain = "}";
                return "= {";
            }

            if (str == "={}")
            {
                remain = "}";
                return "= {";
            }

            if (str == "= {")
            {
                remain = "";
                return str;
            }

            if (str.StartsWith("= {"))
            {
                remain = str.Substring(3);
                return "= {";
            }

            if (str == "={")
            {
                remain = "";
                return "= {";
            }

            if (str.StartsWith("={"))
            {
                remain = str.Substring(2);
                return "= {";
            }

            if (str.StartsWith("=="))
            {
                remain = str.Substring(2);
                return "==";
            }

            if (str.StartsWith(">="))
            {
                remain = str.Substring(2);
                return ">=";
            }

            if (str.StartsWith("<="))
            {
                remain = str.Substring(2);
                return "<=";
            }

            if (str.StartsWith(">"))
            {
                remain = str.Substring(2);
                return ">";
            }

            if (str.StartsWith("<"))
            {
                remain = str.Substring(2);
                return "<";
            }


            for (int x = 0; x < str.Length; x++)
            {
                var c = str[x];

                if ((c == '"'))
                {
                    int end = str.IndexOf('"', x + 1);

                    if (end != -1)
                    {
                        string token = str.Substring(x, end - x + 1);
                        remain = str.Substring(end+1).Trim();
                        return token.Trim();
                    }
                }

                if ((c == ' ' || c == '=') && x > 0)
                {
                    string token = str.Substring(0, x);
                    remain = str.Substring(x).Trim();

                    return token.Trim();
                }
                else if (c == '=' && x==0)
                {
                    string token = "=";
                    remain = str.Substring(1).Trim();
                    return token.Trim();
                }
                else if (c == '}')
                {
                    if (x == 0)
                    {
                        string token2 = "}";
                        remain = str.Substring(1).Trim();
                        return token2.Trim();
                    }

                    string token = str.Substring(0, x);
                    remain = str.Substring(x).Trim();
                    return token.Trim();
                }
            }

            remain = "";
            return str.Trim();
        }
    }
}
