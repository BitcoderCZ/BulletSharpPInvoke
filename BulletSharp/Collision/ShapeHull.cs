using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class ShapeHull : BulletDisposableObject
{
    private readonly ConvexShape _shape;
    private UIntArray? _indices;
    private Vector3Array? _vertices;

    public ShapeHull(ConvexShape shape)
    {
        IntPtr native = btShapeHull_new(shape.Native);
        InitializeUserOwned(native);
        _shape = shape;
    }

    public IntPtr IndexPointer => btShapeHull_getIndexPointer(Native);

    public UIntArray Indices
    {
        get
        {
            if (_indices == null)
            {
                _indices = new UIntArray(IndexPointer, NumIndices);
            }

            return _indices;
        }
    }

    public int NumIndices => btShapeHull_numIndices(Native);

    public int NumTriangles => btShapeHull_numTriangles(Native);

    public int NumVertices => btShapeHull_numVertices(Native);

    public IntPtr VertexPointer => btShapeHull_getVertexPointer(Native);

    public Vector3Array Vertices
    {
        get
        {
            if (_vertices == null || _vertices.Count != NumVertices)
            {
                _vertices = new Vector3Array(VertexPointer, NumVertices);
            }

            return _vertices;
        }
    }

    public bool BuildHull(float margin, int highRes = 0)
        => btShapeHull_buildHull(Native, margin, highRes);

    protected override void Dispose(bool disposing)
        => btShapeHull_delete(Native);
}
