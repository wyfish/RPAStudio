using GalaSoft.MvvmLight;
using NuGet.Packaging;
using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace RPAStudio.ViewModel
{
    public class PackageSourceSearchResultItem : ViewModelBase
    {
        private PackageManagerViewModel m_vm;

        public bool RequireLicenseAcceptance { get; set; }

        public PackageIdentity Identity { get; set; }

        public PackageSourceSearchResultItem(PackageManagerViewModel vm,string id, string authors, string description, string icon_url = "")
        {
            this.m_vm = vm;

            this.Id = id;
            this.Authors = authors;
            this.Description = description;
            this.IconUrl = icon_url;
        }


        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        private string _idProperty = "";

        /// <summary>
        /// Sets and gets the Id property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Id
        {
            get
            {
                return _idProperty;
            }

            set
            {
                if (_idProperty == value)
                {
                    return;
                }

                _idProperty = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string _titleProperty = "";

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Title
        {
            get
            {
                if(string.IsNullOrEmpty(_titleProperty))
                {
                    return this.Id;//无标题时返回packageid值
                }
                return _titleProperty;
            }

            set
            {
                if (_titleProperty == value)
                {
                    return;
                }

                _titleProperty = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Authors" /> property's name.
        /// </summary>
        public const string AuthorsPropertyName = "Authors";

        private string _authorsProperty = "";

        /// <summary>
        /// Sets and gets the Authors property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Authors
        {
            get
            {
                return _authorsProperty;
            }

            set
            {
                if (_authorsProperty == value)
                {
                    return;
                }

                _authorsProperty = value;
                RaisePropertyChanged(AuthorsPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _descriptionProperty = "";

        /// <summary>
        /// Sets and gets the Description property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Description
        {
            get
            {
                return _descriptionProperty;
            }

            set
            {
                if (_descriptionProperty == value)
                {
                    return;
                }

                _descriptionProperty = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IconUrl" /> property's name.
        /// TODO WJF IconUrl在nuspec后期会被废除，用Icon替代
        /// </summary>
        public const string IconUrlPropertyName = "IconUrl";

        private string _iconUrlProperty = "";

        /// <summary>
        /// Sets and gets the IconUrl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string IconUrl
        {
            get
            {
                return _iconUrlProperty;
            }

            set
            {
                if (_iconUrlProperty == value)
                {
                    return;
                }

                _iconUrlProperty = value;
                RaisePropertyChanged(IconUrlPropertyName);

            }
        }


        /// <summary>
        /// The <see cref="IsInstalled" /> property's name.
        /// </summary>
        public const string IsInstalledPropertyName = "IsInstalled";

        private bool _isInstalledProperty = false;

        /// <summary>
        /// Sets and gets the IsInstalled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                return _isInstalledProperty;
            }

            set
            {
                if (_isInstalledProperty == value)
                {
                    return;
                }

                _isInstalledProperty = value;
                RaisePropertyChanged(IsInstalledPropertyName);
            }
        }




        /// <summary>
        /// The <see cref="InstalledVersion" /> property's name.
        /// </summary>
        public const string InstalledVersionPropertyName = "InstalledVersion";

        private string _installedVersionProperty = "";

        /// <summary>
        /// Sets and gets the InstalledVersion property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string InstalledVersion
        {
            get
            {
                return _installedVersionProperty;
            }

            set
            {
                if (_installedVersionProperty == value)
                {
                    return;
                }

                _installedVersionProperty = value;
                RaisePropertyChanged(InstalledVersionPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="LatestVersion" /> property's name.
        /// </summary>
        public const string LatestVersionPropertyName = "LatestVersion";

        private string _latestVersionProperty = "";

        /// <summary>
        /// Sets and gets the LatestVersion property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LatestVersion
        {
            get
            {
                return _latestVersionProperty;
            }

            set
            {
                if (_latestVersionProperty == value)
                {
                    return;
                }

                _latestVersionProperty = value;
                RaisePropertyChanged(LatestVersionPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="LicenseUrl" /> property's name.
        /// </summary>
        public const string LicenseUrlPropertyName = "LicenseUrl";

        private string _licenseUrlProperty = "";

        /// <summary>
        /// Sets and gets the LicenseUrl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LicenseUrl
        {
            get
            {
                return _licenseUrlProperty;
            }

            set
            {
                if (_licenseUrlProperty == value)
                {
                    return;
                }

                _licenseUrlProperty = value;
                RaisePropertyChanged(LicenseUrlPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ProjectUrl" /> property's name.
        /// </summary>
        public const string ProjectUrlPropertyName = "ProjectUrl";

        private string _projectUrlProperty = "";

        /// <summary>
        /// Sets and gets the ProjectUrl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ProjectUrl
        {
            get
            {
                return _projectUrlProperty;
            }

            set
            {
                if (_projectUrlProperty == value)
                {
                    return;
                }

                _projectUrlProperty = value;
                RaisePropertyChanged(ProjectUrlPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Tags" /> property's name.
        /// </summary>
        public const string TagsPropertyName = "Tags";

        private string _tagsProperty = "";

        /// <summary>
        /// Sets and gets the Tags property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Tags
        {
            get
            {
                return _tagsProperty;
            }

            set
            {
                if (_tagsProperty == value)
                {
                    return;
                }

                _tagsProperty = value;
                RaisePropertyChanged(TagsPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Dependencies" /> property's name.
        /// </summary>
        public const string DependenciesPropertyName = "Dependencies";

        private List<PackageDependencyGroup> _dependenciesProperty = new List<PackageDependencyGroup>();

        /// <summary>
        /// Sets and gets the Dependencies property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<PackageDependencyGroup> Dependencies
        {
            get
            {
                return _dependenciesProperty;
            }

            set
            {
                if (_dependenciesProperty == value)
                {
                    return;
                }

                _dependenciesProperty = value;
                RaisePropertyChanged(DependenciesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="PublishTime" /> property's name.
        /// </summary>
        public const string PublishTimePropertyName = "PublishTime";

        private string _publishTimeProperty = "";

        /// <summary>
        /// Sets and gets the PublishTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PublishTime
        {
            get
            {
                return _publishTimeProperty;
            }

            set
            {
                if (_publishTimeProperty == value)
                {
                    return;
                }

                _publishTimeProperty = value;
                RaisePropertyChanged(PublishTimePropertyName);
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
                    m_vm.CurrentSelectedPackageSourceSearchResultItem = this;
                    m_vm.ShowPackageSourceSearchResultItemDetail(this);
                }
                
            }
        }

        
    }
}