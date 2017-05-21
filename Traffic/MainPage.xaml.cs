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
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using Traffic.Model;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.IO.IsolatedStorage;


namespace Traffic
{
    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageSettings isolatedStorageSettings;
        private const string Id = "AmptuEf6gI3g55lRaJNzSQ47SvHNHaiOy7j2ROkiwOIGyBkrmaDgNQTMot6uOp3O";
        Pushpin Current_Location = new Pushpin();
        GeoCoordinateWatcher watcher;
        System.Windows.Point MyLocation;
        System.Windows.Point MyPreviousLocation;
        ProgressIndicator prog;
        public ObservableCollection<TrafficIncident> IncidentList { get; private set; }
        public ObservableCollection<TrafficIncident> FreewayList { get; set; }
        public ObservableCollection<TrafficIncident> RoadList { get; set; }
        private WebClient wc;
        public ObservableCollection<PushpinModel> Pushpins { get; set; }
        public ObservableCollection<PushpinModel> FreewayPushpins { get; set; }
        public ObservableCollection<PushpinModel> RoadPushpins { get; set; }

        private string TRAFFIC_ADDRESS;
        private TrafficIncident selectedIncident;
        private Boolean map_expanded = false;
        double MapHeight = 800;

        #region Initialization
        // Constructor
        public MainPage()
        {

            InitializeComponent();


            #region trial Check
            if (App.IsTrial==true)
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


            OrientationChanged += new EventHandler<OrientationChangedEventArgs>(MainPage_OrientationChanged);

            // initially check 
            PageOrientation po = this.Orientation;
            if (po == PageOrientation.Portrait || po == PageOrientation.PortraitDown || po == PageOrientation.PortraitUp)
            {
                // return pivot to original position
                MapHeight = 800;
                MainPivot.Margin = new Thickness(0, 125, 0, 0);
                AllPivot.Header = "all";
                FreewayPivot.Header = "highways";
                RoadPivot.Header = "roads";

               // this.ApplicationBar.IsVisible = this.ApplicationBar.IsVisible;
            }
            else if (po == PageOrientation.Landscape || po==PageOrientation.LandscapeLeft || po==PageOrientation.LandscapeRight)
            {
                // hiding pivot header in landscape mode
                MapHeight = 480;
                MainPivot.Margin = new Thickness(0,30,0,0);
                AllPivot.Header = "";
                FreewayPivot.Header = "";
                RoadPivot.Header = "";
            //    this.ApplicationBar.IsVisible = !this.ApplicationBar.IsVisible;
            }



            isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;


            // disable lock screen timer
            Microsoft.Phone.Shell.PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            // References:
            // http://weblogs.asp.net/scottgu/archive/2010/03/18/building-a-windows-phone-7-twitter-application-using-silverlight.aspx
            // http://msdn.microsoft.com/en-us/library/hh441726.aspx
            // http://json.codeplex.com
            // Bing Maps REST Services API Reference: http://msdn.microsoft.com/en-us/library/ff701722.aspx
            // Adding Tile Layers (overlays) http://msdn.microsoft.com/en-us/library/ee681902.aspx

            wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(REST_traffic);
            //wc.DownloadStringAsync(new Uri("http://dev.virtualearth.net/REST/v1/Traffic/Incidents/37,-105,45,-94?key=" + Id));

            // caching
            Current_Location.CacheMode = new BitmapCache();
            mMap.ZoomLevel = 10;
            //mMap.ZoomBarVisibility = System.Windows.Visibility.Visible;
            Current_Location.Style = (Style)(Application.Current.Resources["PushpinStyle"]);
            Current_Location.PositionOrigin = PositionOrigin.Center;

            MyPreviousLocation.X = 0; MyPreviousLocation.Y = 0;
            #region Progress Indicator
            SystemTray.SetIsVisible(this, true);
            SystemTray.SetOpacity(this, 0.5);
            SystemTray.SetBackgroundColor(this, Colors.Black);
            SystemTray.SetForegroundColor(this, Colors.White);
            prog = new ProgressIndicator();
            prog.IsIndeterminate = true;
            prog.IsVisible = true;
            SystemTray.SetProgressIndicator(this, prog);

            #endregion
           
          }


