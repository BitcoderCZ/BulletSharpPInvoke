using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

[Flags]
public enum RigidBodyFlags
{
    None = 0,
    DisableWorldGravity = 1,
    EnableGyroscopicForceExplicit = 2,
    EnableGyroscopicForceImplicitWorld = 4,
    EnableGyroscopicForceImplicitBody = 8,
    EnableGyroscopicForce = EnableGyroscopicForceImplicitBody,
}

[StructLayout(LayoutKind.Sequential)]
internal struct RigidBodyFloatData
{
    public CollisionObjectFloatData CollisionObjectData;
    public Matrix3x3FloatData InvInertiaTensorWorld;
    public Vector3FloatData LinearVelocity;
    public Vector3FloatData AngularVelocity;
    public Vector3FloatData AngularFactor;
    public Vector3FloatData LinearFactor;
    public Vector3FloatData Gravity;
    public Vector3FloatData GravityAcceleration;
    public Vector3FloatData InvInertiaLocal;
    public Vector3FloatData TotalForce;
    public Vector3FloatData TotalTorque;
    public float InverseMass;
    public float LinearDamping;
    public float AngularDamping;
    public float AdditionalDampingFactor;
    public float AdditionalLinearDampingThresholdSqr;
    public float AdditionalAngularDampingThresholdSqr;
    public float AdditionalAngularDampingFactor;
    public float LinearSleepingThreshold;
    public float AngularSleepingThreshold;
    public int AdditionalDamping;
    //public int Padding;

    public static int Offset(string fieldName)
        => Marshal.OffsetOf(typeof(RigidBodyFloatData), fieldName).ToInt32();
}

[StructLayout(LayoutKind.Sequential)]
internal struct RigidBodyDoubleData
{
    public CollisionObjectDoubleData CollisionObjectData;
    public Matrix3x3DoubleData InvInertiaTensorWorld;
    public Vector3DoubleData LinearVelocity;
    public Vector3DoubleData AngularVelocity;
    public Vector3DoubleData AngularFactor;
    public Vector3DoubleData LinearFactor;
    public Vector3DoubleData Gravity;
    public Vector3DoubleData GravityAcceleration;
    public Vector3DoubleData InvInertiaLocal;
    public Vector3DoubleData TotalForce;
    public Vector3DoubleData TotalTorque;
    public double InverseMass;
    public double LinearDamping;
    public double AngularDamping;
    public double AdditionalDampingFactor;
    public double AdditionalLinearDampingThresholdSqr;
    public double AdditionalAngularDampingThresholdSqr;
    public double AdditionalAngularDampingFactor;
    public double LinearSleepingThreshold;
    public double AngularSleepingThreshold;
    public int AdditionalDamping;
    //public int Padding;

    public static int Offset(string fieldName)
        => Marshal.OffsetOf(typeof(RigidBodyDoubleData), fieldName).ToInt32();
}

public class RigidBody : CollisionObject
{
    internal List<TypedConstraint>? _constraintRefs;
    private MotionState? _motionState;

    public RigidBody(RigidBodyConstructionInfo constructionInfo)
        : base(ConstructionInfo.Null)
    {
        IntPtr native = btRigidBody_new(constructionInfo.Native);
        InitializeCollisionObject(native);

        _collisionShape = constructionInfo.CollisionShape;
        _motionState = constructionInfo.MotionState;
    }

    internal RigidBody(IntPtr native)
        : base(ConstructionInfo.Null)
    {
        InitializeSubObject(native, this);
    }

    public float AngularDamping => btRigidBody_getAngularDamping(Native);

    public Vector3 AngularFactor
    {
        get
        {
            Vector3 value;
            btRigidBody_getAngularFactor(Native, out value);
            return value;
        }
        set => btRigidBody_setAngularFactor(Native, ref value);
    }

    public float AngularSleepingThreshold => btRigidBody_getAngularSleepingThreshold(Native);

    public Vector3 AngularVelocity
    {
        get
        {
            Vector3 value;
            btRigidBody_getAngularVelocity(Native, out value);
            return value;
        }
        set => btRigidBody_setAngularVelocity(Native, ref value);
    }

    public BroadphaseProxy? BroadphaseProxy => BroadphaseProxy.GetManaged(btRigidBody_getBroadphaseProxy(Native));

    public Vector3 CenterOfMassPosition
    {
        get
        {
            Vector3 value;
            btRigidBody_getCenterOfMassPosition(Native, out value);
            return value;
        }
    }

    public Matrix4x4 CenterOfMassTransform
    {
        get
        {
            Matrix4x4 value;
            btRigidBody_getCenterOfMassTransform(Native, out value);
            return value;
        }
        set => btRigidBody_setCenterOfMassTransform(Native, ref value);
    }

