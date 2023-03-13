using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Windows.Input;
using System.Drawing;
using System.Collections;
using System.IO;

namespace CADMTools.DLL
{
    public class ConnectToDataBase
    {
        #region Faileds
        private static DbDataAdapter adapter;
        private SqlDataAdapter sqlDataAdapter;  
        private SqlConnection sqlConnection;
        private OleDbConnection oleDbConnection;
        private static IDbConnection dbConnection;
        private static IDbCommand dbCommand;
        private static StringBuilder connectionSate = new StringBuilder();
        private string serverName;
        private string dataBaseName;
        private string iD;
        private string password;
        private bool enccrypt = false;
        private bool trustedConnection = true;
        private string DataStringConnectionFile = @".\Logs\DataConnectionString.txt";
        #endregion
        #region Properties
        public string ServerName { get => serverName.Trim(); set => serverName = @value.Trim(); }
        public string DataBaseName { get => dataBaseName.Trim(); set => dataBaseName = value.Trim(); }
        public string ID { get => iD.Trim(); set => iD = value.Trim(); }
        public string Password { get => password.Trim(); set => password = value.Trim(); }
        public bool Enccrypt { get => enccrypt; set => enccrypt = value; }
        public bool TrustedConnection { get => trustedConnection; set => trustedConnection = value; }
        #endregion
        #region Enums
        public enum DataBase
        {
            SQL,
            Access,
            Orecal
        }
        #endregion
        private string ConnectionString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Server = {@ServerName};" +
            $"Database = {DataBaseName};" +
            $"User ID = {ID};" +
            $"Password = {Password};" +
            $"Trusted_Connection = {TrustedConnection};" +
            $"Encrypt = {Enccrypt};");
            File.WriteAllText(DataStringConnectionFile, "Data String Connection\n" + stringBuilder.ToString());
            return stringBuilder.ToString();
        }
        public void Connect(DataBase dataBase = DataBase.SQL, string CommandString = null)
        {
            string ConString = ConnectionString();
            switch (dataBase)
            {
                case DataBase.SQL:
                    sqlConnection = new SqlConnection(ConString);
                    sqlConnection.Open();
                    dbConnection = sqlConnection;
                    adapter = sqlDataAdapter;
                    if (connectionSate != null)
                    {
                        connectionSate.Clear();
                    }
                    connectionSate.Append(dbConnection.State.ToString() + " " + dbConnection.Database.ToString());
                    dbCommand = dbConnection.CreateCommand();
                    break;
                case DataBase.Access:
                    oleDbConnection = new OleDbConnection(ConString);
                    oleDbConnection.Open();
                    dbConnection = oleDbConnection;
                    if (connectionSate != null)
                    {
                        connectionSate.Clear();
                    }
                    connectionSate.Append(dbConnection.State.ToString() + " " + dbConnection.Database.ToString());
                    dbCommand = dbConnection.CreateCommand();
                    break;
                case DataBase.Orecal:
                    break;
                default:
                    break;
            }
        }
        public static (StringBuilder CommandState, Color color) ExcuteCommand(string QureyString)
        {
            StringBuilder CommandState = new StringBuilder();
            if (dbConnection.State.ToString() == "Close")
            {
                dbConnection.Open();
            }
            dbCommand.CommandText = QureyString;
            if (QureyString != null || QureyString != "")
            {
                int RowsEffected = dbCommand.ExecuteNonQuery();
                CommandState = connectionSate.Append($"Succeeded To Excute The Command -- The Number Of Rows Effected = [{RowsEffected}]");
                return (CommandState, color: Color.Green);
            }
            if (connectionSate != null)
            {
                connectionSate.Clear();
            }
            CommandState = connectionSate.Append("The Connection To DataBase Is " + dbConnection.State.ToString() + " --- Please Connect To DataBase");
            return (CommandState, color: Color.Red);
        }
        public string ConnectionState()
        {
            return connectionSate.ToString();
        }
        public static IDataReader ExcuteDataReader(string QueryOrder)
        {
            dbCommand.CommandText = QueryOrder;
            return dbCommand.ExecuteReader();
        }
        public static DataTable GetDataTable(string QueryOrder)
        {
            DataTable dataTable = new DataTable();
            IDataReader dataTableReader = ExcuteDataReader(QueryOrder);
            dataTable.Load(dataTableReader);
            dataTableReader.Close();
            return dataTable;
        }
        public static void FillDataBaseWithDataTable(DataTable dataTable)
        {
            adapter.Fill(dataTable);
        }
    }
}
