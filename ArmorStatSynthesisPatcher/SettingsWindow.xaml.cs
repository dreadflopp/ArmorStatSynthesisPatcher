using System.Globalization;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using Newtonsoft.Json;

namespace ArmourStatsSynthesisPatcher
{
    public partial class SettingsWindow : Window
    {
        private readonly Settings _settings;
        private readonly string? _settingsFolder;
        private readonly TreeView _settingsTree;
        private readonly StackPanel _detailPanel;
        private readonly TextBlock _statusText;
        private readonly Button _closeButton;
        private readonly Button _saveButton;
        private string _lastSavedSettingsSnapshot = string.Empty;

        private sealed class PendingTextSnapshot
        {
            public required string OriginalText { get; init; }
        }

        private enum NodeKind
        {
            Global,
            Category,
            ArmorType
        }

        private sealed class NodeRef
        {
            public NodeKind Kind { get; init; }
            public PropertyInfo? CategoryProperty { get; init; }
            public ArmorCategory? Category { get; init; }
            public string? ArmorTypeKey { get; init; }
            public ArmorType? ArmorType { get; init; }
        }

        private sealed class EnumOption<TEnum> where TEnum : struct, Enum
        {
            public required TEnum Value { get; init; }
            public required string Label { get; init; }
        }

        private sealed class EnumOptionItem
        {
            public required object Value { get; init; }
            public required string Label { get; init; }
        }

        private static readonly HashSet<string> ExpandableMultilineGlobalFields = new(StringComparer.Ordinal)
        {
            nameof(Settings.PluginIncludeList),
            nameof(Settings.PluginExcludeList),
            nameof(Settings.IgnoredArmorFormKeys),
            nameof(Settings.ArmorPieceKeywords)
        };

        public SettingsWindow(Settings settings, string? settingsFolder)
        {
            LoadComponent();
            _settings = settings;
            _settingsFolder = settingsFolder;

            _settingsTree = GetRequiredControl<TreeView>("SettingsTree");
            _detailPanel = GetRequiredControl<StackPanel>("DetailPanel");
            _statusText = GetRequiredControl<TextBlock>("StatusText");
            _closeButton = GetRequiredControl<Button>("CloseButton");
            _saveButton = GetRequiredControl<Button>("SaveButton");

            _settingsTree.SelectedItemChanged += (_, _) => RenderSelectedNode();
            _saveButton.Click += (_, _) => Save();
            _closeButton.Click += (_, _) => Close();
            Closing += OnWindowClosing;

            _lastSavedSettingsSnapshot = GetSettingsSnapshot();

            BuildTree();
            SetStatus("Loaded settings.");
        }

        private void BuildTree()
        {
            _settingsTree.Items.Clear();

            var globalNode = new TreeViewItem
            {
                Header = "Global Settings",
                Tag = new NodeRef { Kind = NodeKind.Global }
            };
            _settingsTree.Items.Add(globalNode);

            foreach (var property in GetCategoryProperties())
            {
                if (property.GetValue(_settings) is not ArmorCategory category)
                {
                    continue;
                }

                var categoryNode = new TreeViewItem
                {
                    Header = GetPropertyLabel(property),
                    Tag = new NodeRef
                    {
                        Kind = NodeKind.Category,
                        CategoryProperty = property,
                        Category = category
                    }
                };

                foreach (var armorEntry in category.Armor.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
                {
                    var armorTypeNode = new TreeViewItem
                    {
                        Header = armorEntry.Key,
                        Tag = new NodeRef
                        {
                            Kind = NodeKind.ArmorType,
                            CategoryProperty = property,
                            Category = category,
                            ArmorTypeKey = armorEntry.Key,
                            ArmorType = armorEntry.Value
                        }
                    };

                    categoryNode.Items.Add(armorTypeNode);
                }

                _settingsTree.Items.Add(categoryNode);
            }

            if (_settingsTree.Items.Count > 0)
            {
                if (_settingsTree.Items[0] is TreeViewItem first)
                {
                    first.IsSelected = true;
                }
            }
        }

        private IEnumerable<PropertyInfo> GetCategoryProperties()
        {
            return typeof(Settings)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(ArmorCategory))
                .OrderBy(p => p.MetadataToken);
        }

