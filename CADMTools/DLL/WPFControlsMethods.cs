using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Spire.Pdf.General.Render.Font.OpenTypeFile;
using Spire.Xls.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using CheckBox = System.Windows.Controls.CheckBox;
using DataColumn = System.Data.DataColumn;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridColumn = System.Windows.Controls.DataGridColumn;
using DataTable = System.Data.DataTable;
using ListBox = System.Windows.Controls.ListBox;

namespace CADMTools.DLL
{
    public static class WPFControlsMethods
    {
        const string SelectAllContain = "Select All";
        public static string GetTabNameAccordingSelection(TabControl tabControl)
        {
            TabItem tabItem = (TabItem)tabControl.SelectedItem;
            return tabItem.Header.ToString();

        }
        public static void FillComboBoxFromListOfString(ComboBox comboBox, List<string> fillValuesList)
        {
            comboBox.ItemsSource = fillValuesList;
            comboBox.SelectedIndex = 0;
        }
        public static void FillComboBoxByCheckBoxsFromListOfString(ComboBox comboBox, List<string> StringList)
        {
            if (comboBox != null)
            {
                comboBox.Items.Clear();
            }
            CheckBox checkBoxSelectAll = new CheckBox();
            checkBoxSelectAll.Content = SelectAllContain;
            checkBoxSelectAll.IsChecked = true;
            comboBox.Items.Add(checkBoxSelectAll);
            foreach (var item in StringList)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = $"{item}";
                checkBox.IsChecked = true;
                comboBox.Items.Add(checkBox);
            }
            comboBox.SelectedIndex = 0;
        }
        public static void FillListBoxByCheckBoxsFromListOfString(ListBox listBox, List<string> StringList)
        {
            if (listBox.Items != null)
            {
                listBox.Items.Clear();
            }
            CheckBox checkBoxSelectAll = new CheckBox();
            checkBoxSelectAll.Content = SelectAllContain;
            checkBoxSelectAll.FontSize = 10;
            checkBoxSelectAll.IsChecked = true;
            listBox.Items.Add(checkBoxSelectAll);
            foreach (var item in StringList)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = $"{item}";
                checkBox.FontSize = 10;
                checkBox.IsChecked = true;
                listBox.Items.Add(checkBox);
            }
            listBox.SelectedIndex = 0;
        }
        public static void CheckBoxsInMapping(ComboBox comboBox)
        {
            var ComboBoxListItems = comboBox.Items;
            if (comboBox.Items != null)
            {
                foreach (var item in comboBox.Items)
                {
                    CheckBox checkBox = (CheckBox)item;
                    if (ComboBoxListItems.Contains(checkBox.Content.ToString() == SelectAllContain))
                    {
                        var SelectALlCheckBox = (CheckBox)ComboBoxListItems.GetItemAt(ComboBoxListItems.IndexOf(checkBox.Content.ToString() == SelectAllContain));
                        if (SelectALlCheckBox.IsChecked == true)
                        {
                            if (checkBox.IsChecked == false)
                            {
                                checkBox.IsChecked = true;
                            }
                        }
                        else
                        {
                            SelectALlCheckBox.IsChecked = true;
                            if (ComboBoxListItems.Contains(checkBox.IsChecked == false))
                            {
                                SelectALlCheckBox.IsChecked = false;
                            }
                        }
                    }
                }
            }
        }

        #region ListBox Mapping
        static ItemCollection ListBoxItems;
        static CheckBox SelectAllCheckBox;
        public static void CheckBoxsInMapping(ListBox listBox)
        {
            ListBoxItems = listBox.Items;
            foreach (CheckBox item in ListBoxItems)
            {
                if (item.Content.ToString() == SelectAllContain)
                {
                    SelectAllCheckBox = item;
                }
            }
            foreach (CheckBox item in ListBoxItems)
            {
                item.Checked += Item_Checked;
                item.Click += Item_Checked;
            }
        }
        private static void CheckAllCheckBox()
        {
            foreach (CheckBox item in ListBoxItems)
            {
                if (item.IsChecked.Value == false)
                {
                    item.IsChecked = true;
                }
            }
        }
        private static void UnCheckAllCheckBox()
        {
            foreach (CheckBox item in ListBoxItems)
            {
                if (item.IsChecked.Value == true)
                {
                    item.IsChecked = false;
                }
            }
        }
        private static void Item_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox item = (CheckBox)sender;
            if (item.Content.ToString() == SelectAllContain)
            {
                if (item.IsChecked == true)
                {
                    CheckAllCheckBox();
                }
                else
                {
                    UnCheckAllCheckBox();
                }
            }
            else
            {
                if (item.IsChecked == false)
                {
                    SelectAllCheckBox.IsChecked = false;
                }
            }
        }
        #endregion

        public static (List<string> UnCheckedItemsName, List<string> CheckedItemsName) GetUnCheckedAndCheckedItemsNameFromListOfCheckedBoxsInComboBox(ComboBox comboBox)
        {
            var unCheckedItemsName = new List<string>();
            var CheckedItemsName = new List<string>();
            var ComboBoxListItems = comboBox.Items;
            if (ComboBoxListItems != null)
            {
                foreach (var item in ComboBoxListItems)
                {
                    CheckBox checkBox = (CheckBox)item;
                    if (ComboBoxListItems.Contains(checkBox.Content.ToString() == SelectAllContain))
                    {
                        var SelectALlCheckBox = (CheckBox)ComboBoxListItems.GetItemAt(ComboBoxListItems.IndexOf(checkBox.Content.ToString() == SelectAllContain));
                        if (SelectALlCheckBox.IsChecked == true)
                        {
                            CheckedItemsName.Add(checkBox.Content.ToString());
                        }
                        else
                        {
                            if (checkBox.IsChecked == true)
                            {

                                CheckedItemsName.Add(checkBox.Content.ToString());
                            }
                            else
                            {
                                unCheckedItemsName.Add(checkBox.Content.ToString());
                            }
                        }
                    }
                    else
                    {
                        if (checkBox.IsChecked == true)
                        {

                            CheckedItemsName.Add(checkBox.Content.ToString());
                        }
                        else
                        {
                            unCheckedItemsName.Add(checkBox.Content.ToString());
                        }
                    }
                }
            }
            return (CheckedItemsName, unCheckedItemsName);
        }
        public static (List<string> UnCheckedItemsName, List<string> CheckedItemsName) GetUnCheckedAndCheckedItemsNameFromListBox(ListBox listBox)
        {
            var unCheckedItemsName = new List<string>();
            var CheckedItemsName = new List<string>();
            ListBoxItems = listBox.Items;
            if (ListBoxItems != null)
            {
                foreach (CheckBox item in ListBoxItems)
                {
                    if (item.IsChecked.Value == false)
                    {
                        if (item.Content.ToString() != SelectAllContain)
                        {
                            CheckedItemsName.Add(item.Content.ToString());
                        }
                    }
                    else
                    {
                        if (item.Content.ToString() != SelectAllContain)
                        {
                            unCheckedItemsName.Add(item.Content.ToString());
                        }
                    }
                }
            }
            return (CheckedItemsName, unCheckedItemsName);
        }
        public static void HideColumnsFromDataTable(DataTable dataTable, List<string> HiddenColumnsList)
        {
            if (HiddenColumnsList != null)
            {
                foreach (var columnName in HiddenColumnsList)
                {
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        if (col.ColumnName == columnName)
                        {
                            col.ColumnMapping = MappingType.Hidden;
                        }
                    }
                }
            }
        }
        public static DataTable GetDataTableWithHiddenColumns(DataTable dataTable, List<string> HiddenColumnsList)
        {
            DataTable NewDataTable = new DataTable();
            if (HiddenColumnsList != null)
            {
                if (dataTable.Columns != null)
                {
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        DataColumn NewColumns = new DataColumn();
                        NewColumns.ColumnName = col.ColumnName;
                        NewDataTable.Columns.Add(NewColumns);
                    }
                    if (dataTable.Rows != null)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            DataRow NewRow = NewDataTable.NewRow();
                            NewRow.ItemArray = row.ItemArray;
                            var SpacificColumnList = NewRow.ItemArray;
                            NewDataTable.Rows.Add(NewRow);
                        }
                    }
                }
                if (NewDataTable != null)
                {
                    foreach (DataColumn Hcol in NewDataTable.Columns)
                    {
                        foreach (var Lcol in HiddenColumnsList)
                        {
                            if (Hcol.ColumnName == Lcol)
                            {
                                Hcol.ColumnMapping = MappingType.Hidden;
                            }
                        }
                    }
                }
            }
            return NewDataTable;
        }
        public static void HideColumnsFromDataGrid(DataGrid dataGrid, List<string> HiddenColumnsList, List<string> ShowColumnsList)
        {
            if (HiddenColumnsList != null)
            {
                foreach (DataGridColumn col in dataGrid.Columns)
                {
                    foreach (var columnName in HiddenColumnsList)
                    {
                        if (col.Header.ToString() == columnName)
                        {
                            if (col.Visibility == System.Windows.Visibility.Visible)
                            {
                                col.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                    }
                    foreach (var ShowColumnName in ShowColumnsList)
                    {
                        if (col.Header.ToString() == ShowColumnName)
                        {
                            if (col.Visibility == System.Windows.Visibility.Hidden)
                            {
                                col.Visibility = System.Windows.Visibility.Visible;
                            }
                            if (col.Header.ToString().Trim() == "Id")
                            {
                                col.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                    }
                }
            }
        }
        public static void HideColumnsFromDataGridByColumnName(DataGrid dataGrid, string columnName)
        {
            foreach (DataGridColumn col in dataGrid.Columns)
            {
                if (col.Header.ToString() == columnName)
                {
                    if (col.Visibility == System.Windows.Visibility.Visible)
                    {
                        col.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
            }
        }
        public static void ShowColumnsFromDataGridByColumnName(DataGrid dataGrid, string columnName)
        {
            foreach (DataGridColumn col in dataGrid.Columns)
            {

                if (col.Header.ToString() == columnName)
                {
                    if (col.Visibility == System.Windows.Visibility.Hidden)
                    {
                        col.Visibility = System.Windows.Visibility.Visible;
                    }
                    if (col.Header.ToString().Trim() == "Id")
                    {
                        col.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
            }

        }
        public static DataTable GetCurrentDataTable(DataGrid dataGrid)
        {
            DataTable NewDataTable = new DataTable();
            if (dataGrid.ItemsSource != null)
            {
                NewDataTable = ((DataView)dataGrid.ItemsSource).ToTable();
            }
            return NewDataTable;
        }
        public static void HideRowsFromDataGrid(DataGrid dataGrid, string RowFilterValue, string columnName)
        {
            foreach (DataGridColumn col in dataGrid.Columns)
            {
                if (col.Header.ToString() == columnName)
                { 
                    foreach (DataRowView Rows in dataGrid.Items)
                    {
                        if (Rows.Row[col.DisplayIndex].ToString() != RowFilterValue.ToString())
                        {
                            dataGrid.SetDetailsVisibilityForItem(Rows,System.Windows.Visibility.Hidden);
                        }
                        else
                        {
                            dataGrid.SetDetailsVisibilityForItem(Rows, System.Windows.Visibility.Visible);
                        }
                    }
                }
            }

        }
        public static void MappingWithTowComboBoxs(ComboBox comboBox , string SelectedValueFromCoboBox,List<string> FillingComboBoxData)
        {
            var DataCBList = new List<string>();
            if (SelectedValueFromCoboBox == "None" || SelectedValueFromCoboBox == "Select All" || SelectedValueFromCoboBox.ToLower() == "null" || SelectedValueFromCoboBox == null)
            {
                DataCBList.Add("None");
                comboBox.ItemsSource = DataCBList;
            }
            else
            {
                DataCBList = CollectionMethods.GetListWithNoneOrSelectAll(FillingComboBoxData);
                comboBox.ItemsSource = DataCBList;
            }
        }
        public static void MappingBetweenComboBoxAndDataGrid(DataGrid dataGrid, string SelectedColumnNameFromCoboBox, string SelectedValueFromCoboBox,string tableName)
        {
            if (SelectedColumnNameFromCoboBox == "None" || SelectedColumnNameFromCoboBox == "Select All" || SelectedColumnNameFromCoboBox.ToLower() == "null" || SelectedColumnNameFromCoboBox == null || SelectedColumnNameFromCoboBox == ""
               || SelectedValueFromCoboBox == "None" || SelectedValueFromCoboBox == "Select All" || SelectedValueFromCoboBox.ToLower() == "null" || SelectedValueFromCoboBox == null || SelectedValueFromCoboBox == "")
            {
                CURD SELECTALL = new CURD();
                SELECTALL.TableName = tableName;
                var QureyString = SELECTALL.SelectAll();
                ControlsMethods.FillDataGridByDataTableFromDataBase(dataGrid, QureyString);
            }
            else
            {
                CURD SELECTALLWIHCONDITION = new CURD();
                SELECTALLWIHCONDITION.TableName = tableName;
                string CondtionString = SelectedColumnNameFromCoboBox + " = " + "'" + SelectedValueFromCoboBox + "'";
                var QureyString = SELECTALLWIHCONDITION.SelectAllWithCondtion(CondtionString);
                ControlsMethods.FillDataGridByDataTableFromDataBase(dataGrid, QureyString);
            }
        }
        public static double GetSumFromColumnInDataTable(DataGrid dataGrid,string ColumnName)
        {
            DataTable NewDataTable = ((DataView)dataGrid.ItemsSource).ToTable();
            var ColumnIndex = 0;
            ColumnIndex = NewDataTable.Columns.IndexOf(ColumnName);
            double Value = 0;
            foreach (DataRow row in NewDataTable.Rows)
            {
                var Items = row.ItemArray;
                Value += StringMethods.IsDoubleNumber(Items[ColumnIndex].ToString());
            }
            return Value;   
        }
        public static List<string> GetListOfColumnItemsFromDataTable(DataGrid dataGrid, string ColumnName)
        {
            DataTable NewDataTable = ((DataView)dataGrid.ItemsSource).ToTable();
            var ColumnIndex = 0;
            ColumnIndex = NewDataTable.Columns.IndexOf(ColumnName);
            List<string> Value = new List<string>();
            foreach (DataRow row in NewDataTable.Rows)
            {
                var Items = row.ItemArray;
                Value.Add(Items[ColumnIndex].ToString());
            }
            return Value;
        }
    }

}