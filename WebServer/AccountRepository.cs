using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Controllers;

namespace WebServer
{
    public interface IAccountRepository
    {
        void InsertAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(Account account);

        List<Account> Query(IAccountSpecification specification);
    }

    public interface IAccountSpecification
    {
        bool Specified(Account account);
    }

    public interface ISqlSpecification
    {
        string ToSqlClauses();
    }
}
