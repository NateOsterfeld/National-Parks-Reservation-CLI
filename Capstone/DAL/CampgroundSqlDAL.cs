using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
    public class CampgroundSqlDAL
    {
        private string _connectionString;

        //constructor
        public CampgroundSqlDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Campground> GetCampgrounds(int park_id)
        {
            List<Campground> output = new List<Campground>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select campground.campground_id, campground.park_id, campground.name, " +
                                  "campground.daily_fee, campground.open_from_mm, campground.open_to_mm " +
                                  "From campground Join park On campground.park_id = park.park_id " +
                                  "Where park.park_id = @parkID Order By campground.name";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@parkID", park_id);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Campground campground = PopulateCampgroundFromReader(reader);

                    output.Add(campground);
                }
            }

            return output;
        }

        private Campground PopulateCampgroundFromReader(SqlDataReader reader)
        {
            Campground item = new Campground();

            item.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            item.ParkId = Convert.ToInt32(reader["park_id"]);
            item.CampgroundName = Convert.ToString(reader["name"]);
            item.MonthOpen = Convert.ToInt32(reader["open_from_mm"]);
            item.MonthClose = Convert.ToInt32(reader["open_to_mm"]);
            item.DailyFee = Convert.ToInt32(reader["daily_fee"]);

            return item;
        }
    }
}
