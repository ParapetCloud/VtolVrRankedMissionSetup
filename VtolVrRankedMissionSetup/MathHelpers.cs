using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VTS;

namespace VtolVrRankedMissionSetup
{
    public static class MathHelpers
    {
        public static void ClampRotation(ref Vector3 rotation)
        {
            while (rotation.X < -360)
                rotation.X += 360;
            while (rotation.X > 360)
                rotation.X -= 360;

            while (rotation.Y < -360)
                rotation.Y += 360;
            while (rotation.Y > 360)
                rotation.Y -= 360;

            while (rotation.Z < -360)
                rotation.Z += 360;
            while (rotation.Z > 360)
                rotation.Z -= 360;
        }

        public static Vector3 BaseToWorld(Vector3 offset, BaseInfo baseInfo)
        {
            Matrix4x4 baseRotation = Matrix4x4.CreateFromYawPitchRoll(DegToRad(baseInfo.Prefab.Rotation.Y), DegToRad(baseInfo.Prefab.Rotation.X), DegToRad(baseInfo.Prefab.Rotation.Z));
            return baseInfo.Prefab.GlobalPos + Vector3.Transform(offset, baseRotation);
        }

        public static float DegToRad(float degrees) => (float)(degrees * (Math.PI / 180.0));
    }
}
