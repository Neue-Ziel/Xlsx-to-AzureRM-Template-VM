using System.Collections.Generic;
using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    // Reference https://docs.microsoft.com/en-us/azure/templates/microsoft.compute/2018-04-01/virtualmachines
    class Resource_VirtualMachine : Resource
    {
        public Resource_VirtualMachine(bool _multiVM, bool _hasDataDisk, bool _useAvSet, bool _useDiagAcc, bool _createNewDiagAcc)
        {
            // Name
            Name = _multiVM ?
                "[concat(variables('VM_NamePrefix'), copyIndex(1))]" :
                "[variables('VM_NamePrefix')]";

            // Type
            Type = "Microsoft.Compute/virtualMachines";

            // ApiVersion
            ApiVersion = "2018-04-01";

            // Location
            Location = "[variables('location')]";

            // DependsOn
            DependsOn = new List<string>();

            // NIC, must have, but multiple VMs and single VM wil have different names.
            if (_multiVM)
            {
                DependsOn.Add("[concat('Microsoft.Network/networkInterfaces/', variables('VM_NamePrefix'), copyIndex(1), '-nic')]");
            }
            else
            {
                DependsOn.Add("[concat('Microsoft.Network/networkInterfaces/', variables('VM_NamePrefix'), '-nic')]");
            }

            // Availability Set
            if (_useAvSet)
            {
                DependsOn.Add("[concat('Microsoft.Compute/availabilitySets/', variables('AvSet_Name'))]");
            }

            // Diagnostic Account if create required
            if (_createNewDiagAcc)
            {
                DependsOn.Add("[concat('Microsoft.Storage/storageAccounts/', variables('DiagnosticsStorageAccount_Name'))]");
            }

            // Copy, for multiple VMs
            if (_multiVM)
            {
                Copy = new CopyClass { Name = "MultiVMLoop", Count = "[variables('VM_InstanceCount')]" };
            }

            // Property
            Properties = new Property_VirtualMachine(_multiVM, _hasDataDisk, _useAvSet, _useDiagAcc);

        }
    }

    /// <summary>
    /// The Property class for VirtualMachine resource.
    /// </summary>
    class Property_VirtualMachine : PropertyClass
    {
        [JsonProperty("osProfile")]
        private OsProfile osProfile { get; set; }

        [JsonProperty("hardwareProfile")]
        private HardwareProfile hardwareProfile { get; set; }

        [JsonProperty("storageProfile")]
        private StorageProfile storageProfile { get; set; }

        [JsonProperty("networkProfile")]
        private NetworkProfile networkProfile { get; set; }

        [JsonProperty("diagnosticsProfile")]
        private DiagnosticsProfile diagnosticsProfile { get; set; }

        [JsonProperty("availabilitySet")]
        private AvailabilitySet availabilitySet { get; set; }

        /// <summary>
        /// Virtual Machine property
        /// </summary>
        /// <param name="_multiVM"></param>
        /// <param name="_hasDataDisk"></param>
        /// <param name="_useAvSet"></param>
        /// <param name="_useDiagAcc"></param>
        public Property_VirtualMachine(bool _multiVM, bool _hasDataDisk, bool _useAvSet, bool _useDiagAcc)
        {
            osProfile = new OsProfile(_multiVM);
            hardwareProfile = new HardwareProfile();
            storageProfile = new StorageProfile(_hasDataDisk);
            networkProfile = new NetworkProfile(_multiVM);
            // Diagnostic Account
            if(_useDiagAcc)
            {
                diagnosticsProfile = new DiagnosticsProfile();
            }
            // Availability Set
            if (_useAvSet)
            {
                availabilitySet = new AvailabilitySet();
            }

        }





        class OsProfile
        {
            [JsonProperty("computerName")]
            string ComputerName;

            [JsonProperty("adminUsername")]
            readonly string AdminUsername = "[variables('AdminUsername')]";

            [JsonProperty("adminPassword")]
            readonly string AdminPassword = "[variables('AdminPassword')]";

            public OsProfile(bool _multiVM)
            {
                if(_multiVM)
                {
                    ComputerName = "[concat(variables('VM_NamePrefix'), copyIndex(1))]";
                }
                else
                {
                    ComputerName = "[variables('VM_NamePrefix')]";
                }

            }
        }

        class HardwareProfile
        {
            [JsonProperty("vmSize")]
            readonly string VMSize = "[variables('VM_Size')]";
        }

        class StorageProfile
        {
            [JsonProperty("imageReference")]
            private ImageReference imageReference { get; set; }

            [JsonProperty("osDisk")]
            private OsDisk osDisk { get; set; }

            [JsonProperty("copy")]
            private List<CopyClass> copyList { get; set; }

            public StorageProfile(bool _hasDataDisk)
            {
                imageReference = new ImageReference();

                osDisk = new OsDisk();

                if (_hasDataDisk)
                {
                    copyList = new List<CopyClass>();
                    copyList.Add(new CopyClass
                    {
                        Name = "dataDisks",
                        Count = "[variables('VM_DataDiskCount')]",
                        Input = new InputClass
                        {
                            Lun = "[copyIndex('dataDisks')]",
                            CreateOption = "Empty",
                            DiskSizeGB = "[variables('VM_DataDiskSize')]"
                        }
                    }
                    );
                }
            }


            class ImageReference
            {
                [JsonProperty("publisher")]
                readonly string Publisher = "[variables('Image_Publisher')]";
                [JsonProperty("offer")]
                readonly string Offer = "[variables('Image_Offer')]";
                [JsonProperty("sku")]
                readonly string Sku = "[variables('Image_Sku')]";
                [JsonProperty("version")]
                readonly string Version = "latest";
            }

            class OsDisk
            {
                [JsonProperty("createOption")]
                readonly string CreateOption = "fromImage";

                [JsonProperty("managedDisk")]
                private ManagedDisk managedDisk { get; set; }

                public OsDisk()
                {
                    managedDisk = new ManagedDisk();
                }

                class ManagedDisk
                {
                    [JsonProperty("storageAccountType")]
                    readonly string StorageAccountType = "[variables('VM_DiskStorageType')]";
                }
            }
            
        }

        class NetworkProfile
        {
            [JsonProperty("networkInterfaces")]
            private List<NetworkInterface> networkInterfaces;

            public NetworkProfile(bool _multiVM)
            {
                networkInterfaces = new List<NetworkInterface>();
                networkInterfaces.Add(new NetworkInterface(_multiVM));
            }
            class NetworkInterface
            {
                [JsonProperty("id")]
                private string id;
                public NetworkInterface(bool _multiVM)
                {
                    if(_multiVM)
                    {
                        id = "[resourceId('Microsoft.Network/networkInterfaces', concat(variables('VM_NamePrefix'), copyIndex(1), '-nic'))]";
                    }
                    else
                    {
                        id = "[resourceId('Microsoft.Network/networkInterfaces', concat(variables('VM_NamePrefix'), '-nic'))]";
                    }

                }
            }
        }

        class DiagnosticsProfile
        {
            [JsonProperty("bootDiagnostics")]
            private BootDiagnostics bootDiagnostics { get; set; }
            public DiagnosticsProfile()
            {
                bootDiagnostics = new BootDiagnostics();
            }
            class BootDiagnostics
            {
                [JsonProperty("enabled")]
                readonly bool enabled = true;

                [JsonProperty("storageUri")]
                readonly string StorageUri = "[concat('https://', variables('DiagnosticsStorageAccount_Name'), variables('storageSuffix'))]";
            }
        }

        class AvailabilitySet
        {
            [JsonProperty("id")]
            readonly string id = "[resourceId('Microsoft.Compute/availabilitySets', variables('AvSet_Name'))]";
        }

    }



}
