using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeystoneCollecter
{
    public class _Dungeon
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public Dictionary<int,int> RunsPerLevel { get; set; }
        public int Runs { get; set; }
        public float TotalScore { get; set; }
        public List<Rankedgroup> RunsItems { get; set; }
        public string Slug { get; set; }
        public CompositionInfo Compositions { get; set; }
        public ClassItems Classes { get; set; }
        public List<FactionInfo> Factions { get; set; }
    }
    public class RioData
    {
        public List<_Affixes> Affixes { get; set; }
        public string Season { get; set; }
    }
    public class _Affixes
    {
        public string slug { get; set; }
        public List<Weekly_Modifiers> Affixes { get; set; }
        public List<_Dungeon> Dungeons { get; set; }
        public CompositionInfo Compositions { get; set; }
        public ClassItems Classes { get; set; }
        public List<FactionInfo> Factions { get; set; }



    }
    public class CompositionInfo
    {
        public List<CompositionContainer> TeamComposition { get; set; }
        public List<CompositionContainer> HealerTankComposition { get; set; }
        public List<CompositionContainer> DPSComposition { get; set; }
    }
    public class CompositionContainer
    {
        public float Score { get; set; }
        public int Runs { get; set; }
        public int Rank { get; set; }
        public List<SpecInfo> Tank { get; set; }
        public List<SpecInfo> Healer { get; set; }
        public List<SpecInfo> DPSs { get; set; }

    }
    public class ClassItems
    {
        public List<ClassInfo> Tanks { get; set; }
        public List<ClassInfo> Healers { get; set; }
        public List<ClassInfo> DPSs { get; set; }
    }

    public class ClassInfo
    {
        public float Score { get; set; }
        public int Runs { get; set; }
        public int Rank { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public string ClassSlug { get; set; }
        public List<SpecInfo> Specs { get; set; }
    }
    public class SpecInfo
    {
        public float? Score { get; set; }
        public int? Runs { get; set; }
        public int? SpecID { get; set; }
        public int? Rank { get; set; }
        public string SpecName { get; set; }
        public string SpecSlug { get; set; }
        public string SpecRole { get; set; }
        public string ClassName { get; set; }
        public string ClassSlug { get; set; }
        public int ClassID { get; set; }
    }
    public class FactionInfo
    {
        public float Score { get; set; }
        public int Runs { get; set; }
        public int Rank { get; set; }
        public string Faction { get; set; }
    }
}
