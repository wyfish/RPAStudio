using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Linq;

namespace RPA.Core.Activities.DataTableActivity.Dialog
{
    public partial class IntCollectionEditor

    {

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(List<int>), typeof(IntCollectionEditor));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(IntCollectionEditor));

        private int? _newValue;

        //internal IntCollectionEditor CollectionEditor;

        internal Path Arrow = null;

        //internal TextBox ComboboxInputBox;

        //internal ComboBox PropertiesComboBox;

        //internal ComboBoxItem NewValueElement;

        //private bool _contentLoaded;

        public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<ElementInfo> CollectionElements
    {
        get;
        set;
    }

    public List<int> ItemsSource
    {
        get
        {
            return base.GetValue(ItemsSourceProperty) as List<int>;
        }
        set
        {
            base.SetValue(ItemsSourceProperty, value);
        }
    }

    public int? NewValue
    {
        get
        {
            return this._newValue;
        }
        set
        {
            if (this._newValue != value)
            {
                this._newValue = value;
                this.OnPropertyChanged("NewValue");
            }
        }
    }

    public string SelectedItem
    {
        get
        {
            return base.GetValue(IntCollectionEditor.SelectedItemProperty) as string;
        }
        set
        {
            base.SetValue(IntCollectionEditor.SelectedItemProperty, value);
        }
    }

    public IntCollectionEditor()
    {
        CollectionElements = new ObservableCollection<ElementInfo>();
        this.InitializeComponent();
    }

    protected void OnPropertyChanged(string name)
    {
        PropertyChangedEventHandler expr_06 = this.PropertyChanged;
        if (expr_06 == null)
        {
            return;
        }
        expr_06(this, new PropertyChangedEventArgs(name));
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.NewValue.HasValue && this.NewValue.HasValue)
        {
            this.CollectionElements.Add(new ElementInfo(this.NewValue.Value));
            this.ItemsSource.Add(this.NewValue.Value);
            this.NewValue = null;
            this.NewValueElement.Focus();
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.PropertiesComboBox.IsDropDownOpen = true;
        this.CollectionElements.Clear();
        if (this.ItemsSource != null)
        {
            foreach (int current in this.ItemsSource)
            {
                this.CollectionElements.Add(new ElementInfo(current));
            }
        }
    }

    private void ItemsPanel_LostFocus(object sender, RoutedEventArgs e)
    {
            this.ItemsSource = new List<int>(from element in this.CollectionElements select element.Value);
    }

    private void PropertiesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        this.PropertiesComboBox.IsDropDownOpen = true;
        e.Handled = true;
    }

    private void RemoveElementButton_Click(object sender, RoutedEventArgs e)
    {
            ElementInfo dataContext = (sender as Button).DataContext as ElementInfo;
            if (dataContext != null)
            {
                this.CollectionElements.Remove(dataContext);
                this.ItemsSource = new List<int>(from elem in this.CollectionElements select elem.Value);
            }
    }
    }
}
