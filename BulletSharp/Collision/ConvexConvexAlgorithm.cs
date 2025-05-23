using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class ConvexConvexAlgorithm : ActivatingCollisionAlgorithm
{
    public ConvexConvexAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap, VoronoiSimplexSolver simplexSolver, ConvexPenetrationDepthSolver pdSolver, int numPerturbationIterations, int minimumPointsPerturbationThreshold)
        : base()
    {
        IntPtr native = btConvexConvexAlgorithm_new(mf.Native, ci.Native, body0Wrap.Native, body1Wrap.Native, simplexSolver.Native, pdSolver.Native, numPerturbationIterations, minimumPointsPerturbationThreshold);
        InitializeUserOwned(native);
    }

    internal ConvexConvexAlgorithm(IntPtr native, BulletObject? owner)
    {
        InitializeSubObject(native, owner);
    }

    public PersistentManifold Manifold => new PersistentManifold(btConvexConvexAlgorithm_getManifold(Native), this);

    public void SetLowLevelOfDetail(bool useLowLevel)
        => btConvexConvexAlgorithm_setLowLevelOfDetail(Native, useLowLevel);

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        private ConvexPenetrationDepthSolver _pdSolver;

        public CreateFunc(ConvexPenetrationDepthSolver pdSolver)
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btConvexConvexAlgorithm_CreateFunc_new(pdSolver.Native);
            InitializeUserOwned(native);
            _pdSolver = pdSolver;
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);

            _pdSolver = null!;
        }

        public int MinimumPointsPerturbationThreshold
        {
            get => btConvexConvexAlgorithm_CreateFunc_getMinimumPointsPerturbationThreshold(Native);
            set => btConvexConvexAlgorithm_CreateFunc_setMinimumPointsPerturbationThreshold(Native, value);
        }

        public int NumPerturbationIterations
        {
            get => btConvexConvexAlgorithm_CreateFunc_getNumPerturbationIterations(Native);
            set => btConvexConvexAlgorithm_CreateFunc_setNumPerturbationIterations(Native, value);
        }

        public ConvexPenetrationDepthSolver PdSolver
        {
            get => _pdSolver;
            set
            {
                btConvexConvexAlgorithm_CreateFunc_setPdSolver(Native, value.Native);
                _pdSolver = value;
            }
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new ConvexConvexAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }
}
