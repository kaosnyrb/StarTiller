
using Microsoft.Xna.Framework;
using System.Drawing;

namespace KNPE
{

    class Camera
    {
        public static Vector3 CameraForward = new Vector3(0, 0, 0);
        public static Vector3 CameraRight = new Vector3(0, 0, 0);
        public static Vector3 CameraPosition = new Vector3(0, 0, 0);
        public static Vector3 CameraTarget = new Vector3(0, 0, 1);

        public static Matrix ViewMatrix;
        public static BoundingFrustum Frustum;

        public static Vector3 CameraUp = Vector3.Up;

        public static float rot = 0.0f;

        public static Matrix GetViewMatrix()
        {
            CreateLookAt(CameraPosition, CameraTarget);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    (float)1280 / (float)720,
                                                                    1,
                                                                    175000);
            ViewMatrix = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
            Frustum = new BoundingFrustum(ViewMatrix * projection);

            return ViewMatrix;
        }

        public static void CreateLookAt(Vector3 Position, Vector3 Target)
        {
            CameraPosition = Position;
            CameraTarget = Target;
            CameraForward = CameraPosition - CameraTarget;
            CameraForward.Normalize();

            CameraRight = Vector3.Cross(CameraForward, CameraUp);
            CameraRight.Normalize();

            ViewMatrix = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
        }
    }
}