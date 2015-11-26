using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace HorseLeague
{
    public partial class _Default : Page
    {
        public void Page_Load(object sender, System.EventArgs e)
        {
            string originalPath = Request.Path;
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            HttpContext.Current.Server.TransferRequest(Request.ApplicationPath);
            HttpContext.Current.RewritePath(originalPath, false);

            //HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            //IHttpHandler httpHandler = new MvcHttpHandler();
            //httpHandler.ProcessRequest(HttpContext.Current);
        }
    }
}
