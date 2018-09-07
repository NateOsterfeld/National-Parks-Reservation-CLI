using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ParkSqlDAL
    {
        private string _connectionString;

        //constructor
        public ParkSqlDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        //returns a list of all praks within the reservation system
        public List<Park> GetParks()
        {
            List<Park> output = new List<Park>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select * From park Order By park.name";
                cmd.Connection = connection;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Park park = PopulateParkFromReader(reader);

                    output.Add(park);
                }
            }

            return output;
        }

        public List<Park> GetParkInfo(int park_id)
        {
            List<Park> output = new List<Park>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select * From park Where park_id = @parkID";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@parkID", park_id);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //string park = Convert.ToString(reader["name"]);
                    Park park = PopulateParkFromReader(reader);

                    output.Add(PopulateParkFromReader(reader));
                }
            }

            return output;
        }

        private Park PopulateParkFromReader(SqlDataReader reader)
        {
            Park item = new Park();

            item.ParkId = Convert.ToInt32(reader["park_id"]);
            item.ParkName = Convert.ToString(reader["name"]);
            item.Location = Convert.ToString(reader["location"]);
            item.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
            item.Area = Convert.ToInt32(reader["area"]);
            item.AnnualVisitorCount = Convert.ToInt32(reader["visitors"]);
            item.Description = Convert.ToString(reader["description"]);

            return item;
        }
    }
}
