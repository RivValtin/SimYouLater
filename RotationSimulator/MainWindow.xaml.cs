using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RotationSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<ActionDef> activeRotation = new List<ActionDef>();
        public int activeRotationTimeOffset = 0;

        public MainWindow()
        {
            InitializeComponent();

            activeRotationTimeOffset = -ActionBank.actions["SMN_Ruin3"].CastTime; //remove the 1.5s cast time of ruin3, since it's happening before the pull starts
            activeRotation.Add(ActionBank.actions["SMN_Ruin3"]);
            activeRotation.Add(ActionBank.actions["SMN_Devotion"]);
            activeRotation.Add(ActionBank.actions["SMN_SummonBahamut"]);
            activeRotation.Add(ActionBank.actions["SMN_BahamutEnkindle"]);
            activeRotation.Add(ActionBank.actions["SMN_EnergyDrain"]);
            activeRotation.Add(ActionBank.actions["SMN_BahamutFiller"]);
            activeRotation.Add(ActionBank.actions["SMN_BahamutFiller"]);
            activeRotation.Add(ActionBank.actions["SMN_BahamutFiller"]);
            activeRotation.Add(ActionBank.actions["SMN_BahamutFiller"]);
            activeRotation.Add(ActionBank.actions["SMN_Fester"]);
            activeRotation.Add(ActionBank.actions["SMN_Deathflare"]);
            activeRotation.Add(ActionBank.actions["SMN_BahamutFiller"]);
            activeRotation.Add(ActionBank.actions["SMN_BahamutEnkindle"]);

            activeRotation.Add(ActionBank.actions["SMN_SummonIfrit"]);
            activeRotation.Add(ActionBank.actions["SMN_Fester"]);
            activeRotation.Add(ActionBank.actions["SMN_IfritEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_Ruin4"]);
            activeRotation.Add(ActionBank.actions["SMN_IfritEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_IfritEA2"]);
            activeRotation.Add(ActionBank.actions["SMN_IfritEA2"]);

            activeRotation.Add(ActionBank.actions["SMN_SummonGaruda"]);
            activeRotation.Add(ActionBank.actions["SMN_GarudaEA2"]);
            activeRotation.Add(ActionBank.actions["SMN_GarudaEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_GarudaEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_GarudaEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_GarudaEA1"]);

            activeRotation.Add(ActionBank.actions["SMN_SummonTitan"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA2"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA2"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA2"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA1"]);
            activeRotation.Add(ActionBank.actions["SMN_TitanEA2"]);

            activeRotation.Add(ActionBank.actions["SMN_Ruin3"]);
            activeRotation.Add(ActionBank.actions["SMN_Ruin3"]);
            activeRotation.Add(ActionBank.actions["SMN_Ruin3"]);

            UpdateRotationDisplay();
            UpdateActionSet();
        }

        private void UpdateRotationDisplay() {
            rotationPanel.Children.Clear();

            foreach (ActionDef step in activeRotation) {
                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;

                if (!step.IsGCD) {
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
                    Source = new BitmapImage(new Uri("/images/icons/" + step.IconName, UriKind.Relative)),
                    Width = 32,
                    Height = 32
                });

                TextBlock textBlock = new TextBlock();
                textBlock.Text = step.DisplayName;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                newPanel.Children.Add(textBlock);

                rotationPanel.Children.Add(newPanel);
            }
        }

        private void UpdateActionSet() {
            actionSetPanel.Children.Clear();

            foreach (ActionDef action in activeRotation) {
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
                    type = EActiveEffect.NIN_TrickAttack,
                    DisplayName = "Trick Attack"
                });
            }

            SimulationResults results = simulator.Simulate(activeRotation, activeRotationTimeOffset, externalEffects);

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
