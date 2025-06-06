﻿// *
// * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
// *
// * Bullet Continuous Collision Detection and Physics Library
// * Copyright (c) 2003-2008 Erwin Coumans  http://www.bulletphysics.com/
// *
// * This software is provided 'as-is', without any express or implied warranty.
// * In no event will the authors be held liable for any damages arising from
// * the use of this software.
// * 
// * Permission is granted to anyone to use this software for any purpose, 
// * including commercial applications, and to alter it and redistribute it
// * freely, subject to the following restrictions:
// * 
// * 1. The origin of this software must not be misrepresented; you must not
// *    claim that you wrote the original software. If you use this software
// *    in a product, an acknowledgment in the product documentation would be
// *    appreciated but is not required.
// * 2. Altered source versions must be plainly marked as such, and must not be
// *    misrepresented as being the original software.
// * 3. This notice may not be removed or altered from any source distribution.
// */

using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace BulletSharp;

public static class MathUtil
{
#pragma warning disable SA1310 // Field names should not contain underscore
    //public const float SIMD_EPSILON = 0.0000001f;
    public const float SIMD_EPSILON = 1.192092896e-07f;
    public const float SIMDSQRT12 = 0.7071067811865475244008443621048490f;

    public const float BT_LARGE_FLOAT = 1e18f;
    public const float SIMD_2_PI = 6.283185307179586232f;
    public const float SIMD_PI = SIMD_2_PI * 0.5f;
    public const float SIMD_HALF_PI = SIMD_PI * 0.5f;
    public const float SIMD_QUARTER_PI = SIMD_PI * 0.25f;

    public const float SIMD_INFINITY = float.MaxValue;
    public const float SIMD_RADS_PER_DEG = SIMD_2_PI / 360.0f;
    public const float SIMD_DEGS_PER_RAD = 360.0f / SIMD_2_PI;
    public static Vector3 MAX_VECTOR = new Vector3(BT_LARGE_FLOAT);
    public static Vector3 MIN_VECTOR = new Vector3(-BT_LARGE_FLOAT);
#pragma warning restore SA1310 // Field names should not contain underscore

    //        //public static Matrix TransposeTimesBasis(ref Matrix a, ref Matrix b)
    //        public static Matrix TransposeTimesBasis(ref Matrix mA, ref Matrix mB)

    //        {
    //            Matrix ba = MathUtil.BasisMatrix(ref mA);
    //            ba = Matrix.Transpose(ba);
    //            Matrix bb = MathUtil.BasisMatrix(ref mB);
    //            return BulletMatrixMultiply(ref ba, ref bb);
    //        }

    //        public static Matrix InverseTimes(Matrix a, Matrix b)
    //        {
    //            return InverseTimes(ref a, ref b);
    //        }

    //        public static Matrix InverseTimes(ref Matrix a, ref Matrix b)
    //        {
    //            Matrix m = Matrix.Invert(a);
    //            return BulletMatrixMultiply(ref m, ref b);
    //        }

    //        public static Matrix TransposeBasis(Matrix m)
    //        {
    //            return TransposeBasis(ref m);
    //        }

    //        public static Matrix TransposeBasis(ref Matrix m)
    //        {
    //            return Matrix.Transpose(BasisMatrix(ref m));
    //        }

    //        public static Matrix InverseBasis(Matrix m)
    //        {
    //            return InverseBasis(ref m);
    //        }

    //        public static Matrix InverseBasis(ref Matrix m)
    //        {
    //            Matrix b = BasisMatrix(ref m);
    //            b = Matrix.Invert(b);
    //            return b;
    //        }

    //        public static float Cofac(ref Matrix m,int r1, int c1, int r2, int c2)
    //        {
    //            float a = MatrixComponent(ref m, r1, c1);
    //            float b = MatrixComponent(ref m, r2, c2);
    //            float c = MatrixComponent(ref m, r1, c2);
    //            float d = MatrixComponent(ref m, r2, c1);

    //            return a * b - c * d;
    //        }

    // dodgy but seems necessary for rounding issues.
    //return a >= -0.00001 ? b : c;
    public static float FSel(float a, float b, float c)
        => a >= 0 ? b : c;

    public static int MaxAxis(ref Vector3 a)
        => a.X < a.Y ? (a.Y < a.Z ? 2 : 1) : (a.X < a.Z ? 2 : 0);

    public static int MaxAxis(Vector4 a)
        => MaxAxis(ref a);

    public static int MaxAxis(ref Vector4 a)
    {
        int maxIndex = -1;
        float maxVal = -BT_LARGE_FLOAT;
        if (a.X > maxVal)
        {
            maxIndex = 0;
            maxVal = a.X;
        }

        if (a.Y > maxVal)
        {
            maxIndex = 1;
            maxVal = a.Y;
        }

        if (a.Z > maxVal)
        {
            maxIndex = 2;
            maxVal = a.Z;
        }

        if (a.W > maxVal)
        {
            maxIndex = 3;
            //maxVal = a.W;
        }

        return maxIndex;
    }

    public static int ClosestAxis(ref Vector4 a)
        => MaxAxis(AbsoluteVector4(ref a));

    public static Vector4 AbsoluteVector4(ref Vector4 vec)
        => new Vector4(MathF.Abs(vec.X), MathF.Abs(vec.Y), MathF.Abs(vec.Z), MathF.Abs(vec.W));

    //        public static float VectorComponent(Vector3 v, int i)
    //        {
    //            return VectorComponent(ref v, i);
    //        }

    //        public static float VectorComponent(ref Vector3 v, int i)
    //        {
    //            switch (i)
    //            {
    //                case 0:
    //                    return v.X;
    //                case 1:
    //                    return v.Y;
    //                case 2:
    //                    return v.Z;
    //                default:
    //                    Debug.Assert(false);
    //                    return 0.0f;
    //            }
    //        }

    //        public static void VectorComponent(ref Vector3 v, int i, float f)
    //        {
    //            switch (i)
    //            {
    //                case 0:
    //                    v.X = f;
    //                    return;
    //                case 1:
    //                    v.Y = f;
    //                    return;
    //                case 2:
    //                    v.Z = f;
    //                    return;
    //            }
    //            Debug.Assert(false);
    //        }

