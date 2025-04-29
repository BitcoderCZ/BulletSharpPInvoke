using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

[StructLayout(LayoutKind.Sequential)]
internal struct TriangleMeshShapeData
{
    public CollisionShapeData CollisionShapeData;
    public StridingMeshInterfaceData MeshInterface;
    public IntPtr QuantizedFloatBvh;
    public IntPtr QuantizedDoubleBvh;
    public IntPtr TriangleInfoMap;
    public float CollisionMargin;
    public int Pad;

    public static int Offset(string fieldName)
        => Marshal.OffsetOf(typeof(TriangleMeshShapeData), fieldName).ToInt32();
}

public class TriangleMeshShape : ConcaveShape
{
#pragma warning disable CS8618
    protected internal TriangleMeshShape()
#pragma warning restore CS8618
    {
    }

    public Vector3 LocalAabbMax
    {
        get
        {
            Vector3 value;
            btTriangleMeshShape_getLocalAabbMax(Native, out value);
            return value;
        }
    }

    public Vector3 LocalAabbMin
    {
        get
        {
            Vector3 value;
            btTriangleMeshShape_getLocalAabbMin(Native, out value);
            return value;
        }
    }

    public StridingMeshInterface MeshInterface { get; private set; }

    public void LocalGetSupportingVertex(ref Vector3 vec, out Vector3 value)
        => btTriangleMeshShape_localGetSupportingVertex(Native, ref vec, out value);

    public Vector3 LocalGetSupportingVertex(Vector3 vec)
    {
        Vector3 value;
        btTriangleMeshShape_localGetSupportingVertex(Native, ref vec, out value);
        return value;
    }

    public void LocalGetSupportingVertexWithoutMargin(ref Vector3 vec, out Vector3 value)
        => btTriangleMeshShape_localGetSupportingVertexWithoutMargin(Native, ref vec, out value);

    public Vector3 LocalGetSupportingVertexWithoutMargin(Vector3 vec)
    {
        Vector3 value;
        btTriangleMeshShape_localGetSupportingVertexWithoutMargin(Native, ref vec, out value);
        return value;
    }

    public void RecalcLocalAabb()
        => btTriangleMeshShape_recalcLocalAabb(Native);

    [MemberNotNull(nameof(MeshInterface))]
    protected internal void InitializeMembers(StridingMeshInterface meshInterface)
        => MeshInterface = meshInterface;
}
