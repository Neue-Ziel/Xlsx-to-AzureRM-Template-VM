using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template
{
    // Reference https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates
    class Template
    {

        [JsonProperty("$schema", Order = 1)]
        [JsonRequired]
        public readonly string schema = "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#";

        [JsonProperty("contentVersion", Order = 2)]
        [JsonRequired]
        public readonly string contentVersion = "1.0.0.0";

        [JsonProperty("variables", Order = 3)]
        [JsonRequired]
        public Variables_IAAS variables { get; set; }

        [JsonProperty("resources", Order = 4)]
        [JsonRequired]
        public List<Resource> resources { get; set; }

        [JsonProperty("outputs", Order = 5)]
        [JsonRequired]
        public Outputs outputs { get; } = new Outputs();

    }

}
