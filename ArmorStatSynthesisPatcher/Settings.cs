using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace ArmorStatSynthesisPatcher
{
    public enum PluginFilter
    {
        [SettingName("All Plugins")]
        [Tooltip("Process all plugins in the load order")]
        AllPlugins,

        [SettingName("Exclude Plugins")]
        [Tooltip("Process all plugins except those in the exclude list")]
        ExcludePlugins,

        [SettingName("Include Plugins")]
        [Tooltip("Only process plugins in the include list")]
        IncludePlugins
    }

    public enum ArmorMatchBehavior
    {
        [SettingName("Modify Armor")]
        ModifyArmor,

        [SettingName("Deactivate Rule")]
        DeactivateRule,

        [SettingName("Pass")]
        Pass
    }

    public class ArmorCategory
    {
        [JsonProperty]
        [SettingName("Armor (add more by adding panes)")]
        public Dictionary<string, ArmorType> Armor { get; set; } = new();
    }

    public enum ArmorTypeEnum
    {
        LightArmor,
        HeavyArmor,
        Clothing
    }

    public class ArmorType
    {
        [SettingName("Match Behavior")]
        [Tooltip("How this armor type should be processed")]
        public ArmorMatchBehavior MatchBehavior { get; set; } = ArmorMatchBehavior.ModifyArmor;

        [JsonProperty]
        [SettingName("Identifiers (semicolon separated)")]
        public string Identifiers { get; set; } = "";

        public ArmorTypeEnum Type { get; set; }

        [JsonProperty]
        [SettingName("Armor Piece")]
        public Dictionary<string, ArmorPiece> Armor { get; set; } = new();
    }

    public class ArmorPiece
    {
        [SettingName("Identifiers (semicolon separated)")]
        [JsonProperty]
        public string Identifiers { get; set; } = "";

        [SettingName("Rating")]
        [JsonProperty]
        public float Rating { get; set; } = 0;

        [JsonProperty]
        [SettingName("Weight")]
        public float Weight { get; set; } = 0;
    }

    public class Settings
    {
        public Settings()
        {
            TryLoadBundledDefaults();
        }

        private void TryLoadBundledDefaults()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return;
            }

            var defaultPath = Path.Combine(assemblyDirectory, "Data", "settings.json");
            if (!File.Exists(defaultPath))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(defaultPath);
                JsonConvert.PopulateObject(
                    json,
                    this,
                    new JsonSerializerSettings
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });
            }
            catch
            {
                // Keep CLR defaults if bundled settings cannot be parsed.
            }
        }

        [SettingName("Debug Mode")]
        [Tooltip("Enable debug logging")]
        public bool DebugMode { get; set; }

        [SettingName("Plugin Processing")]
        [Tooltip("Choose which plugins to process\nAll Plugins: Process all plugins in the load order\nExclude Plugins: Process all plugins except those in the exclude list\nInclude Plugins: Only process plugins in the include list")]
        public PluginFilter PluginFilter { get; set; }

        [SettingName("Plugin Include List")]
        [Tooltip("Plugins to process (semicolon separated)")]
        public string PluginIncludeList { get; set; } = "";

        [SettingName("Plugin Exclude List")]
        [Tooltip("Plugins to skip (semicolon separated)")]
        public string PluginExcludeList { get; set; } = "";

        [SettingName("Ignored Armor Form Keys")]
        [Tooltip("Skip armors with these form keys (semicolon separated)")]
        public string IgnoredArmorFormKeys { get; set; } = "";

        [SettingName("Modify Ratings")]
        [Tooltip("Modify armor ratings")]
        public bool ModifyRatings { get; set; }

        [SettingName("Modify Weights")]
        [Tooltip("Modify armor weights")]
        public bool ModifyWeights { get; set; }

        [SettingName("Remove rating for armor pieces without armor keywords")]
        [Tooltip("Remove rating from parsed armor pieces that do not have any armor keywords. This is useful for removing rating from clothing items.")]
        public bool RemoveRatingForUnknownArmorPieces { get; set; }

        [SettingName("Remove weight for armor pieces without armor keywords")]
        [Tooltip("Remove weight from parsed armor pieces that do not have any armor keywords. This is useful for removing weight from clothing items.")]
        public bool RemoveWeightForUnknownArmorPieces { get; set; }

        [SettingName("Armor piece keywords")]
        [Tooltip("Armor piece keywords to use when removing rating/weight (semicolon separated). If the armor piece does not have any of these keywords, its rating/weight will be removed.")]
        public string ArmorPieceKeywords { get; set; } = "";

        [SettingName("Remove rating for armor pieces without the correct armor slots")]
        [Tooltip("Remove rating from parsed armor pieces that do not have any armor slots. This is useful for removing rating from clothing items.")]
        public bool RemoveRatingForUnknownArmorSlots { get; set; }

        [SettingName("Remove weight for armor pieces without the correct armor slots")]
        [Tooltip("Remove weight from parsed armor pieces that do not have any armor slots. This is useful for removing weight from clothing items.")]
        public bool RemoveWeightForUnknownArmorSlots { get; set; }

        [SettingName("Armor slots")]
        [Tooltip("Armor slots to use when removing rating/weight (semicolon separated). If the armor piece does not have any of these slots, its rating/weight will be removed.")]
        public string ArmorSlots { get; set; } = "";

        [JsonProperty]
        [SettingName("Vanilla Light Armors")]
        public ArmorCategory VanillaLightArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Dawnguard Light Armors")]
        public ArmorCategory DawnguardLightArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Dragonborn Light Armors")]
        public ArmorCategory DragonbornLightArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Creation Club Light Armors")]
        public ArmorCategory CreationClubLightArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Unique Light Armors")]
        public ArmorCategory UniqueLightArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Other Light Armors")]
        public ArmorCategory OtherLightArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Vanilla Heavy Armors")]
        public ArmorCategory VanillaHeavyArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Dawnguard Heavy Armors")]
        public ArmorCategory DawnguardHeavyArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Dragonborn Heavy Armors")]
        public ArmorCategory DragonbornHeavyArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Creation Club Heavy Armors")]
        public ArmorCategory CreationClubHeavyArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Unique Heavy Armors")]
        public ArmorCategory UniqueHeavyArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Other Heavy Armors")]
        public ArmorCategory OtherHeavyArmors { get; set; } = new ArmorCategory();

        [JsonProperty]
        [SettingName("Clothing Armors")]
        public ArmorCategory ClothingArmors { get; set; } = new ArmorCategory();

        public IEnumerable<ArmorType> GetAllLightArmors()
        {
            foreach (var category in new[] { VanillaLightArmors, DawnguardLightArmors, DragonbornLightArmors, CreationClubLightArmors, OtherLightArmors, UniqueLightArmors })
            {
                foreach (var armor in category.Armor.Values)
                {
                    if (armor.MatchBehavior != ArmorMatchBehavior.DeactivateRule)
                    {
                        yield return armor;
                    }
                }
            }
        }

        public IEnumerable<ArmorType> GetAllHeavyArmors()
        {
            foreach (var category in new[] { VanillaHeavyArmors, DawnguardHeavyArmors, DragonbornHeavyArmors, CreationClubHeavyArmors, UniqueHeavyArmors, OtherHeavyArmors })
            {
                foreach (var armor in category.Armor.Values)
                {
                    if (armor.MatchBehavior != ArmorMatchBehavior.DeactivateRule)
                    {
                        yield return armor;
                    }
                }
            }
        }

        public IEnumerable<ArmorType> GetAllClothing()
        {
            foreach (var armor in ClothingArmors.Armor.Values)
            {
                if (armor.MatchBehavior != ArmorMatchBehavior.DeactivateRule)
                {
                    yield return armor;
                }
            }
        }
    }
}
