using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ArmourStatsSynthesisPatcher
{
    public class Program
    {
        static Lazy<Settings> Settings = null!;

        private static void DebugLog(string message)
        {
            if (Settings.Value.DebugMode)
            {
                Console.WriteLine(message);
            }
        }

        private static void LogWarning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }

        private static void LogError(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }

        private static void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        private class ArmorMatcher
        {
            private readonly List<(string Identifier, ArmorType Type)> _lightArmorPatterns;
            private readonly List<(string Identifier, ArmorType Type)> _heavyArmorPatterns;
            private readonly List<(string Identifier, ArmorType Type)> _clothingPatterns;

            public ArmorMatcher(IEnumerable<ArmorType> armorTypes)
            {
                _lightArmorPatterns = armorTypes
                    .Where(t => t.Type == ArmorTypeEnum.LightArmor)
                    .SelectMany(t => t.Identifiers
                        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(id => (Identifier: id, Type: t)))
                    .OrderByDescending(x => x.Identifier.Length)
                    .ToList();

                _heavyArmorPatterns = armorTypes
                    .Where(t => t.Type == ArmorTypeEnum.HeavyArmor)
                    .SelectMany(t => t.Identifiers
                        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(id => (Identifier: id, Type: t)))
                    .OrderByDescending(x => x.Identifier.Length)
                    .ToList();

                _clothingPatterns = armorTypes
                    .Where(t => t.Type == ArmorTypeEnum.Clothing)
                    .SelectMany(t => t.Identifiers
                        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(id => (Identifier: id, Type: t)))
                    .OrderByDescending(x => x.Identifier.Length)
                    .ToList();

                DebugLog("Initialized ArmorMatcher with patterns:");
                DebugLog("Light Armor Patterns:");
                foreach (var (identifier, type) in _lightArmorPatterns)
                {
                    DebugLog($"  Type: {type.Type}");
                    DebugLog($"  Identifier: {identifier}");
                }
                DebugLog("Heavy Armor Patterns:");
                foreach (var (identifier, type) in _heavyArmorPatterns)
                {
                    DebugLog($"  Type: {type.Type}");
                    DebugLog($"  Identifier: {identifier}");
                }
                DebugLog("Clothing Patterns:");
                foreach (var (identifier, type) in _clothingPatterns)
                {
                    DebugLog($"  Type: {type.Type}");
                    DebugLog($"  Identifier: {identifier}");
                }
            }

            private (bool Matched, T? Result) TryMatchIdentifiers<T>(
                IArmorGetter armor,
                IEnumerable<(string Identifier, T Result)> patterns,
                ILinkCache linkCache)
            {
                // Try matching against FormID first
                DebugLog("Checking FormID matches");
                var armorFormKey = armor.FormKey.ToString();
                foreach (var (identifier, result) in patterns)
                {
                    DebugLog($"  Checking against identifier: {identifier}");
                    if (identifier.Equals(armorFormKey, StringComparison.OrdinalIgnoreCase))
                    {
                        DebugLog($"  Found FormID match with identifier: {identifier}");
                        return (true, result);
                    }
                }

                // Try matching against armor name
                var armorName = armor.Name?.String?.ToLowerInvariant() ?? "";
                if (!string.IsNullOrEmpty(armorName))
                {
                    DebugLog($"Checking name match: {armorName}");
                    foreach (var (identifier, result) in patterns)
                    {
                        DebugLog($"  Checking against identifier: {identifier}");
                        if (Regex.IsMatch(armorName, $@"\b{Regex.Escape(identifier.ToLowerInvariant())}\b", RegexOptions.IgnoreCase))
                        {
                            DebugLog($"  Found name match with identifier: {identifier}");
                            return (true, result);
                        }
                    }
                }

                // Try matching against keywords
                if (armor.Keywords != null)
                {
                    DebugLog("Checking keyword matches");
                    // Sort patterns by rating for keyword matches
                    var sortedPatterns = patterns;
                    if (typeof(T) == typeof(ArmorPiece))
                    {
                        sortedPatterns = patterns.OrderByDescending(p =>
                        {
                            if (p.Result is ArmorPiece piece && piece != null)
                            {
                                return piece.Rating;
                            }
                            return 0;
                        });
                    }
                    else if (typeof(T) == typeof(ArmorType))
                    {
                        sortedPatterns = patterns.OrderByDescending(p =>
                        {
                            if (p.Result is ArmorType type && type != null && type.Armor != null)
                            {
                                return type.Armor.Values
                                    .Where(piece => piece != null)
                                    .Max(piece => piece.Rating);
                            }
                            return 0;
                        });
                    }

                    foreach (var keyword in armor.Keywords)
                    {
                        if (keyword.TryResolve(linkCache, out var keywordRecord))
                        {
                            var keywordName = keywordRecord.EditorID?.ToLowerInvariant() ?? "";
                            DebugLog($"  Checking keyword: {keywordName}");
                            foreach (var (identifier, result) in sortedPatterns)
                            {
                                DebugLog($"    Checking against identifier: {identifier}");
                                if (Regex.IsMatch(keywordName, $@"\b{Regex.Escape(identifier.ToLowerInvariant())}\b", RegexOptions.IgnoreCase))
                                {
                                    DebugLog($"    Found keyword match with identifier: {identifier}");
                                    return (true, result);
                                }
                            }
                        }
                    }
                }

                return (false, default);
            }

            public (bool TypeMatched, bool PieceMatched, ArmorType? Type, ArmorPiece? Piece) TryMatch(IArmorGetter armor, ILinkCache linkCache, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
            {
                DebugLog($"------------------------------------------");
                DebugLog($"Trying to match armor: {armor.EditorID}");
                DebugLog($"Name: {armor.Name?.String}");
                DebugLog($"Armor Type: {armor.BodyTemplate?.ArmorType}");

                var patterns = armor.BodyTemplate?.ArmorType switch
                {
                    Mutagen.Bethesda.Skyrim.ArmorType.LightArmor => _lightArmorPatterns,
                    Mutagen.Bethesda.Skyrim.ArmorType.HeavyArmor => _heavyArmorPatterns,
                    Mutagen.Bethesda.Skyrim.ArmorType.Clothing => _clothingPatterns,
                    _ => null
                };

                if (patterns == null)
                {
                    DebugLog("Unknown armor type, cannot match");
                    return (false, false, null, null);
                }

                // Try to match the armor type
                var (matched, armorType) = TryMatchIdentifiers(armor, patterns, linkCache);
                if (!matched || armorType == null)
                {
                    DebugLog("No match found for armor");
                    return (false, false, null, null);
                }

                // Now try to match the specific piece
                var pieces = armorType.Armor.Values
                    .SelectMany(p => p.Identifiers
                        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(id => (Identifier: id, Result: p)))
                    .OrderByDescending(x => x.Identifier.Length)
                    .ToList();

                DebugLog("Available pieces to match against:");
                foreach (var piece in pieces)
                {
                    DebugLog($"  Piece: {piece.Identifier}");
                    DebugLog($"    Rating: {piece.Result.Rating}");
                    DebugLog($"    Weight: {piece.Result.Weight}");
                }

                // If there's only one piece in the armor type and it has the same identifier as the armor type,
                // we can return that piece directly
                if (pieces.Count == 1 && pieces[0].Identifier.Equals(armorType.Identifiers, StringComparison.OrdinalIgnoreCase))
                {
                    DebugLog($"Found direct match with armor type identifier: {armorType.Identifiers}");
                    return (true, true, armorType, pieces[0].Result);
                }

                var (pieceMatched, matchedPiece) = TryMatchIdentifiers(armor, pieces, linkCache);
                if (pieceMatched && matchedPiece != null)
                {
                    DebugLog($"Matching succeeded with rating: {matchedPiece.Rating}");
                    DebugLog($"Matched piece weight: {matchedPiece.Weight}");
                    return (true, true, armorType, matchedPiece);
                }

                DebugLog("No piece match found, but armor type matched");
                return (true, false, armorType, null);
            }
        }

        private static bool HasArmorPieceKeywords(IArmorGetter armor, ILinkCache linkCache)
        {
            if (armor.Keywords == null) return false;

            // Get armor piece keywords from settings
            var armorPieceKeywords = Settings.Value.ArmorPieceKeywords
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var keyword in armor.Keywords)
            {
                if (keyword.TryResolve(linkCache, out var keywordRecord))
                {
                    var keywordName = keywordRecord.EditorID ?? "";
                    if (armorPieceKeywords.Contains(keywordName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool HasArmorSlots(IArmorGetter armor)
        {
            if (armor.BodyTemplate?.FirstPersonFlags == null) return false;

            // Get armor slots from settings and convert xEdit decimal values to Mutagen bitmasks
            var armorSlots = Settings.Value.ArmorSlots
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(slot => uint.TryParse(slot, out var value) ? value : 0u)
                .Where(slot => slot >= 30 && slot <= 61) // Valid xEdit slot range
                .Select(slot => 1u << (int)(slot - 30)) // Convert to Mutagen bitmask: 1 << (SlotID - 30)
                .Aggregate(0u, (current, bitmask) => current | bitmask); // Combine all bitmasks with OR

            // Check if any of the armor's FirstPersonFlags match the specified slots
            var armorFlags = armor.BodyTemplate.FirstPersonFlags;
            return (armorFlags & (BipedObjectFlag)armorSlots) != 0;
        }

        private static void CheckSettingsIntegrity(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            LogInfo("Checking settings integrity...");
            bool hasIssues = false;

            // Get all enabled armor types
            var enabledArmorTypes = Settings.Value.GetAllLightArmors().Concat(Settings.Value.GetAllHeavyArmors()).Concat(Settings.Value.GetAllClothing());

            // Get available plugins
            var availablePlugins = state.LoadOrder.PriorityOrder
                .Select(mod => mod.ModKey.FileName.String)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Check for missing ratings
            var missingRatings = ArmorUtilities.FindMissingRatings(enabledArmorTypes);
            if (missingRatings.Any())
            {
                hasIssues = true;
                LogWarning("Found armor pieces with missing or negative ratings:");
                foreach (var (armorType, piece) in missingRatings)
                {
                    LogWarning($"  - {armorType} -> {piece}");
                }
            }

            // Check for missing weights
            var missingWeights = ArmorUtilities.FindMissingWeights(enabledArmorTypes);
            if (missingWeights.Any())
            {
                hasIssues = true;
                LogWarning("Found armor pieces with missing or negative weights:");
                foreach (var (armorType, piece) in missingWeights)
                {
                    LogWarning($"  - {armorType} -> {piece}");
                }
            }

            // Check for duplicate identifiers in armor types
            var duplicateTypeIdentifiers = ArmorUtilities.FindDuplicateArmorTypeIdentifiers(enabledArmorTypes);
            if (duplicateTypeIdentifiers.Any())
            {
                hasIssues = true;
                LogWarning("Found duplicate identifiers across armor types:");
                foreach (var (identifier, armorType) in duplicateTypeIdentifiers)
                {
                    LogWarning($"  - Identifier '{identifier}' appears in multiple armor types: {armorType}");
                    // Disable all armor types with this identifier
                    foreach (var type in enabledArmorTypes.Where(t => t.Identifiers.Contains(identifier)))
                    {
                        type.MatchBehavior = ArmorMatchBehavior.DeactivateRule;
                        LogWarning($"    Disabled armor type: {type.Identifiers}");
                    }
                }
            }

            // Check for duplicate identifiers in armor pieces
            foreach (var armorType in enabledArmorTypes)
            {
                var duplicatePieceIdentifiers = ArmorUtilities.FindDuplicateArmorPieceIdentifiers(armorType);
                if (duplicatePieceIdentifiers.Any())
                {
                    hasIssues = true;
                    LogWarning($"Found duplicate identifiers in armor type '{armorType.Identifiers}':");
                    foreach (var (identifier, piece) in duplicatePieceIdentifiers)
                    {
                        LogWarning($"  - Identifier '{identifier}' appears in multiple pieces: {piece}");
                        // Remove the piece with the duplicate identifier
                        var piecesToRemove = armorType.Armor.Where(kvp => kvp.Value.Identifiers.Contains(identifier)).ToList();
                        foreach (var pieceToRemove in piecesToRemove)
                        {
                            armorType.Armor.Remove(pieceToRemove.Key);
                            LogWarning($"    Removed piece: {pieceToRemove.Key}");
                        }
                    }
                }
            }

            // Check ignored armor form keys
            var formKeyValidations = Settings.Value.IgnoredArmorFormKeys
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(formKey => (FormKey: formKey, IsValid: ArmorUtilities.IsValidFormKey(formKey, state.LinkCache)))
                .ToList();

            var invalidFormKeys = formKeyValidations
                .Where(x => x.IsValid == false) // Only warn about invalid records, not missing plugins
                .Select(x => x.FormKey)
                .ToList();

            if (invalidFormKeys.Any())
            {
                hasIssues = true;
                LogWarning("Found invalid form keys in ignored armor list (these are not valid armor records):");
                foreach (var formKey in invalidFormKeys)
                {
                    LogWarning($"  - {formKey}");
                }
            }

            if (!hasIssues)
            {
                LogInfo("Settings integrity check passed.");
            }
            else
            {
                LogWarning("Settings integrity check found issues.");
            }
        }

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(
                    nickname: "Settings",
                    path: "settings.json",
                    out Settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "Armour_Stats_Synthesis_Patcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            LogInfo("========================================");
            LogInfo("Starting Armor Stats Synthesis Patcher");
            LogInfo("========================================");

            // Check settings integrity before proceeding
            CheckSettingsIntegrity(state);

            // Print settings
            DebugLog("Settings:");
            DebugLog($"  - PluginFilter: {Settings.Value.PluginFilter}");
            DebugLog($"  - DebugMode: {Settings.Value.DebugMode}");
            DebugLog($"  - ModifyRatings: {Settings.Value.ModifyRatings}");
            DebugLog($"  - ModifyWeights: {Settings.Value.ModifyWeights}");
            DebugLog($"  - RemoveRatingForUnknownArmorPieces: {Settings.Value.RemoveRatingForUnknownArmorPieces}");
            DebugLog($"  - RemoveWeightForUnknownArmorPieces: {Settings.Value.RemoveWeightForUnknownArmorPieces}");
            DebugLog($"  - RemoveRatingForUnknownArmorSlots: {Settings.Value.RemoveRatingForUnknownArmorSlots}");
            DebugLog($"  - RemoveWeightForUnknownArmorSlots: {Settings.Value.RemoveWeightForUnknownArmorSlots}");
            DebugLog($"  - ArmorSlots: {Settings.Value.ArmorSlots}");

            // Get the list of plugins to include
            var pluginIncludeList = Settings.Value.PluginIncludeList
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Get the list of plugins to exclude
            var pluginExcludeList = Settings.Value.PluginExcludeList
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Get the list of ignored armor form keys
            var ignoredArmorFormKeys = Settings.Value.IgnoredArmorFormKeys
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(formKeyStr =>
                {
                    FormKey? formKey = null;
                    if (FormKey.TryFactory(formKeyStr, out var key))
                        formKey = key;
                    return formKey;
                })
                .Where(fk => fk.HasValue)
                .Select(fk => fk!.Value)
                .ToHashSet();

            // Create the armor matcher
            var matcher = new ArmorMatcher(Settings.Value.GetAllLightArmors().Concat(Settings.Value.GetAllHeavyArmors()).Concat(Settings.Value.GetAllClothing()));

            DebugLog("Plugin Include List:");
            foreach (var plugin in pluginIncludeList)
            {
                DebugLog($"     {plugin}");
            }
            DebugLog("Plugin Exclude List:");
            foreach (var plugin in pluginExcludeList)
            {
                DebugLog($"     {plugin}");
            }

            // Get all available plugins in load order
            var availablePlugins = state.LoadOrder.PriorityOrder
                .Select(mod => mod.ModKey.FileName.String)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Check for missing plugins if we're in include mode
            if (Settings.Value.PluginFilter == PluginFilter.IncludePlugins)
            {
                var missingPlugins = pluginIncludeList.Where(p => !availablePlugins.Contains(p)).ToList();
                if (missingPlugins.Any())
                {
                    LogWarning("The following plugins were listed for inclusion but could not be found:");
                    foreach (var plugin in missingPlugins)
                    {
                        LogWarning($"     {plugin}");
                    }
                    LogInfo("");
                }
            }

            DebugLog("Processing the following plugins:");
            foreach (var modGetter in state.LoadOrder.PriorityOrder)
            {
                var shouldProcess = Settings.Value.PluginFilter switch
                {
                    PluginFilter.AllPlugins => true,
                    PluginFilter.ExcludePlugins => !pluginExcludeList.Contains(modGetter.ModKey.FileName.String),
                    PluginFilter.IncludePlugins => pluginIncludeList.Contains(modGetter.ModKey.FileName.String),
                    _ => true
                };

                if (shouldProcess)
                {
                    DebugLog($"     {modGetter.ModKey.FileName}");
                }
            }
            DebugLog("");

            // Process armors from plugins
            foreach (var winningArmor in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                // Skip if the armor is in a plugin we don't want to process
                var shouldProcess = Settings.Value.PluginFilter switch
                {
                    PluginFilter.AllPlugins => true,
                    PluginFilter.ExcludePlugins => !pluginExcludeList.Contains(winningArmor.FormKey.ModKey.FileName.String),
                    PluginFilter.IncludePlugins => pluginIncludeList.Contains(winningArmor.FormKey.ModKey.FileName.String),
                    _ => true
                };

                if (!shouldProcess) continue;

                DebugLog($"------------------------------------------");
                DebugLog($"Processing armor: {winningArmor.EditorID}");
                DebugLog($"Name: {winningArmor.Name?.String}");
                DebugLog($"FormKey: {winningArmor.FormKey}");
                DebugLog($"Armor Type: {winningArmor.BodyTemplate?.ArmorType}");
                DebugLog($"Current Rating: {winningArmor.ArmorRating}");
                DebugLog($"Current Weight: {winningArmor.Weight}");

                // Skip if the armor is in the ignored list
                if (ignoredArmorFormKeys.Contains(winningArmor.FormKey))
                {
                    DebugLog("Skipping ignored armor");
                    continue;
                }

                // Skip if the armor uses a template
                if (UsesTemplate.Check(winningArmor, state.LinkCache))
                {
                    DebugLog("Skipping template armor");
                    continue;
                }

                // Skip if the armor is not playable
                if (!IsPlayable.Check(winningArmor))
                {
                    DebugLog("Skipping non-playable armor");
                    continue;
                }

                // Skip if the armor type is not Heavy or Light or Clothing
                if (winningArmor.BodyTemplate?.ArmorType != Mutagen.Bethesda.Skyrim.ArmorType.LightArmor &&
                    winningArmor.BodyTemplate?.ArmorType != Mutagen.Bethesda.Skyrim.ArmorType.HeavyArmor &&
                    winningArmor.BodyTemplate?.ArmorType != Mutagen.Bethesda.Skyrim.ArmorType.Clothing)
                {
                    DebugLog($"Skipping armor with unsupported type: {winningArmor.BodyTemplate?.ArmorType}");
                    continue;
                }

                // Try to match the armor
                var (typeMatched, pieceMatched, armorType, armorPiece) = matcher.TryMatch(winningArmor, state.LinkCache, state);

                // Process ALL armor pieces that match the armor type (Light/Heavy)
                if (typeMatched && armorType != null)
                {
                    // If the armor set to be passed, skip modifications
                    if (armorType.MatchBehavior == ArmorMatchBehavior.Pass)
                    {
                        DebugLog("Armor is set to be passed, skipping modifications");
                        continue;
                    }

                    // Check if armor has armor piece keywords and armor slots
                    bool hasVanillaKeywords = HasArmorPieceKeywords(winningArmor, state.LinkCache);
                    bool hasArmorSlots = HasArmorSlots(winningArmor);
                    DebugLog($"  Has vanilla armor keywords: {hasVanillaKeywords}");
                    DebugLog($"  Has armor slots: {hasArmorSlots}");
                    DebugLog($"  Type matched: {typeMatched}");
                    DebugLog($"  Piece matched: {pieceMatched}");

                    // Start with current values
                    float finalRating = winningArmor.ArmorRating;
                    float finalWeight = winningArmor.Weight;

                    // Apply configured values if piece was matched
                    if (pieceMatched && armorPiece != null)
                    {
                        DebugLog("  Applying configured values for matched piece");
                        finalRating = armorPiece.Rating;
                        finalWeight = armorPiece.Weight;
                    }

                    // Override with zero if missing keywords
                    if (!hasVanillaKeywords)
                    {
                        if (Settings.Value.RemoveRatingForUnknownArmorPieces)
                        {
                            DebugLog($"  Removing rating for missing armor keywords");
                            finalRating = 0f;
                        }
                        if (Settings.Value.RemoveWeightForUnknownArmorPieces)
                        {
                            DebugLog($"  Removing weight for missing armor keywords");
                            finalWeight = 0f;
                        }
                    }

                    // Override with zero if missing slots
                    if (!hasArmorSlots)
                    {
                        if (Settings.Value.RemoveRatingForUnknownArmorSlots)
                        {
                            DebugLog($"  Removing rating for missing armor slots");
                            finalRating = 0f;
                        }
                        if (Settings.Value.RemoveWeightForUnknownArmorSlots)
                        {
                            DebugLog($"  Removing weight for missing armor slots");
                            finalWeight = 0f;
                        }
                    }

                    // Check if we need to make any changes
                    bool needsChanges = false;
                    if (Settings.Value.ModifyRatings && winningArmor.ArmorRating != finalRating)
                    {
                        needsChanges = true;
                        DebugLog($"  Rating change needed: {winningArmor.ArmorRating} -> {finalRating}");
                    }
                    if (Settings.Value.ModifyWeights && winningArmor.Weight != finalWeight)
                    {
                        needsChanges = true;
                        DebugLog($"  Weight change needed: {winningArmor.Weight} -> {finalWeight}");
                    }

                    // Only create an override if we need to make changes
                    if (needsChanges)
                    {
                        var overrideArmor = state.PatchMod.Armors.GetOrAddAsOverride(winningArmor);

                        // Update the armor rating and weight
                        if (Settings.Value.ModifyRatings)
                        {
                            DebugLog($"  Current rating: {overrideArmor.ArmorRating}");
                            DebugLog($"  Setting rating to: {finalRating}");
                            overrideArmor.ArmorRating = finalRating;
                        }
                        if (Settings.Value.ModifyWeights)
                        {
                            DebugLog($"  Current weight: {overrideArmor.Weight}");
                            DebugLog($"  Setting weight to: {finalWeight}");
                            overrideArmor.Weight = finalWeight;
                        }

                        DebugLog($"Successfully processed armor:");
                        DebugLog($"  New Rating: {finalRating}");
                        DebugLog($"  New Weight: {finalWeight}");
                    }
                    else
                    {
                        DebugLog("Armor already has desired values, skipping modifications");
                    }
                }
                else
                {
                    DebugLog("No armor type match found for armor");
                }
            }

            LogInfo("========================================");
            LogInfo("Finished processing all armors");
            LogInfo("========================================");
        }
    }
}