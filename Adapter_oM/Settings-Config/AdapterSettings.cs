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

using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.Adapter
{
    [Description("General settings for any Adapter, to be specified when instantiating the Adapter." +
        "Implement this class to make your own Toolkit settings, e.g. SpeckleAdapterSettings.")]
    public class AdapterSettings : IObject
    {
        /****************************************/
        /****         Push settings         *****/
        /****************************************/

        [Description("If your Toolkit needs support for non-BHoM objects, set this to true.")]
        public virtual bool WrapNonBHoMObjects { get; set; } = false;

        [Description("If your Toolkit does not support the Full Push (FullCRUD method), you can select another behaviour here (e.g. CreateOnly).")]
        public PushType DefaultPushType { get; set; } = PushType.FullPush;

        [Description("Deep clones the objects before Pushing them." +
            "As the objects get modified during the Push (e.g. their externalId is added to them)," +
            "this avoids backpropagation in visual programming environments like Grasshopper.")]
        public virtual bool CloneBeforePush { get; set; } = true;


        /****************************************/
        /****         Pull settings         *****/
        /****************************************/

        public PullType DefaultPullType { get; set; } // no setting on this yet.


        /****************************************/
        /****     General CRUD settings     *****/
        /****************************************/

        [Description("If your adapter does not define DependencyTypes, this can be set to false for performance.")]
        public virtual bool HandleDependencies { get; set; } = true;
        public virtual bool UseAdapterId { get; set; } = true;
        public virtual bool UseHashComparerAsDefault { get; set; } = false;
        public virtual bool ProcessInMemory { get; set; } = false;
        [Description("If true, Objects found to be the same according to the AdapterComparer for the provided type are checked for full equality using the HashComparer of the type (which by default checks every property except BHoM_Guid). If equal according to the HashComparer, they are not updated.\n" + 
                     "Otherwise, Objects found identical according to the AdapterComparer for the provided type are sent to the Update method.")]
        public virtual bool OnlyUpdateChangedObjects { get; set; } = true;

        [Description("If true, CRUD operations will attempt read/write cache objects to speed things during a same Action.")]
        public virtual bool CacheCRUDobjects { get; set; } = true;

        /****************************************/
        /****      CreateOnly settings      *****/
        /****************************************/
        [Description("If true, CreateOnly method calls Distinct() on the first-level objects.")]
        public virtual bool CreateOnly_DistinctObjects { get; set; } = false;
        [Description("If true, CreateOnly method calls Distinct() on the dependency objects.")]
        public virtual bool CreateOnly_DistinctDependencies { get; set; } = true;
    }
}





