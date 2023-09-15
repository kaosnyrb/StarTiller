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

        public static Vector3 RotateVectorAroundPivot(Vector3 pivot, Vector3 p, double angle)
        {
            angle = angle * (Math.PI / 180);

            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            // translate point back to origin:
            p.X -= pivot.X;
            p.Y -= pivot.Y;

            // rotate point
            double xnew = p.X * c - p.Y * s;
            double ynew = p.X * s + p.Y * c;

            // translate point back:
            p.X = (float)(xnew + pivot.X);
            p.Y = (float)(ynew + pivot.Y);

            return p;
        }
    }
}
