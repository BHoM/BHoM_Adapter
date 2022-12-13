using BH.Adapter.Tests;
using BH.oM.Base;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

namespace BHoM_Adapter_Tests
{
    public class PushTests
    {
        StructuralAdapter sa;
        [SetUp]
        public void Setup()
        {
            sa = new StructuralAdapter();
        }

        [Test]
        public void GroupByParentInterface()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<Bar>(10));
            inputObjects.AddRange(Create.RandomObjects<SteelSection>(10));

            sa.Push(inputObjects);

            IEnumerable<BH.oM.Base.IBHoMObject>? sectionProperties = sa.Created.Where(c => c.Item1 == typeof(ISectionProperty)).FirstOrDefault()?.Item2 ?? new List<IBHoMObject>();
            Assert.IsTrue(sectionProperties.OfType<SteelSection>().Any() && sectionProperties.OfType<AluminiumSection>().Any(),
                "Section properties should include both the input SteelSections and the AluminiumSection generated via RandomObject().");
        }


        [Test]
        public void DependencyOrder_BarLoads()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<Bar>(10));
            inputObjects.AddRange(Create.RandomObjects<BarUniformlyDistributedLoad>(10));
            inputObjects.AddRange(Create.RandomObjects<Loadcase>(10));

            sa.Push(inputObjects);

            string correctOrder = "BH.oM.Structure.Constraints.Constraint6DOF, BH.oM.Structure.MaterialFragments.IMaterialFragment, " +
                "BH.oM.Structure.SectionProperties.ISectionProperty, BH.oM.Structure.Elements.Node, " +
                "BH.oM.Structure.Constraints.BarRelease, BH.oM.Structure.Offsets.Offset, " +
                "BH.oM.Structure.Elements.Bar, BH.oM.Structure.Loads.Loadcase, " +
                "BH.oM.Structure.Loads.BarUniformlyDistributedLoad";

            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));
            Assert.IsTrue(createdOrder == correctOrder);
        }


        [Test]
        public void DependencyOrder_MostStructuralObjects()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<Constraint6DOF>(10));
            inputObjects.AddRange(Create.RandomObjects<Bar>(10));
            inputObjects.AddRange(Create.RandomObjects<Node>(10));
            inputObjects.AddRange(Create.RandomObjects<SteelSection>(10));
            inputObjects.AddRange(Create.RandomObjects<BarUniformlyDistributedLoad>(10));
            inputObjects.AddRange(Create.RandomObjects<Loadcase>(10));
            inputObjects.AddRange(Create.RandomObjects<FEMesh>(10));
            inputObjects.AddRange(Create.RandomObjects<Panel>(10));
            inputObjects.AddRange(Create.RandomObjects<ConstantThickness>(10));

            sa.Push(inputObjects);

            string correctOrder = "BH.oM.Structure.Constraints.Constraint4DOF, BH.oM.Structure.MaterialFragments.IMaterialFragment, " +
                "BH.oM.Structure.Constraints.Constraint6DOF, BH.oM.Structure.SectionProperties.ISectionProperty, " +
                "BH.oM.Structure.Elements.Node, BH.oM.Structure.Constraints.BarRelease, BH.oM.Structure.Offsets.Offset, " +
                "BH.oM.Structure.Elements.Bar, BH.oM.Structure.Loads.Loadcase, BH.oM.Structure.SurfaceProperties.ISurfaceProperty, " +
                "BH.oM.Structure.Elements.Opening, BH.oM.Structure.Elements.Edge, BH.oM.Structure.Loads.BarUniformlyDistributedLoad, " +
                "BH.oM.Structure.Elements.FEMesh, BH.oM.Structure.Elements.Panel";

            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));
            Assert.AreEqual(correctOrder, createdOrder);
        }
    }
}