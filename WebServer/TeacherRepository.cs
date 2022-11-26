using WebServer.Controllers;

namespace WebServer
{
    public interface ITeacherRepository
    {
        void Insert(Teacher teacher);
        void Update(Teacher teacher);
        void Delete(Teacher teacher);

        List<Teacher> Query(ISqlSpecification specification);
    }

    public class TeacherRepository : ITeacherRepository
    {
        private readonly string ConnectionString;
        private readonly string _table = "Teachers";

        public TeacherRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Insert(Teacher teacher)
        {
            new MyORM(ConnectionString).Insert(teacher, _table);
        }

        public void Update(Teacher teacher)
        {
            new MyORM(ConnectionString).Update(teacher, _table);
        }

        public void Delete(Teacher teacher)
        {
            new MyORM(ConnectionString).Delete(teacher);
        }

        public List<Teacher> Query(ISqlSpecification specification)
        {
            return new MyORM(ConnectionString).ExecuteQuery<Teacher>($"SELECT * FROM {_table} " + specification.ToSqlClauses()).ToList();
        }
    }

    public class TeacherSpecification : ISqlSpecification
    {
        public string ToSqlClauses()
        {
            return "";
        }
    }

    public class TeacherSpecificationById : ISqlSpecification
    {
        private readonly int _id;

        public TeacherSpecificationById(int id)
        {
            this._id = id;
        }

        public string ToSqlClauses()
        {
            return $"WHERE TeacherId={_id}";
        }
    }
}