    //        public static void VectorComponentAddAssign(ref Vector3 v, int i, float f)
    //        {
    //            switch (i)
    //            {
    //                case 0:
    //                    v.X += f;
    //                    return;
    //                case 1:
    //                    v.Y += f;
    //                    return;
    //                case 2:
    //                    v.Z += f;
    //                    return;
    //            }
    //            Debug.Assert(false);
    //        }

    //        public static void VectorComponentMinusAssign(ref Vector3 v, int i, float f)
    //        {
    //            switch (i)
    //            {
    //                case 0:
    //                    v.X -= f;
    //                    return;
    //                case 1:
    //                    v.Y -= f;
    //                    return;
    //                case 2:
    //                    v.Z -= f;
    //                    return;
    //            }
    //            Debug.Assert(false);
    //        }

    //        public static void VectorComponentMultiplyAssign(ref Vector3 v, int i, float f)
    //        {
    //            switch (i)
    //            {
    //                case 0:
    //                    v.X *= f;
    //                    return;
    //                case 1:
    //                    v.Y *= f;
    //                    return;
    //                case 2:
    //                    v.Z *= f;
    //                    return;
    //            }
    //            Debug.Assert(false);
    //        }

    //        public static void VectorComponentDivideAssign(ref Vector3 v, int i, float f)
    //        {
    //            switch (i)
    //            {
    //                case 0:
    //                    v.X /= f;
    //                    return;
    //                case 1:
    //                    v.Y /= f;
    //                    return;
    //                case 2:
    //                    v.Z /= f;
    //                    return;
    //            }
    //            Debug.Assert(false);
    //        }

    //        public static Matrix AbsoluteMatrix(Matrix input)
    //        {
    //            return AbsoluteMatrix(ref input);
    //        }

    //        public static Matrix AbsoluteMatrix(ref Matrix input)
    //        {
    //            Matrix output;
    //            AbsoluteMatrix(ref input, out output);
    //            return output;
    //        }

    //        public static void AbsoluteMatrix(ref Matrix input, out Matrix output)
    //        {
    //            output = new Matrix(
    //                System.Math.Abs(input.M11),
    //                System.Math.Abs(input.M12),
    //                System.Math.Abs(input.M13),
    //                System.Math.Abs(input.M14),
    //                System.Math.Abs(input.M21),
    //                System.Math.Abs(input.M22),
    //                System.Math.Abs(input.M23),
    //                System.Math.Abs(input.M24),
    //                System.Math.Abs(input.M31),
    //                System.Math.Abs(input.M32),
    //                System.Math.Abs(input.M33),
    //                System.Math.Abs(input.M34),
    //                System.Math.Abs(input.M41),
    //                System.Math.Abs(input.M42),
    //                System.Math.Abs(input.M43),
    //                System.Math.Abs(input.M44));
    //        }

    //        public static Matrix AbsoluteBasisMatrix(ref Matrix input)
    //        {
    //            Matrix output;
    //            AbsoluteBasisMatrix(ref input, out output);
    //            return output;
    //        }

    //        public static void AbsoluteBasisMatrix(ref Matrix input, out Matrix output)
    //        {
    //            output = new Matrix(
    //                System.Math.Abs(input.M11), System.Math.Abs(input.M12), System.Math.Abs(input.M13), 0.0f,
    //                System.Math.Abs(input.M21), System.Math.Abs(input.M22), System.Math.Abs(input.M23), 0.0f,
    //                System.Math.Abs(input.M31), System.Math.Abs(input.M32), System.Math.Abs(input.M33), 0.0f,
    //                0.0f, 0.0f, 0.0f, 1.0f);
    //        }

    //        public static void AbsoluteVector(ref Vector3 input, out Vector3 output)
    //        {
    //            output = new Vector3(
    //                System.Math.Abs(input.X),
    //                System.Math.Abs(input.Y),
    //                System.Math.Abs(input.Z));
    //        }

    //        public static void RotateVector(ref Vector3 vec, ref Matrix m, out Vector3 output)
    //        {
    //            Quaternion rotation;
    //            Vector3 component;
    //            m.Decompose(out component, out rotation, out component);
    //            output = Vector3.Transform(vec, rotation);
    //        }

    //        public static void TransformAabb(Vector3 halfExtents, float margin, Matrix trans, out Vector3 aabbMinOut, out Vector3 aabbMaxOut)
    //        {
    //            //TransformAabb(ref halfExtents,margin,ref trans,out aabbMinOut,out aabbMaxOut);
    //            Vector3 halfExtentsWithMargin = halfExtents + new Vector3(margin);
    //            Vector3 center, extent;
    //            AbsoluteExtents(ref trans, ref halfExtentsWithMargin, out center, out extent);
    //            aabbMinOut = center - extent;
    //            aabbMaxOut = center + extent;
    //        }

    //        public static void TransformAabb(ref Vector3 halfExtents, float margin, ref Matrix trans, out Vector3 aabbMinOut, out Vector3 aabbMaxOut)
    //        {
    //            Vector3 halfExtentsWithMargin = halfExtents + new Vector3(margin);
    //            Vector3 center, extent;
    //            AbsoluteExtents(ref trans, ref halfExtentsWithMargin, out center, out extent);
    //            aabbMinOut = center - extent;
    //            aabbMaxOut = center + extent;
    //        }

    //        public static void TransformAabb(Vector3 localAabbMin, Vector3 localAabbMax, float margin, Matrix trans, out Vector3 aabbMinOut, out Vector3 aabbMaxOut)
    //        {
    //            TransformAabb(ref localAabbMin, ref localAabbMax, margin, ref trans, out aabbMinOut, out aabbMaxOut);
    //        }