        private void RenderSelectedNode()
        {
            _detailPanel.Children.Clear();

            if (_settingsTree.SelectedItem is not TreeViewItem item || item.Tag is not NodeRef node)
            {
                AddText("Select a node from the tree.", FontWeights.Normal, 14, Brushes.DimGray);
                return;
            }

            switch (node.Kind)
            {
                case NodeKind.Global:
                    RenderGlobalSettings();
                    break;
                case NodeKind.Category:
                    RenderCategory(node);
                    break;
                case NodeKind.ArmorType:
                    RenderArmorType(node);
                    break;
            }
        }

        private void RenderGlobalSettings()
        {
            AddText("Global Settings", FontWeights.SemiBold, 20, Brushes.Black);
            //AddText("These values map directly to top-level keys in settings.json.", FontWeights.Normal, 12, Brushes.DimGray);

            var presetButtonRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 2)
            };

            var restoreDefaultButton = new Button
            {
                Content = "Restore Default Settings",
                Width = 190,
                ToolTip = "Restore default settings preset."
            };
            restoreDefaultButton.Click += (_, _) =>
            {
                if (!ConfirmRestorePreset("default settings"))
                {
                    return;
                }

                RestoreFromBundledPreset(
                    "settings.json",
                    "Restored default settings preset. Click Save to persist.");
            };

            var restoreDebuffedButton = new Button
            {
                Content = "Restore Debuffed Settings",
                Width = 200,
                Margin = new Thickness(8, 0, 0, 0),
                ToolTip = "Restore default debuffed settings preset. This preset debuffs Anniversary Edition CC content."
            };
            restoreDebuffedButton.Click += (_, _) =>
            {
                if (!ConfirmRestorePreset("debuffed settings"))
                {
                    return;
                }

                RestoreFromBundledPreset(
                    "settings_debuffed.json",
                    "Restored debuffed preset (debuffs Anniversary Edition CC content). Click Save to persist.");
            };

            presetButtonRow.Children.Add(restoreDefaultButton);
            presetButtonRow.Children.Add(restoreDebuffedButton);
            _detailPanel.Children.Add(presetButtonRow);

