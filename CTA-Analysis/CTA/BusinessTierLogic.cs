//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{

    //
    // Business:
    //
    public class Business
    {
        //
        // Fields:
        //
        private string _DBFile;
        private DataAccessTier.Data dataTier;


        ///
        /// <summary>
        /// Constructs a new instance of the business tier.  The format
        /// of the filename should be either |DataDirectory|\filename.mdf,
        /// or a complete Windows pathname.
        /// </summary>
        /// <param name="DatabaseFilename">Name of database file</param>
        /// 
        public Business(string DatabaseFilename)
        {
            _DBFile = DatabaseFilename;

            dataTier = new DataAccessTier.Data(DatabaseFilename);
        }


        ///
        /// <summary>
        ///  Opens and closes a connection to the database, e.g. to
        ///  startup the server and make sure all is well.
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        /// 
        public bool TestConnection()
        {
            return dataTier.OpenCloseConnection();
        }


        ///
        /// <summary>
        /// Returns all the CTA Stations, ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStation objects</returns>
        /// 
        public IReadOnlyList<CTAStation> GetStations()
        {
            List<CTAStation> stations = new List<CTAStation>(); 
            DataSet result = new DataSet() ;

            try
            {

                //
                // TODO!
                // 
                dataTier = new DataAccessTier.Data(_DBFile);

                string sql = "SELECT Name, StationID FROM Stations ORDER BY Name ASC;";
                result = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in result.Tables["TABLE"].Rows)
                {
                    string name = string.Format("{0}", Convert.ToString(row["Name"]));
                    int id = Convert.ToInt32(row["StationID"]);
                    var s = new BusinessTier.CTAStation(id, name);
                    stations.Add(s);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return stations;
        }



        ///
        /// <summary>
        /// Returns the CTA Stops associated with a given station,
        /// ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStop objects</returns>
        ///
        public IReadOnlyList<CTAStop> GetStops(int stationID)
        {
            List<CTAStop> stops = new List<CTAStop>();
            DataSet result;

            try
            {

                //
                // TODO!
                //
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

                string sql = string.Format(@"SELECT * FROM Stops  
WHERE Stops.StationID = '{0}'; ", stationID);
                result = dataTier.ExecuteNonScalarQuery(sql);
                foreach(DataRow row in result.Tables["TABLE"].Rows)
                {
                    int Stationid = Convert.ToInt32(row["StationID"]);
                    int Stopid = Convert.ToInt32(row["StopID"]);
                    string name = string.Format("{0}", Convert.ToString(row["Name"]));
                    string direction = string.Format("{0}", Convert.ToString(row["Direction"]));
                    bool ada =  Convert.ToBoolean(row["ADA"]);
                    double lat = Convert.ToDouble(row["Latitude"]);
                    double lon = Convert.ToDouble(row["Longitude"]);
                    var s = new BusinessTier.CTAStop(Stopid, name, Stationid, direction, ada, lat, lon);
                    stops.Add(s);
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stops;
        }



        ///
        /// <summary>
        /// Returns the top N CTA Stations by ridership, 
        /// ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStation objects</returns>
        /// 
        public IReadOnlyList<CTAStation> GetTopStations(int N)
        {
            if (N < 1)
                throw new ArgumentException("GetTopStations: N must be positive");

            List<CTAStation> stations = new List<CTAStation>();
            DataSet result;

            try
            {

                //
                // TODO!
                //
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT TOP {0} Name, Sum(DailyTotal) As TotalRiders 
FROM Riderships
INNER JOIN Stations ON Riderships.StationID = Stations.StationID 
GROUP BY Stations.StationID, Name
ORDER BY TotalRiders DESC;
", N);
                result = dataTier.ExecuteNonScalarQuery(sql);
                foreach(DataRow row in result.Tables["TABLE"].Rows)
                {
                    string name = string.Format("{0}", Convert.ToString(row["Name"]));
                    var s = new BusinessTier.CTAStation(123, name);
                    stations.Add(s);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }

        public IReadOnlyList<Riderships> GetRiderships()
        {
            List<Riderships> rider = new List<Riderships>();
            DataSet result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"SELECT * FROM Riderships; ");
                result = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in result.Tables["TABLE"].Rows)
                {
                    int Stationid = Convert.ToInt32(row["StationID"]);
                    int Riderid = Convert.ToInt32(row["RiderID"]);
                    string date = string.Format("{0}", Convert.ToString(row["TheDate"]));
                    string day = string.Format("{0}", Convert.ToString(row["TypeOfDay"]));
                    int total = Convert.ToInt32(row["DailyTotal"]);
                    var r = new BusinessTier.Riderships(Riderid, Stationid, date, day, total);
                    rider.Add(r);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetRiderships: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return rider;    
        }

        public IReadOnlyList<Lines> GetLines(int id)
        {
            List<Lines> lines = new List<Lines>();
            DataSet result = new DataSet();

            try
            {

                //
                // TODO!
                // 
                dataTier = new DataAccessTier.Data(_DBFile);

                string sql = string.Format(@"SELECT Color, LineID FROM Lines WHERE LineID = '{0}' ORDER BY Color ASC;", id);
                result = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in result.Tables["TABLE"].Rows)
                {
                    string color = string.Format("{0}", Convert.ToString(row["Color"]));
                    int lineid = Convert.ToInt32(row["LineID"]);
                    var l = new BusinessTier.Lines(lineid, color);
                    lines.Add(l);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetLines: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return lines;
        }

        public long TotalRidership()
        {
            long total = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
                SELECT Sum(Convert(bigint,DailyTotal)) As TotalOverall
                FROM Riderships;");
                result = dataTier.ExecuteScalarQuery(sql);
                total = Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.TotalRidership: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return total;
        }

        public int StationTotal(string name)
        {
            int total = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Sum(DailyTotal) As TotalRiders
FROM Riderships
INNER JOIN Stations ON Riderships.StationID = Stations.StationID
WHERE Name = '{0}';
", name);
                result = dataTier.ExecuteScalarQuery(sql);
                total = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.StationTotal: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return total;
        }

        public int StationAverage(string name)
        {
            int average = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Avg(DailyTotal) As AvgRiders
FROM Riderships
INNER JOIN Stations ON Riderships.StationID = Stations.StationID
WHERE Name = '{0}';
", name);
                result = dataTier.ExecuteScalarQuery(sql);
                average = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.StationAverage: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return average;
        }

        public int DailyTotal(string name)
        {
            int total = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Sum(DailyTotal) As TotalRiders
FROM Riderships
INNER JOIN Stations ON Riderships.StationID = Stations.StationID
WHERE Name = '{0}';
", name);
                result = dataTier.ExecuteScalarQuery(sql);
                total = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.DailyTotal: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return total;
        }

        public int GetStationID(string name)
        {
            int id = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT StationID FROM Stations
WHERE Name = '{0}';
", name);
                result = dataTier.ExecuteScalarQuery(sql);
                id = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStationID: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return id;
        }

        public int WeekDayTotal(int id)
        {
            int total = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Sum(DailyTotal) FROM Riderships
 WHERE Riderships.StationID = '{0}' AND
       TypeOfDay = 'W';", id);
                result = dataTier.ExecuteScalarQuery(sql);
                total = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.WeekDayTotal: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return total;
        }

        public int SaturdayTotal(int id)
        {
            int total = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Sum(DailyTotal) FROM Riderships
 WHERE Riderships.StationID = '{0}'AND
       TypeOfDay = 'A';", id);
                result = dataTier.ExecuteScalarQuery(sql);
                total = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.SaturdayTotal: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return total;
        }

        public int HolidayTotal(int id)
        {
            int total = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Sum(DailyTotal) FROM Riderships
 WHERE Riderships.StationID = '{0}' AND
       TypeOfDay = 'U';", id);
                result = dataTier.ExecuteScalarQuery(sql);
                total = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.HolidayTotal: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return total;
        }

        public int GetStopID(string name)
        {
            int id = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT StopID FROM Stops
WHERE Name = '{0}';
", name);
                result = dataTier.ExecuteScalarQuery(sql);
                id = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStationID: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return id;
        }
        public int GetLineID(int id)
        {
            int lineid = 0;
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT LineID FROM StopDetails
WHERE StopID = '{0}';
", id);
                result = dataTier.ExecuteScalarQuery(sql);
                lineid = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetLineID: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return lineid;
        }

        public string GetAccessible(int id)
        {
            bool acc;
            string ada = "";
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT ADA FROM Stops
WHERE StopID = '{0}';
", id);
                result = dataTier.ExecuteScalarQuery(sql);
                acc = Convert.ToBoolean(result);
                if (acc)
                {
                    ada = "Yes";
                }
                else
                    ada = "No";
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetAccessible: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return ada;
        }

        public string GetDirection(int id)
        {
            string d = "";
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Direction FROM Stops
WHERE StopID = '{0}';
", id);
                result = dataTier.ExecuteScalarQuery(sql);
                d = Convert.ToString(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetDirection: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return d;
        }

        public string GetLat(int id)
        {
            string L = "";
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Latitude FROM Stops
WHERE StopID = '{0}';
", id);
                result = dataTier.ExecuteScalarQuery(sql);
                L = Convert.ToString(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetLat: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return L;
        }

        public string GetLon(int id)
        {
            string L = "";
            object result;
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
SELECT Longitude FROM Stops
WHERE StopID = '{0}';
", id);
                result = dataTier.ExecuteScalarQuery(sql);
                L = Convert.ToString(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetLon: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return L;
        }

    }//class
}//namespace
