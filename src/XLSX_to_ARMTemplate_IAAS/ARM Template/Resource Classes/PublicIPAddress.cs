using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    class Resource_PublicIPAddress : Resource
    {
        public Resource_PublicIPAddress(bool _multiVM)
        {
            Name = _multiVM ?
                "[concat(variables('VM_NamePrefix'), copyIndex(1), '-pip')]"
                : "[concat(variables('VM_NamePrefix'), '-pip')]";

            Type = "Microsoft.Network/publicIPAddresses";
            ApiVersion = "2018-04-01";
            Location = "[variables('Location')]";

            if (_multiVM)
            {
                Copy = new CopyClass { Name = "MultiVMPIPLoop", Count = "[variables('VM_InstanceCount')]" };
            }

            Properties = new Property_PublicIPAddress();

            Sku = new SKUClass { name = "[variables('PIP_Sku')]" };
        }
    }

    class Property_PublicIPAddress : PropertyClass
    {
        [JsonProperty("publicIPAllocationMethod")]
        readonly string publicIPAllocationMethod = "Static";

    }
}
