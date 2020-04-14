using System.Collections.Generic;
using Newtonsoft.Json;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    class Resource_NetworkSecurityGroup : Resource
    {
        public Resource_NetworkSecurityGroup()
        {
            Name = "[variables('NSG_Name')]";
            Type = "Microsoft.Network/networkSecurityGroups";
            ApiVersion = "2018-04-01";
            Location = "[variables('Location')]";
            Properties = new Property_NetworkSecurityGroup();
        }
    }


    /// <summary>
    /// To be used in the variables segment for my ARM template, may be used in property of the nsg source or in parameters segment.
    /// </summary>
    class Property_NetworkSecurityGroup : PropertyClass
    {
        [JsonProperty("securityRules")]
        readonly string securityRules = "[variables('NSG_Rules')]";
    }

    class SecurityRule
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("properties")]
        public SecurityRule_Property properties { get; set; }
    }

    class SecurityRule_Property
    {
        [JsonProperty("priority")]
        public int priority { get; set; }

        [JsonProperty("protocol")]
        public string protocol { get; set; }

        [JsonProperty("access")]
        public string access { get; set; }

        [JsonProperty("direction")]
        public string direction { get; set; }

        [JsonProperty("sourceAddressPrefix")]
        public string sourceAddressPrefix { get; set; }

        [JsonProperty("sourceAddressPrefixes")]
        public string[] sourceAddressPrefixes { get; set; }

        [JsonProperty("sourcePortRange")]
        public string sourcePortRange { get; set; }

        [JsonProperty("sourcePortRanges")]
        public string[] sourcePortRanges { get; set; }

        [JsonProperty("destinationAddressPrefix")]
        public string destinationAddressPrefix { get; set; }

        [JsonProperty("destinationAddressPrefixes")]
        public string[] destinationAddressPrefixes { get; set; }

        [JsonProperty("destinationPortRange")]
        public string destinationPortRange { get; set; }

        [JsonProperty("destinationPortRanges")]
        public string[] destinationPortRanges { get; set; }

        public SecurityRule_Property(string[] _sourceAddressPrefixes, string[] _sourcePortRanges, 
            string[] _destinationAddressPrefixes, string[] _destinationPortRanges)
        {
            // sourceAddressPrefix(es)
            if (_sourceAddressPrefixes.Length > 1)
            {
                this.sourceAddressPrefixes = _sourceAddressPrefixes;
            }
            else
            {
                this.sourceAddressPrefix = _sourceAddressPrefixes[0];
            }

            // sourcePortRange(s)
            if (_sourcePortRanges.Length > 1)
            {
                this.sourcePortRanges = _sourcePortRanges;
            }
            else
            {
                this.sourcePortRange = _sourcePortRanges[0];
            }

            // destinationAddressPrefix(es)
            if (_destinationAddressPrefixes.Length > 1)
            {
                this.destinationAddressPrefixes = _destinationAddressPrefixes;
            }
            else
            {
                this.destinationAddressPrefix = _destinationAddressPrefixes[0];
            }

            // destinationPortRange(s)
            if (_destinationPortRanges.Length > 1)
            {
                this.destinationPortRanges = _destinationPortRanges;
            }
            else
            {
                this.destinationPortRange = _destinationPortRanges[0];
            }

        }
    }
}
