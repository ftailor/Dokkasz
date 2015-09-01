using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dokkasz.Utilities
{
    public class PropertyAdapter<T>
    {
        private readonly PropertyInfo property;
        private readonly string displayName;

        public string DisplayName
        {
            get { return displayName; }
        }
        
        public PropertyAdapter(PropertyInfo property)
        {
            this.property = property;
            displayName = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name;
        }

        public object GetValue(T obj)
        {
            return property.GetValue(obj);
        }
    }
}