    //        public static void TransformAabb(ref Vector3 localAabbMin, ref Vector3 localAabbMax, float margin, ref Matrix trans, out Vector3 aabbMinOut, out Vector3 aabbMaxOut)
    //        {
    //            Debug.Assert(localAabbMin.X <= localAabbMax.X);
    //            Debug.Assert(localAabbMin.Y <= localAabbMax.Y);
    //            Debug.Assert(localAabbMin.Z <= localAabbMax.Z);
    //            Vector3 localHalfExtents = 0.5f * (localAabbMax - localAabbMin);
    //            localHalfExtents += new Vector3(margin);

    //            Vector3 localCenter = 0.5f * (localAabbMax + localAabbMin);
    //            Matrix abs_b = MathUtil.AbsoluteBasisMatrix(ref trans);

    //            Vector3 center = Vector3.Transform(localCenter, trans);

    //            Vector3 extent = new Vector3(Vector3.Dot(abs_b.Right, localHalfExtents),
    //                                            Vector3.Dot(abs_b.Up, localHalfExtents),
    //                                            Vector3.Dot(abs_b.Backward, localHalfExtents));

    //            aabbMinOut = center - extent;
    //            aabbMaxOut = center + extent;
    //        }

    //        public static void AbsoluteExtents(ref Matrix trans, ref Vector3 vec, out Vector3 center, out Vector3 extent)
    //        {
    //            Matrix abs_b;
    //            AbsoluteMatrix(ref trans, out abs_b);

    //            center = trans._origin;
    //            extent = new Vector3(Vector3.Dot(abs_b.Right, vec),
    //                                            Vector3.Dot(abs_b.Up, vec),
    //                                            Vector3.Dot(abs_b.Backward, vec));
    //        }

    //        public static void SetMatrixVector(ref Matrix matrix, int row, Vector3 vector)
    //        {
    //            SetMatrixVector(ref matrix, row, ref vector);
    //        }

    //        public static void SetMatrixVector(ref Matrix matrix, int row, ref Vector3 vector)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    matrix.M11 = vector.X;
    //                    matrix.M12 = vector.Y;
    //                    matrix.M13 = vector.Z;
    //                    return;
    //                case 1:
    //                    matrix.M21 = vector.X;
    //                    matrix.M22 = vector.Y;
    //                    matrix.M23 = vector.Z;
    //                    return;
    //                case 2:
    //                    matrix.M31 = vector.X;
    //                    matrix.M32 = vector.Y;
    //                    matrix.M33 = vector.Z;
    //                    return;
    //                case 3:
    //                    matrix.M41 = vector.X;
    //                    matrix.M42 = vector.Y;
    //                    matrix.M43 = vector.Z;
    //                    return;
    //            }
    //            Debug.Assert(false);
    //        }

    //        public static void AddMatrixVector(ref Matrix matrix, int row, ref Vector3 vector)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    matrix.M11 += vector.X;
    //                    matrix.M12 += vector.Y;
    //                    matrix.M13 += vector.Z;
    //                    return;
    //                case 1:
    //                    matrix.M21 += vector.X;
    //                    matrix.M22 += vector.Y;
    //                    matrix.M23 += vector.Z;
    //                    return;
    //                case 2:
    //                    matrix.M31 += vector.X;
    //                    matrix.M32 += vector.Y;
    //                    matrix.M33 += vector.Z;
    //                    return;
    //                case 3:
    //                    matrix.M41 += vector.X;
    //                    matrix.M42 += vector.Y;
    //                    matrix.M43 += vector.Z;
    //                    return;
    //            }
    //            Debug.Assert(false);
    //        }

    public static float Vector3Triple(ref Vector3 a, ref Vector3 b, ref Vector3 c)
        => (a.X * ((b.Y * c.Z) - (b.Z * c.Y))) +
            (a.Y * ((b.Z * c.X) - (b.X * c.Z))) +
            (a.Z * ((b.X * c.Y) - (b.Y * c.X)));

    //        // FIXME - MAN - make sure this is being called how we'd expect , may need to
    //        // swap i,j for row/column differences

    //        public static float MatrixComponent(ref Matrix m, int index)
    //        {
    //            //int i = index % 4;
    //            //int j = index / 4;

    //            int j = index % 4;
    //            int i = index / 4;

    //            return MatrixComponent(ref m,i,j);
    //        }

    //        public static float MatrixComponent(ref Matrix m, int row, int column)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    if (column == 0) return m.M11;
    //                    if (column == 1) return m.M12;
    //                    if (column == 2) return m.M13;
    //                    if (column == 3) return m.M14;
    //                    break;
    //                case 1:
    //                    if (column == 0) return m.M21;
    //                    if (column == 1) return m.M22;
    //                    if (column == 2) return m.M23;
    //                    if (column == 3) return m.M24;
    //                    break;
    //                case 2:
    //                    if (column == 0) return m.M31;
    //                    if (column == 1) return m.M32;
    //                    if (column == 2) return m.M33;
    //                    if (column == 3) return m.M34;
    //                    break;
    //                case 3:
    //                    if (column == 0) return m.M41;
    //                    if (column == 1) return m.M42;
    //                    if (column == 2) return m.M43;
    //                    if (column == 3) return m.M44;
    //                    break;
    //            }
    //            return 0;
    //        }

    //        public static void MatrixComponent(ref Matrix m, int row, int column, float val)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    if (column == 0) m.M11 = val;
    //                    if (column == 1) m.M12 = val;
    //                    if (column == 2) m.M13 = val;
    //                    if (column == 3) m.M14 = val;
    //                    break;
    //                case 1:
    //                    if (column == 0) m.M21 = val;
    //                    if (column == 1) m.M22 = val;
    //                    if (column == 2) m.M23 = val;
    //                    if (column == 3) m.M24 = val;
    //                    break;
    //                case 2:
    //                    if (column == 0) m.M31 = val;
    //                    if (column == 1) m.M32 = val;
    //                    if (column == 2) m.M33 = val;
    //                    if (column == 3) m.M34 = val;
    //                    break;
    //                case 3:
    //                    if (column == 0) m.M41 = val;
    //                    if (column == 1) m.M42 = val;
    //                    if (column == 2) m.M43 = val;
    //                    if (column == 3) m.M44 = val;
    //                    break;
    //            }
    //        }

