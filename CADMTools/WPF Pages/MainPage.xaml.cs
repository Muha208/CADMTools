using CADMTools.DLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CADMTools.Data.DLL;
using System.Data;
using Autodesk.AutoCAD.DatabaseServices;
using DataTable = System.Data.DataTable;
using System.Web.UI.WebControls;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Diagnostics;
using DataGridColumn = System.Windows.Controls.DataGridColumn;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.ComponentModel;
using Autodesk.AutoCAD.Runtime;
using Spire.Pdf.General.Render.Font.OpenTypeFile;
using static OfficeOpenXml.FormulaParsing.EpplusExcelDataProvider;
using DataGrid = System.Windows.Controls.DataGrid;
using TextBox = System.Windows.Controls.TextBox;

namespace CADMTools.WPF_Pages
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        #region Prop
        static DataTable WallsDataTablePolyLine;
        static DataTable PlasterDataTablePolyLine;
        static DataTable CeramicDataTablePolyLine;
        static List<Entity> SelectedPolylines;
        static List<string> UnCheckedItemsNameFromFilter;
        static List<string> CheckedItemsNameFromFilter;
        static List<string> ColumnNamesList;
        static List<string> DataBAseColumnNamesList;
        static DataTable CurrentDataTable;
        static DataTable FilterdDataTable;
        static ItemCollection ListBoxItems;
        static string FilteredBySelection;
        static List<string> TotalLengthPlList;
        static List<string> WallVolumePlList;
        static List<string> WallAreaPlList;
        static List<string> UniqeStringPlList;
        static List<ObjectId> WallsPolyLinesID;
        static List<ObjectId> PlasterPolyLinesID;
        static List<ObjectId> CeramicPolyLinesID;
        static List<ObjectId> CurrentPolyLinesID;
        static string SelectedPageName;
        static string FilterResultValue;
        static string Pl_CountColName = "Pl_Count";
        static string Length_Or_ParameterColName = "Length_Or_Parameter";
        static string HeightColName = "Height";
        static string Wall_ThicknessColName = "Wall_Thickness";
        static string Wall_VolumeColName = "Wall_Volume";
        static string Wall_AreaColName = "Wall_Area";
        static string Type_MarkeColName = "Type_Mark";
        private const string Format = "{0:0.00}";
        static ObjectId objectId;
        #endregion
        #region Connecting To DataBase
        static string DataBaseName = "CADMTools";
        ConnectToDataBase connectToDataBase = new ConnectToDataBase()
        {
            DataBaseName = DataBaseName
,
            ID = @"Muha208"
,
            Password = ""
,
            TrustedConnection = true
,
            ServerName = @"muha208\sqlexpress"
,
            Enccrypt = false
        };
        #endregion
        public MainPage()
        {
            InitializeComponent();
        }
        #region Helping Methods
        public (object castClass, object ClassList) GettingTheCurrentTableClass()
        {
            WallsTable wallsTable = new WallsTable();
            object CalssList;
            switch (SelectedPageName)
            {

                case "Walls":
                    CalssList = new List<WallsTable>();
                    return (wallsTable, CalssList);
                case "Palster":
                    CalssList = new List<WallsTable>();
                    return (wallsTable, CalssList);
                case "Ceramic":
                    CalssList = new List<WallsTable>();
                    return (wallsTable, CalssList);
                default:
                    CalssList = new List<WallsTable>();
                    return (wallsTable, CalssList);

            }
        }
        public void FillingControlsAccordingPageNameChange()
        {

            //--------------Data Grid Fill
            Dg_NameOfLayers.DataContext = CurrentDataTable.DefaultView;
            UnCheckedItemsNameFromFilter = WPFControlsMethods.GetUnCheckedAndCheckedItemsNameFromListBox(Lsb_Filter).UnCheckedItemsName;
            CheckedItemsNameFromFilter = WPFControlsMethods.GetUnCheckedAndCheckedItemsNameFromListBox(Lsb_Filter).CheckedItemsName;
            WPFControlsMethods.HideColumnsFromDataGrid(Dg_NameOfLayers, UnCheckedItemsNameFromFilter, CheckedItemsNameFromFilter);
            //--------------Get Columns Name According DataTable
            ColumnNamesList = CollectionMethods.GetColumnsNameAsStringList(CurrentDataTable);
            //---------------ListBox fill
            WPFControlsMethods.FillListBoxByCheckBoxsFromListOfString(Lsb_Filter, ColumnNamesList);
            //--------------Cb_Filter Selection Fill
            WPFControlsMethods.FillComboBoxFromListOfString(Cb_FilterSelection, ColumnNamesList);
            FilteredBySelection = Cb_FilterSelection.SelectedValue.ToString();
            //---------------Cb_Filtered Result Fill By Uniqe Values According Cb_Filter Selection Fill 
            UniqeStringPlList = CollectionMethods.GetUniqeItemsListOfColumnFromDataTable(CurrentDataTable, FilteredBySelection);
            WPFControlsMethods.FillComboBoxFromListOfString(Cb_FilteredResult, UniqeStringPlList);
            //---------------Get Checked And UnChecked Items Name From Filter
            CheckedItemsNameFromFilter = WPFControlsMethods.GetUnCheckedAndCheckedItemsNameFromListBox(Lsb_Filter).CheckedItemsName;
            UnCheckedItemsNameFromFilter = WPFControlsMethods.GetUnCheckedAndCheckedItemsNameFromListBox(Lsb_Filter).UnCheckedItemsName;
            switch (SelectedPageName)
            {
                case "Walls":
                    TotalLengthPlList = CollectionMethods.GetListOfColumnItemsFromDataTableOneColumnName(CurrentDataTable, "Length - Parameter");
                    Tx_Total_Length.Text = CollectionMethods.GetNumberListFromStringList(TotalLengthPlList).Sum().ToString("0.00");
                    Tx_Wall_Thickness.Text = Cb_FilteredResult.Text;
                    WallVolumePlList = CollectionMethods.GetListOfColumnItemsFromDataTableOneColumnName(CurrentDataTable, "Wall Volume");
                    Tx_Total_Volume.Text = CollectionMethods.GetNumberListFromStringList(WallVolumePlList).Sum().ToString("0.00");
                    break;
                case "Palster":
                    CurrentDataTable = PlasterDataTablePolyLine;
                    break;
                case "Ceramic":
                    CurrentDataTable = CeramicDataTablePolyLine;
                    break;
                default:
                    break;

            }
        }
        public void FillingControlsAccordingResultOfFilter()
        {
            FilterResultValue = Cb_FilteredResult.SelectedValue.ToString();
            FilterdDataTable = CollectionMethods.GetFilteredDataTable(SwitchTableAccordingPageName().CurrentDatatable, FilteredBySelection, FilterResultValue);
            if (FilterResultValue != "None")
            {
                //--------------Data Grid Fill
                WPFControlsMethods.HideRowsFromDataGrid(Dg_NameOfLayers, FilterResultValue, FilteredBySelection);
                switch (SelectedPageName)
                {
                    case "Walls":
                        TotalLengthPlList = CollectionMethods.GetListOfColumnItemsFromDataTableOneColumnName(FilterdDataTable, "Length - Parameter");
                        Tx_Total_Length.Text = CollectionMethods.GetNumberListFromStringList(TotalLengthPlList).Sum().ToString("0.00");
                        Tx_Wall_Thickness.Text = Cb_FilteredResult.Text;
                        WallVolumePlList = CollectionMethods.GetListOfColumnItemsFromDataTableOneColumnName(FilterdDataTable, "Wall Volume");
                        Tx_Total_Volume.Text = CollectionMethods.GetNumberListFromStringList(WallVolumePlList).Sum().ToString("0.00");
                        break;
                    case "Palster":
                        CurrentDataTable = PlasterDataTablePolyLine;
                        break;
                    case "Ceramic":
                        CurrentDataTable = CeramicDataTablePolyLine;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Dg_NameOfLayers.DataContext = CurrentDataTable.DefaultView;
                Dg_NameOfLayers.Items.Refresh();
            }
        }
        private (DataTable CurrentDatatable, List<ObjectId> CurrentIdList) SwitchTableAccordingPageName()
        {
            WallsDataTablePolyLine = CADMethods.GetDataTableOfPolyLinesFromSelectionByListOfColumn(SelectedPolylines, CADMethods.TableType.WallsTable).dataTable;
            PlasterDataTablePolyLine = CADMethods.GetDataTableOfPolyLinesFromSelectionByListOfColumn(SelectedPolylines, CADMethods.TableType.PlasterTable).dataTable;
            CeramicDataTablePolyLine = CADMethods.GetDataTableOfPolyLinesFromSelectionByListOfColumn(SelectedPolylines, CADMethods.TableType.CeramicTable).dataTable;

            WallsPolyLinesID = CADMethods.GetDataTableOfPolyLinesFromSelectionByListOfColumn(SelectedPolylines, CADMethods.TableType.WallsTable).objectIdList;
            PlasterPolyLinesID = CADMethods.GetDataTableOfPolyLinesFromSelectionByListOfColumn(SelectedPolylines, CADMethods.TableType.PlasterTable).objectIdList;
            CeramicPolyLinesID = CADMethods.GetDataTableOfPolyLinesFromSelectionByListOfColumn(SelectedPolylines, CADMethods.TableType.CeramicTable).objectIdList;
            switch (SelectedPageName)
            {
                case "Walls":
                    CurrentDataTable = WallsDataTablePolyLine;
                    CurrentPolyLinesID = WallsPolyLinesID;
                    break;
                case "Palster":
                    CurrentDataTable = PlasterDataTablePolyLine;
                    CurrentPolyLinesID = PlasterPolyLinesID;
                    break;
                case "Ceramic":
                    CurrentDataTable = CeramicDataTablePolyLine;
                    CurrentPolyLinesID = CeramicPolyLinesID;
                    break;
                default:
                    break;
            }
            return (CurrentDataTable, CurrentPolyLinesID);
        }
        #endregion
        private void Bt_Selection_PolyLine_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageName = WPFControlsMethods.GetTabNameAccordingSelection(TabC_Pages);

            #region Get List Of Columns Name 
            var TableToGet = GettingTheCurrentTableClass().castClass;
            var ListClass = GettingTheCurrentTableClass().ClassList;
            CURD CREATTABLE = new CURD();
            CREATTABLE.TableName = SelectedPageName;
            DataBAseColumnNamesList = ClassMethods.GetListOfColumnNamesFromClass(TableToGet).ColumnsNameForDataBase;
            ColumnNamesList = ClassMethods.GetListOfColumnNamesFromClass(TableToGet).ColumnsName;
            #endregion

            #region Insert Data From CAD In DataBase
            this.WindowState = WindowState.Minimized;
            SelectedPolylines = CADMethods.GetListOfSelectedPolylines();

            CURD TruncateDataBaseTable = new CURD();
            TruncateDataBaseTable.TableName = SelectedPageName;
            var TruncateCommand = TruncateDataBaseTable.TruncateTable();
            ConnectToDataBase.ExcuteCommand(TruncateCommand);

            CURD INSERTDataFromSelectionInDataBaseTable = new CURD();
            INSERTDataFromSelectionInDataBaseTable.TableName = SelectedPageName;
            var ListOfRowsValules = CADMethods.GetRowsDataListOfPolyLinesFromSelectionByListOfColumn(SelectedPolylines, SelectedPageName).RowsDataList;
            CurrentPolyLinesID = SwitchTableAccordingPageName().CurrentIdList;
            var INSERTString = INSERTDataFromSelectionInDataBaseTable.InsertData(ListOfRowsValules, DataBAseColumnNamesList);
            ConnectToDataBase.ExcuteCommand(INSERTString);
            this.WindowState = WindowState.Normal;
            #endregion

            #region Fill The DataGrid From DataBase
            CURD SELECTALLFROMTable = new CURD();
            SELECTALLFROMTable.TableName = SelectedPageName;
            ControlsMethods.FillDataGridByDataTableFromDataBase(Dg_NameOfLayers, SELECTALLFROMTable.SelectAll());
            #endregion

            #region Fill Volum And Area
            WallVolumePlList = MathematicsMethods.GetVolumeListFromDataBase(SelectedPageName, Pl_CountColName, HeightColName, Length_Or_ParameterColName, Wall_ThicknessColName).Volume;
            WallAreaPlList = MathematicsMethods.GetVolumeListFromDataBase(SelectedPageName, Pl_CountColName, HeightColName, Length_Or_ParameterColName, Wall_ThicknessColName).Area;
            CURD VolumeColumnFill = new CURD();
            VolumeColumnFill.TableName = SelectedPageName;
            var VolumeFillCommand = VolumeColumnFill.UpdateColumnByIdColumn(Wall_VolumeColName, WallVolumePlList, CurrentPolyLinesID);
            ConnectToDataBase.ExcuteCommand(VolumeFillCommand);
            CURD AreaColumnFill = new CURD();
            AreaColumnFill.TableName = SelectedPageName;
            var AreaFillCommand = AreaColumnFill.UpdateColumnByIdColumn(Wall_AreaColName, WallAreaPlList, CurrentPolyLinesID);
            ConnectToDataBase.ExcuteCommand(AreaFillCommand);
            Tx_Total_Volume.Text = WPFControlsMethods.GetSumFromColumnInDataTable(Dg_NameOfLayers, Wall_VolumeColName).ToString();
            #endregion

            #region Fill Class With Values
            var ListOFClassWithValues = ClassMethods.FillClassPropertiesWithValue(TableToGet, ListOfRowsValules);
            #endregion

            #region Fill ComboBox (Filtered By - Result)
            //--------------Cb_Filter Selection Fill
            Cb_FilterSelection.ItemsSource = CollectionMethods.GetListWithNoneOrSelectAll(ColumnNamesList);
            Cb_FilterSelection.SelectedIndex = 0;
            FilteredBySelection = Cb_FilterSelection.Text.Replace(" ", "_");
            //---------------Cb_Filtered Result Fill By Uniqe Values According Cb_Filter Selection Fill 
            CURD SELECTDISTICTForResultCB = new CURD();
            SELECTDISTICTForResultCB.TableName = SelectedPageName;
            WPFControlsMethods.MappingWithTowComboBoxs(Cb_FilteredResult, FilteredBySelection, ControlsMethods.GetListFromQueryFromDataBase(SELECTDISTICTForResultCB.SelectDistictColumnFromOneTable(FilteredBySelection)));
            #endregion

            #region Fill ListBox
            //---------------ListBox fill
            WPFControlsMethods.FillListBoxByCheckBoxsFromListOfString(Lsb_Filter, ColumnNamesList);
            #endregion
        }
        #region Tools Bar
        private void Bt_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Bt_Min_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                UIElement uIElement = sender as UIElement;  
                uIElement.AllowDrop = true;
                this.AllowDrop = true;
                this.DragMove();
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.AllowDrop = true;
                this.DragMove();
            }
        }
        #endregion
        private void Cb_FilteredResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cb_FilteredResult.SelectedItem == null)
            {
                WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.Text, SelectedPageName);
            }
            else
            {
                WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.SelectedItem.ToString(), SelectedPageName);
            }
            Dg_NameOfLayers.Items.Refresh();
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            WPFControlsMethods.CheckBoxsInMapping(Lsb_Filter);
            CheckedItemsNameFromFilter = WPFControlsMethods.GetUnCheckedAndCheckedItemsNameFromListBox(Lsb_Filter).CheckedItemsName;
            CURD SELECTCheckedColumnsOnly = new CURD();
            SELECTCheckedColumnsOnly.TableName = SelectedPageName;
            ControlsMethods.FillDataGridByDataTableFromDataBase(Dg_NameOfLayers, SELECTCheckedColumnsOnly.SelectColumnsFromOneTable(CheckedItemsNameFromFilter));

        }
        private void Lsb_Filter_MouseMove(object sender, MouseEventArgs e)
        {
            ListBoxItems = Lsb_Filter.Items;
            if (ListBoxItems != null)
            {
                foreach (var item in ListBoxItems)
                {
                    CheckBox checkBox = (CheckBox)item;
                    if (checkBox.IsChecked == true)
                    {
                        checkBox.Checked += CheckBox_Checked;
                        checkBox.Unchecked += CheckBox_Checked;
                    }
                }
            }
        }
        private void Cb_FilterSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //---------------Cb_Filtered Result Fill By Uniqe Values According Cb_Filter Selection Fill 
            FilteredBySelection = Cb_FilterSelection.SelectedValue.ToString().Replace(" ", "_");
            CURD SELECTDISTICTForResultCB = new CURD();
            SELECTDISTICTForResultCB.TableName = SelectedPageName;
            WPFControlsMethods.MappingWithTowComboBoxs(Cb_FilteredResult, FilteredBySelection, ControlsMethods.GetListFromQueryFromDataBase(SELECTDISTICTForResultCB.SelectDistictColumnFromOneTable(FilteredBySelection)));
            //---------------Data Grid Change
            Cb_FilteredResult.SelectedIndex = 0;
            WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.SelectedItem.ToString(), SelectedPageName);
            Dg_NameOfLayers.Items.Refresh();
        }

        private void TabC_Pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPageName = WPFControlsMethods.GetTabNameAccordingSelection(TabC_Pages);
            SwitchTableAccordingPageName();
            Dg_NameOfLayers.Items.Refresh();
        }

        private void Dg_NameOfLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dg_NameOfLayers.SelectedCells != null)
            {
                foreach (var cellInfo in Dg_NameOfLayers.SelectedCells)
                {
                    switch (SelectedPageName)
                    {
                        case "Walls":
                            if (cellInfo.IsValid)
                            {
                                var content = cellInfo.Column.GetCellContent(cellInfo.Item);
                                if (content != null)
                                {
                                    ObjectId PreviousObjectId;
                                    if (objectId != ObjectId.Null)
                                    {
                                        PreviousObjectId = objectId;
                                    }
                                    else
                                    {
                                        PreviousObjectId = ObjectId.Null;
                                    }
                                    //get the datacontext from FrameworkElement and typecast to DataRowView
                                    var row = (DataRowView)content.DataContext;
                                    //ItemArray returns an object array with single element
                                    var Rows = row.Row.ItemArray;
                                    var ID = Rows[1].ToString();

                                    CURD SELECTID = new CURD();
                                    SELECTID.TableName = SelectedPageName;
                                    ControlsMethods.FillLableAccordingToQueryFromDataBase(Lb_PolylineID, SELECTID.SelectColumnWHEREFromOneTable("Id", $"Id = '{ID}'"));
                                    objectId = CADMethods.GetObjectId(ID, CurrentPolyLinesID);
                                    ControlsMethods.FillLableAccordingToQueryFromDataBase(Lb_PolylineWidth, SELECTID.SelectColumnWHEREFromOneTable(Wall_ThicknessColName, $"Id = '{ID}'"));

                                    if (PreviousObjectId != ObjectId.Null)
                                    {
                                        CADMethods.HighlightPolyLine(PreviousObjectId, false);
                                    }
                                    CADMethods.HighlightPolyLine(objectId);
                                }
                            }
                            break;
                        case "Palster":
                            break;
                        case "Ceramic":
                            break;
                        default:
                            break;

                    }
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Connecting To Database
            //1- Open The Connecton (SQL)
            connectToDataBase.Connect();
            //2- Put The Connection Status In Lable To Show It
            Lb_Log.Text = connectToDataBase.ConnectionState(); ;
            #endregion
            SelectedPageName = WPFControlsMethods.GetTabNameAccordingSelection(TabC_Pages);
        }
        private void Bt_PolyLinesTypeMark_Click(object sender, RoutedEventArgs e)
        {
            CURD SELECTColumn = new CURD();
            SELECTColumn.TableName = SelectedPageName;
            var SELECTCOlCommand = SELECTColumn.SelectColumnFromOneTable(Type_MarkeColName);
            var TypeMArkList = ControlsMethods.GetListFromQueryFromDataBase(SELECTCOlCommand);
            int count = 0;
            foreach (var objectId in CurrentPolyLinesID)
            {
                CADMethods.CreateTextAlongPolyLine(objectId, TypeMArkList[count]);
                count++;
            }
        }
        private void Dg_NameOfLayers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                FrameworkElement content = e.EditingElement;
                if (e.Column.Header.ToString() == Wall_ThicknessColName)
                {

                    if (content != null)
                    {
                        var VTextBlock = (TextBox)content;
                        var dataGrid = (DataGrid)sender;
                        DataRowView row = (DataRowView)dataGrid.SelectedItem;
                        var Rows = row.Row.ItemArray;
                        var ID = Rows[1].ToString();

                        CURD UPDATEWallThicknessCol = new CURD();
                        UPDATEWallThicknessCol.TableName = SelectedPageName;
                        var UpdateCommand = UPDATEWallThicknessCol.UpdateColumnWithCondition(Wall_ThicknessColName, $"{VTextBlock.Text}", $"Id = '{ID}'");
                        ConnectToDataBase.ExcuteCommand(UpdateCommand);

                        //---------------Cb_Filtered Result Fill By Uniqe Values According Cb_Filter Selection Fill 
                        CURD SELECTDISTICTForResultCB = new CURD();
                        SELECTDISTICTForResultCB.TableName = SelectedPageName;
                        WPFControlsMethods.MappingWithTowComboBoxs(Cb_FilteredResult, FilteredBySelection, ControlsMethods.GetListFromQueryFromDataBase(SELECTDISTICTForResultCB.SelectDistictColumnFromOneTable(FilteredBySelection)));

                        WallVolumePlList = MathematicsMethods.GetVolumeListFromDataBase(SelectedPageName, Pl_CountColName, HeightColName, Length_Or_ParameterColName, Wall_ThicknessColName).Volume;
                        WallAreaPlList = MathematicsMethods.GetVolumeListFromDataBase(SelectedPageName, Pl_CountColName, HeightColName, Length_Or_ParameterColName, Wall_ThicknessColName).Area;
                        CURD VolumeColumnFill = new CURD();
                        VolumeColumnFill.TableName = SelectedPageName;
                        var VolumeFillCommand = VolumeColumnFill.UpdateColumnByIdColumn(Wall_VolumeColName, WallVolumePlList, CurrentPolyLinesID);
                        ConnectToDataBase.ExcuteCommand(VolumeFillCommand);
                        CURD AreaColumnFill = new CURD();
                        AreaColumnFill.TableName = SelectedPageName;
                        var AreaFillCommand = AreaColumnFill.UpdateColumnByIdColumn(Wall_AreaColName, WallAreaPlList, CurrentPolyLinesID);
                        ConnectToDataBase.ExcuteCommand(AreaFillCommand);

                        //---------------Data Grid Change
                        var CheckValue = Cb_FilteredResult.Items.Contains(VTextBlock.Text);
                        if (CheckValue)
                        {
                            Cb_FilteredResult.SelectedItem = VTextBlock.Text;
                            WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.SelectedItem.ToString(), SelectedPageName);

                        }
                        else
                        {
                            Cb_FilteredResult.SelectedIndex = 0;
                            WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.SelectedItem.ToString(), SelectedPageName);
                        }

                        Tx_Total_Volume.Text = WPFControlsMethods.GetSumFromColumnInDataTable(Dg_NameOfLayers, Wall_VolumeColName).ToString();
                    }
                }
                else if (e.Column.Header.ToString() == Type_MarkeColName)
                {
                    if (content != null)
                    {
                        var VTextBlock = (TextBox)content;
                        var dataGrid = (DataGrid)sender;
                        DataRowView row = (DataRowView)dataGrid.SelectedItem;
                        var Rows = row.Row.ItemArray;
                        var ID = Rows[1].ToString();

                        CURD UPDATETypeMarkCol = new CURD();
                        UPDATETypeMarkCol.TableName = SelectedPageName;
                        var UpdateCommand = UPDATETypeMarkCol.UpdateColumnWithCondition(Type_MarkeColName, $"{VTextBlock.Text}", $"Id = '{ID}'");
                        ConnectToDataBase.ExcuteCommand(UpdateCommand);

                        //---------------Cb_Filtered Result Fill By Uniqe Values According Cb_Filter Selection Fill 
                        CURD SELECTDISTICTForResultCB = new CURD();
                        SELECTDISTICTForResultCB.TableName = SelectedPageName;
                        WPFControlsMethods.MappingWithTowComboBoxs(Cb_FilteredResult, FilteredBySelection, ControlsMethods.GetListFromQueryFromDataBase(SELECTDISTICTForResultCB.SelectDistictColumnFromOneTable(FilteredBySelection)));

                        //---------------Data Grid Change
                        if (FilteredBySelection == Type_MarkeColName)
                        {
                            var CheckValue = Cb_FilteredResult.Items.Contains(VTextBlock.Text);
                            if (CheckValue)
                            {
                                Cb_FilteredResult.SelectedItem = VTextBlock.Text;
                                WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.SelectedItem.ToString(), SelectedPageName);

                            }
                            else
                            {
                                Cb_FilteredResult.SelectedIndex = 0;
                                WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.SelectedItem.ToString(), SelectedPageName);
                            }
                        }
                    }
                }
            }
            e.Cancel = true;
        }

        private void Bt_AutoTypeMark_Click(object sender, RoutedEventArgs e)
        {
            CollectionMethods.AutomaticTypeMarkUpdate(SelectedPageName, Length_Or_ParameterColName, Wall_ThicknessColName, "Id", Type_MarkeColName);
            if (Cb_FilteredResult.SelectedItem == null)
            {
                WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.Text, SelectedPageName);
            }
            else
            {
                WPFControlsMethods.MappingBetweenComboBoxAndDataGrid(Dg_NameOfLayers, FilteredBySelection, Cb_FilteredResult.SelectedItem.ToString(), SelectedPageName);
            }
            Dg_NameOfLayers.Items.Refresh();
        }

        private void Bt_JoinPolyLines_Click(object sender, RoutedEventArgs e)
        {
            CADMethods.JoinPolyLinesByThereIds(CurrentPolyLinesID);

        }
    }
}
