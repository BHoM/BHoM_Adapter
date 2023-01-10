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

using BH.Adapter;
using BH.Adapter.Tests;
using BH.Engine.Structure;
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Offsets;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Adapter
{
    public class StructuralAdapter : BHoMAdapter
    {
        public List<Tuple<Type, IEnumerable<IBHoMObject>>> Created { get; set; } = new List<Tuple<Type, IEnumerable<IBHoMObject>>>();
        public List<Tuple<Type, IList>> ReadTypes { get; set; } = new List<Tuple<Type, IList>>();
        public List<Tuple<Type, IEnumerable<IBHoMObject>>> Updated { get; set; } = new List<Tuple<Type, IEnumerable<IBHoMObject>>>();
        public List<Tuple<Type, IEnumerable<object>>> Deleted { get; set; } = new List<Tuple<Type, IEnumerable<object>>>();

        public StructuralAdapter()
        {
            m_AdapterSettings = new AdapterSettings()
            {
                UseAdapterId = false
            };

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
                { typeof(IElementLoad<Node>), new List<Type>{ typeof(Node)} },
                { typeof(GravityLoad), new List<Type>{ typeof(Bar), typeof(Panel), typeof(FEMesh)} }
            };

            AdapterComparers = new Dictionary<Type, object>
            {
                {typeof(Bar), new BarEndNodesDistanceComparer(3) },
                {typeof(Node), new NodeDistanceComparer(3) },
                {typeof(ISectionProperty), new NameOrDescriptionComparer() },
                {typeof(ISurfaceProperty), new NameOrDescriptionComparer() },
                {typeof(IMaterialFragment), new NameOrDescriptionComparer() },
                {typeof(LinkConstraint), new NameOrDescriptionComparer() },
                {typeof(Constraint6DOF), new NameOrDescriptionComparer() },
            };

            AdapterIdFragmentType = typeof(StructuralAdapterId);
            BH.Adapter.Modules.Structure.ModuleLoader.LoadModules(this);
            this.m_AdapterSettings.OnlyUpdateChangedObjects = true;
        }

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            Created.Add(new Tuple<Type, IEnumerable<IBHoMObject>>(typeof(T), objects.OfType<IBHoMObject>()));

            return true;
        }

        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            ReadTypes.Add(new Tuple<Type, IList>(type, ids));

            List<IBHoMObject> modelObejcts = Created.Where(x => x.Item1.IsAssignableFrom(type)).SelectMany(x => x.Item2).ToList();

            return modelObejcts;
        }

        protected override bool IUpdate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            Updated.Add(new Tuple<Type, IEnumerable<IBHoMObject>>(typeof(T), objects.OfType<IBHoMObject>()));

            return true;
        }

        protected override int IDelete(Type type, IEnumerable<object> ids, ActionConfig actionConfig = null)
        {
            Deleted.Add(new Tuple<Type, IEnumerable<object>>(type, ids));

            return 0;
        }
    }
}