using System;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using log4net;
using RPAStudio.ViewModel;
using RPAStudio.Librarys;
using RPAStudio.Localization;

namespace RPAStudio.DragDropHandler
{
    public class ProjectDropHandler : DefaultDropHandler
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
            {
                var data = typeof(DropInfo).GetProperty("Data");
                data.SetValue(dropInfo, dropInfo.DragInfo.Data);

                var sourceItem = dropInfo.Data as ProjectTreeItem;
                var targetItem = dropInfo.TargetItem as ProjectTreeItem;

                if (sourceItem != null && targetItem != null)
                {
                    bool bCanDo = System.IO.Directory.Exists(targetItem.Path)//目标路径必须是个目录
                                    && !(sourceItem.Name == ".screenshots" && System.IO.Directory.Exists(sourceItem.Path))//不能为截图目录
                                    && !(sourceItem.Name == "project.json" && System.IO.File.Exists(sourceItem.Path))//不能为项目配置文件
                                    && !((targetItem.Path + @"\" + sourceItem.Name).ToLower() == sourceItem.Path.ToLower())//源路径已经在目标路径下没必要拖拽
                                    ;

                    if (bCanDo)
                    {
                        base.DragOver(dropInfo);

                        //禁用插入点显示
                        if (dropInfo.DropTargetAdorner == DropTargetAdorners.Insert)
                        {
                            dropInfo.Effects = DragDropEffects.None;
                            dropInfo.DropTargetAdorner = null;
                        }
                    }

                }
                
            }
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
            {
                var sourceItem = dropInfo.Data as ProjectTreeItem;
                var targetItem = dropInfo.TargetItem as ProjectTreeItem;

                Logger.Debug(string.Format("sourceItem.Path={0}", sourceItem?.Path), logger);
                Logger.Debug(string.Format("targetItem.Path={0}", targetItem?.Path), logger);

                if(sourceItem != null && targetItem != null)
                {
                    if(System.IO.File.Exists(sourceItem.Path))
                    {
                        if (MoveFileToDir(sourceItem, targetItem))
                        {
                            base.Drop(dropInfo);
                        }
                    }
                    else
                    {
                        //原路径是目录，则除了目录，目录下的所有子目录及文件也要复制过去
                        if (MoveDirToDir(sourceItem, targetItem))
                        {
                            base.Drop(dropInfo);
                        }
                    }
                    
                }

                
            }
        }

        private bool MoveDirToDir(ProjectTreeItem sourceItem, ProjectTreeItem targetItem)
        {
            var srcPath = sourceItem.Path;
            var dstPath = targetItem.Path;

            var dstPathCombine = System.IO.Path.Combine(dstPath, sourceItem.Name);

            if (System.IO.Directory.Exists(dstPathCombine))
            {
                // 目标目录有重名目录，无法移动目录
                MessageBox.Show(App.Current.MainWindow, ResxIF.GetString("msgDirectoryAlredyExists"), ResxIF.GetString("msgWarning"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                System.IO.Directory.Move(srcPath, dstPathCombine);

                //遍历目录所有文件
                foreach (var file in System.IO.Directory.GetFiles(dstPathCombine, "*.*"))
                {
                    var relativeFile = Common.MakeRelativePath(dstPathCombine, file);
                    var srcFile = System.IO.Path.Combine(srcPath, relativeFile);

                    foreach (var doc in ViewModelLocator.Instance.Dock.Documents)
                    {
                        if (doc.XamlPath.EqualsIgnoreCase(srcFile))
                        {
                            doc.XamlPath = file;
                            break;
                        }
                    }
                }
                //刷新工程树视图

                ViewModelLocator.Instance.Project.RefreshCommand.Execute(null);

                return true;
            }
            return false;
        }

        private bool MoveFileToDir(ProjectTreeItem sourceItem, ProjectTreeItem targetItem)
        {
            var srcFile = sourceItem.Path;
            var dstPath = targetItem.Path;

            //拷贝源文件到目录路径中去，若源文件所对应的旧有路径已经在设计器中打开，则需要更新设计器中对应的路径
            var dstFile = System.IO.Path.Combine(dstPath, sourceItem.Name);
            if (System.IO.File.Exists(dstFile))
            {
                // 目标目录有重名文件，无法移动文件
                MessageBox.Show(App.Current.MainWindow, ResxIF.GetString("msgFileAlredyExists"), ResxIF.GetString("msgWarning"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                System.IO.File.Move(srcFile, dstFile);
                sourceItem.Path = dstFile;//更新VM
                foreach (var doc in ViewModelLocator.Instance.Dock.Documents)
                {
                    if (doc.XamlPath.EqualsIgnoreCase(srcFile))
                    {
                        doc.XamlPath = dstFile;
                        break;
                    }
                }

                if (sourceItem.IsMain)
                {
                    //如果是主文件，则移动过去后还是主文件
                    sourceItem.SetAsMainCommand.Execute(null);
                }

                return true;
            }

            return false;
        }


    }
}