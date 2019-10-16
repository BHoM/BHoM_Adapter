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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {

        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected virtual int Delete(Type type, string tag = "", Dictionary<string, object> config = null) 
        {
            if (tag == "")
            {
                return Delete(type, null as List<object>);
            }
            else
            {
                // Get all with tag
                IEnumerable<IBHoMObject> withTag = Read(type, tag);

                // Get indices of all with that tag only
                IEnumerable<object> ids = withTag.Where(x => x.Tags.Count == 1).Select(x => x.CustomData[AdapterId]).OrderBy(x => x);
                Delete(type, ids);

                // Remove tag if other tags as well
                IEnumerable<IBHoMObject> multiTags = withTag.Where(x => x.Tags.Count > 1);
                UpdateProperty(type, multiTags.Select(x => x.CustomData[AdapterId]), "Tags", multiTags.Select(x => x.Tags));

                return ids.Count();
            }
        }


        /***************************************************/
        /**** Protected CRUD Methods                    ****/
        /***************************************************/

        // This is the 
        // It must be implemented at the Toolkit level.
        // It gets called by the Push, in the context of the CRUD method.
        protected virtual int Delete(Type type, IEnumerable<object> ids)
        {
            return 0;
        }

    }
}
