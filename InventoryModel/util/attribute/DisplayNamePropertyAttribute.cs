// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS.InventoryModel.util.attribute
{
    /// <summary>
    /// Simple Attribute to mark which property to use as a display name for a given item
    /// Note: this is not how to display, it is which 1 property to display to user
    /// to identify this object
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class DisplayNamePropertyAttribute : Attribute
    {
        public DisplayNamePropertyAttribute() : base()
        {
        }
    }
}