    //        public static Vector3 MatrixColumn(Matrix matrix, int row)
    //        {
    //            return MatrixColumn(ref matrix, row);
    //        }

    //        public static Vector3 MatrixColumn(ref Matrix matrix, int row)
    //        {
    //            Vector3 vectorRow;
    //            MatrixColumn(ref matrix, row, out vectorRow);
    //            return vectorRow;
    //        }

    //        public static void MatrixColumn(Matrix matrix, int row, out Vector3 vectorRow)
    //        {
    //            MatrixColumn(ref matrix,row, out vectorRow);
    //        }

    //        public static void MatrixColumn(ref Matrix matrix, int row, out Vector3 vectorRow)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    vectorRow = new Vector3(matrix.M11, matrix.M12, matrix.M13);
    //                    break;
    //                case 1:
    //                    vectorRow = new Vector3(matrix.M21, matrix.M22, matrix.M23);
    //                    break;
    //                case 2:
    //                    vectorRow = new Vector3(matrix.M31, matrix.M32, matrix.M33);
    //                    break;
    //                case 3:
    //                    vectorRow = new Vector3(matrix.M41, matrix.M42, matrix.M43);
    //                    break;
    //                default:
    //                    vectorRow = Vector3.Zero;
    //                    break;
    //            }
    //        }

    //        public static Vector3 MatrixRow(Matrix matrix, int row)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    return new Vector3(matrix.M11, matrix.M21, matrix.M31);
    //                case 1:
    //                    return new Vector3(matrix.M12, matrix.M22, matrix.M32);
    //                case 2:
    //                    return new Vector3(matrix.M13, matrix.M23, matrix.M33);
    //                case 3:
    //                    return new Vector3(matrix.M14, matrix.M24, matrix.M34);
    //                default:
    //                    return Vector3.Zero;
    //            }
    //        }

    //        public static Vector3 MatrixRow(ref Matrix matrix, int row)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    return new Vector3(matrix.M11, matrix.M21, matrix.M31);
    //                case 1:
    //                    return new Vector3(matrix.M12, matrix.M22, matrix.M32);
    //                case 2:
    //                    return new Vector3(matrix.M13, matrix.M23, matrix.M33);
    //                case 3:
    //                    return new Vector3(matrix.M14, matrix.M24, matrix.M34);
    //                default:
    //                    return Vector3.Zero;
    //            }
    //        }

    //        public static void MatrixRow(ref Matrix matrix, int row, out Vector3 vectorRow)
    //        {
    //            switch (row)
    //            {
    //                case 0:
    //                    vectorRow = new Vector3(matrix.M11, matrix.M21, matrix.M31);
    //                    break;
    //                case 1:
    //                    vectorRow = new Vector3(matrix.M12, matrix.M22, matrix.M32);
    //                    break;
    //                case 2:
    //                    vectorRow = new Vector3(matrix.M13, matrix.M23, matrix.M33);
    //                    break;
    //                case 3:
    //                    vectorRow = new Vector3(matrix.M14, matrix.M24, matrix.M34);
    //                    break;
    //                default:
    //                    vectorRow = Vector3.Zero;
    //                    break;
    //            }
    //        }

    public static int GetQuantized(float x)
        => x < 0.0
        ? (int)(x - 0.5)
        : (int)(x + 0.5);

    public static int Clamp(int value, int min, int max)
        => (value < min) ? min : (value > max) ? max : value;

    public static void VectorClampMax(ref Vector3 input, ref Vector3 bounds)
    {
        input.X = MathF.Min(input.X, bounds.X);
        input.Y = MathF.Min(input.Y, bounds.Y);
        input.Z = MathF.Min(input.Z, bounds.Z);
    }

    public static void VectorClampMin(ref Vector3 input, ref Vector3 bounds)
    {
        input.X = MathF.Max(input.X, bounds.X);
        input.Y = MathF.Max(input.Y, bounds.Y);
        input.Z = MathF.Max(input.Z, bounds.Z);
    }

    public static void VectorMin(ref Vector3 input, ref Vector3 output)
    {
        output.X = MathF.Min(input.X, output.X);
        output.Y = MathF.Min(input.Y, output.Y);
        output.Z = MathF.Min(input.Z, output.Z);
    }

    public static void VectorMin(ref Vector3 input1, ref Vector3 input2, out Vector3 output)
        => output = new Vector3(
            Math.Min(input1.X, input2.X),
            Math.Min(input1.Y, input2.Y),
            Math.Min(input1.Z, input2.Z));

    public static void VectorMax(ref Vector3 input, ref Vector3 output)
    {
        output.X = MathF.Max(input.X, output.X);
        output.Y = MathF.Max(input.Y, output.Y);
        output.Z = MathF.Max(input.Z, output.Z);
    }

    public static void VectorMax(ref Vector3 input1, ref Vector3 input2, out Vector3 output)
        => output = new Vector3(
            MathF.Max(input1.X, input2.X),
            MathF.Max(input1.Y, input2.Y),
            MathF.Max(input1.Z, input2.Z));

    public static float RecipSqrt(float a)
        => (float)(1d / Math.Sqrt(a)); // TODO: MathF.Sqrt?

    public static bool CompareFloat(float val1, float val2)
        => MathF.Abs(val1 - val2) <= SIMD_EPSILON;

    public static bool FuzzyZero(float val)
        => MathF.Abs(val) <= SIMD_EPSILON;

    public static bool FuzzyZero(Vector3 val)
        => val.LengthSquared() < SIMD_EPSILON * SIMD_EPSILON;

    public static uint Select(uint condition, uint valueIfConditionNonZero, uint valueIfConditionZero)
    {
        // Set testNz to 0xFFFFFFFF if condition is nonzero, 0x00000000 if condition is zero
        // Rely on positive value or'ed with its negative having sign bit on
        // and zero value or'ed with its negative (which is still zero) having sign bit off 
        // Use arithmetic shift right, shifting the sign bit through all 32 bits
        uint testNz = (uint)(((int)condition | -(int)condition) >> 31);
        uint testEqz = ~testNz;
        return (valueIfConditionNonZero & testNz) | (valueIfConditionZero & testEqz);
    }

