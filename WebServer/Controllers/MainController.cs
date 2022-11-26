using System.Net;

namespace WebServer.Controllers
{
    public interface IMainController
    {
        public void GetHtml(HttpListenerRequest request, HttpListenerResponse response);
    }
}
