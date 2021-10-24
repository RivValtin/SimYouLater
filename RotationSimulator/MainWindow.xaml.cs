using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Windows.Documents;

namespace RotationSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RotationCollection rotations = new RotationCollection();
        public Rotation activeRotation = new Rotation();
        public string activeRotationName = string.Empty;

        public List<GearSet> gearSets = new List<GearSet>();
        public GearSet activeGearset = null;
        public ESimulationMode simMode = ESimulationMode.Simple;

        public MainWindow() {
            InitializeComponent();
            //TODO: Load rotations from disk here.
            string rotationsFilePath = GetRotationsSaveFilePath();
            if (File.Exists(rotationsFilePath)) {
                XmlSerializer serializer = new XmlSerializer(typeof(RotationCollection));
                TextReader reader = new StreamReader(rotationsFilePath);
                object serializerOutput = serializer.Deserialize(reader);

                rotations = serializerOutput as RotationCollection;
            }
            string gearSetsPath = GetGearSetsSaveFilePath();
            if (File.Exists(gearSetsPath)) {
                XmlSerializer serializer = new XmlSerializer(typeof(List<GearSet>));
                TextReader reader = new StreamReader(gearSetsPath);
                object serializerOutput = serializer.Deserialize(reader);

                gearSets = (serializerOutput as IEnumerable<GearSet>).ToList();

                if (gearSets.Count > 0) {
                    GearSet defaultSet = gearSets.First();
                    ChangeActiveGearset(defaultSet.Name);
                }
            }
            if (rotations.Keys.Count > 0) {
                activeRotation = rotations.First().Value;
                activeRotationName = rotations.First().Key;
            }

            UpdateRotationListDisplay();
            UpdateRotationDisplay();
            UpdateActionSet();
            UpdateUIFromActiveGearset();
            UpdateGearsetList();
        }
        private void OnExit(object sender, EventArgs e) {
            {
                string saveFilePath = GetRotationsSaveFilePath();
                XmlSerializer serializer = new XmlSerializer(typeof(RotationCollection));
                TextWriter writer = new StreamWriter(saveFilePath);
                serializer.Serialize(writer, rotations);
            }
            {
                string saveFilePath = GetGearSetsSaveFilePath();
                XmlSerializer serializer = new XmlSerializer(typeof(List<GearSet>));
                TextWriter writer = new StreamWriter(saveFilePath);
                serializer.Serialize(writer, gearSets);
            }
        }

        private string GetRotationsSaveFilePath() {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fileDirectory = Path.Combine(appDataPath, "SimYouLater");
            string saveFilePath = Path.Combine(fileDirectory, "rotations.xml");
            if (!Directory.Exists(fileDirectory)) {
                Directory.CreateDirectory(fileDirectory);
            }
            return saveFilePath;
        }
        private string GetGearSetsSaveFilePath() {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fileDirectory = Path.Combine(appDataPath, "SimYouLater");
            string saveFilePath = Path.Combine(fileDirectory, "gearsets.xml");
            if (!Directory.Exists(fileDirectory)) {
                Directory.CreateDirectory(fileDirectory);
            }
            return saveFilePath;
        }

        private void ChangeActiveRotation(string rotationName) {
            activeRotationName = rotationName;
            activeRotation = rotations[activeRotationName];
            UpdateRotationDisplay();
            UpdateRotationListDisplay();
            UpdateActionSet();
        }

        private void ChangeActiveGearset(string gearsetName) {
            if (activeGearset?.Name == gearsetName) {
                return;
            }
            activeGearset = gearSets.Find(x => x.Name == gearsetName);
            UpdateUIFromActiveGearset();
            lb_gearsetList.SelectedItem = gearsetName;
            cb_simulationGearsetSelection.SelectedItem = gearsetName;
        }


        #region Simulator Tab Specific
        private void button_Simulate(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(activeRotationName) || activeGearset == null) {
                return;
            }

            SimLog.Clear();

            Simulator simulator = new Simulator();
            simulator.AnimationLock = int.Parse(tb_opt_ping.Text) / 10 + 60;
            if (simulator.AnimationLock < 61) {
                simulator.AnimationLock = 61;
            }

            int partyBonus = (bool)cb_opt_partyBonus.IsChecked ? 105 : 100;
            CharacterStats charStats = new CharacterStats((EJobId)Enum.Parse(typeof(EJobId), activeRotation.JobCode, true))
            {
                PhysicalDamage = activeGearset.PhysicalDamage,
                MagicalDamage = activeGearset.MagicalDamage,
                Strength = activeGearset.Strength * partyBonus / 100,
                Dexterity = activeGearset.Dexterity * partyBonus / 100,
                Intelligence = activeGearset.Intelligence * partyBonus / 100,
                Mind = activeGearset.Mind * partyBonus / 100,
                CriticalHitSubstat = activeGearset.CriticalHit,
                DirectHitSubstat = activeGearset.DirectHit,
                DeterminationSubstat = activeGearset.Determination,
                Tenacity = activeGearset.Tenacity,
                SkillSpeed = activeGearset.SkillSpeed,
                SpellSpeed = activeGearset.SpellSpeed
            };
            simulator.CharStats = charStats;

            SimulationResults results = simulator.Simulate(activeRotation.RotationSteps, activeRotation.StartTimeOffset, GetExteralEffectsFromOptions(), simMode);

            if (ESimulationMode.Variation == simMode) {
                textBlock.Text = "Worst run: " + results.minDamage + " dps\n" +
                                 "Average: " + results.averageDamage + " dps\n" +
                                 "Best run: " + results.maxDamage + " dps\n";
            } else {
                textBlock.Text = "PPS: " + results.pps + "\n" +
                                 "ePPS: " + results.epps + "\n" +
                                 "Estimated DPS: " + results.dps + "\n" +
                                 "Time: " + (float)results.totalTime / 100 + "s";
            }
            UpdateLogText();
        }

        private List<ActiveEffect> GetExteralEffectsFromOptions() {
            List<ActiveEffect> externalEffects = new List<ActiveEffect>();
            if ((bool)cb_opt_simulateDivination.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 910,
                        ActiveEndTime = 12000 * i + 1500 + 910,
                        effect = EffectsBank.effects["AST_Divination"],
                    });
                }
            }
            if ((bool)cb_opt_simulateBardBuffs.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 420,
                        ActiveEndTime = 12000 * i + 2000 + 420,
                        effect = EffectsBank.effects["BRD_BattleVoice"],
                    });
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 350,
                        ActiveEndTime = 12000 * i + 4500 + 350,
                        effect = EffectsBank.effects["BRD_WanderersMinuet_Party"],
                    });
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 4850,
                        ActiveEndTime = 12000 * i + 4500 + 4850,
                        effect = EffectsBank.effects["BRD_MagesBallad_Party"],
                    });
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 9350,
                        ActiveEndTime = 12000 * i + 3000 + 9350,
                        effect = EffectsBank.effects["BRD_ArmysPaeon_Party"],
                    });
                }
            }
            if ((bool)cb_opt_simulateTechnicalStep.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 830,
                        ActiveEndTime = 12000 * i + 2000 + 830,
                        effect = EffectsBank.effects["DNC_TechnicalFinish"],
                    });
                }
            }
            if ((bool)cb_opt_simulateDancePartner.IsChecked) {
                externalEffects.Add(new ActiveEffect()
                {
                    ActiveStartTime = 130,
                    ActiveEndTime = int.MaxValue,
                    effect = EffectsBank.effects["DNC_StandardFinish"],
                });
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 1050,
                        ActiveEndTime = 12000 * i + 2000 + 1050,
                        effect = EffectsBank.effects["DNC_Devilment"],
                    });
                }
            }
            if ((bool)cb_opt_simulateBattleLitany.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 520,
                        ActiveEndTime = 12000 * i + 1500 + 520,
                        effect = EffectsBank.effects["DRG_BattleLitany"],
                    });
                }
            }
            if ((bool)cb_opt_simulateDragoonTether.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 450,
                        ActiveEndTime = 12000 * i + 2000 + 450,
                        effect = EffectsBank.effects["DRG_LeftEye"],
                    });
                }
            }
            if ((bool)cb_opt_simulateBrotherhood.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 670,
                        ActiveEndTime = 12000 * i + 1500 + 670,
                        effect = EffectsBank.effects["MNK_Brotherhood"],
                    });
                }
            }
            if ((bool)cb_opt_simulateTrickAttack.IsChecked) {
                for (int i = 0; i < 20; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 6000 * i + 850,
                        ActiveEndTime = 6000 * i + 1500 + 850,
                        effect = EffectsBank.effects["NIN_TrickAttack"],
                    });
                }
            }
            if ((bool)cb_opt_simulateEmbolden.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 650,
                        ActiveEndTime = 12000 * i + 400 + 650,
                        effect = EffectsBank.effects["RDM_Embolden_Party"],
                        Stacks = 5,
                    });
                }
            }
            if ((bool)cb_opt_simulateChainStrat.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 880,
                        ActiveEndTime = 12000 * i + 1500 + 880,
                        effect = EffectsBank.effects["SCH_ChainStratagem"],
                    });
                }
            }
            if ((bool)cb_opt_simulateSearingLight.IsChecked) {
                for (int i = 0; i < 10; i++) {
                    externalEffects.Add(new ActiveEffect()
                    {
                        ActiveStartTime = 12000 * i + 70,
                        ActiveEndTime = 12000 * i + 3000 + 70,
                        effect = EffectsBank.effects["SMN_SearingLight"],
                    });
                }
            }

            return externalEffects;
        }

        private void UpdateLogText() {
            tb_logOutput.Inlines.Clear();
            IEnumerable<SimLogEvent> logsToParse;
            switch (cmb_logLevel.SelectedIndex) {
                case 0:
                    logsToParse = SimLog.GetErrors();
                    break;
                case 1:
                    logsToParse = SimLog.GetErrorsWarnings();
                    break;
                case 2:
                    logsToParse = SimLog.GetInfoOrWorse();
                    break;
                case 3:
                default:
                    logsToParse = SimLog.GetAll();
                    break;
            }
            foreach (SimLogEvent logEvent in logsToParse) {
                int minutes = logEvent.TimeStamp / 6000;
                float seconds = Math.Abs((logEvent.TimeStamp % 6000) / 100.0f);
                string timeString = (logEvent.TimeStamp < 0 ? "-" : "") + minutes.ToString() + "m" + seconds.ToString("00.00") + "s";

                tb_logOutput.Inlines.Add(timeString + " - " + logEvent.Message + " ");
                if (logEvent.RelevantAction != null) {
                    BitmapImage actionIconSource = new BitmapImage(new Uri("/images/icons/" + logEvent.RelevantAction.IconName, UriKind.Relative));
                    Image actionIconImage = new Image()
                    {
                        Source = actionIconSource,
                        Width = 12,
                        Height = 12,
                        Stretch = Stretch.Fill,
                        Opacity = 1
                    };
                    InlineUIContainer actionIconInline = new InlineUIContainer(actionIconImage);
                    tb_logOutput.Inlines.Add(actionIconInline);
                    tb_logOutput.Inlines.Add(logEvent.RelevantAction.DisplayName);
                }
                tb_logOutput.Inlines.Add("\n");
            }
        }
        private void cb_simulationRotationSelection_Changed(object sender, SelectionChangedEventArgs e) {
            ComboBox comboBox = sender as ComboBox;
            string newRotation = comboBox.SelectedItem as string;
            if (newRotation != null && newRotation != activeRotationName) {
                ChangeActiveRotation(newRotation);
            }
        }
        private void cb_simulationGearsetSelection_Changed(object sender, SelectionChangedEventArgs e) {
            ComboBox comboBox = sender as ComboBox;
            string newGearsetName = comboBox.SelectedItem as string;
            if (newGearsetName != null) {
                ChangeActiveGearset(newGearsetName);
            }
        }
        private void cmb_logLevel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (tb_logOutput != null) {
                UpdateLogText();
            }
        }

        private void cmb_simulationMode_Changed(object sender, SelectionChangedEventArgs e) {
            if (cmb_simulationModeSelection != null) {
                simMode = (ESimulationMode)cmb_simulationModeSelection.SelectedIndex;
            }
        }
        #endregion

        #region Rotation Tab Specific
        private void RotationElement_ContextMenu_Delete(object sender, RoutedEventArgs e) {
            if (sender != null && MyXaml.GetRotationStepId(sender as DependencyObject) != null) {
                int itemIndex = activeRotation.RotationSteps.FindIndex(x => x.Id == MyXaml.GetRotationStepId(sender as DependencyObject));
                activeRotation.RotationSteps.RemoveAt(itemIndex);

                UpdateRotationDisplay();
                UpdateLayout();
            }
        }

        private void DragRotationElement(object sender, MouseEventArgs e) {
            if (sender != null && e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop((DependencyObject)sender, activeRotation.RotationSteps.First(x => x.Id == MyXaml.GetRotationStepId(sender as DependencyObject)), DragDropEffects.Move);
            }
        }

        private void DragActionFromBank(object sender, MouseEventArgs e) {
            if (sender != null && e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop((DependencyObject)sender, ActionBank.actions[MyXaml.GetActionIdProperty(sender as DependencyObject)], DragDropEffects.Move);
            }
        }

        private void rotationPanel_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(typeof(ActionDef))) {
                ActionDef action = (ActionDef)e.Data.GetData(typeof(ActionDef));

                RotationStep rotationStep = new RotationStep()
                {
                    Type = ERotationStepType.Action,
                    Parameters = new RotationStep.RotationStepParameters()
                    {
                        {"action", action.UniqueID }
                    }
                };
                activeRotation.RotationSteps.Add(rotationStep);
                UpdateRotationDisplay();
                UpdateLayout();

                e.Handled = true;
            }
        }

        private void rotationElement_drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(typeof(ActionDef))) {
                ActionDef action = (ActionDef)e.Data.GetData(typeof(ActionDef));
                int targetedRotationStepId = (int)MyXaml.GetRotationStepId(sender as DependencyObject);

                RotationStep rotationStep = new RotationStep()
                {
                    Type = ERotationStepType.Action,
                    Parameters = new RotationStep.RotationStepParameters()
                    {
                        {"action", action.UniqueID }
                    }
                };

                int index = activeRotation.RotationSteps.FindIndex(x => x.Id == targetedRotationStepId);
                activeRotation.RotationSteps.Insert(index, rotationStep);

                UpdateRotationDisplay();
                UpdateLayout();
                e.Handled = true;
            }
            if (e.Data.GetDataPresent(typeof(RotationStep))) {
                //This is a "move" operation.
                RotationStep movingRotationStep = (RotationStep)e.Data.GetData(typeof(RotationStep));
                int targetedRotationStepId = (int)MyXaml.GetRotationStepId(sender as DependencyObject);

                if (movingRotationStep.Id != targetedRotationStepId) {
                    int itemIndex = activeRotation.RotationSteps.FindIndex(x => x.Id == movingRotationStep.Id);
                    activeRotation.RotationSteps.RemoveAt(itemIndex);
                    int targetedIndex = activeRotation.RotationSteps.FindIndex(x => x.Id == targetedRotationStepId);
                    activeRotation.RotationSteps.Insert(targetedIndex, movingRotationStep);

                    UpdateRotationDisplay();
                    UpdateLayout();
                }

                e.Handled = true;
            }
        }
        private void button_AddRotation(object sender, RoutedEventArgs e) {
            AddRotationDialog dialog = new AddRotationDialog();
            dialog.ShowDialog();
            string newRotationName = dialog.tb_rotationName.Text;
            if (dialog.confirmed && !rotations.ContainsKey(newRotationName)) {
                Rotation newRotation = new Rotation()
                {
                    DisplayName = newRotationName,
                    JobCode = dialog.cb_jobSelector.Text
                };
                rotations.Add(newRotationName, newRotation);
                activeRotationName = newRotationName;
                activeRotation = newRotation;
                lb_rotationsList_SelectionChanged(lb_rotationsList, null);

                UpdateRotationDisplay();
                UpdateRotationListDisplay();
                UpdateActionSet();
            }
        }

        private void button_DeleteRotation(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(activeRotationName)) {
                MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to delete the rotation named \"" + activeRotationName + "\"?", "Confirm Rotation Deletion", MessageBoxButton.YesNo);
                if (messageResult == MessageBoxResult.Yes) {
                    rotations.Remove(activeRotationName);
                    activeRotation = null;
                    activeRotationName = null;
                    if (rotations.Count > 0) {
                        activeRotation = rotations.Values.First();
                        activeRotationName = rotations.Keys.First();
                        lb_rotationsList.SelectedItem = activeRotationName;
                    }
                    UpdateRotationDisplay();
                    UpdateRotationListDisplay();
                    UpdateActionSet();
                }
            }
        }
        private void button_ImportRotation(object sender, RoutedEventArgs e) {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            openFiledialog.Filter = "xml files|*.xml";
            openFiledialog.RestoreDirectory = true;

            bool? success = openFiledialog.ShowDialog();
            if (success != null && success == true) {
                string filePath = openFiledialog.FileName;

                XmlSerializer serializer = new XmlSerializer(typeof(Rotation));
                TextReader reader = new StreamReader(filePath);
                object serializerOutput = serializer.Deserialize(reader);
                Rotation newRotation = serializerOutput as Rotation;
                if (newRotation != null) {
                    rotations[newRotation.DisplayName] = newRotation;
                }

                UpdateRotationListDisplay();
                UpdateLayout();
            }
        }

        private void button_ExportRotation(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML File|*.xml";
            saveFileDialog.Title = "Save Rotation";
            saveFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog.FileName)) {
                XmlSerializer serializer = new XmlSerializer(typeof(Rotation));
                TextWriter writer = new StreamWriter(saveFileDialog.FileName);
                serializer.Serialize(writer, activeRotation);
            }
        }
        private void lb_rotationsList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListBox listBox = sender as ListBox;
            string newRotationName = listBox.SelectedItem as string;
            if (newRotationName != null && newRotationName != activeRotationName) {
                ChangeActiveRotation(newRotationName);
            }
        }
        private void UpdateActionSet() {
            actionSetPanel.Children.Clear();

            foreach (ActionDef action in ActionBank.actionSets[activeRotation.JobCode]) {
                if (action.LevelBasedUpgrade != null) {
                    continue; //TODO: Actually do this level-based instead of just hiding anything that isn't the max upgrade.
                }

                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;
                newPanel.MouseMove += DragActionFromBank;
                newPanel.Margin = new Thickness(1);
                MyXaml.SetActionIdProperty(newPanel, action.UniqueID);

                Image abilityIcon = new Image();
                abilityIcon.Stretch = Stretch.Fill;
                abilityIcon.Source = new BitmapImage(new Uri("/images/icons/" + action.IconName, UriKind.Relative));
                abilityIcon.Width = 32;
                abilityIcon.Height = 32;
                newPanel.Children.Add(abilityIcon);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = action.DisplayName;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Margin = new Thickness(3, 0, 0, 0);
                newPanel.Children.Add(textBlock);

                actionSetPanel.Children.Add(newPanel);
            }
        }

        private void UpdateRotationListDisplay() {
            //-- Update the list in the rotation tab
            lb_rotationsList.Items.Clear();

            foreach (string rotationName in rotations.Keys) {
                lb_rotationsList.Items.Add(rotationName);
            }

            lb_rotationsList.SelectedItem = activeRotationName;

            //-- Update the combo box in the simulate tab
            cb_simulationRotationSelection.Items.Clear();

            foreach (string rotName in rotations.Keys) {
                cb_simulationRotationSelection.Items.Add(rotName);
            }

            cb_simulationRotationSelection.SelectedItem = activeRotationName;
        }

        private void UpdateRotationDisplay() {
            rotationPanel.Children.Clear();

            foreach (RotationStep rotationStep in activeRotation.RotationSteps) {
                if (rotationStep.Type != ERotationStepType.Action)
                    continue;

                string actionDefId = rotationStep.Parameters["action"];
                ActionDef actionDef = ActionBank.actions[actionDefId];

                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;
                newPanel.Margin = new Thickness(1);

                MyXaml.SetRotationStepId(newPanel, rotationStep.Id);
                newPanel.MouseMove += DragRotationElement;
                newPanel.AllowDrop = true;
                newPanel.Drop += rotationElement_drop;

                newPanel.ContextMenu = new ContextMenu();
                MenuItem contextMenuItem = new MenuItem();
                contextMenuItem.Header = "Delete";
                contextMenuItem.Click += RotationElement_ContextMenu_Delete;
                MyXaml.SetRotationStepId(contextMenuItem, rotationStep.Id);
                newPanel.ContextMenu.Items.Add(contextMenuItem);

                if (!actionDef.IsGCD) {
                    newPanel.Children.Add(new Image
                    {
                        Stretch = Stretch.Fill,
                        Source = new BitmapImage(new Uri("/images/icons/down_right_arrow.png", UriKind.Relative)),
                        Width = 32,
                        Height = 32,
                        ToolTip = "This ability is an off-GCD."
                    });
                }
                newPanel.Children.Add(new Image
                {
                    Stretch = Stretch.Fill,
                    Source = new BitmapImage(new Uri("/images/icons/" + actionDef.IconName, UriKind.Relative)),
                    Width = 32,
                    Height = 32
                });

                TextBlock textBlock = new TextBlock();
                textBlock.Text = actionDef.DisplayName;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Margin = new Thickness(3, 0, 0, 0);
                newPanel.Children.Add(textBlock);

                rotationPanel.Children.Add(newPanel);
            }
        }
        #endregion

        #region Gearset Tab Specific

        private GearSet CreateDefaultSetForJob(string jobCode) {
            EJobId jobId = (EJobId)Enum.Parse(typeof(EJobId), jobCode);

            GearSet retVal = new GearSet();
            retVal.JobCode = jobCode;
            retVal.Level = 80; //TODO EW Patch Stuff

            retVal.CriticalHit = StatMath.LEVEL_SUB;
            retVal.DirectHit = StatMath.LEVEL_SUB;
            retVal.SkillSpeed = StatMath.LEVEL_SUB;
            retVal.SpellSpeed = StatMath.LEVEL_SUB;
            retVal.Determination = StatMath.LEVEL_MAIN;
            retVal.Tenacity = StatMath.LEVEL_SUB;
            retVal.Piety = StatMath.LEVEL_MAIN;
            retVal.PhysicalDamage = 1;
            retVal.MagicalDamage = 1;

            retVal.Strength = StatMath.GetBaseStat(jobId, EJobModifierId.STR);
            retVal.Dexterity = StatMath.GetBaseStat(jobId, EJobModifierId.DEX);
            retVal.Intelligence = StatMath.GetBaseStat(jobId, EJobModifierId.INT);
            retVal.Mind = StatMath.GetBaseStat(jobId, EJobModifierId.MND);
            retVal.Vitality = StatMath.GetBaseStat(jobId, EJobModifierId.VIT);

            return retVal;
        }

        private void button_AddGearset(object sender, RoutedEventArgs e) {
            AddGearsetDialog dialog = new AddGearsetDialog();
            dialog.gearsets = gearSets;
            dialog.ShowDialog();
            string newGearsetName = dialog.tb_gearsetName.Text;
            if (dialog.confirmed && !rotations.ContainsKey(newGearsetName)) {
                GearSet newGearset = CreateDefaultSetForJob(dialog.cb_jobSelector.Text);
                newGearset.Name = newGearsetName;
                gearSets.Add(newGearset);
                activeGearset = newGearset;

                UpdateUIFromActiveGearset();
                UpdateGearsetList();
            }
        }

        private void UpdateGearsetList() {
            lb_gearsetList.Items.Clear();
            cb_simulationGearsetSelection.Items.Clear();
            foreach (GearSet gearset in gearSets) {
                lb_gearsetList.Items.Add(gearset.Name);
                cb_simulationGearsetSelection.Items.Add(gearset.Name);
            }
        }

        private void UpdateUIFromActiveGearset() {
            if (activeGearset == null) {
                return;
            }

            tb_gearClass.Text = activeGearset.JobCode;

            EJobId jobId = (EJobId)Enum.Parse(typeof(EJobId), activeGearset.JobCode);
            //--Handle enable/disable of irrelevant stats.
            tb_strength.IsEnabled = false;
            tb_dexterity.IsEnabled = false;
            tb_intelligence.IsEnabled = false;
            tb_mind.IsEnabled = false;
            tb_physDamage.IsEnabled = false;
            tb_magDamage.IsEnabled = false;
            switch (jobId) {
                case EJobId.WHM:
                case EJobId.AST:
                case EJobId.SCH:
                //case EJobId.SGE: TODO EW Patch Stuff
                    tb_tenacity.IsEnabled = false;
                    tb_piety.IsEnabled = true;
                    tb_mind.IsEnabled = true;
                    tb_magDamage.IsEnabled = true;
                    break;
                case EJobId.NIN:
                case EJobId.BRD:
                case EJobId.MCH:
                case EJobId.DNC:
                    tb_dexterity.IsEnabled = true;
                    tb_physDamage.IsEnabled = true;
                    break;
                case EJobId.BLM:
                case EJobId.SMN:
                case EJobId.RDM:
                case EJobId.BLU:
                    tb_intelligence.IsEnabled = true;
                    tb_magDamage.IsEnabled = true;
                    break;
                case EJobId.PLD:
                case EJobId.WAR:
                case EJobId.DRK:
                case EJobId.GNB:
                    tb_tenacity.IsEnabled = true;
                    tb_piety.IsEnabled = false;
                    tb_strength.IsEnabled = true;
                    tb_physDamage.IsEnabled = true;
                    break;
                default:
                    tb_tenacity.IsEnabled = false;
                    tb_piety.IsEnabled = false;
                    tb_strength.IsEnabled = true;
                    tb_physDamage.IsEnabled = true;
                    break;
            }

            tb_physDamage.Text = activeGearset.PhysicalDamage.ToString();
            tb_magDamage.Text = activeGearset.MagicalDamage.ToString();

            tb_strength.Text = activeGearset.Strength.ToString();
            tb_dexterity.Text = activeGearset.Dexterity.ToString();
            tb_intelligence.Text = activeGearset.Intelligence.ToString();
            tb_mind.Text = activeGearset.Mind.ToString();

            tb_critical.Text = activeGearset.CriticalHit.ToString();
            tb_directHit.Text = activeGearset.DirectHit.ToString();
            tb_determination.Text = activeGearset.Determination.ToString();
            tb_skillspeed.Text = activeGearset.SkillSpeed.ToString();
            tb_spellspeed.Text = activeGearset.SpellSpeed.ToString();
            tb_tenacity.Text = activeGearset.Tenacity.ToString();
            tb_piety.Text = activeGearset.Piety.ToString();
        }

        private void button_DeleteGearset(object sender, RoutedEventArgs e) {
            if (activeGearset != null) {
                MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to delete the rotation named \"" + activeGearset.Name + "\"?", "Confirm Rotation Deletion", MessageBoxButton.YesNo);
                if (messageResult == MessageBoxResult.Yes) {
                    gearSets.Remove(activeGearset);
                    activeGearset = null;
                    if (gearSets.Count > 0) {
                        GearSet newSelection = gearSets.First();
                        ChangeActiveGearset(newSelection.Name);
                        UpdateGearsetList();
                    }
                }
            }
        }

        private void lb_gearsetList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListBox listBox = sender as ListBox;
            string newGearsetName = listBox.SelectedItem as string;
            if (newGearsetName != null) {
                ChangeActiveGearset(newGearsetName);
            }
        }

        #region Gear properties text changes
        private void tb_physDamage_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_physDamage.Text);
                activeGearset.PhysicalDamage = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_magDamage_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_magDamage.Text);
                activeGearset.MagicalDamage = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_strength_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_strength.Text);
                activeGearset.Strength = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_dexterity_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try{
                int newValue = Int32.Parse(tb_dexterity.Text);
                activeGearset.Dexterity = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_intelligence_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_intelligence.Text);
                activeGearset.Intelligence = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_mind_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_mind.Text);
                activeGearset.Mind = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_critical_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_critical.Text);
                activeGearset.CriticalHit = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_directHit_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_directHit.Text);
                activeGearset.DirectHit = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_determination_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_determination.Text);
                activeGearset.Determination = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_skillspeed_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_skillspeed.Text);
                activeGearset.SkillSpeed = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_spellspeed_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_spellspeed.Text);
                activeGearset.SpellSpeed = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_tenacity_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_tenacity.Text);
                activeGearset.Tenacity = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        private void tb_piety_TextChanged(object sender, TextChangedEventArgs e) {
            if (activeGearset == null) {
                return;
            }

            try {
                int newValue = Int32.Parse(tb_piety.Text);
                activeGearset.Piety = newValue;
            } catch (Exception) {
                //TODO: Show red?
            }
        }
        #endregion
        #endregion

    }
}
