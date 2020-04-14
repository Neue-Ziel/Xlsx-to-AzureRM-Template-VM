using Newtonsoft.Json;
using System.Collections.Generic;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    class Resource_NetworkInterface : Resource
    {
        public Resource_NetworkInterface(bool _multiVM, bool _nsgOnNIC, bool _usePIP, bool _createNewVnet)
        {
            Name = _multiVM ?
                "[concat(variables('VM_NamePrefix'), copyIndex(1), '-nic')]" :
                "[concat(variables('VM_NamePrefix'), '-nic')]";

            Type = "Microsoft.Network/networkInterfaces";
            ApiVersion = "2018-04-01";
            Location = "[variables('Location')]";

            DependsOn = new List<string>();

            if (_createNewVnet)
            {
                DependsOn.Add("[concat('Microsoft.Network/virtualNetworks/', variables('VNET_Name'))]");
            }

            if (_usePIP)
            {
                if (_multiVM)
                {
                    DependsOn.Add("[concat('Microsoft.Network/publicIpAddresses/', variables('VM_NamePrefix'), copyIndex(1), '-pip')]");
                }
                else
                {
                    DependsOn.Add("[concat('Microsoft.Network/publicIpAddresses/', variables('VM_NamePrefix'), '-pip')]");
                }
            }

            if (_nsgOnNIC)
            {
                DependsOn.Add("[concat('Microsoft.Network/networkSecurityGroups/', variables('NSG_Name'))]");
            }

            if(_multiVM)
            {
                Copy = new CopyClass { Name = "MultiVMNICLoop", Count = "[variables('VM_InstanceCount')]" };
            }

            Properties = new Property_NetworkInterface(_multiVM, _nsgOnNIC, _usePIP);
        }
    }

    class Property_NetworkInterface : PropertyClass
    {
        [JsonProperty("ipConfigurations")]
        public List<NetworkInterfaces_Properties_IpConfiguration> ipConfigurations { get; set; }

        [JsonProperty("networkSecurityGroup")]
        public NetworkInterfaces_Properties_NetworkSecurityGroup networkSecurityGroup { get; set; }
        
        public Property_NetworkInterface(bool _multiVM, bool _nsgOnNIC, bool _enablePIP)
        {
            ipConfigurations = new List<NetworkInterfaces_Properties_IpConfiguration>();
            ipConfigurations.Add(new NetworkInterfaces_Properties_IpConfiguration(_multiVM, _enablePIP));

            if (_nsgOnNIC)
            {
                networkSecurityGroup = new NetworkInterfaces_Properties_NetworkSecurityGroup();
            }
        }
    }

    class NetworkInterfaces_Properties_IpConfiguration
    {
        [JsonProperty("name")]
        readonly string name = "ipconfig1";

        [JsonProperty("properties")]
        public NetworkInterfaces_Properties_IpConfiguration_Properties IpConfiguration_Properties { get;set;}

        public NetworkInterfaces_Properties_IpConfiguration(bool _multiVM, bool _enablePIP)
        {
            IpConfiguration_Properties = new NetworkInterfaces_Properties_IpConfiguration_Properties(_multiVM, _enablePIP);
        }

    }

    class NetworkInterfaces_Properties_IpConfiguration_Properties
    {
        [JsonProperty("subnet")]
        public NetworkInterfaces_Properties_IpConfiguration_Properties_Subnet subnet { get; set; }
        [JsonProperty("privateIPAllocationMethod")]
        readonly string privateIPAllocationMethod = "Static";
        [JsonProperty("privateIPAddressVersion")]
        readonly string privateIPAddressVersion = "IPv4";
        [JsonProperty("privateIPAddress")]
        readonly string privateIPAddress;
        [JsonProperty("publicIPAddress")]
        readonly NetworkInterfaces_Properties_IpConfiguration_Properties_PublicIPAddress publicIPAddress;

        public NetworkInterfaces_Properties_IpConfiguration_Properties(bool _multiVM, bool _usePIP)
        {
            subnet = new NetworkInterfaces_Properties_IpConfiguration_Properties_Subnet();

            privateIPAddress = _multiVM ?
                "[concat(variables('IP_Internal_1_3'), add(variables('IP_Internal_4'), copyIndex()))]" :
                "[concat(variables('IP_Internal_1_3'), variables('IP_Internal_4') )]";

            if (_usePIP)
            {
                publicIPAddress = new NetworkInterfaces_Properties_IpConfiguration_Properties_PublicIPAddress(_multiVM);
            }
        }
    }

    class NetworkInterfaces_Properties_IpConfiguration_Properties_Subnet
    {
        [JsonProperty("id")]
        readonly string id = "[variables('subnetRef')]";
    }

    class NetworkInterfaces_Properties_IpConfiguration_Properties_PublicIPAddress
    {
        [JsonProperty("id")]
        readonly string id;
        public NetworkInterfaces_Properties_IpConfiguration_Properties_PublicIPAddress(bool _multiVM)
        {
            id = _multiVM ?
                "[resourceId(variables('ResourceGroup_Name'),'Microsoft.Network/publicIpAddresses', concat(variables('VM_NamePrefix'), copyIndex(1), '-pip'))]"
                : "[resourceId(variables('ResourceGroup_Name'),'Microsoft.Network/publicIpAddresses', concat(variables('VM_NamePrefix'), '-pip'))]";
        }
    }

    class NetworkInterfaces_Properties_NetworkSecurityGroup
    {
        [JsonProperty("id")]
        readonly string id = "[resourceId(variables('ResourceGroup_Name'), 'Microsoft.Network/networkSecurityGroups', variables('NSG_Name'))]";
    }
}
