// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Linq;
using System.Reflection;

namespace TEMS.InventoryModel.util.attribute
{
    /// <summary>
    /// Simple Attribute to allow labeling data models, including with ToolTips
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class FieldLabelAttribute : Attribute
    {
        public FieldLabelAttribute(string PrettyName) : base()
        {
            this.PrettyName = PrettyName;
        }

        /// <summary>
        /// Name to use for a header, title or other places where value is a user facing label
        /// </summary>
        public string PrettyName { get; private set; }

        /// <summary>
        /// A long description or other information useful to user as a tool tip
        /// </summary>
        public Object ToolTip { get; set; } = null;
    }

    /// <summary>
    /// Provides simple extension method to get FieldLabel information from PropertyInfo instance
    /// </summary>
    public static class FieldLabelAttributeHelperExtension
    {
        public static string PrettyName(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttributes(typeof(FieldLabelAttribute), true).FirstOrDefault<object>();
            return (attr as FieldLabelAttribute)?.PrettyName;
        }

        public static Object ToolTip(this PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(FieldLabelAttribute), true);
            if (attrs.Length > 0)
            {
                var attr = (FieldLabelAttribute)attrs[0];
                return attr.ToolTip;
            }

            return null;
        }
    }
}