using WebServer.Attributes;
using System.Net;
using HTMLEngineLibrary;

namespace WebServer.Controllers
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        public string AcademicDegree { get; set; }
        public int Subject { get; set; }
    }

    public class Subject
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public int Hours { get; set; }
    }

    [ApiController]
    public class Teachers : IMainController
    {
        private readonly ITeacherRepository _db =
            new TeacherRepository(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UniversityDB;Integrated Security=True;");

        [HttpGET("list")]
        public void GetHtml(HttpListenerRequest request, HttpListenerResponse response)
        {
            var cookie = request.Cookies["SessionId"];
            var part = new HTMLPart();
            part.Part = string.Join('\n', _db.Query(new TeacherSpecification())
                .Select(t => $"<div class=\"content-block\">\n<div><a href=/teachers/id/{t.TeacherId}>{t.Firstname + ' ' + t.Lastname + ' ' + t.Patronymic}</a></div>\n</div>"));

            response.ContentType = "text/html";

            byte[] buffer = new EngineHTMLService().GetHTMLInByte(
                new EngineHTMLService().GetHTMLInByte(File.ReadAllBytes("./Template/teachers.html"), part),
                Tuple.Create("cookie", cookie));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
        }

        [HttpGET("id")]
        public void GetTeacherByid(int id, HttpListenerRequest request, HttpListenerResponse response)
        {
            var cookie = request.Cookies["SessionId"];
            var teacher = _db.Query(new TeacherSpecificationById(id)).FirstOrDefault();
            var subject = new Subjects().GetSubjectById(teacher.Subject);
            var engine = new EngineHTMLService();

            response.ContentType = "text/html";

            byte[] buffer = engine.GetHTMLInByte(engine.GetHTMLInByte(
                engine.GetHTMLInByte(File.ReadAllBytes("./Template/teacher.html"), teacher),
                Tuple.Create("cookie", cookie)), subject);
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
        }
    }
}
