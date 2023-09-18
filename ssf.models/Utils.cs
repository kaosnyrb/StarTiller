using ssf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ssf
{
    public class Utils
    {
        //An AI wrote these. Future.
        public static Vector3 ConvertStringToVector3(string input)
        {
            if (input == null) return new Vector3(0, 0, 0);

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

        public static bool DoBoundingBoxesIntersect(Vector3 box1Min, Vector3 box1Max, Vector3 box2Min, Vector3 box2Max)
        {
            // Check for intersection along each axis (x, y, z)
            if (box1Max.X < box2Min.X || box1Min.X > box2Max.X)
                return false;
            if (box1Max.Y < box2Min.Y || box1Min.Y > box2Max.Y)
                return false;
            if (box1Max.Z < box2Min.Z || box1Min.Z > box2Max.Z)
                return false;

            return true;
        }

        public static Block TranslateBlock(Block block, Vector3 Pos, float Rotation)
        {
            //SSFEventLog.EventLogs.Enqueue("Translating " + block.path + " by " + Pos + " and " + Rotation);
            Vector3 Pivot = block.blockDetails.startpoint;

            for (int i = 0; i < block.placedObjects.Count; i++)
            {
                block.placedObjects[i].Placement.translate(Pivot, Pos, Rotation);
            }
            for (int i = 0; i < block.blockDetails.Connectors.Count; i++)
            {
                block.blockDetails.Connectors[i].startpoint = Utils.RotateVectorAroundPivot(Pivot, block.blockDetails.Connectors[i].startpoint, Rotation);
                block.blockDetails.Connectors[i].startpoint += Pos;
                block.blockDetails.Connectors[i].rotation += Rotation;
            }
            for (int i = 0; i < block.navmeshs.Count; i++)
            {
                block.navmeshs[i].translate(Pivot, Pos, Rotation);
            }
            return block;
        }

        public static Vector3 ToEulerAngles(Quaternion q)
        {
            Vector3 angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public static Vector3 LocalToGlobal(Vector3 localAngles, Vector3 globalAngles)
        {
            // Convert the local angles to radians
            float localYaw = localAngles.X;
            float localPitch = localAngles.Y;
            float localRoll = localAngles.Z;

            // Convert the global angles to radians
            float globalYaw = globalAngles.X;
            float globalPitch = globalAngles.Y;
            float globalRoll = globalAngles.Z;

            // Create quaternions for the local and global rotations
            Quaternion localRotation = Quaternion.CreateFromYawPitchRoll(localYaw, localPitch, localRoll);
            Quaternion globalRotation = Quaternion.CreateFromYawPitchRoll(globalYaw, globalPitch, globalRoll);

            // Rotate the local rotation by the global rotation
            Quaternion resultRotation = globalRotation * localRotation;

            // Convert the result to Euler angles in degrees
            Vector3 resultAngles = ToEulerAngles(resultRotation);

            return resultAngles;
        }

        public static float ToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180));
        }
        public static float ToDegrees(float rads)
        {
            return (float)(rads * (180 / Math.PI));
        }
    }
}
