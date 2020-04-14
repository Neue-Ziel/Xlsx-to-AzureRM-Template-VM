using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template
{

    /// <summary>
    /// The class for outputs
    /// </summary>
    class Outputs
    {
        public Outputs_AdminUserName adminUsername { get; }
        = new Outputs_AdminUserName();


    }

    class Outputs_AdminUserName
    {
        [JsonProperty("type")]
        private string type { get; } = "string";

        [JsonProperty("value")]
        private string value { get; } = "[variables('adminUsername')]";
    }

}
