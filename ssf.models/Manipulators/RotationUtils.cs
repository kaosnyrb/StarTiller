using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Manipulators
{
    internal class RotationUtils
    {
        //This is based off a papyrus function
        //credit of dafydd99 - https://forums.nexusmods.com/index.php?/topic/9011983-local-to-global-rotations-that-old-chestnut/page-2

        public static Vector3 rotateAroundZ(Vector3 Rotation, float angle)
        {
            //float[] normalisedAngles = getLocalAngles(originalXAngle, originalYAngle, -originalZAngle)
            Vector3 normalisedAngles = getLocalAngles(new Vector3(Rotation.X, Rotation.Y, -Rotation.Z));
            //float[] angles = getLocalAngles(normalisedAngles[0], normalisedAngles[1], -normalisedAngles[2] + angleZRotate)
            Vector3 angles = getLocalAngles(new Vector3(normalisedAngles.X, normalisedAngles.Y, -normalisedAngles.Z + angle));
            return angles;
        }

        public static Vector3 getLocalAngles(Vector3 localAng)
        {
            Vector3 worldAng;

            localAng.X = -localAng.X;
            localAng.Y = -localAng.Y;
            localAng.Z = -localAng.Z;

            //sine and cosine of local x, y, z angles

            float sx = (float)Math.Sin(localAng.X);
            float cx = (float)Math.Cos(localAng.X);
            float sy = (float)Math.Sin(localAng.Y);
            float cy = (float)Math.Cos(localAng.Y);
            float sz = (float)Math.Sin(localAng.Z);
            float cz = (float)Math.Cos(localAng.Z);

            // ZYX
            float r11 = cz * cy;
            float r12 = -sz * cx + cz * sy * sx;
            float r13 = sz * sx + cz * sy * cx;
            float r21 = sz * cy;
            float r22 = cz * cx + sz * sy * sx;
            float r23 = cz * -sx + sz * sy * cx;
            float r31 = -sy;
            float r32 = cy * sx;
            float r33 = cy * cx;

            // Extraction of worldangles from rotation matrix

            if (r13 > 0.9998)
            {
                //positive gimbal lock
                worldAng.X = (float)-Math.Atan2(r32, r22);
                worldAng.Y = -1.5708f;
                worldAng.Z = 0;

            }
            else if (r13 < -0.9998)
            {
                //negative gimbal lock
                worldAng.X = (float)-Math.Atan2(r32, r22);
                worldAng.Y = 1.5708f;
                worldAng.Z = 0;
            }
            else
            {
                //no gimbal lock
                r23 = -r23;
                r12 = -r12;
                worldAng.X = (float)-Math.Atan2(r23, r33);
                worldAng.Y = (float)-Math.Asin(r13);
                worldAng.Z = (float)-Math.Atan2(r12, r11);
            }

            return worldAng;
        }
    }
}
