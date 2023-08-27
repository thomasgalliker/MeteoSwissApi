using System;
using System.Globalization;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    /// <summary>
    /// Abstraction of a geolocation.
    /// </summary>
    /// <remarks>
    /// https://github.com/microsoft/Bing-Maps-Fleet-Tracker/blob/master/Backend/src/System.Device/Location/GeoCoordinate.cs
    /// </remarks>
    public class GeoCoordinate : IEquatable<GeoCoordinate>
    {
        private double latitude = double.NaN;
        private double longitude = double.NaN;
        private double verticalAccuracy = double.NaN;
        private double horizontalAccuracy = double.NaN;
        private Speed? speed;
        private double course = double.NaN;

        public static readonly GeoCoordinate Unknown = new GeoCoordinate();

        public GeoCoordinate()
        {
        }

        public GeoCoordinate(double latitude, double longitude)
            : this(latitude, longitude, altitude: null)
        {
        }

        public GeoCoordinate(double latitude, double longitude, Length? altitude)
            : this(latitude, longitude, altitude, double.NaN, double.NaN, null, double.NaN)
        {
        }

        public GeoCoordinate(double latitude, double longitude, Length? altitude, double horizontalAccuracy, double verticalAccuracy, Speed? speed, double course)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;

            this.Altitude = altitude;

            this.HorizontalAccuracy = horizontalAccuracy;
            this.VerticalAccuracy = verticalAccuracy;

            this.Speed = speed;
            this.Course = course;
        }

        public double Latitude
        {
            get
            {
                return this.latitude;
            }
            set
            {
                if (value is > 90.0 or < (-90.0))
                {
                    throw new ArgumentOutOfRangeException("Latitude", "The value of the parameter must be from -90.0 to 90.0.");
                }
                this.latitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return this.longitude;
            }
            set
            {
                if (value is > 180.0 or < (-180.0))
                {
                    throw new ArgumentOutOfRangeException("Longitude", "The value of the parameter must be from -180.0 to 180.0.");
                }
                this.longitude = value;
            }
        }

        public Length? Altitude { get; set; }

        public double HorizontalAccuracy
        {
            get
            {
                return this.horizontalAccuracy;
            }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("HorizontalAccuracy", "The value of the parameter must be greater than or equal to zero.");
                }
                this.horizontalAccuracy = value == 0.0 ? double.NaN : value;
            }
        }

        public double VerticalAccuracy
        {
            get
            {
                return this.verticalAccuracy;
            }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("VerticalAccuracy", "The value of the parameter must be greater than or equal to zero.");
                }
                this.verticalAccuracy = value == 0.0 ? double.NaN : value;
            }
        }

        public Speed? Speed
        {
            get
            {
                return this.speed;
            }
            set
            {
                if (value < UnitsNet.Speed.Zero)
                {
                    throw new ArgumentOutOfRangeException("speed", "The value of the parameter must be greater than or equal to zero.");
                }
                this.speed = value;
            }
        }

        public double Course
        {
            get
            {
                return this.course;
            }
            set
            {
                if (value is < 0.0 or > 360.0)
                {
                    throw new ArgumentOutOfRangeException("course", "The value of the parameter must be from 0.0 to 360.0.");
                }
                this.course = value;
            }
        }

        public bool IsUnknown => this.Equals(Unknown);

        public Length GetDistanceTo(GeoCoordinate other)
        {
            //  The Haversine formula according to Dr. Math.
            //  http://mathforum.org/library/drmath/view/51879.html

            //  dlon = lon2 - lon1
            //  dlat = lat2 - lat1
            //  a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
            //  c = 2 * atan2(sqrt(a), sqrt(1-a)) 
            //  d = R * c

            //  Where
            //    * dlon is the change in longitude
            //    * dlat is the change in latitude
            //    * c is the great circle distance in Radians.
            //    * R is the radius of a spherical Earth.
            //    * The locations of the two points in 
            //        spherical coordinates (longitude and 
            //        latitude) are lon1,lat1 and lon2, lat2.

            if (double.IsNaN(this.Latitude) || double.IsNaN(this.Longitude) ||
                double.IsNaN(other.Latitude) || double.IsNaN(other.Longitude))
            {
                throw new ArgumentException("The coordinate's latitude or longitude is not a number.");
            }

            var dLat1 = this.Latitude * (Math.PI / 180.0);
            var dLon1 = this.Longitude * (Math.PI / 180.0);
            var dLat2 = other.Latitude * (Math.PI / 180.0);
            var dLon2 = other.Longitude * (Math.PI / 180.0);

            var dLon = dLon2 - dLon1;
            var dLat = dLat2 - dLat1;

            // Intermediate result a.
            var a = Math.Pow(Math.Sin(dLat / 2.0), 2.0) +
                       (Math.Cos(dLat1) * Math.Cos(dLat2) *
                       Math.Pow(Math.Sin(dLon / 2.0), 2.0));

            // Intermediate result c (great circle distance in Radians).
            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            // Distance.
            const double kEarthRadiusMs = 6376500;
            var dDistance = kEarthRadiusMs * c;
            return Length.FromMeters(dDistance).ToUnit(UnitsNet.Units.LengthUnit.Kilometer);
        }

        public override int GetHashCode()
        {
            return this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is not GeoCoordinate)
            {
                return base.Equals(obj);
            }

            return this.Equals(obj as GeoCoordinate);
        }
        public override string ToString()
        {
            if (this == Unknown)
            {
                return "Unknown";
            }
            else
            {
                return this.Latitude.ToString("G", CultureInfo.InvariantCulture) + ", " +
                       this.Longitude.ToString("G", CultureInfo.InvariantCulture);
            }
        }

        public bool Equals(GeoCoordinate other)
        {
            if (other is null)
            {
                return false;
            }
            return this.Latitude.Equals(other.Latitude) && this.Longitude.Equals(other.Longitude);
        }

        public static bool operator ==(GeoCoordinate left, GeoCoordinate right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(GeoCoordinate left, GeoCoordinate right)
        {
            return !(left == right);
        }
    }
}