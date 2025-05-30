using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

[StructLayout(LayoutKind.Sequential)]
internal struct CylinderShapeData
{
    public ConvexInternalShapeData ConvexInternalShapeData;
    public int UpAxis;
    public int Padding;

    public static int Offset(string fieldName)
        => Marshal.OffsetOf(typeof(CylinderShapeData), fieldName).ToInt32();
}

public class CylinderShape : ConvexInternalShape
{
    public CylinderShape(Vector3 halfExtents)
    {
        IntPtr native = btCylinderShape_new(ref halfExtents);
        InitializeCollisionShape(native);
    }

    public CylinderShape(float halfExtentX, float halfExtentY, float halfExtentZ)
    {
        IntPtr native = btCylinderShape_new2(halfExtentX, halfExtentY, halfExtentZ);
        InitializeCollisionShape(native);
    }

    protected internal CylinderShape()
    {
    }

    public Vector3 HalfExtentsWithMargin
    {
        get
        {
            Vector3 value;
            btCylinderShape_getHalfExtentsWithMargin(Native, out value);
            return value;
        }
    }

    public Vector3 HalfExtentsWithoutMargin
    {
        get
        {
            Vector3 value;
            btCylinderShape_getHalfExtentsWithoutMargin(Native, out value);
            return value;
        }
    }

    public float Radius => btCylinderShape_getRadius(Native);

    public int UpAxis => btCylinderShape_getUpAxis(Native);
}

public class CylinderShapeX : CylinderShape
{
    public CylinderShapeX(Vector3 halfExtents)
    {
        IntPtr native = btCylinderShapeX_new(ref halfExtents);
        InitializeCollisionShape(native);
    }

    public CylinderShapeX(float halfExtentX, float halfExtentY, float halfExtentZ)
    {
        IntPtr native = btCylinderShapeX_new2(halfExtentX, halfExtentY, halfExtentZ);
        InitializeCollisionShape(native);
    }
}

public class CylinderShapeZ : CylinderShape
{
    public CylinderShapeZ(Vector3 halfExtents)
    {
        IntPtr native = btCylinderShapeZ_new(ref halfExtents);
        InitializeCollisionShape(native);
    }

    public CylinderShapeZ(float halfExtentX, float halfExtentY, float halfExtentZ)
    {
        IntPtr native = btCylinderShapeZ_new2(halfExtentX, halfExtentY, halfExtentZ);
        InitializeCollisionShape(native);
    }
}
