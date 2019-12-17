using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace RPAStudio.ViewModel
{
    public class SettingItem : ViewModelBase
    {
        private PackageManagerViewModel m_vm;
        public enum enSettingItemType
        {
            Null = 0,
            Settings,//包源设置
            ProjectDependencies,//项目依赖包
            AllPackages,//所有包
            PackageItem,//单独的包列表
        }

        public string PackageItemSource { get; set; } //当类型为PackageItem时记录Source值


        public SettingItem(PackageManagerViewModel vm, enSettingItemType itemType, string name, string toolTip, string icon = null)
        {
            this.m_vm = vm;
            this.ItemType = itemType;
            this.Name = name;
            this.ToolTip = toolTip;
            this.Icon = icon;
        }

        public enSettingItemType ItemType = enSettingItemType.Null;//标记当前节点的类型

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _nameProperty = "";

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _nameProperty;
            }

            set
            {
                if (_nameProperty == value)
                {
                    return;
                }

                _nameProperty = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="ToolTip" /> property's name.
        /// </summary>
        public const string ToolTipPropertyName = "ToolTip";

        private string _toolTipProperty = "";

        /// <summary>
        /// Sets and gets the ToolTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ToolTip
        {
            get
            {
                return _toolTipProperty;
            }

            set
            {
                if (_toolTipProperty == value)
                {
                    return;
                }

                _toolTipProperty = value;
                RaisePropertyChanged(ToolTipPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsHasIcon" /> property's name.
        /// </summary>
        public const string IsHasIconPropertyName = "IsHasIcon";

        private bool _isHasIconProperty = false;

        /// <summary>
        /// Sets and gets the IsHasIcon property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsHasIcon
        {
            get
            {
                return _isHasIconProperty;
            }

            set
            {
                if (_isHasIconProperty == value)
                {
                    return;
                }

                _isHasIconProperty = value;
                RaisePropertyChanged(IsHasIconPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Icon" /> property's name.
        /// </summary>
        public const string IconPropertyName = "Icon";

        private string _iconProperty = "";

        /// <summary>
        /// Sets and gets the Icon property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Icon
        {
            get
            {
                return _iconProperty;
            }

            set
            {
                if (_iconProperty == value)
                {
                    return;
                }

                if(string.IsNullOrEmpty(value))
                {
                    value = "pack://application:,,,/Resources/Image/Dock/null.png";//避免编译时的错误提示信息
                    IsHasIcon = false;
                }
                else
                {
                    IsHasIcon = true;
                }

                _iconProperty = value;
                RaisePropertyChanged(IconPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsSelected" /> property's name.
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelectedProperty = false;

        /// <summary>
        /// Sets and gets the IsSelected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelectedProperty;
            }

            set
            {
                if (_isSelectedProperty == value)
                {
                    return;
                }

                _isSelectedProperty = value;
                RaisePropertyChanged(IsSelectedPropertyName);

                if(value)
                {
                    m_vm.CurrentSelectedSettingItem = this;

                    if (ItemType == enSettingItemType.Settings)
                    {
                        //包源设置
                        //重置包源之前的选择项
                        m_vm.DefaultPackageSourceItemsUnselectAll();
                        m_vm.UserDefinePackageSourceItemsUnselectAll();
                        m_vm.IsSettingsShow = true;
                    }
                    else
                    {
                        m_vm.IsSettingsShow = false;

                        //包列表展示
                        m_vm.DoSearch();
                    }
                }
            }
        }

        
    }
}