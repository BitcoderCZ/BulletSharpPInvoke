#define ROLLING_INFLUENCE_FIX

using System;
using System.Diagnostics;
using System.Numerics;

namespace BulletSharp;

public class VehicleTuning
{
    public float SuspensionStiffness;
    public float SuspensionCompression;
    public float SuspensionDamping;
    public float MaxSuspensionTravelCm;
    public float FrictionSlip;
    public float MaxSuspensionForce;

    public VehicleTuning()
    {
        SuspensionStiffness = 5.88f;
        SuspensionCompression = 0.83f;
        SuspensionDamping = 0.88f;
        MaxSuspensionTravelCm = 500.0f;
        FrictionSlip = 10.5f;
        MaxSuspensionForce = 6000.0f;
    }
}

public class RaycastVehicle : IAction
{
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SX1309 // Field names should begin with underscore
    private WheelInfo[] wheelInfo = [];
    private Vector3[] forwardWS = [];
    private Vector3[] axle = [];
    private float[] forwardImpulse = [];
    private float[] sideImpulse = [];

    /*if (RigidBody.MotionState != null)
{
return RigidBody.MotionState.WorldTransform;
}*/
    public Matrix4x4 ChassisWorldTransform => RigidBody.CenterOfMassTransform;

    private float currentVehicleSpeedKmHour;

    public int NumWheels => wheelInfo.Length;

    private int indexRightAxis = 0;

    public int RightAxis => indexRightAxis;

    private int indexUpAxis = 2;
    private int indexForwardAxis = 1;
#pragma warning disable SA1214 // Readonly fields should appear before non-readonly fields
    private readonly RigidBody chassisBody;
#pragma warning restore SA1214 // Readonly fields should appear before non-readonly fields

    public RigidBody RigidBody => chassisBody;

    private readonly IVehicleRaycaster vehicleRaycaster;
#pragma warning disable SA1204 // Static elements should appear before instance elements
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
    private static readonly RigidBody fixedBody;
#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1204 // Static elements should appear before instance elements
#pragma warning restore SX1309 // Field names should begin with underscore
#pragma warning restore SA1201 // Elements should appear in the correct order

    static RaycastVehicle()
    {
        using (RigidBodyConstructionInfo ci = new RigidBodyConstructionInfo(0, null, null))
        {
            fixedBody = new RigidBody(ci);
            fixedBody.SetMassProps(0, Vector3.Zero);
        }
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public RaycastVehicle(VehicleTuning tuning, RigidBody chassis, IVehicleRaycaster raycaster)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        chassisBody = chassis;
        vehicleRaycaster = raycaster;
    }

    public void SetBrake(float brake, int wheelIndex)
    {
        Debug.Assert((wheelIndex >= 0) && (wheelIndex < NumWheels));
        GetWheelInfo(wheelIndex).Brake = brake;
    }

    public float GetSteeringValue(int wheel)
        => GetWheelInfo(wheel).Steering;

    public void SetSteeringValue(float steering, int wheel)
    {
        Debug.Assert(wheel >= 0 && wheel < NumWheels);

        WheelInfo wheelInfo = GetWheelInfo(wheel);
        wheelInfo.Steering = steering;
    }

    public void SetCoordinateSystem(int rightIndex, int upIndex, int forwardIndex)
    {
        indexRightAxis = rightIndex;
        indexUpAxis = upIndex;
        indexForwardAxis = forwardIndex;
    }

    public Matrix4x4 GetWheelTransformWS(int wheelIndex)
    {
        Debug.Assert(wheelIndex < NumWheels);
        return wheelInfo[wheelIndex].WorldTransform;
    }