    //        public static void BasisMatrix(Matrix matrixIn, out Matrix matrixOut)
    //        {
    //            BasisMatrix(ref matrixIn, out matrixOut);
    //        }
    //        public static void BasisMatrix(ref Matrix matrixIn, out Matrix matrixOut)
    //        {
    //            matrixOut = matrixIn;
    //            matrixOut.M41 = 0.0f;
    //            matrixOut.M42 = 0.0f;
    //            matrixOut.M43 = 0.0f;
    //            matrixOut.M44 = 1.0f;
    //        }

    //        public static Matrix BasisMatrix(Matrix matrixIn)
    //        {
    //            return BasisMatrix(ref matrixIn);
    //        }
    //        public static Matrix BasisMatrix(ref Matrix matrixIn)
    //        {
    //            Matrix matrixOut = matrixIn;
    //            matrixOut.M41 = 0.0f;
    //            matrixOut.M42 = 0.0f;
    //            matrixOut.M43 = 0.0f;
    //            matrixOut.M44 = 1.0f;
    //            return matrixOut;
    //        }

    public static Quaternion ShortestArcQuat(ref Vector3 axisInA, ref Vector3 axisInB)
    {
        Vector3 c = Vector3.Cross(axisInA, axisInB);
        float d = Vector3.Dot(axisInA, axisInB);

        if (d < -1.0 + SIMD_EPSILON)
        {
            return new Quaternion(0.0f, 1.0f, 0.0f, 0.0f); // just pick any vector
        }

        float s = MathF.Sqrt((1.0f + d) * 2.0f);
        float rs = 1.0f / s;

        return new Quaternion(c.X * rs, c.Y * rs, c.Z * rs, s * 0.5f);
    }

    public static float QuatAngle(ref Quaternion quat)
        => 2f * MathF.Acos(quat.W);

    public static Quaternion QuatFurthest(ref Quaternion input1, ref Quaternion input2)
    {
        Quaternion diff, sum;
        diff = input1 - input2;
        sum = input1 + input2;

        return Quaternion.Dot(diff, diff) > Quaternion.Dot(sum, sum)
            ? input2
            : -input2;
    }

    public static Vector3 QuatRotate(ref Quaternion rotation, ref Vector3 v)
    {
        Quaternion q = QuatVectorMultiply(ref rotation, ref v);
        q *= QuaternionInverse(ref rotation);
        return new Vector3(q.X, q.Y, q.Z);
    }

    public static Quaternion QuatVectorMultiply(ref Quaternion q, ref Vector3 w)
        => new Quaternion(
            (q.W * w.X) + (q.Y * w.Z) - (q.Z * w.Y),
            (q.W * w.Y) + (q.Z * w.X) - (q.X * w.Z),
            (q.W * w.Z) + (q.X * w.Y) - (q.Y * w.X),
            (-q.X * w.X) - (q.Y * w.Y) - (q.Z * w.Z));

    //      /**@brief diagonalizes this matrix by the Jacobi method.
    //       * @param rot stores the rotation from the coordinate system in which the matrix is diagonal to the original
    //       * coordinate system, i.e., old_this = rot * new_this * rot^T. 
    //       * @param threshold See iteration
    //       * @param iteration The iteration stops when all off-diagonal elements are less than the threshold multiplied 
    //       * by the sum of the absolute values of the diagonal, or when maxSteps have been executed. 
    //       * 
    //       * Note that this matrix is assumed to be symmetric. 
    //       */
    //        public static void Diagonalize(ref Matrix inMatrix,ref Matrix rot, float threshold, int maxSteps)
    //        {
    //            Debug.Assert(false);
    //            rot = Matrix.Identity;
    //            for (int step = maxSteps; step > 0; step--)
    //            {
    //                // find off-diagonal element [p][q] with largest magnitude
    //                int p = 0;
    //                int q = 1;
    //                int r = 2;
    //                float max = System.Math.Abs(inMatrix.M12);
    //                float v = System.Math.Abs(inMatrix.M13);
    //                if (v > max)
    //                {
    //                   q = 2;
    //                   r = 1;
    //                   max = v;
    //                }
    //                v = System.Math.Abs(inMatrix.M23);
    //                if (v > max)
    //                {
    //                   p = 1;
    //                   q = 2;
    //                   r = 0;
    //                   max = v;
    //                }

    //                float t = threshold * (System.Math.Abs(inMatrix.M11) + System.Math.Abs(inMatrix.M22) + System.Math.Abs(inMatrix.M33));
    //                if (max <= t)
    //                {
    //                   if (max <= SIMD_EPSILON * t)
    //                   {
    //                      return;
    //                   }
    //                   step = 1;
    //                }

    //                // compute Jacobi rotation J which leads to a zero for element [p][q] 
    //                float mpq = MathUtil.MatrixComponent(ref inMatrix,p,q);
    //                float theta = (MathUtil.MatrixComponent(ref inMatrix,q,q)-MathUtil.MatrixComponent(ref inMatrix,p,p)) / (2 * mpq);
    //                float theta2 = theta * theta;
    //                float cos;
    //                float sin;
    //                if (theta2 * theta2 < 10f / SIMD_EPSILON)
    //                {
    //                   t = (theta >= 0f) ? (float)(1f / (theta + System.Math.Sqrt(1 + theta2)))
    //                                            : (float)(1f / (theta - System.Math.Sqrt(1 + theta2)));
    //                   cos = (float)(1f / System.Math.Sqrt(1 + t * t));
    //                   sin = cos * t;
    //                }
    //                else
    //                {
    //                   // approximation for large theta-value, i.e., a nearly diagonal matrix
    //                   t = 1 / (theta * (2 + 0.5f / theta2));
    //                   cos = 1 - 0.5f * t * t;
    //                   sin = cos * t;
    //                }

