/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using NUnit.Framework;
using BH.Adapter.Tests;
using BH.oM.Base;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Adapter;
using System.Diagnostics.Contracts;
using AutoBogus;
using Shouldly;

namespace BH.Tests.Adapter.Structure
{
    public class PushTests
    {
        StructuralAdapter sa;
        [SetUp]
        public void Setup()
        {
            sa = new StructuralAdapter();
        }

        private static IEnumerable<TestCaseData> GetTestContainers()
        {
            // BH.Engine.Base.Create.RandomObject() can't deal with generics. Using AutoFaker instead. Example:
            // AutoFaker creates 1 objects of the requested type per each IEnumerable property.
            // E.g. Container<Bar> will have 1 + 1 + 1 = 3 Bars.  
            yield return new TestCaseData(new AutoFaker<TestContainer<Bar>>().Generate(), 5, 50);
        }


        [Test]
        [TestCaseSource(nameof(GetTestContainers))]
        public void UnpackObjsDuringPush<T>(TestContainer<T> container, int numberOfTypes, int minTotalObjects)
        {
            sa.Push(new List<object>() { container });

            IEnumerable<BH.oM.Base.IBHoMObject>? sectionProperties = sa.Created.Where(c => c.Item1 == typeof(ISectionProperty)).FirstOrDefault()?.Item2 ?? new List<IBHoMObject>();
            sa.Created.Count.ShouldBe(numberOfTypes);
            sa.Created.SelectMany(kv => kv.Item2).Count().ShouldBeGreaterThan(minTotalObjects);
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

            string correctOrder = "BH.oM.Structure.MaterialFragments.IMaterialFragment, " +
                                  "BH.oM.Structure.Constraints.Constraint6DOF, " +
                                  "BH.oM.Structure.SectionProperties.ISectionProperty, " +
                                  "BH.oM.Structure.Elements.Node, " +
                                  "BH.oM.Structure.Constraints.BarRelease, " +
                                  "BH.oM.Structure.Offsets.Offset, " +
                                  "BH.oM.Structure.Elements.Bar, " +
                                  "BH.oM.Structure.Loads.Loadcase, " +
                                  "BH.oM.Structure.Loads.BarUniformlyDistributedLoad";

            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));
            Assert.AreEqual(correctOrder, createdOrder);
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

