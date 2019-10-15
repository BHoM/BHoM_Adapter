/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.Engine.Reflection;
using BH.Engine.Data;
using System;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {

        /***************************************************/
        /**** Protected Abstract CRUD Method            ****/
        /***************************************************/

        public virtual int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue)
        {
            return 0;
        }


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        public int PullUpdatePush(FilterRequest filter, string property, object newValue) 
        {
            if (Config.ProcessInMemory)
            {
                IEnumerable<IBHoMObject> objects = UpdateInMemory(filter, property, newValue);
                Create(objects);
                return objects.Count();
            }
            else
                return UpdateThroughAPI(filter, property, newValue);
        }


        /***************************************************/
        /**** Helper Methods                            ****/
        /***************************************************/

        public IEnumerable<IBHoMObject> UpdateInMemory(FilterRequest filter, string property, object newValue)
        {
            // Pull the objects to update
            IEnumerable<IBHoMObject> objects = Read(filter.Type);

            // Set the property of the objects matching the filter
            filter.FilterData(objects).ToList().SetPropertyValue(filter.Type, property, newValue);

            return objects;
        }

        /***************************************************/

        public int UpdateThroughAPI(FilterRequest filter, string property, object newValue)
        {
            IEnumerable<object> ids = Pull(filter).Select(x => ((IBHoMObject)x).CustomData[AdapterId]);
            return UpdateProperty(filter.Type, ids, property, newValue);
        }
    }
}
