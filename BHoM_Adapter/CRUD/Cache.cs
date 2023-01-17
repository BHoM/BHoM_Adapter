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
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using BH.oM.Data.Requests;
using BH.oM.Adapter;
using BH.Engine.Adapter;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Basic methods                             ****/
        /***************************************************/

        protected bool CreateAndCache<T>(IEnumerable<T> objects, ActionConfig actionConfig = null) where T : IBHoMObject
        {
            if (!ICreate(objects, actionConfig))
                return false;

            Type t = typeof(T);
            Dictionary<object, IBHoMObject> typeCache;
            if (!m_cache.TryGetValue(t, out typeCache))
                typeCache = new Dictionary<object, IBHoMObject>();

            if (m_AdapterSettings.UseAdapterId)
            {
                foreach (IBHoMObject bhObj in objects)
                {
                    typeCache[this.GetAdapterId(bhObj)] = bhObj;
                }
            }
            else
            {
                // For adapters not using Ids, simply cache with integers counting up from 0.

                int n = typeCache.Count;
                foreach (IBHoMObject bhObj in objects)
                {
                    typeCache[n] = bhObj;
                    n++;
                }
            }

            m_cache[t] = typeCache;

            return true;
        }

        /***************************************************/

        protected virtual IEnumerable<T> GetCachedOrRead<T>(string tag = "", ActionConfig actionConfig = null) where T : IBHoMObject
        {
            // Call the Basic Method Read() to get the objects based on the ids
            IEnumerable<T> objects = GetCachedOrRead<T>(new List<object>(), actionConfig);

            // Null guard
            objects = objects ?? new List<T>();

            // Filter by tag if any 
            if (tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(tag));
        }

        /***************************************************/

        protected List<T> GetCachedOrRead<T>(IList ids, ActionConfig actionConfig = null)
        {
            return GetCachedOrReadAsDictionary<object, T>(ids, actionConfig).Values.ToList();
        }

        /***************************************************/

        protected Dictionary<TId, TObj> GetCachedOrReadAsDictionary<TId, TObj>(IList ids = null, ActionConfig actionConfig = null)
        {

            Type t = typeof(TObj);
            if (ids == null || ids.Count == 0)
            {
                if (m_FullyCachedTypes.Contains(typeof(TObj)))
                {
                    Dictionary<object, IBHoMObject> typeCache;
                    if (m_cache.TryGetValue(t, out typeCache))
                        return typeCache.ToDictionary(x => (TId)x.Key, x => (TObj)x.Value);
                    else
                        return new Dictionary<TId, TObj>();
                }
                else
                {
                    // Because we are in the case of no Ids specified, we want to be reading the entire model.
                    // There may be objects that may have been read, although objects of their type may not be all read yet.
                    // This means that, because there is no way of knowing which ids (objects) are missing to be read,
                    // we need to re-read the entire model and cache it fully, although some reads may be redundant.
                    IEnumerable<IBHoMObject> readObjects = IRead(t, ids, actionConfig);
                    m_FullyCachedTypes.Add(t);
                    Dictionary<object, IBHoMObject> typeCache;
                    if (m_AdapterSettings.UseAdapterId)
                    {
                        typeCache = readObjects.ToDictionary(x => this.GetAdapterId(x), x => x);
                        m_cache[t] = typeCache;
                    }
                    else
                    {
                        // For adapters not using Ids, simply cache with integers counting up from 0.
                        typeCache = new Dictionary<object, IBHoMObject>();

                        int n = 0;
                        foreach (IBHoMObject bhObj in readObjects)
                        {
                            typeCache[n] = bhObj;
                            n++;
                        }

                        m_cache[t] = typeCache;
                    }

                    return typeCache.ToDictionary(x => (TId)x.Key, x => (TObj)x.Value);
                }
            }
            else
            {
                Dictionary<object, IBHoMObject> filteredObjects = new Dictionary<object, IBHoMObject>();
                Dictionary<object, IBHoMObject> typeCache;
                if (m_cache.TryGetValue(t, out typeCache))
                {
                    List<object> idsNotInCache = new List<object>();
                    List<IBHoMObject> cachedObjects = new List<IBHoMObject>();

                    //Loop through the ids and try to fetch from cache.
                    foreach (object id in ids)
                    {
                        if (typeCache.TryGetValue(id, out IBHoMObject obj))
                        {
                            cachedObjects.Add(obj);
                            filteredObjects[id] = obj;
                        }
                        else
                            idsNotInCache.Add(id);
                    }

                    if (idsNotInCache.Count > 0)
                    {
                        IEnumerable<IBHoMObject> additionalObjects = IRead(t, idsNotInCache, actionConfig);

                        if (m_AdapterSettings.UseAdapterId)
                        {
                            foreach (IBHoMObject bhObj in additionalObjects)
                            {
                                object id = this.GetAdapterId(bhObj);
                                typeCache[id] = bhObj;
                                filteredObjects[id] = bhObj;
                            }
                        }
                        else
                        {
                            int n = typeCache.Count;
                            foreach (IBHoMObject bhObj in additionalObjects)
                            {
                                typeCache[n] = bhObj;
                                filteredObjects[n] = bhObj;
                                n++;
                            }
                        }
                        m_cache[t] = typeCache;
                    }

                    return filteredObjects.ToDictionary(x => (TId)x.Key, x => (TObj)x.Value);
                }
                else
                {
                    typeCache = new Dictionary<object, IBHoMObject>();
                    IEnumerable<IBHoMObject> readObjects = IRead(t, ids, actionConfig);

                    if (m_AdapterSettings.UseAdapterId)
                    {
                        foreach (IBHoMObject bhObj in readObjects)
                        {
                            typeCache[this.GetAdapterId(bhObj)] = bhObj;
                        }
                    }
                    else
                    {
                        int n = typeCache.Count;
                        foreach (IBHoMObject bhObj in readObjects)
                        {
                            typeCache[n] = bhObj;
                            n++;
                        }
                    }
                    m_cache[t] = typeCache;

                    return typeCache.ToDictionary(x => (TId)x.Key, x => (TObj)x.Value);
                }
            }
        }

        /***************************************************/

        protected virtual bool UpdateIncludingCache<T>(IEnumerable<T> objects, ActionConfig actionConfig = null) where T : IBHoMObject
        {
            if (!IUpdate(objects, actionConfig))
                return false;

            if (!m_AdapterSettings.UseAdapterId)
            {
                Engine.Base.Compute.RecordWarning("Unable to update cache for adapters not using AdapterIds.");
                return true;
            }

            Type t = typeof(T);
            Dictionary<object, IBHoMObject> typeCache;
            if (!m_cache.TryGetValue(t, out typeCache))
                typeCache = new Dictionary<object, IBHoMObject>();

            foreach (IBHoMObject bhObj in objects)
            {
                typeCache[this.GetAdapterId(bhObj)] = bhObj;
            }

            m_cache[t] = typeCache;

            return true;
        }

        /***************************************************/

        protected virtual int UpdateTagsIncludingCache<T>(IEnumerable<object> ids, IEnumerable<HashSet<string>> newTags, ActionConfig actionConfig = null)
        {
            Type t = typeof(T);
            int updateCount = IUpdateTags(t, ids, newTags, actionConfig);

            if (updateCount == 0)
                return 0;

            if (!m_AdapterSettings.UseAdapterId)
            {
                Engine.Base.Compute.RecordWarning("Unable to update cache for adapters not using AdapterIds.");
                return updateCount;
            }

            Dictionary<object, IBHoMObject> typeCache;
            if (!m_cache.TryGetValue(t, out typeCache))
                typeCache = new Dictionary<object, IBHoMObject>();

            List<object> idsList = ids.ToList();
            List<HashSet<string>> tagsList = newTags.ToList();

            if (idsList.Count != tagsList.Count)
                return updateCount; //Not raising a warning here due to the assumption that the concrete adapter will already have done that

            for (int i = 0; i < idsList.Count; i++)
            {
                object id = idsList[i];
                IBHoMObject bhObj;
                if (typeCache.TryGetValue(id, out bhObj))
                    bhObj.Tags = tagsList[i];
            }

            m_cache[t] = typeCache;

            return updateCount;
        }

        /***************************************************/

        protected int DeleteIncludingCache<T>(IEnumerable<object> ids, ActionConfig actionConfig = null) where T : IBHoMObject
        {
            Type t = typeof(T);
            int deleteCount = IDelete(t, ids, actionConfig);
            if (deleteCount == 0)
                return deleteCount;

            if (ids == null || !ids.Any())
            {
                m_FullyCachedTypes.Remove(t);
                m_cache.Remove(t);
            }
            else
            {
                Dictionary<object, IBHoMObject> typeCache;
                if (m_cache.TryGetValue(t, out typeCache))
                {
                    foreach (object id in ids)
                    {
                        typeCache.Remove(id);
                    }
                }
                m_cache[t] = typeCache;
            }

            return deleteCount;
        }

        /***************************************************/

        protected void ClearCache()
        {
            m_cache = new Dictionary<Type, Dictionary<object, IBHoMObject>>();
            m_FullyCachedTypes = new HashSet<Type>();
        }

        /***************************************************/

        private Dictionary<Type, Dictionary<object, IBHoMObject>> m_cache = new Dictionary<Type, Dictionary<object, IBHoMObject>>();
        private HashSet<Type> m_FullyCachedTypes = new HashSet<Type>();
    }
}