            string correctOrder = "BH.oM.Structure.Constraints.Constraint6DOF, " +
                                  "BH.oM.Structure.MaterialFragments.IMaterialFragment, " +
                                  "BH.oM.Structure.Constraints.Constraint4DOF, " +
                                  "BH.oM.Structure.Elements.Node, " +
                                  "BH.oM.Structure.Elements.Edge, " +
                                  "BH.oM.Structure.SectionProperties.ISectionProperty, " +
                                  "BH.oM.Structure.Constraints.BarRelease, " +
                                  "BH.oM.Structure.Offsets.Offset, " +
                                  "BH.oM.Structure.SurfaceProperties.ISurfaceProperty, " +
                                  "BH.oM.Structure.Elements.Bar, " +
                                  "BH.oM.Structure.Loads.Loadcase, " +
                                  "BH.oM.Structure.Elements.Opening, " +
                                  "BH.oM.Structure.Loads.BarUniformlyDistributedLoad, " +
                                  "BH.oM.Structure.Elements.FEMesh, " +
                                  "BH.oM.Structure.Elements.Panel";

            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));
            Assert.AreEqual(correctOrder, createdOrder);
        }

        [Test]
        public void Dependecies_UpdateOnly()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<Node>(10));
            inputObjects.AddRange(Create.RandomObjects<Bar>(10));
            inputObjects.AddRange(Create.RandomObjects<SteelSection>(10));
            inputObjects.AddRange(Create.RandomObjects<AluminiumSection>(10));

            sa.Push(inputObjects, "", BH.oM.Adapter.PushType.UpdateOnly);

            string correctOrderCreated = "BH.oM.Structure.Constraints.Constraint6DOF, " +
                                         "BH.oM.Structure.MaterialFragments.IMaterialFragment, " +
                                         "BH.oM.Structure.SectionProperties.ISectionProperty, " +
                                         "BH.oM.Structure.Elements.Node, " +
                                         "BH.oM.Structure.Constraints.BarRelease, " +
                                         "BH.oM.Structure.Offsets.Offset";
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

        [Test]
        public void DependencyOrder_UpdateAndFullPush()
        {
            List<object> inputObjects = new List<object>();
            Node n = Create.RandomObject<Node>();
            Bar bar = Create.RandomObject<Bar>();
            bar.StartNode = n;
            inputObjects.Add(bar);
            inputObjects.Add(n);
            inputObjects.Add(Create.RandomObject<AluminiumSection>()); // this should be moved up before the Bar's AluminiumSection's FullPush.
            inputObjects.Add(new Aluminium());
            inputObjects.Add(new Constraint6DOF()); // this should not "jump ahead"

            var orderedObjects = Engine.Adapter.Query.GetDependencySortedObjects(inputObjects.OfType<IBHoMObject>().ToList(), BH.oM.Adapter.PushType.UpdateOnly, sa);

            var onlyNodes = orderedObjects.Where(t => t.Item1 == typeof(Node));

            Assert.IsTrue(onlyNodes.Any(t => t.Item2 == PushType.UpdateOnly), "Missing UpdateOnly in the list of pushed nodes.");
            Assert.IsTrue(onlyNodes.Any(t => t.Item2 == PushType.FullPush), "Missing FullPush in the list of pushed nodes.");

            Assert.IsTrue(orderedObjects.Where(t => t.Item1 == typeof(Node)).First().Item2 == PushType.UpdateOnly,
                "For Node objects, UpdateOnly should have come before FullPush.");
        }

        [Test]
        public void DependencyOrder_CreateLoadNoObjectsWithIds()
        {
            List<BarUniformlyDistributedLoad> loads = Create.RandomObjects<BarUniformlyDistributedLoad>(3);

            sa.Push(loads);

            string correctOrder = "BH.oM.Structure.MaterialFragments.IMaterialFragment, " +
                                  "BH.oM.Structure.Constraints.Constraint6DOF, " +
                                  "BH.oM.Structure.SectionProperties.ISectionProperty, " +
                                  "BH.oM.Structure.Elements.Node, " +
                                  "BH.oM.Structure.Constraints.BarRelease, " +
                                  "BH.oM.Structure.Offsets.Offset, " +
                                  "BH.oM.Structure.Elements.Bar, " +
                                  "BH.oM.Structure.Loads.Loadcase, " +
                                  "BH.oM.Structure.Loads.BarUniformlyDistributedLoad";
            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));

            Assert.AreEqual(correctOrder, createdOrder);
        }

        [Test]
        public void DependencyOrder_CreateLoadAllObjectsWithIds()
        {
            List<Bar> bars = Create.RandomObjects<Bar>(10);
            for (int i = 0; i < bars.Count; i++)
            {
                Engine.Adapter.Modify.SetAdapterId(bars[i], new StructuralAdapterId { Id = i + 1 });
            }
            BarUniformlyDistributedLoad load = Create.RandomObject<BarUniformlyDistributedLoad>();
            load.Objects.Elements = bars;

            sa.Push(new List<object> { load });

            string correctOrder = "BH.oM.Structure.Loads.Loadcase, BH.oM.Structure.Loads.BarUniformlyDistributedLoad";  //All bars contain Ids, hence no bars should be created even if there is a dependency on the bars
            string createdOrder = string.Join(", ", sa.Created.Select(c => c.Item1.FullName));

            Assert.AreEqual(correctOrder, createdOrder);
        }

        [Test]
        public void DependencyOrder_CreateLoadHalfObjectsWithIds()
        {
            int objectCount = 10;
            List<Bar> bars = Create.RandomObjects<Bar>(objectCount);
            int withIdCount = objectCount / 2;
            int withoutIdCount = objectCount - withIdCount;
            for (int i = 0; i < withIdCount; i++)
            {
                Engine.Adapter.Modify.SetAdapterId(bars[i], new StructuralAdapterId { Id = i + 1 });
            }

            //Shuffle the order fo the bars.
            //Doing this to test that the order of bars with and without Id does not matter
            Random random = new Random(2);
            bars = bars.OrderBy(x => random.Next()).ToList();

            BarUniformlyDistributedLoad load = Create.RandomObject<BarUniformlyDistributedLoad>();
            load.Objects.Elements = bars;

            sa.Push(new List<object> { load });

            Assert.IsTrue(sa.Created.Any(x => x.Item1 == typeof(Bar)), "No bars created.");
            int barCreationCount = sa.Created.First(x => x.Item1 == typeof(Bar)).Item2.Count();

            Assert.AreEqual(withoutIdCount, barCreationCount, "Wrong number of bars created.");
        }

        [Test]
        public void DependencyOrder_CreateGravityLoadHalfObjectsWithIds()
        {
            int objectCount = 10;
            List<Bar> bars = Create.RandomObjects<Bar>(objectCount);
            List<Panel> panels = Create.RandomObjects<Panel>(objectCount);
            int withIdCount = objectCount / 2;
            int withoutIdCount = objectCount - withIdCount;
            for (int i = 0; i < withIdCount; i++)
            {
                Engine.Adapter.Modify.SetAdapterId(bars[i], new StructuralAdapterId { Id = i + 1 });
                Engine.Adapter.Modify.SetAdapterId(panels[i], new StructuralAdapterId { Id = i + 1 });
            }

            //Shuffle the order fo the bars.
            //Doing this to test that the order of bars with and without Id does not matter
            Random random = new Random(2);
            bars = bars.OrderBy(x => random.Next()).ToList();
            panels = panels.OrderBy(x => random.Next()).ToList();

            GravityLoad load = Create.RandomObject<GravityLoad>();
            load.Objects.Elements = bars.Cast<BHoMObject>().Concat(panels).ToList();

            sa.Push(new List<object> { load });

            Assert.IsTrue(sa.Created.Any(x => x.Item1 == typeof(Bar)), "No bars created.");
            int barCreationCount = sa.Created.First(x => x.Item1 == typeof(Bar)).Item2.Count();

            Assert.AreEqual(withoutIdCount, barCreationCount, "Wrong number of bars created.");

            Assert.IsTrue(sa.Created.Any(x => x.Item1 == typeof(Panel)), "No Panels created.");
            int panelsCreationCount = sa.Created.First(x => x.Item1 == typeof(Panel)).Item2.Count();

            Assert.AreEqual(withoutIdCount, panelsCreationCount, "Wrong number of Panels created.");
        }

        [Test]
        public void UpdateOnlyChanged()
        {
            //Create some random objects
            int objectCount = 10;

            List<Bar> bars = new List<Bar>();
            List<Node> nodes = new List<Node>();
            List<SteelSection> sectionProperties = new List<SteelSection>();
            List<Steel> steels = new List<Steel>();

            //Using methodology below to ensure the same random obejcts are created each set of the run.
            //For some edge cases not using the methodology, some strings turned out the same/empty leading to the test failing
            for (int i = 0; i < objectCount; i++)
            {
                bars.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar), (i + 1) * 3) as Bar);
                nodes.Add(BH.Engine.Base.Create.RandomObject(typeof(Node), (i + 1) * 7) as Node);
                sectionProperties.Add(BH.Engine.Base.Create.RandomObject(typeof(SteelSection), (i + 1) * 37) as SteelSection);
                steels.Add(BH.Engine.Base.Create.RandomObject(typeof(Steel), (i + 1) * 13) as Steel);
            }


            List<IBHoMObject> allObjects = bars.Cast<IBHoMObject>().Concat(nodes).Concat(sectionProperties).Concat(steels).ToList();

            sa.Push(allObjects);

            int changeCount = objectCount / 2;

            HashSet<int> randomIds = new HashSet<int>();
            Random random = new Random(2);
            //Generate random ids to change
            while (randomIds.Count < changeCount)
            {
                randomIds.Add((int)Math.Floor(random.NextDouble() * objectCount));
            }

            //Update the random obejcts with the random ids
            //The update is ensured to not change the part of the object used by the comparer to identify the objects as the same
            //Using methodology below to ensure the same random obejcts are created each set of the run.
            //For some edge cases not using the methodology, some strings turned out the same/empty leading to the test failing
            foreach (int i in randomIds)
            {
                bars[i].SectionProperty = BH.Engine.Base.Create.RandomObject(typeof(SteelSection), (i + 1) * 17) as SteelSection;
                nodes[i].Support = BH.Engine.Base.Create.RandomObject(typeof(Constraint6DOF), (i + 1) * 19) as Constraint6DOF;
                SteelSection newSection = BH.Engine.Base.Create.RandomObject(typeof(SteelSection), (i + 1) * 23) as SteelSection;
                newSection.Name = sectionProperties[i].Name;
                sectionProperties[i] = newSection;
                Steel newMaterial = BH.Engine.Base.Create.RandomObject(typeof(Steel), (i + 1) * 31) as Steel;
                newMaterial.Name = steels[i].Name;
                steels[i] = newMaterial;
            }

            allObjects = bars.Cast<IBHoMObject>().Concat(nodes).Concat(sectionProperties).Concat(steels).ToList();

            //Push the updated objects again
            sa.Push(allObjects);

            Assert.IsTrue(sa.Updated.Any(x => x.Item1 == typeof(Bar)), "No Bars Updated.");
            int barUpdateCount = sa.Updated.First(x => x.Item1 == typeof(Bar)).Item2.Count();

            Assert.AreEqual(changeCount, barUpdateCount, "Wrong number of Bars Updated.");

            Assert.IsTrue(sa.Updated.Any(x => x.Item1 == typeof(Node)), "No Nodes Updated.");
            int nodeUpdateCount = sa.Updated.First(x => x.Item1 == typeof(Node)).Item2.Count();

            Assert.AreEqual(changeCount, nodeUpdateCount, "Wrong number of Nodes Updated.");

            Assert.IsTrue(sa.Updated.Any(x => x.Item1 == typeof(ISectionProperty)), "No SectionProperty Updated.");
            int sectionUpdateCount = sa.Updated.First(x => x.Item1 == typeof(ISectionProperty)).Item2.Count();

            Assert.AreEqual(changeCount, sectionUpdateCount, "Wrong number of ISectionProperties Updated.");

            Assert.IsTrue(sa.Updated.Any(x => x.Item1 == typeof(IMaterialFragment)), "No Materials Updated.");
            int materialUpdateCount = sa.Updated.First(x => x.Item1 == typeof(IMaterialFragment)).Item2.Count();

            Assert.AreEqual(changeCount, materialUpdateCount, "Wrong number of Materials Updated.");
        }

        [Test]
        public void CountCRUDCallsPerType()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<Bar>(10));
            inputObjects.AddRange(Create.RandomObjects<SteelSection>(10));

            sa.Push(inputObjects);

            Assert.IsTrue(sa.CallsToCreatePerType.All(kv => kv.Value == 1), "Calls to Create should be done once per each type.");
            Assert.IsTrue(sa.CallsToReadPerType.All(kv => kv.Value == 1), "Calls to Read should be done once per each type.");
        }

        [Test]
        public void CountCRUDCallsPerType_UpdateOnly()
        {
            List<object> inputObjects = new List<object>();
            inputObjects.AddRange(Create.RandomObjects<Bar>(10));
            inputObjects.AddRange(Create.RandomObjects<SteelSection>(10));

            sa.Push(inputObjects);
            sa.Push(inputObjects, "", PushType.UpdateOnly);

            Assert.IsTrue(sa.CallsToCreatePerType.All(kv => kv.Value == 1), "Calls to Create should be done once per each type.");
            Assert.IsTrue(sa.CallsToReadPerType.Where(kv => kv.Key == typeof(Bar)).First().Value == 1, "The Bar should be read only once.");
            Assert.IsTrue(sa.CallsToReadPerType.Where(kv => kv.Key != typeof(Bar)).All(kv => kv.Value == 2));
            Assert.IsTrue(sa.CallsToUpdatePerType.All(kv => kv.Value == 1), "Calls to Update should be done once per each type.");
        }

        [Test]
        public void Preprocess_PanelLoads()
        {
            List<object> inputObjects = new List<object>();
            List<Panel> panels = Create.RandomObjects<Panel>();
            AreaUniformlyDistributedLoad load = Create.RandomObject<AreaUniformlyDistributedLoad>();
            load.Objects.Elements = panels.Cast<IAreaElement>().ToList();
            inputObjects.AddRange(panels);
            inputObjects.Add(load);

            sa.Push(inputObjects);


        }
    }
}