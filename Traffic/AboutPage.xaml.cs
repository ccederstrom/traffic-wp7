using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Coding4Fun.Phone.Controls.Data;
using System.IO.IsolatedStorage;

namespace Traffic
{
    public partial class AboutPage : PhoneApplicationPage
    {
        IsolatedStorageSettings isolatedStorageSettings;
        MarketplaceReviewTask _marketplaceReview = new MarketplaceReviewTask();
        MarketplaceSearchTask _marketplaceSearch = new MarketplaceSearchTask();
        EmailComposeTask _emailComposeTask = new EmailComposeTask();

        public AboutPage()
        {
            InitializeComponent();


            #region trial Check
            if (App.IsTrial == true)
            {
                // enable ads
                adControl.Visibility = System.Windows.Visibility.Visible;
                //adControl.IsAutoRefreshEnabled = true;
                adControl.IsEnabled = true;
                adControl.IsAutoCollapseEnabled = false;
            }
            else
            {
                // disables ads
                adControl.Visibility = System.Windows.Visibility.Collapsed;
                //adControl.IsAutoRefreshEnabled = false;
                adControl.IsEnabled = false;
                adControl.IsAutoCollapseEnabled = false;
            }
            #endregion


            isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;

          //  txtHelp.Text = "We will continue to roll out new features and your suggestions.";
            #region Paid version Check
            //if (App.isPaidVersion == true)
            //{
            //    // disables ads
            //    adControl.Visibility = System.Windows.Visibility.Collapsed;
            //    //adControl.IsAutoRefreshEnabled = false;
            //    adControl.IsEnabled = false;
            //    adControl.IsAutoCollapseEnabled = false;
            //    //direction.Margin = new Thickness(0, 0, 0, 0);
            //}
            //else
            //{
            //    // enable ads
            //    adControl.Visibility = System.Windows.Visibility.Visible;
            //    //adControl.IsAutoRefreshEnabled = true;
            //    adControl.IsEnabled = true;
            //    adControl.IsAutoCollapseEnabled = false;
            //}
            #endregion
            // http://www.windowsphonegeek.com/articles/Getting-data-out-of-WP7-WMAppManifest-is-easy-with-Coding4Fun-PhoneHelper
           
            txtAppName.Text = PhoneHelper.GetAppAttribute("Title") + " by " + PhoneHelper.GetAppAttribute("Author");
            txtVersion.Text = "version " + PhoneHelper.GetAppAttribute("Version");
            txtDescription.Text = PhoneHelper.GetAppAttribute("Description");

        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            isolatedStorageSettings["trackCurrentLocation"] = TrackCurrentLocationToggleSwitch.IsChecked;
            isolatedStorageSettings["accidents"] = AccidentsToggleSwitch.IsChecked;
            isolatedStorageSettings["congestion"] = CongestionToggleSwitch.IsChecked;
            isolatedStorageSettings["disabledVehicles"] = DisabledVehiclesToggleSwitch.IsChecked;
            isolatedStorageSettings["massTransit"] = MassTransitToggleSwitch.IsChecked;
            isolatedStorageSettings["miscellaneous"] = MiscellaneousToggleSwitch.IsChecked;
            isolatedStorageSettings["otherNews"] = OtherNewsToggleSwitch.IsChecked;
            isolatedStorageSettings["plannedEvents"] = PlannedEventsToggleSwitch.IsChecked;
            isolatedStorageSettings["roadHazard"] = RoadHazardToggleSwitch.IsChecked;
            isolatedStorageSettings["construction"] = ConstructionToggleSwitch.IsChecked;
            isolatedStorageSettings["alert"] = AlertToggleSwitch.IsChecked;
            isolatedStorageSettings["weather"] = WeatherToggleSwitch.IsChecked;
            isolatedStorageSettings["lowImpact"] = LowImpactToggleSwitch.IsChecked;
            isolatedStorageSettings["minorImpact"] = MinorToggleSwitch.IsChecked;
            isolatedStorageSettings["moderateImpact"] = ModerateToggleSwitch.IsChecked;
            isolatedStorageSettings["seriousImpact"] = SeriousToggleSwitch.IsChecked;
            isolatedStorageSettings.Save();

        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isolatedStorageSettings.Count > 0)
            {
                if (isolatedStorageSettings.Contains("trackCurrentLocation"))
                {
                    TrackCurrentLocationToggleSwitch.IsChecked = (bool)isolatedStorageSettings["trackCurrentLocation"];
                }
                
                if (isolatedStorageSettings.Contains("accidents"))
                {
                    AccidentsToggleSwitch.IsChecked = (bool)isolatedStorageSettings["accidents"];
                }

                if (isolatedStorageSettings.Contains("congestion"))
                {
                    CongestionToggleSwitch.IsChecked = (bool)isolatedStorageSettings["congestion"];
                }

                if (isolatedStorageSettings.Contains("disabledVehicles"))
                {
                    DisabledVehiclesToggleSwitch.IsChecked = (bool)isolatedStorageSettings["disabledVehicles"];
                }

                if (isolatedStorageSettings.Contains("massTransit"))
                {
                    MassTransitToggleSwitch.IsChecked = (bool)isolatedStorageSettings["massTransit"];
                }

                if (isolatedStorageSettings.Contains("miscellaneous"))
                {
                    MiscellaneousToggleSwitch.IsChecked = (bool)isolatedStorageSettings["miscellaneous"];
                }

                if (isolatedStorageSettings.Contains("otherNews"))
                {
                    OtherNewsToggleSwitch.IsChecked = (bool)isolatedStorageSettings["otherNews"];
                }

                if (isolatedStorageSettings.Contains("plannedEvents"))
                {
                    PlannedEventsToggleSwitch.IsChecked = (bool)isolatedStorageSettings["plannedEvents"];
                }
                if (isolatedStorageSettings.Contains("roadHazard"))
                {
                    RoadHazardToggleSwitch.IsChecked = (bool)isolatedStorageSettings["roadHazard"];
                }

                if (isolatedStorageSettings.Contains("construction"))
                {
                    ConstructionToggleSwitch.IsChecked = (bool)isolatedStorageSettings["construction"];
                }

                if (isolatedStorageSettings.Contains("alert"))
                {
                    AlertToggleSwitch.IsChecked = (bool)isolatedStorageSettings["alert"];
                }

                if (isolatedStorageSettings.Contains("weather"))
                {
                    WeatherToggleSwitch.IsChecked = (bool)isolatedStorageSettings["weather"];
                }

                if (isolatedStorageSettings.Contains("lowImpact"))
                {
                    LowImpactToggleSwitch.IsChecked = (bool)isolatedStorageSettings["lowImpact"];
                }

                if (isolatedStorageSettings.Contains("minorImpact"))
                {
                    MinorToggleSwitch.IsChecked = (bool)isolatedStorageSettings["minorImpact"];
                }

                if (isolatedStorageSettings.Contains("moderateImpact"))
                {
                    ModerateToggleSwitch.IsChecked = (bool)isolatedStorageSettings["moderateImpact"];
                }

                if (isolatedStorageSettings.Contains("seriousImpact"))
                {
                    SeriousToggleSwitch.IsChecked = (bool)isolatedStorageSettings["seriousImpact"];
                }
            }
        }



