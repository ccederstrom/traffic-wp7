using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Device.Location;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace Traffic.Model
{
    public class PushpinModel : INotifyPropertyChanged
    {
        public PushpinModel(double x, double y, int type, string typeDescription, string severityColor, TrafficIncident trafficIncident)
        {
            Location = new GeoCoordinate(x, y);
            this.type = type;
            this.typeDescription = typeDescription;
            this.severityColor = severityColor;
            pushpinContentTypeDescription = false;
            this.trafficIncident = trafficIncident;
            foregroundColor = "Black";
            // using object to get image... other constructors are useless ... 
            this.typeImage = trafficIncident.typeImage;

        }

        private string _foregroundColor;
        public string foregroundColor {
            get
            {
                return _foregroundColor;
            }
            set
            {
                if (value != _foregroundColor)
                {
                    _foregroundColor = value;
                    NotifyPropertyChanged("foregroundColor");
                }
            }
        }

        private bool _pushpinContentTypeDescription;
        public bool pushpinContentTypeDescription
        {
            get
            {
                return _pushpinContentTypeDescription;
            }
            set
            {
                if (value != _pushpinContentTypeDescription)
                {
                    _pushpinContentTypeDescription = value;
                    NotifyPropertyChanged("pushpinContentTypeDescription");
                }
            }
        }

        private GeoCoordinate _Location;
        public GeoCoordinate Location
        {
            get
            {
                return _Location;
            }
            set
            {
                if (value != _Location)
                {
                    _Location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        private int _type;
        public int type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    NotifyPropertyChanged("type");
                }
            }
        }

        private string _severityColor;
        public string severityColor
        {
            get
            {
                return _severityColor;
            }
            set
            {
                if (value != _severityColor)
                {
                    _severityColor = value;
                    NotifyPropertyChanged("severityColor");
                }
            }
        }

        private string _typeDescription;
        public string typeDescription
        {
            get
            {
                return _typeDescription;
            }
            set
            {
                if (value != _typeDescription)
                {
                    _typeDescription = value;
                    NotifyPropertyChanged("typeDescription");
                }
            }
        }

        private Image _typeImage;
        public Image typeImage
        {
            get
            {
                return _typeImage;
            }
            set
            {
                if (value != _typeImage)
                {
                    _typeImage = value;
                    NotifyPropertyChanged("typeImage");
                }
            }
        }

        private TrafficIncident _trafficIncident;
        public TrafficIncident trafficIncident
        {
            get
            {
                return _trafficIncident;
            }
            set
            {
                if (value != _trafficIncident)
                {
                    _trafficIncident = value;
                    NotifyPropertyChanged("trafficIncident");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
