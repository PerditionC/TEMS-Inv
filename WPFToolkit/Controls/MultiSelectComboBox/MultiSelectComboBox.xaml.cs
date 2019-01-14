// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

#region license
/// based on MultiSelectComboBox - https://www.codeproject.com/Articles/563862/Multi-Select-ComboBox-in-WPF
/// Santhosh Kumar Jayaraman 
/// License: The Code Project Open License (CPOL) 1.02, see CPOL.htm
#endregion license

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        private Node AllNode = null;

        #region constructor

        public MultiSelectComboBox()
        {
            InitializeComponent();
            AllNode = new Node("All", false);
            _nodeList = new ObservableDictionary<object, Node>();
        }

        #endregion // constructor

        /// <summary>
        /// observable collection of items to display in dropdown
        /// node list is rebuilt each time there is a change to items source
        /// </summary>
        public ObservableDictionary<object, Node> _nodeList
        {
            get { return (ObservableDictionary<object, Node>)GetValue(NodeListProperty); }
            set { SetValue(NodeListProperty, value); }
        }
        private static readonly DependencyProperty NodeListProperty =
            DependencyProperty.Register("_nodeList", typeof(ObservableDictionary<object, Node>), typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(null));


        #region Dependency Properties

        public static readonly DependencyProperty TitleFuncProperty =
            DependencyProperty.Register("TitleFunc", typeof(Func<object, string>), typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<object>), typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnItemsSourceChanged)));

        // Note: in XAML SelectedItems should be bound after ItemsSource (ie order of initialization)
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList<object>), typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnSelectedItemsChanged)));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty MaxTextLengthProperty =
            DependencyProperty.Register("MaxTextLength", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));


        /// <summary>
        /// Delegate that takes an object from ItemsSource collection and returns Title to use in Combobox
        /// </summary>
        public Func<object, string> TitleFunc
        {
            get { return (Func<object, string>)GetValue(TitleFuncProperty); }
            set { SetValue(TitleFuncProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection used to generate the content of the MultiSelectComboBox
        /// Expects a ObservableCollection with arbitrary object
        /// </summary>
        public IList<object> ItemsSource
        {
            get { return (IList<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection of objects from ItemsSource that are currently selected
        /// Expects a ObservableCollection with arbitrary object
        /// Note: due to implementation details SelectedItems property triggers a change
        /// due to new collection on every item [un]selection.
        /// </summary>
        public IList<object> SelectedItems
        {
            get { return (IList<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets text displayed in the textbox portion of the combobox
        /// i.e. a simple string representation of selected items
        /// Note: may not directly correlate to list of all selected items
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text displayed when nothing is selected
        /// </summary>
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum length (in characters) to display of selected item list
        /// If list exceeds then remaining characters are replaced with ...
        /// </summary>
        public string MaxTextLength
        {
            get { return (string)GetValue(MaxTextLengthProperty); }
            set { SetValue(MaxTextLengthProperty, value); }
        }

        #endregion

        #region Events

        /// <summary>
        /// triggered when the ItemsSource collection is set/changed
        /// </summary>
        /// <param name="d">the MultiSelectComboBox object</param>
        /// <param name="e">unused</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MultiSelectComboBox control = (MultiSelectComboBox)d;
                control.CreateInternalNodeList();
            }
        }

        /// <summary>
        /// triggered when the SelectedItems collection is set/changed
        /// </summary>
        /// <param name="d">the MultiSelecComboBox object</param>
        /// <param name="e">unused</param>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                MultiSelectComboBox control = (MultiSelectComboBox)d;
                if (control != null)
                {
                    control.SelectNodes();
                    control.SetText();
                }
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox clickedBox = (CheckBox)sender;
            if (sender == null) return;

            // handle user [un]checking "All" option
            if (clickedBox.Content?.ToString() == "All")
            {
                // ensure all nodes are [un]selected, IsSelected = "All".IsChecked
                foreach (KeyValuePair<object, Node> kvp in _nodeList)
                {
                    var node = kvp.Value;
                    node.IsSelected = clickedBox.IsChecked.Value;
                }
            }
            else // other than "All" selected, see if causes all options (except All) to be [un]checked
            {
                var AllIsSelected = true;
                foreach (KeyValuePair<object, Node> kvp in _nodeList)
                {
                    var node = kvp.Value;

                    // except "All", every checkbox must be selected for All to be set
                    AllIsSelected = (node.IsSelected || (node.Title == "All"));

                    // exit early once one found that is unchecked [and not All]
                    if (!AllIsSelected) break;
                }
                // set or clear "All" accordingly
                AllNode.IsSelected = AllIsSelected;
            }

            // update SelectedItems property to match internal selected nodes representation
            // triggers OnSelectedItemsChanged() which calls SetText() to update [truncated] 
            // simple list of selected items to be displayed when dropdown closed
            UpdateSelectedItems();
        }
        #endregion


        #region Methods
        // a new SelectedItems collection is assigned to SelectedItems property
        // updates internal representation to match items selected in new SelectedItems collection
        private void SelectNodes()
        {
            // loop through set all nodes in internal _nodeList to unselected (baseline)
            // then loop through all SelectedItems and set corresponding internal node to selected
            // Note: we assume an object from SelectedItems hashes to same object from ItemsSource
            foreach (KeyValuePair<object, Node> kvp in _nodeList)
            {
                kvp.Value.IsSelected = false;
            }
            foreach (var selItem in SelectedItems)
            {
                _nodeList[selItem].IsSelected = true;
            }
            AllNode.IsSelected = SelectedItems.Count == ItemsSource.Count;
        }

        // a checkbox corresponding to a node was selected, update SelectedItems property 
        // to match internal node list's idea of selected items
        private void UpdateSelectedItems()
        {
            // initialize to none selected, then looping through internal node list
            // adding all selected nodes to SelectedItems collection

            // Warning: SelectedItems may be set to same collection as ItemsSource.
            // We want to always trigger a SelectedItems property change so we
            // always create a new instance.
            IList<object> _selectedItems = new List<object>(ItemsSource?.Count ?? 0);

            // add all selected nodes to SelectedItems collection
            foreach (KeyValuePair<object, Node> kvp in _nodeList)
            {
                // node.IsSelected is bound to Checkbox.IsSelected so reflects current checked state
                if (kvp.Value.IsSelected && (kvp.Value.Title != "All"))
                {
                    _selectedItems.Add(kvp.Key);
                }
            }

            // warning: we must do this after adding selected items as this may trigger
            // SelectNodes() call which resets internal state
            SelectedItems = _selectedItems;
        }

        // initialize _nodeList from ItemsSource
        // and set XAML combobox controls items source to newly generated _nodeList
        // Note: initialized with nothing selected, set SelectedItems (after in XAML) to set which items are initially selected
        private void CreateInternalNodeList()
        {
            // invalidate any existing nodes
            _nodeList.Clear();

            // add shadow copies to our internal node list

            // only add "All" option if at least one choice, otherwise leave empty
            if (this.ItemsSource.Count > 0)
            {
                _nodeList.Add(AllNode, AllNode);
            }
            foreach (var item in this.ItemsSource)
            {
                // get Title, if provided use accessor delegate function, otherwise fallback to ToString()
                string nodeTitle = (TitleFunc != null) ? TitleFunc(item) : item.ToString();

                // add new Node
                Node node = new Node(nodeTitle, false);
                _nodeList.Add(item, node);
            }

            // update XAML combobox control
            MultiSelectCombo.ItemsSource = _nodeList;

            // clear existing selected items
            // When intialized the first time this will be overridden by SelectedItems binding and not passed to ViewModel
            SelectedItems = null;
        }


        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                if (AllNode.IsSelected)
                {
                    this.Text = AllNode.Title;
                }
                else
                {
                    StringBuilder displayText = new StringBuilder();
                    foreach (KeyValuePair<object, Node> kvp in _nodeList)
                    {
                        var node = kvp.Value;

                        // if selected then append current item to list to display
                        if (node.IsSelected)
                        {
                            if (displayText.Length > 0) displayText.Append(',');
                            displayText.Append(node.Title);
                        }
                    }

                    this.Text = displayText.ToString();
                }

                // truncate and append ellipses if too long
                if (!string.IsNullOrEmpty(this.MaxTextLength))
                {
                    int maxTextLen;
                    if (Int32.TryParse(this.MaxTextLength, out maxTextLen))
                    {
                        if (this.Text.Length > maxTextLen)
                        {
                            this.Text = this.Text.Substring(0, maxTextLen) + "...";
                        }
                    }
                }

            }

            // set DefaultText if nothing else selected
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = this.DefaultText;
            }
        }
        #endregion
    }


    public class Node : ContentControl
    {
        public Node(string Title, bool IsSelected)
        {
            this.Title = Title;
            this.IsSelected = IsSelected;
        }


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Node), new PropertyMetadata(""));


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(Node), new PropertyMetadata(false));
    }
}
