
using Microsoft.Xna.Framework;
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

        public static Matrix GetViewMatrix()
        {           
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    (float)1280 / (float)720,
                                                                    1,
                                                                    175000);
            Frustum = new BoundingFrustum(ViewMatrix * projection);
            ViewMatrix = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
        

            return ViewMatrix;
        }

        public static void CreateLookAt(Vector3 Position, Vector3 Target)
        {
            CameraPosition = Position;
            CameraTarget = Target;
            CameraForward = CameraTarget - CameraPosition;
            CameraForward.Normalize();
            CameraRight = Vector3.Cross(CameraForward, CameraUp);
            CameraRight.Normalize();
            ViewMatrix = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
        }
    }
}