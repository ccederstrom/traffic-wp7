using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Traffic.Model
{
    [DataContract]
    public class TrafficIncident
    {
        // http://msdn.microsoft.com/en-us/library/hh441730.aspx


        [DataMember]
        public string __type { get; set; }

        /// <summary>
        /// Required - The latitude and longitude coordinates where you encounter the incident.
        /// </summary>
        [DataMember]
        public Point point{get;set;}


        [DataMember]
        public Point coordinates { get; set; }

        /// <summary>
        /// Optional - A description of the congestion. 
        /// Examples: generally slow,  sluggish
        /// </summary>
        [DataMember]
        public string congestion { get; set; }

        /// <summary>
        /// Optional - A description of the incident. 
        /// Examples: 
        ///     W 95th St between Switzer Rd and Bluejacket Dr - construction
        ///     WB Johnson Dr at I-435 - bridge repair
        /// </summary>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        /// Optional - A description of a detour.
        /// Examples:
        ///     Take 63rd St to Roe Ave and head south to 67th St
        ///     take US-40 to Blue Ridge Cut-Off
        /// </summary>
        [DataMember]
        public string detour { get; set; }

        /// <summary>
        /// TODO DOUBLE CHECK TYPE
        /// Required. The time the incident occurred. For more information about the format, see the About Time Values section below. 
        /// Examples:
        ///     JSON: Date(1295704800000)
        ///     XML: 2011-06-29T23:44:56.593Z
        /// 
        /// Time values in the TrafficIncident resource data use UTC time. The format for XML and JSON responses are different. 
        /// For JSON responses, the time is specified as UTC time in seconds and uses the following format.
        ///     Date(seconds)
        /// </summary>
        [DataMember]
        public DateTime start { get; set; }

        /// <summary>
        /// TODO DOUBLE CHECK TYPE
        /// Required. The time that the traffic incident will end. For more information about the format, see the About Time Values section below. 
        /// Examples:
        ///     JSON: Date(1295704800000)
        ///     XML: 2011-06-29T23:44:56.593Z
        /// </summary>
        [DataMember]
        public DateTime end { get; set; }

        /// <summary>
        /// Required. A unique ID for the incident.
        /// </summary>
        [DataMember]
        public long incidentId { get; set; }

        /// <summary>
        /// A description specific to lanes, such as lane closures. 
        /// Examples:
        ///     All lanes blocked
        ///     Left lane blocked
        /// </summary>
        [DataMember]
        public string lane { get; set; }

        public string laneWithDash {
            get
            {
                string des = "";
                if(!lane.Equals("")) des = " - " + lane;
                return des;
            }
        }
        /// <summary>
        /// Required. The time the incident information was last updated. For more information about the format, see the About Time Values section below. 
        /// Examples:
        ///     JSON: Date(1295704800000)
        ///     XML: 2011-06-29T23:44:56.593Z
        /// </summary>
        [DataMember]
        public DateTime lastModified { get; set; }

        /// <summary>
        /// Required. A value of true indicates that there is a road closure.
        /// </summary>
        [DataMember]
        public bool roadClosed { get; set; }

        /// <summary>
        /// Required. Specifies the level of importance of incident.
        ///     1: LowImpact
        ///     2: Minor
        ///     3: Moderate
        ///     4: Serious
        /// </summary>
        [DataMember]
        public int severity { get; set; }

        public string severityDescription
        {
            get
            {
                string des = "";
                if (severity == 1)
                {
                    des = "Low Impact";
                }
                else if (severity == 2)
                {
                    des = "Minor";
                }
                else if (severity == 3)
                {
                    des = "Moderate";
                }
                else if (severity == 4)
                {
                    des = "Serious";
                }
                return des;
            }
        }
        public string severityColor
        {
            get
            {
                string des = "";
                if (severity == 1)
                {
                    des = "#4066af";
                }
                else if (severity == 2)
                {
                    des = "#9dbe56";
                }
                else if (severity == 3)
                {
                    des = "#ffea36";
                }
                else if (severity == 4)
                {
                    des = "#f01800";
                }
                return des;
            }
        }
        /// <summary>
        /// The coordinates of the end of a traffic incident, such as the end of a construction zone.
        /// </summary>
        [DataMember]
        public Point toPoint { get; set; }

        /// <summary>
        /// A collection of traffic location codes. This field is provided when you set the includeLocationCodes parameter to true in the request. These codes associate an incident with pre-defined road segments. A subscription is typically required to be able to interpret these codes for a geographical area or country.
        /// </summary>
        [DataMember]
        public List<string> locationCodes { get; set; }

        /// <summary>
        /// Required. Specifies the type of incident. 
        ///     1: Accident
        ///     2: Congestion
        ///     3: DisabledVehicle
        ///     4: MassTransit
        ///     5: Miscellaneous
        ///     6: OtherNews
        ///     7: PlannedEvent
        ///     8: RoadHazard
        ///     9: Construction
        ///     10: Alert
        ///     11: Weather
        /// </summary>
        [DataMember]
        public int type { get; set; }

        public string typeDescription
        {
            get
            {
                string des = "";
                if (type == 1)
                {
                    des = "Accident";
                }
                else if (type == 2)
                {
                    des = "Congestion";
                }
                else if (type == 3)
                {
                    des = "Disabled Vehicle";
                }
                else if (type == 4)
                {
                    des = "Mass Transit";
                }
                else if (type == 5)
                {
                    des = "Miscellaneous";
                }
                else if (type == 6)
                {
                    des = "Other News";
                }
                else if (type == 7)
                {
                    des = "Planned Event";
                }
                else if (type == 8)
                {
                    des = "Road Hazard";
                }
                else if (type == 9)
                {
                    des = "Construction";
                }
                else if (type == 10)
                {
                    des = "Alert";
                }
                else if (type == 11)
                {
                    des = "Weather";
                }

                return des;
            }
        }


        public Image typeImage
        {
            get
            {
                BitmapImage bitmapImage = null;
                Image image = new Image();
                Uri uri = null;
                if (type == 1)
                {
                    //Accident
                    uri = new Uri("/Resources/Images/White Border icons/accident.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 2)
                {
                    // Congestion
                    uri = new Uri("/Resources/Images/White Border icons/appbar.sleep.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 3)
                {
                    // Disabled vehicle
                    uri = new Uri("/Resources/Images/White Border icons/disabled vehicle2.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 4)
                {
                    // Mass transit
                    uri = new Uri("/Resources/Images/White Border icons/appbar.train.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 5)
                {
                    // Miscellaneous
                    uri = new Uri("/Resources/Images/White Border icons/appbar.exclamation.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 6)
                {
                    // Other news
                    uri = new Uri("/Resources/Images/White Border icons/appbar.information.circle.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 7)
                {
                    // Planned events
                    uri = new Uri("/Resources/Images/White Border icons/appbar.timer.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 8)
                {
                    // Road Hazard
                    uri = new Uri("/Resources/Images/White Border icons/appbar.cone.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 9)
                {
                    // Construction
                    uri = new Uri("/Resources/Images/White Border icons/construction.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 10)
                {
                    // Alert
                    uri = new Uri("/Resources/Images/White Border icons/appbar.exclamation.png", UriKind.RelativeOrAbsolute);
                }
                else if (type == 11)
                {
                    // Weather
                    uri = new Uri("/Resources/Images/White Border icons/appbar.weather.thunder.png", UriKind.RelativeOrAbsolute);
                }

                bitmapImage = new BitmapImage(uri);
                image.CacheMode = new BitmapCache(); 
                image.Source = bitmapImage;
                image.Width = 21; // 21 is max width without altering the width of the pushpin
                image.Height = 24; // 24 is max height without altering the height of the pushpin
                return image;
            } 
        }



        /// <summary>
        /// Required. A value of true indicates that the incident has been visually verified or otherwise officially confirmed by a source like the local police department.
        /// </summary>
        [DataMember]
        public bool verified { get; set; }
    }
    public class Point
    {
        [DataMember]
        public String type { get; set; }

        [DataMember]
        public List<Double> coordinates { get; set; }
    }
}
