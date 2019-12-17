using GalaSoft.MvvmLight;
using Plugins.Shared.Library.Nuget;
using NuGet.Configuration;
using System.Linq;

namespace RPAStudio.ViewModel
{
    public class CommonPackageSourceItem : ViewModelBase
    {
        private PackageManagerViewModel m_vm;
        //是否是默认源
        public bool IsDefault { get; set; }

        public PackageSource PackageSourceItem { get; set; }

        public CommonPackageSourceItem(PackageManagerViewModel vm)
        {
            this.m_vm = vm;
        }

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
        /// The <see cref="Source" /> property's name.
        /// </summary>
        public const string SourcePropertyName = "Source";

        private string _sourceProperty = "";

        /// <summary>
        /// Sets and gets the Source property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Source
        {
            get
            {
                return _sourceProperty;
            }

            set
            {
                if (_sourceProperty == value)
                {
                    return;
                }

                _sourceProperty = value;
                RaisePropertyChanged(SourcePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsChecked" /> property's name.
        /// </summary>
        public const string IsCheckedPropertyName = "IsChecked";

        private bool _isCheckedProperty = false;

        /// <summary>
        /// Sets and gets the IsChecked property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isCheckedProperty;
            }

            set
            {
                if (_isCheckedProperty == value)
                {
                    return;
                }

                _isCheckedProperty = value;
                RaisePropertyChanged(IsCheckedPropertyName);

                IPackageSourceProvider provider  = null;

                if(IsDefault)
                {
                    provider = NuGetPackageController.Instance.DefaultSourceRepositoryProvider.PackageSourceProvider;
                }
                else
                {
                    provider = NuGetPackageController.Instance.UserDefineSourceRepositoryProvider.PackageSourceProvider;
                }

                
                if(value)
                {
                    //启用包源（老版本库不支持调用EnablePackageSource）
                    if (!provider.IsPackageSourceEnabled(PackageSourceItem))
                    {
                        var sources = provider.LoadPackageSources();

                        foreach (var item in sources)
                        {
                            if(item.Name == PackageSourceItem.Name)
                            {
                                item.IsEnabled = true;
                                break;
                            }
                        }

                        provider.SavePackageSources(sources);
                    }
                }
                else
                {
                    //禁用包源
                    if (provider.IsPackageSourceEnabled(PackageSourceItem))
                    {
                        provider.DisablePackageSource(PackageSourceItem);
                    }                   
                }

                m_vm.InitSettingItems(SettingItem.enSettingItemType.Settings);
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

                //包源点击时处理相应界面变化
                if (value)
                {
                    m_vm.IsAddPackageSourceEnabled = true;

                    if (IsDefault)
                    {
                        //默认源
                        m_vm.UserDefinePackageSourceItemsUnselectAll();

                        m_vm.IsRemovePackageSourceEnabled = false;
                        m_vm.IsMoveUpPackageSourceEnabled = false;
                        m_vm.IsMoveDownPackageSourceEnabled = false;
                    }
                    else
                    {
                        //自定义源
                        m_vm.DefaultPackageSourceItemsUnselectAll();

                        IPackageSourceProvider provider = NuGetPackageController.Instance.UserDefineSourceRepositoryProvider.PackageSourceProvider;
                        var sources = provider.LoadPackageSources();
                        var first_source = sources.FirstOrDefault();
                        var last_source = sources.LastOrDefault();

                        m_vm.IsRemovePackageSourceEnabled = true;

                        if (PackageSourceItem.Name == first_source.Name)
                        {
                            //第一个条目已经在最上面了，所以不能上移
                            m_vm.IsMoveUpPackageSourceEnabled = false;
                        }
                        else
                        {
                            m_vm.IsMoveUpPackageSourceEnabled = true;
                        }
  
                        if(PackageSourceItem.Name == last_source.Name)
                        {
                            //最后一个条目已经在最下面了，所以不能下移
                            m_vm.IsMoveDownPackageSourceEnabled = false;
                        }else
                        {
                            m_vm.IsMoveDownPackageSourceEnabled = true;
                        }
                        
                    }
                }

                if (value)
                {
                    m_vm.IsShowUpdatePackageSourceBtn = true;
                    m_vm.CurrentSelectPackageSourceName = this.Name;
                    m_vm.PackageSourceName = this.Name;
                    m_vm.PackageSourceUri = this.Source;

                    if (IsDefault)
                    {
                        //默认源
                        m_vm.IsOperatePackageSourceEnabled = false;
                    }
                    else
                    {
                        //自定义源
                        m_vm.IsOperatePackageSourceEnabled = true;
                    }
                }
                

            }
        }








    }
}