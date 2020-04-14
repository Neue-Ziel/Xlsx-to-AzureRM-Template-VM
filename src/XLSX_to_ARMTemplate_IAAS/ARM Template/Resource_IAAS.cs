using System.Collections.Generic;

using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template
{
    // Reference https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-manager-templates-resources
    abstract class Resource
    {
        [JsonProperty("name")]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonProperty("apiVersion")]
        [JsonRequired]
        public string ApiVersion { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("tags")]
        public object Tags { get; set; }  // currently not utilized

        [JsonProperty("dependsOn")]
        public List<string> DependsOn { get; set; }

        [JsonProperty("copy")]
        public CopyClass Copy { get; set; }

        [JsonProperty("properties")]
        [JsonRequired]
        public PropertyClass Properties { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("sku")]
        public SKUClass Sku { get; set; }
    }

    /// <summary>
    /// The copy segment
    /// </summary>
    class CopyClass
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("count")]
        public string Count { get; set; }

        [JsonProperty("input")]
        public InputClass Input { get; set; }

    }

    /// <summary>
    /// The input segment
    /// </summary>
    class InputClass
    {
        public InputClass()
        {
            managedDisk = new ManagedDisk();
        }

        [JsonProperty("lun")]
        public string Lun { get; set; }

        [JsonProperty("createOption")]
        public string CreateOption { get; set; }

        [JsonProperty("diskSizeGB")]
        public string DiskSizeGB { get; set; }

        [JsonProperty("managedDisk")]
        private ManagedDisk managedDisk{ get; set; }

        class ManagedDisk
        {
            [JsonProperty("storageAccountType")]
            readonly string StorageAccountType = "[variables('VM_DiskStorageType')]";
        }
    }

    /// <summary>
    /// The sku segment.
    /// </summary>
    class SKUClass
    {
        public string name { get; set; }
    }

    /// <summary>
    /// Base Class for property.
    /// </summary>
    abstract class PropertyClass
    {

    }
}
