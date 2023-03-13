
using Autodesk.AutoCAD.DatabaseServices;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Spire.Pdf.General.Render.Font.OpenTypeFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DataColumn = System.Data.DataColumn;
using DataTable = System.Data.DataTable;

namespace CADMTools.DLL
{
    public static class CollectionMethods
    {
        private static string GetListWithNoneOrSelectAllFilePath = @".\Logs\GetListWithNoneOrSelectAll.txt";
        public static List<string> GetListOfColumnItemsFromDataTableOneColumnName(DataTable dataTable, string columnName)
        {
            var ListOfStringValues = new List<string>();
            if (dataTable.Columns.Contains(columnName))
            {
                ListOfStringValues = dataTable.AsEnumerable().Select(row => row.Field<string>(columnName)).ToList();

            }
            return ListOfStringValues;
        }
        public static List<string> GetListOfColumnItemsFromDataTableListOfColumnsNames(DataTable dataTable, List<string> columnsNamesList)
        {
            var ListOfStringValues = new List<string>();
            if (columnsNamesList != null)
            {
                foreach (var columnName in columnsNamesList)
                {
                    ListOfStringValues = dataTable.AsEnumerable().Select(row => row.Field<string>(columnName)).ToList();
                }
            }
            return ListOfStringValues;
        }
        public static List<double> GetNumberListFromStringList(List<string> stringlist)
        {
            var intlist = new List<double>();
            foreach (var item in stringlist)
            {
                intlist.Add(StringMethods.IsDoubleNumber(item));
            }
            return intlist;
        }
        public static List<string> GetUniqeItemsListOfColumnFromDataTable(DataTable dataTable, string columnName)
        {
            var ListOfStringValues = new List<string>();
            ListOfStringValues.Insert(0, "None");
            var dataAsList = dataTable.AsEnumerable();
            if (dataAsList != null && columnName != "")
            {
                ListOfStringValues.AddRange(dataAsList.Select(row => row.Field<string>(columnName)).Distinct().ToList());
            }
            return ListOfStringValues;
        }
        public static DataTable GetFilteredDataTable(DataTable dataTable, string columnName, string condition)
        {
            DataTable FilteredDataTable = new DataTable();
            var filteredList = new List<DataRow>();
            int colIndex = 0;
            int NewCount = 1;
            if (dataTable.Columns != null)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    DataColumn NewColumns = new DataColumn();
                    NewColumns.ColumnName = col.ColumnName;
                    FilteredDataTable.Columns.Add(NewColumns);
                    if (col.ColumnName == columnName)
                    {
                        colIndex = col.Ordinal;
                    }
                }
                if (dataTable.Rows != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var SpacificColumnList = row.ItemArray.ToList();
                        if (SpacificColumnList[colIndex].ToString() == condition)
                        {
                            filteredList.Add(row);
                        }
                    }
                    foreach (var row in filteredList)
                    {
                        DataRow NewRow = FilteredDataTable.NewRow();
                        NewRow.ItemArray = row.ItemArray;
                        var SpacificColumnList = NewRow.ItemArray;
                        foreach (var item in SpacificColumnList)
                        {
                            SpacificColumnList[0] = NewCount.ToString();
                            NewCount++;
                        }
                        FilteredDataTable.Rows.Add(NewRow);
                    }
                }
            }
            return FilteredDataTable;
        }
        public static List<string> GetColumnsNameAsStringList(DataTable dataTable)
        {
            var ColNames = new List<string>();
            foreach (DataColumn col in dataTable.Columns)
            {
                if (col.ColumnName != "Id")
                {
                    ColNames.Add(col.ColumnName);
                }
            }
            return ColNames;
        }
        public static int GetColumnIndexInDataGridAccordingName(DataGrid datagrid, string columnName)
        {
            var ConstantWidthColumn = datagrid.Columns.AsEnumerable().Select(x => x.Header.ToString() == columnName).ToList();
            int ConstantWidthColumnIndex = -1;
            for (int i = 0; i < ConstantWidthColumn.Count(); i++)
            {
                if (ConstantWidthColumn[i])
                {
                    ConstantWidthColumnIndex = i;
                }
            }
            return ConstantWidthColumnIndex;
        }
        public static List<string> GetListWithNoneOrSelectAll(List<string> MainList, string StringToList = "None")
        {
            var ColumnsListWithSelectAllOrNone = new StringBuilder();
            var ListOfFilterSelection = new List<string>();
            ListOfFilterSelection.Add(StringToList);
            ListOfFilterSelection.AddRange(MainList);
            foreach (var item in ListOfFilterSelection)
            {
                ColumnsListWithSelectAllOrNone.AppendLine(item);
            }
            File.WriteAllText(GetListWithNoneOrSelectAllFilePath, ColumnsListWithSelectAllOrNone.ToString());
            return ListOfFilterSelection;
        }
        public static void AutomaticTypeMarkUpdate(string TableName, string LengthColumnName, string ThicknessColumnName, string IDColumnName, string TypeMarkColumnName)
        {
            CURD SELECTThicknessColumnDis = new CURD();
            SELECTThicknessColumnDis.TableName = TableName;
            var ThicknesstListDis = ControlsMethods.GetListFromQueryFromDataBase(SELECTThicknessColumnDis.SelectDistictColumnFromOneTable(ThicknessColumnName));
            CURD SELECTThicknessColumn = new CURD();
            SELECTThicknessColumn.TableName = TableName;
            var ThicknesstList = ControlsMethods.GetListFromQueryFromDataBase(SELECTThicknessColumn.SelectColumnFromOneTable(ThicknessColumnName));
            var TypeMarkString = new List<string>();
            var OrderedIDList = new List<string>();
            int countThickness = 1;
            foreach (var Thicknessitem in ThicknesstListDis)
            {
                CURD SELECTLengthColumn = new CURD();
                SELECTLengthColumn.TableName = TableName;
                var LengthList = ControlsMethods.GetListFromQueryFromDataBase(SELECTLengthColumn.SelectColumnWHEREFromOneTable(LengthColumnName, $"{ThicknessColumnName} = '{Thicknessitem}'"));
                CURD SELECTLengthColumnDis = new CURD();
                SELECTLengthColumnDis.TableName = TableName;
                var LengthListDis = LengthList.Distinct().ToList();
                int countLegth = 1;
                foreach (var LengthitemDIS in LengthListDis)
                {
                    foreach (var Lengthitem in LengthList)
                    {
                        CURD SELECTIDColumn = new CURD();
                        SELECTIDColumn.TableName = TableName;
                        var IDList = ControlsMethods.GetListFromQueryFromDataBase(SELECTIDColumn.SelectColumnWHEREFromOneTable(IDColumnName, $"{ThicknessColumnName} = '{Thicknessitem}' AND {LengthColumnName} = '{Lengthitem}' "));
                        if (Lengthitem == LengthitemDIS)
                        {
                            foreach (var ID in IDList)
                            {
                                CURD UPDATE = new CURD();
                                UPDATE.TableName = TableName;
                                string UpdateID = UPDATE.UpdateColumnWithCondition(TypeMarkColumnName, $"P{countThickness} - {countLegth}", $"Id = '{ID}'");
                                ConnectToDataBase.ExcuteCommand(UpdateID);
                            }
                        }
                    }
                    countLegth++;
                }
                countThickness++;
            }
        }
    }
}