        void MainPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            PageOrientation po = this.Orientation;
            //Make any changes here before screen gets refreshed

            if (po == PageOrientation.Portrait || po == PageOrientation.PortraitDown || po == PageOrientation.PortraitUp)
            {
                // set pivot to original position
                MainPivot.Margin = new Thickness(0, 125, 0, 0);
                AllPivot.Header = "all";
                FreewayPivot.Header = "highways";
                RoadPivot.Header = "roads";
                MapHeight = 800; // update setting for button click
                this.ApplicationBar.IsVisible = this.ApplicationBar.IsVisible;
                Debug.WriteLine("portrait orientation");
                adControl.Visibility = System.Windows.Visibility.Visible;
            }
            else if (po == PageOrientation.Landscape || po == PageOrientation.LandscapeLeft || po == PageOrientation.LandscapeRight)
            {
                // hiding pivot header in landscape mode
                MainPivot.Margin = new Thickness(0, 30, 0, 0);
                AllPivot.Header = "";
                FreewayPivot.Header = "";
                RoadPivot.Header = "";
                MapHeight = 480; // update setting for button click
                this.ApplicationBar.IsVisible = !this.ApplicationBar.IsVisible;
                Debug.WriteLine("landscape orientation");
                adControl.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (map_expanded == true)
            {
                MapGrid.Height = MapHeight;
                mMap.Height = MapHeight;
                PushpinLayer.Height = MapHeight;
            }
        }
         
        


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            isolatedStorageSettings["currentLatitude"] = mMap.Center.Latitude;
            isolatedStorageSettings["currentLongitude"] = mMap.Center.Longitude;
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // set center of map to last location
            if (isolatedStorageSettings.Contains("currentLatitude"))
            {
                mMap.Center = new GeoCoordinate((double)isolatedStorageSettings["currentLatitude"], (double)isolatedStorageSettings["currentLongitude"]);
            }
            

