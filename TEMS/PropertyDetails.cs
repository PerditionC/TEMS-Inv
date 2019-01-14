// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using DW.WPFToolkit.Controls;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.util.attribute;


namespace TEMS_Inventory.UserControls
{
    public class PropertyDetails
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private TitledItemsControl DetailView = null;

        public PropertyDetails(TitledItemsControl DetailView)
        {
            this.DetailView = DetailView;
        }

        /// <summary>
        /// removes any automatically added items for view/editing
        /// </summary>
        public void ResetDetailView()
        {
            try
            {
                // cheat, we want to keep 1st two items (GUID and Name)
                Object[] keep = { DetailView.Items[0], DetailView.Items[1] };
                // remove any previously automatically added items
                DetailView.Items.Clear();
                // add back items to keep
                foreach (var i in keep) { DetailView.Items.Add(i); }
            }
            catch (Exception e)
            {
                logger.Warn(e);
                throw;
            }
        }

        /// <summary>
        /// adds a new TitledItem to DetailView
        /// </summary>
        /// <param name="title">short description of what this item represents</param>
        /// <param name="content">the item to display</param>
        private void AddDetailTitledItem(string title, object content)
        {
            try
            {
                // add our new item
                var ti = new TitledItem();
                ti.Title = title;
                ti.Content = content;
                DetailView.Items.Add(ti);
            }
            catch (Exception e)
            {
                logger.Warn(e);
                throw;
            }
        }

        /// <summary>
        /// create a TextBox and binds to given property
        /// </summary>
        /// <param name="boundPropName">property name value is bound to</param>
        /// <param name="tooltip">long description of what this item represents</param>
        /// <returns>initialized TextBox control</returns>
        private TextBox GetTextBox(string boundPropName, Object toolTip)
        {
            try
            {
                var tb = new TextBox();
                if (toolTip != null) tb.ToolTip = toolTip;

                // bind value to given property
                var binding = new Binding(boundPropName);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tb.SetBinding(TextBox.TextProperty, binding);
                // don't allow editing unless there is a backing object, ie in edit mode
                binding = new Binding("DataContext.isDetailViewInActive");
                binding.ElementName = "rootItem";
                tb.SetBinding(TextBox.IsReadOnlyProperty, binding);

                return tb;
            }
            catch (Exception e)
            {
                logger.Warn(e);
                throw;
            }
        }

        private CheckBox GetCheckBox(string boundPropName, Object toolTip)
        {
            try
            {
                var cb = new CheckBox();
                if (toolTip != null) cb.ToolTip = toolTip;

                // bind value to given property
                var binding = new Binding(boundPropName);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                cb.SetBinding(CheckBox.IsCheckedProperty, binding);

                cb.IsThreeState = false;

                return cb;
            }
            catch (Exception e)
            {
                logger.Warn(e);
                throw;
            }
        }

        /// <summary>
        /// create a TextBox limited to numbers with Up/Down controls to adjust value and binds to given property
        /// </summary>
        /// <param name="boundPropName">property name value is bound to</param>
        /// <param name="tooltip">long description of what this item represents</param>
        /// <param name="numberType"></param>
        /// <returns>initialized NumberBox control</returns>
        private NumberBox GetNumberBox(string boundPropName, Object toolTip, NumberType numberType)
        {
            try
            {
                var nb = new NumberBox();
                if (toolTip != null) nb.ToolTip = toolTip;

                // bind value to given property
                var binding = new Binding(boundPropName);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                nb.SetBinding(NumberBox.NumberProperty, binding);

                nb.NumberType = numberType;

                nb.Minimum = 0;
                //nb.Maximum = ?;
                nb.DefaultNumber = 0;

                // can bind to disable until some condition is met
                nb.HasCheckBox = false;
                nb.CheckBoxBehavior = NumberBoxCheckBoxBehavior.None;
                nb.IsChecked = false; // bind me
                nb.CheckBoxPosition = Dock.Left;

                // some other properties that normally don't need to be changed
                nb.Step = 1;
                nb.PredefinesCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                nb.NumberSelectionBehavior = NumberBoxSelection.OnFocusAndUpDown;
                nb.UpDownButtonsPosition = Dock.Right;
                nb.UpDownBehavior = UpDownBehavior.ArrowsAndButtons;

                nb.LostFocusBehavior = new LostFocusBehavior(ValueBehavior.PlaceDefaultNumber);
                nb.LostFocusBehavior.TrimLeadingZero = true;
                //nb.LostFocusBehavior.FormatText={}{0:D2}}

                return nb;
            }
            catch (Exception e)
            {
                logger.Warn(e);
                throw;
            }
        }

        /// <summary>
        /// create a Enhanced ComboBox and binds to given property
        /// </summary>
        /// <param name="boundPropName">property name value is bound to</param>
        /// <param name="tooltip">long description of what this item represents</param>
        /// <returns>initialized ComboBox control</returns>
        private ComboBox GetComboBox(string boundPropName, Object toolTip)
        {
            try
            {
                var cb = new EnhancedComboBox();
                if (toolTip != null) cb.ToolTip = toolTip;

                //<WPFToolkit:EnhancedComboBox IsEditable="True" InfoText="Required" InfoAppearance="OnEmpty" />
                // bind value to given property
                // WARNING: this assumes either Equals() has been overridden so the same
                // data loaded into two different instances (ie loading ComboBox values directly will be
                // a different object instance than same data loaded elsewhere (e.g. nested object)
                var binding = new Binding(boundPropName);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                cb.SetBinding(EnhancedComboBox.SelectedItemProperty, binding);
                cb.DisplayMemberPath = "name";

                cb.IsEditable = false;
                /* InfoText only used if items are directly editable, i.e. Collection of strings
                cb.InfoText = "hint";
                cb.InfoAppearance = InfoAppearance.OnEmpty;
                */

                return cb;
            }
            catch (Exception e)
            {
                logger.Warn(e);
                throw;
            }
        }

