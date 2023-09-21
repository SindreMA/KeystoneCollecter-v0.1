using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeystoneCollecter
{

        public class Mythic_Plus_Best_Runs
        {
            public string player { get; set; }
            public string realm { get; set; }

            public string dungeon { get; set; }
            public string short_name { get; set; }
            public int mythic_level { get; set; }
            public DateTime completed_at { get; set; }
            public int clear_time_ms { get; set; }
            public int num_keystone_upgrades { get; set; }
            public double score { get; set; }
            public string url { get; set; }
            public int val1 { get; set; }
            public string val2 { get; set; }
            public string val3 { get; set; }
            public string val4 { get; set; }
            public string val5 { get; set; }
        }
        public class DungeonRankings
        {
            public int Rank { get; set; }
            public int Top100Ranks { get; set; }
            public string DungeonName { get; set; }
        }
        public class Affixdata
        {
            public Weekly_Modifiers[] weekly_modifiers { get; set; }
            public List<DungeonRankings> DungeonRanks { get; set; }
        }
        public class Rootobject
        {
            public Rankings rankings { get; set; }
        }

        public class Rankings
        {
            public Rankedgroup[] rankedGroups { get; set; }
            public Ui ui { get; set; }
            public List<Rankedgroup> rankedGroupsList { get; set; }
            public Region region { get; set; }
            public Dungeon dungeon { get; set; }
        }

        public class Ui
        {
            public string season { get; set; }
            public string dungeon { get; set; }
            public string region { get; set; }
            public bool strict { get; set; }
            public int page { get; set; }
            public int limit { get; set; }
            public int minMythicLevel { get; set; }
            public int maxMythicLevel { get; set; }
            public int eventId { get; set; }
            public int lastPage { get; set; }
        }

        public class Region
        {
            public string name { get; set; }
            public string short_name { get; set; }
            public string slug { get; set; }
        }

        public class Dungeon
        {
            public int id { get; set; }
            public string name { get; set; }
            public string short_name { get; set; }
            public string slug { get; set; }
            public int keystone_timer_ms { get; set; }
        }

        public class Rankedgroup
        {
            public int rank { get; set; }
            public float score { get; set; }
            public Run run { get; set; }
        public string dungeonstring { get; set; }
    }

        public class Run
        {
            public string season { get; set; }
            public Dungeon1 dungeon { get; set; }
            public int keystone_run_id { get; set; }
            public int keystone_team_id { get; set; }
            public object keystone_platoon_id { get; set; }
            public int mythic_level { get; set; }
            public int clear_time_ms { get; set; }
            public int keystone_time_ms { get; set; }
            public DateTime completed_at { get; set; }
            public int num_chests { get; set; }
            public int time_remaining_ms { get; set; }
            public string faction { get; set; }
            public Weekly_Modifiers[] weekly_modifiers { get; set; }
            public int num_modifiers_active { get; set; }
            public Roster[] roster { get; set; }
            public object platoon { get; set; }
        }

        public class Dungeon1
        {
            public int id { get; set; }
            public string name { get; set; }
            public string short_name { get; set; }
            public string slug { get; set; }
            public int keystone_timer_ms { get; set; }
        }

        public class Weekly_Modifiers
        {
            public int id { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public string description { get; set; }

        }

        public class Roster
        {
            public Character character { get; set; }
            public Oldcharacter oldCharacter { get; set; }
            public string role { get; set; }
        }

        public class Character
        {
            public int id { get; set; }
            public int persona_id { get; set; }
            public string name { get; set; }
            public Class1 Class { get; set; }
            public Race race { get; set; }
            public string faction { get; set; }
            public Spec spec { get; set; }
            public string path { get; set; }
            public Realm realm { get; set; }
            public Region1 region { get; set; }
        }

        public class Class1
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        public List<Spec> Specs { get; set; }

    }

        public class Race
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Spec
        {
            public string name { get; set; }
            public string slug { get; set; }
            public int id { get; set; }
        }

        public class Realm
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string altSlug { get; set; }
            public string locale { get; set; }
            public bool isConnected { get; set; }
        }

        public class Region1
        {
            public string name { get; set; }
            public string slug { get; set; }
            public string short_name { get; set; }
        }

        public class Oldcharacter
        {
            public int id { get; set; }
            public int persona_id { get; set; }
            public string name { get; set; }
            public Class2 _class { get; set; }
            public Race1 race { get; set; }
            public string faction { get; set; }
            public Spec1 spec { get; set; }
            public string path { get; set; }
            public Realm1 realm { get; set; }
            public Region2 region { get; set; }
        }

        public class Class2
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Race1
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Spec1
        {
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Realm1
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string altSlug { get; set; }
            public string locale { get; set; }
            public bool isConnected { get; set; }
        }

        public class Region2
        {
            public string name { get; set; }
            public string slug { get; set; }
            public string short_name { get; set; }
        }


    public class IntData
    {
        public class Rootobject
        {
            public Initialdata initialData { get; set; }
            public bool maintenance { get; set; }
            public string assetsBaseUri { get; set; }
            public Assetmanifest assetManifest { get; set; }
            public string currentRaidSlug { get; set; }
            public string currentSeasonSlug { get; set; }
            public int currentExpansionId { get; set; }
            public string currentRaidDifficulty { get; set; }
            public Currentfeatures currentFeatures { get; set; }
        }

        public class Initialdata
        {
            public Sitemeta siteMeta { get; set; }
            public Config config { get; set; }
            public object auth { get; set; }
        }

        public class Sitemeta
        {
            public Region[] regions { get; set; }
            public Subregion[] subRegions { get; set; }
            public Race[] races { get; set; }
            public Class1[] classes { get; set; }
            public Spec[] specs { get; set; }
            public Raid[] raids { get; set; }
            public Dungeon[] dungeons { get; set; }
            public Affixschedules affixSchedules { get; set; }
            public Dungeonaffixschedule[] dungeonAffixSchedule { get; set; }
            public Season[] seasons { get; set; }
            public Streamers streamers { get; set; }
        }

        public class Affixschedules
        {
            public _6[] _6 { get; set; }
            public _7[] _7 { get; set; }
        }

        public class _6
        {
            public string name { get; set; }
            public string slug { get; set; }
            public Affix[] affixes { get; set; }
        }

        public class Affix
        {
            public int id { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
        }

        public class _7
        {
            public string name { get; set; }
            public string slug { get; set; }
            public Affix1[] affixes { get; set; }
        }

        public class Affix1
        {
            public int id { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
        }

        public class Streamers
        {
            public int count { get; set; }
            public Features features { get; set; }
        }

        public class Features
        {
            public Domain domain { get; set; }
        }

        public class Domain
        {
            public object domain { get; set; }
            public _Events _events { get; set; }
            public int _eventsCount { get; set; }
            public object[] members { get; set; }
        }

        public class _Events
        {
        }

        public class Region
        {
            public string name { get; set; }
            public string slug { get; set; }
            public string short_name { get; set; }
        }

        public class Subregion
        {
            public string name { get; set; }
            public string slug { get; set; }
            public string short_name { get; set; }
        }

        public class Race
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string faction { get; set; }
        }

        public class Class1
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Spec
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public int class_id { get; set; }
            public string role { get; set; }
            public bool is_melee { get; set; }
        }

        public class Raid
        {
            public string difficulty { get; set; }
            public string name { get; set; }
            public string short_name { get; set; }
            public string slug { get; set; }
            public bool can_show_raid_mythic_details { get; set; }
            public bool can_show_raid_heroic_details { get; set; }
            public bool can_show_raid_normal_details { get; set; }
            public int expansion_id { get; set; }
            public Encounter[] encounters { get; set; }
        }

        public class Encounter
        {
            public int encounterId { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public int ordinal { get; set; }
            public int wingId { get; set; }
            public string iconUrl { get; set; }
        }

        public class Dungeon
        {
            public int id { get; set; }
            public string name { get; set; }
            public string short_name { get; set; }
            public string slug { get; set; }
            public int expansion_id { get; set; }
            public int keystone_timer_ms { get; set; }
        }

        public class Dungeonaffixschedule
        {
            public string name { get; set; }
            public string slug { get; set; }
            public Affix2[] affixes { get; set; }
        }

        public class Affix2
        {
            public int id { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
        }

        public class Season
        {
            public string slug { get; set; }
            public string name { get; set; }
            public string short_name { get; set; }
        }

        public class Config
        {
            public string assetsBaseUri { get; set; }
        }

        public class Assetmanifest
        {
            public string SalesforceSansLightwoff2 { get; set; }
            public string SalesforceSansLightwoff { get; set; }
            public string SalesforceSansLightItalicwoff2 { get; set; }
            public string SalesforceSansLightItalicwoff { get; set; }
            public string SalesforceSansRegularwoff2 { get; set; }
            public string SalesforceSansRegularwoff { get; set; }
            public string SalesforceSansItalicwoff2 { get; set; }
            public string SalesforceSansItalicwoff { get; set; }
            public string SalesforceSansBoldwoff2 { get; set; }
            public string SalesforceSansBoldwoff { get; set; }
            public string SalesforceSansBoldItalicwoff2 { get; set; }
            public string SalesforceSansBoldItalicwoff { get; set; }
            public string faregular400eot { get; set; }
            public string faregular400woff2 { get; set; }
            public string faregular400woff { get; set; }
            public string faregular400ttf { get; set; }
            public string faregular400svg { get; set; }
            public string falight300eot { get; set; }
            public string falight300woff2 { get; set; }
            public string falight300woff { get; set; }
            public string falight300ttf { get; set; }
            public string falight300svg { get; set; }
            public string fasolid900eot { get; set; }
            public string fasolid900woff2 { get; set; }
            public string fasolid900woff { get; set; }
            public string fasolid900ttf { get; set; }
            public string fasolid900svg { get; set; }
            public string fabrands400eot { get; set; }
            public string fabrands400woff2 { get; set; }
            public string fabrands400woff { get; set; }
            public string fabrands400ttf { get; set; }
            public string fabrands400svg { get; set; }
            public string masthead_bg_shorterjpg { get; set; }
            public string raiderio_logopng { get; set; }
            public string modalbg2png { get; set; }
            public string mainjs { get; set; }
            public string mainStylesjs { get; set; }
            public string maincss { get; set; }
            public string mainStylescss { get; set; }
            public string mainjsmap { get; set; }
            public string maincssmap { get; set; }
            public string mainStylesjsmap { get; set; }
            public string mainStylescssmap { get; set; }
        }

        public class Currentfeatures
        {
        }
    }
    public class RioAffixGet
    {




        public class Rootobject

        {

            public string region { get; set; }

            public string title { get; set; }

            public string leaderboard_url { get; set; }

            public Affix_Details[] affix_details { get; set; }

        }



        public class Affix_Details

        {

            public int id { get; set; }

            public string name { get; set; }

            public string description { get; set; }

            public string wowhead_url { get; set; }

        }

    }



}