            if (isolatedStorageSettings.Contains("trackCurrentLocation"))
            {
                if ((bool)isolatedStorageSettings["trackCurrentLocation"] == true)
                {
                    StartLocationService(GeoPositionAccuracy.High);
                }
                else
                {
                    // stop location
                    StopLocationService();
                }
            }
            else
            {
                // first time running app
                StartLocationService(GeoPositionAccuracy.High);
            }
        }

        #endregion
        


        #region LocationService
        private void StartLocationService(GeoPositionAccuracy accuracy)
        {
            Debug.WriteLine("StartLocationService");
            // Reinitialize the GeoCoordinateWatcher
            //StatusTextBlock.Text = "starting, " + accuracyText;
            watcher = new GeoCoordinateWatcher(accuracy);
            watcher.MovementThreshold = 10;

            // Add event handlers for StatusChanged and PositionChanged events
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            // Start data acquisition
            watcher.Start();
        }

        private void StopLocationService()
        {
            if (watcher == null)
            {
                // do nothing
            }
            else
            {
                watcher.Stop();
            }
        }


        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Debug.WriteLine("watcher_StatusChanged");
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    Debug.WriteLine("disabled");
                    break;
                case GeoPositionStatus.Initializing:
                    Debug.WriteLine("initializing");
                    break;
                case GeoPositionStatus.NoData:
                    Debug.WriteLine("nodata");
                    break;
                case GeoPositionStatus.Ready:
                    Debug.WriteLine("ready");
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Debug.WriteLine("watcher_PositionChanged");
            MyLocation.X = e.Position.Location.Longitude;
            MyLocation.Y = e.Position.Location.Latitude;
            //speed_text.Text = e.Position.Location.Speed.ToString();
            Debug.WriteLine("({0},{1})", e.Position.Location.Latitude, e.Position.Location.Longitude);

            mMap.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            //---set the location for the Current_Location pushpin---
            Current_Location.Location = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);


            //drawCircle(new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude), 10);
            //drawCircle(new GeoCoordinate(e.Position.Location.Latitude-10, e.Position.Location.Longitude-10), 10);

            //---add the Current_Location pushpin to the map---
            if (mMap.Children.Contains(Current_Location) != true)
            {
                mMap.Children.Add(Current_Location);
            }
            else
            {
                mMap.Children.Remove(Current_Location);
                mMap.Children.Add(Current_Location);
            }

            //Update Traffic Incident: http://msdn.microsoft.com/en-us/library/hh441726.aspx
            // String address = "http://dev.virtualearth.net/REST/v1/Traffic/Incidents/";
            //address += (e.Position.Location.Latitude - 0.5) + ",";
            //address += (e.Position.Location.Longitude - 0.5) + ",";
            //address += (e.Position.Location.Latitude + 0.5) + ",";
            //address += (e.Position.Location.Longitude + 0.5);
            //address += "/true?t=1,2,3,4,5,6,7,8,9,10,11&s=1,2,3,4&key=";

            Debug.WriteLine("The distance is "+ GeoCodeCalc.CalcDistance(MyLocation.Y,MyLocation.X, MyPreviousLocation.Y, MyPreviousLocation.X));
            double distance = GeoCodeCalc.CalcDistance(MyLocation.Y, MyLocation.X, MyPreviousLocation.Y, MyPreviousLocation.X);
            if (distance >= 2)
            {
                TRAFFIC_ADDRESS = updateTrafficIncidentUrl(e.Position.Location); // method creates traffic url

                // if traffic url is not empty
                if (TRAFFIC_ADDRESS.Count() > 1)
                {
                    if (!wc.IsBusy)
                    {
                        wc.DownloadStringAsync(new Uri(TRAFFIC_ADDRESS));
                    }
                }
                MyPreviousLocation = MyLocation;
                if (Pushpins != null && selectedIncident != null)
                {
                    ObservableCollection<PushpinModel> tempPuspins = Pushpins;
                    PivotItem currentPivotItem = (PivotItem)MainPivot.SelectedItem;
                    string currentHeader = (string)currentPivotItem.Header;
                    if (currentHeader == "all") tempPuspins = Pushpins;
                    else if (currentHeader == "highways") tempPuspins = FreewayPushpins;
                    else if (currentHeader == "roads") tempPuspins = RoadPushpins;
                    foreach (PushpinModel pm in tempPuspins)
                    {
                        pm.severityColor = pm.trafficIncident.severityColor;
                        pm.foregroundColor = "Black";
                        if (pm.trafficIncident.Equals(selectedIncident))
                        {
                            pm.severityColor = "Black";
                            pm.foregroundColor = "White";
                            PushpinLayer.ItemsSource = null; 
                            PushpinLayer.ItemsSource = Pushpins;
                            break;
                        }
                    }
                }
            }
        }

        public static class GeoCodeCalc
        {
            public const double EarthRadiusInMiles = 3956.0;
            public const double EarthRadiusInKilometers = 6367.0;
            public static double ToRadian(double val) { return val * (Math.PI / 180); }
            public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }
            /// <summary> 
            /// Calculate the distance between two geocodes. Defaults to using Miles. 
            /// </summary> 
            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
            {
                return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
            }
            /// <summary> 
            /// Calculate the distance between two geocodes. 
            /// </summary> 
            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
            {
                double radius = GeoCodeCalc.EarthRadiusInMiles;
                if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoCodeCalc.EarthRadiusInKilometers; }
                return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
            }
        }
        public enum GeoCodeCalcMeasurement : int
        {
            Miles = 0,
            Kilometers = 1
        }
        #endregion

        #region Map Event
        private void Map_MapResolved(object sender, EventArgs e)
        {
            Debug.WriteLine("Map_MapResolved");
            if (mMap.IsDownloading)
            {
                prog.IsVisible = true;
                SystemTray.SetIsVisible(this, true);
            }
            else
            {
                prog.IsVisible = false;
                SystemTray.SetIsVisible(this, false);               
            }
            if (isolatedStorageSettings.Contains("trackCurrentLocation"))
            {
                Debug.WriteLine("trackCurrentLocation is " + (bool)isolatedStorageSettings["trackCurrentLocation"]);
            }
            else
            {
                Debug.WriteLine("trackCurrentLocation does not exist");
            }
            if (isolatedStorageSettings.Contains("trackCurrentLocation") && (bool)isolatedStorageSettings["trackCurrentLocation"] == false)
            {
                updatePushpinAroundMapcenter();
            }
        }
        #endregion
        void REST_traffic(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;
            RestResponse rr = JsonConvert.DeserializeObject<RestResponse>(e.Result);


            
            Pushpins = new ObservableCollection<PushpinModel>();
            FreewayPushpins = new ObservableCollection<PushpinModel>();
            RoadPushpins = new ObservableCollection<PushpinModel>();

            #region  All Pushpins

            foreach (ResourceSet t in rr.resourceSets)
            {

                IncidentList = t.resources;
                mIncidentList.ItemsSource = IncidentList;
                foreach (TrafficIncident p in t.resources)
                {

                    Pushpins.Add(new PushpinModel(p.point.coordinates.ElementAt(0), p.point.coordinates.ElementAt(1), p.type, p.typeDescription, p.severityColor, p));
                }
            }
            #endregion
            // PushpinLayer.ItemsSource = Pushpins;

            #region Freeways 
            RestResponse freewaySet = JsonConvert.DeserializeObject<RestResponse>(e.Result);
            foreach (ResourceSet frwy in freewaySet.resourceSets)
            {
                FreewayList = frwy.resources;
                for (int i = FreewayList.Count - 1; i >= 0; i--)
                {
                    TrafficIncident freeway = FreewayList.ElementAt(i);
                    if (freeway.description.ToLower().Contains("frwy") || freeway.description.ToLower().Contains("hwy"))
                    {
                        FreewayPushpins.Add(new PushpinModel(freeway.point.coordinates.ElementAt(0), freeway.point.coordinates.ElementAt(1), freeway.type, freeway.typeDescription, freeway.severityColor, freeway));
                    }
                    else
                    {
                        FreewayList.RemoveAt(i);
                    }
                }
                mFreewayList.ItemsSource = FreewayList;
            }
            #endregion


            #region Roads
            RestResponse roadSet = JsonConvert.DeserializeObject<RestResponse>(e.Result);
            foreach (ResourceSet roads in roadSet.resourceSets)
            {
                RoadList = roads.resources;
                for (int i = RoadList.Count - 1; i >= 0; i--)
                {
                    TrafficIncident check = RoadList.ElementAt(i);
                    if (check.description.ToLower().Contains("frwy") || check.description.ToLower().Contains("hwy"))
                    {
                        RoadList.RemoveAt(i);
                    }
                    else
                    {
                        RoadPushpins.Add(new PushpinModel(check.point.coordinates.ElementAt(0), check.point.coordinates.ElementAt(1), check.type, check.typeDescription, check.severityColor, check));
                    }
                }
                mRoadList.ItemsSource = RoadList;
            }
            #endregion

            // show the pins
            //if (MainPivot.SelectedIndex == 0)
            //{
            //    PushpinLayer.ItemsSource = null;
            //    PushpinLayer.ItemsSource = Pushpins;
            //}
            //else if (MainPivot.SelectedIndex == 1)
            //{
            //    PushpinLayer.ItemsSource = null;
            //    PushpinLayer.ItemsSource = FreewayPushpins;
            //}
            //else
            //{
            //    PushpinLayer.ItemsSource = null;
            //    PushpinLayer.ItemsSource = RoadPushpins;
            //}
            PushpinLayer.ItemsSource = Pushpins;
            FreewayPushpinLayer.ItemsSource = FreewayPushpins;
            RoadPushpinLayer.ItemsSource = RoadPushpins;
            showPushpin();
        }

       

        private void Map_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //  this.mMap = new Microsoft.Phone.Controls.Maps.Map();
            MapGrid.Height = 800;
            mMap.Height = 800;
            PushpinLayer.Height = 800;
            this.mMap.ScaleVisibility = System.Windows.Visibility.Visible;

            Debug.WriteLine("Expand expand!!!!");
            event_list.Visibility = Visibility.Collapsed;

            //     map_button.Visibility = Visibility.Collapsed;
            //GridLength titleGrid = new GridLength(0);
            // LayoutRoot.RowDefinitions[1].Height = titleGrid;
            //mMap.UpdateLayout();
            //mMap.InvalidateArrange();
            //mMap.InvalidateMeasure();
            Debug.WriteLine("ASD " + (mMap.Children.Count));
            //((MapLayer)mMap.Children[0]).Height = 800;
            //((MapLayer)mMap.Children[1]).Height = 800;
            //((MapItemsControl)((MapLayer)mMap.Children[0]).Children[0]).Height = 800;
            //((MapItemsControl)((MapLayer)mMap.Children[1]).Children[0]).Height = 800;

        }


        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            this.mMap.ZoomLevel += 1;
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            this.mMap.ZoomLevel -= 1;
        }

        private void IncidentTextBlock_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //var tap = sender as TextBlock;
            //TrafficIncident trafficIncident = tap.DataContext as TrafficIncident;
            //foreach (PushpinModel pm in Pushpins)
            //{
            //    pm.severityColor = pm.trafficIncident.severityColor;
            //    pm.foregroundColor = "Black";
            //    if (pm.trafficIncident.Equals(trafficIncident))
            //    {
            //        pm.severityColor = "Black";
            //        pm.foregroundColor = "White";
            //        PushpinLayer.ItemsSource = null;
            //        PushpinLayer.ItemsSource = Pushpins;
            //    }   
            //}
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            map_expanded = true;

            // center before resize
            double smallMapLatitude = mMap.Center.Latitude;
            double smallMapLongitude = mMap.Center.Longitude;


            MapGrid.Height = MapHeight;
            mMap.Height = MapHeight;
            PushpinLayer.Height = MapHeight;
            this.mMap.ScaleVisibility = System.Windows.Visibility.Visible;

            mMap.Center.Latitude = smallMapLatitude;
            mMap.Center.Longitude = smallMapLongitude;

            Debug.WriteLine("Expand expand!!!!");
            MainPivot.Visibility = Visibility.Collapsed;
            event_list.Visibility = Visibility.Collapsed;
            freeway_list.Visibility = Visibility.Collapsed;
            road_list.Visibility = Visibility.Collapsed;
            map_button.Visibility = Visibility.Collapsed;
            
           
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (map_expanded == true)
            {
                double largeMapLatitude = mMap.Center.Latitude;
                double largeMapLongitude = mMap.Center.Longitude;

                e.Cancel = true;
                map_expanded = false;
                MapGrid.Height = 150;
                mMap.Height = 150;
                PushpinLayer.Height = 150;
                this.mMap.ScaleVisibility = System.Windows.Visibility.Collapsed;
                MainPivot.Visibility = Visibility.Visible;
                event_list.Visibility = Visibility.Visible;
                road_list.Visibility = Visibility.Visible;
                freeway_list.Visibility = Visibility.Visible;
                map_button.Visibility = Visibility.Visible;

                if (isolatedStorageSettings.Contains("trackCurrentLocation"))
                {
                    if ((bool)isolatedStorageSettings["trackCurrentLocation"] == true)
                    {
                        mMap.Center = new GeoCoordinate(this.Current_Location.Location.Latitude, this.Current_Location.Location.Longitude);
                    }
                    else
                    {
                        // return to the center of the large map
                        mMap.Center.Latitude = largeMapLatitude;
                        mMap.Center.Longitude = largeMapLongitude;
                    } 
                }
                else
                {
                    // track current location hasnt been set yet, (first time running app)
                    mMap.Center = new GeoCoordinate(this.Current_Location.Location.Latitude, this.Current_Location.Location.Longitude);
                }

                
                mMap.ZoomLevel = 10;
            }
        }

        private void MainPivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            showPushpin();
        }
        private void showPushpin()
        {
            if (MainPivot.SelectedIndex == 0)
            {
                PushpinLayer.Visibility = Visibility.Visible;
                FreewayPushpinLayer.Visibility = Visibility.Collapsed;
                RoadPushpinLayer.Visibility = Visibility.Collapsed;
            }
            else if (MainPivot.SelectedIndex == 1)
            {
                PushpinLayer.Visibility = Visibility.Collapsed;
                FreewayPushpinLayer.Visibility = Visibility.Visible;
                RoadPushpinLayer.Visibility = Visibility.Collapsed;
            }
            else
            {
                PushpinLayer.Visibility = Visibility.Collapsed;
                FreewayPushpinLayer.Visibility = Visibility.Collapsed;
                RoadPushpinLayer.Visibility = Visibility.Visible;
            }
        }




        public string updateTrafficIncidentUrl(GeoCoordinate e)
        {
            String address = "http://dev.virtualearth.net/REST/v1/Traffic/Incidents/";
            address += (e.Latitude - 0.35) + ",";
            address += (e.Longitude - 0.35) + ",";
            address += (e.Latitude + 0.35) + ",";
            address += (e.Longitude + 0.35);
            
            List<string> type = new List<string>();
            List<string> severity = new List<string>();

            // create the URL the #'s represent the type and severity of data to return
            if (isolatedStorageSettings.Count > 0)
            {
                if (isolatedStorageSettings.Contains("accidents"))
                {

                    if ((bool)isolatedStorageSettings["accidents"] == true)
                    {
                        type.Add("1");
                    }

                }

                if (isolatedStorageSettings.Contains("congestion"))
                {
                    if ((bool)isolatedStorageSettings["congestion"])
                    {
                        type.Add("2");
                    }

                }

                if (isolatedStorageSettings.Contains("disabledVehicles"))
                {
                    if ((bool)isolatedStorageSettings["disabledVehicles"])
                    {
                        type.Add("3");
                    }

                }

                if (isolatedStorageSettings.Contains("massTransit"))
                {
                    if ((bool)isolatedStorageSettings["massTransit"])
                    {
                        type.Add("4");
                    }
                }

                if (isolatedStorageSettings.Contains("miscellaneous"))
                {
                    if ((bool)isolatedStorageSettings["miscellaneous"])
                    {
                        type.Add("5");
                    }
                }

                if (isolatedStorageSettings.Contains("otherNews"))
                {
                    if ((bool)isolatedStorageSettings["otherNews"])
                    {
                        type.Add("6");
                    }
                }

                if (isolatedStorageSettings.Contains("plannedEvents"))
                {
                    if ((bool)isolatedStorageSettings["plannedEvents"])
                    {
                        type.Add("7");
                    }
                }
                if (isolatedStorageSettings.Contains("roadHazard"))
                {
                    if ((bool)isolatedStorageSettings["roadHazard"])
                    {
                        type.Add("8");
                    }
                }

                if (isolatedStorageSettings.Contains("construction"))
                {
                    if ((bool)isolatedStorageSettings["construction"])
                    {
                        type.Add("9");
                    }
                }

                if (isolatedStorageSettings.Contains("alert"))
                {
                    if ((bool)isolatedStorageSettings["alert"])
                    {
                        type.Add("10");
                    }
                }

                if (isolatedStorageSettings.Contains("weather"))
                {
                    if ((bool)isolatedStorageSettings["weather"])
                    {
                        type.Add("11");
                    }
                }

                if (isolatedStorageSettings.Contains("lowImpact"))
                {
                    if ((bool)isolatedStorageSettings["lowImpact"])
                    {
                        severity.Add("1");
                    }
                }

                if (isolatedStorageSettings.Contains("minorImpact"))
                {
                    if ((bool)isolatedStorageSettings["minorImpact"])
                    {
                        severity.Add("2");
                    }
                }

                if (isolatedStorageSettings.Contains("moderateImpact"))
                {
                    if ((bool)isolatedStorageSettings["moderateImpact"])
                    {
                        severity.Add("3");
                    }
                }

                if (isolatedStorageSettings.Contains("seriousImpact"))
                {
                    if ((bool)isolatedStorageSettings["seriousImpact"])
                    {
                        severity.Add("4");
                    }
                }
            }
            else
            {
                // if isolatedstorage is empty/running app for first time use default search
                address = "http://dev.virtualearth.net/REST/v1/Traffic/Incidents/";
                address += (e.Latitude - 0.5) + ",";
                address += (e.Longitude - 0.5) + ",";
                address += (e.Latitude + 0.5) + ",";
                address += (e.Longitude + 0.5);
                address += "/false?t=1,2,3,4,5,6,7,8,9,10,11&s=1,2,3,4&key=" + Id;
                return address;
            }

            // use th users settings...
            if (type.Count == 0)
            {
                // user didnt pick any types, do not show traffic pushpins
                return "";
            }

            if (severity.Count == 0)
            {
                // user didnt pick severity, do not display traffic pushpins
                return "";
            }


            // use the user settings
            // type
            address += "/false?t=";
            
            for(int i=0 ; i < type.Count; i++){
                address += type.ElementAt(i);
                if( i < type.Count-1){
                    address += ",";
                }
            }
            // severitry
            address += "&s=";
            for(int j=0; j < severity.Count;j++){
                address += severity.ElementAt(j);
                if( j < severity.Count -1){
                    address += ",";
                }
            }
            address += "&key=" + Id; // false because do not include location codes
            return address;
        }




        private void mMap_MapPan(object sender, MapDragEventArgs e)
        {
            Debug.WriteLine("Map_MapPan");
            if (mMap.IsDownloading)
            {
                prog.IsVisible = true;
                SystemTray.SetIsVisible(this, true);
            }
            else
            {
                prog.IsVisible = false;
                SystemTray.SetIsVisible(this, false);

            }
        }

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Pushpin p = (Pushpin)sender;
            PushpinModel pm = (PushpinModel)p.DataContext;          

            if (pm.pushpinContentTypeDescription == true)
            {
                p.Content = pm.typeImage;
                pm.pushpinContentTypeDescription = false;
            }
            else
            {
                p.Content = pm.typeDescription;
                pm.pushpinContentTypeDescription = true;
            }

        }

        private void Pushpin_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Pushpin p = (Pushpin)sender;
            PushpinModel pm = (PushpinModel)p.DataContext;
            MessageBox.Show(pm.trafficIncident.severityDescription + " " + pm.trafficIncident.typeDescription.ToLower() + ", " + pm.trafficIncident.description);

        }


        private void mIncidentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TrafficIncident trafficIncident = (sender as ListBox).SelectedItem as TrafficIncident;
            selectedIncident = (sender as ListBox).SelectedItem as TrafficIncident;
            //TrafficIncident trafficIncident = tap.DataContext as TrafficIncident;
            //ObservableCollection<PushpinModel> tempPushpins=Pushpins;
            //foreach (PushpinModel pm in Pushpins)
            //{
            //    pm.severityColor = pm.trafficIncident.severityColor;
            //    pm.foregroundColor = "Black";
            //    if (pm.trafficIncident.Equals(trafficIncident))
            //    {
            //        pm.severityColor = "Black";
            //        pm.foregroundColor = "White";
            //        //PushpinLayer.ItemsSource = null;
            //        //PushpinLayer.ItemsSource = Pushpins;
            //    }
            //}
            //tempPushpins = Pushpins;
            //PushpinLayer.ItemsSource = tempPushpins;
            foreach (PushpinModel pm in Pushpins)
            {
                pm.severityColor = pm.trafficIncident.severityColor;
                pm.foregroundColor = "Black";
                if (pm.trafficIncident.Equals(trafficIncident))
                {
                    pm.severityColor = "Black";
                    pm.foregroundColor = "White";
                }
            }
            //PushpinLayer.ItemsSource = Pushpins;
        }

        private void mFreewayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TrafficIncident trafficIncident = (sender as ListBox).SelectedItem as TrafficIncident;
            selectedIncident = (sender as ListBox).SelectedItem as TrafficIncident;
            //TrafficIncident trafficIncident = tap.DataContext as TrafficIncident;
            //foreach (PushpinModel pm in FreewayPushpins)
            //{
            //    pm.severityColor = pm.trafficIncident.severityColor;
            //    pm.foregroundColor = "Black";
            //    if (pm.trafficIncident.Equals(trafficIncident))
            //    {
            //        pm.severityColor = "Black";
            //        pm.foregroundColor = "White";
            //        FreewayPushpinLayer.ItemsSource = null;
            //        FreewayPushpinLayer.ItemsSource = FreewayPushpins;
            //    }
            //}
            foreach (PushpinModel pm in FreewayPushpins)
            {
                pm.severityColor = pm.trafficIncident.severityColor;
                pm.foregroundColor = "Black";
                if (pm.trafficIncident.Equals(trafficIncident))
                {
                    pm.severityColor = "Black";
                    pm.foregroundColor = "White";
                }
            }
        }




        private void mRoadList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TrafficIncident trafficIncident = (sender as ListBox).SelectedItem as TrafficIncident;
            selectedIncident = (sender as ListBox).SelectedItem as TrafficIncident;
            //TrafficIncident trafficIncident = tap.DataContext as TrafficIncident;
            //foreach (PushpinModel pm in RoadPushpins)
            //{
            //    pm.severityColor = pm.trafficIncident.severityColor;
            //    pm.foregroundColor = "Black";
            //    if (pm.trafficIncident.Equals(trafficIncident))
            //    {
            //        pm.severityColor = "Black";
            //        pm.foregroundColor = "White";
            //        RoadPushpinLayer.ItemsSource = null;
            //        RoadPushpinLayer.ItemsSource = RoadPushpins;
            //    }
            //}
            //mMap.Center.p
            foreach (PushpinModel pm in RoadPushpins)
            {
                pm.severityColor = pm.trafficIncident.severityColor;
                pm.foregroundColor = "Black";
                if (pm.trafficIncident.Equals(trafficIncident))
                {
                    pm.severityColor = "Black";
                    pm.foregroundColor = "White";
                }
            }
        }

        private void mMap_MapPan_1(object sender, MapDragEventArgs e)
        {
            
        }


        private void updatePushpinAroundMapcenter()
        {
            Debug.WriteLine("Update the pushpin");
            TRAFFIC_ADDRESS = updateTrafficIncidentUrl(mMap.Center); // method creates traffic url

            // if traffic url is not empty
            if (TRAFFIC_ADDRESS.Count() > 1)
            {
                if (!wc.IsBusy)
                {
                    wc.DownloadStringAsync(new Uri(TRAFFIC_ADDRESS));
                }
            }
            if (Pushpins != null && selectedIncident != null)
            {
                ObservableCollection<PushpinModel> tempPuspins = Pushpins;
                PivotItem currentPivotItem = (PivotItem)MainPivot.SelectedItem;
                string currentHeader = (string)currentPivotItem.Header;
                if (currentHeader == "all") tempPuspins = Pushpins;
                else if (currentHeader == "highways") tempPuspins = FreewayPushpins;
                else if (currentHeader == "roads") tempPuspins = RoadPushpins;
                foreach (PushpinModel pm in tempPuspins)
                {
                    pm.severityColor = pm.trafficIncident.severityColor;
                    pm.foregroundColor = "Black";
                    if (pm.trafficIncident.Equals(selectedIncident))
                    {
                        pm.severityColor = "Black";
                        pm.foregroundColor = "White";
                        PushpinLayer.ItemsSource = null;
                        PushpinLayer.ItemsSource = Pushpins;
                        break;
                    }
                }
            }
        }

    }
}