/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */


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

        [Test]
        public void Dependecies_UpdateOnly()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<Bar>(10));
            inputObjects.AddRange(Create.RandomObjects<Node>(10));
            inputObjects.AddRange(Create.RandomObjects<SteelSection>(10));
            inputObjects.AddRange(Create.RandomObjects<AluminiumSection>(10));

            sa.Push(inputObjects, "", BH.oM.Adapter.PushType.UpdateOnly);

            string correctOrderCreated = "BH.oM.Structure.Constraints.Constraint6DOF, BH.oM.Structure.MaterialFragments.IMaterialFragment, BH.oM.Structure.SectionProperties.ISectionProperty, BH.oM.Structure.Elements.Node, BH.oM.Structure.Constraints.BarRelease, BH.oM.Structure.Offsets.Offset";
            string correctOrderUpdated = "BH.oM.Structure.Elements.Node, BH.oM.Structure.SectionProperties.ISectionProperty, BH.oM.Structure.Elements.Bar";

            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));
            string updateOrder = string.Join(", ", sa.Updated.Select(c => c.Item1.FullName));
            Assert.AreEqual(correctOrderCreated, createdOrder);
            Assert.AreEqual(correctOrderUpdated, updateOrder);
        }

        [Test]
        public void DependencyOrder_MultipleSectionTypes()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<ConcreteSection>(10));
            inputObjects.AddRange(Create.RandomObjects<TimberSection>(10));
            inputObjects.AddRange(Create.RandomObjects<SteelSection>(10));

            sa.Push(inputObjects);

            string correctOrder = "BH.oM.Structure.MaterialFragments.IMaterialFragment, BH.oM.Structure.SectionProperties.ISectionProperty";

            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));

            Assert.AreEqual(correctOrder, createdOrder);

            List<Type> correctCreatedSectionTypes = inputObjects.Select(x => x.GetType()).Distinct().ToList();

            Assert.IsTrue(sa.Created.Count == 2, "Wrong number of created object types.");
            Assert.IsTrue(sa.Created[1].Item1 == typeof(ISectionProperty), "Sections not created as second item.");

            List<Type> createdSectionTypes = sa.Created[1].Item2.Select(x => x.GetType()).Distinct().ToList();

            Assert.AreEqual(correctCreatedSectionTypes, createdSectionTypes);

            
        }
    }
}