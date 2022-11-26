using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controllers
{
    public class Subjects
    {
        private readonly ISubjectRepository _db =
            new SubjectRepository(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UniversityDB;Integrated Security=True;");

        public Subject GetSubjectById(int id)
        {
            return _db.Query(new SubjectSpecificationById(id)).FirstOrDefault();
        }
    }
}
