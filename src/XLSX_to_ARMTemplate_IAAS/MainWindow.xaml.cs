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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace XLSX_to_ARMTemplate_IAAS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Stage variables
        int openingTick;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        // Column index when reading from xlsx file.
        readonly int COLUMNINDEX = 3;

        // The temporary NSG_Rules instance when reading.
        List<ARM_Template.Resource_Classes.SecurityRule> nsgRules;

        public MainWindow()
        {
            InitializeComponent();

            //  DispatcherTimer setup            
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            // Set opacity = 0
            Opacity = 0.0f;
            openingTick = 0;

            // Start the DispatcherTimer
            dispatcherTimer.Start();

        }

        //  System.Windows.Threading.DispatcherTimer.Tick handler
        //
        //  Updates the current seconds display and calls
        //  InvalidateRequerySuggested on the CommandManager to force 
        //  the Command to raise the CanExecuteChanged event.
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Starting animation
            if (openingTick < 100)
            {
                Opacity = openingTick / 100.0f;
                openingTick += 10;
            }
            else
            {
                Opacity = 1.0f;
                dispatcherTimer.Stop();
            }
        }

        // Sync TextBox_VMPassword and PasswordBox_VMPassword
        private void TextBox_VMPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBox_VMPassword != null)
            {
                PasswordBox_VMPassword.Password = TextBox_VMPassword.Text;
            }
        }

        // Sync TextBox_VMPassword and PasswordBox_VMPassword
        private void PasswordBox_VMPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox_VMPassword != null)
            {
                TextBox_VMPassword.Text = PasswordBox_VMPassword.Password;
            }
        }

        // Switch Password to be visible
        private void CheckBox_VMPasswordVisible_Checked(object sender, RoutedEventArgs e)
        {
            TextBox_VMPassword.Visibility = Visibility.Visible;
        }

        // Switch Password to be hidden
        private void CheckBox_VMPasswordVisible_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBox_VMPassword.Visibility = Visibility.Hidden;
        }

        // Read from an XLSX file and display in the form
        // NSG Rules sheet will not be displayed but will 
        // be kept in a variable only if NSG Name is not empty

        // This Method should better to be moved to under Utils
        private void Read_From_XLSX(string filePathFrom)
        {
            // Open the XLSX File
            XSSFWorkbook xssfwb;

            using (FileStream file = new FileStream(filePathFrom, FileMode.Open, FileAccess.Read))
            {
                xssfwb = new XSSFWorkbook(file);
            }

            // Read the Excel file

            try
            {
                // Get the first sheet
                ISheet sheetIAAS = xssfwb.GetSheetAt(0);

                // Location
                TextBox_Location.Text = sheetIAAS.GetRow(3).GetCell(COLUMNINDEX).ToString().Trim();

                // Resource Group
                var RGNameRead = sheetIAAS.GetRow(4).GetCell(COLUMNINDEX).ToString().Trim();
                if(RGNameRead == null || RGNameRead == "")
                {
                    MessageBox.Show("File Corrupt or Incorrect File Format", "Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                TextBox_ResourceGroup.Text = RGNameRead;

                // VM Username
                TextBox_VMUsername.Text = sheetIAAS.GetRow(5).GetCell(COLUMNINDEX).ToString().Trim();

                // Password
                PasswordBox_VMPassword.Password = sheetIAAS.GetRow(6).GetCell(COLUMNINDEX).ToString().Trim();
                TextBox_VMPassword.Text = sheetIAAS.GetRow(6).GetCell(COLUMNINDEX).ToString().Trim();

                // VNET Resource Group
                TextBox_VNETRG.Text = sheetIAAS.GetRow(8).GetCell(COLUMNINDEX).ToString().Trim();

                // VNET Name
                TextBox_VNetName.Text = sheetIAAS.GetRow(9).GetCell(COLUMNINDEX).ToString().Trim();

                // VNET Address Space
                TextBox_VNetAddrSpace.Text = sheetIAAS.GetRow(10).GetCell(COLUMNINDEX).ToString().Trim();

                // Subnet Name
                TextBox_SubnetName.Text = sheetIAAS.GetRow(11).GetCell(COLUMNINDEX).ToString().Trim();

                // Subnet Address Space
                TextBox_SubnetAddrSpace.Text = sheetIAAS.GetRow(12).GetCell(COLUMNINDEX).ToString().Trim();

                // VM Name Prefix
                TextBox_VMNamePrefix.Text = sheetIAAS.GetRow(14).GetCell(COLUMNINDEX).ToString().Trim();

                // VM Size
                TextBox_VMSize.Text = sheetIAAS.GetRow(15).GetCell(COLUMNINDEX).ToString().Trim();

                // VM Instance Count
                if (int.TryParse(sheetIAAS.GetRow(16).GetCell(COLUMNINDEX).ToString().Trim(), out int Parse_VMInstanceCount) == false)
                {
                    MessageBox.Show("Format incorrect when parsing", "VM Instance Count", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    IntegerUpDownBox_VMInstanceCount.Value = Parse_VMInstanceCount;
                }

                // VM Disk Storage Type
                TextBox_VMStorageType.Text = sheetIAAS.GetRow(17).GetCell(COLUMNINDEX).ToString().Trim();

                // VM Data Disk Size
                if(int.TryParse(sheetIAAS.GetRow(18).GetCell(COLUMNINDEX).ToString().Trim(), out int Parse_VMDataDiskSize) == false)
                {
                    MessageBox.Show("Format incorrect when parsing", "VM Data Disk Size", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    IntegerUpDownBox_VMDataDiskSize.Value = Parse_VMDataDiskSize;
                }

                // VM Data Disk Count
                if (int.TryParse(sheetIAAS.GetRow(19).GetCell(COLUMNINDEX).ToString().Trim(), out int Parse_VMDataDiskCount) == false)
                {
                    MessageBox.Show("Format incorrect when parsing", "VM Data Disk Count", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    IntegerUpDownBox_VMDataDiskCount.Value = Parse_VMDataDiskCount;
                }

                // NSG Name
                var NSGNameRead = sheetIAAS.GetRow(21).GetCell(COLUMNINDEX).ToString().Trim();
                TextBox_NSGName.Text = NSGNameRead;

                // NSG Attach to NIC
                CheckBox_NSGAttachToNIC.IsChecked = (sheetIAAS.GetRow(22).GetCell(COLUMNINDEX).ToString().Trim() == "VM NIC");

                // VM LAN IP
                TextBox_VMLANIP.Text = sheetIAAS.GetRow(23).GetCell(COLUMNINDEX).ToString().Trim();

                // Enable PIP
                CheckBox_PIP.IsChecked = (sheetIAAS.GetRow(24).GetCell(COLUMNINDEX).ToString().Trim().ToUpper() == "YES");

                // Image Publisher
                var tempPublisher = TextBox_ImagePublisher.Text = sheetIAAS.GetRow(26).GetCell(COLUMNINDEX).ToString().Trim();

                // Image Offer
                var tempOffer = TextBox_ImageOffer.Text = sheetIAAS.GetRow(27).GetCell(COLUMNINDEX).ToString().Trim();

                // Image SKU
                TextBox_ImageSKU.Text = sheetIAAS.GetRow(28).GetCell(COLUMNINDEX).ToString().Trim();

                // AvSet Name
                TextBox_AvSetName.Text = sheetIAAS.GetRow(29).GetCell(COLUMNINDEX).ToString().Trim();

                // Diag Account Name
                TextBox_DiagStrAccName.Text = sheetIAAS.GetRow(30).GetCell(COLUMNINDEX).ToString().Trim().ToLower();

                // Remind the CoreOS and CentOS problem
                if(tempPublisher.ToLower().Contains("coreos") && tempOffer.ToLower().Contains("centos"))
                {
                    MessageBox.Show("CentOS的Publisher是OpenLogic不是CoreOS，如果想建CentOS请选OpenLogic",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //
                //
                //
                //
                // If has NSG Name, then read the second sheet
                if ((NSGNameRead != null) && (NSGNameRead != ""))
                {
                    try
                    {
                        // Get the NSG rules sheet
                        ISheet sheetNSGRules = xssfwb.GetSheetAt(1);

                        // READ NSG Rules
                        nsgRules = Utils.ReadNSGRules(sheetNSGRules);

                        // Read NSG rules sheet finishes
                        if (nsgRules != null && nsgRules != new List<ARM_Template.Resource_Classes.SecurityRule>() )
                        {
                            Label_NSGNewWarning.Visibility = Visibility.Visible;
                            Label_NSGName.Content = "NSG Name (rules loaded)";
                            Label_NSGName.FontWeight = FontWeights.ExtraBold;
                            Label_NSGName.Margin = new Thickness(93, 400, 0, 0);
                        }
                    }
                    catch(Exception sheetNsgEx)
                    {
                        MessageBox.Show(sheetNsgEx.Message, "Read NSG Rules sheet exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

            }
            
            //
            catch(Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                //"Read Excel file exception"
            }

            MessageBox.Show(filePathFrom, "Read Excel file finished", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // click the Read button to read from an xlsx file
        private void Button_Read_Click(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog readXlsxOFDlg = new Microsoft.Win32.OpenFileDialog
            {

                // Set filter for file extension and default file extension  
                ReadOnlyChecked = true,
                ValidateNames = true,
                DefaultExt = ".xlsx",
                Filter = "Excel file|*.xlsx"
            };

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = readXlsxOFDlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    string xlsxFilePath = readXlsxOFDlg.FileName;
                    // Call the Read File Function
                    Read_From_XLSX(xlsxFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            string armTemplateJson;

            // Check fields
            if (
                Utils.ValidateInputFields(TextBox_Location.Text.Trim(), TextBox_ResourceGroup.Text.Trim(),
                TextBox_VMUsername.Text.Trim(), PasswordBox_VMPassword.Password,
                TextBox_VNETRG.Text.Trim(), TextBox_VNetName.Text.Trim(), TextBox_VNetAddrSpace.Text.Trim(), TextBox_SubnetName.Text.Trim(), TextBox_SubnetAddrSpace.Text.Trim(),
                TextBox_NSGName.Text.Trim(), (bool)CheckBox_NSGAttachToNIC.IsChecked,
                TextBox_VMNamePrefix.Text.Trim(), TextBox_VMSize.Text.Trim(), (int)IntegerUpDownBox_VMInstanceCount.Value,
                TextBox_VMStorageType.Text.Trim(), (int)IntegerUpDownBox_VMDataDiskSize.Value, (int)IntegerUpDownBox_VMDataDiskCount.Value,
                TextBox_ImagePublisher.Text.Trim(), TextBox_ImageOffer.Text.Trim(), TextBox_ImageSKU.Text.Trim(),
                TextBox_VMLANIP.Text.Trim(), (bool)CheckBox_PIP.IsChecked, TextBox_AvSetName.Text.Trim(), TextBox_DiagStrAccName.Text.Trim())
                )
            {

                // Read to template class
                var armTemplateInstance = Utils.GenerateTemplateClass(TextBox_Location.Text.Trim(), TextBox_ResourceGroup.Text.Trim(),
                TextBox_VMUsername.Text.Trim(), PasswordBox_VMPassword.Password,
                TextBox_VNETRG.Text.Trim(), TextBox_VNetName.Text.Trim(), TextBox_VNetAddrSpace.Text.Trim(), TextBox_SubnetName.Text.Trim(), TextBox_SubnetAddrSpace.Text.Trim(),
                TextBox_NSGName.Text.Trim(), (bool)CheckBox_NSGAttachToNIC.IsChecked,
                TextBox_VMNamePrefix.Text.Trim(), TextBox_VMSize.Text.Trim(), (int)IntegerUpDownBox_VMInstanceCount.Value,
                TextBox_VMStorageType.Text.Trim(), (int)IntegerUpDownBox_VMDataDiskSize.Value, (int)IntegerUpDownBox_VMDataDiskCount.Value,
                TextBox_ImagePublisher.Text.Trim(), TextBox_ImageOffer.Text.Trim(), TextBox_ImageSKU.Text.Trim(),
                TextBox_VMLANIP.Text.Trim(), (bool)CheckBox_PIP.IsChecked, TextBox_AvSetName.Text.Trim(), TextBox_DiagStrAccName.Text.Trim(),
                nsgRules,
                (bool)CheckBox_DiagStrAccNew.IsChecked);

                // Generate JSON string
                armTemplateJson = Utils.GenerateJson(armTemplateInstance);
            }
            else
            {
                return;
            }

            var now = DateTime.Now;
            // Create SaveFileDialog
            Microsoft.Win32.SaveFileDialog genARMSFDlg = new Microsoft.Win32.SaveFileDialog
            {

                // Set filter for file extension and default file extension
                DefaultExt = ".json",
                FileName = TextBox_ResourceGroup.Text + "_template_" + now.Year + now.Month + now.Day,
                Filter = "ARM template file|*.json",
                OverwritePrompt = true,
                Title = "Save to ARM template"
            };

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = genARMSFDlg.ShowDialog();

            if (result == true)
            {
                string ARMTempFilePath = genARMSFDlg.FileName;

                try
                {
                    // Write to file
                    System.IO.File.WriteAllText(ARMTempFilePath, armTemplateJson);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    MessageBox.Show(msg);
                }


                MessageBox.Show(ARMTempFilePath, "file write successful.", MessageBoxButton.OK, MessageBoxImage.Information);

                var AzureRmResourceGroupCommand = "New-AzureRmResourceGroup -Name \'" + TextBox_ResourceGroup.Text 
                    + "\' -Location " + TextBox_Location.Text;
                var AzureRmDeploymentCommand = "New-AzureRmResourceGroupDeployment -ResourceGroupName " + TextBox_ResourceGroup.Text
                    + " -TemplateFile \'" + ARMTempFilePath + "\' -Verbose";

                var AzResourceGroupCommand = "New-AzResourceGroup -Name \'" + TextBox_ResourceGroup.Text 
                    + "\' -Location " + TextBox_Location.Text;
                var AzDeploymentCommand = "New-AzResourceGroupDeployment -ResourceGroupName " + TextBox_ResourceGroup.Text
                    + " -TemplateFile \'" + ARMTempFilePath + "\' -Verbose";

                var CliResourceGroupCommand = "az group create -l " + TextBox_Location.Text
                    + " -n " + TextBox_ResourceGroup.Text;
                var CliDeploymentCommand = "az group deployment create -g " + TextBox_ResourceGroup.Text
                    + " --template-file \'" + ARMTempFilePath + "\' --verbose";

                // Create a CopyCommand Dialog Window
                var CopyCommandDialog = new CopyCommandWindow(AzureRmResourceGroupCommand, AzureRmDeploymentCommand, 
                    AzResourceGroupCommand, AzDeploymentCommand,
                    CliResourceGroupCommand, CliDeploymentCommand);
                // Make the dialog window in appear in the center of the main window
                CopyCommandDialog.Owner = Application.Current.MainWindow;
                CopyCommandDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                // Show dialog window
                CopyCommandDialog.ShowDialog();

            }

        }

        // Exit the program
        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