    public WheelInfo AddWheel(Vector3 connectionPointCS, Vector3 wheelDirectionCS0, Vector3 wheelAxleCS, float suspensionRestLength, float wheelRadius, VehicleTuning tuning, bool isFrontWheel)
    {
        WheelInfoConstructionInfo ci = new WheelInfoConstructionInfo()
        {
            ChassisConnectionCS = connectionPointCS,
            WheelDirectionCS = wheelDirectionCS0,
            WheelAxleCS = wheelAxleCS,
            SuspensionRestLength = suspensionRestLength,
            WheelRadius = wheelRadius,
            IsFrontWheel = isFrontWheel,
            SuspensionStiffness = tuning.SuspensionStiffness,
            WheelsDampingCompression = tuning.SuspensionCompression,
            WheelsDampingRelaxation = tuning.SuspensionDamping,
            FrictionSlip = tuning.FrictionSlip,
            MaxSuspensionTravelCm = tuning.MaxSuspensionTravelCm,
            MaxSuspensionForce = tuning.MaxSuspensionForce,
        };

        Array.Resize(ref wheelInfo, wheelInfo.Length + 1);
        WheelInfo wheel = new WheelInfo(ci);
        wheelInfo[^1] = wheel;

        UpdateWheelTransformsWS(wheel, false);
        UpdateWheelTransform(NumWheels - 1, false);
        return wheel;
    }

    public void ApplyEngineForce(float force, int wheel)
    {
        Debug.Assert(wheel >= 0 && wheel < NumWheels);
        WheelInfo wheelInfo = GetWheelInfo(wheel);
        wheelInfo.EngineForce = force;
    }

    private float CalcRollingFriction(RigidBody body0, RigidBody body1, Vector3 contactPosWorld, Vector3 frictionDirectionWorld, float maxImpulse)
    {
        float denom0 = body0.ComputeImpulseDenominator(contactPosWorld, frictionDirectionWorld);
        float denom1 = body1.ComputeImpulseDenominator(contactPosWorld, frictionDirectionWorld);
        const float relaxation = 1.0f;
        float jacDiagABInv = relaxation / (denom0 + denom1);

        float j1;

        Vector3 rel_pos1 = contactPosWorld - body0.CenterOfMassPosition;
        Vector3 rel_pos2 = contactPosWorld - body1.CenterOfMassPosition;

        Vector3 vel1 = body0.GetVelocityInLocalPoint(rel_pos1);
        Vector3 vel2 = body1.GetVelocityInLocalPoint(rel_pos2);
        Vector3 vel = vel1 - vel2;

        float vrel = Vector3.Dot(frictionDirectionWorld, vel);

        // calculate j that moves us to zero relative velocity
        j1 = -vrel * jacDiagABInv;
        j1 = MathF.Min(j1, maxImpulse);
        j1 = MathF.Max(j1, -maxImpulse);

        return j1;
    }

#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SX1309 // Field names should begin with underscore
    private Vector3 blue = new Vector3(0, 0, 1);
    private Vector3 magenta = new Vector3(1, 0, 1);
#pragma warning restore SX1309 // Field names should begin with underscore
#pragma warning restore SA1201 // Elements should appear in the correct order

    public void DebugDraw(DebugDraw debugDrawer)
    {
        for (int v = 0; v < NumWheels; v++)
        {
            WheelInfo wheelInfo = GetWheelInfo(v);

            Vector3 wheelColor;
            if (wheelInfo.RaycastInfo.IsInContact)
            {
                wheelColor = blue;
            }
            else
            {
                wheelColor = magenta;
            }

            Matrix4x4 transform = wheelInfo.WorldTransform;
            Vector3 wheelPosWS = transform.Translation;

            Vector3 axle = new Vector3(
                transform.GetComponent(0, RightAxis),
                transform.GetComponent(1, RightAxis),
                transform.GetComponent(2, RightAxis));

            Vector3 to1 = wheelPosWS + axle;
            Vector3 to2 = GetWheelInfo(v).RaycastInfo.ContactPointWS;

            //debug wheels (cylinders)
            debugDrawer.DrawLine(ref wheelPosWS, ref to1, ref wheelColor);
            debugDrawer.DrawLine(ref wheelPosWS, ref to2, ref wheelColor);
        }
    }

    public WheelInfo GetWheelInfo(int index)
    {
        Debug.Assert((index >= 0) && (index < NumWheels));

        return wheelInfo[index];
    }