    //                // apply rotation to matrix (this = J^T * this * J)
    //                MathUtil.MatrixComponent(ref inMatrix,p,q,0f);
    //                MathUtil.MatrixComponent(ref inMatrix,q,p,0f);
    //                MathUtil.MatrixComponent(ref inMatrix,p,p,MathUtil.MatrixComponent(ref inMatrix,p,p)-t*mpq);
    //                MathUtil.MatrixComponent(ref inMatrix,q,q,MathUtil.MatrixComponent(ref inMatrix,q,q)+t*mpq);

    //                float  mrp = MathUtil.MatrixComponent(ref inMatrix,r,p);
    //                float  mrq = MathUtil.MatrixComponent(ref inMatrix,r,q);

    //                MathUtil.MatrixComponent(ref inMatrix,r,p,cos * mrp - sin * mrq);
    //                MathUtil.MatrixComponent(ref inMatrix,p,r,cos * mrp - sin * mrq);

    //                MathUtil.MatrixComponent(ref inMatrix,r,q,cos * mrp + sin * mrq);
    //                MathUtil.MatrixComponent(ref inMatrix,q,r,cos * mrp + sin * mrq);

    //                // apply rotation to rot (rot = rot * J)
    //                for (int i = 0; i < 3; i++)
    //                {
    //                    float  mrp2 = MathUtil.MatrixComponent(ref rot,i,p);
    //                    float  mrq2 = MathUtil.MatrixComponent(ref rot,i,q);
    //                    MathUtil.MatrixComponent(ref rot, i, p, cos * mrp - sin * mrq);
    //                    MathUtil.MatrixComponent(ref rot, i, q, cos * mrp + sin * mrq);
    //                }
    //            }
    //        }

    public static void GetSkewSymmetricMatrix(ref Vector3 vecin, out Vector3 v0, out Vector3 v1, out Vector3 v2)
    {
        v0 = new Vector3(0f, -vecin.Z, vecin.Y);
        v1 = new Vector3(vecin.Z, 0f, -vecin.X);
        v2 = new Vector3(-vecin.Y, vecin.X, 0f);
    }

    [Conditional("DEBUG")]
    public static void ZeroCheckVector(ref Vector3 v)
    {
        if (FuzzyZero(v.LengthSquared()))
        {
            //Debug.Assert(false);
        }
    }

