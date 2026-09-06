using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace ArmorStatSynthesisPatcher
{
    public class Program
    {
        private static Settings CurrentSettings = new Settings();

        // Developer-only diagnostic switch. Keep this out of user settings/UI.
        // When enabled, every matched armor's final rating and weight are raised
        // by one so the record is emitted even when it already has desired stats.
        private static bool DeveloperShiftMatchedArmorStatsByOne = false;

        private static void DebugLog(string message)
        {
            if (CurrentSettings.DebugMode)
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

        internal class ArmorMatcher
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

            private static List<T> GetFormIdMatches<T>(IArmorGetter armor, IEnumerable<(string Identifier, T Result)> patterns)
            {
                var armorFormKey = armor.FormKey.ToString();
                return patterns
                    .Where(p => p.Identifier.Equals(armorFormKey, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.Result)
                    .Distinct()
                    .ToList();
            }

            private static T? GetFirstNameMatch<T>(IArmorGetter armor, IEnumerable<(string Identifier, T Result)> patterns)
            {
                var armorName = armor.Name?.String?.ToLowerInvariant() ?? "";
                if (string.IsNullOrEmpty(armorName)) return default;

                foreach (var (identifier, result) in patterns)
                {
                    if (Regex.IsMatch(armorName, $@"\b{Regex.Escape(identifier.ToLowerInvariant())}\b", RegexOptions.IgnoreCase))
                    {
                        return result;
                    }
                }

                return default;
            }

            private static List<T> GetKeywordMatches<T>(IArmorGetter armor, IEnumerable<(string Identifier, T Result)> patterns, ILinkCache linkCache)
            {
                var matches = new List<T>();
                if (armor.Keywords == null) return matches;

                foreach (var keyword in armor.Keywords)
                {
                    if (!keyword.TryResolve(linkCache, out var keywordRecord)) continue;

                    var keywordName = keywordRecord.EditorID?.ToLowerInvariant() ?? "";
                    foreach (var (identifier, result) in patterns)
                    {
                        if (Regex.IsMatch(keywordName, $@"\b{Regex.Escape(identifier.ToLowerInvariant())}\b", RegexOptions.IgnoreCase))
                        {
                            if (!matches.Contains(result))
                            {
                                matches.Add(result);
                            }
                        }
                    }
                }

                return matches;
            }

            private List<ArmorType> GetMatchingArmorTypes(
                IArmorGetter armor,
                IEnumerable<(string Identifier, ArmorType Result)> patterns,
                ILinkCache linkCache)
            {
                DebugLog("Checking FormID matches for armor type");
                var formIdMatches = GetFormIdMatches(armor, patterns);
                if (formIdMatches.Count > 1)
                {
                    LogWarning($"Multiple armor type FormID matches for {armor.FormKey}. Using the first match. This is likely a settings error.");
                }
                if (formIdMatches.Count > 0)
                {
                    return new List<ArmorType> { formIdMatches[0] };
                }

                DebugLog("Checking name matches for armor type");
                var nameMatch = GetFirstNameMatch(armor, patterns);
                if (nameMatch != null)
                {
                    return new List<ArmorType> { nameMatch };
                }

                DebugLog("Checking keyword matches for armor type");
                return GetKeywordMatches(armor, patterns, linkCache);
            }

            private ArmorPiece? GetBestMatchingArmorPiece(
                IArmorGetter armor,
                IEnumerable<(string Identifier, ArmorPiece Result)> patterns,
                ILinkCache linkCache)
            {
                var formIdMatches = GetFormIdMatches(armor, patterns);
                if (formIdMatches.Count > 0)
                {
                    return formIdMatches
                        .OrderByDescending(piece => piece.Rating)
                        .First();
                }

                var nameMatch = GetFirstNameMatch(armor, patterns);
                if (nameMatch != null)
                {
                    return nameMatch;
                }

                return GetKeywordMatches(armor, patterns, linkCache)
                    .OrderByDescending(piece => piece.Rating)
                    .FirstOrDefault();
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
                var matchedArmorTypes = GetMatchingArmorTypes(armor, patterns, linkCache);
                if (!matchedArmorTypes.Any())
                {
                    DebugLog("No match found for armor");
                    return (false, false, null, null);
                }

                ArmorType? bestArmorType = null;
                ArmorPiece? bestArmorPiece = null;
                float bestRating = float.MinValue;

                foreach (var armorType in matchedArmorTypes)
                {
                    var pieces = armorType.Armor.Values
                        .SelectMany(p => p.Identifiers
                            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(id => (Identifier: id, Result: p)))
                        .OrderByDescending(x => x.Identifier.Length)
                        .ToList();

                    DebugLog($"Available pieces to match against from set {armorType.Identifiers}:");
                    foreach (var piece in pieces)
                    {
                        DebugLog($"  Piece: {piece.Identifier}");
                        DebugLog($"    Rating: {piece.Result.Rating}");
                        DebugLog($"    Weight: {piece.Result.Weight}");
                    }

                    var matchingPiece = GetBestMatchingArmorPiece(armor, pieces, linkCache);

                    // If there's only one piece in the armor type and it has the same identifier as the armor type,
                    // we can include that piece directly as a candidate
                    if (matchingPiece == null && pieces.Count == 1 && pieces[0].Identifier.Equals(armorType.Identifiers, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingPiece = pieces[0].Result;
                    }

                    if (matchingPiece != null && matchingPiece.Rating > bestRating)
                    {
                        bestRating = matchingPiece.Rating;
                        bestArmorType = armorType;
                        bestArmorPiece = matchingPiece;
                    }
                }

                if (bestArmorPiece != null && bestArmorType != null)
                {
                    DebugLog($"Selected set: {bestArmorType.Identifiers}");
                    DebugLog($"Matching succeeded with rating: {bestArmorPiece.Rating}");
                    DebugLog($"Matched piece weight: {bestArmorPiece.Weight}");
                    return (true, true, bestArmorType, bestArmorPiece);
                }

                DebugLog("No piece match found, but armor type matched");
                return (true, false, matchedArmorTypes[0], null);
            }
        }

        private static bool HasArmorPieceKeywords(IArmorGetter armor, ILinkCache linkCache)
        {
            if (armor.Keywords == null) return false;

            // Get armor piece keywords from settings
            var armorPieceKeywords = CurrentSettings.ArmorPieceKeywords
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
            var armorSlots = CurrentSettings.ArmorSlots
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
            var enabledArmorTypes = CurrentSettings.GetAllLightArmors().Concat(CurrentSettings.GetAllHeavyArmors()).Concat(CurrentSettings.GetAllClothing());

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
            var formKeyValidations = CurrentSettings.IgnoredArmorFormKeys
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

        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetOpenForSettings(OpenForSettings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "Armour_Stats_Synthesis_Patcher.esp")
                .Run(args);
        }

        public static int OpenForSettings(IOpenForSettingsState state)
        {
            try
            {
                Exception? openException = null;
                int openResult = 0;

                var uiThread = new Thread(() =>
                {
                    try
                    {
                        var app = Application.Current;
                        if (app == null)
                        {
                            app = new Application
                            {
                                ShutdownMode = ShutdownMode.OnExplicitShutdown
                            };
                        }

                        var settings = SettingsFileStore.Load(state.ExtraSettingsDataPath);
                        var window = new SettingsWindow(settings, state.ExtraSettingsDataPath)
                        {
                            ShowInTaskbar = true
                        };

                        _ = window.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        openException = ex;
                        openResult = 1;
                    }
                });

                uiThread.SetApartmentState(ApartmentState.STA);
                uiThread.Start();
                uiThread.Join();

                if (openException != null)
                {
                    LogError($"Failed to open custom settings UI: {openException}");
                    try
                    {
                        var logPath = Path.Combine(AppContext.BaseDirectory, "open-settings-error.log");
                        File.WriteAllText(logPath, openException.ToString());
                        MessageBox.Show(
                            $"Failed to open settings UI.\n\nDetails were written to:\n{logPath}",
                            "Armor Stats Synthesis Patcher",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    catch
                    {
                        // Best effort diagnostics only.
                    }
                }

                return openResult;
            }
            catch (Exception ex)
            {
                LogError($"Failed to open custom settings UI: {ex}");
                return 1;
            }
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            LogInfo("========================================");
            LogInfo("Starting Armor Stats Synthesis Patcher");
            LogInfo("========================================");

            if (DeveloperShiftMatchedArmorStatsByOne)
            {
                LogWarning("Developer stat shift is enabled: matched armor rating and weight will be increased by 1.");
            }

            CurrentSettings = SettingsFileStore.Load(state.ExtraSettingsDataPath);

            // Check settings integrity before proceeding
            CheckSettingsIntegrity(state);

            // Print settings
            DebugLog("Settings:");
            DebugLog($"  - PluginFilter: {CurrentSettings.PluginFilter}");
            DebugLog($"  - DebugMode: {CurrentSettings.DebugMode}");
            DebugLog($"  - ModifyRatings: {CurrentSettings.ModifyRatings}");
            DebugLog($"  - ModifyWeights: {CurrentSettings.ModifyWeights}");
            DebugLog($"  - RemoveRatingForUnknownArmorPieces: {CurrentSettings.RemoveRatingForUnknownArmorPieces}");
            DebugLog($"  - RemoveWeightForUnknownArmorPieces: {CurrentSettings.RemoveWeightForUnknownArmorPieces}");
            DebugLog($"  - RemoveRatingForUnknownArmorSlots: {CurrentSettings.RemoveRatingForUnknownArmorSlots}");
            DebugLog($"  - RemoveWeightForUnknownArmorSlots: {CurrentSettings.RemoveWeightForUnknownArmorSlots}");
            DebugLog($"  - ArmorSlots: {CurrentSettings.ArmorSlots}");

            // Get the list of plugins to include
            var pluginIncludeList = CurrentSettings.PluginIncludeList
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Get the list of plugins to exclude
            var pluginExcludeList = CurrentSettings.PluginExcludeList
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Get the list of ignored armor form keys
            var ignoredArmorFormKeys = CurrentSettings.IgnoredArmorFormKeys
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
            var matcher = new ArmorMatcher(CurrentSettings.GetAllLightArmors().Concat(CurrentSettings.GetAllHeavyArmors()).Concat(CurrentSettings.GetAllClothing()));

            // TEMPORARY: Produce the WACCF/CC Integration armor audit used to
            // design the WACCF settings preset.
            WaccfArmorAuditExporter.Export(state, CurrentSettings, matcher);

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
            if (CurrentSettings.PluginFilter == PluginFilter.IncludePlugins)
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
                var shouldProcess = CurrentSettings.PluginFilter switch
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
                var shouldProcess = CurrentSettings.PluginFilter switch
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
                var keywordNames = winningArmor.Keywords?
                    .Select(keyword => keyword.TryResolve(state.LinkCache, out var keywordRecord)
                        ? keywordRecord.EditorID ?? keyword.FormKey.ToString()
                        : keyword.FormKey.ToString())
                    .ToList() ?? new List<string>();
                DebugLog($"Keywords: {(keywordNames.Count > 0 ? string.Join("; ", keywordNames) : "<none>")}");

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
                        if (CurrentSettings.RemoveRatingForUnknownArmorPieces)
                        {
                            DebugLog($"  Removing rating for missing armor keywords");
                            finalRating = 0f;
                        }
                        if (CurrentSettings.RemoveWeightForUnknownArmorPieces)
                        {
                            DebugLog($"  Removing weight for missing armor keywords");
                            finalWeight = 0f;
                        }
                    }

                    // Override with zero if missing slots
                    if (!hasArmorSlots)
                    {
                        if (CurrentSettings.RemoveRatingForUnknownArmorSlots)
                        {
                            DebugLog($"  Removing rating for missing armor slots");
                            finalRating = 0f;
                        }
                        if (CurrentSettings.RemoveWeightForUnknownArmorSlots)
                        {
                            DebugLog($"  Removing weight for missing armor slots");
                            finalWeight = 0f;
                        }
                    }

                    bool modifyRating = CurrentSettings.ModifyRatings;
                    bool modifyWeight = CurrentSettings.ModifyWeights;
                    if (DeveloperShiftMatchedArmorStatsByOne)
                    {
                        finalRating += 1f;
                        finalWeight += 1f;
                        modifyRating = true;
                        modifyWeight = true;
                        DebugLog($"  Developer shift applied: rating {finalRating}, weight {finalWeight}");
                    }

                    // Check if we need to make any changes
                    bool needsChanges = false;
                    if (modifyRating && winningArmor.ArmorRating != finalRating)
                    {
                        needsChanges = true;
                        DebugLog($"  Rating change needed: {winningArmor.ArmorRating} -> {finalRating}");
                    }
                    if (modifyWeight && winningArmor.Weight != finalWeight)
                    {
                        needsChanges = true;
                        DebugLog($"  Weight change needed: {winningArmor.Weight} -> {finalWeight}");
                    }

                    // Only create an override if we need to make changes
                    if (needsChanges)
                    {
                        var overrideArmor = state.PatchMod.Armors.GetOrAddAsOverride(winningArmor);

                        // Update the armor rating and weight
                        if (modifyRating)
                        {
                            DebugLog($"  Current rating: {overrideArmor.ArmorRating}");
                            DebugLog($"  Setting rating to: {finalRating}");
                            overrideArmor.ArmorRating = finalRating;
                        }
                        if (modifyWeight)
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
