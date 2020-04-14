using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XLSX_to_ARMTemplate_IAAS
{
    /// <summary>
    /// Interaction logic for CopyCommandWindow.xaml
    /// </summary>
    public partial class CopyCommandWindow : Window
    {
        string CommandAzureRmResourceGroup;
        string CommandAzureRmDeployment;
        string CommandAzResourceGroup;
        string CommandAzDeployment;
        string CommandCliResourceGroup;
        string CommandCliDeployment;
        public CopyCommandWindow(string _AzureRmResourceGroup, string _AzureRmDeployment,
            string _AzResourceGroup, string _AzDeployment,
            string _CliResourceGroup, string _CliDeployment)
        {
            InitializeComponent();
            TextBlock_AzureRmResourceGroup.Text = CommandAzureRmResourceGroup = _AzureRmResourceGroup;
            TextBlock_AzureRmDeployment.Text = CommandAzureRmDeployment = _AzureRmDeployment;

            TextBlock_AzResourceGroup.Text = CommandAzResourceGroup = _AzResourceGroup;
            TextBlock_AzDeployment.Text = CommandAzDeployment = _AzDeployment;

            TextBlock_CliResourceGroup.Text = CommandCliResourceGroup = _CliResourceGroup;
            TextBlock_CliDeployment.Text = CommandCliDeployment = _CliDeployment;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_AzureRmResourceGroup_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(CommandAzureRmResourceGroup);
        }

        private void Button_AzureRmDeployment_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(CommandAzureRmDeployment);
        }

        private void Button_AzResourceGroup_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(CommandAzResourceGroup);
        }

        private void Button_AzDeployment_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(CommandAzDeployment);
        }

        private void Button_CliResourceGroup_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(CommandCliResourceGroup);
        }

        private void Button_CliDeployment_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(CommandCliDeployment);
        }

        private void CopyToClipboard(string _content)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(_content, true);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                MessageBox.Show(msg);
            }

        }
    }
}