    [Conditional("DEBUG")]
    public static void SanityCheckVector(ref Vector3 v)
    {
        if (float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z))
        {
            Debug.Fail($"{nameof(v)} should not be NaN.");
        }
    }

    [Conditional("DEBUG")]
    public static void SanityCheckFloat(float f)
        => Debug.Assert(!float.IsInfinity(f) && !float.IsNaN(f), $"{nameof(f)} should not be infinity or NaN.");

    //        public static void Vector3FromFloat(out Vector3 v, float[] fa)
    //        {
    //            v = new Vector3(fa[0], fa[1], fa[2]);
    //        }

    //        //public static void FloatFromVector3(Vector3 v, float[] fa)
    //        //{
    //        //    FloatFromVector3(ref v, fa);
    //        //}

    //        //public static void FloatFromVector3(ref Vector3 v, float[] fa)
    //        //{
    //        //    fa[0] = v.X;
    //        //    fa[1] = v.Y;
    //        //    fa[2] = v.Z;
    //        //}

    //        //public static float[] FloatFromVector3(Vector3 v)
    //        //{
    //        //    return FloatFromVector3(ref v);
    //        //}

    //        //public static float[] FloatFromVector3(ref Vector3 v)
    //        //{
    //        //    return new float[] { v.X, v.Y, v.Z };
    //        //}
    /*
    public static bool MatrixToEulerXYZ(ref IndexedBasisMatrix mat, out Vector3 xyz)
    {
        //	// rot =  cy*cz          -cy*sz           sy
        //	//        cz*sx*sy+cx*sz  cx*cz-sx*sy*sz -cy*sx
        //	//       -cx*cz*sy+sx*sz  cz*sx+cx*sy*sz  cx*cy
        //

        float matElem0 = MathUtil.GetMatrixElem(ref mat, 0);
        float matElem1 = MathUtil.GetMatrixElem(ref mat, 1);
        float matElem2 = MathUtil.GetMatrixElem(ref mat, 2);
        float matElem3 = MathUtil.GetMatrixElem(ref mat, 3);
        float matElem4 = MathUtil.GetMatrixElem(ref mat, 4);
        float matElem5 = MathUtil.GetMatrixElem(ref mat, 5);
        float matElem6 = MathUtil.GetMatrixElem(ref mat, 6);
        float matElem7 = MathUtil.GetMatrixElem(ref mat, 7);
        float matElem8 = MathUtil.GetMatrixElem(ref mat, 8);

        float fi = matElem2;
        if (fi < 1.0f)
        {
            if (fi > -1.0f)
            {
                xyz = new Vector3(
                    (float)Math.Atan2(-matElem5, matElem8),
                    (float)Math.Asin(matElem2),
                    (float)Math.Atan2(-matElem1, matElem0));
                return true;
            }
            else
            {
                // WARNING.  Not unique.  XA - ZA = -atan2(r10,r11)
                xyz = new Vector3(
                    (float)-Math.Atan2(matElem3, matElem4),
                    -SIMD_HALF_PI,
                    0f);
                return false;
            }
        }
        else
        {
            // WARNING.  Not unique.  XAngle + ZAngle = atan2(r10,r11)
            xyz = new Vector3(
                (float)Math.Atan2(matElem3, matElem4),
                SIMD_HALF_PI,
                0.0f);
        }
        return false;
    }
    */

    //        public static Vector3 MatrixToEuler(ref Matrix m)
    //        {
    //            Vector3 translate;
    //            Vector3 scale;
    //            Quaternion rotate;
    //            m.Decompose(out scale, out rotate, out translate);
    //            return quaternionToEuler(ref rotate);
    //        }

    //        // Taken from Fabian Vikings post at : http://forums.xna.com/forums/p/4574/23763.aspx  
    //        public static Vector3 quaternionToEuler(ref Quaternion q)
    //        {
    //            Vector3 v = Vector3.Zero;

    //            v.X = (float)Math.Atan2
    //            (
    //                2 * q.Y * q.W - 2 * q.X * q.Z,
    //                   1 - 2 * Math.Pow(q.Y, 2) - 2 * Math.Pow(q.Z, 2)
    //            );

    //            v.Z = (float)Math.Asin
    //            (
    //                2 * q.X * q.Y + 2 * q.Z * q.W
    //            );

    //            v.Y = (float)Math.Atan2
    //            (
    //                2 * q.X * q.W - 2 * q.Y * q.Z,
    //                1 - 2 * Math.Pow(q.X, 2) - 2 * Math.Pow(q.Z, 2)
    //            );

    //            if (q.X * q.Y + q.Z * q.W == 0.5)
    //            {
    //                v.X = (float)(2 * Math.Atan2(q.X, q.W));
    //                v.Y = 0;
    //            }

    //            else if (q.X * q.Y + q.Z * q.W == -0.5)
    //            {
    //                v.X = (float)(-2 * Math.Atan2(q.X, q.W));
    //                v.Y = 0;
    //            }

    //            return v;
    //        }

    public static Quaternion QuaternionInverse(ref Quaternion q)
        => new Quaternion(-q.X, -q.Y, -q.Z, q.W);

    //public static Matrix BulletMatrixMultiply(Matrix m1, Matrix m2)
    //{
    //    return m1 * m2;
    //}

    //public static Matrix BulletMatrixMultiply(ref Matrix m1, ref Matrix m2)
    //{
    //    return m1 * m2;
    //}

    //        public static Matrix BulletMatrixMultiplyBasis(Matrix m1, Matrix m2)
    //        {
    //            return BulletMatrixMultiplyBasis(ref m1, ref m2);
    //        }

    //        public static Matrix BulletMatrixMultiplyBasis(ref Matrix m1, ref Matrix m2)
    //        {
    //            Matrix mb1;
    //            BasisMatrix(ref m1, out mb1);
    //            Matrix mb2;
    //            BasisMatrix(ref m2, out mb2);
    //            return BulletMatrixMultiply(ref mb1, ref mb2);
    //        }

    public static float NormalizeAngle(float angleInRadians)
    {
        // Need to check this mod operator works with floats...
        angleInRadians = angleInRadians % SIMD_2_PI;
        return angleInRadians < -SIMD_PI
            ? angleInRadians + SIMD_2_PI
            : angleInRadians > SIMD_PI ? angleInRadians - SIMD_2_PI : angleInRadians;
    }

    public static float DegToRadians(float degrees)
        => degrees / 360.0f * SIMD_2_PI;

    /*
    public static Matrix SetEulerZYX(float eulerX, float eulerY, float eulerZ)
    {
        //return Matrix.CreateFromYawPitchRoll(y, x,z);
        // This version tested and compared to c++ version. don't break it.
        // note that the row/column settings are switched from c++
        Matrix m = Matrix.Identity;
        m._basis.SetEulerZYX(eulerX, eulerY, eulerZ);
        return m;

    }

    //        public static Vector3 MatrixToVector(Matrix m, Vector3 v)
    //        {
    //            return new Vector3(
    //                Vector3.Dot(new Vector3(m.M11, m.M12, m.M13), v) + m._origin.X,
    //                Vector3.Dot(new Vector3(m.M21, m.M22, m.M23), v) + m._origin.Y,
    //                Vector3.Dot(new Vector3(m.M31, m.M32, m.M33), v) + m._origin.Z
    //                );
    //        }
    */

    public static Vector3 Vector4ToVector3(ref Vector4 v4)
        => new Vector3(v4.X, v4.Y, v4.Z);

    //        public static Vector3 TransposeTransformNormal(Vector3 v,Matrix m)
    //        {
    //            return TransposeTransformNormal(ref v, ref m);
    //        }

    //        public static Vector3 TransposeTransformNormal(ref Vector3 v,ref Matrix m)
    //        {
    //            Matrix mt = TransposeBasis(ref m);
    //            return Vector3.TransformNormal(v, mt);
    //        }

    //        //public static Vector3 TransposeTransformNormal(ref Vector3 v, ref Matrix m)
    //        //{
    //        //    Matrix mt = TransposeBasis(ref m);
    //        //    return Vector3.TransformNormal(ref v, ref mt);
    //        //}

    public static bool IsAlmostZero(ref Vector3 v)
        => MathF.Abs(v.X) <= 1e-6 && MathF.Abs(v.Y) <= 1e-6 && MathF.Abs(v.Z) <= 1e-6;

    public static float Vector3Distance2XZ(Vector3 x, Vector3 y)
    {
        Vector3 xa = new Vector3(x.X, 0, x.Z);
        Vector3 ya = new Vector3(y.X, 0, y.Z);
        return (xa - ya).LengthSquared();
    }

    public static void SetRotation(this Matrix4x4 matrix, Quaternion newRotation, out Matrix4x4 result)
    {
        Matrix4x4.Decompose(matrix, out Vector3 scale, out _, out Vector3 translation);
        result = Matrix4x4.CreateScale(scale) * Matrix4x4.CreateFromQuaternion(newRotation) * Matrix4x4.CreateTranslation(translation);
    }

    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
        Matrix4x4.Decompose(matrix, out _, out Quaternion rot, out _);
        return rot;
    }

    public static Vector3 GetColumn(this Matrix4x4 matrix, int column)
        => new Vector3(matrix.GetComponent(0, column), matrix.GetComponent(1, column), matrix.GetComponent(2, column));

    public static Vector3 GetRow(this Matrix4x4 matrix, int row)
        => new Vector3(matrix.GetComponent(row, 0), matrix.GetComponent(row, 1), matrix.GetComponent(row, 2));

    /// <summary>
    /// Gets the component at the specified index.
    /// </summary>
    /// <param name="vector">The vector to get the component of.</param>
    /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, and 2 for the Z component.</param>
    /// <returns>The value of the component at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
    public static float GetComponent(this Vector3 vector, int index)
        => index switch
        {
            0 => vector.X,
            1 => vector.Y,
            2 => vector.Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector3 run from 0 to 2, inclusive."),
        };

    /// <summary>
    /// Sets the component at the specified index.
    /// </summary>
    /// <value>The value of the X, Y, or Z component, depending on the index.</value>
    /// <param name="vector">The vector to set the component of.</param>
    /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, and 2 for the Z component.</param>
    /// <param name="value">The new value of the component.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
    public static void SetComponent(ref this Vector3 vector, int index, float value)
    {
        switch (index)
        {
            case 0: vector.X = value; break;
            case 1: vector.Y = value; break;
            case 2: vector.Z = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector3 run from 0 to 2, inclusive.");
        }
    }

    public static Matrix4x4 GetBasis(this Matrix4x4 matrix)
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
        => new Matrix4x4(
            matrix.M11, matrix.M12, matrix.M13, 0,
            matrix.M21, matrix.M22, matrix.M23, 0,
            matrix.M31, matrix.M32, matrix.M33, 0,
            0, 0, 0, 1);
