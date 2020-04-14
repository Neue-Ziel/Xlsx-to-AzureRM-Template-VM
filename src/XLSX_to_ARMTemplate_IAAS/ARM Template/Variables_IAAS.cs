using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template
{
    class Variables_IAAS
    {
        /// <summary>
        /// No need the JsonProperty field as the variable name is exactly same as it in the Json.
        /// </summary>
        public string Location { get; set; }
        public string ResourceGroup_Name { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string VNET_ResourceGroup { get; set; }
        public string VNET_Name { get; set; }
        public string VNET_AddressSpace { get; set; }
        public string Subnet_Name { get; set; }
        public string Subnet_AddressSpace { get; set; }
        public string NSG_Name { get; set; }
        public string VM_NamePrefix { get; set; }
        public string VM_Size { get; set; }
        public string VM_DiskStorageType { get; set; }
        public Nullable<int> VM_InstanceCount { get; set; } = null;
        public Nullable<int> VM_DataDiskSize { get; set; } = null;
        public Nullable<int> VM_DataDiskCount { get; set; } = null;
        public string Image_Publisher { get; set; }
        public string Image_Offer { get; set; }
        public string Image_Sku { get; set; }
        public string IP_Internal_1_3 { get; set; }
        public int IP_Internal_4 { get; set; }
        public string PIP_Sku { get; }
        public string AvSet_Name { get; set; }
        public Nullable<int> AvSet_PlatformFaultDomainCount { get; } = null;
        public Nullable<int> AvSet_PlatformUpdateDomainCount { get; } = null;
        public string DiagnosticsStorageAccount_Name { get; set; }
        public string DiagnosticsStorageAccount_Type { get; }
        public string DiagnosticsStorageAccount_Kind { get; }
        public object Tags_Common { get; set; } // currently not utilized

        public string vnetId { get; } = "[resourceId(variables('VNET_ResourceGroup'), 'Microsoft.Network/virtualNetworks', variables('VNET_Name'))]";
        public string subnetRef { get; } = "[concat(variables('vnetId'), '/subnets/', variables('Subnet_Name'))]";
        public string storageSuffix { get; }

        public List<SecurityRule> NSG_Rules { get; set; }

        public Variables_IAAS(bool _usePIP, bool _useAvSet, bool _useDiagAcc, bool _createNewDiagAcc)
        {
            // If Utilise PIP
            if (_usePIP)
            {
                PIP_Sku = "Basic";
            }

            // If Utilise Availability Set
            if (_useAvSet)
            {
                AvSet_PlatformFaultDomainCount = 2;
                AvSet_PlatformUpdateDomainCount = 5;
            }

            // If Utilise Diagnostic Account
            if (_useDiagAcc)
            {
                storageSuffix = "[if(contains(toLower(variables('Location')), 'china'), '.blob.core.chinacloudapi.cn/', '.blob.core.windows.net/')]";

                // If the Diagnostic Account needs to be created
                if(_createNewDiagAcc)
                {
                    DiagnosticsStorageAccount_Type = "Standard_LRS";
                    DiagnosticsStorageAccount_Kind = "Storage";
                }
            }
        }
    }

}
