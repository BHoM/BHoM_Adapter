/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Adapter.Module;
using System;
using System.Collections.Generic;
using System.Text;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace BH.Adapter.Modules.Structure
{
    [Description("Module for replacing the objects in the loads with objects with the same BHoM_Guid being pushed at the same time.\n" +
                 "No action is taken if loads are pushed in isolation, without the elements pushed at the same time as individual instances.")]
    public class ReplaceObjectsInLoadsModule : IPushPreProcessModule
    {
        public IEnumerable<IBHoMObject> PreprocessObjects(IEnumerable<IBHoMObject> objects)
        {
            List<ILoad> loads = new List<ILoad>();
            Dictionary<Guid, List<IBHoMObject>> nonLoads = new Dictionary<Guid, List<IBHoMObject>>();

            //Split load obejcts from non-load objects
            foreach (IBHoMObject obj in objects)
            {
                if (obj is ILoad load)
                    loads.Add(load);
                else if (!nonLoads.ContainsKey(obj.BHoM_Guid))
                    nonLoads[obj.BHoM_Guid] = new List<IBHoMObject> { obj };
                else
                    nonLoads[obj.BHoM_Guid].Add(obj);
            }

            //If no non-load obejcts are being pushed or the list does not contain any load, can simply return, as nothing can be replaced
            if (nonLoads.Count == 0 || loads.Count == 0)
                return objects;

            bool duplicatesFound = false;
            foreach (ILoad load in loads)
            {
                //Loop through all loads, and try to update the objects
                duplicatesFound |= ReplaceObjects(load as dynamic, nonLoads);
            }

            if (duplicatesFound)
                BH.Engine.Base.Compute.RecordWarning("Some objects pushed have duplicate BHoM_Guids. This means objects on loads not able to be updated.");

            //Returns the objects in order of first non-loads followed by loads
            //This ensures that the objects are pushed before loads
            //For many cases this will be handled by dependency types, but for cases where this is yet to be implemented, this solution helps fix the order
            return nonLoads.Values.SelectMany(x => x).Concat(loads);
        }


        private bool ReplaceObjects<T>(IElementLoad<T> load, Dictionary<Guid, List<IBHoMObject>> objects) where T : IBHoMObject
        {
            bool duplicatesFound = false;
            //Run through all elements stored on the load
            for (int i = 0; i < load.Objects.Elements.Count; i++)
            {
                //Try to find an item with the same guid in the non-load objects
                if (objects.TryGetValue(load.Objects.Elements[i].BHoM_Guid, out List<IBHoMObject> replacement))
                {
                    if (replacement.Count != 1)
                    {
                        duplicatesFound = true;
                        continue;
                    }
                    //Ensure the found object is of the correct type
                    if (replacement[0] is T tObject)
                    {
                        //replace with the other object
                        load.Objects.Elements[i] = tObject;
                    }
                }
            }
            return duplicatesFound;
        }

        private void ReplaceObjects(ILoad load, Dictionary<Guid, IBHoMObject> objects)
        {
            //Do nothing for non-element loads
        }
    }
}