    private float RayCast(WheelInfo wheel)
    {
        UpdateWheelTransformsWS(wheel, false);

        float depth = -1;
        float raylen = wheel.SuspensionRestLength + wheel.WheelsRadius;

        Vector3 rayvector = wheel.RaycastInfo.WheelDirectionWS * raylen;
        Vector3 source = wheel.RaycastInfo.HardPointWS;
        wheel.RaycastInfo.ContactPointWS = source + rayvector;
        Vector3 target = wheel.RaycastInfo.ContactPointWS;

        float param;
        VehicleRaycasterResult rayResults = new VehicleRaycasterResult();

        Debug.Assert(vehicleRaycaster is not null, $"{nameof(vehicleRaycaster)} should not be null.");
        object? obj = vehicleRaycaster.CastRay(ref source, ref target, rayResults);

        wheel.RaycastInfo.GroundObject = null;

        if (obj != null)
        {
            param = rayResults.DistFraction;
            depth = raylen * rayResults.DistFraction;
            wheel.RaycastInfo.ContactNormalWS = rayResults.HitNormalInWorld;
            wheel.RaycastInfo.IsInContact = true;

            wheel.RaycastInfo.GroundObject = fixedBody;//@todo for driving on dynamic/movable objects!;
            /////wheel.RaycastInfo.GroundObject = object;

            float hitDistance = param * raylen;
            wheel.RaycastInfo.SuspensionLength = hitDistance - wheel.WheelsRadius;
            //clamp on max suspension travel

            float minSuspensionLength = wheel.SuspensionRestLength - (wheel.MaxSuspensionTravelCm * 0.01f);
            float maxSuspensionLength = wheel.SuspensionRestLength + (wheel.MaxSuspensionTravelCm * 0.01f);
            if (wheel.RaycastInfo.SuspensionLength < minSuspensionLength)
            {
                wheel.RaycastInfo.SuspensionLength = minSuspensionLength;
            }

            if (wheel.RaycastInfo.SuspensionLength > maxSuspensionLength)
            {
                wheel.RaycastInfo.SuspensionLength = maxSuspensionLength;
            }

            wheel.RaycastInfo.ContactPointWS = rayResults.HitPointInWorld;

            float denominator = Vector3.Dot(wheel.RaycastInfo.ContactNormalWS, wheel.RaycastInfo.WheelDirectionWS);

            Vector3 chassis_velocity_at_contactPoint;
            Vector3 relpos = wheel.RaycastInfo.ContactPointWS - RigidBody.CenterOfMassPosition;

            chassis_velocity_at_contactPoint = RigidBody.GetVelocityInLocalPoint(relpos);

            float projVel = Vector3.Dot(wheel.RaycastInfo.ContactNormalWS, chassis_velocity_at_contactPoint);

            if (denominator >= -0.1f)
            {
                wheel.SuspensionRelativeVelocity = 0;
                wheel.ClippedInvContactDotSuspension = 1.0f / 0.1f;
            }
            else
            {
                float inv = -1.0f / denominator;
                wheel.SuspensionRelativeVelocity = projVel * inv;
                wheel.ClippedInvContactDotSuspension = inv;
            }
        }
        else
        {
            //put wheel info as in rest position
            wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
            wheel.SuspensionRelativeVelocity = 0.0f;
            wheel.RaycastInfo.ContactNormalWS = -wheel.RaycastInfo.WheelDirectionWS;
            wheel.ClippedInvContactDotSuspension = 1.0f;
        }

        return depth;
    }

    private void ResetSuspension()
    {
        for (int i = 0; i < NumWheels; i++)
        {
            WheelInfo wheel = GetWheelInfo(i);
            wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
            wheel.SuspensionRelativeVelocity = 0;

            wheel.RaycastInfo.ContactNormalWS = -wheel.RaycastInfo.WheelDirectionWS;
            //wheel.ContactFriction = 0;
            wheel.ClippedInvContactDotSuspension = 1;
        }
    }

