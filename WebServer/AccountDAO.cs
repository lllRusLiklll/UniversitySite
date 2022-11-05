using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Controllers;

namespace WebServer
{
    public interface AccountDAO
    {
        List<Account> GetAccounts();
        Account GetAccountById(int id);
        void Insert(Account account);
        void Update(Account account);
        void Delete(Account account);
    }

    public class AccountController : AccountDAO
    {
        public string ConnectionString;
        
        public AccountController(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<Account> GetAccounts()
        {
            return new MyORM(ConnectionString).Select<Account>();
        }

        public Account GetAccountById(int id)
        {
            return new MyORM(ConnectionString).Select<Account>(id);
        }

        public void Insert(Account account)
        {
            new MyORM(ConnectionString).Insert(account);
        }

        public void Update(Account account)
        {
            new MyORM(ConnectionString).Update(account);
        }

        public void Delete(Account account)
        {
            new MyORM(ConnectionString).Delete(account);
        }
    }
}
