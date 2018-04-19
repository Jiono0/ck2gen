// <copyright file="CultureManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Parsers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class CultureManager
    {
        public static CultureManager instance = new CultureManager();

        private Script script;
        public List<CultureParser> AllCultures = new List<CultureParser>();
        public Dictionary<string, CultureParser> CultureMap = new Dictionary<string, CultureParser>();
        public Dictionary<string, CultureGroupParser> CultureGroupMap = new Dictionary<string, CultureGroupParser>();
        public List<CultureGroupParser> AllCultureGroups = new List<CultureGroupParser>();

        public Script Script
        {
            get { return this.script; }
            set { this.script = value; }
        }

        public Dictionary<string, CultureGroupParser> GroupMap = new Dictionary<string, CultureGroupParser>();
        private ProvinceParser distanceTest;

        public CultureGroupParser AddCultureGroup(string name, CultureGroupParser group = null)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            this.script.Root.Add(scope);

            CultureGroupParser r = new CultureGroupParser(scope);
            r.Name = scope.Name;
            r.Init();
            if (group != null)
            {
                r.chosenGfx = this.GetRelatedCultureGfx(group);
            }

            this.GroupMap[name] = r;
            this.AllCultureGroups.Add(r);
            this.AllCultureGroups = this.AllCultureGroups.Distinct().ToList();
            r.color = Color.FromArgb(255, RandomIntHelper.Next(255), RandomIntHelper.Next(255), RandomIntHelper.Next(255));
            r.chosenGfx = scope.Scopes[0].Data;
            return r;
        }

        private string GetRelatedCultureGfx(CultureGroupParser group)
        {
            return CultureParser.GetRandomCultureGraphics(group);
        }

        public void Save()
        {
            if (this.SaveCultures)
            {
                this.script.Save();
            }
        }

        public void Init()
        {
            Script s = new Script();
            this.script = s;
            s.Name = Globals.ModDir + "common\\cultures\\00_cultures.txt";
            s.Root = new ScriptScope();
            CultureGroupParser r = this.AddCultureGroup("barbarian");

            this.AllCultureGroups.Add(r);
            this.CultureGroupMap["barbarian"] = r;
            var cul = r.AddCulture("norse");
            r.Name = "barbarian";
            cul.dna = CulturalDnaManager.instance.GetNewFromVanillaCulture();
            cul.dna.horde = false;

            cul.DoDetailsForCulture();
            LanguageManager.instance.Add("barbarian", cul.dna.GetPlaceName());

            cul.Name = cul.Scope.Name;
            this.CultureMap[cul.Scope.Name] = cul;

            this.AllCultures.Add(cul);
        }

        public CultureParser BranchCulture(string Culture)
        {
            var rel = this.CultureMap[Culture];
            var group = rel.Group;

            var naa = rel.dna.GetPlaceName();
            while (this.GroupMap.ContainsKey(StarHelper.SafeName(naa)))
            {
                naa = rel.dna.GetPlaceName();
            }

            CultureParser rel2 = null;

            if (!this.allowMultiCultureGroups)
            {
                var na = rel.dna.GetPlaceName();
                while (this.GroupMap.ContainsKey(StarHelper.SafeName(na)))
                {
                    na = rel.dna.GetPlaceName();
                }

                LanguageManager.instance.Add(StarHelper.SafeName(na), na);

                var group2 = this.AddCultureGroup(StarHelper.SafeName(na), group);
                group2.Name = StarHelper.SafeName(na);
                rel2 = group2.AddCulture(naa);
                group2.AddCulture(rel2);

                rel2.Init();
                rel2.dna = rel.dna.Mutate(16, rel);
                rel2.dna.DoRandom();
                this.CultureGroupMap[group2.Name] = group2;
            }
            else
            {
                rel2 = group.AddCulture(naa);
                rel2.Init();
                rel2.dna = rel.dna.MutateSmall(4);
            }

            rel2.DoDetailsForCulture();

            return rel2;
        }

        public void CalculateCulturesProper()
        {
            foreach (var cultureGroupParser in this.AllCultureGroups)
            {
                if (cultureGroupParser.Name == "norse")
                {
                    continue;
                }

                if (cultureGroupParser.Provinces.Count == 0)
                {
                    continue;
                }

                var province = cultureGroupParser.Provinces[RandomIntHelper.Next(cultureGroupParser.Provinces.Count)];

                List<ProvinceParser> target = new List<ProvinceParser>();
                target.Add(province);
                target.AddRange(province.Adjacent.Where(p => p.land == true && p.ProvinceTitle != null));

                for (int x = 0; x < 8; x++)
                {
                    var toAdd = new List<ProvinceParser>();
                    target.ForEach(p => toAdd.AddRange(p.Adjacent.Where(pp => pp.land && pp.ProvinceTitle != null && !target.Contains(pp))));
                    target.AddRange(toAdd);
                }

                HashSet<ProvinceParser> toDo = new HashSet<ProvinceParser>(target);
                foreach (var provinceParser in toDo)
                {
                    provinceParser.Culture = cultureGroupParser.Cultures[0];
                    if (provinceParser.Culture.Group.ReligionGroup != null)
                    {
                        provinceParser.Religion = provinceParser.Culture.Group.ReligionGroup.Religions[0];
                    }
                    else
                    {
                        provinceParser.Religion = ReligionManager.instance.AllReligions[0];
                    }
                }
            }

            for (int index = 0; index < this.AllCultureGroups.Count; index++)
            {
                var cultureGroupParser = this.AllCultureGroups[index];

                if (cultureGroupParser.Provinces.Count < 20)
                {
                    bool possible = true;
                    while (cultureGroupParser.Provinces.Count > 0 && possible)
                    {
                        for (int i = 0; i < cultureGroupParser.Provinces.Count; i++)
                        {
                            var provinceParser = cultureGroupParser.Provinces[i];
                            var difcul =
                                provinceParser.Adjacent.Where(
                                    p => p.Culture != provinceParser.Culture && p.Culture != null);
                            if (!difcul.Any())
                            {
                                if (i == cultureGroupParser.Provinces.Count - 1)
                                {
                                    possible = false;
                                }

                                continue;
                            }

                            var list = new List<ProvinceParser>(difcul);
                            provinceParser.Culture = list[RandomIntHelper.Next(list.Count)].Culture;
                            provinceParser.Religion = list[RandomIntHelper.Next(list.Count)].Religion;
                            break;
                        }
                    }
                }

                if (cultureGroupParser.Provinces.Count == 0)
                {
                    this.AllCultureGroups.Remove(cultureGroupParser);
                    this.Script.Root.Remove(cultureGroupParser.Scope);
                    this.CultureMap.Remove(cultureGroupParser.Cultures[0].Name);
                    this.AllCultures.Remove(cultureGroupParser.Cultures[0]);
                    foreach (var characterParser in CharacterManager.instance.Characters)
                    {
                        if (characterParser.culture == cultureGroupParser.Cultures[0].Name)
                        {
                            characterParser.culture = this.AllCultures[this.AllCultures.Count - 1].Name;
                        }
                    }

                    foreach (var value in DynastyManager.instance.DynastyMap.Values)
                    {
                        if ((string)(value.Scope.Children[1] as ScriptCommand).Value == cultureGroupParser.Cultures[0].Name)
                        {
                            (value.Scope.Children[1] as ScriptCommand).Value = this.AllCultures[this.AllCultures.Count - 1].Name;
                        }
                    }

                    index--;
                }
            }

            this.allowMultiCultureGroups = true;
            for (int index = 0; index < this.AllCultureGroups.Count; index++)
            {
                var cultureGroupParser = this.AllCultureGroups[index];

                var provinces = new List<ProvinceParser>(cultureGroupParser.Provinces);
                // Now do the same for cultures...

                var mainCulture = cultureGroupParser.Cultures[0];

                int size = cultureGroupParser.Provinces.Count;
                if (size <= 4)
                {
                    size = 2;
                }
                else if (size < 12)
                {
                    size = 4;
                }
                else if (size < 24)
                {
                    size = 5;
                }
                else if (size < 32)
                {
                    size = 6;
                }
                else if (size < 40)
                {
                    size = 7;
                }
                else
                {
                    size = 8;
                }

                for (int c = 0; c < size; c++)
                {
                    if (provinces.Count == 0)
                    {
                        break;
                    }

                    var start = provinces[RandomIntHelper.Next(provinces.Count)];

                    if (!CultureManager.instance.CultureMap.ContainsKey(mainCulture.Name))
                    {
                        mainCulture = cultureGroupParser.Cultures[cultureGroupParser.Cultures.Count - 1];
                    }

                    if (!this.CultureMap.ContainsKey(mainCulture.Name))
                    {
                        this.CultureMap[mainCulture.Name] = mainCulture;
                    }

                    start.Culture = this.BranchCulture(mainCulture.Name);
                    var newC = start.Culture;
                    List<ProvinceParser> target = new List<ProvinceParser>();
                    target.Add(start);
                    target.AddRange(start.Adjacent.Where(p => provinces.Contains(p)));
                    int s = 1;
                    if (size > 8)
                    {
                        s = 2;
                    }

                    if (size > 15)
                    {
                        s = 3;
                    }

                    for (int x = 0; x < s; x++)
                    {
                        var toAdd = new List<ProvinceParser>();
                        target.ForEach(p => toAdd.AddRange(p.Adjacent.Where(pp => pp.land && pp.ProvinceTitle != null)));
                        target.AddRange(toAdd);
                    }

                    HashSet<ProvinceParser> toDo = new HashSet<ProvinceParser>(target);
                    foreach (var provinceParser in toDo)
                    {
                        provinceParser.Culture = newC;
                        provinces.Remove(provinceParser);
                    }
                }
            }

            // Create big religion groups covering multiple culture groups

            foreach (var religionGroupParser in ReligionManager.instance.AllReligionGroups)
            {
                var cgenum = this.AllCultureGroups.Where(cg => cg.ReligionGroup == religionGroupParser);

                var cultureGroupList = new List<CultureGroupParser>(cgenum);

                int n = RandomIntHelper.Next(5) + 4;

                for (int x = 0; x < n; x++)
                {
                    var adjacentProv = new List<ProvinceParser>();
                    var adjacent = new HashSet<CultureGroupParser>();
                    cultureGroupList.ForEach(g => g.Provinces.ForEach(p => adjacentProv.AddRange(p.Adjacent.Where(pa => pa.land && pa.ProvinceTitle != null && pa.Culture != null && pa.Culture.Group != g))));
                    adjacentProv.ForEach(p => adjacent.Add(p.Culture.Group));

                    if (adjacent.Count > 0)
                    {
                        List<CultureGroupParser> list = new List<CultureGroupParser>(adjacent);

                        var chosen = list[RandomIntHelper.Next(list.Count)];
                        chosen.ReligionGroup = religionGroupParser;
                        chosen.Provinces.ForEach(p => p.Religion = religionGroupParser.Religions[0]);
                    }
                }
            }

            // Cut out small ones

            // Now find the biggest two and make them bigger...
            ReligionGroupParser biggest = null;
            ReligionGroupParser second = null;

            for (int index = 0; index < ReligionManager.instance.AllReligionGroups.Count; index++)
            {
                var religionGroupParser = ReligionManager.instance.AllReligionGroups[index];

                if (religionGroupParser.Provinces.Count < 50)
                {
                    while (religionGroupParser.Provinces.Count > 0)
                    {
                        bool possible = true;
                        while (religionGroupParser.Provinces.Count > 0 && possible)
                        {
                            for (int i = 0; i < religionGroupParser.Provinces.Count; i++)
                            {
                                var provinceParser = religionGroupParser.Provinces[i];
                                var difcul =
                                    provinceParser.Adjacent.Where(
                                        p => p.Religion != provinceParser.Religion && p.Religion != null);
                                if (!difcul.Any())
                                {
                                    if (i == religionGroupParser.Provinces.Count - 1)
                                    {
                                        possible = false;
                                    }

                                    continue;
                                }

                                var list = new List<ProvinceParser>(difcul);
                                provinceParser.Religion = list[RandomIntHelper.Next(list.Count)].Religion;
                                break;
                            }
                        }

                        if (!possible)
                        {
                            var provinceParser = religionGroupParser.Provinces[0];

                            var list =
                                MapManager.instance.Provinces.Where(
                                    p => p.land && p.ProvinceTitle != null && p.Religion.Group != religionGroupParser).ToList();

                            this.distanceTest = provinceParser;
                            list.Sort(this.SortByDistance);

                            provinceParser.Religion = list[0].Religion;
                            provinceParser.Culture.Group.ReligionGroup = list[0].Religion.Group;
                        }
                    }
                }

                if (religionGroupParser.Provinces.Count == 0)
                {
                    ReligionManager.instance.AllReligionGroups.Remove(religionGroupParser);
                    System.Console.Out.WriteLine(religionGroupParser.Religions[0].Name + " removed");

                    religionGroupParser.Scope.Remove(religionGroupParser.Religions[0].Scope);
                    ReligionManager.instance.Script.Root.Remove(religionGroupParser.Scope);
                    ReligionManager.instance.ReligionMap.Remove(religionGroupParser.Religions[0].Name);
                    ReligionManager.instance.AllReligions.Remove(religionGroupParser.Religions[0]);

                    foreach (var characterParser in CharacterManager.instance.Characters)
                    {
                        if (characterParser.religion == religionGroupParser.Religions[0].Name)
                        {
                            characterParser.religion = ReligionManager.instance.AllReligions[ReligionManager.instance.AllReligions.Count - 1].Name;
                        }
                    }

                    index--;
                }
            }

            ReligionManager.instance.AllReligionGroups.Sort(ReligionManager.instance.SortByBelievers);

            biggest = ReligionManager.instance.AllReligionGroups[0];
            if (ReligionManager.instance.AllReligionGroups.Count > 1)
            {
                second = ReligionManager.instance.AllReligionGroups[1];
            }

            for (int index = 0; index < ReligionManager.instance.AllReligionGroups.Count; index++)
            {
                var religionGroup = ReligionManager.instance.AllReligionGroups[index];

                var provinces = new List<ProvinceParser>(religionGroup.Provinces);
                // Now do the same for cultures...

                var mainReligion = religionGroup.Religions[0];

                int size = religionGroup.Provinces.Count;
                if (size <= 4)
                {
                    size = 1;
                }
                else if (size < 12)
                {
                    size = 1;
                }
                else if (size < 32)
                {
                    size = 2;
                }
                else
                {
                    size = 3;
                }

                if (biggest == religionGroup || second == religionGroup)
                {
                    size = 3;
                }

                for (int c = 0; c < size; c++)
                {
                    if (provinces.Count == 0)
                    {
                        break;
                    }

                    var start = provinces[RandomIntHelper.Next(provinces.Count)];

                    start.Religion = ReligionManager.instance.BranchReligion(mainReligion.Name, start.Culture.Name);
                    var newC = start.Religion;
                    List<ProvinceParser> target = new List<ProvinceParser>();
                    target.Add(start);
                    target.AddRange(start.Adjacent.Where(p => provinces.Contains(p)));
                    int s = 2;
                    if (size > 16)
                    {
                        s = 3;
                    }

                    if (size > 32)
                    {
                        s = 4;
                    }

                    if (biggest == religionGroup || second == religionGroup)
                    {
                        if (c <= 2)
                        {
                            s += 2;
                        }
                    }

                    for (int x = 0; x < s; x++)
                    {
                        var toAdd = new List<ProvinceParser>();
                        target.ForEach(p => toAdd.AddRange(p.Adjacent.Where(pp => pp.land && pp.ProvinceTitle != null && pp.Religion.Group == start.Religion.Group)));
                        target.AddRange(toAdd);
                    }

                    HashSet<ProvinceParser> toDo = new HashSet<ProvinceParser>(target);
                    foreach (var provinceParser in toDo)
                    {
                        provinceParser.Religion = newC;
                        provinces.Remove(provinceParser);
                    }
                }
            }

            for (int index = 0; index < ReligionManager.instance.AllReligionGroups.Count; index++)
            {
                var religionParser = ReligionManager.instance.AllReligionGroups[index];
                if (religionParser.Provinces.Count == 0)
                {
                    ReligionManager.instance.AllReligionGroups.Remove(religionParser);
                    index--;
                    continue;
                }

                religionParser.TryFillHolySites();
            }

            for (int index = 0; index < ReligionManager.instance.AllReligions.Count; index++)
            {
                var religionParser = ReligionManager.instance.AllReligions[index];
                if (religionParser.Provinces.Count == 0)
                {
                    System.Console.Out.WriteLine(religionParser.Name + " removed");
                    if (religionParser.Scope.Name == "enuique")
                    {
                    }

                    ReligionManager.instance.AllReligions.Remove(religionParser);
                    ReligionManager.instance.ReligionMap.Remove(religionParser.Name);
                    religionParser.Group.Scope.Remove(religionParser.Scope);
                    index--;
                    continue;
                }

                religionParser.TryFillHolySites();
            }

            foreach (var characterParser in CharacterManager.instance.Characters)
            {
                if (characterParser.Titles.Count > 0)
                {
                    if (characterParser.PrimaryTitle.Rank == 2)
                    {
                        characterParser.culture = characterParser.PrimaryTitle.SubTitles.First().Value.Owns[0].Culture.Name;
                        characterParser.religion = characterParser.PrimaryTitle.SubTitles.First().Value.Owns[0].Religion.Name;
                    }
                    else
                    {
                        characterParser.culture = characterParser.PrimaryTitle.Owns[0].Culture.Name;
                        characterParser.religion = characterParser.PrimaryTitle.Owns[0].Religion.Name;
                    }

                    foreach (var titleParser in characterParser.Titles)
                    {
                        titleParser.culture = characterParser.culture;
                    }
                }
            }

            var l = MapManager.instance.Provinces.Where(p => p.ProvinceTitle != null).ToList();

            foreach (var provinceParser in l)
            {
                provinceParser.initialReligion = provinceParser.Religion.Name;
                provinceParser.initialCulture = provinceParser.Culture.Name;
            }

            foreach (var religionParser in ReligionManager.instance.AllReligions)
            {
                if (religionParser.Provinces.Count > 0)
                {
                    ReligionManager.instance.ReligionMap[religionParser.Name] = religionParser;
                }

                if (religionParser.Provinces.Count > 0 && (religionParser.hasLeader || religionParser.autocephaly))
                {
                    religionParser.DoLeader(religionParser.Provinces[RandomIntHelper.Next(religionParser.Provinces.Count)]);
                }
            }
        }

        private int SortByDistance(ProvinceParser x, ProvinceParser y)
        {
            float dist = x.DistanceTo(this.distanceTest);
            float dist2 = y.DistanceTo(this.distanceTest);

            if (dist < dist2)
            {
                return -1;
            }

            if (dist > dist2)
            {
                return 1;
            }

            return 0;
        }

        public bool allowMultiCultureGroups { get; set; }

        public void LoadVanilla()
        {
            this.SaveCultures = false;
            var files = ModManager.instance.GetFileKeys("common\\cultures");
            foreach (var file in files)
            {
                Script s = ScriptLoader.instance.LoadKey(file);
                foreach (var rootChild in s.Root.Children)
                {
                    CultureGroupParser p = new CultureGroupParser(rootChild as ScriptScope);

                    this.AllCultureGroups.Add(p);
                    this.CultureGroupMap[p.Name] = p;
                    foreach (var scopeChild in p.Scope.Children)
                    {
                        if (scopeChild is ScriptScope)
                        {
                            var sc = scopeChild as ScriptScope;

                            if (sc.Name == "graphical_cultures")
                            {
                                continue;
                            }

                            CultureParser r = new CultureParser(sc, p);
                            this.AllCultures.Add(r);
                            this.CultureMap[r.Name] = r;
                            p.Cultures.Add(r);
                            r.Group = p;
                            r.LanguageName = LanguageManager.instance.Get(r.Name);
                            KingdomHelper dna = new KingdomHelper();
                            foreach (var scope in r.Scope.Scopes)
                            {
                                if (scope.Name == "male_names" || scope.Name == "female_names")
                                {
                                    string[] male_names = scope.Data.Split(new[] { ' ', '_', '\t' });
                                    foreach (var maleName in male_names)
                                    {
                                        var mName = maleName.Trim();
                                        if (mName.Length > 0)
                                        {
                                            dna.Cannibalize(mName);
                                        }
                                    }
                                }
                            }

                            r.dna = dna;
                        }
                    }
                }
            }
        }

        public bool SaveCultures { get; set; } = true;
    }
}