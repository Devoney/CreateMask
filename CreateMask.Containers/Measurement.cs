using System;

namespace CreateMask.Containers
{
    [Serializable]
    public class Measurement
    {
        public byte MaskIntensity { get; set; }
        public int Resistance { get; set; }

        public Measurement()
        {
            
        }

        public Measurement(byte maskIntensity, int resistance)
        {
            MaskIntensity = maskIntensity;
            Resistance = resistance;
        }
    }
}
