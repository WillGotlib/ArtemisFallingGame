using protoBuff;
using UnityEngine;

namespace Online
{
    internal class Helpers
    {
        
        public static Position ToPosition(Vector3 position)
        {
            return new Position
            {
                X = position.x,
                Y = position.y,
                Z = position.z
            };
        }

        public static Rotation ToRotation(Quaternion rotation)
        {
            return new Rotation
            {
                X = rotation.x,
                Y = rotation.y,
                Z = rotation.z,
                W = rotation.w
            };
        }

        public static Vector3 ToVector3(Position position)
        {
            return new Vector3
            {
                x = position.X,
                y = position.Y,
                z = position.Z
            };
        }

        public static Quaternion ToQuaternion(Rotation rotation)
        {
            return new Quaternion
            {
                x = rotation.X,
                y = rotation.Y,
                z = rotation.Z,
                w = rotation.W
            };
        }
    }
}