using WebServer.Controllers;

namespace WebServer
{
    public interface IStudentRepository
    {
        void Insert(Student student);
        void Update(Student student);
        void Delete(Student student);

        List<Student> Query(ISqlSpecification specification);
    }

    public interface ISqlSpecification
    {
        string ToSqlClauses();
    }

    public class StudentRepository : IStudentRepository
    {
        private readonly MyORM _orm;
        private readonly string _table = "Students";

        public StudentRepository(string connectionString)
        {
            _orm = new MyORM(connectionString);
        }

        public void Insert(Student student)
        {
            _orm.Insert(student, _table);
        }

        public void Update(Student student)
        {
            _orm.Update(student);
        }

        public void Delete(Student student)
        {
            _orm.Delete(student);
        }

        public List<Student> Query(ISqlSpecification specification)
        {
            return _orm.ExecuteQuery<Student>("SELECT * FROM Students " + specification.ToSqlClauses()).ToList();
        }
    }

    public class StudentSpecification : ISqlSpecification
    {
        public string ToSqlClauses()
        {
            return "";
        }
    }

    public class StudentSpecificationById : ISqlSpecification
    {
        private readonly int _id;

        public StudentSpecificationById(int id)
        {
            this._id = id;
        }

        public string ToSqlClauses()
        {
            return $"WHERE Id={_id}";
        }
    }

    public class StudentSpecificationByEmailPassword : ISqlSpecification
    {
        private readonly string _email;
        private readonly string _password;

        public StudentSpecificationByEmailPassword(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public string ToSqlClauses()
        {
            return $"WHERE Email='{_email}' AND Password='{_password}'";
        }
    }
}
