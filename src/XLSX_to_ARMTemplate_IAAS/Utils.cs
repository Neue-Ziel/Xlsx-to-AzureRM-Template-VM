using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS
{
    static class Utils
    {
        /// <summary>
        /// Reads the NSG sheet and returns a NSG_Rules_Class instance.
        /// </summary>
        /// <param name="_nsgRulesSheet"></param>
        /// <returns></returns>
        public static List<ARM_Template.Resource_Classes.SecurityRule> ReadNSGRules(ISheet _nsgRulesSheet)
        {
            // Initialise a NSGRules instance
            var localNsgRules = new List<ARM_Template.Resource_Classes.SecurityRule>();

            // Start to read the sheet from line 2
            int row = 1;


            // Try get line 2
            var currentRow = _nsgRulesSheet.GetRow(row);


            // Set some temp variables to save the value
            string tempName;
            //int tempPriority;
            string tempProtocol;
            string tempAccess;
            string tempDirection;
            string[] tempSourceAddressPrefixes;
            string[] tempSourcePortRanges;
            string[] tempDestinationAddressPrefixes;
            string[] tempDestinationPortRanges;

            // Get each row and read cells
            while (currentRow != null &&
                currentRow.GetCell(0).CellType == CellType.String)
            {
                tempName = currentRow.GetCell(0).ToString().Trim().Replace(" ", "");

                // Read the priority using TryParse, because it is int
                if(!int.TryParse(currentRow.GetCell(1).ToString().Trim(), out int tempPriority))
                {
                    MessageBox.Show("NSG Priority format wrong.", "At Row " + row.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                }


                tempProtocol = currentRow.GetCell(2).ToString().Trim();
                tempAccess = currentRow.GetCell(3).ToString().Trim();
                tempDirection = currentRow.GetCell(4).ToString().Trim();
                // Get Source Address Prefix(es) first
                tempSourceAddressPrefixes = currentRow.GetCell(5).ToString().Trim().Replace(" ", "").Split(new char[] { ',', '\n' });
                tempSourcePortRanges = currentRow.GetCell(6).ToString().Trim().Replace(" ", "").Split(new char[] { ',', '\n' });
                tempDestinationAddressPrefixes = currentRow.GetCell(7).ToString().Trim().Replace(" ", "").Split(new char[] { ',', '\n' });
                tempDestinationPortRanges = currentRow.GetCell(8).ToString().Trim().Replace(" ", "").Split(new char[] { ',', '\n' });


                localNsgRules.Add(new ARM_Template.Resource_Classes.SecurityRule
                {
                    // Read the name
                    name = tempName,
                    // Read the properties
                    properties = new ARM_Template.Resource_Classes.SecurityRule_Property(
                                        tempSourceAddressPrefixes,
                                        tempSourcePortRanges,
                                        tempDestinationAddressPrefixes,
                                        tempDestinationPortRanges)
                    {
                        priority = tempPriority,
                        protocol = tempProtocol,
                        access = tempAccess,
                        direction = tempDirection
                    }
                }
                                );

                // Don't forget ++ and get new row.
                row++;
                currentRow = _nsgRulesSheet.GetRow(row);
            }

            return localNsgRules;
        }

        /// <summary>
        /// Validate the input field before creating.
        /// </summary>
        /// <param name="_Location"></param>
        /// <param name="_ResourceGroup"></param>
        /// <param name="_AdminUsername"></param>
        /// <param name="_AdminPassword"></param>
        /// <param name="_VNET_ResourceGroup"></param>
        /// <param name="_VNETName"></param>
        /// <param name="_VNETAddressSpace"></param>
        /// <param name="_SubnetName"></param>
        /// <param name="_SubnetAddressSpace"></param>
        /// <param name="_NSGName"></param>
        /// <param name="_NSGtoNIC"></param>
        /// <param name="_VMNamePrefix"></param>
        /// <param name="_VMSize"></param>
        /// <param name="_VMInstanceCount"></param>
        /// <param name="_VMDiskStorageType"></param>
        /// <param name="_VMDataDiskSize"></param>
        /// <param name="_VMDataDiskCount"></param>
        /// <param name="_ImagePublisher"></param>
        /// <param name="_ImageOffer"></param>
        /// <param name="_ImageSku"></param>
        /// <param name="_VMLANIP"></param>
        /// <param name="_UsePIP"></param>
        /// <param name="_AvSetName"></param>
        /// <param name="_DiagStrAccName"></param>
        /// <returns></returns>
        public static bool ValidateInputFields(string _Location, string _ResourceGroup,
            string _AdminUsername, string _AdminPassword,
            string _VNET_ResourceGroup, string _VNETName, string _VNETAddressSpace,
            string _SubnetName, string _SubnetAddressSpace,
            string _NSGName, bool _NSGtoNIC, 
            string _VMNamePrefix, string _VMSize, int _VMInstanceCount,
            string _VMDiskStorageType, int _VMDataDiskSize, int _VMDataDiskCount,
            string _ImagePublisher, string _ImageOffer, string _ImageSku,
            string _VMLANIP, bool _UsePIP, string _AvSetName, string _DiagStrAccName)
        {
            // Check if required fields are filled.
            if (_Location == null || _Location == "")
            {
                MessageBox.Show("Location Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_ResourceGroup == null || _ResourceGroup == "")
            {
                MessageBox.Show("Resource Group Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_AdminUsername == null || _AdminUsername == "")
            {
                MessageBox.Show("AdminUsername Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_AdminPassword == null || _AdminPassword == "")
            {
                MessageBox.Show("AdminPassword Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_VNETName == null || _VNETName == "")
            {
                MessageBox.Show("VNet Name Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_SubnetName == null || _SubnetName == "")
            {
                MessageBox.Show("Subnet Name Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_VMNamePrefix == null || _VMNamePrefix == "")
            {
                MessageBox.Show("VM Name Prefix Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_VMSize == null || _VMSize == "")
            {
                MessageBox.Show("VM Size Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_VMDiskStorageType == null || _VMDiskStorageType == "")
            {
                MessageBox.Show("VM Disk Storage Type Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_VMLANIP == null || _VMLANIP == "")
            {
                MessageBox.Show("VM LAN IP Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_ImagePublisher == null || _ImagePublisher == "")
            {
                MessageBox.Show("Image Publisher Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_ImageOffer == null || _ImageOffer == "")
            {
                MessageBox.Show("Image Offer Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_ImageSku == null || _ImageSku == "")
            {
                MessageBox.Show("Image Sku Required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_VNETAddressSpace != null && _VNETAddressSpace != "" && (_SubnetAddressSpace == null || _SubnetAddressSpace == ""))
            {
                MessageBox.Show("Subnet Address Space required while VNet Address Space is not empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create the template instance, to be used after validate.
        /// </summary>
        /// <param name="_Location"></param>
        /// <param name="_ResourceGroup"></param>
        /// <param name="_AdminUsername"></param>
        /// <param name="_AdminPassword"></param>
        /// <param name="_VNET_ResourceGroup"></param>
        /// <param name="_VNETName"></param>
        /// <param name="_VNETAddressSpace"></param>
        /// <param name="_SubnetName"></param>
        /// <param name="_SubnetAddressSpace"></param>
        /// <param name="_NSGName"></param>
        /// <param name="_NSGtoNIC"></param>
        /// <param name="_VMNamePrefix"></param>
        /// <param name="_VMSize"></param>
        /// <param name="_VMInstanceCount"></param>
        /// <param name="_VMDiskStorageType"></param>
        /// <param name="_VMDataDiskSize"></param>
        /// <param name="_VMDataDiskCount"></param>
        /// <param name="_ImagePublisher"></param>
        /// <param name="_ImageOffer"></param>
        /// <param name="_ImageSku"></param>
        /// <param name="_VMLANIP"></param>
        /// <param name="_UsePIP"></param>
        /// <param name="_AvSetName"></param>
        /// <param name="_DiagStrAccName"></param>
        /// <param name="_nsgRules"></param>
        /// <returns></returns>
        public static ARM_Template.Template GenerateTemplateClass(string _Location, string _ResourceGroup,
            string _AdminUsername, string _AdminPassword,
            string _VNET_ResourceGroup, string _VNETName, string _VNETAddressSpace,
            string _SubnetName, string _SubnetAddressSpace,
            string _NSGName, bool _NSGtoNIC,
            string _VMNamePrefix, string _VMSize, int _VMInstanceCount,
            string _VMDiskStorageType, int _VMDataDiskSize, int _VMDataDiskCount,
            string _ImagePublisher, string _ImageOffer, string _ImageSku,
            string _VMLANIP, bool _UsePIP, string _AvSetName, string _DiagStrAccName,
            List<ARM_Template.Resource_Classes.SecurityRule> _nsgRules,
            bool _DiagStrAccNew)
        {
            // Create a return instance
            var localTemplate = new ARM_Template.Template();


            // Determine some bool
            // More than 1 VM?
            bool multiVM = (_VMInstanceCount > 1);
            // Has Data Disk?
            bool hasDataDisk = ( (_VMDataDiskCount > 0) && (_VMDataDiskSize > 0) );
            // Use AvSet?
            //bool useAvSet = (_AvSetName != "" && _AvSetName != null);
            // Use DiagAccount?
            bool useDiagAcc = (_DiagStrAccName != "" && _DiagStrAccName != null);
            // The DiagAccount should be created?
            bool createNewDiagAcc = (useDiagAcc && _DiagStrAccNew);
            // VNet in a different Resource Group?
            bool differentRGforVNET = ((_VNET_ResourceGroup != "" && _VNET_ResourceGroup != null) 
                && _VNET_ResourceGroup != _ResourceGroup);
            // Create a new VNet?
            bool createNewVNet = (_VNETAddressSpace != "" && _VNETAddressSpace != null);

            // Create a new NSG?
            bool createNSG = (_NSGName != "" && _NSGName != null);

            // NSG attach to NIC? This will be true only when new NSG to be created, AND NSGtoNIC is ticked.
            bool attachNsgToNic = _NSGtoNIC && createNSG;

            // Utilize AvSet?
            bool createAvSet = (_AvSetName != "" && _AvSetName != null);

            ////////////////////////////////////////////////////////////

            // Initialize the Resource List
            localTemplate.resources = new List<ARM_Template.Resource>();

            // Initialize the Variables_IAAS List
            localTemplate.variables = new ARM_Template.Variables_IAAS(_UsePIP, createAvSet, useDiagAcc, createNewDiagAcc);

            // Add VM to Resource segment
            localTemplate.resources.Add(new ARM_Template.Resource_Classes.Resource_VirtualMachine(multiVM, hasDataDisk, createAvSet, useDiagAcc, createNewDiagAcc));
            // Add NIC to Resource segment
            localTemplate.resources.Add(new ARM_Template.Resource_Classes.Resource_NetworkInterface(multiVM, attachNsgToNic, _UsePIP, createNewVNet));
            // Location and RG
            localTemplate.variables.Location = _Location;
            localTemplate.variables.ResourceGroup_Name = _ResourceGroup;

            // OS Credential
            localTemplate.variables.AdminUsername = _AdminUsername;
            localTemplate.variables.AdminPassword = _AdminPassword;

            // VNET & Subnet
            localTemplate.variables.VNET_ResourceGroup = differentRGforVNET ? 
                _VNET_ResourceGroup : _ResourceGroup;

            localTemplate.variables.VNET_Name = _VNETName;
            if(createNewVNet && !differentRGforVNET)
            {
                localTemplate.variables.VNET_AddressSpace = _VNETAddressSpace;
                localTemplate.variables.Subnet_AddressSpace = _SubnetAddressSpace;
                // Add VNET to Resource segment
                localTemplate.resources.Add(new ARM_Template.Resource_Classes.Resource_VirtualNetwork());
            }
            localTemplate.variables.Subnet_Name = _SubnetName;

            // NSG
            if(createNSG)
            {
                localTemplate.variables.NSG_Name = _NSGName;
                localTemplate.resources.Add(new ARM_Template.Resource_Classes.Resource_NetworkSecurityGroup());
            }

            // VM
            localTemplate.variables.VM_NamePrefix = _VMNamePrefix;
            localTemplate.variables.VM_Size = _VMSize;
            // VM Instance Count
            if (_VMInstanceCount > 1)
                localTemplate.variables.VM_InstanceCount = _VMInstanceCount;

            localTemplate.variables.VM_DiskStorageType = _VMDiskStorageType;
            // Data Disk
            if (hasDataDisk)
            {
                localTemplate.variables.VM_DataDiskSize = _VMDataDiskSize;
                localTemplate.variables.VM_DataDiskCount = _VMDataDiskCount;
            }

            // Image
            localTemplate.variables.Image_Publisher = _ImagePublisher;
            localTemplate.variables.Image_Offer = _ImageOffer;
            localTemplate.variables.Image_Sku = _ImageSku;

            // LAN IP
            // Split VM LAN IP
            string[] tempLANIPArray = _VMLANIP.Split('.');
            localTemplate.variables.IP_Internal_1_3 = tempLANIPArray[0] + "." + tempLANIPArray[1] + "." + tempLANIPArray[2] + ".";
            int.TryParse(tempLANIPArray[3], out int temp_Internal_4);
            localTemplate.variables.IP_Internal_4 = temp_Internal_4;

            // AvSet
            if(createAvSet)
            {
                localTemplate.variables.AvSet_Name = _AvSetName;
                localTemplate.resources.Add(new ARM_Template.Resource_Classes.Resource_AvailabilitySet());
            }

            // Diag Account
            if(useDiagAcc)
            {
                localTemplate.variables.DiagnosticsStorageAccount_Name = _DiagStrAccName;

                if (createNewDiagAcc)
                {
                    localTemplate.resources.Add(new ARM_Template.Resource_Classes.Resource_DiagnosticsStorageAccount());
                }
            }

            // PIP
            if(_UsePIP)
            {
                localTemplate.resources.Add(new ARM_Template.Resource_Classes.Resource_PublicIPAddress(multiVM));
            }


            // Read the NSG Rules if exists
            if (_nsgRules != null && createNSG)
            {
                localTemplate.variables.NSG_Rules = _nsgRules;
            }
            else if (_nsgRules == null && createNSG)
            {
                localTemplate.variables.NSG_Rules = 
                    new List<ARM_Template.Resource_Classes.SecurityRule>() ;
            }


            return localTemplate;
        }

        /// <summary>
        /// Generate the JSON string.
        /// </summary>
        /// <param name="_template"></param>
        /// <returns></returns>
        public static string GenerateJson(ARM_Template.Template _template)
        {
            string json = (JsonConvert.SerializeObject
                (_template, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }).ToString());

            return json;
        }
    }
}
