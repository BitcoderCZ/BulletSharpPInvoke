using System.Numerics;

namespace BulletSharp;

public interface IVehicleRaycaster
{
    object? CastRay(ref Vector3 from, ref Vector3 to, VehicleRaycasterResult result);
}

public class VehicleRaycasterResult
{
    public float DistFraction { get; set; }

    public Vector3 HitNormalInWorld { get; set; }

    public Vector3 HitPointInWorld { get; set; }
}