using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Materials;
using BH.oM.Base;
using BH.oM.Structural.Elements;

namespace BH.Adapter
{
    public static partial class StructuralPush //TODO: Update name
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static bool MapProperties(object target, object source, string idKey)
        {
            return _MapProperties(target as dynamic, source as dynamic, idKey);
        }


        ///***************************************************/
        ///**** Private Methods                           ****/
        ///***************************************************/

        private static bool _MapProperties(BHoMObject target, BHoMObject source, string idKey)
        {
            bool found = true;

            //Add tags from source to target
            foreach (string tag in source.Tags)
                target.Tags.Add(tag);

            //Check for id of the source and apply to the target
            object id;
            if (source.CustomData.TryGetValue(idKey, out id))
            {
                target.CustomData[idKey] = id;
                found = true;
            }

            return found;
        }

        /***************************************************/

        private static bool _MapProperties(Node target, Node source, string idKey)
        {
            bool found = true;

            //Add tags from source to target
            foreach (string tag in source.Tags)
                target.Tags.Add(tag);

            //Check if the source is constraint och taget not, if so add source constraint to target
            if (source.Constraint != null && target.Constraint == null)
                target.Constraint = source.Constraint;

            //If target does not have name, take sources name
            if (string.IsNullOrWhiteSpace(target.Name))
                target.Name = source.Name;

            //Check for id of the source and apply to the target
            object id;
            if (source.CustomData.TryGetValue(idKey, out id))
            {
                target.CustomData[idKey] = id;
                found = true;
            }

            return found;
        }
    }
}
