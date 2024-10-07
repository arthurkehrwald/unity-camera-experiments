using UnityEngine;

namespace Vector3Extensions
{
    public static class Vector3Extensions
    {
        public static Vector4 AddWCoordinate(this Vector3 vector, float w)
        {
            return new Vector4(vector.x, vector.y, vector.z, w);
        }
    }
}