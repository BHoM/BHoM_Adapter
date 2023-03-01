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
        public void PreprocessObjects(IEnumerable<object> objects)
        {
            IEnumerable<IBHoMObject> bhObjs = objects.OfType<IBHoMObject>();

            List<ILoad> loads = new List<ILoad>();
            Dictionary<Guid, IBHoMObject> nonLoads = new Dictionary<Guid, IBHoMObject>();

            //Split load obejcts from non-load objects
            foreach (IBHoMObject obj in bhObjs)
            {
                if (obj is ILoad load)
                    loads.Add(load);
                else if(!(obj is ICase))
                    nonLoads[obj.BHoM_Guid] = obj;
            }

            //If no non-load obejcts are being pushed, can simply return, as nothing can be replaced
            if (nonLoads.Count == 0)
                return;

            foreach (ILoad load in loads)
            {
                //Load through all loads, and try to update the objects
                ReplaceObjects(load as dynamic, nonLoads);
            }
        }


        private void ReplaceObjects<T>(IElementLoad<T> load, Dictionary<Guid, IBHoMObject> objects) where T : IBHoMObject
        {
            //Run through all elements stored on the load
            for (int i = 0; i < load.Objects.Elements.Count; i++)
            {
                //Try to find an item with the same guid in the non-load objects
                if (objects.TryGetValue(load.Objects.Elements[i].BHoM_Guid, out IBHoMObject replacement))
                {
                    //Ensure the found object is of the correct type
                    if (replacement is T tObject)
                    {
                        //replace with the other object
                        load.Objects.Elements[i] = tObject;
                    }
                }
            }
        }

        private void ReplaceObjects(ILoad load, Dictionary<Guid, IBHoMObject> objects)
        {
            //Do nothing for non-element loads
        }
    }
}
