using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
   public class ReservationSqlDAL
    {
        private string _connectionString;

        //constructor
        public ReservationSqlDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Reservation> GetAllReservations(int parkID, int campgroundID, int siteID)
        {
            List<Reservation> output = new List<Reservation>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select * From reservation " +
                                  "Join site On reservation.site_id = site.site_id " +
                                  "Join campground On site.campground_id = campground.campground_id " +
                                  "Join park On campground.park_id = park.park_id " +
                                  "Where park.park_id = @parkID And campground.campground_id = @campgroundID " +
                                  "And site.site_id = @siteID";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@parkID", parkID);
                cmd.Parameters.AddWithValue("@campgroundID", campgroundID);
                cmd.Parameters.AddWithValue("@siteID", siteID);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Reservation reservation = PopulateReservationFromReader(reader);

                    output.Add(reservation);
                }
            }

            return output;
        }

        //public List<Reservation> SearchAvailableSites(int parkID, int campgroundID)
        //{
        //    List<Reservation> output = new List<Reservation>();

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandText = "Select site.site_number, site.max_occupancy, site.accessible, " +
        //                          "site.max_rv_length, site.utilities, campground.daily_fee From site " +
        //                          "Left Join reservation On site.site_id = reservation.site_id " +
        //                          "Join campground On site.campground_id = campground.campground_id " +
        //                          "Join park On campground.park_id = park.park_id " +
        //                          "Where park.park_id = @parkID And campground.campground_id = @campgroundID " +
        //                          "And reservation.reservation_id Is Null";
        //        cmd.Connection = connection;
        //        cmd.Parameters.AddWithValue("@parkID", parkID);
        //        cmd.Parameters.AddWithValue("@campgroundID", campgroundID);

        //        SqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            Reservation reservation = PopulateReservationFromReader(reader);

        //            output.Add(reservation);
        //        }
        //    }

        //    return output;
        //}

        public List<Reservation> CreateReservation(string userName, DateTime userStartDate, DateTime userEndDate)
        {
            List<Reservation> output = new List<Reservation>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Insert Into reservation (name, from_date, to_date) " +
                                  "Values (@userName, @userStartDate, @userEndDate)";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@userStartDate", userStartDate);
                cmd.Parameters.AddWithValue("@userEndDate", userEndDate);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Reservation reservation = PopulateReservationFromReader(reader);

                    output.Add(reservation);
                }
            }

            return output;
        }

        public bool DeleteReservation(int reservationID)
        {
            bool output = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Delete From reservation where reservation.reservation_id = @reservationID";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@reservationID", reservationID);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    output = true;
                }
                else
                {
                    Console.WriteLine("There was an issue deleting the requested reservation");
                }
            }

            return output;
        }

        private Reservation PopulateReservationFromReader(SqlDataReader reader)
        {
            Reservation item = new Reservation();

            item.ReservationId = Convert.ToInt32(reader["reservation_id"]);
            item.SiteId = Convert.ToInt32(reader["site_id"]);
            item.ReservationName = Convert.ToString(reader["name"]);
            item.StartReservationDate = Convert.ToDateTime(reader["from_date"]);
            item.EndReservationDate = Convert.ToDateTime(reader["to_date"]);
            item.CreateReservationDate = Convert.ToDateTime(reader["create_date"]);

            return item;
        }
    }
}
