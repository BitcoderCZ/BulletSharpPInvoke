using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

[StructLayout(LayoutKind.Sequential)]
internal struct StaticPlaneShapeData
{
    public CollisionShapeData CollisionShapeData;
    public Vector3FloatData LocalScaling;
    public Vector3FloatData PlaneNormal;
    public float PlaneConstant;
    public int Padding;

    public static int Offset(string fieldName)
        => Marshal.OffsetOf(typeof(StaticPlaneShapeData), fieldName).ToInt32();
}

public class StaticPlaneShape : ConcaveShape
{
    public StaticPlaneShape(Vector3 planeNormal, float planeConstant)
    {
        IntPtr native = btStaticPlaneShape_new(ref planeNormal, planeConstant);
        InitializeCollisionShape(native);
    }

    public float PlaneConstant => btStaticPlaneShape_getPlaneConstant(Native);

    public Vector3 PlaneNormal
    {
        get
        {
            Vector3 value;
            btStaticPlaneShape_getPlaneNormal(Native, out value);
            return value;
        }
    }
}
