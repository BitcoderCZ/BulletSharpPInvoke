using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class IndexedMesh : BulletDisposableObject
{
    private bool _ownsData;

    public IndexedMesh()
    {
        IntPtr native = btIndexedMesh_new();
        InitializeUserOwned(native);
    }

    internal IndexedMesh(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public PhyScalarType IndexType
    {
        get => btIndexedMesh_getIndexType(Native);
        set => btIndexedMesh_setIndexType(Native, value);
    }

    public int NumTriangles
    {
        get => btIndexedMesh_getNumTriangles(Native);
        set => btIndexedMesh_setNumTriangles(Native, value);
    }

    public int NumVertices
    {
        get => btIndexedMesh_getNumVertices(Native);
        set => btIndexedMesh_setNumVertices(Native, value);
    }

    public IntPtr TriangleIndexBase
    {
        get => btIndexedMesh_getTriangleIndexBase(Native);
        set => btIndexedMesh_setTriangleIndexBase(Native, value);
    }

    public int TriangleIndexStride
    {
        get => btIndexedMesh_getTriangleIndexStride(Native);
        set => btIndexedMesh_setTriangleIndexStride(Native, value);
    }

    public IntPtr VertexBase
    {
        get => btIndexedMesh_getVertexBase(Native);
        set => btIndexedMesh_setVertexBase(Native, value);
    }

    public int VertexStride
    {
        get => btIndexedMesh_getVertexStride(Native);
        set => btIndexedMesh_setVertexStride(Native, value);
    }

    public PhyScalarType VertexType
    {
        get => btIndexedMesh_getVertexType(Native);
        set => btIndexedMesh_setVertexType(Native, value);
    }

    public void Allocate(int numTriangles, int numVertices, int triangleIndexStride = sizeof(int) * 3, int vertexStride = sizeof(float) * 3)
    {
        if (_ownsData)
        {
            Free();
        }
        else
        {
            _ownsData = true;
        }

        IndexType = triangleIndexStride switch
        {
            sizeof(byte) * 3 => PhyScalarType.Byte,
            sizeof(short) * 3 => PhyScalarType.Int16,
            _ => PhyScalarType.Int32,
        };

        VertexType = PhyScalarType.Single;

        NumTriangles = numTriangles;
        TriangleIndexBase = Marshal.AllocHGlobal(numTriangles * triangleIndexStride);
        TriangleIndexStride = triangleIndexStride;
        NumVertices = numVertices;
        VertexBase = Marshal.AllocHGlobal(numVertices * vertexStride);
        VertexStride = vertexStride;
    }

    public void Free()
    {
        if (_ownsData)
        {
            Marshal.FreeHGlobal(TriangleIndexBase);
            Marshal.FreeHGlobal(VertexBase);
            _ownsData = false;
        }
    }

    public unsafe UnmanagedMemoryStream GetTriangleStream()
    {
        int length = NumTriangles * TriangleIndexStride;
        return new UnmanagedMemoryStream((byte*)TriangleIndexBase.ToPointer(), length, length, FileAccess.ReadWrite);
    }

    public unsafe UnmanagedMemoryStream GetVertexStream()
    {
        int length = NumVertices * VertexStride;
        return new UnmanagedMemoryStream((byte*)btIndexedMesh_getVertexBase(Native).ToPointer(), length, length, FileAccess.ReadWrite);
    }

    public void SetData(ICollection<int> triangles, ICollection<float> vertices)
    {
        SetTriangles(triangles);

        float[]? vertexArray = vertices as float[];
        if (vertexArray == null)
        {
            vertexArray = new float[vertices.Count];
            vertices.CopyTo(vertexArray, 0);
        }

        Marshal.Copy(vertexArray, 0, VertexBase, vertices.Count);
    }

    public void SetData(ICollection<int> triangles, ICollection<Vector3> vertices)
    {
        SetTriangles(triangles);

        float[] vertexArray = new float[vertices.Count * 3];
        int i = 0;
        foreach (Vector3 v in vertices)
        {
            vertexArray[i] = v.X;
            vertexArray[i + 1] = v.Y;
            vertexArray[i + 2] = v.Z;
            i += 3;
        }

        Marshal.Copy(vertexArray, 0, VertexBase, vertexArray.Length);
    }

    protected override void Dispose(bool disposing)
    {
        Free();
        if (IsUserOwned)
        {
            btIndexedMesh_delete(Native);
        }
    }

    private void SetTriangles(ICollection<int> triangles)
    {
        int[]? triangleArray = triangles as int[];
        if (triangleArray == null)
        {
            triangleArray = new int[triangles.Count];
            triangles.CopyTo(triangleArray, 0);
        }

        Marshal.Copy(triangleArray, 0, TriangleIndexBase, triangleArray.Length);
    }
}

public class TriangleIndexVertexArray : StridingMeshInterface
{
    private IndexedMesh? _initialMesh;

    public TriangleIndexVertexArray()
    {
        IntPtr native = btTriangleIndexVertexArray_new();
        InitializeUserOwned(native);
        InitializeMembers();
    }

    public TriangleIndexVertexArray(ICollection<int> triangles, ICollection<float> vertices)
    {
        IntPtr native = btTriangleIndexVertexArray_new();
        InitializeUserOwned(native);
        InitializeMembers();

        _initialMesh = new IndexedMesh();
        _initialMesh.Allocate(triangles.Count / 3, vertices.Count / 3);
        _initialMesh.SetData(triangles, vertices);
        AddIndexedMesh(_initialMesh);
    }

    public TriangleIndexVertexArray(ICollection<int> triangles, ICollection<Vector3> vertices)
    {
        IntPtr native = btTriangleIndexVertexArray_new();
        InitializeUserOwned(native);
        InitializeMembers();

        _initialMesh = new IndexedMesh();
        _initialMesh.Allocate(triangles.Count / 3, vertices.Count);
        _initialMesh.SetData(triangles, vertices);
        AddIndexedMesh(_initialMesh);
    }

    public TriangleIndexVertexArray(int numTriangles, IntPtr triangleIndexBase, int triangleIndexStride, int numVertices, IntPtr vertexBase, int vertexStride)
    {
        IntPtr native = btTriangleIndexVertexArray_new2(numTriangles, triangleIndexBase, triangleIndexStride, numVertices, vertexBase, vertexStride);
        InitializeUserOwned(native);
        InitializeMembers();
    }

#pragma warning disable CS8618
#pragma warning disable IDE0060
    internal TriangleIndexVertexArray(ConstructionInfo? info)
#pragma warning restore IDE0060
#pragma warning restore CS8618
    {
    }

    public AlignedIndexedMeshArray IndexedMeshArray { get; private set; }

    public void AddIndexedMesh(IndexedMesh mesh, PhyScalarType indexType = PhyScalarType.Int32)
    {
        mesh.IndexType = indexType;
        IndexedMeshArray.Add(mesh);
    }

    [MemberNotNull(nameof(IndexedMeshArray))]
    protected internal void InitializeMembers()
        => IndexedMeshArray = new AlignedIndexedMeshArray(btTriangleIndexVertexArray_getIndexedMeshArray(Native), this);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_initialMesh != null)
            {
                _initialMesh.Dispose();
                _initialMesh = null;
            }
        }

        base.Dispose(disposing);
    }
}
