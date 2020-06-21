using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CTA
{

    public partial class Form1 : Form
    {
        private string BuildConnectionString()
        {
            string version = "MSSQLLocalDB";
            string filename = this.txtDatabaseFilename.Text;

            string connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;", version, filename);

            return connectionInfo;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //
            // setup GUI:
            //
            this.lstStations.Items.Add("");
            this.lstStations.Items.Add("[ Use File>>Load to display L stations... ]");
            this.lstStations.Items.Add("");

            this.lstStations.ClearSelected();

            toolStripStatusLabel1.Text = string.Format("Number of stations:  0");

            // 
            // open-close connect to get SQL Server started:
            //
            SqlConnection db = null;

            try
            {
                db = new SqlConnection(BuildConnectionString());
                db.Open();
            }
            catch
            {
                //
                // ignore any exception that occurs, goal is just to startup
                //
            }
            finally
            {
                // close connection:
                if (db != null && db.State == ConnectionState.Open)
                    db.Close();
            }
        }


        //
        // File>>Exit:
        //
        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //
        // File>>Load Stations:
        //
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
            ClearStationUI(true /*clear stations*/);

            try
            {

                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                var stations = bizTier.GetStations();

                foreach(BusinessTier.CTAStation station in stations)
                {
                    this.lstStations.Items.Add(station.Name);
                }

                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", stations.Count);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                //if (db != null && db.State == ConnectionState.Open)
                //  db.Close();
            }
        }


        //
        // User has clicked on a station for more info:
        //
        private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
                return;

            //
            // clear GUI in case this fails:
            //
            ClearStationUI();

            //
            // now display info about selected station:
            //
            string stationName = this.lstStations.Text;
            stationName = stationName.Replace("'", "''");

            //SqlConnection db = null;

            try
            {
                BusinessTier.Business bTier;
                bTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                var totalOverall = bTier.TotalRidership();
                var stationTotal = bTier.StationTotal(stationName);
                var stationAvg = bTier.StationAverage(stationName);

                double percentage = ((double)stationTotal) / totalOverall * 100.0;

                this.txtTotalRidership.Text = stationTotal.ToString("#,##0");
                this.txtAvgDailyRidership.Text = string.Format("{0:#,##0}/day", stationAvg);
                this.txtPercentRidership.Text = string.Format("{0:0.00}%", percentage);

                //
                // now ridership values for Weekday, Saturday, and
                // sunday/holiday:
                //

                int stationID = bTier.GetStationID(stationName);
                this.txtStationID.Text = stationID.ToString();

                int total = bTier.WeekDayTotal(stationID);
                this.txtWeekdayRidership.Text = total.ToString("#,##0");

                total = bTier.SaturdayTotal(stationID);
                this.txtSaturdayRidership.Text = total.ToString("#,##0");

                total = bTier.HolidayTotal(stationID);
                this.txtSundayHolidayRidership.Text = total.ToString("#,##0");

                //
                // finally, what stops do we have at this station?
                //

                // display stops:
                var stops = bTier.GetStops(stationID);
                foreach (BusinessTier.CTAStop stop in stops)
                {
                    this.lstStops.Items.Add(stop.Name);
                    
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                //if (db != null && db.State == ConnectionState.Open)
                //    db.Close();
            }
        }

        private void ClearStationUI(bool clearStatations = false)
        {
            ClearStopUI();

            this.txtTotalRidership.Clear();
            this.txtTotalRidership.Refresh();

            this.txtAvgDailyRidership.Clear();
            this.txtAvgDailyRidership.Refresh();

            this.txtPercentRidership.Clear();
            this.txtPercentRidership.Refresh();

            this.txtStationID.Clear();
            this.txtStationID.Refresh();

            this.txtWeekdayRidership.Clear();
            this.txtWeekdayRidership.Refresh();
            this.txtSaturdayRidership.Clear();
            this.txtSaturdayRidership.Refresh();
            this.txtSundayHolidayRidership.Clear();
            this.txtSundayHolidayRidership.Refresh();

            this.lstStops.Items.Clear();
            this.lstStops.Refresh();

            if (clearStatations)
            {
                this.lstStations.Items.Clear();
                this.lstStations.Refresh();
            }
        }


        //
        // user has clicked on a stop for more info:
        //
        private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStops.SelectedIndex < 0)   // so return now in this case:
                return;

            //
            // clear GUI in case this fails:
            //
            ClearStopUI();

            //
            // now display info about this stop:
            //
            string stopName = this.lstStops.Text;
            stopName = stopName.Replace("'", "''");

            try
            {
                //
                // now we need to know what lines are associated 
                // with this stop:
                //
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                int stopID = bizTier.GetStopID(stopName);
                int lineID = bizTier.GetLineID(stopID);
                var lines = bizTier.GetLines(lineID);
                var stops = bizTier.GetStops(stopID);
                var ada = bizTier.GetAccessible(stopID);
                var direction = bizTier.GetDirection(stopID);
                string lat = bizTier.GetLat(stopID);
                string lon = bizTier.GetLon(stopID);

                // display colors:
                foreach (BusinessTier.Lines line in lines)
                {
                    this.lstLines.Items.Add(line.Color);
                }
                this.txtAccessible.Text = ada;
                this.txtDirection.Text = direction;
                this.txtLocation.Text = "(" + lat + "," + lon + ")";
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                //if (db != null && db.State == ConnectionState.Open)
                //    db.Close();
            }
        }

        private void ClearStopUI()
        {
            this.txtAccessible.Clear();
            this.txtAccessible.Refresh();

            this.txtDirection.Clear();
            this.txtDirection.Refresh();

            this.txtLocation.Clear();
            this.txtLocation.Refresh();

            this.lstLines.Items.Clear();
            this.lstLines.Refresh();
        }


        //
        // Top-10 stations in terms of ridership:
        //
        private void top10StationsByRidershipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
            ClearStationUI(true /*clear stations*/);

            //
            // now load top-10 stations:
            //
            SqlConnection db = null;

            try
            {
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                var top10 = bizTier.GetTopStations(10);
                foreach (BusinessTier.CTAStation top in top10)
                {
                    this.lstStations.Items.Add(top.Name);
                }

                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", top10.Count);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                if (db != null && db.State == ConnectionState.Open)
                    db.Close();
            }
        }

        private void txtAvgDailyRidership_TextChanged(object sender, EventArgs e)
        {

        }
    }//class
}//namespace
