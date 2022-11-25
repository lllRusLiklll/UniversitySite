using WebServer.Attributes;
using System.Net;
using System.Text;

namespace WebServer.Controllers
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Group { get; set; }
        public int CreditBook { get; set; }
    }

    [ApiController]
    public class Students
    {
        private readonly IStudentRepository _db =
            new StudentRepository(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UniversityDB;Integrated Security=True;");

        [HttpGET("list")]
        public List<Student> GetStudents(HttpListenerRequest request, HttpListenerResponse response)
        {
            var cookie = request.Cookies["SessionId"];
            if (cookie != null)
                return _db.Query(new StudentSpecification());
            else
            {
                response.StatusCode = 401;
                return null;
            }
        }

        [HttpGET("item")]
        public Student? GetAccountById(int id)
        {
            return _db.Query(new StudentSpecificationById(id)).FirstOrDefault();
        }

        [HttpGET("info")]
        public Student GetAccountInfo(HttpListenerRequest request, HttpListenerResponse response)
        {
            var manager = SessionManager.GetInstance();
            var cookie = request.Cookies["SessionId"];
            if (cookie != null)
            {
                if (manager.CheckSession(Guid.Parse(cookie.Value)))
                {
                    var session = manager.GetInformation(Guid.Parse(cookie.Value));
                    var result = _db.Query(new StudentSpecificationById(session.AccountId)).FirstOrDefault();
                    if (result != null)
                        return result;
                }
            }
            response.StatusCode = 401;
            return null;

        }

        [HttpPOST("signup")]
        public void SaveStudent(string firstname, string lastname, string patronymic, string email,
            string password, string repeatPassword, DateTime birthdate, string gender, HttpListenerResponse response)
        {
            _db.Insert(new Student() 
            {
                Firstname = firstname,
                Lastname = lastname,
                Patronymic = patronymic,
                Gender = gender,
                BirthDate = birthdate,
                Email = email,
                Group = "11-106",
                CreditBook = 123456789
            });
            response.Redirect("login.html");
        }

        [HttpPOST("login")]
        public bool login(string email, string password, HttpListenerResponse response)
        {
            var student = _db.Query(new StudentSpecificationByEmailPassword(email, password)).First();
            if (true)
            {
                var sessionId = Guid.NewGuid();
                response.Headers.Set("Set-Cookie", $"SessionId={sessionId}; Path=/");

                var manager = SessionManager.GetInstance();
                var session = new Session()
                {
                    Id = sessionId,
                    AccountId = student.StudentId,
                    Email = student.Email,
                    CreatedDateTime = DateTime.Now,
                };
                manager.CreateSession(session);

                return true;
            }
            return false;
        }
    }
}
