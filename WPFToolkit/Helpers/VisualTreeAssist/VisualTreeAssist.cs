#region License
/*
The MIT License (MIT)

Copyright (c) 2009-2016 David Wendland

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE
*/
#endregion License

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Brings many possibilities to find elements in the visual tree. See <see cref="System.Windows.Media.VisualTreeHelper" />.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// <![CDATA[
    /// var childButton = VisualTreeAssist.FindChild<Button>(this);
    /// 
    /// var namedChildTextBox = VisualTreeAssist.FindNamedChild<TextBox>(this, "PART_TextBox");
    /// 
    /// var firstUserControlInWindow = VisualTreeAssist.GetParentsUntil<UserControl, Window>(this);
    /// ]]>
    /// </code>
    /// </example>
    public static class VisualTreeAssist
    {
        /// <summary>
        /// Searches for a parent control by its type.
        /// </summary>
        /// <typeparam name="TParentType">The type of the parent control to search for.</typeparam>
        /// <param name="item">The child control where the search is start from. If this its null the default(TParentType) will returned.</param>
        /// <returns>The found parent control if any; otherwise default(TParentType). Its also default(TParentType) if the passed item is null.</returns>
        public static TParentType FindParent<TParentType>(DependencyObject item) where TParentType : DependencyObject
        {
            if (item == null)
                return default(TParentType);

            var parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TParentType))
                parent = VisualTreeHelper.GetParent(parent);

            return parent != null ? (TParentType)parent : default(TParentType);
        }

        /// <summary>
        /// Searches for a parent control by its name.
        /// </summary>
        /// <param name="item">The child control where the search is start from. If this its null this method returns null.</param>
        /// <param name="name">The name of the parent control to search for. If this its null, empty or just whitespaces this method returns null.</param>
        /// <returns>The found parent control if any; otherwise null. Its also null if the passed item is null or the passed parent name is null, empty or just whitespaces.</returns>
        public static DependencyObject FindNamedParent(DependencyObject item, string name)
        {
            if (item == null || string.IsNullOrWhiteSpace(name))
                return null;

            var parent = VisualTreeHelper.GetParent(item);
            while (parent != null)
            {
                if (HasName(parent, name))
                    return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Searches for a parent control by its type and name.
        /// </summary>
        /// <typeparam name="TParentType">The type of the parent control to search for.</typeparam>
        /// <param name="item">The child control where the search is start from. If this its null the default(TParentType) will returned.</param>
        /// <param name="name">The name of the parent control to search for. If this its null, empty or just whitespaces this method returns null.</param>
        /// <returns>The found parent control if any; otherwise default(TParentType). Its also default(TParentType) if the passed item is null or the name is null, empty or just whitespaces.</returns>
        public static TParentType FindNamedParent<TParentType>(DependencyObject item, string name) where TParentType : DependencyObject
        {
            var foundItem = FindNamedParent(item, name);
            return foundItem is TParentType ? (TParentType)foundItem : default(TParentType);
        }

        /// <summary>
        /// Gets all parent controls with the specific type.
        /// </summary>
        /// <typeparam name="TParentType">The type of the parent controls to search for.</typeparam>
        /// <param name="item">The child control where the search is start from. If this is null the returned list is empty.</param>
        /// <returns>A list of found parent controls. This is empty if no control has been found.</returns>
        public static List<TParentType> GetParents<TParentType>(DependencyObject item) where TParentType : DependencyObject
        {
            var parents = new List<TParentType>();
            var parent = FindParent<TParentType>(item);
            while (parent != null)
            {
                parents.Add(parent);
                parent = FindParent<TParentType>(parent);
            }
            return parents;
        }

        /// <summary>
        /// Gets the amount of parent controls which are the given type.
        /// </summary>
        /// <typeparam name="TParentType">The type of the parent control to check for.</typeparam>
        /// <param name="item">The child control where the search is start from. If this its null this method will return 0.</param>
        /// <returns>The amount of found parent controls with the specific type. 0 if nothing is found or the passed item is null.</returns>
        public static int GetParentCount<TParentType>(DependencyObject item) where TParentType : DependencyObject
        {
            return GetParents<TParentType>(item).Count;
        }

        /// <summary>
        /// Gets all parent controls with the specific type as long no control with the second control type is found.
        /// </summary>
        /// <typeparam name="TParentType">The type of the parent controls to search for.</typeparam>
        /// <typeparam name="TEndType">The type of the parent control when to stop searching.</typeparam>
        /// <param name="item">The child control where the search is start from. If this is null the returned list is empty.</param>
        /// <returns>A list of found parent controls. This is empty if no control has been found.</returns>
        public static List<TParentType> GetParentsUntil<TParentType, TEndType>(DependencyObject item) where TParentType : DependencyObject where TEndType : DependencyObject
        {
            var parents = new List<TParentType>();
            if (item == null)
                return parents;

            var parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TEndType))
            {
                if (parent is TParentType)
                    parents.Add((TParentType)parent);
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parents;
        }

        /// <summary>
        /// Gets the amount of parent controls which are the given type as long no control with the second control type is found.
        /// </summary>
        /// <typeparam name="TParentType">The type of the parent control to check for.</typeparam>
        /// <typeparam name="TEndType">The type of the parent control when to stop counting.</typeparam>
        /// <param name="item">The child control where the search is start from. If this its null this method will return 0.</param>
        /// <returns>The amount of found parent controls with the specific type. 0 if nothing is found or the passed item is null.</returns>
        public static int GetParentsUntilCount<TParentType, TEndType>(DependencyObject item) where TParentType : DependencyObject where TEndType : DependencyObject
        {
            return GetParentsUntil<TParentType, TEndType>(item).Count;
        }

        /// <summary>
        /// Searches for a specific child element by its type.
        /// </summary>
        /// <typeparam name="TChildType">The type of the child control to search for.</typeparam>
        /// <param name="item">The control where the search is start from. If this its null the default(TChildType) will returned.</param>
        /// <returns>The found child control if any; otherwise default(TChildType). Its also default(TParentType) if the passed item is null.</returns>
        public static TChildType FindChild<TChildType>(DependencyObject item) where TChildType : DependencyObject
        {
            if (item == null)
                return default(TChildType);

            var childrenCount = VisualTreeHelper.GetChildrenCount(item);
            for (int i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(item, i);
                if (child is TChildType)
                    return (TChildType)child;
                
                var foundChild = FindChild<TChildType>(child);
                if (foundChild != null)
                    return foundChild;
            }

            return default(TChildType);
        }

        /// <summary>
        /// Searches for a specific child element by its name.
        /// </summary>
        /// <param name="item">The control where the search is start from. If this its null this method returns null.</param>
        /// <param name="name">The name of the child control to search for. If this its null, empty or just whitespaces this method returns null.</param>
        /// <returns>The found child control if any; otherwise null. It is also null if the passed item is null or the passed name is null, empty or just whitespaces.</returns>
        public static DependencyObject FindNamedChild(DependencyObject item, string name)
        {
            if (item == null || string.IsNullOrWhiteSpace(name))
                return null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(item);
            for (int i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(item, i);
                if (HasName(child, name))
                    return child;
                
                var foundChild = FindNamedChild(child, name);
                if (foundChild != null)
                    return foundChild;
            }

            return null;
        }

        /// <summary>
        /// Searches for a specific child element by its type and name
        /// </summary>
        /// <typeparam name="TChildType">The type of the child control to search for.</typeparam>
        /// <param name="item">The control where the search is start from. If this its null this method returns default(TChildType).</param>
        /// <param name="name">The name of the child control to search for if any; otherwise default(TChildType). If this its null, empty or just whitespaces this method returns default(TChildType).</param>
        /// <returns>The found child control if any; otehrwise default(TChildType). It is also default(TChildType) if the item passed is null or the passed name is null, empty or just whitespaces.</returns>
        public static TChildType FindNamedChild<TChildType>(DependencyObject item, string name) where TChildType : DependencyObject
        {
            var foundItem = FindNamedChild(item, name);
            return foundItem is TChildType ? (TChildType)foundItem : default(TChildType);
        }

        /// <summary>
        /// Gets all child elements with a specific type.
        /// </summary>
        /// <typeparam name="TChildType">The type of the child control to search for.</typeparam>
        /// <param name="item">The control where the search is start from. If this its null this method returns an empty list.</param>
        /// <returns>A list of found child controls. This is empty if no control has been found.</returns>
        public static List<TChildType> GetChildren<TChildType>(DependencyObject item) where TChildType : DependencyObject
        {
            var children = new List<TChildType>();
            GetChildren(item, children);
            return children;
        }

        private static void GetChildren<TChildType>(DependencyObject item, ICollection<TChildType> target) where TChildType : DependencyObject
        {
            if (item == null)
                return;

            var childrenCount = VisualTreeHelper.GetChildrenCount(item);
            for (var i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(item, i);
                if (child is TChildType)
                    target.Add((TChildType)child);
                GetChildren(child, target);
            }
        }

        /// <summary>
        /// Gets the amount of child elements by their type.
        /// </summary>
        /// <typeparam name="TChildType">The type of the child control to check for.</typeparam>
        /// <param name="item">The control where the search is start from. If this its null this method returns 0.</param>
        /// <returns>The amount of child controls by the specific type if any; otherwise 0. It is also 0 if the passed item is null.</returns>
        public static int GetChildrenCount<TChildType>(DependencyObject item) where TChildType : DependencyObject
        {
            return GetChildren<TChildType>(item).Count;
        }

        private static bool HasName(DependencyObject item, string name)
        {
            return item is FrameworkElement &&
                   ((FrameworkElement)item).Name == name;
        }
    }
}
