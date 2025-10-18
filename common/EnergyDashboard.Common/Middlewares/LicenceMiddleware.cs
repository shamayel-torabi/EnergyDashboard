using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using EnergyDashboard.Common.Options;

namespace EnergyDashboard.Common.Middlewares
{
    public class LicenceMiddleware
    {
        private const string html =
           "<!DOCTYPE html>" +
           "<html dir='rtl'>" +
           "<head>" +
           "<meta charset=\"UTF-8\">" +
           "<title>خطا</title>" +
               "<style>" +
               "body {display: flex; flex-direction: column; align-items: center; justify-content: center; height: 100vh }" +
               "</style>" +
           "</head>" +
           "<body>" +
           "<h1 style='text-align:center; color:#FF0000'>شما مجوز استفاده از نرم افزار را ندارید!!!!</h1>" +
           "<h1 style='text-align:center; color:#FF0000'>برای دریافت مجوز با مشاوران برنامه در شرکت دانش بنیان پایش گستر  <a href='http://www.pitc.co.ir/' style='text-decoration:none;'>پرمون</a> تماس بگیرید</h1>" +
           "</body>" +
           "</html>";


        private readonly RequestDelegate next;

        public LicenceMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IOptionsMonitor<LicenceSettings> option)
        {
            if (option == null || !option.CurrentValue.Status)
            {
                await context.Response.WriteAsync(html);
            }
            else
                await next(context);
        }
    }
}
