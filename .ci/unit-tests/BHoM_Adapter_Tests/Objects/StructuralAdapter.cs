/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BH.Engine.Adapter;
using BH.Engine.Base;

namespace BH.Tests.Adapter
{
    public class StructuralAdapter : BHoMAdapter
    {
        public List<Tuple<Type, IEnumerable<IBHoMObject>>> Created { get; set; } = new List<Tuple<Type, IEnumerable<IBHoMObject>>>();
        public List<Tuple<Type, IList>> ReadTypes { get; set; } = new List<Tuple<Type, IList>>();
        public List<Tuple<Type, IEnumerable<IBHoMObject>>> Updated { get; set; } = new List<Tuple<Type, IEnumerable<IBHoMObject>>>();
        public List<Tuple<Type, IEnumerable<object>>> Deleted { get; set; } = new List<Tuple<Type, IEnumerable<object>>>();

        [Description("Useful e.g. to compare how many calls to Create are done with/without the caching mechanism.")]
        public Dictionary<Type, int> CallsToCreatePerType { get; set; } = new Dictionary<Type, int>();
        [Description("Useful e.g. to compare how many calls to Create are done with/without the caching mechanism.")]
        public Dictionary<Type, int> CallsToReadPerType { get; set; } = new Dictionary<Type, int>();
        [Description("Useful e.g. to compare how many calls to Create are done with/without the caching mechanism.")]
        public Dictionary<Type, int> CallsToUpdatePerType { get; set; } = new Dictionary<Type, int>();
        [Description("Useful e.g. to compare how many calls to Create are done with/without the caching mechanism.")]
        public Dictionary<Type, int> CallsToDeletePerType { get; set; } = new Dictionary<Type, int>();

        public StructuralAdapter(bool cacheCRUDobjects = true)
        {
            m_AdapterSettings = new AdapterSettings()
            {
                UseAdapterId = true,
                OnlyUpdateChangedObjects = true,
                CacheCRUDobjects = cacheCRUDobjects
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
                {typeof(Offset), new NameOrDescriptionComparer() },
                {typeof(BarRelease), new NameOrDescriptionComparer() }
            };


            BH.Adapter.Modules.Structure.ModuleLoader.LoadModules(this);
            AdapterIdFragmentType = typeof(StructuralAdapterId);
        }

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {

            ValidateCreateObjects(objects as dynamic);

            Created.Add(new Tuple<Type, IEnumerable<IBHoMObject>>(typeof(T), objects.OfType<IBHoMObject>()));

            if (!CallsToCreatePerType.TryGetValue(typeof(T), out int n))
                CallsToCreatePerType[typeof(T)] = 1;
            else
                CallsToCreatePerType[typeof(T)] = n + 1;

            return true;
        }

        private void ValidateCreateObjects(IEnumerable<object> objects)
        { 
            
        }

        private void ValidateCreateObjects<T>(IEnumerable<IElementLoad<T>> objects) where T : IBHoMObject
        {
            foreach (IElementLoad<T> load in objects)
            {
                foreach (IBHoMObject bhObj in load.Objects.Elements)
                {
                    StructuralAdapterId id = bhObj.FindFragment<StructuralAdapterId>();
                    if (id == null)
                        throw new Exception("Elements on loads do not contain required Ids.");
                }
            }
        }

        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            ReadTypes.Add(new Tuple<Type, IList>(type, ids));

            List<IBHoMObject> modelObjects = Created.Where(x => x.Item1.IsAssignableFrom(type)).SelectMany(x => x.Item2).ToList();

            List<Type> dependencyTypes = this.GetDependencyTypes(type);

            MethodInfo readCached = typeof(BHoMAdapter).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.GetGenericArguments().Length == 1).FirstOrDefault(x => x.Name == nameof(GetCachedOrRead));

            foreach (Type t in dependencyTypes)
            {
                MethodInfo generic = readCached.MakeGenericMethod(t);
                generic.Invoke(this, new object[] { null, null, actionConfig });
            }

            if (!CallsToReadPerType.TryGetValue(type, out int n))
                CallsToReadPerType[type] = 1;
            else
                CallsToReadPerType[type] = n + 1;

            return modelObjects;
        }

        protected override bool IUpdate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            Updated.Add(new Tuple<Type, IEnumerable<IBHoMObject>>(typeof(T), objects.OfType<IBHoMObject>()));

            if (!CallsToUpdatePerType.TryGetValue(typeof(T), out int n))
                CallsToUpdatePerType[typeof(T)] = 1;
            else
                CallsToUpdatePerType[typeof(T)] = n + 1;

            return true;
        }

        protected override int IDelete(Type type, IEnumerable<object> ids, ActionConfig actionConfig = null)
        {
            Deleted.Add(new Tuple<Type, IEnumerable<object>>(type, ids));

            if (!CallsToDeletePerType.TryGetValue(type, out int n))
                CallsToDeletePerType[type] = 1;
            else
                CallsToDeletePerType[type] = n + 1;

            return 0;
        }

        protected override object NextFreeId(Type objectType, bool refresh = false)
        {
            if (refresh || !m_nextId.ContainsKey(objectType))
            {
                int nextId = Created.Where(x => x.Item1 == objectType).SelectMany(x => x.Item2).Count();
                m_nextId[objectType] = nextId;
                return nextId;
            }
            else
            { 
                int prev = m_nextId[objectType];
                int next = prev + 1;
                m_nextId[objectType] = next;
                return next;
            }
        }

        Dictionary<Type, int> m_nextId = new Dictionary<Type, int>();
    }
}

