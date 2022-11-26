using WebServer.Controllers;

namespace WebServer
{
    public interface IFacultyRepository
    {
        void Insert(Faculty faculty);
        void Update(Faculty faculty);
        void Delete(Faculty faculty);

        List<Faculty> Query(ISqlSpecification specification);
    }

    public class FacultyRepository : IFacultyRepository
    {
        private readonly string ConnectionString;
        private readonly string _table = "Faculties";

        public FacultyRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Insert(Faculty faculty)
        {
            new MyORM(ConnectionString).Insert(faculty, _table);
        }

        public void Update(Faculty faculty)
        {
            new MyORM(ConnectionString).Update(faculty, _table);
        }

        public void Delete(Faculty faculty)
        {
            new MyORM(ConnectionString).Delete(faculty);
        }

        public List<Faculty> Query(ISqlSpecification specification)
        {
            return new MyORM(ConnectionString).ExecuteQuery<Faculty>($"SELECT * FROM {_table} " + specification.ToSqlClauses()).ToList();
        }
    }

    public class FacultySpecification : ISqlSpecification
    {
        public string ToSqlClauses()
        {
            return "";
        }
    }

    public class FacultySpecificationById : ISqlSpecification
    {
        private readonly int _id;

        public FacultySpecificationById(int id)
        {
            this._id = id;
        }

        public string ToSqlClauses()
        {
            return $"WHERE FacultyId={_id}";
        }
    }
}