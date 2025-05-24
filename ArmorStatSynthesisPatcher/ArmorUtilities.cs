using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ArmourStatsSynthesisPatcher
{
    public static class UsesTemplate
    {
        public static bool Check(IArmorGetter armor, ILinkCache linkCache)
        {
            return !armor.TemplateArmor.IsNull;
        }
    }

    public static class IsPlayable
    {
        public static bool Check(IArmorGetter armor)
        {
            return !armor.MajorFlags.HasFlag(Armor.MajorFlag.NonPlayable);
        }
    }

    public static class ArmorUtilities
    {
        public static bool HasKeyword(IArmorGetter armor, string keyword, ILinkCache linkCache)
        {
            return armor.Keywords?.Any(k =>
            {
                if (k.TryResolve(linkCache, out var keywordRecord))
                {
                    return keywordRecord.EditorID == keyword;
                }
                return false;
            }) ?? false;
        }

        public static void PrintProperties(object obj, int indent = 0, int maxDepth = 2)
        {
            if (obj == null || indent > maxDepth) return;
            var type = obj.GetType();
            var indentStr = new string(' ', indent * 2);
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                object? value = null;
                try { value = prop.GetValue(obj); } catch { value = "<unreadable>"; }
                if (value == null)
                {
                    Console.WriteLine($"{indentStr}{prop.Name}: null");
                }
                else if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum)
                {
                    Console.WriteLine($"{indentStr}{prop.Name}: {value}");
                }
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    Console.WriteLine($"{indentStr}{prop.Name}: [");
                    var enumerable = value as System.Collections.IEnumerable;
                    int count = 0;
                    foreach (var item in enumerable!)
                    {
                        if (count++ > 5) { Console.WriteLine($"{indentStr}  ..."); break; }
                        PrintProperties(item, indent + 2, maxDepth);
                    }
                    Console.WriteLine($"{indentStr}]");
                }
                else
                {
                    Console.WriteLine($"{indentStr}{prop.Name}:");
                    PrintProperties(value, indent + 1, maxDepth);
                }
            }
        }

        /// <summary>
        /// Validates a form key string and checks if it points to an armor record.
        /// </summary>
        /// <param name="formKeyString">The form key string in format "FormID:PluginName.esp"</param>
        /// <param name="linkCache">The link cache to verify the form key against</param>
        /// <returns>
        /// - true: The form key is valid and points to an armor record in the load order
        /// - false: The form key is invalid or points to a non-armor record
        /// - null: The form key is valid but its plugin is not in the load order
        /// </returns>
        public static bool? IsValidFormKey(string formKeyString, ILinkCache linkCache)
        {
            if (string.IsNullOrWhiteSpace(formKeyString)) return false;

            // Try to create a FormKey using Mutagen's factory
            if (!FormKey.TryFactory(formKeyString, out var formKey)) return false;

            // Try to resolve the form key in the link cache
            // This will return false if:
            // 1. The plugin isn't in the load order
            // 2. The form ID doesn't exist in the plugin
            // 3. The record type doesn't match IArmorGetter
            if (!linkCache.TryResolve<IArmorGetter>(formKey, out _))
            {
                // Check if the plugin exists in the load order
                // If it doesn't, return null to indicate missing plugin
                return linkCache.ListedOrder.Any(x => x.ModKey.Equals(formKey.ModKey)) ? false : null;
            }

            return true;
        }

        /// <summary>
        /// Checks if there are any duplicate identifiers across armor types with the same armor type enum.
        /// </summary>
        /// <param name="armorTypes">Collection of armor types to check</param>
        /// <returns>List of duplicate identifiers found</returns>
        public static List<(string Identifier, string ArmorTypeName)> FindDuplicateArmorTypeIdentifiers(IEnumerable<ArmorType> armorTypes)
        {
            var duplicates = new List<(string Identifier, string ArmorTypeName)>();
            var identifierMap = new Dictionary<string, string>();

            foreach (var armorType in armorTypes)
            {
                var identifiers = armorType.Identifiers
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var identifier in identifiers)
                {
                    if (identifierMap.TryGetValue(identifier, out var existingArmorType))
                    {
                        // Only report duplicates if they have the same armor type enum
                        var existingArmorTypeObj = armorTypes.First(at => at.Identifiers.Contains(existingArmorType));
                        if (existingArmorTypeObj.Type == armorType.Type)
                        {
                            duplicates.Add((identifier, armorType.Identifiers));
                        }
                    }
                    else
                    {
                        identifierMap[identifier] = armorType.Identifiers;
                    }
                }
            }

            return duplicates;
        }

        /// <summary>
        /// Checks if there are any duplicate identifiers within an armor type's pieces.
        /// </summary>
        /// <param name="armorType">The armor type to check</param>
        /// <returns>List of duplicate identifiers found</returns>
        public static List<(string Identifier, string ArmorPieceName)> FindDuplicateArmorPieceIdentifiers(ArmorType armorType)
        {
            var duplicates = new List<(string Identifier, string ArmorPieceName)>();
            var identifierMap = new Dictionary<string, string>();

            foreach (var piece in armorType.Armor)
            {
                var identifiers = piece.Value.Identifiers
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var identifier in identifiers)
                {
                    if (identifierMap.TryGetValue(identifier, out var existingPiece))
                    {
                        duplicates.Add((identifier, piece.Key));
                    }
                    else
                    {
                        identifierMap[identifier] = piece.Key;
                    }
                }
            }

            return duplicates;
        }

        /// <summary>
        /// Checks if any armor piece in the given armor types has a missing or zero rating value.
        /// </summary>
        /// <param name="armorTypes">Collection of armor types to check</param>
        /// <returns>List of armor pieces with missing ratings, containing their armor type name and piece name</returns>
        public static List<(string ArmorTypeName, string ArmorPieceName)> FindMissingRatings(IEnumerable<ArmorType> armorTypes)
        {
            var missingRatings = new List<(string ArmorTypeName, string ArmorPieceName)>();

            foreach (var armorType in armorTypes)
            {
                foreach (var piece in armorType.Armor)
                {
                    if (piece.Value.Rating < 0)
                    {
                        missingRatings.Add((armorType.Identifiers, piece.Key));
                    }
                }
            }

            return missingRatings;
        }

        /// <summary>
        /// Checks if any armor piece in the given armor types has a missing or zero weight value.
        /// </summary>
        /// <param name="armorTypes">Collection of armor types to check</param>
        /// <returns>List of armor pieces with missing weights, containing their armor type name and piece name</returns>
        public static List<(string ArmorTypeName, string ArmorPieceName)> FindMissingWeights(IEnumerable<ArmorType> armorTypes)
        {
            var missingWeights = new List<(string ArmorTypeName, string ArmorPieceName)>();

            foreach (var armorType in armorTypes)
            {
                foreach (var piece in armorType.Armor)
                {
                    if (piece.Value.Weight < 0)
                    {
                        missingWeights.Add((armorType.Identifiers, piece.Key));
                    }
                }
            }

            return missingWeights;
        }
    }
}