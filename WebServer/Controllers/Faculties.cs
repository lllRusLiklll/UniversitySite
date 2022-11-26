using WebServer.Attributes;
using System.Net;
using HTMLEngineLibrary;

namespace WebServer.Controllers
{
    public class Faculty
    {
        public int FacultyId { get; set; }
        public string Name { get; set; }
        public int StudentsNumber { get; set; }
    }

    public class HTMLPart
    {
        public string Part { get; set; }
    }

    [ApiController]
    public class Faculties : IMainController
    {
        private readonly IFacultyRepository _db =
            new FacultyRepository(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UniversityDB;Integrated Security=True;");

        [HttpGET("list")]
        public void GetHtml(HttpListenerRequest request, HttpListenerResponse response)
        {
            var cookie = request.Cookies["SessionId"];
            var part = new HTMLPart();
            part.Part = string.Join('\n', _db.Query(new FacultySpecification())
                .Select(f => $"<div class=\"faculty-block\">\n<div>Факультет: <b>{f.Name}</b></div>\n<div>Кол-во учащихся: <b>{f.StudentsNumber}</b></div>\n</div>"));

            response.ContentType = "text/html";

            byte[] buffer = new EngineHTMLService().GetHTMLInByte(
                new EngineHTMLService().GetHTMLInByte(File.ReadAllBytes("./Template/faculties.html"), part),
                Tuple.Create("cookie", cookie));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
        }
    }
}
