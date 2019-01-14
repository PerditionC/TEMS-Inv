// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS.InventoryModel.util.attribute
{
    /// <summary>
    /// Simple Attribute to mark which properties to not display to a user
    /// When showing a propertly list or other auto-generated list of values
    /// do not show this one to user unless explicitly requested, e.g. DB key
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class HidePropertyAttribute : Attribute
    {
        public HidePropertyAttribute() : base()
        {
        }
    }
}