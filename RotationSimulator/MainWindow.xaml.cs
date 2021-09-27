using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Linq;

namespace RotationSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<RotationStep> activeRotation = new List<RotationStep>();
        public int actionsListTimeOffset = 0;

        public MainWindow()
        {
            InitializeComponent();

            actionsListTimeOffset = -ActionBank.actions["SMN_Ruin3"].CastTime; //remove the 1.5s cast time of ruin3, since it's happening before the pull starts

            List<ActionDef> actionsList = new List<ActionDef>();
            actionsList.Add(ActionBank.actions["SMN_Ruin3"]);
            actionsList.Add(ActionBank.actions["SMN_Devotion"]);
            actionsList.Add(ActionBank.actions["SMN_SummonBahamut"]);
            actionsList.Add(ActionBank.actions["SMN_BahamutEnkindle"]);
            actionsList.Add(ActionBank.actions["SMN_EnergyDrain"]);
            actionsList.Add(ActionBank.actions["SMN_BahamutFiller"]);
            actionsList.Add(ActionBank.actions["SMN_BahamutFiller"]);
            actionsList.Add(ActionBank.actions["SMN_BahamutFiller"]);
            actionsList.Add(ActionBank.actions["SMN_BahamutFiller"]);
            actionsList.Add(ActionBank.actions["SMN_Fester"]);
            actionsList.Add(ActionBank.actions["SMN_Deathflare"]);
            actionsList.Add(ActionBank.actions["SMN_BahamutFiller"]);
            actionsList.Add(ActionBank.actions["SMN_BahamutEnkindle"]);
            actionsList.Add(ActionBank.actions["SMN_Fester"]);

            actionsList.Add(ActionBank.actions["SMN_SummonIfrit"]);
            actionsList.Add(ActionBank.actions["SMN_IfritEA1"]);
            actionsList.Add(ActionBank.actions["SMN_Ruin4"]);
            actionsList.Add(ActionBank.actions["SMN_IfritEA1"]);
            actionsList.Add(ActionBank.actions["SMN_IfritEA2"]);
            actionsList.Add(ActionBank.actions["SMN_IfritEA2"]);

            actionsList.Add(ActionBank.actions["SMN_SummonGaruda"]);
            actionsList.Add(ActionBank.actions["SMN_GarudaEA2"]);
            actionsList.Add(ActionBank.actions["SMN_GarudaEA1"]);
            actionsList.Add(ActionBank.actions["SMN_GarudaEA1"]);
            actionsList.Add(ActionBank.actions["SMN_GarudaEA1"]);
            actionsList.Add(ActionBank.actions["SMN_GarudaEA1"]);

            actionsList.Add(ActionBank.actions["SMN_SummonTitan"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA1"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA2"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA1"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA2"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA1"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA2"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA1"]);
            actionsList.Add(ActionBank.actions["SMN_TitanEA2"]);

            actionsList.Add(ActionBank.actions["SMN_Ruin3"]);
            actionsList.Add(ActionBank.actions["SMN_Ruin3"]);
            actionsList.Add(ActionBank.actions["SMN_Ruin3"]);

            activeRotation = actionsList.Select(x => new RotationStep()
            {
                Type = ERotationStepType.Action,
                parameters = new Dictionary<string, object>()
                {
                    { "action", x }
                }
            }).ToList();

            UpdateRotationDisplay();
            UpdateActionSet();
        }

        private void UpdateRotationDisplay() {
            rotationPanel.Children.Clear();

            foreach (RotationStep rotationStep in activeRotation) {
                if (rotationStep.Type != ERotationStepType.Action)
                    continue;

                ActionDef actionDef = rotationStep.parameters["action"] as ActionDef;

                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;
                newPanel.MouseMove += DragRotationElement;

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
                newPanel.Children.Add(textBlock);

                rotationPanel.Children.Add(newPanel);
            }
        }

        private void DragRotationElement(object sender, MouseEventArgs e) {
            if (sender != null && e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop((DependencyObject)sender, "Test", DragDropEffects.Move);
            }
        }

        private void UpdateActionSet() {
            actionSetPanel.Children.Clear();

            foreach (RotationStep rotationStep in activeRotation) {
                if (rotationStep.Type != ERotationStepType.Action)
                    continue;

                ActionDef action = rotationStep.parameters["action"] as ActionDef;

                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;

                Image abilityIcon = new Image();
                abilityIcon.Stretch = Stretch.Fill;
                abilityIcon.Source = new BitmapImage(new Uri("/images/icons/" + action.IconName, UriKind.Relative));
                newPanel.Children.Add(abilityIcon);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = action.DisplayName;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                newPanel.Children.Add(textBlock);

                actionSetPanel.Children.Add(newPanel);
            }
        }


        private void button_Simulate(object sender, RoutedEventArgs e)
        {
            Simulator simulator = new Simulator();

            List<ActiveEffect> externalEffects = new List<ActiveEffect>();
            if ((bool)cb_opt_simulateTrickAttack.IsChecked) {
                externalEffects.Add(new ActiveEffect()
                {
                    ActiveStartTime = 850,
                    ActiveEndTime = 1500 + 850,
                    effect = EffectsBank.effects["NIN_TrickAttack"],
                });
            }

            SimulationResults results = simulator.Simulate(activeRotation, actionsListTimeOffset, externalEffects);

            textBlock.Text = "PPS: " + results.pps + "\n" +
                             "ePPS: " + results.epps + "\n" + 
                             "Time: " + (float)results.totalTime/100 + "s";
        }

        private void button_AddRotation(object sender, RoutedEventArgs e) {
            AddRotationDialog dialog = new AddRotationDialog();
            dialog.ShowDialog();
        }


    }
}