    public int ContactSolverType
    {
        get => btRigidBody_getContactSolverType(Native);
        set => btRigidBody_setContactSolverType(Native, value);
    }

    public RigidBodyFlags Flags
    {
        get => btRigidBody_getFlags(Native);
        set => btRigidBody_setFlags(Native, value);
    }

    public int FrictionSolverType
    {
        get => btRigidBody_getFrictionSolverType(Native);
        set => btRigidBody_setFrictionSolverType(Native, value);
    }

    public Vector3 Gravity
    {
        get
        {
            Vector3 value;
            btRigidBody_getGravity(Native, out value);
            return value;
        }
        set => btRigidBody_setGravity(Native, ref value);
    }

    public Vector3 InvInertiaDiagLocal
    {
        get
        {
            Vector3 value;
            btRigidBody_getInvInertiaDiagLocal(Native, out value);
            return value;
        }
        set => btRigidBody_setInvInertiaDiagLocal(Native, ref value);
    }

    public Matrix4x4 InvInertiaTensorWorld
    {
        get
        {
            Matrix4x4 value;
            btRigidBody_getInvInertiaTensorWorld(Native, out value);
            return value;
        }
    }

    public float InvMass => btRigidBody_getInvMass(Native);

    public bool IsInWorld => btRigidBody_isInWorld(Native);

    public float LinearDamping => btRigidBody_getLinearDamping(Native);

    public Vector3 LinearFactor
    {
        get
        {
            Vector3 value;
            btRigidBody_getLinearFactor(Native, out value);
            return value;
        }
        set => btRigidBody_setLinearFactor(Native, ref value);
    }

    public float LinearSleepingThreshold => btRigidBody_getLinearSleepingThreshold(Native);

    public Vector3 LinearVelocity
    {
        get
        {
            Vector3 value;
            btRigidBody_getLinearVelocity(Native, out value);
            return value;
        }
        set => btRigidBody_setLinearVelocity(Native, ref value);
    }

    public Vector3 LocalInertia
    {
        get
        {
            Vector3 value;
            btRigidBody_getLocalInertia(Native, out value);
            return value;
        }
    }

    public MotionState? MotionState
    {
        get => _motionState;
        set
        {
            btRigidBody_setMotionState(Native, (value != null) ? value.Native : IntPtr.Zero);
            _motionState = value;
        }
    }

    public int NumConstraintRefs => (_constraintRefs != null) ? _constraintRefs.Count : 0;

    public Quaternion Orientation
    {
        get
        {
            Quaternion value;
            btRigidBody_getOrientation(Native, out value);
            return value;
        }
    }

    public Vector3 TotalForce
    {
        get
        {
            Vector3 value;
            btRigidBody_getTotalForce(Native, out value);
            return value;
        }
    }

    public Vector3 TotalTorque
    {
        get
        {
            Vector3 value;
            btRigidBody_getTotalTorque(Native, out value);
            return value;
        }
    }

    public static RigidBody? Upcast(CollisionObject colObj)
        => GetManaged(btRigidBody_upcast(colObj.Native)) as RigidBody;

    public void AddConstraintRef(TypedConstraint constraint)
    {
        if (_constraintRefs == null)
        {
            _constraintRefs = [];
        }

        _constraintRefs.Add(constraint);
        btRigidBody_addConstraintRef(Native, constraint.Native);
    }

    public void ApplyCentralForceRef(ref Vector3 force)
        => btRigidBody_applyCentralForce(Native, ref force);

    public void ApplyCentralForce(Vector3 force)
        => btRigidBody_applyCentralForce(Native, ref force);

    public void ApplyCentralImpulseRef(ref Vector3 impulse)
        => btRigidBody_applyCentralImpulse(Native, ref impulse);

    public void ApplyCentralImpulse(Vector3 impulse)
        => btRigidBody_applyCentralImpulse(Native, ref impulse);

    public void ApplyDamping(float timeStep)
        => btRigidBody_applyDamping(Native, timeStep);

    public void ApplyForceRef(ref Vector3 force, ref Vector3 relPos)
        => btRigidBody_applyForce(Native, ref force, ref relPos);

    public void ApplyForce(Vector3 force, Vector3 relPos)
        => btRigidBody_applyForce(Native, ref force, ref relPos);

    public void ApplyGravity()
        => btRigidBody_applyGravity(Native);

    public void ApplyImpulseRef(ref Vector3 impulse, ref Vector3 relPos)
        => btRigidBody_applyImpulse(Native, ref impulse, ref relPos);

    public void ApplyImpulse(Vector3 impulse, Vector3 relPos)
        => btRigidBody_applyImpulse(Native, ref impulse, ref relPos);

