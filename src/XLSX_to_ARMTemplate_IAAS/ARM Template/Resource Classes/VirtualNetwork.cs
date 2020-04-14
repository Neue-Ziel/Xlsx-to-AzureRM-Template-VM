using System.Collections.Generic;

using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    class Resource_VirtualNetwork : Resource
    {
        public Resource_VirtualNetwork()
        {
            Name = "[variables('VNET_Name')]";
            Type = "Microsoft.Network/virtualNetworks";
            ApiVersion = "2018-04-01";
            Location = "[variables('Location')]";
            Properties = new Property_VirtualNetwork();
        }
    }

    class Property_VirtualNetwork : PropertyClass
    {
        [JsonProperty("addressSpace")]
        readonly VirtualNetworks_Property_AddressSpace addressSpace = new VirtualNetworks_Property_AddressSpace();

        [JsonProperty("subnets")]
        readonly List<VirtualNetworks_Property_Subnet> subnets = new List<VirtualNetworks_Property_Subnet>()
        { new VirtualNetworks_Property_Subnet() };
    }

    class VirtualNetworks_Property_AddressSpace
    {
        [JsonProperty("addressPrefixes")]
        readonly List<string> addressPrefixes = new List<string> { "[variables('VNET_AddressSpace')]" };
    }

    class VirtualNetworks_Property_Subnet
    {
        [JsonProperty("name")]
        readonly string name = "[variables('Subnet_Name')]";
        [JsonProperty("properties")]
        readonly VirtualNetworks_Property_Subnet_Properties properties = new VirtualNetworks_Property_Subnet_Properties();

    }

    class VirtualNetworks_Property_Subnet_Properties
    {
        [JsonProperty("addressPrefix")]
        readonly string addressPrefix = "[variables('Subnet_AddressSpace')]";
    }

}
