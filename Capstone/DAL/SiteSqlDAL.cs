using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
    public class SiteSqlDAL
    {
        private string _connectionString;

        //constructor
        public SiteSqlDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Site> GetTakenSites(DateTime arrivalDate, DateTime endDate)
        {
            List<Site> output = new List<Site>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select * From site " +
                                  "Join reservation On reservation.site_id = site.site_id " +
                                  "Join campground On campground.campground_id = site.campground_id " +
                                  "Join park On park.park_id = campground.park_id " +
                                  "Where (reservation.to_date > @arrivalDate and reservation.to_date < @endDate) or " +
                                  "(reservation.from_date > @arrivalDate and reservation.from_date < @endDate)";

                //"Select * From site " +
                //                  "Join reservation On reservation.site_id = site.site_id " +
                //                  "Join campground on campground.campground_id = site.campground_id " +
                //                  "Join park on park.park_id = campground.park_id " +
                //                  "Where reservation.from_date < @arrivalDate and reservation.to_date > @endDate";

                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@arrivalDate", arrivalDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = PopulateSiteFromReader(reader);

                    output.Add(site);
                }
            }

            return output;
        }

        public List<Site> GetAllSites(int campgroundID)
        {
            List<Site> output = new List<Site>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select * from site " +
                                  "join campground on campground.campground_id = site.campground_id " +
                                  "where campground.campground_id = @campgroundID";

                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@campgroundID", campgroundID);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = PopulateSiteFromReader(reader);

                    output.Add(site);
                }
            }

            return output;
        }

        public List<Site> SearchAvailableSites(int parkID, int campgroundID)
        {
            List<Site> output = new List<Site>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select site.site_number, site.max_occupancy, site.accessible, " +
                                  "site.max_rv_length, site.utilities, campground.daily_fee From site " +
                                  "Left Join reservation On site.site_id = reservation.site_id " +
                                  "Join campground On site.campground_id = campground.campground_id " +
                                  "Join park On campground.park_id = park.park_id " +
                                  "Where park.park_id = @parkID And campground.campground_id = @campgroundID " +
                                  "And reservation.reservation_id Is Null";

                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@parkID", parkID);
                cmd.Parameters.AddWithValue("@campgroundID", campgroundID);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = PopulateSiteFromReader(reader);

                    output.Add(site);
                }
            }

            return output;
        }

        private Site PopulateSiteFromReader(SqlDataReader reader)
        {
            Site item = new Site();

            item.SiteId = Convert.ToInt32(reader["site_id"]);
            item.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            item.SiteNumber = Convert.ToInt32(reader["site_number"]);
            item.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            item.Accessibility = Convert.ToBoolean(reader["accessible"]);
            item.MaxRvLength = Convert.ToInt32(reader["max_rv_length"]);
            item.Utilities = Convert.ToBoolean(reader["utilities"]);

            return item;
        }
    }
}
