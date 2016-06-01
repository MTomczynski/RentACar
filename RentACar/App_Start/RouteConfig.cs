using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RentACar
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "PaymentApi",
                url: "Payment/DotpayConfirm/",
                defaults: new { controller = "Rents", action = "DotpayConfirm"}
                );
            routes.MapRoute(
                name: "AfterPayment",
                url: "Payment/Thanks",
                defaults: new {controller = "Rents", action = "PaymentThanks"});
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