    private void ResolveSingleBilateral(RigidBody body1, Vector3 pos1, RigidBody body2, Vector3 pos2, float distance, Vector3 normal, ref float impulse, float timeStep)
    {
        float normalLenSqr = normal.LengthSquared();
        Debug.Assert(MathF.Abs(normalLenSqr) < 1.1f);
        if (normalLenSqr > 1.1f)
        {
            impulse = 0;
            return;
        }

        Vector3 rel_pos1 = pos1 - body1.CenterOfMassPosition;
        Vector3 rel_pos2 = pos2 - body2.CenterOfMassPosition;

        Vector3 vel1 = body1.GetVelocityInLocalPoint(rel_pos1);
        Vector3 vel2 = body2.GetVelocityInLocalPoint(rel_pos2);
        Vector3 vel = vel1 - vel2;

        Matrix4x4 centerOfMass1 = body1.CenterOfMassTransform;
        Matrix4x4 centerOfMass2 = body2.CenterOfMassTransform;
        Matrix4x4 world2A = Matrix4x4.Transpose(centerOfMass1.GetBasis());
        Matrix4x4 world2B = Matrix4x4.Transpose(centerOfMass2.GetBasis());
        Vector3 m_aJ = Vector3.Transform(Vector3.Cross(rel_pos1, normal), world2A);
        Vector3 m_bJ = Vector3.Transform(Vector3.Cross(rel_pos2, -normal), world2B);
        Vector3 m_0MinvJt = body1.InvInertiaDiagLocal * m_aJ;
        Vector3 m_1MinvJt = body2.InvInertiaDiagLocal * m_bJ;
        float dot0, dot1;
        dot0 = Vector3.Dot(m_0MinvJt, m_aJ);
        dot1 = Vector3.Dot(m_1MinvJt, m_bJ);
        float jacDiagAB = body1.InvMass + dot0 + body2.InvMass + dot1;
        float jacDiagABInv = 1.0f / jacDiagAB;

        float rel_vel;
        rel_vel = Vector3.Dot(normal, vel);

        //todo: move this into proper structure
        const float contactDamping = 0.2f;

#if ONLY_USE_LINEAR_MASS
	        float massTerm = 1.0f / (body1.InvMass + body2.InvMass);
	        impulse = - contactDamping * rel_vel * massTerm;
#else
        float velocityImpulse = -contactDamping * rel_vel * jacDiagABInv;
        impulse = velocityImpulse;
#endif
    }

#pragma warning disable SA1202 // Elements should be ordered by access
    public void UpdateAction(CollisionWorld collisionWorld, float deltaTimeStep)
#pragma warning restore SA1202 // Elements should be ordered by access
        => UpdateVehicle(deltaTimeStep);

#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1203 // Constants should appear before fields
#pragma warning disable SA1303 // Const field names should begin with upper-case letter
    private const float sideFrictionStiffness2 = 1.0f;

#pragma warning restore SA1303 // Const field names should begin with upper-case letter
#pragma warning restore SA1203 // Constants should appear before fields
#pragma warning restore SA1201 // Elements should appear in the correct order
    public void UpdateFriction(float timeStep)
    {
        //calculate the impulse, so that the wheels don't move sidewards
        int numWheel = NumWheels;
        if (numWheel == 0)
        {
            return;
        }

        Array.Resize(ref forwardWS, numWheel);
        Array.Resize(ref axle, numWheel);
        Array.Resize(ref forwardImpulse, numWheel);
        Array.Resize(ref sideImpulse, numWheel);

        int numWheelsOnGround = 0;

        //collapse all those loops into one!
        for (int i = 0; i < NumWheels; i++)
        {
            RigidBody? groundObject = wheelInfo[i].RaycastInfo.GroundObject as RigidBody;
            if (groundObject != null)
            {
                numWheelsOnGround++;
            }

            sideImpulse[i] = 0;
            forwardImpulse[i] = 0;
        }

        for (int i = 0; i < NumWheels; i++)
        {
            WheelInfo wheel = wheelInfo[i];

            RigidBody? groundObject = wheel.RaycastInfo.GroundObject as RigidBody;
            if (groundObject != null)
            {
                Matrix4x4 wheelTrans = GetWheelTransformWS(i);

                axle[i] = new Vector3(
                    wheelTrans.GetComponent(0, indexRightAxis),
                    wheelTrans.GetComponent(1, indexRightAxis),
                    wheelTrans.GetComponent(2, indexRightAxis));

                Vector3 surfNormalWS = wheel.RaycastInfo.ContactNormalWS;
                float proj;
                proj = Vector3.Dot(axle[i], surfNormalWS);
                axle[i] -= surfNormalWS * proj;
                axle[i] = Vector3.Normalize(axle[i]);

                forwardWS[i] = Vector3.Cross(surfNormalWS, axle[i]);
                forwardWS[i] = Vector3.Normalize(forwardWS[i]);

                ResolveSingleBilateral(chassisBody, wheel.RaycastInfo.ContactPointWS, groundObject, wheel.RaycastInfo.ContactPointWS, 0, axle[i], ref sideImpulse[i], timeStep);

                sideImpulse[i] *= sideFrictionStiffness2;
            }
        }

        const float sideFactor = 1.0f;
        const float fwdFactor = 0.5f;

        bool sliding = false;

        for (int i = 0; i < NumWheels; i++)
        {
            WheelInfo wheel = wheelInfo[i];
            RigidBody? groundObject = wheel.RaycastInfo.GroundObject as RigidBody;

            float rollingFriction = 0.0f;

            if (groundObject != null)
            {
                if (wheel.EngineForce != 0.0f)
                {
                    rollingFriction = wheel.EngineForce * timeStep;
                }
                else
                {
                    float defaultRollingFrictionImpulse = 0.0f;
                    float maxImpulse = (wheel.Brake != 0) ? wheel.Brake : defaultRollingFrictionImpulse;
                    rollingFriction = CalcRollingFriction(chassisBody, groundObject, wheel.RaycastInfo.ContactPointWS, forwardWS[i], maxImpulse);
                }
            }

            //switch between active rolling (throttle), braking and non-active rolling friction (no throttle/break)

            forwardImpulse[i] = 0;
            wheelInfo[i].SkidInfo = 1.0f;

            if (groundObject != null)
            {
                wheelInfo[i].SkidInfo = 1.0f;

                float maximp = wheel.WheelsSuspensionForce * timeStep * wheel.FrictionSlip;
                float maximpSide = maximp;

                float maximpSquared = maximp * maximpSide;

                forwardImpulse[i] = rollingFriction;//wheel.EngineForce* timeStep;

                float x = forwardImpulse[i] * fwdFactor;
                float y = sideImpulse[i] * sideFactor;

                float impulseSquared = (x * x) + (y * y);

                if (impulseSquared > maximpSquared)
                {
                    sliding = true;

                    float factor = maximp / MathF.Sqrt(impulseSquared);

                    wheelInfo[i].SkidInfo *= factor;
                }
            }
        }

        if (sliding)
        {
            for (int wheel = 0; wheel < NumWheels; wheel++)
            {
                if (sideImpulse[wheel] != 0)
                {
                    if (wheelInfo[wheel].SkidInfo < 1.0f)
                    {
                        forwardImpulse[wheel] *= wheelInfo[wheel].SkidInfo;
                        sideImpulse[wheel] *= wheelInfo[wheel].SkidInfo;
                    }
                }
            }
        }

        // apply the impulses
        for (int i = 0; i < NumWheels; i++)
        {
            WheelInfo wheel = wheelInfo[i];

            Vector3 rel_pos = wheel.RaycastInfo.ContactPointWS -
                    chassisBody.CenterOfMassPosition;

            if (forwardImpulse[i] != 0)
            {
                chassisBody.ApplyImpulse(forwardWS[i] * forwardImpulse[i], rel_pos);
            }

            if (sideImpulse[i] != 0)
            {
                RigidBody? groundObject = wheel.RaycastInfo.GroundObject as RigidBody;

                Debug.Assert(groundObject is not null, $"{nameof(groundObject)} should not be null.");

                Vector3 rel_pos2 = wheel.RaycastInfo.ContactPointWS - groundObject.CenterOfMassPosition;

                Vector3 sideImp = axle[i] * sideImpulse[i];

#if ROLLING_INFLUENCE_FIX // fix. It only worked if car's up was along Y - VT.
                //Vector4 vChassisWorldUp = RigidBody.CenterOfMassTransform.get_Columns(indexUpAxis);
                Matrix4x4 centerOfMass = RigidBody.CenterOfMassTransform;
                Vector3 vChassisWorldUp = new Vector3(
                    centerOfMass.GetComponent(0, indexUpAxis),
                    centerOfMass.GetComponent(1, indexUpAxis),
                    centerOfMass.GetComponent(2, indexUpAxis));
                float dot = Vector3.Dot(vChassisWorldUp, rel_pos);
                rel_pos -= vChassisWorldUp * (dot * (1.0f - wheel.RollInfluence));
#else
                rel_pos[indexUpAxis] *= wheel.RollInfluence;
#endif
                chassisBody.ApplyImpulse(sideImp, rel_pos);

                //apply friction impulse on the ground
                groundObject.ApplyImpulse(-sideImp, rel_pos2);
            }
        }
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void UpdateSuspension(float step)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        float chassisMass = 1.0f / chassisBody.InvMass;

        for (int w_it = 0; w_it < NumWheels; w_it++)
        {
            WheelInfo wheel_info = wheelInfo[w_it];

            if (wheel_info.RaycastInfo.IsInContact)
            {
                float force;
                //	Spring
                {
                    float susp_length = wheel_info.SuspensionRestLength;
                    float current_length = wheel_info.RaycastInfo.SuspensionLength;

                    float length_diff = susp_length - current_length;

                    force = wheel_info.SuspensionStiffness
                        * length_diff * wheel_info.ClippedInvContactDotSuspension;
                }

                // Damper
                {
                    float projected_rel_vel = wheel_info.SuspensionRelativeVelocity;
                    {
                        float susp_damping;
                        if (projected_rel_vel < 0.0f)
                        {
                            susp_damping = wheel_info.WheelsDampingCompression;
                        }
                        else
                        {
                            susp_damping = wheel_info.WheelsDampingRelaxation;
                        }

                        force -= susp_damping * projected_rel_vel;
                    }
                }

                // RESULT
                wheel_info.WheelsSuspensionForce = force * chassisMass;
                if (wheel_info.WheelsSuspensionForce < 0)
                {
                    wheel_info.WheelsSuspensionForce = 0;
                }
            }
            else
            {
                wheel_info.WheelsSuspensionForce = 0;
            }
        }
    }

