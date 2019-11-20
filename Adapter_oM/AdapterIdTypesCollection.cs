using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapter
{

    //public class AdapterIdTypesCollection<T,A> : KeyedCollection<A, IAdapterIdFragment<T>>
    //{
    //    public bool AddOrReplace(IAdapterIdFragment<T> fragment)
    //    {
    //        int? idx = this.Dictionary?.Keys.ToList().IndexOf(fragment.GetType());

    //        if (idx == null || idx == -1)
    //            base.Add(fragment);
    //        else
    //            base.SetItem((int)idx, fragment);
    //        return true;
    //    }

    //    protected override A GetKeyForItem(IAdapterIdFragment<T> adapterIdFragment)
    //    {
    //        return adapterIdFragment.GetType();
    //    }
    //}
}
