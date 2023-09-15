using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ssf
{
    public class Utils
    {
        //An AI wrote these. Future.
        public static Vector3 ConvertStringToVector3(string input)
        {
            // Split the input string by commas.
            string[] components = input.Split(',');

            // Check if there are exactly three components (x, y, and z).
            if (components.Length != 3)
            {
                return Vector3.Zero; // Return a default Vector3 if the input is invalid.
            }

            // Parse the components as floats.
            float x, y, z;
            if (float.TryParse(components[0], out x) &&
                float.TryParse(components[1], out y) &&
                float.TryParse(components[2], out z))
            {
                // Create and return the Vector3.
                return new Vector3(x, y, z);
            }
            else
            {
                return Vector3.Zero; // Return a default Vector3 if parsing fails.
            }
        }
        public static string ConvertVector3ToString(Vector3 vector)
        {
            // Use string interpolation to format the Vector3 as a string.
            string result = $"{vector.X}, {vector.Y}, {vector.Z}";
            return result;
        }
    }
}