            var props = typeof(Settings)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType != typeof(ArmorCategory))
                .OrderBy(p => p.MetadataToken)
                .ToList();

            foreach (var prop in props)
            {
                var type = prop.PropertyType;
                var value = prop.GetValue(_settings);
                var label = GetPropertyLabel(prop);
                var tooltip = GetPropertyTooltip(prop) ?? $"Configure {label}.";

                if (type == typeof(bool))
                {
                    var check = new CheckBox
                    {
                        IsChecked = (bool?)value ?? false
                    };
                    check.Checked += (_, _) => prop.SetValue(_settings, true);
                    check.Unchecked += (_, _) => prop.SetValue(_settings, false);
                    AddLabeledControlRow(_detailPanel, label, check, tooltip, topMargin: 8, labelWidth: 280);
                }
                else if (type.IsEnum)
                {
                    var combo = new ComboBox
                    {
                        ItemsSource = GetEnumOptions(type),
                        DisplayMemberPath = nameof(EnumOptionItem.Label),
                        SelectedValuePath = nameof(EnumOptionItem.Value),
                        SelectedValue = value,
                        MinWidth = 220
                    };
                    combo.SelectionChanged += (_, _) =>
                    {
                        if (combo.SelectedValue != null)
                        {
                            prop.SetValue(_settings, combo.SelectedValue);
                        }
                    };
                    AddLabeledControlRow(_detailPanel, label, combo, tooltip, topMargin: 8, labelWidth: 280);
                }
                else
                {
                    if (type == typeof(string) && ExpandableMultilineGlobalFields.Contains(prop.Name))
                    {
                        var expander = CreateExpandableTextEditor(value?.ToString() ?? string.Empty, text => prop.SetValue(_settings, text));
                        AddLabeledControlRow(_detailPanel, label, expander, tooltip, topMargin: 8, labelWidth: 280);
                    }
                    else
                    {
                        var box = new TextBox
                        {
                            Text = value?.ToString() ?? string.Empty,
                            MinWidth = 360
                        };
                        box.TextChanged += (_, _) => prop.SetValue(_settings, box.Text);
                        AddLabeledControlRow(_detailPanel, label, box, tooltip, topMargin: 8, labelWidth: 280);
                    }
                }
            }
        }

        private static Expander CreateExpandableTextEditor(string value, Action<string> onChanged)
        {
            var textBox = new TextBox
            {
                Text = value,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                MinWidth = 360,
                MinHeight = 72
            };
            textBox.TextChanged += (_, _) => onChanged(textBox.Text);

            var expander = new Expander
            {
                Header = "Expand",
                IsExpanded = false,
                Content = textBox
            };
            expander.Expanded += (_, _) => expander.Header = "Collapse";
            expander.Collapsed += (_, _) => expander.Header = "Expand";

            return expander;
        }

        private void RenderCategory(NodeRef node)
        {
            if (node.Category == null || node.CategoryProperty == null)
            {
                return;
            }

            AddText(GetPropertyLabel(node.CategoryProperty), FontWeights.SemiBold, 20, Brushes.Black);
            AddText($"Armor sets: {node.Category.Armor.Count}", FontWeights.Normal, 12, Brushes.DimGray);

            var addButton = new Button
            {
                Content = "Add Armor Set",
                Width = 160,
                Margin = new Thickness(0, 12, 0, 0)
            };
            addButton.Click += (_, _) =>
            {
                var key = GenerateUniqueKey(node.Category.Armor.Keys, "New Armor Set");
                node.Category.Armor[key] = new ArmorType
                {
                    MatchBehavior = ArmorMatchBehavior.ModifyArmor,
                    Identifiers = string.Empty,
                    Type = GuessArmorType(node.CategoryProperty.Name),
                    Armor = new Dictionary<string, ArmorPiece>()
                };
                BuildTree();
                SelectArmorTypeNode(node.CategoryProperty, key);
                SetStatus($"Added armor set '{key}'.");
            };
            _detailPanel.Children.Add(addButton);
        }

        private void RenderArmorType(NodeRef node)
        {
            if (node.Category == null || node.ArmorType == null || node.ArmorTypeKey == null || node.CategoryProperty == null)
            {
                return;
            }

            AddText("Armor Set", FontWeights.SemiBold, 20, Brushes.Black);
            AddText(GetPropertyLabel(node.CategoryProperty), FontWeights.Normal, 12, Brushes.DimGray);

            var keyBox = new TextBox
            {
                Text = node.ArmorTypeKey,
                MinWidth = 360
            };
            TrackPendingText(keyBox, node.ArmorTypeKey);
            AddLabeledControlRow(_detailPanel, "Name", keyBox, "Name of this armor set for UI organization only; it is not used in the patching process.");
            keyBox.LostFocus += (_, _) =>
            {
                RenameArmorType(node, keyBox.Text.Trim());
            };

            var matchBehaviorBox = new ComboBox
            {
                ItemsSource = GetEnumOptions<ArmorMatchBehavior>(),
                DisplayMemberPath = nameof(EnumOption<ArmorMatchBehavior>.Label),
                SelectedValuePath = nameof(EnumOption<ArmorMatchBehavior>.Value),
                SelectedValue = node.ArmorType.MatchBehavior,
                MinWidth = 220
            };
            AddLabeledControlRow(_detailPanel, "Match Behavior", matchBehaviorBox, "Controls what happens when this rule matches an armor item.\nModify Armor: Modifies the armor's stats.\nPass: Matches the armor item but does not modify it.\nDeactivate Rule: Allows other rules to process this armor item.");
            matchBehaviorBox.SelectionChanged += (_, _) =>
            {
                if (matchBehaviorBox.SelectedValue is ArmorMatchBehavior value)
                {
                    node.ArmorType.MatchBehavior = value;
                }
            };

            var identifiersBox = new TextBox
            {
                Text = node.ArmorType.Identifiers,
                MinWidth = 360
            };
            AddLabeledControlRow(_detailPanel, "Identifiers", identifiersBox, "Semicolon-separated identifiers used to match armor records. Matches against the armor's FormKey, Name, and keywords in this order.");
            identifiersBox.TextChanged += (_, _) => node.ArmorType.Identifiers = identifiersBox.Text;

            var typeBox = new ComboBox
            {
                ItemsSource = GetEnumOptions<ArmorTypeEnum>(),
                DisplayMemberPath = nameof(EnumOption<ArmorTypeEnum>.Label),
                SelectedValuePath = nameof(EnumOption<ArmorTypeEnum>.Value),
                SelectedValue = node.ArmorType.Type,
                MinWidth = 220
            };
            AddLabeledControlRow(_detailPanel, "Type", typeBox, "The target armor class for this set.");
            typeBox.SelectionChanged += (_, _) =>
            {
                if (typeBox.SelectedValue is ArmorTypeEnum value)
                {
                    node.ArmorType.Type = value;
                }
            };

            var buttonRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 12, 0, 0)
            };

            var addPieceButton = new Button
            {
                Content = "Add Armor Piece",
                Width = 150,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            addPieceButton.Click += (_, _) =>
            {
                var key = GenerateUniqueKey(node.ArmorType.Armor.Keys, "New Piece");
                node.ArmorType.Armor[key] = new ArmorPiece
                {
                    Identifiers = string.Empty,
                    Rating = 0,
                    Weight = 0
                };
                BuildTree();
                SelectArmorTypeNode(node.CategoryProperty, node.ArmorTypeKey);
                SetStatus($"Added armor piece '{key}'.");
            };

            var deleteButton = new Button
            {
                Content = "Delete Armor Set",
                Width = 150,
                Margin = new Thickness(0, 0, 0, 0)
            };
            deleteButton.Click += (_, _) =>
            {
                node.Category.Armor.Remove(node.ArmorTypeKey);
                BuildTree();
                SetStatus($"Deleted armor set '{node.ArmorTypeKey}'.");
            };

            buttonRow.Children.Add(deleteButton);
            _detailPanel.Children.Add(buttonRow);

            AddText("Armor Pieces", FontWeights.SemiBold, 16, Brushes.Black, topMargin: 14);

            addPieceButton.Margin = new Thickness(0, 8, 0, 0);
            _detailPanel.Children.Add(addPieceButton);

            foreach (var pieceEntry in node.ArmorType.Armor.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                var pieceKey = pieceEntry.Key;
                var piece = pieceEntry.Value;

                var pieceContainer = new Border
                {
                    BorderBrush = Brushes.Gainsboro,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 8, 0, 0)
                };

                var pieceRow = new Grid();
                pieceRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                pieceRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var piecePanel = new StackPanel();
                Grid.SetColumn(piecePanel, 0);
                pieceRow.Children.Add(piecePanel);

                var deletePieceButton = new Button
                {
                    Content = "Delete",
                    Width = 90,
                    Margin = new Thickness(12, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                deletePieceButton.Click += (_, _) =>
                {
                    node.ArmorType.Armor.Remove(pieceKey);
                    BuildTree();
                    SelectArmorTypeNode(node.CategoryProperty, node.ArmorTypeKey);
                    SetStatus($"Deleted armor piece '{pieceKey}'.");
                };
                Grid.SetColumn(deletePieceButton, 1);
                pieceRow.Children.Add(deletePieceButton);

                pieceContainer.Child = pieceRow;
                _detailPanel.Children.Add(pieceContainer);

                var pieceNameBox = new TextBox
                {
                    Text = pieceKey,
                    MinWidth = 360
                };
                TrackPendingText(pieceNameBox, pieceKey);
                AddLabeledControlRow(piecePanel, "Name", pieceNameBox, "Name of this armor piece entry for UI organization only; it is not used in the patching process.", topMargin: 0);
                pieceNameBox.LostFocus += (_, _) =>
                {
                    RenameArmorPiece(node, pieceKey, pieceNameBox.Text.Trim());
                };

                var pieceIdentifiersBox = new TextBox
                {
                    Text = piece.Identifiers,
                    MinWidth = 360
                };
                AddLabeledControlRow(piecePanel, "Identifiers", pieceIdentifiersBox, "Semicolon-separated identifiers used to match this piece. Matches against the armor's FormKey, Name, and keywords in this order.");
                pieceIdentifiersBox.TextChanged += (_, _) => piece.Identifiers = pieceIdentifiersBox.Text;

                var ratingBox = new TextBox
                {
                    Text = piece.Rating.ToString(CultureInfo.InvariantCulture),
                    MinWidth = 180
                };
                TrackPendingText(ratingBox, piece.Rating.ToString(CultureInfo.InvariantCulture));
                AddLabeledControlRow(piecePanel, "Rating", ratingBox, "Armor rating to apply when this piece matches.");
                ratingBox.LostFocus += (_, _) =>
                {
                    if (TryParseFloat(ratingBox.Text, out var rating))
                    {
                        piece.Rating = rating;
                        SetStatus("Rating updated.");
                    }
                    else
                    {
                        SetStatus("Rating must be a number.", isError: true);
                    }
                };

                var weightBox = new TextBox
                {
                    Text = piece.Weight.ToString(CultureInfo.InvariantCulture),
                    MinWidth = 180
                };
                TrackPendingText(weightBox, piece.Weight.ToString(CultureInfo.InvariantCulture));
                AddLabeledControlRow(piecePanel, "Weight", weightBox, "Weight to apply when this piece matches.");
                weightBox.LostFocus += (_, _) =>
                {
                    if (TryParseFloat(weightBox.Text, out var weight))
                    {
                        piece.Weight = weight;
                        SetStatus("Weight updated.");
                    }
                    else
                    {
                        SetStatus("Weight must be a number.", isError: true);
                    }
                };

            }

            _ = matchBehaviorBox;
            _ = typeBox;
        }

        private void RenameArmorType(NodeRef node, string newKey)
        {
            if (node.Category == null || node.ArmorType == null || node.ArmorTypeKey == null || node.CategoryProperty == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(newKey) || string.Equals(newKey, node.ArmorTypeKey, StringComparison.Ordinal))
            {
                return;
            }

            if (node.Category.Armor.ContainsKey(newKey))
            {
                SetStatus($"Armor set name '{newKey}' already exists.", isError: true);
                return;
            }

            node.Category.Armor.Remove(node.ArmorTypeKey);
            node.Category.Armor[newKey] = node.ArmorType;
            BuildTree();
            SelectArmorTypeNode(node.CategoryProperty, newKey);
            SetStatus($"Renamed armor set to '{newKey}'.");
        }

        private void RenameArmorPiece(NodeRef node, string oldKey, string newKey)
        {
            if (node.ArmorType == null || node.CategoryProperty == null || node.ArmorTypeKey == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(newKey) || string.Equals(newKey, oldKey, StringComparison.Ordinal))
            {
                return;
            }

            if (!node.ArmorType.Armor.TryGetValue(oldKey, out var piece))
            {
                return;
            }

            if (node.ArmorType.Armor.ContainsKey(newKey))
            {
                SetStatus($"Armor piece name '{newKey}' already exists.", isError: true);
                return;
            }

            node.ArmorType.Armor.Remove(oldKey);
            node.ArmorType.Armor[newKey] = piece;
            BuildTree();
            SelectArmorTypeNode(node.CategoryProperty, node.ArmorTypeKey);
            SetStatus($"Renamed armor piece to '{newKey}'.");
        }

        private ComboBox AddEnumComboBox<TEnum>(string label, TEnum current, Action<TEnum> onChanged) where TEnum : struct, Enum
        {
            AddText(label, FontWeights.SemiBold, 13, Brushes.Black, topMargin: 10);
            var combo = new ComboBox
            {
                Margin = new Thickness(0, 4, 0, 0),
                ItemsSource = GetEnumOptions<TEnum>(),
                DisplayMemberPath = nameof(EnumOption<TEnum>.Label),
                SelectedValuePath = nameof(EnumOption<TEnum>.Value),
                SelectedValue = current,
                MinWidth = 220
            };
            combo.SelectionChanged += (_, _) =>
            {
                if (combo.SelectedValue is TEnum value)
                {
                    onChanged(value);
                }
            };
            _detailPanel.Children.Add(combo);
            return combo;
        }

        private static List<EnumOption<TEnum>> GetEnumOptions<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(value => new EnumOption<TEnum>
                {
                    Value = value,
                    Label = GetEnumLabel(value)
                })
                .ToList();
        }

        private static List<EnumOptionItem> GetEnumOptions(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<object>()
                .Select(value => new EnumOptionItem
                {
                    Value = value,
                    Label = GetEnumLabel(enumType, value.ToString() ?? string.Empty)
                })
                .ToList();
        }

        private static string GetEnumLabel<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var name = value.ToString() ?? string.Empty;
            return GetEnumLabel(typeof(TEnum), name);
        }

        private static string GetEnumLabel(Type enumType, string enumMemberName)
        {
            var member = enumType.GetMember(enumMemberName).FirstOrDefault();
            var settingName = GetSettingName(member);
            if (!string.IsNullOrWhiteSpace(settingName))
            {
                return settingName;
            }

            return SplitPascalCase(enumMemberName);
        }

        private static string GetPropertyLabel(PropertyInfo property)
        {
            var settingName = GetSettingName(property);
            if (!string.IsNullOrWhiteSpace(settingName))
            {
                return settingName;
            }

            return SplitPascalCase(property.Name);
        }

        private static string? GetPropertyTooltip(PropertyInfo property)
        {
            return GetTooltipText(property);
        }

        private static string? GetSettingName(MemberInfo? member)
        {
            var settingNameAttribute = member?
                .GetCustomAttributes(inherit: false)
                .FirstOrDefault(attribute =>
                    string.Equals(attribute.GetType().Name, "SettingName", StringComparison.Ordinal) ||
                    string.Equals(attribute.GetType().Name, "SettingNameAttribute", StringComparison.Ordinal));

            if (settingNameAttribute == null)
            {
                return null;
            }

            var nameProperty = settingNameAttribute.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            return nameProperty?.GetValue(settingNameAttribute) as string;
        }

        private static string? GetTooltipText(MemberInfo? member)
        {
            var tooltipAttributeData = member?
                .CustomAttributes
                .FirstOrDefault(attribute =>
                    string.Equals(attribute.AttributeType.Name, "Tooltip", StringComparison.Ordinal)
                    || string.Equals(attribute.AttributeType.Name, "TooltipAttribute", StringComparison.Ordinal));

            if (tooltipAttributeData == null)
            {
                return null;
            }

            if (tooltipAttributeData.ConstructorArguments.Count > 0
                && tooltipAttributeData.ConstructorArguments[0].Value is string constructorValue
                && !string.IsNullOrWhiteSpace(constructorValue))
            {
                return constructorValue;
            }

            var namedValue = tooltipAttributeData.NamedArguments
                .Select(arg => arg.TypedValue.Value as string)
                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

            return namedValue;
        }

        private static string SplitPascalCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var chars = new List<char>(value.Length + 5);
            chars.Add(value[0]);

            for (var i = 1; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]) && !char.IsUpper(value[i - 1]))
                {
                    chars.Add(' ');
                }

                chars.Add(value[i]);
            }

            return new string(chars.ToArray());
        }

        private TextBox AddLabeledTextBox(string label, string value)
        {
            AddText(label, FontWeights.SemiBold, 13, Brushes.Black, topMargin: 10);
            var box = new TextBox
            {
                Text = value,
                Margin = new Thickness(0, 4, 0, 0),
                MinWidth = 360
            };
            _detailPanel.Children.Add(box);
            return box;
        }

        private static void AddLabeledControlRow(Panel panel, string label, FrameworkElement control, string? tooltip = null, double topMargin = 10, double labelWidth = 130)
        {
            var row = new Grid
            {
                Margin = new Thickness(0, topMargin, 0, 0)
            };

            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(labelWidth) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var labelBlock = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 10, 0)
            };

            if (!string.IsNullOrWhiteSpace(tooltip))
            {
                labelBlock.ToolTip = tooltip;
                control.ToolTip = tooltip;
            }

            control.VerticalAlignment = VerticalAlignment.Top;
            control.HorizontalAlignment = HorizontalAlignment.Stretch;

            Grid.SetColumn(labelBlock, 0);
            Grid.SetColumn(control, 1);
            row.Children.Add(labelBlock);
            row.Children.Add(control);

            panel.Children.Add(row);
        }

        private void AddText(string text, FontWeight weight, double size, Brush color, double topMargin = 0)
        {
            _detailPanel.Children.Add(new TextBlock
            {
                Text = text,
                FontWeight = weight,
                FontSize = size,
                Foreground = color,
                Margin = new Thickness(0, topMargin, 0, 0),
                TextWrapping = TextWrapping.Wrap
            });
        }

        private static void AddTextToPanel(Panel panel, string text, FontWeight weight, double size, Brush color, double topMargin = 0)
        {
            panel.Children.Add(new TextBlock
            {
                Text = text,
                FontWeight = weight,
                FontSize = size,
                Foreground = color,
                Margin = new Thickness(0, topMargin, 0, 0),
                TextWrapping = TextWrapping.Wrap
            });
        }

        private static string GenerateUniqueKey(IEnumerable<string> existingKeys, string baseKey)
        {
            var existing = new HashSet<string>(existingKeys, StringComparer.OrdinalIgnoreCase);
            if (!existing.Contains(baseKey))
            {
                return baseKey;
            }

            var index = 2;
            while (existing.Contains($"{baseKey} {index}"))
            {
                index++;
            }

            return $"{baseKey} {index}";
        }

        private static ArmorTypeEnum GuessArmorType(string categoryName)
        {
            if (categoryName.Contains("Clothing", StringComparison.OrdinalIgnoreCase))
            {
                return ArmorTypeEnum.Clothing;
            }

            if (categoryName.Contains("Heavy", StringComparison.OrdinalIgnoreCase))
            {
                return ArmorTypeEnum.HeavyArmor;
            }

            return ArmorTypeEnum.LightArmor;
        }

        private static bool TryParseFloat(string text, out float value)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }

        private void SetStatus(string message, bool isError = false)
        {
            _statusText.Text = message;
            _statusText.Foreground = isError ? Brushes.DarkRed : Brushes.DimGray;
        }

        private bool Save()
        {
            try
            {
                SettingsFileStore.Save(_settingsFolder, _settings);
                _lastSavedSettingsSnapshot = GetSettingsSnapshot();
                SetStatus("Saved settings.json.");
                return true;
            }
            catch (Exception ex)
            {
                SetStatus($"Failed to save settings: {ex.Message}", isError: true);
                return false;
            }
        }

        private string GetSettingsSnapshot()
        {
            return JsonConvert.SerializeObject(_settings, Formatting.None);
        }

        private bool HasUnsavedChanges()
        {
            return HasPendingTextBoxChanges()
                || !string.Equals(_lastSavedSettingsSnapshot, GetSettingsSnapshot(), StringComparison.Ordinal);
        }

        private static void TrackPendingText(TextBox textBox, string originalText)
        {
            textBox.Tag = new PendingTextSnapshot
            {
                OriginalText = originalText
            };
        }

        private bool HasPendingTextBoxChanges()
        {
            foreach (var textBox in GetVisualDescendants<TextBox>(_detailPanel))
            {
                if (textBox.Tag is not PendingTextSnapshot snapshot)
                {
                    continue;
                }

                if (!string.Equals(textBox.Text, snapshot.OriginalText, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<T> GetVisualDescendants<T>(DependencyObject root) where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is T match)
                {
                    yield return match;
                }

                foreach (var descendant in GetVisualDescendants<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private void CommitPendingInputEdits()
        {
            // Some fields (name/rating/weight) commit on LostFocus. When closing via window X,
            // force a focus transition first so those handlers run consistently.
            if (Keyboard.FocusedElement is not UIElement focusedElement)
            {
                return;
            }

            _saveButton.Focus();
            focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            CommitPendingInputEdits();

            if (!HasUnsavedChanges())
            {
                return;
            }

            var result = MessageBox.Show(
                "You have unsaved changes.\n\nDo you want to save before closing?",
                "Unsaved Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes);

            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (result == MessageBoxResult.Yes)
            {
                if (!Save())
                {
                    e.Cancel = true;
                }
            }
        }

        private void RestoreFromBundledPreset(string presetFileName, string successMessage)
        {
            try
            {
                var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (string.IsNullOrWhiteSpace(assemblyDirectory))
                {
                    SetStatus("Could not locate patcher directory for bundled presets.", isError: true);
                    return;
                }

                var presetPath = Path.Combine(assemblyDirectory, "Data", presetFileName);
                if (!File.Exists(presetPath))
                {
                    SetStatus($"Bundled preset was not found: {presetFileName}", isError: true);
                    return;
                }

                var json = File.ReadAllText(presetPath);
                JsonConvert.PopulateObject(
                    json,
                    _settings,
                    new JsonSerializerSettings
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });

                BuildTree();
                if (_settingsTree.Items.Count > 0 && _settingsTree.Items[0] is TreeViewItem firstNode)
                {
                    firstNode.IsSelected = true;
                    firstNode.BringIntoView();
                }

                RenderSelectedNode();
                SetStatus(successMessage);
            }
            catch (Exception ex)
            {
                SetStatus($"Failed to restore preset '{presetFileName}': {ex.Message}", isError: true);
            }
        }

        private bool ConfirmRestorePreset(string presetLabel)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to restore {presetLabel}?\n\nThis will replace your current settings, including added, removed, or edited armor sets and armor pieces.\n\nClick Save afterward if you want to persist these restored settings to your settings file.",
                "Confirm Restore",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            return result == MessageBoxResult.Yes;
        }

        private void SelectArmorTypeNode(PropertyInfo categoryProperty, string armorTypeKey)
        {
            foreach (var node in EnumerateTreeNodes(_settingsTree.Items))
            {
                if (node.Tag is not NodeRef tag)
                {
                    continue;
                }

                if (tag.Kind == NodeKind.ArmorType
                    && tag.CategoryProperty?.Name == categoryProperty.Name
                    && string.Equals(tag.ArmorTypeKey, armorTypeKey, StringComparison.Ordinal))
                {
                    node.IsSelected = true;
                    node.BringIntoView();
                    return;
                }
            }
        }

        private static IEnumerable<TreeViewItem> EnumerateTreeNodes(ItemCollection items)
        {
            foreach (var item in items)
            {
                if (item is not TreeViewItem treeItem)
                {
                    continue;
                }

                yield return treeItem;

                foreach (var child in EnumerateTreeNodes(treeItem.Items))
                {
                    yield return child;
                }
            }
        }

        private void LoadComponent()
        {
            var resourceLocater = new Uri("/ArmorStatSynthesisPatcher;component/settingswindow.xaml", UriKind.Relative);
            Application.LoadComponent(this, resourceLocater);
        }

        private TControl GetRequiredControl<TControl>(string name) where TControl : class
        {
            return FindName(name) as TControl
                ?? throw new XamlParseException($"Required control '{name}' was not found in SettingsWindow.xaml.");
        }
    }
}
