using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System.ComponentModel;
using Mutagen.Bethesda.Synthesis.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ArmourStatsSynthesisPatcher
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
        ModifyArmor,
        DeactivateRule,
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
            VanillaLightArmors = new ArmorCategory();
            DawnguardLightArmors = new ArmorCategory();
            DragonbornLightArmors = new ArmorCategory();
            CreationClubLightArmors = new ArmorCategory();
            UniqueLightArmors = new ArmorCategory();
            OtherLightArmors = new ArmorCategory();
            VanillaHeavyArmors = new ArmorCategory();
            DawnguardHeavyArmors = new ArmorCategory();
            DragonbornHeavyArmors = new ArmorCategory();
            CreationClubHeavyArmors = new ArmorCategory();
            UniqueHeavyArmors = new ArmorCategory();
            OtherHeavyArmors = new ArmorCategory();
            ClothingArmors = new ArmorCategory();
            InitializeVanillaLightArmors();
            InitializeDawnguardLightArmors();
            InitializeDragonbornLightArmors();
            InitializeCreationClubLightArmors();
            InitializeUniqueLightArmors();
            InitializeOtherLightArmors();
            InitializeVanillaHeavyArmors();
            InitializeDawnguardHeavyArmors();
            InitializeDragonbornHeavyArmors();
            InitializeCreationClubHeavyArmors();
            InitializeUniqueHeavyArmors();
            InitializeOtherHeavyArmors();
        }

        private void InitializeVanillaLightArmors()
        {
            var hideArmor = new ArmorType { Identifiers = "ArmorMaterialHide;Hide", Type = ArmorTypeEnum.LightArmor };
            hideArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 20f, Weight = 5.0f };
            hideArmor.Armor["Studded Armor"] = new ArmorPiece { Identifiers = "Studded Armor", Rating = 23f, Weight = 6.0f };
            hideArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 5f, Weight = 1.0f };
            hideArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 5f, Weight = 1.0f };
            hideArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 10f, Weight = 2.0f };
            hideArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 15f, Weight = 4.0f };
            VanillaLightArmors.Armor["Hide"] = hideArmor;

            var wornshroudedArmor = new ArmorType { Identifiers = "Worn Shrouded", Type = ArmorTypeEnum.LightArmor };
            wornshroudedArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 20f, Weight = 6.0f };
            wornshroudedArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 3f, Weight = 2.0f };
            wornshroudedArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 4f, Weight = 2.0f };
            wornshroudedArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 8f, Weight = 2.0f };
            VanillaLightArmors.Armor["Worn Shrouded"] = wornshroudedArmor;

            var stormcloakArmor = new ArmorType { Identifiers = "ArmorMaterialStormcloak;0A6D7F:Skyrim.esm;0A6D7D:Skyrim.esm", Type = ArmorTypeEnum.LightArmor };
            stormcloakArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 21f, Weight = 8.0f };
            stormcloakArmor.Armor["Fur Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 5f, Weight = 2.0f };
            stormcloakArmor.Armor["Fur Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 5f, Weight = 2.0f };
            stormcloakArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 10f, Weight = 2.0f };
            VanillaLightArmors.Armor["Stormcloak"] = stormcloakArmor;

            var furArmor = new ArmorType { Identifiers = "Fur", Type = ArmorTypeEnum.LightArmor };
            furArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 23f, Weight = 6.0f };
            furArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 6f, Weight = 2.0f };
            furArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 6f, Weight = 1.0f };
            furArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 11f, Weight = 1.0f };
            VanillaLightArmors.Armor["Fur"] = furArmor;

            var lightGuardArmor = new ArmorType { Identifiers = "Imperial;ArmorMaterialImperialLight;Guard's", Type = ArmorTypeEnum.LightArmor };
            lightGuardArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 23f, Weight = 6.0f };
            lightGuardArmor.Armor["Studded Armor"] = new ArmorPiece { Identifiers = "Studded Imperial Armor", Rating = 23f, Weight = 6.0f };
            lightGuardArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 6f, Weight = 2.0f };
            lightGuardArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 6f, Weight = 1.0f };
            lightGuardArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 11f, Weight = 2.0f };
            lightGuardArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 19f, Weight = 4.0f };
            VanillaLightArmors.Armor["Guard"] = lightGuardArmor;

            var pentiusArmor = new ArmorType { Identifiers = "ArmorMaterialPenitus", Type = ArmorTypeEnum.LightArmor };
            pentiusArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 23f, Weight = 6.0f };
            pentiusArmor.Armor["Studded Armor"] = new ArmorPiece { Identifiers = "Studded Imperial Armor", Rating = 23f, Weight = 6.0f };
            pentiusArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 6f, Weight = 1.0f };
            pentiusArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 6f, Weight = 1.0f };
            pentiusArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 11f, Weight = 1.0f };
            VanillaLightArmors.Armor["Pentius"] = pentiusArmor;

            var elvenlightArmor = new ArmorType { Identifiers = "Elven Light", Type = ArmorTypeEnum.LightArmor };
            elvenlightArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 26f, Weight = 4.0f };
            elvenlightArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 1.0f };
            elvenlightArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 1.0f };
            elvenlightArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 1.0f };
            VanillaLightArmors.Armor["Elven Light"] = elvenlightArmor;

            var forswornArmor = new ArmorType { Identifiers = "ArmorMaterialForsworn", Type = ArmorTypeEnum.LightArmor };
            forswornArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 26f, Weight = 6.0f };
            forswornArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 2.0f };
            forswornArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 2.0f };
            forswornArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 2.0f };
            VanillaLightArmors.Armor["Forsworn"] = forswornArmor;

            var leatherArmor = new ArmorType { Identifiers = "ArmorMaterialLeather;IAKMaterialLeather", Type = ArmorTypeEnum.LightArmor };
            leatherArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 26f, Weight = 6.0f };
            leatherArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 2.0f };
            leatherArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 2.0f };
            leatherArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 2.0f };
            VanillaLightArmors.Armor["Leather"] = leatherArmor;

            var stormcloakofficerArmor = new ArmorType { Identifiers = "ArmorMaterialBearStormcloak", Type = ArmorTypeEnum.LightArmor };
            stormcloakofficerArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 27f, Weight = 8.0f };
            stormcloakofficerArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 2.0f };
            stormcloakofficerArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 2.0f };
            stormcloakofficerArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 2.0f };
            VanillaLightArmors.Armor["Stormcloak Officer"] = stormcloakofficerArmor;

            var eastmarchArmor = new ArmorType { Identifiers = "Eastmarch", Type = ArmorTypeEnum.LightArmor };
            eastmarchArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 27f, Weight = 8.0f };
            eastmarchArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 2.0f };
            eastmarchArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 2.0f };
            eastmarchArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 2.0f };
            VanillaLightArmors.Armor["Eastmarch"] = eastmarchArmor;

            var elvenArmor = new ArmorType { Identifiers = "ArmorMaterialElven;IAKMaterialElven", Type = ArmorTypeEnum.LightArmor };
            elvenArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 29f, Weight = 4.0f };
            elvenArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 8f, Weight = 1.0f };
            elvenArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 8f, Weight = 1.0f };
            elvenArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 13f, Weight = 1.0f };
            elvenArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 21f, Weight = 4.0f };
            VanillaLightArmors.Armor["Elven"] = elvenArmor;

            var shroudedArmor = new ArmorType { Identifiers = "Shrouded;Templar Assassin", Type = ArmorTypeEnum.LightArmor };
            shroudedArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 29f, Weight = 7.0f };
            shroudedArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 8f, Weight = 2.0f };
            shroudedArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 8f, Weight = 2.0f };
            shroudedArmor.Armor["Armor and Gloves"] = new ArmorPiece { Identifiers = "Armor and Gloves", Rating = 37f, Weight = 9.0f };
            shroudedArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 13f, Weight = 2.0f };
            VanillaLightArmors.Armor["Shrouded"] = shroudedArmor;

            var thievesguildArmor = new ArmorType { Identifiers = "ArmorMaterialThievesGuild;Master Thief", Type = ArmorTypeEnum.LightArmor };
            thievesguildArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 29f, Weight = 7.0f };
            thievesguildArmor.Armor["Improved Armor"] = new ArmorPiece { Identifiers = "0D3ACC:Skyrim.esm", Rating = 30f, Weight = 6.0f };
            thievesguildArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 9f, Weight = 1.5f };
            thievesguildArmor.Armor["Improved Boots"] = new ArmorPiece { Identifiers = "0D3ACB:Skyrim.esm", Rating = 10f, Weight = 1.0f };
            thievesguildArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 9f, Weight = 1.0f };
            thievesguildArmor.Armor["Improved Gauntlets"] = new ArmorPiece { Identifiers = "0D3ACD:Skyrim.esm", Rating = 10f, Weight = 1.0f };
            thievesguildArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 13f, Weight = 1.5f };
            thievesguildArmor.Armor["Improved Helmet"] = new ArmorPiece { Identifiers = "0D3ACE:Skyrim.esm", Rating = 15f, Weight = 1.0f };
            VanillaLightArmors.Armor["Thieves Guild"] = thievesguildArmor;

            var scaledArmor = new ArmorType { Identifiers = "ArmorMaterialScaled;IAKMaterialAdvancedScale", Type = ArmorTypeEnum.LightArmor };
            scaledArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 32f, Weight = 6.0f };
            scaledArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 9f, Weight = 2.0f };
            scaledArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 9f, Weight = 2.0f };
            scaledArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 14f, Weight = 2.0f };
            scaledArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 19f, Weight = 6.0f };
            VanillaLightArmors.Armor["Scaled"] = scaledArmor;

            var ancientshroudedArmor = new ArmorType { Identifiers = "Ancient Shrouded", Type = ArmorTypeEnum.LightArmor };
            ancientshroudedArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 33f, Weight = 5.0f };
            ancientshroudedArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 12f, Weight = 0.5f };
            ancientshroudedArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 12f, Weight = 1.0f };
            ancientshroudedArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15f, Weight = 1.0f };
            VanillaLightArmors.Armor["Ancient Shrouded"] = ancientshroudedArmor;

            var gildedElvenArmor = new ArmorType { Identifiers = "ArmorMaterialElvenGilded", Type = ArmorTypeEnum.LightArmor };
            gildedElvenArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 35f, Weight = 4.0f };
            VanillaLightArmors.Armor["Gilded Elven"] = gildedElvenArmor;

            var elvenHunterArmor = new ArmorType { Identifiers = "Elven Hunter", Type = ArmorTypeEnum.LightArmor };
            elvenHunterArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 31f, Weight = 4.0f };
            elvenHunterArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 1.0f };
            elvenHunterArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 1.0f };
            VanillaLightArmors.Armor["Elven Hunter"] = elvenHunterArmor;

            var glassArmor = new ArmorType { Identifiers = "ArmorMaterialGlass;IAKMaterialGlass", Type = ArmorTypeEnum.LightArmor };
            glassArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 38f, Weight = 7.0f };
            glassArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            glassArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            glassArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 2.0f };
            glassArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 27f, Weight = 6.0f };
            VanillaLightArmors.Armor["Glass"] = glassArmor;

            var guildmastersArmor = new ArmorType { Identifiers = "ArmorMaterialThievesGuildLeader", Type = ArmorTypeEnum.LightArmor };
            guildmastersArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 38f, Weight = 10.0f };
            guildmastersArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            guildmastersArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            guildmastersArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 3.0f };
            VanillaLightArmors.Armor["Guild Master's"] = guildmastersArmor;

            var dragonscaleArmor = new ArmorType { Identifiers = "ArmorMaterialDragonscale;IAKMaterialDragonScale", Type = ArmorTypeEnum.LightArmor };
            dragonscaleArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 41f, Weight = 10.0f };
            dragonscaleArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 12f, Weight = 3.0f };
            dragonscaleArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 12f, Weight = 3.0f };
            dragonscaleArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 17f, Weight = 4.0f };
            dragonscaleArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 29f, Weight = 6.0f };
            VanillaLightArmors.Armor["Dragonscale"] = dragonscaleArmor;
        }

        private void InitializeDawnguardLightArmors()
        {
            var dawnguardArmor = new ArmorType { Identifiers = "DLC1ArmorMaterialDawnguard", Type = ArmorTypeEnum.LightArmor };
            dawnguardArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 29f, Weight = 6.0f };
            dawnguardArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 8f, Weight = 1.5f };
            dawnguardArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 8f, Weight = 1.5f };
            dawnguardArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 1.5f };
            dawnguardArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 6.0f };
            DawnguardLightArmors.Armor["Dawnguard"] = dawnguardArmor;

            var vampireArmor = new ArmorType { Identifiers = "Vampire", Type = ArmorTypeEnum.LightArmor };
            vampireArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 25f, Weight = 5.0f };
            vampireArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 8f, Weight = 1.5f };
            vampireArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 2.0f };
            DawnguardLightArmors.Armor["Vampire"] = vampireArmor;

            var ancientfalmerArmor = new ArmorType { Identifiers = "Ancient Falmer;Curate's", Type = ArmorTypeEnum.LightArmor };
            ancientfalmerArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 38f, Weight = 7.0f };
            ancientfalmerArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            ancientfalmerArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            ancientfalmerArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 2.0f };
            DawnguardLightArmors.Armor["Ancient Falmer"] = ancientfalmerArmor;
        }

        private void InitializeDragonbornLightArmors()
        {
            var skaalArmor = new ArmorType { Identifiers = "Skaal", Type = ArmorTypeEnum.LightArmor };
            skaalArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 26f, Weight = 5.0f };
            skaalArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 1.5f };
            skaalArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 1.5f };
            skaalArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 6f, Weight = 1.5f };
            DragonbornLightArmors.Armor["Skaal"] = skaalArmor;

            var moragtongArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialMoragTong", Type = ArmorTypeEnum.LightArmor };
            moragtongArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 26f, Weight = 5.0f };
            moragtongArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 8f, Weight = 2.0f };
            moragtongArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 2.0f };
            moragtongArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 1.5f };
            DragonbornLightArmors.Armor["Morag Tong"] = moragtongArmor;

            var chitinArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialChitinLight;WAF_MaterialChitin", Type = ArmorTypeEnum.LightArmor };
            chitinArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 30f, Weight = 4.0f };
            chitinArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 8.5f, Weight = 1.0f };
            chitinArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 8.5f, Weight = 2.0f };
            chitinArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 13.5f, Weight = 1.0f };
            chitinArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 24.5f, Weight = 8.0f };
            DragonbornLightArmors.Armor["Chitin"] = chitinArmor;

            var blackguardsArmor = new ArmorType { Identifiers = "USLEEPArmorMaterialBlackguard", Type = ArmorTypeEnum.LightArmor };
            blackguardsArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 33f, Weight = 7.0f };
            blackguardsArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 2.0f };
            blackguardsArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 8f, Weight = 2.0f };
            blackguardsArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 2.0f };
            DragonbornLightArmors.Armor["Blackguard's"] = blackguardsArmor;

            var stalhrimlightArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialStalhrimLight", Type = ArmorTypeEnum.LightArmor };
            stalhrimlightArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 39f, Weight = 7.0f };
            stalhrimlightArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11.5f, Weight = 2.0f };
            stalhrimlightArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11.5f, Weight = 2.0f };
            stalhrimlightArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16.5f, Weight = 2.0f };
            stalhrimlightArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 29.5f, Weight = 10.0f };
            DragonbornLightArmors.Armor["Stalhrim Light"] = stalhrimlightArmor;
        }

        private void InitializeCreationClubLightArmors()
        {
            var dwarvenMailArmor = new ArmorType { Identifiers = "ArmorMaterialDwarven;IAKMaterialDwarven", Type = ArmorTypeEnum.LightArmor };
            dwarvenMailArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 25f, Weight = 15.0f };
            dwarvenMailArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 5.0f };
            dwarvenMailArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 4.0f };
            dwarvenMailArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15f, Weight = 5.0f };
            dwarvenMailArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 19f, Weight = 6.0f };
            CreationClubLightArmors.Armor["Dwarven Mail"] = dwarvenMailArmor;

            var leatherscoutArmor = new ArmorType { Identifiers = "Leather Scout", Type = ArmorTypeEnum.LightArmor };
            leatherscoutArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 27f, Weight = 6.0f };
            leatherscoutArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 8f, Weight = 2.0f };
            leatherscoutArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 8f, Weight = 2.0f };
            leatherscoutArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 13f, Weight = 2.0f };
            CreationClubLightArmors.Armor["Leather Scout"] = leatherscoutArmor;

            var netchleatherArmor = new ArmorType { Identifiers = "Netch Leather", Type = ArmorTypeEnum.LightArmor };
            netchleatherArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 32f, Weight = 6.0f };
            netchleatherArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            netchleatherArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            netchleatherArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 2.0f };
            netchleatherArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 18f };
            netchleatherArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "Tower Shield", Rating = 24f, Weight = 8.0f };
            CreationClubLightArmors.Armor["Netch Leather"] = netchleatherArmor;

            var shadowednetchleatherArmor = new ArmorType { Identifiers = "Shadowed Netch Leather", Type = ArmorTypeEnum.LightArmor };
            shadowednetchleatherArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 33f, Weight = 6.0f };
            shadowednetchleatherArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 12f, Weight = 2.0f };
            shadowednetchleatherArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 12f, Weight = 2.0f };
            shadowednetchleatherArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 17f, Weight = 2.0f };
            shadowednetchleatherArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 19f };
            shadowednetchleatherArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "Tower Shield", Rating = 25f, Weight = 8.0f };
            CreationClubLightArmors.Armor["Shadowed Netch Leather"] = shadowednetchleatherArmor;

            var remnantArmor = new ArmorType { Identifiers = "Remnant", Type = ArmorTypeEnum.LightArmor };
            remnantArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 34f, Weight = 6.0f };
            remnantArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 8f, Weight = 1.5f };
            remnantArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 9f, Weight = 1.0f };
            remnantArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 13f, Weight = 1.0f };
            remnantArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 24f, Weight = 6.0f };
            CreationClubLightArmors.Armor["Remnant"] = remnantArmor;

            var paintednetchleatherArmor = new ArmorType { Identifiers = "Painted Netch Leather;Boiled Netch Leather", Type = ArmorTypeEnum.LightArmor };
            paintednetchleatherArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 35f, Weight = 6.0f };
            paintednetchleatherArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            paintednetchleatherArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            paintednetchleatherArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 20f, Weight = 2.0f };
            CreationClubLightArmors.Armor["Boiled/Painted Netch Leather"] = paintednetchleatherArmor;

            var shadowedBoiledNetchLeatherArmor = new ArmorType { Identifiers = "Shadowed Boiled Netch Leather", Type = ArmorTypeEnum.LightArmor };
            shadowedBoiledNetchLeatherArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 36f, Weight = 6.0f };
            shadowedBoiledNetchLeatherArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 22f, Weight = 2.0f };
            CreationClubLightArmors.Armor["Shadowed Boiled/Painted Netch Leather"] = shadowedBoiledNetchLeatherArmor;

            var orcishscaledArmor = new ArmorType { Identifiers = "ArmorMaterialOrcish;IAKMaterialOrcish", Type = ArmorTypeEnum.LightArmor };
            orcishscaledArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 36f, Weight = 14.0f };
            orcishscaledArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 4.0f };
            orcishscaledArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13f, Weight = 4.0f };
            orcishscaledArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 6.0f };
            orcishscaledArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 24f, Weight = 6.0f };
            CreationClubLightArmors.Armor["Orcish Scaled"] = orcishscaledArmor;

            var darkseducerArmor = new ArmorType { Identifiers = "ccBGSSSE025_ArmorMaterialDark", Type = ArmorTypeEnum.LightArmor };
            darkseducerArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 36f, Weight = 5.0f };
            darkseducerArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 1.0f };
            darkseducerArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 1.0f };
            darkseducerArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 14f, Weight = 2.0f };
            darkseducerArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 23f, Weight = 4.0f };
            CreationClubLightArmors.Armor["Dark Seducer"] = darkseducerArmor;

            var reforgedCrusaderArmor = new ArmorType { Identifiers = "Crusader", Type = ArmorTypeEnum.LightArmor };
            reforgedCrusaderArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 38f, Weight = 7.0f };
            reforgedCrusaderArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            reforgedCrusaderArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            reforgedCrusaderArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 2.0f };
            reforgedCrusaderArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield;Shield", Rating = 27f, Weight = 6.0f };
            CreationClubLightArmors.Armor["Reforged Crusader"] = reforgedCrusaderArmor;

            var studdeddragonscaleArmor = new ArmorType { Identifiers = "Studded Dragonscale", Type = ArmorTypeEnum.LightArmor };
            studdeddragonscaleArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 43f, Weight = 10.0f };
            studdeddragonscaleArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 3.0f };
            studdeddragonscaleArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 3.0f };
            studdeddragonscaleArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 4.0f };
            studdeddragonscaleArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 29f, Weight = 10.0f };
            CreationClubLightArmors.Armor["Studded Dragonscale"] = studdeddragonscaleArmor;

            var amberArmor = new ArmorType { Identifiers = "ccBGSSSE025_ArmorMaterialAmber", Type = ArmorTypeEnum.LightArmor };
            amberArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 43f, Weight = 8.0f };
            amberArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13.5f, Weight = 2.0f };
            amberArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13.5f, Weight = 2.0f };
            amberArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 3.0f };
            amberArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 31f, Weight = 10.0f };
            CreationClubLightArmors.Armor["Amber"] = amberArmor;

            var daedricMailArmor = new ArmorType { Identifiers = "ArmorMaterialDaedric;IAKMaterialDaedric", Type = ArmorTypeEnum.LightArmor };
            daedricMailArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 44f, Weight = 12.0f };
            daedricMailArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 22f, Weight = 5.0f };
            daedricMailArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 4.0f };
            daedricMailArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 3.0f };
            daedricMailArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 32f, Weight = 10.0f };
            CreationClubLightArmors.Armor["Daedric Mail"] = daedricMailArmor;
        }

        private void InitializeUniqueLightArmors()
        {
            var lightDragonPriestHelmets = new ArmorType
            {
                Identifiers = "061CB9:Skyrim.esm;061CAB:Skyrim.esm;061C8B:Skyrim.esm;061CCA:Skyrim.esm",
                Type = ArmorTypeEnum.LightArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            lightDragonPriestHelmets.Armor["Krosis"] = new ArmorPiece { Identifiers = "061CB9:Skyrim.esm", Rating = 21f, Weight = 5.0f };
            lightDragonPriestHelmets.Armor["Volsung"] = new ArmorPiece { Identifiers = "061CAB:Skyrim.esm", Rating = 21f, Weight = 5.0f };
            lightDragonPriestHelmets.Armor["Morokei"] = new ArmorPiece { Identifiers = "061C8B:Skyrim.esm", Rating = 5f, Weight = 4.0f };
            lightDragonPriestHelmets.Armor["Wooden Mask"] = new ArmorPiece { Identifiers = "061CCA:Skyrim.esm", Rating = 1, Weight = 2.0f };
            UniqueLightArmors.Armor["Masks Light"] = lightDragonPriestHelmets;

            var oldGodsArmor = new ArmorType { Identifiers = "ArmorMaterialMS02Forsworn", Type = ArmorTypeEnum.LightArmor, MatchBehavior = ArmorMatchBehavior.Pass };
            oldGodsArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 24f, Weight = 3.0f };
            oldGodsArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 1.5f };
            oldGodsArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 0.5f };
            oldGodsArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 1.0f };
            UniqueLightArmors.Armor["Old Gods"] = oldGodsArmor;

            var linwesArmor = new ArmorType { Identifiers = "USKPArmorMaterialLinwe", Type = ArmorTypeEnum.LightArmor, MatchBehavior = ArmorMatchBehavior.Pass };
            linwesArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 31f, Weight = 8.0f };
            linwesArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            linwesArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            linwesArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 2.0f };
            UniqueLightArmors.Armor["Linwe's"] = linwesArmor;

            var deathbrandArmor = new ArmorType { Identifiers = "Deathbrand", Type = ArmorTypeEnum.LightArmor, MatchBehavior = ArmorMatchBehavior.Pass };
            deathbrandArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 39f, Weight = 7.0f };
            deathbrandArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11.5f, Weight = 2.0f };
            deathbrandArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11.5f, Weight = 2.0f };
            deathbrandArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16.5f, Weight = 2.0f };
            UniqueLightArmors.Armor["Deathbrand"] = deathbrandArmor;

            var vampireRoyalArmor = new ArmorType { Identifiers = "Vampire Royal", Type = ArmorTypeEnum.LightArmor, MatchBehavior = ArmorMatchBehavior.Pass };
            vampireRoyalArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 30f, Weight = 9.0f };
            UniqueLightArmors.Armor["Vampire Royal"] = vampireRoyalArmor;

            var nightingaleArmor = new ArmorType { Identifiers = "Nightingale", Type = ArmorTypeEnum.LightArmor, MatchBehavior = ArmorMatchBehavior.Pass };
            nightingaleArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 34f, Weight = 12.0f };
            nightingaleArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 2.0f };
            nightingaleArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 2.0f };
            nightingaleArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15f, Weight = 2.0f };
            UniqueLightArmors.Armor["Nightingale"] = nightingaleArmor;

            var uniqueLightArmors = new ArmorType
            {
                Identifiers = "02AC61:Skyrim.esm;0EE5C0:Skyrim.esm;0C0165:Skyrim.esm;0F5981:Skyrim.esm;0A4DCD:Skyrim.esm;0F9904:Skyrim.esm;096D9B:Skyrim.esm;" +
            "10559D:Skyrim.esm;037B8C:Dragonborn.esm;04D170:ccasvsse001-almsivi.esm;Gray Cowl of Nocturnal;00092B:ccbgssse041-netchleather.esl;000CCF:ccrmssse001-necrohouse.esl;" +
            "011BAF:Dawnguard.esm;000805:cccbhsse001-gaunt.esl;00084C:cccbhsse001-gaunt.esl;01A755:Dawnguard.esm",
                Type = ArmorTypeEnum.LightArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            uniqueLightArmors.Armor["Savior's Hide"] = new ArmorPiece { Identifiers = "02AC61:Skyrim.esm", Rating = 26f, Weight = 6.0f };
            uniqueLightArmors.Armor["Torture's Hood"] = new ArmorPiece { Identifiers = "0EE5C0:Skyrim.esm", Rating = 13f };
            uniqueLightArmors.Armor["Tsun's Armor"] = new ArmorPiece { Identifiers = "0C0165:Skyrim.esm", Rating = 29f, Weight = 0.0f };
            uniqueLightArmors.Armor["Tumblerbane Gloves"] = new ArmorPiece { Identifiers = "0F5981:Skyrim.esm", Rating = 7f, Weight = 2.0f };
            uniqueLightArmors.Armor["Predator's Grace"] = new ArmorPiece { Identifiers = "0A4DCD:Skyrim.esm", Rating = 5f, Weight = 1.0f };
            uniqueLightArmors.Armor["Diadem of the Savant"] = new ArmorPiece { Identifiers = "0F9904:Skyrim.esm", Rating = 7f, Weight = 4.0f };
            uniqueLightArmors.Armor["Movarth's Boots"] = new ArmorPiece { Identifiers = "096D9B:Skyrim.esm", Rating = 5f, Weight = 1.0f };
            uniqueLightArmors.Armor["Eastmarch Guard Helmet"] = new ArmorPiece { Identifiers = "10559D:Skyrim.esm", Rating = 12f, Weight = 2.0f };
            uniqueLightArmors.Armor["Dawnguard Rune Shield"] = new ArmorPiece { Identifiers = "011BAF:Dawnguard.esm", Rating = 27f, Weight = 6.0f };
            uniqueLightArmors.Armor["Cultist Gloves"] = new ArmorPiece { Identifiers = "037B8C:Dragonborn.esm", Rating = 7f, Weight = 1.5f };
            uniqueLightArmors.Armor["Redoran Watchman's Helmet"] = new ArmorPiece { Identifiers = "04D170:ccasvsse001-almsivi.esm", Rating = 15f, Weight = 1.0f };
            uniqueLightArmors.Armor["Gray Cowl of Nocturnal"] = new ArmorPiece { Identifiers = "Gray Cowl of Nocturnal", Rating = 13f, Weight = 2.0f };
            uniqueLightArmors.Armor["Boots of Blinding Speed"] = new ArmorPiece { Identifiers = "00092B:ccbgssse041-netchleather.esl", Rating = 11f, Weight = 2.0f };
            uniqueLightArmors.Armor["Bloodworm Helm"] = new ArmorPiece { Identifiers = "000CCF:ccrmssse001-necrohouse.esl", Rating = 15f, Weight = 5.0f };
            uniqueLightArmors.Armor["Brawler's Leather Bracers"] = new ArmorPiece { Identifiers = "000805:cccbhsse001-gaunt.esl;00084C:cccbhsse001-gaunt.esl", Rating = 7f, Weight = 2.0f };
            uniqueLightArmors.Armor["Reaper Bracers"] = new ArmorPiece { Identifiers = "01A755:Dawnguard.esm", Rating = 9f, Weight = 2.0f };
            UniqueLightArmors.Armor["Unique Light Armors"] = uniqueLightArmors;

            var miraakLight = new ArmorType
            {
                Identifiers = "029A62:Dragonborn.esm;039D2B:Dragonborn.esm;039D2E:Dragonborn.esm;039D2F:Dragonborn.esm",
                Type = ArmorTypeEnum.LightArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            miraakLight.Armor["Miraak's Mask"] = new ArmorPiece { Identifiers = "029A62:Dragonborn.esm", Rating = 23f, Weight = 9.0f };
            miraakLight.Armor["Miraak's Mask lvl 1"] = new ArmorPiece { Identifiers = "039D2B:Dragonborn.esm", Rating = 23f, Weight = 9.0f };
            miraakLight.Armor["Miraak's Mask lvl 2"] = new ArmorPiece { Identifiers = "039D2E:Dragonborn.esm", Rating = 25f, Weight = 9.0f };
            miraakLight.Armor["Miraak's Mask lvl 3"] = new ArmorPiece { Identifiers = "039D2F:Dragonborn.esm", Rating = 27f, Weight = 9.0f };
            UniqueLightArmors.Armor["Miraak's Mask"] = miraakLight;

            var amuletOfArticulation = new ArmorType
            {
                Identifiers = "09DFF7:Skyrim.esm;0F6904:Skyrim.esm;0F690D:Skyrim.esm;0F690E:Skyrim.esm;0F690F:Skyrim.esm;0F6910:Skyrim.esm;0F6911:Skyrim.esm",
                Type = ArmorTypeEnum.LightArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            amuletOfArticulation.Armor["Amulet of Articulation 1"] = new ArmorPiece { Identifiers = "09DFF7:Skyrim.esm", Rating = 2f, Weight = 1.0f };
            amuletOfArticulation.Armor["Amulet of Articulation 2"] = new ArmorPiece { Identifiers = "0F6904:Skyrim.esm", Rating = 3f, Weight = 1.0f };
            amuletOfArticulation.Armor["Amulet of Articulation 3"] = new ArmorPiece { Identifiers = "0F690D:Skyrim.esm", Rating = 4f, Weight = 1.0f };
            amuletOfArticulation.Armor["Amulet of Articulation 4"] = new ArmorPiece { Identifiers = "0F690E:Skyrim.esm", Rating = 5f, Weight = 1.0f };
            amuletOfArticulation.Armor["Amulet of Articulation 5"] = new ArmorPiece { Identifiers = "0F690F:Skyrim.esm", Rating = 6f, Weight = 1.0f };
            amuletOfArticulation.Armor["Amulet of Articulation 6"] = new ArmorPiece { Identifiers = "0F6910:Skyrim.esm", Rating = 7f, Weight = 1.0f };
            amuletOfArticulation.Armor["Amulet of Articulation 7"] = new ArmorPiece { Identifiers = "0F6911:Skyrim.esm", Rating = 8f, Weight = 1.0f };
            UniqueLightArmors.Armor["Amulet of Articulation"] = amuletOfArticulation;

        }

        private void InitializeOtherLightArmors()
        {
            var lightIronArmor = new ArmorType { Identifiers = "ArmorMaterialIron;IAKMaterialIron", Type = ArmorTypeEnum.LightArmor };
            lightIronArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 26f, Weight = 6.0f };
            lightIronArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 6f, Weight = 2.0f };
            lightIronArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 6f, Weight = 2.0f };
            lightIronArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 11f, Weight = 2.0f };
            lightIronArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 19f, Weight = 4.0f };
            OtherLightArmors.Armor["Light Iron"] = lightIronArmor;

            var lightSteelArmor = new ArmorType { Identifiers = "ArmorMaterialSteel;IAKMaterialSteel;IAKMaterialSteelChain", Type = ArmorTypeEnum.LightArmor };
            lightSteelArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 27f, Weight = 8.0f };
            lightSteelArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 7f, Weight = 2.0f };
            lightSteelArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 7f, Weight = 2.0f };
            lightSteelArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 12f, Weight = 2.0f };
            lightSteelArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 20f, Weight = 4.0f };
            OtherLightArmors.Armor["Light Steel"] = lightSteelArmor;

            var lightEbonyArmor = new ArmorType { Identifiers = "ArmorMaterialEbony;IAKMaterialEbony;Ebony", Type = ArmorTypeEnum.LightArmor };
            lightEbonyArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 37f, Weight = 7.0f };
            lightEbonyArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10.5f, Weight = 2.0f };
            lightEbonyArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10.5f, Weight = 2.0f };
            lightEbonyArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15.5f, Weight = 2.0f };
            lightEbonyArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 27.5f, Weight = 10.0f };
            OtherLightArmors.Armor["Light Stalhrim"] = lightEbonyArmor;

            var immersiveArmors = new ArmorType
            {
                Identifiers = "02146C:Hothtrooper44_ArmorCompilation.esp;021467:Hothtrooper44_ArmorCompilation.esp;021465:Hothtrooper44_ArmorCompilation.esp;" +
            "021466:Hothtrooper44_ArmorCompilation.esp;02146A:Hothtrooper44_ArmorCompilation.esp;021462:Hothtrooper44_ArmorCompilation.esp;021468:Hothtrooper44_ArmorCompilation.esp;" +
            "021464:Hothtrooper44_ArmorCompilation.esp;021463:Hothtrooper44_ArmorCompilation.esp;021461:Hothtrooper44_ArmorCompilation.esp;021460:Hothtrooper44_ArmorCompilation.esp;" +
            "021469:Hothtrooper44_ArmorCompilation.esp;02146B:Hothtrooper44_ArmorCompilation.esp;",
                Type = ArmorTypeEnum.LightArmor
            };
            immersiveArmors.Armor["Hide Buckler"] = new ArmorPiece { Identifiers = "021465:Hothtrooper44_ArmorCompilation.esp", Rating = 10f, Weight = 2.0f };
            immersiveArmors.Armor["Iron Buckler"] = new ArmorPiece { Identifiers = "021467:Hothtrooper44_ArmorCompilation.esp", Rating = 12f, Weight = 4.0f };
            immersiveArmors.Armor["Imperial Buckler"] = new ArmorPiece { Identifiers = "021466:Hothtrooper44_ArmorCompilation.esp", Rating = 13.5f, Weight = 5.0f };
            immersiveArmors.Armor["Steel Buckler"] = new ArmorPiece { Identifiers = "02146A:Hothtrooper44_ArmorCompilation.esp", Rating = 15f, Weight = 5.0f };
            immersiveArmors.Armor["Dwarven Buckler"] = new ArmorPiece { Identifiers = "021462:Hothtrooper44_ArmorCompilation.esp", Rating = 16.5f, Weight = 6.0f };
            immersiveArmors.Armor["Orcish Buckler"] = new ArmorPiece { Identifiers = "021468:Hothtrooper44_ArmorCompilation.esp", Rating = 18f, Weight = 6.0f };
            immersiveArmors.Armor["Buckler of Ysgramor"] = new ArmorPiece { Identifiers = "02146C:Hothtrooper44_ArmorCompilation.esp", Rating = 18f, Weight = 6.0f };
            immersiveArmors.Armor["Sithis Buckler"] = new ArmorPiece { Identifiers = "021469:Hothtrooper44_ArmorCompilation.esp", Rating = 19, Weight = 5.0f };
            immersiveArmors.Armor["Winterhold Battlemage Buckler"] = new ArmorPiece { Identifiers = "02146B:Hothtrooper44_ArmorCompilation.esp", Rating = 19f, Weight = 5.0f };
            immersiveArmors.Armor["Glass Buckler"] = new ArmorPiece { Identifiers = "021464:Hothtrooper44_ArmorCompilation.esp", Rating = 19.5f, Weight = 4.0f };
            immersiveArmors.Armor["Ebony Buckler"] = new ArmorPiece { Identifiers = "021463:Hothtrooper44_ArmorCompilation.esp", Rating = 21f, Weight = 7.0f };
            immersiveArmors.Armor["Dragonhide Buckler"] = new ArmorPiece { Identifiers = "021461:Hothtrooper44_ArmorCompilation.esp", Rating = 22.5f, Weight = 7.0f };
            immersiveArmors.Armor["Daedric Buckler"] = new ArmorPiece { Identifiers = "021460:Hothtrooper44_ArmorCompilation.esp", Rating = 24f, Weight = 8.0f };
            OtherLightArmors.Armor["Immersive Armors"] = immersiveArmors;

            var moonMonkArmors = new ArmorType { Identifiers = "Moon Monk's;Ra'kazra's", Type = ArmorTypeEnum.LightArmor };
            moonMonkArmors.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 38f, Weight = 10.0f };
            moonMonkArmors.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 2.0f };
            moonMonkArmors.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 2.0f };
            moonMonkArmors.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 3.0f };
            OtherLightArmors.Armor["Moon Monk's"] = moonMonkArmors;

            var khajiitWillFollow = new ArmorType { Identifiers = "51187E:KhajiitWillFollow.esp", Type = ArmorTypeEnum.LightArmor };
            khajiitWillFollow.Armor["Ascension"] = new ArmorPiece { Identifiers = "51187E:KhajiitWillFollow.esp", Rating = 39f, Weight = 7.0f };
            OtherLightArmors.Armor["Khajiit Will Follow"] = khajiitWillFollow;

            var grayFoxCowlArmor = new ArmorType { Identifiers = "018B4B:Gray Fox Cowl.esm", Type = ArmorTypeEnum.LightArmor };
            grayFoxCowlArmor.Armor["Boots of Springheel Jak"] = new ArmorPiece { Identifiers = "018B4B:Gray Fox Cowl.esm", Rating = 0f, Weight = 2.0f };
            OtherLightArmors.Armor["Gray Fox Cowl"] = grayFoxCowlArmor;

            var midwoodIsleArmors = new ArmorType { Identifiers = "19D1CE:Midwood Isle.esp", Type = ArmorTypeEnum.LightArmor };
            midwoodIsleArmors.Armor["Nahvaal"] = new ArmorPiece { Identifiers = "19D1CE:Midwood Isle.esp", Rating = 21f, Weight = 5.0f };
            OtherLightArmors.Armor["Midwood Isle"] = midwoodIsleArmors;

            var skyrimExtendedCutSaintsAndSeducers = new ArmorType { Identifiers = "2F68C7:Skyrim Extended Cut - Saints and Seducers.esp", Type = ArmorTypeEnum.LightArmor };
            skyrimExtendedCutSaintsAndSeducers.Armor["Grummite Light Shield"] = new ArmorPiece { Identifiers = "2F68C7:Skyrim Extended Cut - Saints and Seducers.esp", Rating = 17f, Weight = 9.0f };
            OtherLightArmors.Armor["Skyrim Extended Cut - Saints and Seducers"] = skyrimExtendedCutSaintsAndSeducers;

            var beyondBruma = new ArmorType { Identifiers = "6026FF:BSAssets.esm", Type = ArmorTypeEnum.LightArmor };
            beyondBruma.Armor["Ayleid Lich Helmet"] = new ArmorPiece { Identifiers = "6026FF:BSAssets.esm", Rating = 21f, Weight = 5.0f };
            OtherLightArmors.Armor["Beyond Bruma"] = beyondBruma;
        }

        private void InitializeVanillaHeavyArmors()
        {

            var heavyGuardArmor = new ArmorType { Identifiers = "Guard's", Type = ArmorTypeEnum.HeavyArmor };
            heavyGuardArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 25f, Weight = 35.0f };
            heavyGuardArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 8.0f };
            heavyGuardArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 4.0f };
            heavyGuardArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15f, Weight = 5.0f };
            heavyGuardArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 17f, Weight = 3.0f };
            VanillaHeavyArmors.Armor["Guard"] = heavyGuardArmor;

            var imperialArmor = new ArmorType { Identifiers = "ArmorMaterialImperialHeavy", Type = ArmorTypeEnum.HeavyArmor };
            imperialArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 25f, Weight = 35.0f };
            imperialArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 8.0f };
            imperialArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 4.0f };
            imperialArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15f, Weight = 5.0f };
            imperialArmor.Armor["Helmet Full"] = new ArmorPiece { Identifiers = "09610D:Skyrim.esm", Rating = 18f, Weight = 5.0f };
            imperialArmor.Armor["Officer's Helmet"] = new ArmorPiece { Identifiers = "0136CF:Skyrim.esm", Rating = 17f, Weight = 4.0f };
            imperialArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 20f, Weight = 12.0f };
            VanillaHeavyArmors.Armor["Imperial"] = imperialArmor;

            var ironArmor = new ArmorType { Identifiers = "ArmorMaterialIron;IAKMaterialIron", Type = ArmorTypeEnum.HeavyArmor };
            ironArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 25f, Weight = 30.0f };
            ironArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 6.0f };
            ironArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 5.0f };
            ironArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15f, Weight = 5.0f };
            ironArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 20f, Weight = 12.0f };
            VanillaHeavyArmors.Armor["Iron"] = ironArmor;

            var ancientnordArmor = new ArmorType { Identifiers = "Ancient Nord", Type = ArmorTypeEnum.HeavyArmor };
            ancientnordArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 25f, Weight = 28.0f };
            ancientnordArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 5.0f };
            ancientnordArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 4.0f };
            ancientnordArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 15f, Weight = 4.0f };
            VanillaHeavyArmors.Armor["Ancient Nord"] = ancientnordArmor;

            var bandedIronArmor = new ArmorType { Identifiers = "Banded Iron", Type = ArmorTypeEnum.HeavyArmor };
            bandedIronArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 28f, Weight = 35.0f };
            bandedIronArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 22f, Weight = 12.0f };
            VanillaHeavyArmors.Armor["Banded Iron"] = bandedIronArmor;

            var wolfArmor = new ArmorType { Identifiers = "Wolf;Armored Fur", Type = ArmorTypeEnum.HeavyArmor };
            wolfArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 31f, Weight = 20.0f };
            wolfArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 12f, Weight = 4.0f };
            wolfArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 12f, Weight = 4.0f };
            wolfArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 17f, Weight = 4.0f };
            VanillaHeavyArmors.Armor["Wolf"] = wolfArmor;

            var falmerArmor = new ArmorType { Identifiers = "DLC1ArmorMaterielFalmerHeavyOriginal", Type = ArmorTypeEnum.HeavyArmor };
            falmerArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 31f, Weight = 20.0f };
            falmerArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 12f, Weight = 4.0f };
            falmerArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 12f, Weight = 4.0f };
            falmerArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 10f, Weight = 5.0f };
            falmerArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 28f, Weight = 15.0f };
            VanillaHeavyArmors.Armor["Falmer"] = falmerArmor;

            var steelArmor = new ArmorType { Identifiers = "ArmorMaterialSteel;IAKMaterialSteel", Type = ArmorTypeEnum.HeavyArmor };
            steelArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 31f, Weight = 35.0f };
            steelArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 12f, Weight = 8.0f };
            steelArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 12f, Weight = 4.0f };
            steelArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 17f, Weight = 5.0f };
            steelArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 24f, Weight = 12.0f };
            VanillaHeavyArmors.Armor["Steel"] = steelArmor;

            var dwarvenArmor = new ArmorType { Identifiers = "ArmorMaterialDwarven;IAKMaterialDwarven", Type = ArmorTypeEnum.HeavyArmor };
            dwarvenArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 34f, Weight = 45.0f };
            dwarvenArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 10.0f };
            dwarvenArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13f, Weight = 8.0f };
            dwarvenArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 12.0f };
            dwarvenArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 12.0f };
            VanillaHeavyArmors.Armor["Dwarven"] = dwarvenArmor;

            var dwarvenPlateArmor = new ArmorType { Identifiers = "Dwarven Plate", Type = ArmorTypeEnum.HeavyArmor };
            dwarvenPlateArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 35f, Weight = 45.0f };
            dwarvenPlateArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 10.0f };
            VanillaHeavyArmors.Armor["Dwarven Plate"] = dwarvenPlateArmor;

            var orcishArmor = new ArmorType { Identifiers = "ArmorMaterialOrcish;IAKMaterialOrcish", Type = ArmorTypeEnum.HeavyArmor };
            orcishArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 35.0f };
            orcishArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 15f, Weight = 7.0f };
            orcishArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 15f, Weight = 7.0f };
            orcishArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 20f, Weight = 8.0f };
            orcishArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 30f, Weight = 14.0f };
            VanillaHeavyArmors.Armor["Orcish"] = orcishArmor;

            var steelplateArmor = new ArmorType { Identifiers = "ArmorMaterialSteelPlate;IAKMaterialAdvancedPlate", Type = ArmorTypeEnum.HeavyArmor };
            steelplateArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 38.0f };
            steelplateArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 9.0f };
            steelplateArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 6.0f };
            steelplateArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 6.0f };
            steelplateArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 29f, Weight = 14.0f };
            VanillaHeavyArmors.Armor["Steel Plate"] = steelplateArmor;

            var ebonyArmor = new ArmorType { Identifiers = "ArmorMaterialEbony;IAKMaterialEbony;Ebony", Type = ArmorTypeEnum.HeavyArmor };
            ebonyArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 43f, Weight = 38.0f };
            ebonyArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 16f, Weight = 7.0f };
            ebonyArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 16f, Weight = 7.0f };
            ebonyArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 21f, Weight = 10.0f };
            ebonyArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 32f, Weight = 14.0f };
            VanillaHeavyArmors.Armor["Ebony"] = ebonyArmor;

            var bladesArmor = new ArmorType { Identifiers = "ArmorMaterialBlades;Blades", Type = ArmorTypeEnum.HeavyArmor };
            bladesArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 34f, Weight = 45.0f };
            bladesArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 10.0f };
            bladesArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13f, Weight = 8.0f };
            bladesArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 12.0f };
            bladesArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 12.0f };
            VanillaHeavyArmors.Armor["Blades"] = bladesArmor;

            var dragonplateArmor = new ArmorType { Identifiers = "ArmorMaterialDragonplate;IAKMaterialDragonPlate", Type = ArmorTypeEnum.HeavyArmor };
            dragonplateArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 46f, Weight = 40.0f };
            dragonplateArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 17f, Weight = 8.0f };
            dragonplateArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 17f, Weight = 8.0f };
            dragonplateArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 22f, Weight = 8.0f };
            dragonplateArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 34f, Weight = 15.0f };
            VanillaHeavyArmors.Armor["Dragonplate"] = dragonplateArmor;

            var daedricArmor = new ArmorType { Identifiers = "ArmorMaterialDaedric;IAKMaterialDaedric", Type = ArmorTypeEnum.HeavyArmor };
            daedricArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 49f, Weight = 50.0f };
            daedricArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 18f, Weight = 10.0f };
            daedricArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 18f, Weight = 6.0f };
            daedricArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 23f, Weight = 15.0f };
            daedricArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 36f, Weight = 15.0f };
            VanillaHeavyArmors.Armor["Daedric"] = daedricArmor;
        }

        private void InitializeDawnguardHeavyArmors()
        {
            var dawnguardArmor = new ArmorType { Identifiers = "DLC1ArmorMaterialDawnguard", Type = ArmorTypeEnum.HeavyArmor };
            dawnguardArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 25f, Weight = 28.0f };
            dawnguardArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 10f, Weight = 5.0f };
            dawnguardArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 10f, Weight = 4.0f };
            DawnguardHeavyArmors.Armor["Dawnguard"] = dawnguardArmor;

            var falmerhardenedArmor = new ArmorType { Identifiers = "DLC1ArmorMaterialFalmerHardened", Type = ArmorTypeEnum.HeavyArmor };
            falmerhardenedArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 34f, Weight = 40.0f };
            falmerhardenedArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 8.0f };
            falmerhardenedArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13f, Weight = 7.0f };
            falmerhardenedArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 10.0f };
            DawnguardHeavyArmors.Armor["Falmer Hardened"] = falmerhardenedArmor;

            var dawnguardHeavyArmor = new ArmorType { Identifiers = "Dawnguard Heavy;Dawnguard Full;Dawnguard Shield", Type = ArmorTypeEnum.HeavyArmor };
            dawnguardHeavyArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 34f, Weight = 42.0f };
            dawnguardHeavyArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 9.0f };
            dawnguardHeavyArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13f, Weight = 7.0f };
            dawnguardHeavyArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 10.0f };
            dawnguardHeavyArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 10.0f };
            DawnguardHeavyArmors.Armor["Dawnguard"] = dawnguardHeavyArmor;

            var falmerheavyArmor = new ArmorType { Identifiers = "DLC1ArmorMaterielFalmerHeavy", Type = ArmorTypeEnum.HeavyArmor };
            falmerheavyArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 43f, Weight = 35.0f };
            falmerheavyArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 16f, Weight = 6.0f };
            falmerheavyArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 16f, Weight = 6.0f };
            falmerheavyArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 21f, Weight = 8.0f };
            DawnguardHeavyArmors.Armor["Falmer Heavy"] = falmerheavyArmor;
        }

        private void InitializeDragonbornHeavyArmors()
        {
            var bonemoldArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialBonemoldHeavy", Type = ArmorTypeEnum.HeavyArmor };
            bonemoldArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 32f, Weight = 34.0f };
            bonemoldArmor.Armor["Pauldron Armor"] = new ArmorPiece { Identifiers = "Pauldron Armor", Rating = 33f, Weight = 36.0f };
            bonemoldArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 12f, Weight = 7.0f };
            bonemoldArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 12f, Weight = 3.5f };
            bonemoldArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 17f, Weight = 4.5f };
            bonemoldArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 21.5f, Weight = 8.0f };
            DragonbornHeavyArmors.Armor["Bonemold"] = bonemoldArmor;

            var improvedbonemoldArmor = new ArmorType { Identifiers = "Improved Bonemold", Type = ArmorTypeEnum.HeavyArmor };
            improvedbonemoldArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 35f, Weight = 43.0f };
            improvedbonemoldArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 9.0f };
            improvedbonemoldArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13f, Weight = 7.0f };
            improvedbonemoldArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 11.0f };
            improvedbonemoldArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 11.0f };
            DragonbornHeavyArmors.Armor["Improved Bonemold"] = improvedbonemoldArmor;

            var ahzidalsArmor = new ArmorType { Identifiers = "Ahzidal's", Type = ArmorTypeEnum.HeavyArmor };
            ahzidalsArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f };
            ahzidalsArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f };
            ahzidalsArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f };
            ahzidalsArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f };

            var chitinheavyArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialChitinHeavy;WAF_MaterialChitin", Type = ArmorTypeEnum.HeavyArmor };
            chitinheavyArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 35.0f };
            chitinheavyArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 6.0f };
            chitinheavyArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 5.0f };
            chitinheavyArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 5.0f };
            DragonbornHeavyArmors.Armor["Chitin Heavy"] = chitinheavyArmor;

            var nordiccarvedArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialNordicHeavy", Type = ArmorTypeEnum.HeavyArmor };
            nordiccarvedArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 43f, Weight = 37.0f };
            nordiccarvedArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 15f, Weight = 6.0f };
            nordiccarvedArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 15f, Weight = 6.0f };
            nordiccarvedArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 20f, Weight = 7.0f };
            nordiccarvedArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26.5f, Weight = 10.0f };
            DragonbornHeavyArmors.Armor["Nordic Carved"] = nordiccarvedArmor;

            var stalhrimheavyArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialStalhrimHeavy", Type = ArmorTypeEnum.HeavyArmor };
            stalhrimheavyArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 46f, Weight = 38.0f };
            stalhrimheavyArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 17f, Weight = 7.0f };
            stalhrimheavyArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 17f, Weight = 7.0f };
            stalhrimheavyArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 22f, Weight = 7.0f };
            DragonbornHeavyArmors.Armor["Stalhrim Heavy"] = stalhrimheavyArmor;
        }

        private void InitializeCreationClubHeavyArmors()
        {
            var ironplateArmor = new ArmorType { Identifiers = "Iron Plate", Type = ArmorTypeEnum.HeavyArmor };
            ironplateArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 27f, Weight = 31.0f };
            ironplateArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 11f, Weight = 6.0f };
            ironplateArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 11f, Weight = 5.0f };
            ironplateArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 16f, Weight = 5.0f };
            CreationClubHeavyArmors.Armor["Iron Plate"] = ironplateArmor;

            var ironspellknightArmor = new ArmorType { Identifiers = "Iron Spell Knight", Type = ArmorTypeEnum.HeavyArmor };
            ironspellknightArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 29f, Weight = 30.0f };
            ironspellknightArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 6.0f };
            ironspellknightArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 5.0f };
            ironspellknightArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 5.0f };
            CreationClubHeavyArmors.Armor["Iron Spell Knight"] = ironspellknightArmor;

            var steelsoldierArmor = new ArmorType { Identifiers = "Steel Soldier;Blades Steel", Type = ArmorTypeEnum.HeavyArmor };
            steelsoldierArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 32f, Weight = 35.0f };
            steelsoldierArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 13f, Weight = 8.0f };
            steelsoldierArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 13f, Weight = 4.0f };
            steelsoldierArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 18f, Weight = 5.0f };
            steelsoldierArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 12.0f };
            CreationClubHeavyArmors.Armor["Steel Soldier"] = steelsoldierArmor;

            var steelspellknightArmor = new ArmorType { Identifiers = "Steel Spell Knight", Type = ArmorTypeEnum.HeavyArmor };
            steelspellknightArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 35f, Weight = 35.0f };
            steelspellknightArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 16f, Weight = 8.0f };
            steelspellknightArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 16f, Weight = 4.0f };
            steelspellknightArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 21f, Weight = 5.0f };
            CreationClubHeavyArmors.Armor["Steel Spell Knight"] = steelspellknightArmor;

            var indorilArmor = new ArmorType { Identifiers = "Indoril", Type = ArmorTypeEnum.HeavyArmor };
            indorilArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 38.0f };
            indorilArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 9.0f };
            indorilArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 6.0f };
            indorilArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 6.0f };
            CreationClubHeavyArmors.Armor["Indoril"] = indorilArmor;

            var vigilcorruptedArmor = new ArmorType { Identifiers = "Vigil Corrupted", Type = ArmorTypeEnum.HeavyArmor };
            vigilcorruptedArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 38.0f };
            vigilcorruptedArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 9.0f };
            vigilcorruptedArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 6.0f };
            vigilcorruptedArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 6.0f };
            vigilcorruptedArmor.Armor["Open Helmet"] = new ArmorPiece { Identifiers = "Open Helmet", Rating = 17f, Weight = 5.0f };
            CreationClubHeavyArmors.Armor["Vigil Corrupted"] = vigilcorruptedArmor;

            var vigilenforcerArmor = new ArmorType { Identifiers = "Vigil Enforcer", Type = ArmorTypeEnum.HeavyArmor };
            vigilenforcerArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 38.0f };
            vigilenforcerArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 9.0f };
            vigilenforcerArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 6.0f };
            vigilenforcerArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 17f, Weight = 5.0f };
            CreationClubHeavyArmors.Armor["Vigil Enforcer"] = vigilenforcerArmor;

            var vigilsilverhandArmor = new ArmorType { Identifiers = "Vigil Silver Hand", Type = ArmorTypeEnum.HeavyArmor };
            vigilsilverhandArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 38.0f };
            vigilsilverhandArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 9.0f };
            vigilsilverhandArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 6.0f };
            vigilsilverhandArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 17f, Weight = 5.0f };
            CreationClubHeavyArmors.Armor["Vigil Silver Hand"] = vigilsilverhandArmor;

            var vigilveteranArmor = new ArmorType { Identifiers = "Vigil Veteran", Type = ArmorTypeEnum.HeavyArmor };
            vigilveteranArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 38.0f };
            vigilveteranArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 9.0f };
            vigilveteranArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 6.0f };
            vigilveteranArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 6.0f };
            vigilveteranArmor.Armor["Open Helmet"] = new ArmorPiece { Identifiers = "Open Helmet", Rating = 17f, Weight = 5.0f };
            CreationClubHeavyArmors.Armor["Vigil Veteran"] = vigilveteranArmor;

            var orcishplateArmor = new ArmorType { Identifiers = "Orcish Plate", Type = ArmorTypeEnum.HeavyArmor };
            orcishplateArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 41f, Weight = 35.0f };
            orcishplateArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 16f, Weight = 7.0f };
            orcishplateArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 16f, Weight = 7.0f };
            orcishplateArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 20f, Weight = 8.0f };
            CreationClubHeavyArmors.Armor["Orcish Plate"] = orcishplateArmor;

            var goldensaintArmor = new ArmorType { Identifiers = "ccBGSSSE025_ArmorMaterialGolden", Type = ArmorTypeEnum.HeavyArmor };
            goldensaintArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 42f, Weight = 35.0f };
            goldensaintArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 15f, Weight = 6.0f };
            goldensaintArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 15f, Weight = 5.0f };
            goldensaintArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 20f, Weight = 5.0f };
            goldensaintArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 12.0f };
            CreationClubHeavyArmors.Armor["Golden Saint"] = goldensaintArmor;

            var silverArmor = new ArmorType { Identifiers = "Silver", Type = ArmorTypeEnum.HeavyArmor };
            silverArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 42f, Weight = 40.0f };
            silverArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 15f, Weight = 8.0f };
            silverArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 15f, Weight = 7.0f };
            silverArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 20f, Weight = 7.0f };
            CreationClubHeavyArmors.Armor["Silver"] = silverArmor;

            var crusaderArmor = new ArmorType { Identifiers = "Crusader", Type = ArmorTypeEnum.HeavyArmor };
            crusaderArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 43f, Weight = 38.0f };
            crusaderArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 16f, Weight = 7.0f };
            crusaderArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 16f, Weight = 7.0f };
            crusaderArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 21f, Weight = 10.0f };
            crusaderArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 32f, Weight = 14.0f };
            CreationClubLightArmors.Armor["Crusader"] = crusaderArmor;

            var herhandArmor = new ArmorType { Identifiers = "Her Hand", Type = ArmorTypeEnum.HeavyArmor };
            herhandArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 43f, Weight = 36.0f };
            herhandArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 16f, Weight = 6.0f };
            herhandArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 16f, Weight = 6.0f };
            herhandArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 21f, Weight = 8.0f };
            CreationClubHeavyArmors.Armor["Her Hand"] = herhandArmor;

            var ebonyplateArmor = new ArmorType { Identifiers = "Ebony Plate;Blades Ebony", Type = ArmorTypeEnum.HeavyArmor };
            ebonyplateArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 44f, Weight = 38.0f };
            ebonyplateArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 17f, Weight = 7.0f };
            ebonyplateArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 17f, Weight = 7.0f };
            ebonyplateArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 22f, Weight = 10.0f };
            ebonyplateArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 34f, Weight = 14.0f };
            CreationClubHeavyArmors.Armor["Ebony Plate"] = ebonyplateArmor;

            var ebonyspellknightArmor = new ArmorType { Identifiers = "Ebony Spell Knight", Type = ArmorTypeEnum.HeavyArmor };
            ebonyspellknightArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 47f, Weight = 38.0f };
            ebonyspellknightArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 20f, Weight = 7.0f };
            ebonyspellknightArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 20f, Weight = 7.0f };
            ebonyspellknightArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 25f, Weight = 10.0f };
            CreationClubHeavyArmors.Armor["Ebony Spell Knight"] = ebonyspellknightArmor;

            var dragonplateinsulatedArmor = new ArmorType { Identifiers = "Dragonplate Insulated", Type = ArmorTypeEnum.HeavyArmor };
            dragonplateinsulatedArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 48f, Weight = 40.0f };
            dragonplateinsulatedArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 19f, Weight = 8.0f };
            dragonplateinsulatedArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 19f, Weight = 8.0f };
            dragonplateinsulatedArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 24f, Weight = 8.0f };
            CreationClubHeavyArmors.Armor["Dragonplate Insulated"] = dragonplateinsulatedArmor;

            var dragonboneMailArmor = new ArmorType { Identifiers = "Dragonbone Mail", Type = ArmorTypeEnum.HeavyArmor };
            dragonboneMailArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 48f, Weight = 50.0f };
            CreationClubHeavyArmors.Armor["Dragonbone Mail"] = dragonboneMailArmor;

            var stalhrimfurArmor = new ArmorType { Identifiers = "Stalhrim Fur", Type = ArmorTypeEnum.HeavyArmor };
            stalhrimfurArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 48f, Weight = 38.0f };
            stalhrimfurArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 19f, Weight = 7.0f };
            stalhrimfurArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 19f, Weight = 7.0f };
            stalhrimfurArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 24f, Weight = 7.0f };
            CreationClubHeavyArmors.Armor["Stalhrim Fur"] = stalhrimfurArmor;

            var imperialdragonArmor = new ArmorType { Identifiers = "Imperial Dragon", Type = ArmorTypeEnum.HeavyArmor };
            imperialdragonArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 49f, Weight = 50.0f };
            imperialdragonArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 18f, Weight = 10.0f };
            imperialdragonArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 18f, Weight = 6.0f };
            imperialdragonArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 23f, Weight = 15.0f };
            imperialdragonArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 36f, Weight = 15.0f };
            CreationClubHeavyArmors.Armor["Imperial Dragon"] = imperialdragonArmor;

            var stormbearArmor = new ArmorType { Identifiers = "Storm-Bear", Type = ArmorTypeEnum.HeavyArmor };
            stormbearArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 49f, Weight = 50.0f };
            stormbearArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 18f, Weight = 10.0f };
            stormbearArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 18f, Weight = 6.0f };
            stormbearArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 23f, Weight = 15.0f };
            stormbearArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 36f, Weight = 15.0f };
            CreationClubHeavyArmors.Armor["Storm-Bear"] = stormbearArmor;

            var daedricPlateArmor = new ArmorType { Identifiers = "Daedric Plate", Type = ArmorTypeEnum.HeavyArmor };
            daedricPlateArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 51f, Weight = 50.0f };
            daedricPlateArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 25f, Weight = 15.0f };
            CreationClubHeavyArmors.Armor["Daedric Plate"] = daedricPlateArmor;

            var madnessArmor = new ArmorType { Identifiers = "ccBGSSSE025_ArmorMaterialMadness", Type = ArmorTypeEnum.HeavyArmor };
            madnessArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 52f, Weight = 52.0f };
            madnessArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 19f, Weight = 11.0f };
            madnessArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 19f, Weight = 8.0f };
            madnessArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 24f, Weight = 17.0f };
            madnessArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 38f, Weight = 16.0f };
            CreationClubHeavyArmors.Armor["Madness"] = madnessArmor;
        }

        private void InitializeUniqueHeavyArmors()
        {
            var uniqueHeavyArmors = new ArmorType
            {
                Identifiers = "052794:Skyrim.esm;045F96:Skyrim.esm;0FC5BF:Skyrim.esm;0295F3:Skyrim.esm;0E41D8:Skyrim.esm;05CBFE:Skyrim.esm;03521F:Skyrim.esm;" +
            "0D2846:Skyrim.esm;0DA750:Skyrim.esm;03399C:Dragonborn.esm;037B88:Dragonborn.esm;038ADD:Dragonborn.esm;15E949:ccasvsse001-almsivi.esm;06668E:ccasvsse001-almsivi.esm;" +
            "000D63:ccbgssse008-wraithguard.esl;Lord's Mail;000867:ccbgssse059-ba_dragonplate.esl;000813:ccbgssse069-contest.esl;000899:cckrtsse001_altar.esl;000CDC:cckrtsse001_altar.esl;" +
            "000D3D:ccrmssse001-necrohouse.esl;000893:cctwbsse001-puzzledungeon.esm;000886:ccvsvsse003-necroarts.esl;012E8A:Dawnguard.esm;00284D:Dawnguard.esm;005759:Dawnguard.esm;",
                Type = ArmorTypeEnum.HeavyArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            uniqueHeavyArmors.Armor["Ebony Mail"] = new ArmorPiece { Identifiers = "052794:Skyrim.esm", Rating = 45f, Weight = 28.0f };
            uniqueHeavyArmors.Armor["Spellbreaker"] = new ArmorPiece { Identifiers = "045F96:Skyrim.esm", Rating = 38f, Weight = 12.0f };
            uniqueHeavyArmors.Armor["Targe of the Blooded"] = new ArmorPiece { Identifiers = "0FC5BF:Skyrim.esm", Rating = 20f, Weight = 8.0f };
            uniqueHeavyArmors.Armor["Helm of Yngol"] = new ArmorPiece { Identifiers = "0295F3:Skyrim.esm", Rating = 21f, Weight = 8.0f };
            uniqueHeavyArmors.Armor["Shield of Ysgramor"] = new ArmorPiece { Identifiers = "0E41D8:Skyrim.esm", Rating = 30f, Weight = 12.0f };
            uniqueHeavyArmors.Armor["General Tullius' Armor"] = new ArmorPiece { Identifiers = "05CBFE:Skyrim.esm", Rating = 31f, Weight = 18.0f };
            uniqueHeavyArmors.Armor["Family Shield"] = new ArmorPiece { Identifiers = "03521F:Skyrim.esm", Rating = 20f, Weight = 10.0f };
            uniqueHeavyArmors.Armor["Masque of Clavicus Vile"] = new ArmorPiece { Identifiers = "0D2846:Skyrim.esm", Rating = 23f, Weight = 7.0f };
            uniqueHeavyArmors.Armor["Jagged Crown"] = new ArmorPiece { Identifiers = "0DA750:Skyrim.esm", Rating = 23f, Weight = 9.0f };
            uniqueHeavyArmors.Armor["Shellbug Helmet"] = new ArmorPiece { Identifiers = "012E8A:Dawnguard.esm", Rating = 22f, Weight = 6.0f };
            uniqueHeavyArmors.Armor["Dented Iron Shield"] = new ArmorPiece { Identifiers = "03399C:Dragonborn.esm", Rating = 15f, Weight = 12.0f };
            uniqueHeavyArmors.Armor["Cultist Mask"] = new ArmorPiece { Identifiers = "037B88:Dragonborn.esm", Rating = 17f, Weight = 5.0f };
            uniqueHeavyArmors.Armor["Visage of Mzund"] = new ArmorPiece { Identifiers = "038ADD:Dragonborn.esm", Rating = 24f, Weight = 12.0f };
            uniqueHeavyArmors.Armor["Kenro's Helmet"] = new ArmorPiece { Identifiers = "15E949:ccasvsse001-almsivi.esm", Rating = 19f, Weight = 6.0f };
            uniqueHeavyArmors.Armor["Ordinator Shield"] = new ArmorPiece { Identifiers = "06668E:ccasvsse001-almsivi.esm", Rating = 26f, Weight = 8.0f };
            uniqueHeavyArmors.Armor["Wraithguard"] = new ArmorPiece { Identifiers = "000D63:ccbgssse008-wraithguard.esl", Rating = 19f, Weight = 8.0f };
            uniqueHeavyArmors.Armor["Lord's Mail"] = new ArmorPiece { Identifiers = "Lord's Mail", Rating = 45f, Weight = 32.0f };
            uniqueHeavyArmors.Armor["Dismal Visage"] = new ArmorPiece { Identifiers = "000867:ccbgssse059-ba_dragonplate.esl", Rating = 12f, Weight = 5.0f };
            uniqueHeavyArmors.Armor["Fists of Randagulf"] = new ArmorPiece { Identifiers = "000813:ccbgssse069-contest.esl", Rating = 15f, Weight = 8.0f };
            uniqueHeavyArmors.Armor["Skull-Sabre Shield"] = new ArmorPiece { Identifiers = "000899:cckrtsse001_altar.esl", Rating = 28f, Weight = 10.0f };
            uniqueHeavyArmors.Armor["Grand Champion's Helm"] = new ArmorPiece { Identifiers = "000CDC:cckrtsse001_altar.esl", Rating = 20f, Weight = 6.0f };
            uniqueHeavyArmors.Armor["Helm of Oreyn Bearclaw"] = new ArmorPiece { Identifiers = "000D3D:ccrmssse001-necrohouse.esl", Rating = 22f, Weight = 8.0f };
            uniqueHeavyArmors.Armor["Ward of the Seasons"] = new ArmorPiece { Identifiers = "000893:cctwbsse001-puzzledungeon.esm", Rating = 34f, Weight = 45.0f };
            uniqueHeavyArmors.Armor["Undying Ghost Helmet"] = new ArmorPiece { Identifiers = "000886:ccvsvsse003-necroarts.esl", Rating = 21f, Weight = 8.0f };
            uniqueHeavyArmors.Armor["Auriel's Shield"] = new ArmorPiece { Identifiers = "00284D:Dawnguard.esm", Rating = 32f, Weight = 14.0f };
            uniqueHeavyArmors.Armor["Aetherial Shield"] = new ArmorPiece { Identifiers = "005759:Dawnguard.esm", Rating = 26f, Weight = 12.0f };
            UniqueHeavyArmors.Armor["Unique Heavy Armors"] = uniqueHeavyArmors;

            var heavyDragonPriestHelmets = new ArmorType
            {
                Identifiers = "061CA5:Skyrim.esm;061CC1:Skyrim.esm;061CC2:Skyrim.esm;061CC0:Skyrim.esm;061CC9:Skyrim.esm;061CD6:Skyrim.esm;" +
            "0240FE:Dragonborn.esm;0240FF:Dragonborn.esm;024037:Dragonborn.esm;00081F:ccasvsse001-almsivi.esm;000821:ccasvsse001-almsivi.esm;000820:ccasvsse001-almsivi.esm;" +
            "000822::ccasvsse001-almsivi.esm",
                Type = ArmorTypeEnum.HeavyArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            heavyDragonPriestHelmets.Armor["Nahkriin"] = new ArmorPiece { Identifiers = "061CA5:Skyrim.esm", Rating = 23f, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Hevnoraak"] = new ArmorPiece { Identifiers = "061CC1:Skyrim.esm", Rating = 23f, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Otar"] = new ArmorPiece { Identifiers = "061CC2:Skyrim.esm", Rating = 23f, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Rahgot"] = new ArmorPiece { Identifiers = "061CC0:Skyrim.esm", Rating = 23, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Vokun"] = new ArmorPiece { Identifiers = "061CC9:Skyrim.esm", Rating = 23, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Konahrik"] = new ArmorPiece { Identifiers = "061CD6:Skyrim.esm", Rating = 24, Weight = 7.0f };
            heavyDragonPriestHelmets.Armor["Ahzidal"] = new ArmorPiece { Identifiers = "0240FE:Dragonborn.esm", Rating = 23, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Dukaan"] = new ArmorPiece { Identifiers = "0240FF:Dragonborn.esm", Rating = 23, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Zahkriisos"] = new ArmorPiece { Identifiers = "024037:Dragonborn.esm", Rating = 23, Weight = 9.0f };
            heavyDragonPriestHelmets.Armor["Almalexia"] = new ArmorPiece { Identifiers = "00081F:ccasvsse001-almsivi.esm", Rating = 19, Weight = 6.0f };
            heavyDragonPriestHelmets.Armor["Dagoth Ur"] = new ArmorPiece { Identifiers = "000821:ccasvsse001-almsivi.esm", Rating = 19, Weight = 6.0f };
            heavyDragonPriestHelmets.Armor["Sotha Sil"] = new ArmorPiece { Identifiers = "000820:ccasvsse001-almsivi.esm", Rating = 19, Weight = 6.0f };
            heavyDragonPriestHelmets.Armor["Vivec"] = new ArmorPiece { Identifiers = "000822:ccasvsse001-almsivi.esm", Rating = 19 };
            UniqueHeavyArmors.Armor["Masks Heavy"] = heavyDragonPriestHelmets;

            var miraakHeavy = new ArmorType
            {
                Identifiers = "029A62:Dragonborn.esm;039FA1:Dragonborn.esm;039FA2:Dragonborn.esm;039FA3:Dragonborn.esm",
                Type = ArmorTypeEnum.HeavyArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            miraakHeavy.Armor["Miraak's Mask"] = new ArmorPiece { Identifiers = "029A62:Dragonborn.esm", Rating = 23f, Weight = 9.0f };
            miraakHeavy.Armor["Miraak's Mask lvl 1"] = new ArmorPiece { Identifiers = "039FA1:Dragonborn.esm", Rating = 23f, Weight = 9.0f };
            miraakHeavy.Armor["Miraak's Mask lvl 2"] = new ArmorPiece { Identifiers = "039FA2:Dragonborn.esm", Rating = 25f, Weight = 9.0f };
            miraakHeavy.Armor["Miraak's Mask lvl 3"] = new ArmorPiece { Identifiers = "039FA3:Dragonborn.esm", Rating = 27f, Weight = 9.0f };
            UniqueHeavyArmors.Armor["Miraak's Mask"] = miraakHeavy;

            var shieldOfSolitude = new ArmorType
            {
                Identifiers = "09E023:Skyrim.esm;10EB62:Skyrim.esm;10EB64:Skyrim.esm;10EB63:Skyrim.esm;10EB65:Skyrim.esm",
                Type = ArmorTypeEnum.HeavyArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            shieldOfSolitude.Armor["Shield of Solitude lvl 12"] = new ArmorPiece { Identifiers = "09E023:Skyrim.esm", Rating = 26f, Weight = 12.0f };
            shieldOfSolitude.Armor["Shield of Solitude lvl 18"] = new ArmorPiece { Identifiers = "10EB62:Skyrim.esm", Rating = 27f, Weight = 12.0f };
            shieldOfSolitude.Armor["Shield of Solitude lvl 25"] = new ArmorPiece { Identifiers = "10EB64:Skyrim.esm", Rating = 28f, Weight = 12.0f };
            shieldOfSolitude.Armor["Shield of Solitude lvl 32"] = new ArmorPiece { Identifiers = "10EB63:Skyrim.esm", Rating = 30f, Weight = 12.0f };
            shieldOfSolitude.Armor["Shield of Solitude lvl 40"] = new ArmorPiece { Identifiers = "10EB65:Skyrim.esm", Rating = 32f, Weight = 12.0f };
            UniqueHeavyArmors.Armor["Shield of Solitude"] = shieldOfSolitude;

            var dwarvenCrown = new ArmorType
            {
                Identifiers = "00089E:cctwbsse001-puzzledungeon.esm;000892:cctwbsse001-puzzledungeon.esm;00088F:cctwbsse001-puzzledungeon.esm;00089F:cctwbsse001-puzzledungeon.esm",
                Type = ArmorTypeEnum.HeavyArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            dwarvenCrown.Armor["Dwarven Crown"] = new ArmorPiece { Identifiers = "00089E:cctwbsse001-puzzledungeon.esm", Rating = 16f, Weight = 9.0f };
            dwarvenCrown.Armor["Dwarven Crown of Spring"] = new ArmorPiece { Identifiers = "000892:cctwbsse001-puzzledungeon.esm", Rating = 22f, Weight = 12.0f };
            dwarvenCrown.Armor["Dwarven Crown of Autumn"] = new ArmorPiece { Identifiers = "00088F:cctwbsse001-puzzledungeon.esm", Rating = 22f, Weight = 12.0f };
            dwarvenCrown.Armor["Dwarven Crown of Winter"] = new ArmorPiece { Identifiers = "00089F:cctwbsse001-puzzledungeon.esm", Rating = 22f, Weight = 12.0f };
            UniqueHeavyArmors.Armor["Dwarven Crown"] = dwarvenCrown;

            var dwarvenVisage = new ArmorType
            {
                Identifiers = "000894:cctwbsse001-puzzledungeon.esm;000890:cctwbsse001-puzzledungeon.esm;000895:cctwbsse001-puzzledungeon.esm",
                Type = ArmorTypeEnum.HeavyArmor,
                MatchBehavior = ArmorMatchBehavior.Pass
            };
            dwarvenVisage.Armor["Dwarven Spring Visage"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 8f, Weight = 5.0f };
            UniqueHeavyArmors.Armor["Dwarven Visage"] = dwarvenVisage;
        }

        private void InitializeOtherHeavyArmors()
        {
            var moragtongArmor = new ArmorType { Identifiers = "DLC2ArmorMaterialMoragTong", Type = ArmorTypeEnum.HeavyArmor };
            moragtongArmor.Armor["Armor"] = new ArmorPiece { Identifiers = "ArmorCuirass", Rating = 40f, Weight = 35.0f };
            moragtongArmor.Armor["Boots"] = new ArmorPiece { Identifiers = "ArmorBoots", Rating = 14f, Weight = 6.0f };
            moragtongArmor.Armor["Gauntlets"] = new ArmorPiece { Identifiers = "ArmorGauntlets", Rating = 14f, Weight = 5.0f };
            moragtongArmor.Armor["Helmet"] = new ArmorPiece { Identifiers = "ArmorHelmet", Rating = 19f, Weight = 5.0f };
            moragtongArmor.Armor["Shield"] = new ArmorPiece { Identifiers = "ArmorShield", Rating = 26f, Weight = 10.0f };
            OtherHeavyArmors.Armor["Morag Tong"] = moragtongArmor;

            var khajiitWillFollow = new ArmorType { Identifiers = "50C777:KhajiitWillFollow.esp", Type = ArmorTypeEnum.HeavyArmor };
            khajiitWillFollow.Armor["Absolution"] = new ArmorPiece { Identifiers = "50C777:KhajiitWillFollow.esp", Rating = 46f, Weight = 38.0f };
            OtherHeavyArmors.Armor["Khajiit Will Follow"] = khajiitWillFollow;

            var skyrimExtendedCutSaintsAndSeducers = new ArmorType { Identifiers = "02E7D2:Skyrim Extended Cut - Saints and Seducers.esp", Type = ArmorTypeEnum.HeavyArmor };
            skyrimExtendedCutSaintsAndSeducers.Armor["Cast Iron Hat"] = new ArmorPiece { Identifiers = "02E7D2:Skyrim Extended Cut - Saints and Seducers.esp", Rating = 10f, Weight = 3.0f };
            OtherHeavyArmors.Armor["Skyrim Extended Cut - Saints and Seducers"] = skyrimExtendedCutSaintsAndSeducers;

        }

        [SettingName("Debug Mode")]
        [Tooltip("Enable debug logging")]
        public bool DebugMode { get; set; } = false;

        [SettingName("Plugin Processing")]
        [Tooltip("Choose which plugins to process")]
        public PluginFilter PluginFilter { get; set; } = PluginFilter.IncludePlugins;

        [SettingName("Plugin Include List")]
        [Tooltip("Only process plugins in this list (comma separated)")]
        public string PluginIncludeList { get; set; } =
            "Skyrim.esm;" +
            "Dawnguard.esm;" +
            "Dragonborn.esm;" +
            "ccasvsse001-almsivi.esm;" +
            "ccbgssse008-wraithguard.esl;" +
            "ccbgssse021-lordsmail.esl;" +
            "ccbgssse025-advdsgs.esm;" +
            "ccbgssse041-netchleather.esl;" +
            "ccbgssse050-ba_daedric.esl;" +
            "ccbgssse051-ba_daedricmail.esl;" +
            "ccbgssse052-ba_iron.esl;" +
            "ccbgssse053-ba_leather.esl;" +
            "ccbgssse054-ba_orcish.esl; " +
            "ccbgssse055-ba_orcishscaled.esl;" +
            "ccbgssse056-ba_silver.esl;" +
            "ccbgssse057-ba_stalhrim.esl;" +
            "ccbgssse058-ba_steel.esl;" +
            "ccbgssse059-ba_dragonplate.esl;" +
            "ccbgssse060-ba_dragonscale.esl;" +
            "ccbgssse061-ba_dwarven.esl; " +
            "ccbgssse062-ba_dwarvenmail.esl;" +
            "ccbgssse063-ba_ebony.esl;" +
            "ccbgssse064-ba_elven.esl;" +
            "ccbgssse069-contest.esl;" +
            "cccbhsse001-gaunt.esl;" +
            "ccedhsse002-splkntset.esl;" +
            "ccedhsse003-redguard.esl;" +
            "ccffbsse001-imperialdragon.esl; " +
            "cckrtsse001_altar.esl;" +
            "ccmtysse001-knightsofthenine.esl;" +
            "ccmtysse002-ve.esl;" +
            "ccrmssse001-necrohouse.esl;" +
            "cctwbsse001-puzzledungeon.esm;" +
            "ccvsvsse003-necroarts.esl;" +
            "Dwarfsphere.esp; " +
            "Girl's Travel Outfit.esp;" +
            "GVLT_LifeswornVestige.esp;" +
            "Hothtrooper44_ArmorCompilation.esp;" +
            "Kad_MoonMonkRobes.esp;" +
            "KhajiitWillFollow.esp;" +
            "Lollygagging Fashions.esp;" +
            "Midwood Isle.esp; " +
            "MoonAndStar_MAS.esp;" +
            "moonpath.esp;" +
            "PigIronSet.esl;" +
            "RihadSwordsmanSet.esl;" +
            "SaviorsHideStandalone.esp;" +
            "Skyrim Extended Cut - Saints and Seducers.esp;" +
            "_Fuse00_ArmorRaven.esp;" +
            "[FB] Master Thief Armor.esp; " +
            "[FB] Templar Assassin.esp;" +
            "018Auri.esp;" +
            "Ashe - Fire and Blood.esp;" +
            "BSMBonemoldSet.esp;" +
            "ForswornArmorVariantsFurMashup.esp;" +
            "ForswornHeaddressVariants.esp;" +
            "JS Armored Circlets SE.esp; " +
            "BSHeartland.esm;" +
            "BSAssets.esm;";

        [SettingName("Plugin Exclude List")]
        [Tooltip("Skip plugins in this list (comma separated)")]
        public string PluginExcludeList { get; set; } = "";

        [SettingName("Ignored Armor Form Keys")]
        [Tooltip("Skip armors with these form keys (semicolon separated)")]
        public string IgnoredArmorFormKeys { get; set; } = "0C33BD:Skyrim.esm;0B8837:Skyrim.esm;015023:MoonAndStar_MAS.esp";

        [SettingName("Modify Ratings")]
        [Tooltip("Modify armor ratings")]
        public bool ModifyRatings { get; set; } = true;

        [SettingName("Modify Weights")]
        [Tooltip("Modify armor weights")]
        public bool ModifyWeights { get; set; } = true;

        [SettingName("Remove rating for armor pieces without armor keywords")]
        [Tooltip("Remove rating from parsed armor pieces that do not have any armor keywords")]
        public bool RemoveRatingForUnknownArmorPieces { get; set; } = false;

        [SettingName("Remove weight for armor pieces without armor keywords")]
        [Tooltip("Remove weight from parsed armor pieces that do not have any armor keywords")]
        public bool RemoveWeightForUnknownArmorPieces { get; set; } = false;

        [SettingName("Armor piece keywords")]
        [Tooltip("Armor piece keywords to use when removing rating/weight (semicolon separated)")]
        public string ArmorPieceKeywords { get; set; } = "ArmorCuirass;ArmorBoots;ArmorGauntlets;ArmorHelmet;ArmorShield";

        [SettingName("Remove rating for armor pieces without armor slots")]
        [Tooltip("Remove rating from parsed armor pieces that do not have any armor slots")]
        public bool RemoveRatingForUnknownArmorSlots { get; set; } = false;

        [SettingName("Remove weight for armor pieces without armor slots")]
        [Tooltip("Remove weight from parsed armor pieces that do not have any armor slots")]
        public bool RemoveWeightForUnknownArmorSlots { get; set; } = false;

        [SettingName("Armor slots")]
        [Tooltip("Armor slots to use when removing rating/weight (semicolon separated xEdit decimal values)")]
        public string ArmorSlots { get; set; } = "30;31;32;33;37;39;42;46";

        [JsonProperty]
        public ArmorCategory VanillaLightArmors { get; set; }

        [JsonProperty]
        public ArmorCategory DawnguardLightArmors { get; set; }

        [JsonProperty]
        public ArmorCategory DragonbornLightArmors { get; set; }

        [JsonProperty]
        public ArmorCategory CreationClubLightArmors { get; set; }

        [JsonProperty]
        public ArmorCategory UniqueLightArmors { get; set; }
        [JsonProperty]
        public ArmorCategory OtherLightArmors { get; set; }

        [JsonProperty]
        public ArmorCategory VanillaHeavyArmors { get; set; }

        [JsonProperty]
        public ArmorCategory DawnguardHeavyArmors { get; set; }

        [JsonProperty]
        public ArmorCategory DragonbornHeavyArmors { get; set; }

        [JsonProperty]
        public ArmorCategory CreationClubHeavyArmors { get; set; }

        [JsonProperty]
        public ArmorCategory UniqueHeavyArmors { get; set; }

        [JsonProperty]
        public ArmorCategory OtherHeavyArmors { get; set; }

        [JsonProperty]
        public ArmorCategory ClothingArmors { get; set; }

        public IEnumerable<ArmorCategory> GetAllArmorCategories()
        {
            yield return VanillaLightArmors;
            yield return DawnguardLightArmors;
            yield return DragonbornLightArmors;
            yield return CreationClubLightArmors;
            yield return OtherLightArmors;
            yield return VanillaHeavyArmors;
            yield return DawnguardHeavyArmors;
            yield return DragonbornHeavyArmors;
            yield return CreationClubHeavyArmors;
            yield return OtherHeavyArmors;
            yield return ClothingArmors;
        }

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