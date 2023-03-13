using Autodesk.AutoCAD.DatabaseServices;
using CADMTools.DLL;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADMTools.DLL
{
    public class CURD
    {
        #region Feilds
        private string txtfilepath = @".\Logs\Text Files\CURD.txt";
        private string newQueryLogFile = @".\Logs\nCURD.txt";
        private string selectQueryLogFile = @".\Logs\SelectQuery.txt";
        private string selecDisticttQueryLogFile = @".\Logs\SelectDistinctQuery.txt";
        private string selectWhereQueryLogFile = @".\Logs\SelectWHEREQuery.txt";
        private string insertQueryLogFile = @".\Logs\InsertQuery.txt";
        private string QueryCREATETABLE = @".\Logs\QueryCREATETABLE.txt";
        #endregion
        #region Properties
        public string TableName { get; set; }
        public string Txtfilepath { get => txtfilepath; set => txtfilepath = value; }
        private string NewQueryLogFile
        {
            get { return Open_StreamFiles.IsDriectoryExists(newQueryLogFile); }
        }
        private string SelectQueryLogFile
        {
            get { return Open_StreamFiles.IsDriectoryExists(selectQueryLogFile); }
        }

        private string InsertQueryLogFile
        {
            get { return Open_StreamFiles.IsDriectoryExists(insertQueryLogFile); }
        }
        #endregion

        public string SelectAll()
        {
            var SELECTstring = new StringBuilder($"SELECT * FROM {TableName}");
            return SELECTstring.ToString();
        }
        public string SelectAllWithCondtion(string Condtion)
        {
            var SELECTstring = new StringBuilder($"SELECT * FROM {TableName} WHERE {Condtion}");
            return SELECTstring.ToString();
        }
        public string SelectAllOrderBy(string ColumnName)
        {
            var SELECTALLORDERBYstring = new StringBuilder($"SELECT * FROM {TableName} ORDER BY {ColumnName}");
            return SELECTALLORDERBYstring.ToString();
        }
        public string SelectColumnFromOneTable(string columnsName)
        {
            if (columnsName == "None" || columnsName == "Select All" || columnsName == null || columnsName == "")
            {
                return SelectAll();
            }
            else
            {
                var SELECTstring = new StringBuilder($"SELECT {columnsName} FROM {TableName}");
                File.WriteAllText(SelectQueryLogFile, "The Query Of Selected Data\n" + SELECTstring.ToString());
                return SELECTstring.ToString();
            }
        }
        public string SelectDistictColumnFromOneTable(string columnsName)
        {
            if (columnsName == "None" || columnsName == "Select All" || columnsName == null || columnsName == "")
            {
                return SelectAll();
            }
            else
            {
                var SELECTstring = new StringBuilder($"SELECT DISTINCT {columnsName} FROM {TableName}");
                File.WriteAllText(selecDisticttQueryLogFile, "The Query Of Selected Data\n" + SELECTstring.ToString());
                return SELECTstring.ToString();
            }
        }
        public string SelectColumnWHEREFromOneTable(string columnsName, string CondtionString)
        {
            if (columnsName == "None" || columnsName == "Select All" || columnsName == null || columnsName == "")
            {
                return SelectAll();
            }
            else
            {
                var SELECTstring = new StringBuilder($"SELECT {columnsName} FROM {TableName} WHERE {CondtionString};");
                File.WriteAllText(selectWhereQueryLogFile, "The Query Of Selected Data\n" + SELECTstring.ToString());
                return SELECTstring.ToString();
            }
        }
        public string SelectColumnsFromOneTable(List<string> columnsName)
        {
            var SELECTstring = new StringBuilder($"SELECT ");
            foreach (var Col in columnsName)
            {
                SELECTstring.Append($"{Col.Replace(" ", "_")},");
            }
            SELECTstring.Remove(SELECTstring.Length - 1, 1);
            SELECTstring.Append($" FROM {TableName}");
            File.WriteAllText(SelectQueryLogFile, "The Query Of Selected Data\n" + SELECTstring.ToString());
            return SELECTstring.ToString();
        }
        public string GetQueryFromTextFile(string Schema = "dbo.")
        {
            StringBuilder Query = new StringBuilder();
            var TextLines = File.ReadLines(Txtfilepath).ToList<string>();
            foreach (var line in TextLines)
            {
                Query.Append($"{line} ");
            }
            Query.Replace(Schema, "");
            File.WriteAllText(NewQueryLogFile, "The New Query After Remove Schema\n" + Query.ToString());
            return Query.ToString();
        }
        public string InsertData(List<string> data, List<string> columnsName)
        {
            var INSERTstring = new StringBuilder($"INSERT INTO {TableName} \n(");
            foreach (var Column in columnsName)
            {
                INSERTstring.Append($"{Column}, ");
            }
            INSERTstring.Remove(INSERTstring.Length - 2, 2);
            INSERTstring.Append($")\nVALUES\n(");
            foreach (var Value in data)
            {
                if (Value == "" || Value == "NULL")
                {
                    INSERTstring.Append($"NULL, ");
                }
                else
                {
                    if (int.TryParse(Value, out int NewValue))
                    {
                        INSERTstring.Append($"{NewValue}, ");
                    }
                    else
                    {
                        INSERTstring.Append($"'{Value}', ");
                    }
                }
            }
            INSERTstring.Remove(INSERTstring.Length - 2, 2);
            INSERTstring.Append($")");
            File.WriteAllText(InsertQueryLogFile, "The Query Of Insert Data\n" + INSERTstring.ToString());
            return INSERTstring.ToString();
        }
        public string InsertDataInOneColumn(List<string> data, string columnsName)
        {
            var INSERTstring = new StringBuilder($"INSERT INTO {TableName} \n(");
            INSERTstring.Append($"{columnsName}");
            INSERTstring.Append($")\nVALUES\n(");
            foreach (var Value in data)
            {
                if (Value == "" || Value == "NULL")
                {
                    INSERTstring.Append($"NULL, ");
                }
                else
                {
                    if (int.TryParse(Value, out int NewValue))
                    {
                        INSERTstring.Append($"{NewValue}, ");
                    }
                    else
                    {
                        INSERTstring.Append($"'{Value}', ");
                    }
                }
            }
            INSERTstring.Remove(INSERTstring.Length - 2, 2);
            INSERTstring.Append($")");
            File.WriteAllText(InsertQueryLogFile, "The Query Of Insert Data\n" + INSERTstring.ToString());
            return INSERTstring.ToString();
        }
        public string InsertData(List<List<string>> dataRow, List<string> columnsName)
        {
            var INSERTstring = new StringBuilder();
            foreach (var Row in dataRow)
            {
                INSERTstring.Append($"\nINSERT INTO {TableName} \n(");
                foreach (var Column in columnsName)
                {
                    INSERTstring.Append($"{Column}, ");
                }
                INSERTstring.Remove(INSERTstring.Length - 2, 2);
                INSERTstring.Append($")\nVALUES\n(");
                foreach (var value in Row)
                {
                    if (value == "" || value == "NULL")
                    {
                        INSERTstring.Append($"NULL, ");
                    }
                    else
                    {
                        if (double.TryParse(value, out double NewValue))
                        {
                            INSERTstring.Append($"{NewValue}, ");
                        }
                        else
                        {
                            INSERTstring.Append($"'{value}', ");
                        }
                    }
                }
                INSERTstring.Remove(INSERTstring.Length - 2, 2);
                INSERTstring.Append($"); \n");
            }
            File.WriteAllText(InsertQueryLogFile, "The Query Of Insert Data\n" + INSERTstring.ToString());
            return INSERTstring.ToString();
        }
        public string CreateTableWithColumnsFromClass<T>(T castlass) where T : class
        {
            var TableName = ClassMethods.GetTableNameFromClassForDataBase(castlass);
            var ColumnsName = ClassMethods.GetListOfColumnNamesFromClass(castlass).ColumnsNameForDataBase;
            var ColumnsType = ClassMethods.GetListOfDataColumnTypeFromClass(castlass).ColumnsDataTypeList;
            int count = 0;
            var CREATEstring = new StringBuilder($"CREATE TABLE {TableName}(");
            foreach (var colName in ColumnsName)
            {
                CREATEstring.Append("\n" + colName + " " + ColumnsType[count] + ",");
                count++;
            }
            CREATEstring.Remove(CREATEstring.Length - 1, 1);
            CREATEstring.Append("\n);");
            File.Delete(QueryCREATETABLE);
            File.AppendAllText(QueryCREATETABLE, CREATEstring.ToString());
            return CREATEstring.ToString();
        }
        public string TruncateTable()
        {
            var SELECTstring = new StringBuilder($"TRUNCATE TABLE {TableName};");
            return SELECTstring.ToString();
        }
        public string UpdateColumnWithCondition(string ColumnName,string ValueToUpdate,string Condtion)
        {
            var UPDATEstring = new StringBuilder($"UPDATE {TableName} " +
                $"SET {ColumnName} = '{ValueToUpdate}'\n"+
                $"WHERE {Condtion}");
            return UPDATEstring.ToString();
        }
        public string UpdateColumnByIdColumn(string ColumnName, List<string> ListOfValuesToUpdate,List<ObjectId> ListOfOjectsId,string IdColumnName = "Id")
        {
            var UPDATEstring = new StringBuilder();
            int ColumnCount = 0;
            foreach (var ValueToUpdate in ListOfValuesToUpdate)
            {
                UPDATEstring.Append($"UPDATE {TableName}\n"+
                    $"SET {ColumnName} = '{ValueToUpdate}'\n"+
                    $"WHERE {IdColumnName} = '{ListOfOjectsId[ColumnCount].ToString().Replace("(","").Replace(")","")}'\n");
                ColumnCount++;
            }
            return UPDATEstring.ToString();
        }
    }
}
