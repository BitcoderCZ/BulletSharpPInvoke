using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

[StructLayout(LayoutKind.Sequential)]
internal struct MeshPartData
{
    public IntPtr Vertices3F;
    public IntPtr Vertices3D;
    public IntPtr Indices32;
    public IntPtr Indices16;
    public IntPtr Indices8;
#pragma warning disable SA1310 // Field names should not contain underscore
    public IntPtr Indices16_2;
#pragma warning restore SA1310 // Field names should not contain underscore
    public int NumTriangles;
    public int NumVertices;

    public static int Offset(string fieldName)
        => Marshal.OffsetOf(typeof(MeshPartData), fieldName).ToInt32();
}

[StructLayout(LayoutKind.Sequential)]
internal struct StridingMeshInterfaceData
{
    public IntPtr MeshPartsPtr;
    public Vector3FloatData Scaling;
    public int NumMeshParts;
    public int Padding;

    public static int Offset(string fieldName)
        => Marshal.OffsetOf(typeof(StridingMeshInterfaceData), fieldName).ToInt32();
}

public abstract class StridingMeshInterface : BulletDisposableObject
{
    protected internal StridingMeshInterface()
    {
    }

    public bool HasPremadeAabb => btStridingMeshInterface_hasPremadeAabb(Native);

    public int NumSubParts => btStridingMeshInterface_getNumSubParts(Native);

    public Vector3 Scaling
    {
        get
        {
            Vector3 value;
            btStridingMeshInterface_getScaling(Native, out value);
            return value;
        }
        set => btStridingMeshInterface_setScaling(Native, ref value);
    }

    public unsafe UnmanagedMemoryStream GetIndexStream(int subpart = 0)
    {
        btStridingMeshInterface_getLockedReadOnlyVertexIndexBase(Native, out _, out _, out _, out _, out var indexBase, out int indexStride, out int numFaces, out _, subpart);

        int length = numFaces * indexStride;
        return new UnmanagedMemoryStream((byte*)indexBase.ToPointer(), length, length, FileAccess.ReadWrite);
    }

    public unsafe UnmanagedMemoryStream GetVertexStream(int subpart = 0)
    {
        btStridingMeshInterface_getLockedReadOnlyVertexIndexBase(Native, out var vertexBase, out int numVerts, out _, out int vertexStride, out _, out _, out _, out _, subpart);

        int length = numVerts * vertexStride;
        return new UnmanagedMemoryStream((byte*)vertexBase.ToPointer(), length, length, FileAccess.ReadWrite);
    }

    public void CalculateAabbBruteForce(out Vector3 aabbMin, out Vector3 aabbMax)
        => btStridingMeshInterface_calculateAabbBruteForce(Native, out aabbMin, out aabbMax);

    public int CalculateSerializeBufferSize()
        => btStridingMeshInterface_calculateSerializeBufferSize(Native);

    public void GetLockedReadOnlyVertexIndexBase(out IntPtr vertexBase, out int numVerts, out PhyScalarType type, out int vertexStride, out IntPtr indexbase, out int indexStride, out int numFaces, out PhyScalarType indicesType, int subpart = 0)
        => btStridingMeshInterface_getLockedReadOnlyVertexIndexBase(Native, out vertexBase, out numVerts, out type, out vertexStride, out indexbase, out indexStride, out numFaces, out indicesType, subpart);

    public void GetLockedVertexIndexBase(out IntPtr vertexBase, out int numVerts, out PhyScalarType type, out int vertexStride, out IntPtr indexbase, out int indexStride, out int numFaces, out PhyScalarType indicesType, int subpart = 0)
        => btStridingMeshInterface_getLockedVertexIndexBase(Native, out vertexBase, out numVerts, out type, out vertexStride, out indexbase, out indexStride, out numFaces, out indicesType, subpart);

    public void GetPremadeAabb(out Vector3 aabbMin, out Vector3 aabbMax)
        => btStridingMeshInterface_getPremadeAabb(Native, out aabbMin, out aabbMax);

    public void InternalProcessAllTriangles(InternalTriangleIndexCallback callback, Vector3 aabbMin, Vector3 aabbMax)
        => btStridingMeshInterface_InternalProcessAllTriangles(Native, callback.Native, ref aabbMin, ref aabbMax);

    public void PreallocateIndices(int numIndices)
        => btStridingMeshInterface_preallocateIndices(Native, numIndices);

    public void PreallocateVertices(int numVerts)
        => btStridingMeshInterface_preallocateVertices(Native, numVerts);

    public string Serialize(IntPtr dataBuffer, Serializer serializer)
        => Marshal.PtrToStringAnsi(btStridingMeshInterface_serialize(Native, dataBuffer, serializer.Native));

    public void SetPremadeAabb(ref Vector3 aabbMin, ref Vector3 aabbMax)
        => btStridingMeshInterface_setPremadeAabb(Native, ref aabbMin, ref aabbMax);

    public void SetPremadeAabb(Vector3 aabbMin, Vector3 aabbMax)
        => btStridingMeshInterface_setPremadeAabb(Native, ref aabbMin, ref aabbMax);

    public void UnLockReadOnlyVertexBase(int subpart)
        => btStridingMeshInterface_unLockReadOnlyVertexBase(Native, subpart);

    public void UnLockVertexBase(int subpart)
        => btStridingMeshInterface_unLockVertexBase(Native, subpart);

    protected override void Dispose(bool disposing)
        => btStridingMeshInterface_delete(Native);
}
