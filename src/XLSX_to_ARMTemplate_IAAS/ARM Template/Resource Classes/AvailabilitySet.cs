
using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    class Resource_AvailabilitySet : Resource
    {
        public Resource_AvailabilitySet()
        {
            Name = "[variables('AvSet_Name')]";
            Type = "Microsoft.Compute/availabilitySets";
            ApiVersion = "2018-04-01";
            Location = "[variables('Location')]";
            Properties = new Property_AvailabilitySet();
            Sku = new SKUClass { name = "Aligned" };

        }

    }

    class Property_AvailabilitySet : PropertyClass
    {
        [JsonProperty("platformFaultDomainCount")]
        readonly string platformFaultDomainCount = "[variables('AvSet_PlatformFaultDomainCount')]";

        [JsonProperty("platformUpdateDomainCount")]
        readonly string platformUpdateDomainCount = "[variables('AvSet_PlatformUpdateDomainCount')]";

        // This property only valid for apiversion 2016-04-30-preview
        //[JsonProperty("managed")]
        //readonly bool managed = true;

    }
}
