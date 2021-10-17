using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace _3_10_HW.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cell { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Balance { get; set; }
    }

    public class Fund
    {
        public int ContributerId { get; set; }
        public int SimchaId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string SimchaName { get; set; }
    }

    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int ContributerCount { get; set; }
        public DateTime Date { get; set; }

    }
    public class SimchosDb
    {
        private readonly string _connectionString;
        public SimchosDb(string connectionstring)
        {
            _connectionString = connectionstring;
        }
        public List<Person> GetPeople()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select c.id, c.Name,  c.Cell, c.AlwaysInclude, SUM(f.amount) as amount from Contributers C" +
                    " LEFT JOIN funds f ON C.id= F.ContributerId  group by c.id, c.Name,  c.Cell, c.AlwaysInclude");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Person> people = new List<Person>();
                while (reader.Read())
                {
                    people.Add(new Person
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["Name"],
                        Cell = (string)reader["Cell"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"],
                        Balance = (decimal)reader["Amount"]

                    });
                }
                return people;
            }
        }

        public void AddPerson(Person p)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("insert into contributers values( @Name, @Cell, @AlwaysInclude) SELECT SCOPE_IDENTITY() as id");
                command.Parameters.AddWithValue("@Name", p.Name);
                command.Parameters.AddWithValue("@Cell", p.Cell);
                command.Parameters.AddWithValue("@AlwaysInclude", p.AlwaysInclude);
                connection.Open();
                p.Id = (int)(decimal)command.ExecuteScalar();
            }
        }
        public void EditPerson(Person person)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("update contributers set values( @Name, @Cell, @AlwaysInclude) where id =@id");
                command.Parameters.AddWithValue("@Name", person.Name);
                command.Parameters.AddWithValue("@Cell", person.Cell);
                command.Parameters.AddWithValue("@AlwaysInclude", person.AlwaysInclude);
                command.Parameters.AddWithValue("@id", person.Id);
                connection.Open();
                command.ExecuteNonQuery();

            }
        }
        public void AddFund(Fund fund)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("insert into funds values( @SimchaId, @ContributerId, @Amount, @date)");
                command.Parameters.AddWithValue("@SimchaId", fund.SimchaId);
                command.Parameters.AddWithValue("@ContributerId", fund.ContributerId);
                command.Parameters.AddWithValue("@Amount", fund.Amount);
                command.Parameters.AddWithValue("@date", fund.Date);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void UpdateContributions(List<Fund> funds)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("delete from funds where SimchaId= @simchaId");
                command.Parameters.AddWithValue("@SimchaId", funds[0].SimchaId);
                connection.Open();
                command.ExecuteNonQuery();
            }
            foreach (Fund fund in funds)
            {
                if(fund.Amount!=0)
                AddFund(new Fund
                {
                    ContributerId = fund.ContributerId,
                    Amount = fund.Amount * -1,
                    Date = fund.Date,
                    SimchaId = fund.SimchaId
                }
                    );
            }
        }
        public decimal GetTotalFunds()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select sum(amount) as amount from funds");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return (decimal)reader["amount"];
            }
        }
        public decimal GetTotalFundsPerPerson(int contributerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select sum(amount) as amount from funds where ContributerId =@contributerId");
                command.Parameters.AddWithValue("@ContributerId", contributerId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return (decimal)reader["amount"];
            }
        }
        public List<Fund> GetFundsPerPerson(int contributerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select * from funds f left join simchos s on f.SimchaId= s.id where ContributerId = @contributerId");
                command.Parameters.AddWithValue("@ContributerId", contributerId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var funds = new List<Fund>();
                while (reader.Read())
                {
                    funds.Add(new Fund
                    {
                        Amount = (decimal)reader["amount"],
                        Date = (DateTime)reader["Date"],
                        SimchaId = (int)reader["SimchaId"],
                        ContributerId = contributerId,
                        SimchaName = (string)reader["name"]
                    });
                }
                return funds;
            }
        }
        public List<Fund> GetFundsPerSimcha(int simchaId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select * from funds where simchaId = @simchaId");
                command.Parameters.AddWithValue("@simchaId", simchaId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var funds = new List<Fund>();
                while (reader.Read())
                {
                    funds.Add(new Fund
                    {
                        Amount = (decimal)reader["amount"],
                        Date = (DateTime)reader["Date"],
                        SimchaId = simchaId,
                        ContributerId = (int)reader["ContributerId"]
                    });
                }
                return funds;
            }


        }
        public decimal GetTotalFundsForSimcha(int simchaId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select sum(amount) as amount from funds where simchaId =@simchaId");
                command.Parameters.AddWithValue("@simchaId", simchaId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return (decimal)reader["amount"];
            }
        }

        public void AddSimcha(Simcha simcha)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("insert into simchos values( @Name, @Date)");
                command.Parameters.AddWithValue("@Name", simcha.Name);
                command.Parameters.AddWithValue("@Date", simcha.Date);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public List<Simcha> GetSimchas()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select  S.id, S.Name, S.date, ISNULL(sum(f.Amount), 0) as amount, ISNULL(count(f.amount), 0) as contributerCount " +
                    "from Simchos S left join Funds F  on S.Id = f.SimchaId where s.id  > 1  group by s.id, S.Name, s.date ");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Simcha> simchas = new List<Simcha>();
                while (reader.Read())
                {
                    simchas.Add(new Simcha
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["Name"],
                        Amount = (decimal)reader["Amount"],
                        ContributerCount = (int)reader["ContributerCount"]
                    });
                }
                return simchas;
            }


        }
        public Simcha GetSimchaById(int simchaid)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("select  S.id, S.Name, S.Date, ISNULL(sum(f.Amount), 0) as amount, ISNULL(count(f.amount), 0) as contributerCount " +
                    "from Simchos S left join Funds F  on S.Id = f.SimchaId where s.id= @id group by s.id, S.Name, S.Date ");
                command.Parameters.AddWithValue("@id", simchaid);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Simcha
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["Name"],
                    Amount = (decimal)reader["Amount"],
                    ContributerCount = (int)reader["ContributerCount"]
                };

            }


        }
        public int GetTotalContributers()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = ("Select Count(c.id) as count from Contributers c");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                reader.Read();

                return (int)reader["count"];
            }

        }
    }
    public class SimchaViewModel
    {

        public List<Simcha> Simchas { get; set; }
        public Simcha Simcha { get; set; }
        public int Contributers { get; set; }
        public List<Person> People { get; set; }
        public List<Fund> Funds { get; set; }
        public List<decimal> Amounts { get; set; }
        public decimal Total { get; set; }
    }

}
