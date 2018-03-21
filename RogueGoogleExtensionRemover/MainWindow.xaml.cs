using System.Collections.Generic;
using System.Windows;
using ChromeExtensionRemoverLibrary;


/*********************************************************************************************************************************
Copyright and Licensing Message

This code is copyright 2018 Gary Cole Jr. 

This code is licensed by Gary Cole to others under the GPLv.3 https://opensource.org/licenses/GPL-3.0 
If you find the code useful or just feel generous a donation is appreciated.

Donate with this link: paypal.me/GColeJr
Please choose Friends and Family

Alternative Licensing Options

If you prefer to license under the LGPL for a project, https://opensource.org/licenses/LGPL-3.0
Single Developers working on their own project can do so with a donation of $20 or more. 
Small and medium companies can do so with a donation of $50 or more. 
Corporations can do so with a donation of $1000 or more.


If you prefer to license under the MS-RL for a project, https://opensource.org/licenses/MS-RL
Single Developers working on their own project can do so with a donation of $40 or more. 
Small and medium companies can do so with a donation of $100 or more.
Corporations can do so with a donation of $2000 or more.


if you prefer to license under the MS-PL for a project, https://opensource.org/licenses/MS-PL
Single Developers working on their own project can do so with a donation of $1000 or more. 
Small and medium companies can do so with a donation of $2000 or more.
Corporations can do so with a donation of $10000 or more.


If you use the code in more than one project, a separate license is required for each project.


Any modifications to this code must retain this message. 
*************************************************************************************************************************************/
namespace RogueGoogleExtensionRemover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ExtensionFinder extensions = new ExtensionFinder();
        public MainWindow()
        {
            InitializeComponent();
            
            txtdirectory.Text = extensions.extensionsdir;
            AddExtensions();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GoogleExtension file = (GoogleExtension)ExtensionList.SelectedItem;
            if (file.Remove())            
                MessageBox.Show("Extension Deleted");                
            else
                MessageBox.Show($"A problem occurred with the deletion: {file.ErrorMessage}");
            assigndirectoryaddextensions();
        }
        private void AddExtensions()
        {
            try
            {
                ExtensionList.Items.Clear();
                List<GoogleExtension> list = extensions.GetExtensions();
                list.Sort();
                foreach (var e in list)
                    ExtensionList.Items.Add(e);                
            }
            catch
            {
                try
                {
                    assigndirectoryaddextensions();
                }
                catch
                {
                    MessageBox.Show("An error occurred when loading the extensions.");
                }
            }
        }
        private void assigndirectoryaddextensions()
        {
            extensions.extensionsdir = txtdirectory.Text;
            AddExtensions();
        }
        private void btnrefresh_Click(object sender, RoutedEventArgs e)
        {
            assigndirectoryaddextensions();
        }

        private void btnchngdirectory_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browserdialog = new System.Windows.Forms.FolderBrowserDialog();
            browserdialog.SelectedPath = txtdirectory.Text;
            if (browserdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtdirectory.Text = browserdialog.SelectedPath;
                assigndirectoryaddextensions();
            }
        }

        private void ExtensionList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            GoogleExtension file = (GoogleExtension)ExtensionList.SelectedItem;
            txtselectedpath.Text = file.GetExtensionPath().Replace(txtdirectory.Text,"").Replace(@"\","");
        }

        private void btnDonate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://paypal.me/GColeJr");
        }
    }
}
