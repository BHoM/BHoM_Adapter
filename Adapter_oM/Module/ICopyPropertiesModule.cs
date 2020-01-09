using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.oM.Adapter
{
    public interface ICopyPropertiesModule : IAdapterModule 
    {
    }

    public interface ICopyPropertiesModule<T> : ICopyPropertiesModule where T : IBHoMObject
    {
        void CopyProperties(T target, T source);
    }
}
