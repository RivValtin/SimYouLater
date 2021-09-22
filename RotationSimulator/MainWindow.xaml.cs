using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RotationSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<RotationStep> activeRotation = new List<RotationStep>();
        public int activeRotationTimeOffset = 0;

        public MainWindow()
        {
            InitializeComponent();

            //Testing with a 2.5s theoretical SMN rotation, even though I think 2.48s will be BiS.
            activeRotationTimeOffset = -ActionBank.SMN_Ruin3.castTime; //remove the 1.5s cast time of ruin3, since it's happening before the pull starts
            activeRotation.Add(ActionBank.SMN_Ruin3);
            activeRotation.Add(ActionBank.SMN_Devotion);
            activeRotation.Add(ActionBank.SMN_SummonBahamut);
            activeRotation.Add(ActionBank.SMN_BahamutEnkindle);
            activeRotation.Add(ActionBank.SMN_EnergyDrain);
            activeRotation.Add(ActionBank.SMN_BahamutFiller);
            activeRotation.Add(ActionBank.SMN_BahamutFiller);
            activeRotation.Add(ActionBank.SMN_BahamutFiller);
            activeRotation.Add(ActionBank.SMN_BahamutFiller);
            activeRotation.Add(ActionBank.SMN_Fester);
            activeRotation.Add(ActionBank.SMN_Deathflare);
            activeRotation.Add(ActionBank.SMN_BahamutFiller);
            activeRotation.Add(ActionBank.SMN_BahamutEnkindle);
            activeRotation.Add(ActionBank.SMN_BahamutFiller);
            activeRotation.Add(ActionBank.SMN_Fester);

            activeRotation.Add(ActionBank.SMN_SummonIfrit);
            activeRotation.Add(ActionBank.SMN_IfritEA1);
            activeRotation.Add(ActionBank.SMN_Ruin4);
            activeRotation.Add(ActionBank.SMN_IfritEA1);
            activeRotation.Add(ActionBank.SMN_IfritEA2);
            activeRotation.Add(ActionBank.SMN_IfritEA2);

            activeRotation.Add(ActionBank.SMN_SummonGaruda);
            activeRotation.Add(ActionBank.SMN_GarudaEA2);
            activeRotation.Add(ActionBank.SMN_GarudaEA1);
            activeRotation.Add(ActionBank.SMN_GarudaEA1);
            activeRotation.Add(ActionBank.SMN_GarudaEA1);
            activeRotation.Add(ActionBank.SMN_GarudaEA1);

            activeRotation.Add(ActionBank.SMN_SummonTitan);
            activeRotation.Add(ActionBank.SMN_TitanEA1);
            activeRotation.Add(ActionBank.SMN_TitanEA2);
            activeRotation.Add(ActionBank.SMN_TitanEA1);
            activeRotation.Add(ActionBank.SMN_TitanEA2);
            activeRotation.Add(ActionBank.SMN_TitanEA1);
            activeRotation.Add(ActionBank.SMN_TitanEA2);
            activeRotation.Add(ActionBank.SMN_TitanEA1);
            activeRotation.Add(ActionBank.SMN_TitanEA2);

            activeRotation.Add(ActionBank.SMN_Ruin3);
            activeRotation.Add(ActionBank.SMN_Ruin3);

            actionSetDropdown.Items.Add("SMN");
            actionSetDropdown.SelectedItem = "SMN";

            UpdateRotationDisplay();
            UpdateActionSet();
        }

        private void UpdateRotationDisplay() {
            rotationPanel.Children.Clear();

            foreach (RotationStep step in activeRotation) {
                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;

                if (!step.IsGCD) {
                    Image blankImage = new Image();
                    blankImage.Stretch = Stretch.Fill;
                    blankImage.Source = new BitmapImage(new Uri("/images/icons/down_right_arrow.png", UriKind.Relative));
                    blankImage.Width = 32;
                    blankImage.Height = 32;
                    blankImage.ToolTip = "This ability is an off-GCD.";

                    newPanel.Children.Add(blankImage);
                }
                Image abilityIcon = new Image();
                abilityIcon.Stretch = Stretch.Fill;
                abilityIcon.Source = new BitmapImage(new Uri("/images/icons/" + step.IconName, UriKind.Relative));
                abilityIcon.Width = 32;
                abilityIcon.Height = 32;
                newPanel.Children.Add(abilityIcon);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = step.DisplayName;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                newPanel.Children.Add(textBlock);

                rotationPanel.Children.Add(newPanel);
            }
        }

        private void UpdateActionSet() {
            actionSetPanel.Children.Clear();
            List<RotationStep> newActionSet = ActionBank.actionSets[actionSetDropdown.SelectedItem.ToString()];

            foreach (RotationStep action in newActionSet) {
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


        private void button_Click(object sender, RoutedEventArgs e)
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
    }
}
