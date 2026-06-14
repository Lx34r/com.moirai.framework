using UnityEngine;
using Moirai.GameProto.Config;

public static class ExternalTypeUtil
{
    public static Vector2 NewVector2(vector2 v)
    {
        return new Vector2(v.X, v.Y);
    }

    public static Vector3 NewVector3(vector3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

    public static Vector4 NewVector4(vector4 v)
    {
        return new Vector4(v.X, v.Y, v.Z, v.W);
    }

    public static Vector2Int NewVector2Int(vector2int v)
    {
        return new Vector2Int(v.X, v.Y);
    }

    public static Vector3Int NewVector3Int(vector3int v)
    {
        return new Vector3Int(v.X, v.Y, v.Z);
    }
}