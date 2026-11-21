using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static WarehouseRequest Warehouses()
                {
                    return new WarehouseRequest().SelectSelfFields();
                }

                public static WarehouseRequest WarehousesWithMinimalFields()
                {
                    return new WarehouseRequest();
                }
                public static SupplierRequest Suppliers()
                {
                    return new SupplierRequest().SelectSelfFields();
                }

                public static SupplierRequest SuppliersWithMinimalFields()
                {
                    return new SupplierRequest();
                }
    }
}