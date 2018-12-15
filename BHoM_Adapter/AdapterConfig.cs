namespace BH.Adapter
{
    public class AdapterConfig
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public bool ProcessInMemory { get; set; } = true;

        public bool SeparateProperties { get; set; } = false;

        public bool MergeWithComparer { get; set; } = false;

        public bool UseAdapterId { get; set; } = true;

        public bool CloneBeforePush { get; set; } = true;

        /***************************************************/
    }
}