#pragma warning restore SA1117 // Parameters should be on same line or separate lines

    public static void SetBasis(ref this Matrix4x4 matrix, Matrix4x4 basis)
    {
        matrix.M11 = basis.M11;
        matrix.M12 = basis.M12;
        matrix.M13 = basis.M13;
        matrix.M21 = basis.M21;
        matrix.M22 = basis.M22;
        matrix.M23 = basis.M23;
        matrix.M31 = basis.M31;
        matrix.M32 = basis.M32;
        matrix.M33 = basis.M33;
    }

    public static float GetComponent(ref this Matrix4x4 matrix, int index)
        => index switch
        {
            0 => matrix.M11,
            1 => matrix.M12,
            2 => matrix.M13,
            3 => matrix.M14,
            4 => matrix.M21,
            5 => matrix.M22,
            6 => matrix.M23,
            7 => matrix.M24,
            8 => matrix.M31,
            9 => matrix.M32,
            10 => matrix.M33,
            11 => matrix.M34,
            12 => matrix.M41,
            13 => matrix.M42,
            14 => matrix.M43,
            15 => matrix.M44,
            _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Matrix run from 0 to 15, inclusive."),
        };

    public static void SetComponent(ref this Matrix4x4 matrix, int index, float value)
    {
        switch (index)
        {
            case 0: matrix.M11 = value; break;
            case 1: matrix.M12 = value; break;
            case 2: matrix.M13 = value; break;
            case 3: matrix.M14 = value; break;
            case 4: matrix.M21 = value; break;
            case 5: matrix.M22 = value; break;
            case 6: matrix.M23 = value; break;
            case 7: matrix.M24 = value; break;
            case 8: matrix.M31 = value; break;
            case 9: matrix.M32 = value; break;
            case 10: matrix.M33 = value; break;
            case 11: matrix.M34 = value; break;
            case 12: matrix.M41 = value; break;
            case 13: matrix.M42 = value; break;
            case 14: matrix.M43 = value; break;
            case 15: matrix.M44 = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Matrix run from 0 to 15, inclusive.");
        }
    }

    public static float GetComponent(this Matrix4x4 matrix, int row, int column)
        => row < 0 || row > 3
            ? throw new ArgumentOutOfRangeException(nameof(row), "Rows and columns for matrices run from 0 to 3, inclusive.")
            : column < 0 || column > 3
            ? throw new ArgumentOutOfRangeException(nameof(column), "Rows and columns for matrices run from 0 to 3, inclusive.")
            : matrix.GetComponent((row * 4) + column);

    public static void SetComponent(ref this Matrix4x4 matrix, int row, int column, float value)
    {
        if (row < 0 || row > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "Rows and columns for matrices run from 0 to 3, inclusive.");
        }

        if (column < 0 || column > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(column), "Rows and columns for matrices run from 0 to 3, inclusive.");
        }

        matrix.SetComponent((row * 4) + column, value);
    }

    public static Matrix4x4 MakeMatrix4x4(ReadOnlySpan<float> values)
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
        => values == null
            ? throw new ArgumentNullException(nameof(values))
            : values.Length != 16
            ? throw new ArgumentOutOfRangeException(nameof(values), "There must be sixteen and only sixteen input values for a Matrix4x4.")
            : new Matrix4x4(values[0], values[1], values[2], values[3],
                             values[4], values[5], values[6], values[7],
                             values[8], values[9], values[10], values[11],
                             values[12], values[13], values[14], values[15]);
#pragma warning restore SA1117 // Parameters should be on same line or separate lines

    public static T Clamp<T>(T value, T min, T max)
     where T : IComparable<T>
    {
        T result = value;
        if (value.CompareTo(max) > 0)
        {
            result = max;
        }

        if (value.CompareTo(min) < 0)
        {
            result = min;
        }

        return result;
    }
}

[StructLayout(LayoutKind.Sequential)]
#pragma warning disable SA1201 // Elements should appear in the correct order
internal struct Matrix3x3FloatData
#pragma warning restore SA1201 // Elements should appear in the correct order
{
    public Vector3FloatData Element0;
    public Vector3FloatData Element1;
    public Vector3FloatData Element2;
}

[StructLayout(LayoutKind.Sequential)]
internal struct Matrix3x3DoubleData
{
    public Vector3DoubleData Element0;
    public Vector3DoubleData Element1;
    public Vector3DoubleData Element2;
}

[StructLayout(LayoutKind.Sequential)]
internal struct TransformFloatData
{
    public Matrix3x3FloatData Basis;
    public Vector3FloatData Origin;

#pragma warning disable SA1203 // Constants should appear before fields
    public const int OriginOffset = 48;
#pragma warning restore SA1203 // Constants should appear before fields
}

[StructLayout(LayoutKind.Sequential)]
internal struct TransformDoubleData
{
    public Matrix3x3DoubleData Basis;
    public Vector3DoubleData Origin;

#pragma warning disable SA1203 // Constants should appear before fields
    public const int OriginOffset = 96;
#pragma warning restore SA1203 // Constants should appear before fields
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Vector3FloatData
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
    public float[] floats;
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Vector3DoubleData
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
    public double[] floats;
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
}
