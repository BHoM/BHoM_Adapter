
using BH.Adapter;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Offsets;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BHoM_Adapter_Tests
{
    public class StructuralAdapter : BHoMAdapter
    {
        public StructuralAdapter()
        {
            DependencyTypes = new Dictionary<Type, List<Type>>
            {
                {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node), typeof(BarRelease), typeof(Offset)}},
                {typeof(ISectionProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(Node), new List<Type> { typeof(Constraint6DOF) } },
                {typeof(ILoad), new List<Type> { typeof(Loadcase) } },
                {typeof(LoadCombination), new List<Type> { typeof(Loadcase) } },
                {typeof(Panel), new List<Type> { typeof(ISurfaceProperty) , typeof(Opening), typeof(Edge)} },
                {typeof(Opening), new List<Type> {typeof(Edge) } },
                {typeof(Edge), new List<Type> { typeof(Constraint6DOF), typeof(Constraint4DOF) } },
                {typeof(ISurfaceProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(RigidLink), new List<Type> { typeof(LinkConstraint), typeof(Node) } },
                {typeof(FEMesh), new List<Type> { typeof(Node), typeof(ISurfaceProperty)} },
                { typeof(IElementLoad<Bar>), new List<Type>{ typeof(Bar)} },
                { typeof(IElementLoad<Node>), new List<Type>{ typeof(Node)} }
            };
        }
    }
}
