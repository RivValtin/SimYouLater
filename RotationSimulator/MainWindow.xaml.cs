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

        public MainWindow() {
            InitializeComponent();
            //TODO: Load rotations from disk here.
            string rotationsFilePath = GetRotationsSaveFilePath();
            if (File.Exists(rotationsFilePath)) {
                XmlSerializer serializer = new XmlSerializer(typeof(RotationCollection));
                TextReader reader = new StreamReader(rotationsFilePath);
                object serializerOutput = serializer.Deserialize(reader);

                RotationCollection rotationCollection = serializerOutput as RotationCollection;
                rotations = rotationCollection;
            }
            if (rotations.Keys.Count > 0) {
                activeRotation = rotations.First().Value;
                activeRotationName = rotations.First().Key;
            }

            UpdateRotationListDisplay();
            UpdateRotationDisplay();
            UpdateActionSet();
            cb_gearClass_SelectionChanged(null, null);
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
                DragDrop.DoDragDrop((DependencyObject)sender, activeRotation.RotationSteps.First(x=>x.Id == MyXaml.GetRotationStepId(sender as DependencyObject)), DragDropEffects.Move);
            }
        }

        private void DragActionFromBank(object sender, MouseEventArgs e) {
            if (sender != null && e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop((DependencyObject)sender, ActionBank.actions[MyXaml.GetActionIdProperty(sender as DependencyObject)], DragDropEffects.Move);
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


        private void button_Simulate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(activeRotationName)) {
                return;
            }

            SimLog.Clear();

            Simulator simulator = new Simulator();
            simulator.AnimationLock = int.Parse(tb_opt_ping.Text) / 10 + 60;
            if (simulator.AnimationLock < 61) {
                simulator.AnimationLock = 61;
            }

            CharacterStats charStats = new CharacterStats((EJobId)Enum.Parse(typeof(EJobId), activeRotation.JobCode, true))
            {
                CriticalHitSubstat = Int32.Parse(tb_critical.Text),
                DirectHitSubstat = Int32.Parse(tb_directHit.Text),
                DeterminationSubstat = Int32.Parse(tb_determination.Text),
                SkillSpeed = Int32.Parse(tb_skillspeed.Text),
                SpellSpeed = Int32.Parse(tb_spellspeed.Text)
            };
            simulator.CharStats = charStats;

            List<ActiveEffect> externalEffects = new List<ActiveEffect>();
            if ((bool)cb_opt_simulateTrickAttack.IsChecked) {
                externalEffects.Add(new ActiveEffect()
                {
                    ActiveStartTime = 850,
                    ActiveEndTime = 1500 + 850,
                    effect = EffectsBank.effects["NIN_TrickAttack"],
                });
            }

            SimulationResults results = simulator.Simulate(activeRotation.RotationSteps, activeRotation.StartTimeOffset, externalEffects);

            textBlock.Text = "PPS: " + results.pps + "\n" +
                             "ePPS: " + results.epps + "\n" +
                             "Time: " + (float)results.totalTime / 100 + "s";
            UpdateLogText();
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
        private void OnExit(object sender, EventArgs e) {
            string saveFilePath = GetRotationsSaveFilePath();

            XmlSerializer serializer = new XmlSerializer(typeof(RotationCollection));
            TextWriter writer = new StreamWriter(saveFilePath);
            serializer.Serialize(writer, rotations);
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

        private void lb_rotationsList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListBox listBox = sender as ListBox;
            string newRotationName = listBox.SelectedItem as string;
            if (newRotationName != null && newRotationName != activeRotationName) {
                ChangeActiveRotation(newRotationName);
            }
        }

        private void cb_simulationRotationSelection_Changed(object sender, SelectionChangedEventArgs e) {
            ComboBox comboBox = sender as ComboBox;
            string newRotation = comboBox.SelectedItem as string;
            if (newRotation != null && newRotation != activeRotationName) {
                ChangeActiveRotation(newRotation);
            }
        }

        private void ChangeActiveRotation(string rotationName) {
            activeRotationName = rotationName;
            activeRotation = rotations[activeRotationName];
            UpdateRotationDisplay();
            UpdateRotationListDisplay();
            UpdateActionSet();
        }

        private void cmb_logLevel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (tb_logOutput != null) {
                UpdateLogText();
            }
        }

        private void cb_gearClass_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (tb_strength != null) {
                tb_strength.IsEnabled = false;
                tb_dexterity.IsEnabled = false;
                tb_intelligence.IsEnabled = false;
                tb_mind.IsEnabled = false;
                tb_tenacity.IsEnabled = false;

                ComboBoxItem selecteditem = cb_gearClass.Items[cb_gearClass.SelectedIndex] as ComboBoxItem;

                EJobId selectedJob = (EJobId)Enum.Parse(typeof(EJobId), selecteditem.Content as string, true);

                string tenText = tb_tenacity.Text;
                tb_tenacity.Text = StatMath.LEVEL_SUB.ToString();
                switch (selectedJob) {
                    case EJobId.WHM:
                    case EJobId.AST:
                    case EJobId.SCH:
                    //case EJobId.SGE: TODO: EW Patch Stuff
                        tb_mind.IsEnabled = true;
                        break;
                    case EJobId.NIN:
                    case EJobId.BRD:
                    case EJobId.MCH:
                    case EJobId.DNC:
                        tb_dexterity.IsEnabled = true;
                        break;
                    case EJobId.SMN:
                    case EJobId.BLM:
                    case EJobId.RDM:
                    case EJobId.BLU:
                        tb_intelligence.IsEnabled = true;
                        break;
                    case EJobId.PLD:
                    case EJobId.DRK:
                    case EJobId.WAR:
                    case EJobId.GNB:
                        tb_strength.IsEnabled = true;
                        tb_tenacity.IsEnabled = true;
                        tb_tenacity.Text = tenText;
                        break;
                    default:
                        tb_strength.IsEnabled = true;
                        break;
                }

                tb_strength.Text = StatMath.GetBaseStat(selectedJob, EJobModifierId.STR).ToString();
                tb_dexterity.Text = StatMath.GetBaseStat(selectedJob, EJobModifierId.DEX).ToString();
                tb_mind.Text = StatMath.GetBaseStat(selectedJob, EJobModifierId.MND).ToString();
                tb_intelligence.Text = StatMath.GetBaseStat(selectedJob, EJobModifierId.INT).ToString();
            }
        }
    }
}
