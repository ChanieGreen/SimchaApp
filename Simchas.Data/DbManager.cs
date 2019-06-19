using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simchas.Data
{
    public class DbManager
    {
        private string _connectionString;

        public DbManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Simcha> GetSimchas()
        {
            List<Simcha> simchas = new List<Simcha>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT s.Id, s.SimchaName, s.SimchaDate, COUNT(sc.ContributorId) AS ContributorCount, SUM(sc.AmountContributing) AS Total FROM Simchas s
                                LEFT JOIN SimchaContributors sc
                                ON s.Id = sc.SimchaId
                                GROUP BY s.Id, s.SimchaName, s.SimchaDate";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Simcha simcha = new Simcha
                    {
                        Id = (int)reader["Id"],
                        SimchaName = (string)reader["SimchaName"],
                        Date = (DateTime)reader["SimchaDate"]
                    };
                    if (reader["ContributorCount"] != DBNull.Value && reader["Total"] != DBNull.Value)
                    {
                        simcha.Contributors = (int)reader["ContributorCount"];
                        simcha.Total = (decimal)reader["Total"];
                    }

                    simchas.Add(simcha);
                }
            }
            return simchas;
        }

        public int GetContributorCount()
        {
            int count;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }

        public void AddSimcha(Simcha simcha)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Simchas (SimchaName, SimchaDate) VALUES (@simchaName, @simchaDate)";
                cmd.Parameters.AddWithValue("@simchaName", simcha.SimchaName);
                cmd.Parameters.AddWithValue("@simchaDate", simcha.Date);
                conn.Open();
                cmd.ExecuteNonQuery();
            };
        }

        public IEnumerable<Contributor> GetContributors()
        {
            List<Contributor> contributors = new List<Contributor>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT *, 
                                    (
	                                    (SELECT ISNULL(SUM(d.Deposit), 0) FROM Deposits d WHERE d.ContributorId = c.Id)
	                                    - 
	                                    (SELECT ISNULL(SUM(sc.AmountContributing), 0) FROM SimchaContributors sc WHERE sc.ContributorId = c.Id)
                                    ) as 'Balance' FROM Contributors c";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Contributor contributor = new Contributor
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"],
                        Balance = (decimal)reader["Balance"]
                    };
                    contributors.Add(contributor);
                }
            }
            return contributors;
        }

        public void AddContributor(Contributor c)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Contributors (FirstName, LastName, PhoneNumber, AlwaysInclude) VALUES (@firstName, @lastName, @phoneNumber, @alwaysInclude) SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@firstName", c.FirstName);
                cmd.Parameters.AddWithValue("@lastName", c.LastName);
                cmd.Parameters.AddWithValue("@phoneNumber", c.PhoneNumber);
                cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
                conn.Open();
                c.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void AddDeposit(Deposit d)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = " INSERT INTO Deposits (ContributorId, Deposit, Date) VALUES (@contributorId, @deposit, @date)";
                cmd.Parameters.AddWithValue("@contributorId", d.ContributorId);
                cmd.Parameters.AddWithValue("@deposit", d.Amount);
                cmd.Parameters.AddWithValue("@date", d.Date);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateContributor(Contributor c)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNumber, AlwaysInclude = @alwaysInclude WHERE Id = @id";
                cmd.Parameters.AddWithValue("@firstName", c.FirstName);
                cmd.Parameters.AddWithValue("@lastName", c.LastName);
                cmd.Parameters.AddWithValue("@phoneNumber", c.PhoneNumber);
                cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
                cmd.Parameters.AddWithValue("@id", c.Id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<Deposit> GetDepositsById(int id)
        {
            List<Deposit> deposits = new List<Deposit>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Deposits WHERE ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    deposits.Add(new Deposit
                    {
                        Id = (int)reader["Id"],
                        ContributorId = (int)reader["ContributorId"],
                        Amount = (decimal)reader["Deposit"],
                        Date = (DateTime)reader["Date"]
                    });
                }
            }
            return deposits;
        }

        public IEnumerable<Contribution> GetContributionsById(int id)
        {
            List<Contribution> contributions = new List<Contribution>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM SimchaContributors sc
                                    JOIN Simchas s
                                    ON sc.SimchaId = s.Id
                                    WHERE sc.ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    contributions.Add(new Contribution
                    {
                        Amount = (decimal)reader["AmountContributing"],
                        Date = (DateTime)reader["Date"],
                        SimchaId = (int)reader["SimchaId"],
                        SimchaName = (string)reader["SimchaName"]
                    });
                }
            }
            return contributions;
        }

        public Simcha GetSimcha(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT *, (
                        SELECT ISNull(SUM(AmountContributing), 0)
                                    FROM SimchaContributors
                                    WHERE SimchaId = s.Id
                    ) as 'Total', (
                    SELECT COUNT(*)
                                    FROM SimchaContributors
                                    WHERE SimchaId = s.Id
                    ) as 'ContributorAmount' FROM Simchas s
                    WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                var simcha = new Simcha();
                simcha.Id = (int)reader["Id"];
                simcha.Date = (DateTime)reader["SimchaDate"];
                simcha.SimchaName = (string)reader["SimchaName"];
                simcha.Contributors = (int)reader["ContributorAmount"];
                simcha.Total = (decimal)reader["Total"];
                return simcha;
            }
        }

        public IEnumerable<SimchaContributor> GetContributionsForSimcha(int simchaId)
        {
            var contributors = GetContributors();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM SimchaContributors WHERE SimchaId = @id";
                cmd.Parameters.AddWithValue("@id", simchaId);
                List<Contribution> contributions = new List<Contribution>();
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    contributions.Add(new Contribution
                    {
                        ContributorId = (int)reader["ContributorId"],
                        SimchaId = simchaId,
                        Amount = (decimal)reader["AmountContributing"],
                        Date = (DateTime)reader["Date"]
                    });
                }

                return contributors.Select(contributor =>
                {
                    var sc = new SimchaContributor();
                    sc.ContributorId = contributor.Id;
                    sc.FirstName = contributor.FirstName;
                    sc.LastName = contributor.LastName;
                    sc.AlwaysInclude = contributor.AlwaysInclude;
                    sc.Balance = contributor.Balance;
                    Contribution contribution = contributions.FirstOrDefault(c => c.ContributorId == contributor.Id);
                    if (contribution != null)
                    {
                        sc.Amount = contribution.Amount;
                    }
                    return sc;
                });
            }
        }

        public void UpdateSimchaContributions(IEnumerable<IncludeInContribution> contributors, int simchaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM SimchaContributors WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);

                connection.Open();
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO SimchaContributors (SimchaId, ContributorId, AmountContributing, Date)
                                    VALUES (@simchaId, @contributorId, @amount, @date)";
                foreach (IncludeInContribution contributor in contributors.Where(c => c.Include))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.Parameters.AddWithValue("@contributorId", contributor.ContributorId);
                    cmd.Parameters.AddWithValue("@amount", contributor.Amount);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }

            }
        }
    }
}
