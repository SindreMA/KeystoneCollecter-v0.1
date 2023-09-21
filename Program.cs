using Force.DeepCloner;
using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeystoneCollecter
{
    class Program
    {
        public static List<List<Weekly_Modifiers>> AffixCombos = new List<List<Weekly_Modifiers>>();
        public static int ExpID = 7;
        public static List<_Affixes> _AffixesLS = new List<_Affixes>();
        public static List<string> skipList = new List<string>();
        static string season = "";
        public static List<Rankedgroup> TempDung = new List<Rankedgroup>();
        public static int lowestAmountOfRuns = 5000;
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...\n");
            Console.WriteLine("Gathering initial RIO data...\n");
            GetIntData();
            Console.WriteLine("Collecting leaderboard data...\n \n");
            FillData();

            var Output = new RioData() { Affixes = new List<_Affixes>(), Season = season };
            foreach (var item in oldData.Affixes)
            {
                Output.Affixes.Add(item.DeepClone());
            }
            var notSkippedCombos = AffixCombos.Where(x=> x.Exists(c=> c.name.ToLower() == "reaping")).Where(z => !skipList.Exists(x => x == GetAffixSlug(z)));
            foreach (var weekly_modifiers in notSkippedCombos)
            {
                var aff = new _Affixes();
                aff.Affixes = weekly_modifiers;
                aff.Dungeons = new List<_Dungeon>();

                foreach (var dungeon in dungeons)
                {
                    var dung = new _Dungeon();
                    dung.Name = dungeon;
                    dung.Runs = 0;
                    dung.RunsPerLevel = new Dictionary<int, int>();
                    dung.TotalScore = 0;
                    dung.RunsItems = new List<Rankedgroup>();
                    var likeDungeons = TempDung.Where(x =>
                    x.dungeonstring == dungeon &&
                        x.run.weekly_modifiers.Any(z => z.id == weekly_modifiers[0].id) &&
                        x.run.weekly_modifiers.Any(z => z.id == weekly_modifiers[1].id) &&
                        x.run.weekly_modifiers.Any(z => z.id == weekly_modifiers[2].id) &&
                        x.run.weekly_modifiers.Any(z => z.id == weekly_modifiers[3].id)
                        );
                    foreach (var item in likeDungeons)
                    {
                        dung.RunsItems.Add(item.DeepClone());
                        weekly_modifiers[0].description = item.run.weekly_modifiers[0].description;
                        weekly_modifiers[1].description = item.run.weekly_modifiers[1].description;
                        weekly_modifiers[2].description = item.run.weekly_modifiers[2].description;
                        weekly_modifiers[3].description = item.run.weekly_modifiers[3].description;
                    }
                    aff.Dungeons.Add(dung.DeepClone());
                }
                Output.Affixes.Add(aff.DeepClone());
            }

            foreach (var item in Output.Affixes)
            {
                
                foreach (var dung in item.Dungeons.Where(x => x.RunsItems != null ))
                {

                    if (dung.RunsItems.Where(x => x.run.mythic_level >= 6).Count() < lowestAmountOfRuns)
                    {
                        lowestAmountOfRuns = dung.RunsItems.Where(x => x.run.mythic_level >= 6).Count();
                    }
                    /*
                    if (dung.RunsItems.Where(x => x.run.num_modifiers_active >= 3).Count() < lowestAmountOfRuns)
                    {
                        lowestAmountOfRuns = dung.RunsItems.Where(x => x.run.num_modifiers_active >= 3).Count();
                    }
                    */
                }
                if (item.Compositions == null)
                {
                    item.Compositions = new CompositionInfo()
                    {
                        DPSComposition = new List<CompositionContainer>(),
                        HealerTankComposition = new List<CompositionContainer>(),
                        TeamComposition = new List<CompositionContainer>()
                    };
                }
                if (item.Factions == null) item.Factions = new List<FactionInfo>();
                if (item.Classes == null)
                {
                    item.Classes = new ClassItems()
                    {
                        DPSs = new List<ClassInfo>(),
                        Healers = new List<ClassInfo>(),
                        Tanks = new List<ClassInfo>(),

                    };
                }
                

                foreach (var dungeon in item.Dungeons.Where(x => x.RunsItems != null))
                {
                    dungeon.Compositions = new CompositionInfo()
                    {
                        DPSComposition = new List<CompositionContainer>(),
                        HealerTankComposition = new List<CompositionContainer>(),
                        TeamComposition = new List<CompositionContainer>()
                    };
                    dungeon.Factions = new List<FactionInfo>();
                    dungeon.Classes = new ClassItems()
                    {
                        DPSs = new List<ClassInfo>(),
                        Healers = new List<ClassInfo>(),
                        Tanks = new List<ClassInfo>(),

                    };



                    dungeon.Slug = dungeon.Name.DeepClone();
                    //old way int.Parse((resultsPerItem * 0.95).ToString().Split(',')[0])
                    foreach (var run in dungeon.RunsItems.Take(lowestAmountOfRuns))
                    {

                        ////Faction gathering
                        ////////////////////////////
                        if (item.Factions.Exists(x => x.Faction == run.run.faction))
                        {
                            var faction = item.Factions.FirstOrDefault(x => x.Faction == run.run.faction);
                            faction.Runs++;
                            faction.Score += run.score;
                        }
                        else
                        {
                            var faction = new FactionInfo();
                            faction.Faction = run.run.faction;
                            faction.Runs = 1;
                            faction.Score = run.score.DeepClone();
                            item.Factions.Add(faction.DeepClone());

                        }
                        if (dungeon.Factions.Exists(x => x.Faction == run.run.faction))
                        {
                            var faction = dungeon.Factions.FirstOrDefault(x => x.Faction == run.run.faction);
                            faction.Runs++;
                            faction.Score += run.score;
                        }
                        else
                        {
                            var faction = new FactionInfo();
                            faction.Faction = run.run.faction.DeepClone();
                            faction.Runs = 1;
                            faction.Score = run.score.DeepClone();
                            dungeon.Factions.Add(faction.DeepClone());

                        }




                        ////////////////////////////
                        ////Class gathering
                        ////////////////////////////


                        foreach (var player in run.run.roster)
                        {


                            if (player.role == "tank")
                            {

                                ///////////////////ADD to affix combo////////////////////////////////
                                if (item.Classes.Tanks.Exists(x => x.ClassID == player.character.Class.id))
                                {
                                    var OS = item.Classes.Tanks.FirstOrDefault(x => x.ClassID == player.character.Class.id);
                                    OS.Runs++;
                                    OS.Score += run.score;

                                    if (OS.Specs.Exists(x => x.SpecID == player.character.spec.id))
                                    {
                                        var spec = OS.Specs.FirstOrDefault(x => x.SpecID == player.character.spec.id);
                                        spec.Runs++;
                                        spec.Score += run.score;
                                    }
                                    else
                                    {
                                        var spec = new SpecInfo();

                                        spec.SpecID = player.character.spec.id.DeepClone();
                                        spec.SpecName = player.character.spec.name.DeepClone();
                                        spec.SpecSlug = player.character.spec.slug.DeepClone();
                                        spec.SpecRole = player.role.DeepClone();

                                        spec.Runs = 1;
                                        spec.Score = run.score.DeepClone();

                                        OS.Specs.Add(spec.DeepClone());
                                    }

                                }
                                else
                                {
                                    var OS = new ClassInfo();

                                    OS.ClassID = player.character.Class.id.DeepClone();
                                    OS.ClassName = player.character.Class.name.DeepClone();
                                    OS.ClassSlug = player.character.Class.slug.DeepClone();
                                    OS.Specs = new List<SpecInfo>();

                                    var spec = new SpecInfo();

                                    spec.SpecID = player.character.spec.id.DeepClone();
                                    spec.SpecName = player.character.spec.name.DeepClone();
                                    spec.SpecSlug = player.character.spec.slug.DeepClone();
                                    spec.SpecRole = player.role.DeepClone();

                                    spec.Runs = 1;
                                    spec.Score = run.score.DeepClone();

                                    OS.Specs.Add(spec.DeepClone());

                                    OS.Runs = 1;
                                    OS.Score += run.score;

                                    item.Classes.Tanks.Add(OS.DeepClone());
                                }
                                ///////////////////////////////ADD TO DUNGEON STATS///////////////////////////////
                                if (dungeon.Classes.Tanks.Exists(x => x.ClassID == player.character.Class.id))
                                {
                                    var OS = dungeon.Classes.Tanks.FirstOrDefault(x => x.ClassID == player.character.Class.id);
                                    OS.Runs++;
                                    OS.Score += run.score;

                                    if (OS.Specs.Exists(x => x.SpecID == player.character.spec.id))
                                    {
                                        var spec = OS.Specs.FirstOrDefault(x => x.SpecID == player.character.spec.id);
                                        spec.Runs++;
                                        spec.Score += run.score;
                                    }
                                    else
                                    {
                                        var spec = new SpecInfo();

                                        spec.SpecID = player.character.spec.id.DeepClone();
                                        spec.SpecName = player.character.spec.name.DeepClone();
                                        spec.SpecSlug = player.character.spec.slug.DeepClone();
                                        spec.SpecRole = player.role.DeepClone();

                                        spec.Runs = 1;
                                        spec.Score = run.score.DeepClone();

                                        OS.Specs.Add(spec.DeepClone());
                                    }

                                }
                                else
                                {
                                    var OS = new ClassInfo();

                                    OS.ClassID = player.character.Class.id.DeepClone();
                                    OS.ClassName = player.character.Class.name.DeepClone();
                                    OS.ClassSlug = player.character.Class.slug.DeepClone();
                                    OS.Specs = new List<SpecInfo>();
                                    var spec = new SpecInfo();

                                    spec.SpecID = player.character.spec.id.DeepClone();
                                    spec.SpecName = player.character.spec.name.DeepClone();
                                    spec.SpecSlug = player.character.spec.slug.DeepClone();
                                    spec.SpecRole = player.role.DeepClone();

                                    spec.Runs = 1;
                                    spec.Score = run.score.DeepClone();

                                    OS.Specs.Add(spec.DeepClone());

                                    OS.Runs = 1;
                                    OS.Score += run.score;

                                    dungeon.Classes.Tanks.Add(OS.DeepClone());
                                }
                            }
                            else if (player.role == "healer")
                            {
                                ///////////////////ADD to affix combo////////////////////////////////
                                if (item.Classes.Healers.Exists(x => x.ClassID == player.character.Class.id))
                                {
                                    var OS = item.Classes.Healers.FirstOrDefault(x => x.ClassID == player.character.Class.id);
                                    OS.Runs++;
                                    OS.Score += run.score;

                                    if (OS.Specs.Exists(x => x.SpecID == player.character.spec.id))
                                    {
                                        var spec = OS.Specs.FirstOrDefault(x => x.SpecID == player.character.spec.id);
                                        spec.Runs++;
                                        spec.Score += run.score;
                                    }
                                    else
                                    {
                                        var spec = new SpecInfo();

                                        spec.SpecID = player.character.spec.id.DeepClone();
                                        spec.SpecName = player.character.spec.name.DeepClone();
                                        spec.SpecSlug = player.character.spec.slug.DeepClone();
                                        spec.SpecRole = player.role.DeepClone();

                                        spec.Runs = 1;
                                        spec.Score = run.score.DeepClone();

                                        OS.Specs.Add(spec.DeepClone());
                                    }

                                }
                                else
                                {
                                    var OS = new ClassInfo();

                                    OS.ClassID = player.character.Class.id.DeepClone();
                                    OS.ClassName = player.character.Class.name.DeepClone();
                                    OS.ClassSlug = player.character.Class.slug.DeepClone();
                                    OS.Specs = new List<SpecInfo>();
                                    var spec = new SpecInfo();

                                    spec.SpecID = player.character.spec.id.DeepClone();
                                    spec.SpecName = player.character.spec.name.DeepClone();
                                    spec.SpecSlug = player.character.spec.slug.DeepClone();
                                    spec.SpecRole = player.role.DeepClone();

                                    spec.Runs = 1;
                                    spec.Score = run.score.DeepClone();

                                    OS.Specs.Add(spec.DeepClone());

                                    OS.Runs = 1;
                                    OS.Score += run.score;

                                    item.Classes.Healers.Add(OS.DeepClone());
                                }
                                ///////////////////////////////ADD TO DUNGEON STATS///////////////////////////////
                                if (dungeon.Classes.Healers.Exists(x => x.ClassID == player.character.Class.id))
                                {
                                    var OS = dungeon.Classes.Healers.FirstOrDefault(x => x.ClassID == player.character.Class.id);
                                    OS.Runs++;
                                    OS.Score += run.score;

                                    if (OS.Specs.Exists(x => x.SpecID == player.character.spec.id))
                                    {
                                        var spec = OS.Specs.FirstOrDefault(x => x.SpecID == player.character.spec.id);
                                        spec.Runs++;
                                        spec.Score += run.score;
                                    }
                                    else
                                    {
                                        var spec = new SpecInfo();

                                        spec.SpecID = player.character.spec.id.DeepClone();
                                        spec.SpecName = player.character.spec.name.DeepClone();
                                        spec.SpecSlug = player.character.spec.slug.DeepClone();
                                        spec.SpecRole = player.role.DeepClone();


                                        spec.Runs = 1;
                                        spec.Score = run.score.DeepClone();

                                        OS.Specs.Add(spec.DeepClone());
                                    }

                                }
                                else
                                {
                                    var OS = new ClassInfo();

                                    OS.ClassID = player.character.Class.id.DeepClone();
                                    OS.ClassName = player.character.Class.name.DeepClone();
                                    OS.ClassSlug = player.character.Class.slug.DeepClone();
                                    OS.Specs = new List<SpecInfo>();
                                    var spec = new SpecInfo();

                                    spec.SpecID = player.character.spec.id.DeepClone();
                                    spec.SpecName = player.character.spec.name.DeepClone();
                                    spec.SpecSlug = player.character.spec.slug.DeepClone();
                                    spec.SpecRole = player.role.DeepClone();

                                    spec.Runs = 1;
                                    spec.Score = run.score.DeepClone();

                                    OS.Specs.Add(spec.DeepClone());

                                    OS.Runs = 1;
                                    OS.Score += run.score;

                                    dungeon.Classes.Healers.Add(OS.DeepClone());
                                }

                            }
                            else if (player.role == "dps")
                            {

                                ///////////////////ADD to affix combo////////////////////////////////
                                if (item.Classes.DPSs.Exists(x => x.ClassID == player.character.Class.id))
                                {
                                    var OS = item.Classes.DPSs.FirstOrDefault(x => x.ClassID == player.character.Class.id);
                                    OS.Runs++;
                                    OS.Score += run.score;

                                    if (OS.Specs.Exists(x => x.SpecID == player.character.spec.id))
                                    {
                                        var spec = OS.Specs.FirstOrDefault(x => x.SpecID == player.character.spec.id);
                                        spec.Runs++;
                                        spec.Score += run.score;
                                    }
                                    else
                                    {
                                        var spec = new SpecInfo();

                                        spec.SpecID = player.character.spec.id.DeepClone();
                                        spec.SpecName = player.character.spec.name.DeepClone();
                                        spec.SpecSlug = player.character.spec.slug.DeepClone();
                                        spec.SpecRole = player.role.DeepClone();

                                        spec.Runs = 1;
                                        spec.Score = run.score.DeepClone();

                                        OS.Specs.Add(spec.DeepClone());
                                    }

                                }
                                else
                                {
                                    var OS = new ClassInfo();

                                    OS.ClassID = player.character.Class.id.DeepClone();
                                    OS.ClassName = player.character.Class.name.DeepClone();
                                    OS.ClassSlug = player.character.Class.slug.DeepClone();
                                    OS.Specs = new List<SpecInfo>();
                                    var spec = new SpecInfo();

                                    spec.SpecID = player.character.spec.id.DeepClone();
                                    spec.SpecName = player.character.spec.name.DeepClone();
                                    spec.SpecSlug = player.character.spec.slug.DeepClone();
                                    spec.SpecRole = player.role.DeepClone();

                                    spec.Runs = 1;
                                    spec.Score = run.score.DeepClone();

                                    OS.Specs.Add(spec.DeepClone());

                                    OS.Runs = 1;
                                    OS.Score += run.score;

                                    item.Classes.DPSs.Add(OS.DeepClone());
                                }
                                ///////////////////////////////ADD TO DUNGEON STATS///////////////////////////////
                                if (dungeon.Classes.DPSs.Exists(x => x.ClassID == player.character.Class.id))
                                {
                                    var OS = dungeon.Classes.DPSs.FirstOrDefault(x => x.ClassID == player.character.Class.id);
                                    OS.Runs++;
                                    OS.Score += run.score;

                                    if (OS.Specs.Exists(x => x.SpecID == player.character.spec.id))
                                    {
                                        var spec = OS.Specs.FirstOrDefault(x => x.SpecID == player.character.spec.id);
                                        spec.Runs++;
                                        spec.Score += run.score;
                                    }
                                    else
                                    {
                                        var spec = new SpecInfo();

                                        spec.SpecID = player.character.spec.id.DeepClone();
                                        spec.SpecName = player.character.spec.name.DeepClone();
                                        spec.SpecSlug = player.character.spec.slug.DeepClone();
                                        spec.SpecRole = player.role.DeepClone();

                                        spec.Runs = 1;
                                        spec.Score = run.score.DeepClone();

                                        OS.Specs.Add(spec.DeepClone());
                                    }

                                }
                                else
                                {
                                    var OS = new ClassInfo();

                                    OS.ClassID = player.character.Class.id.DeepClone();
                                    OS.ClassName = player.character.Class.name.DeepClone();
                                    OS.ClassSlug = player.character.Class.slug.DeepClone();
                                    OS.Specs = new List<SpecInfo>();
                                    var spec = new SpecInfo();

                                    spec.SpecID = player.character.spec.id.DeepClone();
                                    spec.SpecName = player.character.spec.name.DeepClone();
                                    spec.SpecSlug = player.character.spec.slug.DeepClone();
                                    spec.SpecRole = player.role.DeepClone();

                                    spec.Runs = 1;
                                    spec.Score = run.score.DeepClone();

                                    OS.Specs.Add(spec.DeepClone());

                                    OS.Runs = 1;
                                    OS.Score += run.score;

                                    dungeon.Classes.DPSs.Add(OS.DeepClone());
                                }
                            }
                        }
                        ////////////////////////////

                        /*
                         * NOT SURE WHAT NEXT STEP IS
                         * 
                         * 
                        foreach (var TeamSet in item.Compositions.TeamComposition)
                        {
                            if (TeamSet.Classes.Count == TeamComp.Classes.Count)
                            {
                                for (int i = 0; i < TeamSet.Classes.Count; i++)
                                {

                                    if (TeamComp.Classes.Exists(x=> TeamSet.Classes[i].ClassID == ))
                                    {

                                    }
                                }

                            }
                            foreach (var Class in TeamSet.Classes)
                            {
                                bool classExist = false;
                                foreach (var TCclass in TeamComp.Classes)
                                {
                                    if (TCclass.ClassID == Class.ClassID)
                                    {
                                        classExist = true;
                                    }
                                }
                            }
                      
                        }
                    */

                        ////Dungeon Gathering
                        /////////////////////////////////
                        dungeon.TotalScore += run.score;
                        dungeon.Runs++;
                        if (dungeon.RunsPerLevel.Any(x => x.Key == run.run.mythic_level))
                        {
                            dungeon.RunsPerLevel[run.run.mythic_level]++;
                        }
                        else
                        {
                            dungeon.RunsPerLevel.Add(run.run.mythic_level, 1);
                        }
                        dungeon.Name = run.run.dungeon.name.DeepClone();
                        //////////////////////////////////////
                    }

                }

            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////         TEAM COMPOSITION        ////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            foreach (var affixset in Output.Affixes)
            {
                affixset.slug = GetAffixSlug(affixset);
                foreach (var dungeon in affixset.Dungeons.Where(x => x.RunsItems != null))
                {
                    foreach (var run in dungeon.RunsItems.Take(lowestAmountOfRuns))
                    {
                        var comp = new CompositionContainer();
                        comp.Score = run.score.DeepClone();
                        comp.Runs = 1;
                        comp.DPSs = getSpecs(run, "dps").DeepClone();
                        comp.Tank = getSpecs(run, "tank").DeepClone();
                        comp.Healer = getSpecs(run, "healer").DeepClone();
                        if (comp.DPSs.Count != 3 || comp.Healer.Count != 1 || comp.Tank.Count != 1) continue;
                        if (dungeon.Compositions.TeamComposition.Exists(x => IsLike(x.DPSs, comp.DPSs) && IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank)))
                        {
                            var existComp = dungeon.Compositions.TeamComposition.FirstOrDefault(x => IsLike(x.DPSs, comp.DPSs) && IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank));
                            existComp.Runs += comp.Runs;
                            existComp.Score += comp.Score;
                        }
                        else
                        {
                            dungeon.Compositions.TeamComposition.Add(comp.DeepClone());
                        }
                        if (affixset.Compositions.TeamComposition.Exists(x => IsLike(x.DPSs, comp.DPSs) && IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank)))
                        {
                            var existComp = affixset.Compositions.TeamComposition.FirstOrDefault(x => IsLike(x.DPSs, comp.DPSs) && IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank));
                            existComp.Runs += comp.Runs;
                            existComp.Score += comp.Score;
                        }
                        else
                        {
                            affixset.Compositions.TeamComposition.Add(comp.DeepClone());
                        }

                    }
                }
            }
            foreach (var affixset in Output.Affixes)
            {
                int rank = 1;
                foreach (var comp in affixset.Compositions.TeamComposition.OrderByDescending(x => x.Score))
                {
                    comp.Rank = rank.DeepClone();
                    rank++;
                }
                affixset.Compositions.TeamComposition = affixset.Compositions.TeamComposition.OrderByDescending(x => x.Score).ToList().DeepClone();

                foreach (var dungeon in affixset.Dungeons.Where(x => x.RunsItems != null))
                {
                    int dungrank = 1;
                    foreach (var comp in dungeon.Compositions.TeamComposition.OrderByDescending(x => x.Score))
                    {
                        comp.Rank = dungrank.DeepClone();
                        dungrank++;
                    }
                    dungeon.Compositions.TeamComposition = dungeon.Compositions.TeamComposition.OrderByDescending(x => x.Score).ToList().DeepClone();
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////         TANKHEALER COMPOSITION        ////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            foreach (var affixset in Output.Affixes)
            {
                foreach (var dungeon in affixset.Dungeons.Where(x => x.RunsItems != null))
                {
                    foreach (var run in dungeon.RunsItems.Take(lowestAmountOfRuns))
                    {
                        var comp = new CompositionContainer();
                        comp.Score = run.score.DeepClone();
                        comp.Runs = 1;

                        comp.Tank = getSpecs(run, "tank").DeepClone();
                        comp.Healer = getSpecs(run, "healer").DeepClone();
                        if (comp.Healer.Count != 1 || comp.Tank.Count != 1) continue;
                        if (dungeon.Compositions.HealerTankComposition.Exists(x => IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank)))
                        {
                            var existComp = dungeon.Compositions.HealerTankComposition.FirstOrDefault(x => IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank));
                            existComp.Runs += comp.Runs;
                            existComp.Score += comp.Score;
                        }
                        else
                        {
                            dungeon.Compositions.HealerTankComposition.Add(comp.DeepClone());
                        }
                        if (affixset.Compositions.HealerTankComposition.Exists(x => IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank)))
                        {
                            var existComp = affixset.Compositions.HealerTankComposition.FirstOrDefault(x => IsLike(x.Healer, comp.Healer) && IsLike(x.Tank, comp.Tank));
                            existComp.Runs += comp.Runs;
                            existComp.Score += comp.Score;
                        }
                        else
                        {
                            affixset.Compositions.HealerTankComposition.Add(comp.DeepClone());
                        }

                    }
                }
            }
            foreach (var affixset in Output.Affixes)
            {
                int rank = 1;
                foreach (var comp in affixset.Compositions.HealerTankComposition.OrderByDescending(x => x.Score))
                {
                    comp.Rank = rank.DeepClone();
                    rank++;
                }
                affixset.Compositions.HealerTankComposition = affixset.Compositions.HealerTankComposition.OrderByDescending(x => x.Score).ToList().DeepClone();

                foreach (var dungeon in affixset.Dungeons.Where(x => x.RunsItems != null))
                {
                    int dungrank = 1;
                    foreach (var comp in dungeon.Compositions.HealerTankComposition.OrderByDescending(x => x.Score))
                    {
                        comp.Rank = dungrank.DeepClone();
                        dungrank++;
                    }
                    dungeon.Compositions.HealerTankComposition = dungeon.Compositions.HealerTankComposition.OrderByDescending(x => x.Score).ToList().DeepClone();
                }
            }



            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////         DPSs COMPOSITION        ////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            foreach (var affixset in Output.Affixes)
            {
                foreach (var dungeon in affixset.Dungeons.Where(x => x.RunsItems != null))
                {
                    foreach (var run in dungeon.RunsItems.Take(lowestAmountOfRuns))
                    {
                        var comp = new CompositionContainer();
                        comp.Score = run.score.DeepClone();
                        comp.Runs = 1;
                        comp.DPSs = getSpecs(run, "dps").DeepClone();
                        if (comp.DPSs.Count != 3) continue;

                        if (dungeon.Compositions.DPSComposition.Exists(x => IsLike(x.DPSs, comp.DPSs)))
                        {
                            var existComp = dungeon.Compositions.DPSComposition.FirstOrDefault(x => IsLike(x.DPSs, comp.DPSs));
                            existComp.Runs += comp.Runs;
                            existComp.Score += comp.Score;
                        }
                        else
                        {
                            dungeon.Compositions.DPSComposition.Add(comp.DeepClone());
                        }
                        if (affixset.Compositions.DPSComposition.Exists(x => IsLike(x.DPSs, comp.DPSs)))
                        {
                            var existComp = affixset.Compositions.DPSComposition.FirstOrDefault(x => IsLike(x.DPSs, comp.DPSs));
                            existComp.Runs += comp.Runs;
                            existComp.Score += comp.Score;
                        }
                        else
                        {
                            affixset.Compositions.DPSComposition.Add(comp.DeepClone());
                        }

                    }
                }
            }
            foreach (var affixset in Output.Affixes)
            {
                int rank = 1;
                foreach (var comp in affixset.Compositions.DPSComposition.OrderByDescending(x => x.Score))
                {
                    comp.Rank = rank.DeepClone();
                    rank++;
                }
                affixset.Compositions.DPSComposition = affixset.Compositions.DPSComposition.OrderByDescending(x => x.Score).ToList().DeepClone();

                foreach (var dungeon in affixset.Dungeons.Where(x => x.RunsItems != null))
                {
                    int dungrank = 1;
                    foreach (var comp in dungeon.Compositions.DPSComposition.OrderByDescending(x => x.Score))
                    {
                        comp.Rank = dungrank.DeepClone();
                        dungrank++;
                    }
                    dungeon.Compositions.DPSComposition = dungeon.Compositions.DPSComposition.OrderByDescending(x => x.Score).ToList().DeepClone();
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            foreach (var affixset in Output.Affixes)
            {
                int rank = 1;
                foreach (var dungeon in affixset.Dungeons.OrderByDescending(x => x.TotalScore))
                {
                    dungeon.Rank = rank.DeepClone();
                    rank++;
                    dungeon.RunsPerLevel = dungeon.RunsPerLevel.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value).DeepClone();
                }
                affixset.Dungeons = affixset.Dungeons.OrderBy(x => x.Rank).ToList().DeepClone();
            }

            //Add in old data
            foreach (var item in Output.Affixes)
            {
                foreach (var dungeon in item.Dungeons)
                {
                    var rightOrder = dungeon.RunsPerLevel.OrderBy(x => x.Key).DeepClone();
                    dungeon.RunsPerLevel = new Dictionary<int, int>();
                    foreach (var s in rightOrder)
                    {
                        dungeon.RunsPerLevel.Add(s.Key, s.Value);
                    }
                }
            }

            ///////////////////////////////////////ADD RANKS////////////////////////////////////////
            /// FACTRIONS
            foreach (var affixCombo in Output.Affixes)
            {
                int factionRank = 1;
                foreach (var faction in affixCombo.Factions.OrderByDescending(x => x.Score))
                {
                    faction.Rank = factionRank.DeepClone();
                    factionRank++;
                }
                affixCombo.Factions = affixCombo.Factions.OrderBy(x => x.Rank).ToList().DeepClone();
                foreach (var dungeon in affixCombo.Dungeons.Where(x => x.RunsItems != null))
                {
                    int dungeonfactionRank = 1;
                    foreach (var faction in affixCombo.Factions.OrderByDescending(x => x.Score))
                    {
                        faction.Rank = dungeonfactionRank.DeepClone();
                        dungeonfactionRank++;
                    }
                    dungeon.Factions = dungeon.Factions.OrderBy(x => x.Rank).ToList().DeepClone().DeepClone();

                }
            }
            ///////////////////////////////////////ADD RANKS////////////////////////////////////////
            /// DUNGEON CLASSES
            foreach (var affixCombo in Output.Affixes)
            {
                foreach (var dungeon in affixCombo.Dungeons.Where(x => x.RunsItems != null))
                {
                    ///////////////////////////////////////  DPS  //////////////////////////////////
                    int dpsrank = 1;
                    foreach (var Class in dungeon.Classes.DPSs.OrderByDescending(x => x.Score))
                    {
                        Class.Rank = dpsrank.DeepClone();
                        dpsrank++;
                        int specRank = 1;
                        foreach (var spec in Class.Specs.OrderByDescending(x => x.Score))
                        {
                            spec.Rank = specRank.DeepClone();
                            specRank++;
                        }
                        Class.Specs = Class.Specs.OrderBy(x => x.Rank).ToList().DeepClone();
                    }
                    dungeon.Classes.DPSs = dungeon.Classes.DPSs.OrderBy(x => x.Rank).ToList().DeepClone();
                    ///////////////////////////////////////  Healer  //////////////////////////////////
                    int Healerrank = 1;
                    foreach (var Class in dungeon.Classes.Healers.OrderByDescending(x => x.Score))
                    {
                        Class.Rank = Healerrank.DeepClone();
                        Healerrank++;
                        int specRank = 1;
                        foreach (var spec in Class.Specs.OrderByDescending(x => x.Score))
                        {
                            spec.Rank = specRank.DeepClone();
                            specRank++;
                        }
                        Class.Specs = Class.Specs.OrderBy(x => x.Rank).ToList().DeepClone();
                    }
                    dungeon.Classes.Healers = dungeon.Classes.Healers.OrderBy(x => x.Rank).ToList().DeepClone();
                    ///////////////////////////////////////  Tank  //////////////////////////////////
                    int Tankrank = 1;
                    foreach (var Class in dungeon.Classes.Tanks.OrderByDescending(x => x.Score))
                    {
                        Class.Rank = Tankrank.DeepClone();
                        Tankrank++;
                        int specRank = 1;
                        foreach (var spec in Class.Specs.OrderByDescending(x => x.Score))
                        {
                           
                            spec.Rank = specRank.DeepClone();
                            specRank++;
                        }
                        Class.Specs = Class.Specs.OrderBy(x => x.Rank).ToList().DeepClone();
                    }
                    dungeon.Classes.Tanks = dungeon.Classes.Tanks.OrderBy(x => x.Rank).ToList().DeepClone();
                }

            }
            ///////////////////////////////////////ADD RANKS////////////////////////////////////////
            /// OVERALL CLASSES
            foreach (var affixCombo in Output.Affixes)
            {

                ///////////////////////////////////////  DPS  //////////////////////////////////
                int dpsrank = 1;
                foreach (var Class in affixCombo.Classes.DPSs.OrderByDescending(x => x.Score))
                {
                    Class.Rank = dpsrank;
                    dpsrank++;
                    int specRank = 1;
                    foreach (var spec in Class.Specs.OrderByDescending(x => x.Score))
                    {
                        InsertSpecStatsIntoOtherLists(spec, Output);
                        spec.Rank = specRank.DeepClone();
                        specRank++;
                    }
                    Class.Specs = Class.Specs.OrderBy(x => x.Rank).ToList().DeepClone();
                }
                ///////////////////////////////////////  Healer  //////////////////////////////////
                int Healerrank = 1;
                foreach (var Class in affixCombo.Classes.Healers.OrderByDescending(x => x.Score))
                {
                    Class.Rank = Healerrank.DeepClone();
                    Healerrank++;
                    int specRank = 1;
                    foreach (var spec in Class.Specs.OrderByDescending(x => x.Score))
                    {
                        InsertSpecStatsIntoOtherLists(spec, Output);
                        spec.Rank = specRank.DeepClone();
                        specRank++;
                    }
                    Class.Specs = Class.Specs.OrderBy(x => x.Rank).ToList().DeepClone();
                }
                ///////////////////////////////////////  Tank  //////////////////////////////////
                int Tankrank = 1;
                foreach (var Class in affixCombo.Classes.Tanks.OrderByDescending(x => x.Score))
                {
                    Class.Rank = Tankrank.DeepClone();
                    Tankrank++;
                    int specRank = 1;
                    foreach (var spec in Class.Specs.OrderByDescending(x => x.Score))
                    {
                        InsertSpecStatsIntoOtherLists(spec, Output);
                        spec.Rank = specRank.DeepClone();
                        specRank++;
                    }
                    Class.Specs = Class.Specs.OrderBy(x => x.Rank).ToList().DeepClone();
                }
                affixCombo.Classes.DPSs = affixCombo.Classes.DPSs.OrderBy(x => x.Rank).ToList().DeepClone();
                affixCombo.Classes.Healers = affixCombo.Classes.Healers.OrderBy(x => x.Rank).ToList().DeepClone();
                affixCombo.Classes.Tanks = affixCombo.Classes.Tanks.OrderBy(x => x.Rank).ToList().DeepClone();

            }


            //var cacheData = (JsonConvert.SerializeObject(Output, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));

            /// REMOVE RUN ITEMS 
            ////////////////////////////////////////////////////
            foreach (var item in Output.Affixes)
            {
                foreach (var dungeon in item.Dungeons.Where(x => x.RunsItems != null))
                {
                    dungeon.RunsItems = null;
                }
            }
            ////////////////////////////////////////////////////
            Console.WriteLine("Creating Json string");
            var saveString = (JsonConvert.SerializeObject(Output, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
            Console.WriteLine("Saveing to Json File");
            File.WriteAllText(dataFile, saveString);
            //DFile.WriteAllText(LastCollectdatafile, cacheData);

        }

        private static void InsertSpecStatsIntoOtherLists(SpecInfo spec, RioData output)
        {
            foreach (var affixset in output.Affixes)
            {
                foreach (var Comp in affixset.Compositions.DPSComposition)
                {
                    foreach (var spec2 in Comp.DPSs.Where(x=> x.SpecID == spec.SpecID))
                    {
                        spec2.Score = spec.Score.DeepClone();
                        spec2.Runs = spec.Runs.DeepClone();
                    }
                }
                foreach (var Comp in affixset.Compositions.HealerTankComposition)
                {
                    foreach (var spec2 in Comp.Healer.Where(x => x.SpecID == spec.SpecID))
                    {
                        spec2.Score = spec.Score.DeepClone();
                        spec2.Runs = spec.Runs.DeepClone();
                    }
                    foreach (var spec2 in Comp.Tank.Where(x => x.SpecID == spec.SpecID))
                    {
                        spec2.Score = spec.Score.DeepClone();
                        spec2.Runs = spec.Runs.DeepClone();
                    }
                }
                foreach (var Comp in affixset.Compositions.TeamComposition)
                {
                    foreach (var spec2 in Comp.Tank.Where(x => x.SpecID == spec.SpecID))
                    {
                        spec2.Score = spec.Score.DeepClone();
                        spec2.Runs = spec.Runs.DeepClone();
                    }
                    foreach (var spec2 in Comp.Healer.Where(x => x.SpecID == spec.SpecID))
                    {
                        spec2.Score = spec.Score.DeepClone();
                        spec2.Runs = spec.Runs.DeepClone();
                    }
                    foreach (var spec2 in Comp.DPSs.Where(x => x.SpecID == spec.SpecID))
                    {
                        spec2.Score = spec.Score.DeepClone();
                        spec2.Runs = spec.Runs.DeepClone();
                    }
                }
            }
        }

        private static bool IsLike(List<SpecInfo> ListofSpecs1, List<SpecInfo> ListofSpecs2)
        {
            foreach (var spec in ListofSpecs1)
            {
                var list1Spec = ListofSpecs1.Where(x => x.SpecID == spec.SpecID);
                var list2Spec = ListofSpecs2.Where(x => x.SpecID == spec.SpecID);
                if (list1Spec.Count() != list2Spec.Count())
                {
                    return false;
                }
            }
            return true;
        }

        private static List<SpecInfo> getSpecs(Rankedgroup run, string v)
        {
            List<SpecInfo> ls = new List<SpecInfo>();
            foreach (var player in run.run.roster.Where(x => x.role.ToLower() == v.ToLower()))
            {
                SpecInfo spec = new SpecInfo();
                spec.SpecID = player.character.spec.id;
                spec.SpecName = player.character.spec.name;
                spec.SpecSlug = player.character.spec.slug;
                spec.ClassName = player.character.Class.name;
                spec.ClassSlug = player.character.Class.slug;
                spec.ClassID = player.character.Class.id;
                ls.Add(spec.DeepClone());
            }
            return ls;
        }

        public static string dataFile = @"c:\temp\KeystoneData.json";
        public static string LastCollectdatafile = @"c:\temp\LastCollectCache.json";

        public static void GetIntData()
        {
            Uri uri = new Uri("https://raider.io/mythic-plus/season-bfa-1/all/world/leaderboards");


            ScrapingBrowser Browser = new ScrapingBrowser();
            var PageResult = Browser.NavigateToPage(uri);
            HtmlNode rawHTML = PageResult.Html;
            var data = rawHTML.ChildNodes[2].ChildNodes[3].ChildNodes[3].ChildNodes[0].InnerHtml.Replace("\n    window.__RIO = ", "").Replace("};", "}");

            var splitWord = "window.__RIO =";
            var _data = data.Split(new string[] { splitWord }, StringSplitOptions.None)[1];
            var result = JsonConvert.DeserializeObject<IntData.Rootobject>(_data);
            ApiRequests++;
            season = result.currentSeasonSlug;
            ExpID = result.currentExpansionId;

            dungeons = new List<string>();
            foreach (var dungeon in result.initialData.siteMeta.dungeons)
            {
                if (dungeon.expansion_id == ExpID)
                {
                    dungeons.Add(dungeon.slug.DeepClone());
                }
            }


            foreach (var affixSet in result.initialData.siteMeta.dungeonAffixSchedule)
            {
                var ls = new List<Weekly_Modifiers>();
                foreach (var item in affixSet.affixes)
                {
                    var p = new Weekly_Modifiers();
                    p.icon = item.icon;
                    p.id = item.id;
                    p.name = item.name;
                    ls.Add(p.DeepClone());
                }
                if(ls.Exists(x=> x.name.ToLower() == "reaping"))AffixCombos.Add(ls.DeepClone());

                var s = new _Affixes();
                foreach (var item in affixSet.affixes)
                {

                    s.Affixes = new List<Weekly_Modifiers>();
                    var p = new Weekly_Modifiers();
                    p.icon = item.icon;
                    p.id = item.id;
                    s.slug = affixSet.slug;
                    p.name = item.name;
                    s.Affixes.Add(p.DeepClone());
                }
                if(s.slug.Contains("reaping"))_AffixesLS.Add(s.DeepClone());
            }
        }

        public static RioData oldData = new RioData() { Season = season, Affixes = new List<_Affixes>() };
        public static string HTTPGET(string request)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    ApiRequests++;
                    return result;
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Bad Request"))
                    {
                        throw ex;
                    }
                    return null;
                }
            };


        }
        public static List<string> dungeons = new List<string>()
        {
            "ataldazar",
            "freehold",
            "kings-rest",
            "shrine-of-the-storm",
            "siege-of-boralus",
            "temple-of-sethraliss",
            "the-motherlode",
            "the-underrot",
            "tol-dagor",
            "waycrest-manor"
        };
        //static double resultsPerItem = 4900;
        public static string WeeklySlug = null;
        public static string GetWeeklySlug()
        {
            if (WeeklySlug == null)
            {
                var html = HTTPGET(@"https://raider.io/api/v1/mythic-plus/affixes?region=us&locale=en");
                var result = JsonConvert.DeserializeObject<RioAffixGet.Rootobject>(html);
                WeeklySlug = result.title.Replace(", ", "-").ToLower();
                return WeeklySlug;
            }
            else
            {
                return WeeklySlug;
            }

        }

        public static void Combine(Rootobject inn, Rootobject ut)
        {
            foreach (var item in inn.rankings.rankedGroups)
            {
                ut.rankings.rankedGroupsList.Add(item.DeepClone());
            }
        }
        public static int ApiRequests = 0;
        public static string region = "world";
        public static void FillData()
        {

            //CheckExisting data
            if (File.Exists(dataFile))
            {
                oldData = JsonConvert.DeserializeObject<RioData>(File.ReadAllText(dataFile));
            }

            //250
            var now = DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Wednesday)
            {
            }



            int affixsetIndex = 0;
            foreach (var affixSet in _AffixesLS.Where(X=> X.slug.Contains("reaping")))
            {
                var pages = 250;
                //resultsPerItem = (pages * 20);

                Console.WriteLine(affixSet.slug);
                //Checking if can be skipped
                //Exist in old data?
                affixsetIndex++;
                if (oldData.Affixes.Exists(x => GetAffixSlug(x) == affixSet.slug))
                {
                    //is not this weeks affix?
                    if (GetWeeklySlug() != affixSet.slug)
                    {
                        Console.WriteLine($@"Skipping {affixSet.slug} - No new data");
                        skipList.Add(affixSet.slug.DeepClone());
                        Console.WriteLine("\n");
                        continue;
                    }
                    else
                    {
                        oldData.Affixes.RemoveAll(x => GetAffixSlug(x) == affixSet.slug);
                    }

                }

                Console.WriteLine("\n");
                if (GetWeeklySlug() == affixSet.slug && pages !=  1)
                {
                    //Get last page to reduce max requests
                    Console.WriteLine($@" {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - Checking what's the smallest last page" + Environment.NewLine);
                    int dungeonIndex2 = 0;

                    foreach (var dungeon in dungeons)
                    {
                        dungeonIndex2++;
                        try
                        {
                            var result = JsonConvert.DeserializeObject<Rootobject>(HTTPGET($@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=0"));
                            Console.WriteLine($@"[{ApiRequests}] {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - {dungeon}({dungeonIndex2}/{dungeons.Count}) - Last page: {result.rankings.ui.lastPage}");
                            if (pages > result.rankings.ui.lastPage)
                            {
                                pages = result.rankings.ui.lastPage;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            try
                            {
                                var result = JsonConvert.DeserializeObject<Rootobject>(HTTPGET($@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=0"));
                                Console.WriteLine($@"[{ApiRequests}] {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - {dungeon}({dungeonIndex2}/{dungeons.Count}) - Last page: {result.rankings.ui.lastPage} - Retry success");
                                if (pages > result.rankings.ui.lastPage)
                                {
                                    pages = result.rankings.ui.lastPage;
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("skipping");
                            }
                        }

                    }

                    Console.WriteLine("\n");
                    Console.WriteLine($@"[{ApiRequests}] {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - Max caching {pages} pages");
                    Console.WriteLine("\n");

                }
                int dungeonIndex = 0;

                foreach (var dungeon in dungeons.OrderByDescending(x => x))
                {
                    dungeonIndex++;
                    int pageIndex = 0;
                    Rootobject result = new Rootobject() { rankings = new Rankings() { rankedGroupsList = new List<Rankedgroup>() } };
                    //Parallel.For(799, pages,
                    //i =>
                    for (int i = 0; i < pages; i++)
                    {
                        try
                        {
                            if (result == null)
                            {
                                var s = $@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=" + i;
                                result = JsonConvert.DeserializeObject<Rootobject>(HTTPGET($@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=" + i));
                                result.rankings.rankedGroupsList = result.rankings.rankedGroups.ToList();
                            }
                            else
                            {
                                var s = $@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=" + i;
                                var result2 = JsonConvert.DeserializeObject<Rootobject>(HTTPGET($@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=" + i));
                                if (result2.rankings.rankedGroups.Count() == 0 || result2.rankings.rankedGroups[0].run.mythic_level < 6)
                                {
                                    pages = i;
                                    Console.WriteLine($@"[{ApiRequests}] {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - {dungeon}({dungeonIndex}/{dungeons.Count}) - Less than 3 affixs, going to next item");
                                    break;
                                }
                                Combine(result2, result);
                            }
                            pageIndex++;
                            Console.WriteLine($@"[{ApiRequests}] {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - {dungeon}({dungeonIndex}/{dungeons.Count}) - Page({pageIndex}/{pages})");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("- Trying again...");

                            try
                            {
                                if (result == null)
                                {
                                    result = JsonConvert.DeserializeObject<Rootobject>(HTTPGET($@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=" + i));
                                    result.rankings.rankedGroupsList = result.rankings.rankedGroups.ToList();
                                }
                                else
                                {
                                    var result2 = JsonConvert.DeserializeObject<Rootobject>(HTTPGET($@"https://raider.io/api/mythic-plus/rankings/runs?season={season}&dungeon={dungeon}&region={region}&strict=true&affixes={affixSet.slug}&page=" + i));
                                    if (result2.rankings.rankedGroups.Count() == 0 )
                                    {
                                        pages = i;
                                        Console.WriteLine($@"[{ApiRequests}] {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - {dungeon}({dungeonIndex}/{dungeons.Count}) - Less than 3 affixs, going to next item");
                                        break;
                                    }
                                    if (result2.rankings.rankedGroups[0].run.num_modifiers_active < 3)
                                    {
                                        pages = i;
                                        Console.WriteLine($@"[{ApiRequests}] {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - {dungeon}({dungeonIndex}/{dungeons.Count}) - Keystone is lower than  + 10, going to next item");
                                        break;
                                    }
                                    Combine(result2, result);
                                }
                                pageIndex++;
                                Console.WriteLine($@" {affixSet.slug}({affixsetIndex}/{_AffixesLS.Count}) - {dungeon}({dungeonIndex}/{dungeons.Count}) - Page({pageIndex}/{pages}) - Retry Success");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("- Retry failed, skipping page");
                                Console.WriteLine(ex.Message);

                            }
                        }
                    }
                    var itemsInResult = result.rankings.rankedGroupsList.Where(x => x != null && x.run != null && x.run.num_chests != null && x.run.num_chests != 0);
                    foreach (var run in itemsInResult)
                    {
                        run.dungeonstring = dungeon;
                        TempDung.Add(run.DeepClone());
                    }
                    Console.WriteLine("\n");

                }
            }
        }

        private static string GetAffixSlug(_Affixes affixSet)
        {
            string result = "";
            foreach (var item in affixSet.Affixes)
            {
                if (result == "") result = item.name.ToLower();
                else result = result + "-" + item.name.ToLower();
            }
            return result;
        }
        private static string GetAffixSlug(List<Weekly_Modifiers> affixSet)
        {
            string result = "";
            foreach (var item in affixSet)
            {
                if (result == "") result = item.name.ToLower();
                else result = result + "-" + item.name.ToLower();
            }
            return result;
        }

        /*
public void SetJson()
{
   textBox1.Text = "";
   foreach (var _item in listBox1.Items)
   {
       var l = _item.ToString().Split('-');
       var affix = AffixCombos.Single(x => x.weekly_modifiers[0].name == l[0] && x.weekly_modifiers[1].name == l[1] && x.weekly_modifiers[2].name == l[2]);

       textBox1.Text = textBox1.Text + $@"local {affix.weekly_modifiers[0].name}-{affix.weekly_modifiers[1].name}-{affix.weekly_modifiers[2].name} = {{";
       foreach (var item in affix.DungeonRanks.OrderByDescending(x => x.Top100Ranks))
       {
           textBox1.Text = textBox1.Text + $@"""{item.DungeonName}"",";
       }
       textBox1.Text = textBox1.Text + "}\n";
       textBox1.Text = textBox1.Text.Replace(",}", "}");
   }
}*/
    }
}