        /// <summary>
        /// Given a specific property, returns a pretty formated title (short description)
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private string GetTitle(PropertyInfo propertyInfo)
        {
            // see if field annotated with FieldLabel("prettyName") attribute, otherwise use Type name
            return (propertyInfo.PrettyName() ?? propertyInfo.Name[0].ToString().ToUpperInvariant() + propertyInfo.Name.Substring(1)) + ":";
            // return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(propertyInfo.Name)+":";
        }

        /// <summary>
        /// loop through all public properties of given item instance and creates 
        /// field to display/edit details
        /// </summary>
        /// <param name="GetNewItem">an instance of item to view details of</param>
        public void AddDetails(Func<ItemBase> GetNewItem)
        {
            // for all public and settable Properties, add editable item
            // just so we can the type get this instance's Type
            Type type = null;
            if (GetNewItem != null)
                type = GetNewItem()?.GetType();

            AddDetails(type);
        }

        /// <summary>
        /// loop through all public properties of given item instance and creates 
        /// field to display/edit details
        /// </summary>
        /// <param name="type">the Type of item to view details of</param>
        public void AddDetails(Type type)
        {
            // clean slate
            ResetDetailView();

            // exit early if type is unknown
            if (type == null) return;

            // for loading foreign keyed items
            var db = DataRepository.GetDataRepository;

            // get list of all Properties (note, not fields, must be public and settable)
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            // search thru and find ones we support
            foreach (var propertyInfo in propertyInfos)
            {
                try
                {
                    // add appropriate edit field based on type, but skip ones that don't need to stored
                    // which are conveniently marked as SQLIte.Ignore attribute; updated to use explicit HidePropertyAttribute
                    var attrs = propertyInfo.GetCustomAttributes(typeof(HidePropertyAttribute), true);
                    if (attrs.Length == 0) // not marked to ignore
                    {
                        // don't add if id or name property, added explicitly in XAML
                        // don't add isChanged either (read-only internal property)
                        if (!"id".Equals(propertyInfo.Name) && !"name".Equals(propertyInfo.Name) && !"IsChanged".Equals(propertyInfo.Name))
                        {
                            // support custom attributes to get better title and optional tool-tip
                            string title = GetTitle(propertyInfo);
                            Object toolTip = propertyInfo.ToolTip();

                            if (typeof(string) == propertyInfo.PropertyType)
                            {
                                AddDetailTitledItem(title, GetTextBox(propertyInfo.Name, toolTip));
                            }
                            else if (typeof(int) == propertyInfo.PropertyType)
                            {
                                AddDetailTitledItem(
                                    title,
                                    GetNumberBox(propertyInfo.Name, toolTip, NumberType.Int)
                                );
                            }
                            else if (typeof(double) == propertyInfo.PropertyType)
                            {
                                AddDetailTitledItem(
                                    title,
                                    GetNumberBox(propertyInfo.Name, toolTip, NumberType.Double)
                                );
                            }
                            else if (typeof(bool) == propertyInfo.PropertyType)
                            {
                                AddDetailTitledItem(
                                    title,
                                    GetCheckBox(propertyInfo.Name, toolTip)
                                );
                            }
                            else if (typeof(Guid) == propertyInfo.PropertyType)
                            {
                                // these as these are foreign keys, so can only view, not edit
                                var tb = GetTextBox(propertyInfo.Name, toolTip);
                                tb.IsReadOnly = true;
                                AddDetailTitledItem(title, tb);

                                // assume associated item xyz has same name without the trailing "Id"
                                var associatedPropName = propertyInfo.Name.Substring(0, propertyInfo.Name.Length - 2);
                                var fkItemPropertyInfo = propertyInfos.Where(p => p.Name.Equals(associatedPropName)).FirstOrDefault();
                                // but if no match to xyzId then do more thorough search, use first property with Type match
                                if (fkItemPropertyInfo == null)
                                {
                                    logger.Warn("Not Implemented!");
                                    /*
                                    var fkAttrs = propertyInfo.GetCustomAttributes(typeof(SQLiteNetExtensions.Attributes.ForeignKeyAttribute), true);
                                    if (fkAttrs.Length > 0)
                                    {
                                        var fkAttr = (SQLiteNetExtensions.Attributes.ForeignKeyAttribute)fkAttrs[0];
                                        fkItemPropertyInfo = propertyInfos.Where(p => p.PropertyType.Equals(fkAttr.ForeignType)).FirstOrDefault();
                                    }
                                    */
                                }
                                // add combobox to allow changing value, but limited to foreign table values
                                if (fkItemPropertyInfo != null)
                                {
                                    if (fkItemPropertyInfo.PropertyType.IsSubclassOf(typeof(ReferenceData)) ||
                                        (fkItemPropertyInfo.PropertyType == typeof(VendorDetail)))
                                    {
                                        // foreign key items
                                        var cb = GetComboBox(fkItemPropertyInfo.Name, fkItemPropertyInfo.ToolTip());
                                        cb.ItemsSource = db.db.InvokeLoadRows(fkItemPropertyInfo.PropertyType);
                                        AddDetailTitledItem(GetTitle(fkItemPropertyInfo), cb);
                                    }
                                }
                            }
                            else
                            {
                                // TODO
                                logger.Warn($"AddDetails for object { type.Name } with property { propertyInfo.Name }:{ propertyInfo.PropertyType.Name } unsupported Type.");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Warn(e);
                }
            }
        }
    }
}
