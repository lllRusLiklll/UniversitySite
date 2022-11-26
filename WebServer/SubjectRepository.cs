using WebServer.Controllers;

namespace WebServer
{
    public interface ISubjectRepository
    {
        void Insert(Subject subject);
        void Update(Subject subject);
        void Delete(Subject subject);

        List<Subject> Query(ISqlSpecification specification);
    }

    public class SubjectRepository : ISubjectRepository
    {
        private readonly string ConnectionString;
        private readonly string _table = "Subjects";

        public SubjectRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Insert(Subject subject)
        {
            new MyORM(ConnectionString).Insert(subject, _table);
        }

        public void Update(Subject subject)
        {
            new MyORM(ConnectionString).Update(subject, _table);
        }

        public void Delete(Subject subject)
        {
            new MyORM(ConnectionString).Delete(subject);
        }

        public List<Subject> Query(ISqlSpecification specification)
        {
            return new MyORM(ConnectionString).ExecuteQuery<Subject>($"SELECT * FROM {_table} " + specification.ToSqlClauses()).ToList();
        }
    }

    public class SubjectSpecification : ISqlSpecification
    {
        public string ToSqlClauses()
        {
            return "";
        }
    }

    public class SubjectSpecificationById : ISqlSpecification
    {
        private readonly int _id;

        public SubjectSpecificationById(int id)
        {
            this._id = id;
        }

        public string ToSqlClauses()
        {
            return $"WHERE SubjectId={_id}";
        }
    }
}