    public void UpdateVehicle(float step)
    {
        for (int i = 0; i < wheelInfo.Length; i++)
        {
            UpdateWheelTransform(i, false);
        }

        currentVehicleSpeedKmHour = 3.6f * RigidBody.LinearVelocity.Length();

        Matrix4x4 chassisTrans = ChassisWorldTransform;

        Vector3 forwardW = new Vector3(
            chassisTrans.GetComponent(0, indexForwardAxis),
            chassisTrans.GetComponent(1, indexForwardAxis),
            chassisTrans.GetComponent(2, indexForwardAxis));

        if (Vector3.Dot(forwardW, RigidBody.LinearVelocity) < 0)
        {
            currentVehicleSpeedKmHour *= -1.0f;
        }

        // Simulate suspension
        for (int i = 0; i < wheelInfo.Length; i++)
        {
            //float depth = 
            RayCast(wheelInfo[i]);
        }

        UpdateSuspension(step);

        for (int i = 0; i < wheelInfo.Length; i++)
        {
            //apply suspension force
            WheelInfo wheel = wheelInfo[i];

            float suspensionForce = wheel.WheelsSuspensionForce;

            if (suspensionForce > wheel.MaxSuspensionForce)
            {
                suspensionForce = wheel.MaxSuspensionForce;
            }

            Vector3 impulse = wheel.RaycastInfo.ContactNormalWS * suspensionForce * step;
            Vector3 relpos = wheel.RaycastInfo.ContactPointWS - RigidBody.CenterOfMassPosition;

            RigidBody.ApplyImpulse(impulse, relpos);
        }

        UpdateFriction(step);

        for (int i = 0; i < wheelInfo.Length; i++)
        {
            WheelInfo wheel = wheelInfo[i];
            Vector3 relpos = wheel.RaycastInfo.HardPointWS - RigidBody.CenterOfMassPosition;
            Vector3 vel = RigidBody.GetVelocityInLocalPoint(relpos);

            if (wheel.RaycastInfo.IsInContact)
            {
                Matrix4x4 chassisWorldTransform = ChassisWorldTransform;

                Vector3 fwd = new Vector3(
                    chassisWorldTransform.GetComponent(0, indexForwardAxis),
                    chassisWorldTransform.GetComponent(1, indexForwardAxis),
                    chassisWorldTransform.GetComponent(2, indexForwardAxis));

                float proj = Vector3.Dot(fwd, wheel.RaycastInfo.ContactNormalWS);
                fwd -= wheel.RaycastInfo.ContactNormalWS * proj;

                float proj2 = Vector3.Dot(fwd, vel);

                wheel.DeltaRotation = (proj2 * step) / wheel.WheelsRadius;
                wheel.Rotation += wheel.DeltaRotation;
            }
            else
            {
                wheel.Rotation += wheel.DeltaRotation;
            }

            wheel.DeltaRotation *= 0.99f;//damping of rotation when not in contact
        }
    }

