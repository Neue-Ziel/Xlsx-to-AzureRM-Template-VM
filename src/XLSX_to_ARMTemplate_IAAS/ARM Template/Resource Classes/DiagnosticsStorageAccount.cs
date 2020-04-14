
namespace XLSX_to_ARMTemplate_IAAS.ARM_Template.Resource_Classes
{
    class Resource_DiagnosticsStorageAccount : Resource
    {
        public Resource_DiagnosticsStorageAccount()
        {
            Name = "[variables('DiagnosticsStorageAccount_Name')]";
            Type = "Microsoft.Storage/storageAccounts";
            ApiVersion = "2018-02-01";
            Location = "[variables('Location')]";
            Kind = "[variables('DiagnosticsStorageAccount_Kind')]";
            Sku = new SKUClass { name = "[variables('DiagnosticsStorageAccount_Type')]" };
        }
    }
}
