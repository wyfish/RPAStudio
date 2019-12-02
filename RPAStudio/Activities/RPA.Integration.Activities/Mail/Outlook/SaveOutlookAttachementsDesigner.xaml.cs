using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPA.Integration.Activities.Mail
{
    // SaveOutlookAttachementsDesigner.xaml 的交互逻辑
    public partial class SaveOutlookAttachementsDesigner
    {
        private FolderBrowserDialog folderDialog = new FolderBrowserDialog();

        public SaveOutlookAttachementsDesigner()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            folderDialog.SelectedPath = currentDirectory;
            if (folderDialog.ShowDialog() == DialogResult.OK && folderDialog.SelectedPath != null)
            {
                string text = folderDialog.SelectedPath;
                if (text.ToLowerInvariant() == currentDirectory.ToLowerInvariant())
                {
                    text = string.Empty;
                }
                else
                {
                    try
                    {
                        Uri uri = new Uri(System.IO.Path.Combine(currentDirectory, System.IO.Path.PathSeparator.ToString()));
                        Uri uri2 = new Uri(folderDialog.SelectedPath);
                        if (uri.IsBaseOf(uri2))
                        {
                            text = Uri.UnescapeDataString(uri.MakeRelativeUri(uri2).OriginalString).Replace('/', System.IO.Path.DirectorySeparatorChar);
                        }
                    }
                    catch
                    {
                    }
                }
                base.ModelItem.Properties["FolderPath"].SetValue(new InArgument<string>(text));
            }
        }

    }
}
