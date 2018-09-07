using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone
{
    public class NationalParkReservationCLI
    {
        List<string> reservationInfo = new List<string>();
        List<Campground> campgrounds = new List<Campground>();
        //Reservation reservation = new Reservation();
        //Campground campground = new Campground();
        const string _dbConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=dbNationalParkReservation;Integrated Security=true;";

        public void DisplayMainMenu()
        {
            ParkSqlDAL parkDal = new ParkSqlDAL(_dbConnectionString);

            List<Park> parks = parkDal.GetParks();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(" View Parks ");
            Console.WriteLine();
            Console.WriteLine(" Select a Park for further details ");

            bool exit = false;

            while (!exit)
            {
                DisplayParkNames();

                Console.WriteLine(" Q - Quit");
                Console.WriteLine();
                Console.Write(" Select an option... ");

                string command = Console.ReadLine();
                int sel;;
              
                if (command == "q" || command == "Q")
                {
                    Console.WriteLine(" Thank you for visiting the National Parks Reservation System.");
                    Console.WriteLine(" ");
                    Console.Write(" Press any key to exit the application. ");
                    exit = true;
                }
                else if (int.TryParse(command, out sel))
                { 
                    if (sel > 0 && sel <= parks.Count)
                    {
                        DisplaySelectedParkInfo(parks[sel - 1]);
                    }
                }
                else
                {
                    Console.WriteLine(" The command requested was not valid, please try again.");
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayParkNames()
        {
            ParkSqlDAL parkDal = new ParkSqlDAL(_dbConnectionString);

            List<Park> parks = parkDal.GetParks();
           
            Console.WriteLine();

            for (int index = 0; index < parks.Count; index++)
            {
                Console.WriteLine(" " + (index + 1) + ") " + parks[index].ParkName);
            }
        }
        
        private void DisplaySelectedParkInfo(Park park)
        {
            string location = park.Location.ToString();
            string established = park.EstablishDate.ToString("d");
            string area = park.Area.ToString("N");
            string annualVisitors = String.Format("{0:n0}",park.AnnualVisitorCount);

            Console.Clear();

            Console.WriteLine();
            Console.WriteLine(" Park Information");
            Console.WriteLine();
            Console.WriteLine($" {park.ParkName.ToString()}");
            Console.WriteLine("{0,20} {1}", " Location: ", location);
            Console.WriteLine("{0,20} {1}", " Established: ", established);
            Console.WriteLine("{0,20} {1}", " Area: ", area + " sq. miles");
            Console.WriteLine("{0,20} {1}", " Annual Visitors: ", annualVisitors);
            Console.WriteLine();
            Console.WriteLine();;
            Console.WriteLine();
            Console.WriteLine($" {park.Description.ToString()}");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(" Menu Option");
            Console.WriteLine();
            Console.WriteLine(" 1) View Campgrounds");
            Console.WriteLine(" 2) Search for Reservations");
            Console.WriteLine(" 3) Press any key to return to previous screen");
            Console.WriteLine();
            Console.Write(" Select an option... ");

            string input = Console.ReadLine();

            if (input == "1")
            {
                //display campgrounds
                DisplayCampgroundInfo(park);
            }
            else if (input == "2")
            {
                //search for reservations
                //DisplayReservationInfo(park);
            }
        }
        
        private void DisplayCampgroundInfo(Park park)
        {
            CampgroundSqlDAL campgroundDal = new CampgroundSqlDAL(_dbConnectionString);

            List<Campground> campgrounds = campgroundDal.GetCampgrounds(park.ParkId);

            Console.WriteLine();
            Console.WriteLine($" {park.ParkName} Campgrounds");
            Console.WriteLine();
            Console.WriteLine(" {0, 0} {1, 5} {2, 36} {3, 9} {4, 13}", " ",  "Name", "Opens", "Closes", "Daily Fee");
            Console.WriteLine("---------------------------------------------------------------------------");

            for (int index = 0; index < campgrounds.Count; index++)
            {
                Console.WriteLine(" " + (index + 1) + ") " + campgrounds[index].CampgroundName.PadRight(35) +
                                 " " + campgrounds[index].MonthOpenStr.PadRight(8) +
                                 " " + campgrounds[index].MonthCloseStr.PadRight(10) +
                                 " " + campgrounds[index].DailyFee.ToString("c"));
            }

            Console.WriteLine();
            Console.WriteLine(" Menu Options");
            Console.WriteLine();
            Console.WriteLine(" 1) Search for an Available Reservation");
            Console.WriteLine(" 2) Press any key to return to previous screen");
            Console.WriteLine();
            Console.Write(" Select an option... ");

            string input = Console.ReadLine();

            if (input == "1")
            {
                DisplayReservationInfo(park);
            }
            else
            {
                DisplaySelectedParkInfo(park);
            }
        }

        //List<string> reservationInfo = new List<string>();
        //List<Campground> campgrounds = new List<Campground>();
        //Reservation reservation = new Reservation();
        //Campground campground = new Campground();

        private void DisplayReservationInfo(Park park)
        {
            CampgroundSqlDAL campgroundDal = new CampgroundSqlDAL(_dbConnectionString);

            campgrounds = campgroundDal.GetCampgrounds(park.ParkId);

            Console.WriteLine();

            Console.WriteLine(" Search for Campground Reservation");
            Console.WriteLine();
            Console.WriteLine(" {0, 0} {1, 5} {2, 36} {3, 9} {4, 13}", " ", "Name", "Opens", "Closes", "Daily Fee");
            Console.WriteLine("---------------------------------------------------------------------------");
            for (int index = 0; index < campgrounds.Count; index++)
            {
                Console.WriteLine(" " + (index + 1) + ") " + campgrounds[index].CampgroundName.PadRight(35) +
                                 " " + campgrounds[index].MonthOpenStr.PadRight(8) +
                                 " " + campgrounds[index].MonthCloseStr.PadRight(10) +
                                 " " + campgrounds[index].DailyFee.ToString("c"));
            }

            GetUserReservationInput(park);
        }

        private void DisplaySiteInfo(Campground campground)
        {
            SiteSqlDAL siteSqlDal = new SiteSqlDAL(_dbConnectionString);
            List<Site> officiallyTaken = new List<Site>();
            List<Site> sites = siteSqlDal.GetAllSites(campground.CampgroundId);
            List<Site> takenSites = siteSqlDal.GetTakenSites(Convert.ToDateTime(reservationInfo[1]), Convert.ToDateTime(reservationInfo[2]));
            List<Site> matchingSites = new List<Site>();

            foreach(Site site in takenSites)
            {
                if(site.CampgroundId == campground.CampgroundId)
                {
                    officiallyTaken.Add(site);
                }
            }


            foreach(Site site in sites)
            {
                if (officiallyTaken.Contains(site))
                {
                    sites.Remove(site);                   
                }
            }


            foreach (Site site in sites)
            {
                Console.WriteLine($"Did it work? {site.SiteId} + {site.MaxOccupancy} + {site.AccessibilityStr} + {site.MaxRvLength} + {site.UtilitiesStr}");
            }
            Console.ReadKey();
        }

        private void GetUserReservationInput(Park park)
        {
            Console.Write(" Which campground (enter 0 to cancel) ");
            string selectedCampground = Console.ReadLine();
            int selectedCampgroundParsed = int.Parse(selectedCampground);

            if (selectedCampgroundParsed == 0)
            {
                DisplayReservationInfo(park);
            }
            else if (selectedCampgroundParsed > campgrounds.Count)
            {
                Console.WriteLine(" Command not found");
                Console.ReadKey();
                DisplayReservationInfo(park);
            }
            else
            {
                reservationInfo.Add(campgrounds[(selectedCampgroundParsed - 1)].CampgroundId.ToString());
            }

            Console.Write(" What is the arrival date?(mm/dd/yyyy) ");
            string selectedArrivalDate = Console.ReadLine();
            DateTime arrivalDate = Convert.ToDateTime(selectedArrivalDate);
            reservationInfo.Add(selectedArrivalDate);

            Console.Write(" What is the departure date?(mm/dd/yyyy) ");
            string selectedDepartureDate = Console.ReadLine();
            DateTime departureDate = Convert.ToDateTime(selectedArrivalDate);
            reservationInfo.Add(selectedDepartureDate);

            DisplaySiteInfo(campgrounds[(selectedCampgroundParsed - 1)]);
        }

        private void FormatDescription(string description)
        {
            char[] splitChar = { ' ' };
            string[] words = description.Split(splitChar);


           
        }


        
    }
}
