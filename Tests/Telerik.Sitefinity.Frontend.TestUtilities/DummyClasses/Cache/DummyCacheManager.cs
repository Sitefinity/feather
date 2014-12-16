using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache
{
    public class DummyCacheManager : ICacheManager
    {
        public void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
        {
        }

        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public object GetData(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get
            {
                return null;
            }

            set
            {
            }
        }
    }
}