    public void UpdateWheelTransform(int wheelIndex, bool interpolatedTransform)
    {
        WheelInfo wheel = wheelInfo[wheelIndex];
        UpdateWheelTransformsWS(wheel, interpolatedTransform);
        Vector3 up = -wheel.RaycastInfo.WheelDirectionWS;
        Vector3 right = wheel.RaycastInfo.WheelAxleWS;
        Vector3 fwd = Vector3.Cross(up, right);
        fwd = Vector3.Normalize(fwd);
        //up = Vector3.Cross(right, fwd);
        //up.Normalize();

        //rotate around steering over the wheelAxleWS
        Matrix4x4 steeringMat = Matrix4x4.CreateFromAxisAngle(up, wheel.Steering);
        Matrix4x4 rotatingMat = Matrix4x4.CreateFromAxisAngle(right, -wheel.Rotation);

        Matrix4x4 basis2 = default;
        basis2.M11 = right.X;
        basis2.M12 = fwd.X;
        basis2.M13 = up.X;
        basis2.M21 = right.Y;
        basis2.M22 = fwd.Y;
        basis2.M23 = up.Y;
        basis2.M31 = right.Z;
        basis2.M32 = fwd.Z;
        basis2.M13 = up.Z;

        Matrix4x4 transform = steeringMat * rotatingMat * basis2;
        transform.Translation = wheel.RaycastInfo.HardPointWS + (wheel.RaycastInfo.WheelDirectionWS * wheel.RaycastInfo.SuspensionLength);
        wheel.WorldTransform = transform;
    }