        private void btnMarketplace_Click(object sender, RoutedEventArgs e)
        {
            _marketplaceSearch.SearchTerms = "PNGC WP7";
            _marketplaceSearch.Show();
        }

        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            _marketplaceReview.Show();
        }

        private void btnContact_Click(object sender, RoutedEventArgs e)
        {
            _emailComposeTask.To = "pngc.wp7@hotmail.com";
            _emailComposeTask.Subject = "Traffic Feedback";
            _emailComposeTask.Show();
        }

        private void IsolatedStorageSettingsToggle(string setting)
        {
            if (isolatedStorageSettings.Contains(setting))
            {
                bool toggle = (bool)isolatedStorageSettings[setting];
                if (toggle == true)
                {
                    isolatedStorageSettings[setting] = false;
                }
                else
                {
                    isolatedStorageSettings[setting] = true;
                }
            }
            else
            {
                // set to false becasue it will initally be set to true.
                isolatedStorageSettings.Add(setting, false);
            }
        }

        private void AccidentsToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "accidents";
            IsolatedStorageSettingsToggle(setting);

        }

        private void AccidentsToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "accidents";
            IsolatedStorageSettingsToggle(setting);
        }

        private void CongestionToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "congestion";
            IsolatedStorageSettingsToggle(setting);
        }

        private void CongestionToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "congestion";
            IsolatedStorageSettingsToggle(setting);
        }

        private void DisabledVehiclesToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "disabledVehicles";
            IsolatedStorageSettingsToggle(setting);
        }

        private void DisabledVehiclesToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "disabledVehicles";
            IsolatedStorageSettingsToggle(setting);
        }

        private void MassTransitToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "massTransit";
            IsolatedStorageSettingsToggle(setting);
        }

        private void MassTransitToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "massTransit";
            IsolatedStorageSettingsToggle(setting);
        }

        private void MiscellaneousToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "miscellaneous";
            IsolatedStorageSettingsToggle(setting);
        }

        private void MiscellaneousToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "miscellaneous";
            IsolatedStorageSettingsToggle(setting);
        }

        private void OtherNewsToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "otherNews";
            IsolatedStorageSettingsToggle(setting);
        }

        private void OtherNewsToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "otherNews";
            IsolatedStorageSettingsToggle(setting);
        }

        private void PlannedEventsToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "plannedEvents";
            IsolatedStorageSettingsToggle(setting);
        }

        private void PlannedEventsToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "plannedEvents";
            IsolatedStorageSettingsToggle(setting);
        }

        private void RoadHazardToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "roadHazard";
            IsolatedStorageSettingsToggle(setting);
        }

        private void RoadHazardToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "roadHazard";
            IsolatedStorageSettingsToggle(setting);
        }

        private void ConstructionToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "construction";
            IsolatedStorageSettingsToggle(setting);
        }

        private void ConstructionToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "construction";
            IsolatedStorageSettingsToggle(setting);
        }

        private void AlertToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "alert";
            IsolatedStorageSettingsToggle(setting);
        }

        private void AlertToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "alert";
            IsolatedStorageSettingsToggle(setting);
        }

        private void WeatherToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "weather";
            IsolatedStorageSettingsToggle(setting);
        }

        private void WeatherToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "weather";
            IsolatedStorageSettingsToggle(setting);
        }

        private void LowImpactToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "lowImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void LowImpactToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "lowImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void MinorToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "minorImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void MinorToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "minorImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void ModerateToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "moderateImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void ModerateToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "moderateImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void SeriousToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "seriousImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void SeriousToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "seriousImpact";
            IsolatedStorageSettingsToggle(setting);
        }

        private void TrackCurrentLocationToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            string setting = "trackCurrentLocation";
            IsolatedStorageSettingsToggle(setting);
        }

        private void TrackCurrentLocationToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string setting = "trackCurrentLocation";
            IsolatedStorageSettingsToggle(setting);
        }
    }
}