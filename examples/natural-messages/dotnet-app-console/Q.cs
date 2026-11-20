using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static CatalogRequest Catalogs()
                {
                    return new CatalogRequest().SelectSelfFields();
                }

                public static CatalogRequest CatalogsWithMinimalFields()
                {
                    return new CatalogRequest();
                }
                public static EntryRequest Entries()
                {
                    return new EntryRequest().SelectSelfFields();
                }

                public static EntryRequest EntriesWithMinimalFields()
                {
                    return new EntryRequest();
                }
    }
}