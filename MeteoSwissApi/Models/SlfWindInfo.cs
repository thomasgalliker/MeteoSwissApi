using System.Diagnostics;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("Mean: {this.VelocityMean}, Max: {this.VelocityMax}, Direction: {this.Direction}")]
    public class SlfWindInfo
    {
        public Speed VelocityMax { get; set; }

        public Speed VelocityMean { get; set; }

        public Angle Direction { get; set; }
    }
}
