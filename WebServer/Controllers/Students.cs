﻿using WebServer.Attributes;
using System.Net;
using System.Text;
using System.Web;

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
        public string Grp { get; set; }
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
        public bool SaveStudent(string firstname, string lastname, string patronymic, string email,
            string password, string repeatPassword, DateTime birthdate, string gender, HttpListenerResponse response)
        {
            var student = _db.Query(new StudentSpecificationByEmail(email));
            if (student.Count == 0)
            {
                _db.Insert(new Student()
                {
                    Firstname = firstname,
                    Lastname = lastname,
                    Patronymic = patronymic,
                    Gender = gender,
                    BirthDate = birthdate,
                    Email = email,
                    Password = password,
                    Grp = "11-106",
                    CreditBook = 123456780
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGET("login")]
        public bool Login(string email, string password, bool isRemember, HttpListenerRequest request, HttpListenerResponse response)
        {
            var student = _db.Query(new StudentSpecificationByEmailPassword(email, password));
            if (student.Count == 1)
            {
                var sessionId = Guid.NewGuid();
                if (isRemember)
                    response.Headers.Set("Set-Cookie", $"SessionId={sessionId}; Path=/; Max-Age=604800") ;
                else
                    response.Headers.Set("Set-Cookie", $"SessionId={sessionId}; Path=/;");

                var manager = SessionManager.GetInstance();
                var session = new Session()
                {
                    Id = sessionId,
                    AccountId = student.First().StudentId,
                    Email = student.First().Email,
                    CreatedDateTime = DateTime.Now,
                };
                manager.CreateSession(session, isRemember ? 604800 : 300);
                return true;
            }

            return false;
        }
    }
}
