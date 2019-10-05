using System.Web;
using System.Web.Mvc;

namespace DEIS_ESTAGIOS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
