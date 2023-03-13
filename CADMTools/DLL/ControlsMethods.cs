using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CADMTools.Data.DLL;

namespace CADMTools.DLL
{
    public static class ControlsMethods
    {
        private static string dataGridValueLogFile = @".\Logs\DataGridValue.txt";
        public static string DataGridValueLogFile
        {
            get { return Open_StreamFiles.IsDriectoryExists(dataGridValueLogFile); }
        }

        public static string FillTextBoxUsedAsIndexFromDataBase(TextBox textBox, string QueryOrder)
        {
            var dataTable = ConnectToDataBase.GetDataTable(QueryOrder);
            var DataOfRowsinDataTable = dataTable.Rows;
            var listOfData = new List<string>();
            for (int i = 0; i < DataOfRowsinDataTable.Count; i++)
            {
                listOfData.Add(DataOfRowsinDataTable[i].ItemArray.GetValue(0).ToString());
            }
            int count = 1;
            int IndexNumber = 0;
            for (int i = 0; i < listOfData.Count; i++)
            {
                if (listOfData[i].ToString() == count.ToString())
                {
                    count++;
                    IndexNumber = count;
                }
                else
                {
                    IndexNumber = count;
                }

            }
            if (IndexNumber == listOfData.Count)
            {
                IndexNumber = listOfData.Count + 1;
            }
            return IndexNumber.ToString();
        }
        public static void FillDataGridByDataTableFromDataBase(DataGrid dataGrid, string QueryOrder)
        {
            var dataTable = ConnectToDataBase.GetDataTable(QueryOrder);
            dataGrid.DataContext = dataTable.DefaultView; 
        }
        public static void FillGridFromListOfStrings(DataGrid dataGrid, List<string> List)
        {
            var FillLIst = new Dictionary<string, string>();
            int i = 0;
            foreach (var item in List)
            {
                FillLIst.Add(i.ToString(), item);
                i++;
            }
            dataGrid.ItemsSource = FillLIst;
        }
        public static void FillComboxAccordingToQueryFromDataBase(ComboBox comboBox, string QueryOrder)
        {
            var dataTable = ConnectToDataBase.GetDataTable(QueryOrder);
            comboBox.DataContext = dataTable.DefaultView;
        }
        public static List<string> GetListFromQueryFromDataBase(string QueryOrder)
        {
            var ListFromQuery = new List<string>();
            var dataTable = ConnectToDataBase.GetDataTable(QueryOrder);
            foreach (DataRow item in dataTable.Rows)
            {
                ListFromQuery.Add(item[0].ToString()); 
            }
            return ListFromQuery;
        }
        public static void FillTextBoxAccordingToQueryFromDataBase(TextBox textBox, string QueryOrder)
        {
            var dataTable = ConnectToDataBase.GetDataTable(QueryOrder);
            textBox.Text = dataTable.Rows[0].ItemArray[0].ToString();
        }
        public static void FillLableAccordingToQueryFromDataBase(Label lable, string QueryOrder)
        {
            var dataTable = ConnectToDataBase.GetDataTable(QueryOrder);
            lable.Content = dataTable.Rows[0].ItemArray[0].ToString();
        }
    }
}