    private void UpdateWheelTransformsWS(WheelInfo wheel, bool interpolatedTransform)
    {
        wheel.RaycastInfo.IsInContact = false;

        Matrix4x4 chassisTrans = ChassisWorldTransform;
        if (interpolatedTransform && RigidBody.MotionState != null)
        {
            chassisTrans = RigidBody.MotionState.WorldTransform;
        }

        wheel.RaycastInfo.HardPointWS = Vector3.Transform(wheel.ChassisConnectionPointCS, chassisTrans);
        Matrix4x4 chassisTransBasis = chassisTrans.GetBasis();
        wheel.RaycastInfo.WheelDirectionWS = Vector3.Transform(wheel.WheelDirectionCS, chassisTransBasis);
        wheel.RaycastInfo.WheelAxleWS = Vector3.Transform(wheel.WheelAxleCS, chassisTransBasis);
    }
}

public class DefaultVehicleRaycaster : IVehicleRaycaster
{
    private readonly DynamicsWorld _dynamicsWorld;

    public DefaultVehicleRaycaster(DynamicsWorld world)
    {
        _dynamicsWorld = world;
    }

    public object? CastRay(ref Vector3 from, ref Vector3 to, VehicleRaycasterResult result)
    {
        //	RayResultCallback& resultCallback;
        using (ClosestRayResultCallback rayCallback = new ClosestRayResultCallback(ref from, ref to))
        {
            _dynamicsWorld.RayTestRef(ref from, ref to, rayCallback);

            if (rayCallback.HasHit)
            {
                RigidBody? body = RigidBody.Upcast(rayCallback.CollisionObject);
                if (body != null && body.HasContactResponse)
                {
                    result.HitPointInWorld = rayCallback.HitPointWorld;
                    Vector3 hitNormalInWorld = rayCallback.HitNormalWorld;
                    hitNormalInWorld = Vector3.Normalize(hitNormalInWorld);
                    result.HitNormalInWorld = hitNormalInWorld;
                    result.DistFraction = rayCallback.ClosestHitFraction;
                    return body;
                }
            }
        }

        return null;
    }
}
