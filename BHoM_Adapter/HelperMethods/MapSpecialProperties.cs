using BH.oM.Base;
using BH.oM.Structure.Elements;

namespace BH.Adapter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool MapSpecialProperties(this IBHoMObject target, IBHoMObject source, string adapterKey) 
        {
            // Add tags from source to target
            foreach (string tag in source.Tags)
                target.Tags.Add(tag);

            // Map Properties Special Properties
            _MapSpecialProperties(target as dynamic, source as dynamic);

            // Check for id of the source and apply to the target
            bool found = true;
            object id;
            if (source.CustomData.TryGetValue(adapterKey, out id))
            {
                target.CustomData[adapterKey] = id;
                found = true;
            }
            return found;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void _MapSpecialProperties(IBHoMObject target, IBHoMObject source)
        {
        }

        /***************************************************/

        private static void _MapSpecialProperties(Node target, Node source)
        {
            //Check if the source is constraint och taget not, if so add source constraint to target
            if (source.Constraint != null && target.Constraint == null)
                target.Constraint = source.Constraint;

            //If target does not have name, take sources name //TODO: could that be done for all BHoM objects?
            if (string.IsNullOrWhiteSpace(target.Name))
                target.Name = source.Name;
        }

        /***************************************************/
    }
}
