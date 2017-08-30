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
    public static partial class StructuralPusher
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static bool MapProperties(this BHoMObject target, BHoMObject source, string idKey) //TODO: This is quite a general method that could be elevated to the Engine
        {
            // Add tags from source to target
            foreach (string tag in source.Tags)
                target.Tags.Add(tag);

            // Map Properties Special Properties
            _MapSpecialProperties(target as dynamic, source as dynamic, idKey);

            // Check for id of the source and apply to the target
            bool found = true;
            object id;
            if (source.CustomData.TryGetValue(idKey, out id))
            {
                target.CustomData[idKey] = id;
                found = true;
            }
            return found;
        }


        ///***************************************************/
        ///**** Private Methods                           ****/
        ///***************************************************/

        private static void _MapSpecialProperties(BHoMObject target, BHoMObject source, string idKey)
        {
        }

        /***************************************************/

        private static void _MapSpecialProperties(Node target, Node source, string idKey)
        {
            //Check if the source is constraint och taget not, if so add source constraint to target
            if (source.Constraint != null && target.Constraint == null)
                target.Constraint = source.Constraint;

            //If target does not have name, take sources name //TODO: could that be done for all BHoM objects?
            if (string.IsNullOrWhiteSpace(target.Name))
                target.Name = source.Name;
        }
    }
}