    public void ApplyTorqueRef(ref Vector3 torque)
        => btRigidBody_applyTorque(Native, ref torque);

    public void ApplyTorque(Vector3 torque)
        => btRigidBody_applyTorque(Native, ref torque);

    public void ApplyTorqueImpulseRef(ref Vector3 torque)
        => btRigidBody_applyTorqueImpulse(Native, ref torque);

    public void ApplyTorqueImpulse(Vector3 torque)
        => btRigidBody_applyTorqueImpulse(Native, ref torque);

    public void ClearForces()
        => btRigidBody_clearForces(Native);

    public void ClearGravity()
        => btRigidBody_clearGravity(Native);

    public void ComputeAngularImpulseDenominator(ref Vector3 axis, out float result)
        => result = btRigidBody_computeAngularImpulseDenominator(Native, ref axis);

    public float ComputeAngularImpulseDenominator(Vector3 axis)
        => btRigidBody_computeAngularImpulseDenominator(Native, ref axis);

    public Vector3 ComputeGyroscopicForceExplicit(float maxGyroscopicForce)
    {
        Vector3 value;
        btRigidBody_computeGyroscopicForceExplicit(Native, maxGyroscopicForce, out value);
        return value;
    }

    public Vector3 ComputeGyroscopicImpulseImplicitBody(float step)
    {
        Vector3 value;
        btRigidBody_computeGyroscopicImpulseImplicit_Body(Native, step, out value);
        return value;
    }

    public Vector3 ComputeGyroscopicImpulseImplicitWorld(float deltaTime)
    {
        Vector3 value;
        btRigidBody_computeGyroscopicImpulseImplicit_World(Native, deltaTime, out value);
        return value;
    }

    public float ComputeImpulseDenominator(Vector3 pos, Vector3 normal)
        => btRigidBody_computeImpulseDenominator(Native, ref pos, ref normal);

    public void GetAabb(out Vector3 aabbMin, out Vector3 aabbMax)
        => btRigidBody_getAabb(Native, out aabbMin, out aabbMax);

    public TypedConstraint GetConstraintRef(int index)
    {
        System.Diagnostics.Debug.Assert(_constraintRefs is not null, $"{nameof(_constraintRefs)} should not be null.");
        return _constraintRefs[index];
    }

    public void GetVelocityInLocalPoint(ref Vector3 relPos, out Vector3 value)
        => btRigidBody_getVelocityInLocalPoint(Native, ref relPos, out value);

    public Vector3 GetVelocityInLocalPoint(Vector3 relPos)
    {
        Vector3 value;
        btRigidBody_getVelocityInLocalPoint(Native, ref relPos, out value);
        return value;
    }

    public void IntegrateVelocities(float step)
        => btRigidBody_integrateVelocities(Native, step);

    public void PredictIntegratedTransform(float step, out Matrix4x4 predictedTransform)
        => btRigidBody_predictIntegratedTransform(Native, step, out predictedTransform);

    public void ProceedToTransformRef(ref Matrix4x4 newTrans)
        => btRigidBody_proceedToTransform(Native, ref newTrans);

    public void ProceedToTransform(Matrix4x4 newTrans)
        => btRigidBody_proceedToTransform(Native, ref newTrans);

    public void RemoveConstraintRef(TypedConstraint constraint)
    {
        if (_constraintRefs != null)
        {
            _constraintRefs.Remove(constraint);
            btRigidBody_removeConstraintRef(Native, constraint.Native);
        }
    }

    public void SaveKinematicState(float step)
        => btRigidBody_saveKinematicState(Native, step);

    public void SetDamping(float linDamping, float angDamping)
        => btRigidBody_setDamping(Native, linDamping, angDamping);

    public void SetMassPropsRef(float mass, ref Vector3 inertia)
        => btRigidBody_setMassProps(Native, mass, ref inertia);

    public void SetMassProps(float mass, Vector3 inertia)
        => btRigidBody_setMassProps(Native, mass, ref inertia);

    public void SetNewBroadphaseProxy(BroadphaseProxy broadphaseProxy)
        => btRigidBody_setNewBroadphaseProxy(Native, broadphaseProxy.Native);

    public void SetSleepingThresholds(float linear, float angular)
        => btRigidBody_setSleepingThresholds(Native, linear, angular);

    public void TranslateRef(ref Vector3 v)
        => btRigidBody_translate(Native, ref v);

    public void Translate(Vector3 v)
        => btRigidBody_translate(Native, ref v);

    public void UpdateDeactivation(float timeStep)
        => btRigidBody_updateDeactivation(Native, timeStep);

    public void UpdateInertiaTensor()
        => btRigidBody_updateInertiaTensor(Native);

    public bool WantsSleeping()
        => btRigidBody_wantsSleeping(Native);
}
