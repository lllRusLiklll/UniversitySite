using WebServer.Attributes;
using System.Net;
using System.Text;
using HTMLEngineLibrary;

namespace WebServer.Controllers
{
    [ApiController]
    public class Personal : IMainController
    {
        [HttpGET("account")]
        public void GetHtml(HttpListenerRequest request, HttpListenerResponse response)
        {
            var manager = SessionManager.GetInstance();
            var cookie = request.Cookies["SessionId"];
            Student student;
            if (cookie != null)
            {
                if (manager.CheckSession(Guid.Parse(cookie.Value)))
                {
                    var session = manager.GetInformation(Guid.Parse(cookie.Value));
                    student = new Students().GetAccountById(session.AccountId);
                    if (student == null)
                    {
                        student = new Student();
                    }

                    response.ContentType = "text/html";

                    byte[] buffer = new EngineHTMLService().GetHTMLInByte(File.ReadAllBytes("./Template/account.html"), student);
                    response.ContentLength64 = buffer.Length;

                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }
                else
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string error = "401 - not found";

                    byte[] buffer = Encoding.UTF8.GetBytes(error);

                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }
            }
            else
            {
                response.Redirect("/login.html");
                Stream output = response.OutputStream;

                output.Close();
            }
                
        }

        [HttpGET("edit")]
        public void EditAccount(HttpListenerRequest request, HttpListenerResponse response)
        {
            var manager = SessionManager.GetInstance();
            var cookie = request.Cookies["SessionId"];
            Student student;
            if (cookie != null)
            {
                if (manager.CheckSession(Guid.Parse(cookie.Value)))
                {
                    var session = manager.GetInformation(Guid.Parse(cookie.Value));
                    student = new Students().GetAccountById(session.AccountId);
                    if (student == null)
                    {
                        student = new Student();
                    }

                    response.ContentType = "text/html";

                    byte[] buffer = new EngineHTMLService().GetHTMLInByte(
                        File.ReadAllBytes("./Template/editAccount.html"), student);
                    response.ContentLength64 = buffer.Length;

                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }
                else
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string error = "401 - not found";

                    byte[] buffer = Encoding.UTF8.GetBytes(error);

                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }
            }
            else
            {
                response.Redirect("/login.html");
                Stream output = response.OutputStream;

                output.Close();
            }
        }
    }
}
