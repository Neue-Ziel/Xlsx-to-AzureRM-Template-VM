using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    class Resource_ResourceGroup : Resource
    {
        public Resource_ResourceGroup()
        {
            Name = "[variables('ResourceGroup_Name')]";
            Type = "Microsoft.Resources/resourceGroups";
            ApiVersion = "2018-05-01";
            Location = "[variables('Location')]";
        }
    }
}
