using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data;
using IGService3.DTOs;
using NLog;
using IGConnect;
using dto.endpoint.accountactivity.transaction;
using dto.endpoint.accountactivity.activity;

namespace IGService3
{
  
    internal class Datalogger
    {
        string connectionString = @"Driver={SQLite3 ODBC Driver};Database=C:\inetpub\IGService3\db\aicodeDatabase.db";
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();
        public List<double> getHistoricDataEURUSD(int numRecord)
        {
            List<double> dataRead = new List<double>();
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM HistoricDataEURUSD ORDER BY id DESC LIMIT "+ numRecord.ToString();
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string date = reader.GetString(0);
                                string time = reader.GetString(1);
                                double open = reader.GetDouble(2);
                                double high = reader.GetDouble(3);
                                double low = reader.GetDouble(4);
                                double close = reader.GetDouble(5);
                                double volume = reader.GetDouble(6);
                                dataRead.Add( close);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - getHistoricDataEURUSD" + ex.Message);
            }
            return dataRead;
        }

        public int DeleteTransactionClosed()
        {
            int rowsAffected = 0;
            string query = "DELETE FROM TransactionClosed";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - DeleteTransactionClosed" + ex.Message);
            }
            return rowsAffected;
        }

        public int DeletelastActivity()
        {
            int rowsAffected = 0;
            string query = "DELETE FROM lastActivity";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - DeletelastActivity" + ex.Message);
            }
            return rowsAffected;
        }

        public int UpdateLucaOrder3(string dealID, string close)
        {
            int rowsAffected = 0;
            string query = "UPDATE LucaOrder SET close = '" + close + "' WHERE dealID = '" + dealID + "'";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateLucaOrder3" + ex.Message);
            }
            return rowsAffected;
        }
        public int UpdateLucaOrder2(string gainLost, string reference)
        {
            int rowsAffected = 0;
            string query = "UPDATE LucaOrder SET gainLost = '" + gainLost + "' WHERE reference = '" + reference + "'";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateLucaOrder2" + ex.Message);
            }
            return rowsAffected;
        }

        public int UpdateLucaOrder(string gainLost, string dealID)
        {
            int rowsAffected = 0;
            string query = "UPDATE LucaOrder SET gainLost = '"+ gainLost+ "' WHERE dealID = '" + dealID+"'";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateLucaOrder" + ex.Message);
            }
            return rowsAffected;
        }
        public int UpdatelastActivity(Activity activity)
        {
            int rowsAffected = 0;

            string query = "INSERT INTO lastActivity (date,dealId,epic,channel,type,status,description,reference) " +
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            try
            {
                string[] results = activity.result.Split(':');
                string reference = results[1];

                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = activity.date;
                        command.Parameters.Add("@dealId", OdbcType.VarChar).Value = activity.dealId;
                        command.Parameters.Add("@epic", OdbcType.VarChar).Value = activity.epic;
                        command.Parameters.Add("@channel", OdbcType.VarChar).Value = activity.channel;
                        command.Parameters.Add("@type", OdbcType.VarChar).Value = "";
                        command.Parameters.Add("@status", OdbcType.VarChar).Value = "";
                        command.Parameters.Add("@description", OdbcType.VarChar).Value = "";
                        command.Parameters.Add("@reference", OdbcType.VarChar).Value = activity.result; 
                         rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdatelastActivity" + ex.Message);
            }
            return rowsAffected;
        }
        public int UpdateTransactionClosed(Transaction transaction)
        {
            int rowsAffected = 0;
  
            string query = "INSERT INTO TransactionClosed (date,dateUtc,openDateUtc,instrumentName,period,profitAndLoss,reference,openLevel,closeLevel,size) " +
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            try
            {



                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value =transaction.date;
                        command.Parameters.Add("@dateUtc", OdbcType.VarChar).Value = "";
                        command.Parameters.Add("@openDateUtc", OdbcType.VarChar).Value = "";
                        command.Parameters.Add("@instrumentName", OdbcType.VarChar).Value = transaction.instrumentName;
                        command.Parameters.Add("@period", OdbcType.VarChar).Value = transaction.period;
                        command.Parameters.Add("@profitAndLoss", OdbcType.VarChar).Value = transaction.profitAndLoss;
                        command.Parameters.Add("@reference", OdbcType.VarChar).Value = transaction.reference;
                        command.Parameters.Add("@openLevel", OdbcType.VarChar).Value = transaction.openLevel;
                        command.Parameters.Add("@closeLevel", OdbcType.VarChar).Value = transaction.closeLevel;
                        command.Parameters.Add("@size", OdbcType.VarChar).Value = transaction.size;
                         // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - TransactionClosed" + ex.Message);
            }
            return rowsAffected;
        }

        public int UpdateLucaOrder(DateTime _date, string epic, decimal target, decimal? stoloss, string dealID, decimal? open, decimal? close, string direction)
        {
            int rowsAffected = 0;
            if(stoloss==null)
                stoloss = 0;
            string query = "INSERT INTO LucaOrder (date,time,epic,target,stoloss,dealID,open,close,direction) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
            try
            {

                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = _date.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = _date.ToString("HH:mm");
                        command.Parameters.Add("@epic", OdbcType.VarChar).Value = epic;
                        command.Parameters.Add("@target", OdbcType.Double).Value = target;
                        command.Parameters.Add("@stoloss", OdbcType.Double).Value = stoloss;
                        command.Parameters.Add("@dealID", OdbcType.VarChar).Value = dealID;
                        command.Parameters.Add("@open", OdbcType.VarChar).Value = open;
                        command.Parameters.Add("@close", OdbcType.VarChar).Value = close;
                        command.Parameters.Add("@direction", OdbcType.VarChar).Value = direction;
                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateDataEURUSD" + ex.Message);
            }
            return rowsAffected;
        }

        public int UpdateIGBackAnalysis(BackAnalysis backAnalysis)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO IGBackAnalysis (date,time,size,foglio,Tempo,ValT,S3,S2,S1,PV,R1,R2,R3,Bo,BC,Bsg,Bsl,So,SC,Ssg,Ssl,Slop,Trend,Mark,RSI,VolV,GainPerc,Gain,StartO,StopO,Spread,VolM) " +
                                                " VALUES (?,?,?,?,?,?,?,?,?,?, " +
                                                         "?,?,?,?,?,?,?,?,?,?," +
                                                         "?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = DateTime.Now.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = DateTime.Now.ToString("HH:mm");
                        command.Parameters.Add("@size", OdbcType.Double).Value = backAnalysis.Size;
                        command.Parameters.Add("@foglio", OdbcType.VarChar).Value = backAnalysis.foglio;
                        command.Parameters.Add("@Tempo", OdbcType.Double).Value = backAnalysis.Tempo;
                        command.Parameters.Add("@ValT", OdbcType.Double).Value = backAnalysis.ValT;
                        command.Parameters.Add("@S3", OdbcType.Double).Value = backAnalysis.S3;
                        command.Parameters.Add("@S2", OdbcType.Double).Value = backAnalysis.S2;
                        command.Parameters.Add("@S1", OdbcType.Double).Value = backAnalysis.S1;
                        command.Parameters.Add("@PV", OdbcType.Double).Value = backAnalysis.PV;
                        command.Parameters.Add("@R1", OdbcType.Double).Value = backAnalysis.R1;
                        command.Parameters.Add("@R2", OdbcType.Double).Value = backAnalysis.R2;
                        command.Parameters.Add("@R3", OdbcType.Double).Value = backAnalysis.R3;
                        command.Parameters.Add("@Bo", OdbcType.Double).Value = backAnalysis.Bo;
                        command.Parameters.Add("@BC", OdbcType.Double).Value = backAnalysis.BC;
                        command.Parameters.Add("@Bsg", OdbcType.Double).Value = backAnalysis.Bsg;
                        command.Parameters.Add("@Bsl", OdbcType.Double).Value = backAnalysis.Bsl;
                        command.Parameters.Add("@So", OdbcType.Double).Value = backAnalysis.So;
                        command.Parameters.Add("@SC", OdbcType.Double).Value = backAnalysis.SC;
                        command.Parameters.Add("@Ssg", OdbcType.Double).Value = backAnalysis.Ssg;
                        command.Parameters.Add("@Ssl", OdbcType.Double).Value = backAnalysis.Ssl;
                        command.Parameters.Add("@Slop", OdbcType.Double).Value = backAnalysis.Slop;
                        command.Parameters.Add("@Trend", OdbcType.Double).Value = backAnalysis.Trend;
                        command.Parameters.Add("@Mark", OdbcType.Double).Value = backAnalysis.Mark;
                        command.Parameters.Add("@RSI", OdbcType.Double).Value = backAnalysis.RSI;
                        command.Parameters.Add("@VolV", OdbcType.Double).Value = backAnalysis.VolV;
                        command.Parameters.Add("@GainPerc", OdbcType.Double).Value = backAnalysis.GainPerc;
                        command.Parameters.Add("@Gain", OdbcType.Double).Value = backAnalysis.Gain;
                        command.Parameters.Add("@StartO", OdbcType.Int).Value = backAnalysis.StartO;
                        command.Parameters.Add("@StopO", OdbcType.Int).Value = backAnalysis.StopO;
                        command.Parameters.Add("@Spread", OdbcType.Double).Value = backAnalysis.Spread;
                        command.Parameters.Add("@VolM", OdbcType.Double).Value = backAnalysis.VolM;
                          // Execute the INSERT query
                         rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateIGBackAnalysis" + ex.Message);
            }
            return rowsAffected;
        }

        public int  UpdateDataEURUSD(DateTime _date, decimal open, decimal close, decimal high, decimal low, decimal volume)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO HistoricDataEURUSD (date, time,open,high,low,close,volume) VALUES (?, ?, ?, ?, ?, ?, ?)";
            try
            {
 
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = _date.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = _date.ToString("HH:mm");
                        command.Parameters.Add("@open", OdbcType.Double).Value = (double)open;
                        command.Parameters.Add("@high", OdbcType.Double).Value = (double)high;
                        command.Parameters.Add("@low", OdbcType.Double).Value = (double)low;
                        command.Parameters.Add("@close", OdbcType.Double).Value = (double)close;
                        command.Parameters.Add("@volume", OdbcType.Double).Value = (double)volume;

                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateDataEURUSD" + ex.Message);
            }
            return rowsAffected;
        }

      
  		public void getSlopeEURUSD(out double slope)
        {
            slope = 0;

            List<double> dataRead = new List<double>();
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM SlopeEURUSD ORDER BY date,time DESC LIMIT 1";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string date = reader.GetString(0);
                                string time = reader.GetString(1);
                                slope = reader.GetDouble(2)*1000;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - getSlopeEURUSD" + ex.Message);
            }

        }
        public void getRsiEURUSD(out double Rsi)
        {
            Rsi = 0;

            List<double> dataRead = new List<double>();
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM RsiEURUSD ORDER BY date,time DESC LIMIT 1";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string date = reader.GetString(0);
                                string time = reader.GetString(1);
                                Rsi = reader.GetDouble(2);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - getRsiEURUSD" + ex.Message);
            }
        }
		public void getLastDataEURUSD(out double priceask, out double pricebid, out double volume)
        {
            priceask = 0;
            pricebid = 0;
            volume = 0;
            List<double> dataRead = new List<double>();
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM LastEURUSD ORDER BY date,time DESC LIMIT 1";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string date = reader.GetString(0);
                                string time = reader.GetString(1);
                                priceask = reader.GetDouble(2);
                                pricebid = reader.GetDouble(3);
                                volume = reader.GetDouble(4);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - getLastDataEURUSD" + ex.Message);
            }

        }
     
        public int UpdateAccount(StreamingAccountData AccountData)
        {
            int rowsAffected = 0;
            try
            {

                string query = "INSERT INTO AccountState (date,time,available,balance,deposit, profitloss) VALUES (?, ?, ?, ?, ?, ?)";

                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = DateTime.Now.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = DateTime.Now.ToString("HH:mm");
                        command.Parameters.Add("@available", OdbcType.Double).Value = (double)AccountData.AvailableCash;
                        command.Parameters.Add("@balance", OdbcType.Double).Value = (double)AccountData.AvailableCash+(double)AccountData.Deposit;
                        command.Parameters.Add("@deposit", OdbcType.Double).Value = (double)AccountData.Deposit;
                        command.Parameters.Add("@profitloss", OdbcType.Double).Value = (double)AccountData.ProfitAndLoss; ;
                    
                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateAccount" + ex.Message);
            }
            return rowsAffected;
        }
        public int UpdateLastEURUSD(DateTime _date, decimal offer, decimal bid, decimal volume,int gapLong, int gapShort, decimal Open, decimal Close, decimal High, decimal Low )
        {
            int rowsAffected = 0;
             try
            {
                string query = "DELETE FROM LastEURUSD";
                try
                {
                    using (OdbcConnection connection = new OdbcConnection(connectionString))
                    {
                        connection.Open();

                        using (OdbcCommand command = new OdbcCommand(query, connection))
                        {
                            rowsAffected = command.ExecuteNonQuery();

                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    _Logger.Error("Exception - Delete LastEURUSD" + ex.Message);
                }
                query = "INSERT INTO LastEURUSD (date, time,priceask,pricebid,volume, gapLong,  gapShort , Open, Close, High, Low) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = _date.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = _date.ToString("HH:mm");
                        command.Parameters.Add("@priceask", OdbcType.Double).Value = (double)offer;
                        command.Parameters.Add("@pricebid", OdbcType.Double).Value = (double)bid;
                        command.Parameters.Add("@volume", OdbcType.Double).Value = (double)volume;
                        command.Parameters.Add("@gapLong", OdbcType.Int).Value = gapLong;
                        command.Parameters.Add("@gapShort", OdbcType.Int).Value = gapShort;
                        command.Parameters.Add("@open", OdbcType.Double).Value = (double)Open;
                        command.Parameters.Add("@close", OdbcType.Double).Value = (double)Close;
                        command.Parameters.Add("@high", OdbcType.Double).Value = (double)High;
                        command.Parameters.Add("@low", OdbcType.Double).Value = (double)Low;

                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateLastEURUSD" + ex.Message);
            }
            return rowsAffected;
        }

      
    	public void GetPivot(out List<string> data, out List<double> s3, out List<double> s2, out List<double> s1, out List<double> Pivot, out List<double> r3, out List<double> r2, out List<double> r1)
        {
            data = new List<string>();
            Pivot = new List<double>();
            s3 = new List<double>();
            s2 = new List<double>();
            s1 = new List<double>();
            r3 = new List<double>();
            r2 = new List<double>();
            r1 = new List<double>();

            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM pivotPoint";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                data.Add(DateTime.Parse(reader.GetString(0) + " " + reader.GetString(1)).ToString());
                                r3.Add(reader.GetDouble(2));
                                r2.Add(reader.GetDouble(3));
                                r1.Add(reader.GetDouble(4));
                                Pivot.Add(reader.GetDouble(5));
                                s3.Add(reader.GetDouble(6));
                                s2.Add(reader.GetDouble(7));
                                s1.Add(reader.GetDouble(8));
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - GetPivot" + ex.Message);
            }
            if(r3.Count==0)
            {
                try
                {
                    using (OdbcConnection connection = new OdbcConnection(connectionString))
                    {
                        connection.Open();

                        // Execute a query
                        string query = "SELECT * FROM pivotPointMio";
                        using (OdbcCommand command = new OdbcCommand(query, connection))
                        {
                            using (OdbcDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    data.Add(DateTime.Parse(reader.GetString(0) + " " + reader.GetString(1)).ToString());
                                    r3.Add(reader.GetDouble(2));
                                    r2.Add(reader.GetDouble(3));
                                    r1.Add(reader.GetDouble(4));
                                    Pivot.Add(reader.GetDouble(5));
                                    s3.Add(reader.GetDouble(6));
                                    s2.Add(reader.GetDouble(7));
                                    s1.Add(reader.GetDouble(8));
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    _Logger.Error("Exception - GetPivot" + ex.Message);
                }
            }

        }

        public void DeleteData4H()
        {
            string query = "DELETE FROM LastEURUSD";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        var rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - DeleteData4H" + ex.Message);
            }
        }

        public int UpdateData4H(string _date, decimal open, decimal close, decimal high, decimal low, decimal volume, int datiRimanenti)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO HistoricData4H (date, time,open,high,low,close,volume,datiRimanenti) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = _date;
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = "";
                        command.Parameters.Add("@open", OdbcType.Double).Value = (double)open;
                        command.Parameters.Add("@high", OdbcType.Double).Value = (double)high;
                        command.Parameters.Add("@low", OdbcType.Double).Value = (double)low;
                        command.Parameters.Add("@close", OdbcType.Double).Value = (double)close;
                        command.Parameters.Add("@volume", OdbcType.Double).Value = (double)volume;
                        command.Parameters.Add("@datiRimanenti", OdbcType.Int).Value = datiRimanenti;

                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateData4H" + ex.Message);
            }
            return rowsAffected;
        }
    

        public int UpdateTrainingResultEURUSD(double coefficients, double intercept)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO TrainingResultEURUSD (date, time,coefficients,intercept) VALUES (?, ?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = DateTime.Now.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = DateTime.Now.ToString("HH:mm");
                        command.Parameters.Add("@coefficients", OdbcType.Double).Value = coefficients;
                        command.Parameters.Add("@intercept", OdbcType.Double).Value = intercept;
                     
                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateTrainingResultEURUSD" + ex.Message);
            }
            return rowsAffected;
        }
        public int UpdatePredictStockPriceEURUSD(double StockPrice)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO PredictStockPriceEURUSD (date, time, price) VALUES (?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = DateTime.Now.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = DateTime.Now.ToString("HH:mm");
                        command.Parameters.Add("@price", OdbcType.Double).Value = StockPrice;
      

                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdatePredictStockPriceEURUSD" + ex.Message);
            }
            return rowsAffected;
        }
        public double getVolume()
        {

            double volume = 0;
            List<double> dataRead = new List<double>();
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM IGBackAnalysis  ORDER BY ID DESC LIMIT 10";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                  volume = volume+ reader.GetDouble(25);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - getVolume" + ex.Message);
            }
            return (volume/10);
        }


    
        public int UpdateAccount(double Available, double Balance, double Deposit, double Profitloss)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO AccountState (date, time, available, balance, deposit, profitloss) VALUES (?, ?, ?, ?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = DateTime.Now.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = DateTime.Now.ToString("HH:mm");
                        command.Parameters.Add("@available", OdbcType.Double).Value = Available;
                        command.Parameters.Add("@balance", OdbcType.Double).Value = Balance;
                        command.Parameters.Add("@deposit", OdbcType.Double).Value = Deposit;
                        command.Parameters.Add("@profitloss", OdbcType.Double).Value = Profitloss;
                       // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateAccount" + ex.Message);
            }
            return rowsAffected;
        }
        public int DeleteAllStock()
        {
            int rowsAffected = 0;
            string query = "DELETE FROM ActiveStock";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                           rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - DeleteAllStock" + ex.Message);
            }
            return rowsAffected;
        }

        public List<double> getServicesEnable()
        {
            List<double> enabled = new List<double>();
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM servicesEnable";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                enabled.Add(reader.GetDouble(0));
                                enabled.Add(reader.GetDouble(1));
                                enabled.Add(reader.GetDouble(2));
                                enabled.Add(reader.GetDouble(3));
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                   _Logger.Error("Exception - getServicesEnable" + ex.Message);
            }

            return enabled;
        }



        public void enableAllService()
        {
            int rowsAffected = 0;
            string query = "INSERT INTO servicesEnable (IGManagerProd, IGServiceDevelop, IGService, IGServiceTest,IGServiceMonitor) VALUES (?, ?, ?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@IGManagerProd", OdbcType.VarChar).Value = 1;
                        command.Parameters.Add("@IGServiceDevelop", OdbcType.Double).Value = 0;
                        command.Parameters.Add("@IGService", OdbcType.VarChar).Value = 1;
                        command.Parameters.Add("@IGServiceTest", OdbcType.Double).Value = 0;
                        command.Parameters.Add("@IGServiceMonitor", OdbcType.Double).Value = 1;
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - enableAllService" + ex.Message);
            }
        }
        public void updateActivePeriod(Double enabled, Double volume, Double spreadPrice)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO activePeriod (date, time, enabled, volume, spreadPrice) VALUES (?, ?, ?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = DateTime.Now.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = DateTime.Now.ToString("HH:mm");
                        command.Parameters.Add("@enabled", OdbcType.Double).Value = enabled;
                        command.Parameters.Add("@volume", OdbcType.Double).Value = volume;
                        command.Parameters.Add("@spreadPrice", OdbcType.Double).Value = spreadPrice;
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - updateActivePeriod" + ex.Message);
            }
        }

        public void StopAllService()
        {
            int rowsAffected = 0;
            string query = "INSERT INTO servicesEnable (IGManagerProd, IGServiceDevelop, IGService, IGServiceTest,IGServiceMonitor) VALUES (?, ?, ?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@IGManagerProd", OdbcType.VarChar).Value = 0;
                        command.Parameters.Add("@IGServiceDevelop", OdbcType.Double).Value = 0;
                        command.Parameters.Add("@IGService", OdbcType.VarChar).Value = 0;
                        command.Parameters.Add("@IGServiceTest", OdbcType.Double).Value = 0;
                        command.Parameters.Add("@IGServiceMonitor", OdbcType.Double).Value = 0;
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - StopAllService" + ex.Message);
            }
        }
        public string GetTransactions(string dateFilter)
        {
            string Transactions =  "Init transaction\r\n";
            double gain = 0;
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "select date,time,size,gain FROM IGBackAnalysis WHERE  StopO=='1' AND DATE = '" + dateFilter+"'";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string strTmp = "";
                                strTmp = strTmp + reader.GetString(0);
                                strTmp = strTmp + " " + reader.GetString(1);
                                strTmp = strTmp + " " + reader.GetString(2);
                                strTmp = strTmp + " " + reader.GetDouble(3).ToString();
                                gain = gain+ reader.GetDouble(3);

                                Transactions = Transactions + strTmp + "\r\n";
                            }
                            Transactions = Transactions + "Gain/Lost: "+ gain.ToString() + "\r\n";
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - GetTransactions" + ex.Message);
            }
            return Transactions;
        }

        public void GetStock(out List<string> epic, out List<double> size, out List<string> direction, out List<double> gainLost, out List<string> dealId, out List<double> price)
        {
            epic = new List<string>();
            direction = new List<string>();
            size = new List<double>();
            gainLost = new List<double>();
            dealId = new List<string>();
            price = new List<double>();
          
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Execute a query
                    string query = "SELECT * FROM ActiveStock";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                epic.Add(reader.GetString(2));
                                size.Add(reader.GetDouble(3));
                                direction.Add(reader.GetString(4));
                                gainLost.Add(reader.GetDouble(5));
                                dealId.Add(reader.GetString(6));
                                price.Add(reader.GetDouble(7));
                              }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - GetStock" + ex.Message);
            }
        }

   
        public void UpdateStock(string epic, double size, string direction, double gainLost, string dealId, double price)
        {
            int rowsAffected = 0;
            string query = "INSERT INTO ActiveStock (date, time, epic, size, direction, gainLost, dealId, price) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.Add("@date", OdbcType.VarChar).Value = DateTime.Now.ToString("yyyy.MM.dd");
                        command.Parameters.Add("@time", OdbcType.VarChar).Value = DateTime.Now.ToString("HH:mm");
                        command.Parameters.Add("@epic", OdbcType.VarChar).Value = epic;
                        command.Parameters.Add("@size", OdbcType.Double).Value = size;
                        command.Parameters.Add("@direction", OdbcType.VarChar).Value = direction;
                        command.Parameters.Add("@gainLost", OdbcType.Double).Value = gainLost;
                        command.Parameters.Add("@dealId", OdbcType.VarChar).Value = dealId;
                        command.Parameters.Add("@price", OdbcType.Double).Value = price;
                        // Execute the INSERT query
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - UpdateStock" + ex.Message);
            }
        }
    }
}