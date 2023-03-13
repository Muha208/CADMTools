using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Windows.Data;
using Autodesk.AutoCAD.Windows.Features.PointCloud.PointCloudColorMapping;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Spire.Pdf.Graphics;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.WebSockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using static CADMTools.DLL.CADMethods;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using DataColumn = System.Data.DataColumn;
using DataTable = System.Data.DataTable;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace CADMTools.DLL
{
    public static class CADMethods
    {
        public enum TableType
        {
            WallsTable,
            PlasterTable,
            CeramicTable,
        }

        static Document docAP = Application.DocumentManager.MdiActiveDocument;
        static Database database = Application.DocumentManager.MdiActiveDocument.Database;
        static Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

        static List<double> ListOfVolumeValues = new List<double>();

        static Dictionary<int, string> WallsTable = new Dictionary<int, string>(){
            { 0, "No" },
            { 1, "Id"},
            { 2, "Type Mark"},
            { 3, "Count"},
            { 4, "Wall Thickness" },
            { 5, "Length Or Parameter" },
            { 6, "No Of Vertics" },
            { 7, "Height"},
            { 8, "Wall Area"},
            { 9, "Wall Volume"}
        };
        static Dictionary<int, string> PlasterTable = new Dictionary<int, string>(){
            { 0, "No" },
            { 1, "Id"},
            { 2, "Type Mark"},
            { 3, "Count"},
            { 4, "Parameter" },
            { 5, "Height" },
            { 6, "No Of Vertics" },
            { 7, "Is Closed"},
            { 8, "Wall Area"},
        };
        static Dictionary<int, string> CeramicTable = new Dictionary<int, string>(){
            { 0, "No" },
            { 1, "Id"},
            { 2, "Type Mark"},
            { 3, "Count"},
            { 4, "Parameter" },
            { 5, "Overalls Height" },
            { 6, "No Of Vertics" },
            { 7, "Is Closed"},
            { 8, "Floor Area"},
            { 9, "Overalls Area"},
            { 10, "Height Area"},
            { 11, "Wall Area"},
        };
        public static List<double> GetWallVolume(DataTable dataTable)
        {
            List<string> Count = new List<string>();
            List<string> Thickness = new List<string>();
            List<string> Length = new List<string>();
            List<string> Height = new List<string>();
            List<double> WallVolume = new List<double>();
            DataRowCollection RowCollection = dataTable.Rows;
            foreach (DataRow row in RowCollection)
            {
                Count.Add(row[2].ToString());
                Thickness.Add(row[3].ToString());
                Length.Add(row[4].ToString());
                Height.Add(row[6].ToString());

            }
            var MaxListCount = Math.Max(Count.Count, Math.Max(Thickness.Count, Math.Max(Height.Count, Length.Count)));
            {
                for (int i = 0; i < MaxListCount; i++)
                {
                    if (Height[i] != null || Thickness[i] != null || Count[i] != null || Length[i] != null)
                    {
                        double DHeight = StringMethods.IsDoubleNumber(Height[i]);
                        double DThickness = StringMethods.IsDoubleNumber(Thickness[i]);
                        double DCount = StringMethods.IsDoubleNumber(Count[i]);
                        double DLength = StringMethods.IsDoubleNumber(Length[i]);
                        WallVolume.Add(DHeight * DThickness * DCount * DLength);
                    }
                    else
                    {
                        WallVolume.Add(0);
                    }
                }
            }
            return WallVolume;
        }
        public static void FillVolumeColumnInDataTableByValues(DataTable dataTable)
        {
            ListOfVolumeValues = GetWallVolume(dataTable);
            DataRowCollection RowCollection = dataTable.Rows;
            foreach (DataRow row in RowCollection)
            {
                for (int i = 0; i < ListOfVolumeValues.Count; i++)
                {
                    row[9] = ListOfVolumeValues[i];           
                }
            }
        }
        public static List<Entity> GetListOfSelectedElements()
        {
            var EntityList = new List<Entity>();
            using (Transaction SelectItems = database.TransactionManager.StartTransaction())
            {
                docAP.LockDocument();
                PromptSelectionResult SelectionPrompt = docAP.Editor.GetSelection();
                // If the prompt status is OK, objects were selected
                if (SelectionPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet NewSelection = SelectionPrompt.Value;
                    // Step through the objects in the selection set
                    foreach (SelectedObject SelectedItem in NewSelection)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (SelectedItem != null)
                        {
                            // Open the selected object for write
                            Entity ItemReadyToWrite = SelectItems.GetObject(SelectedItem.ObjectId,
                                                             OpenMode.ForWrite) as Entity;
                            EntityList.Add(ItemReadyToWrite);
                        }
                    }
                }
                return EntityList;
            }
        }
        public static List<Entity> GetListOfSelectedPolylines()
        {
            var EntityList = new List<Entity>();
            using (Transaction SelectPolyLines = database.TransactionManager.StartTransaction())
            {
                docAP.LockDocument();
                var CommandMassege = new PromptSelectionOptions();
                TypedValue[] typedValue = new TypedValue[1];
                var FilterOfPoly = DxfFiler.GetClass(typeof(Polyline));

                typedValue.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
                SelectionFilter plselectionFilter = new SelectionFilter(typedValue);
                PromptSelectionResult SelectionPrompt = docAP.Editor.GetSelection(CommandMassege, plselectionFilter);
                // If the prompt status is OK, objects were selected
                if (SelectionPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet NewSelection = SelectionPrompt.Value;
                    // Step through the objects in the selection set
                    foreach (SelectedObject SelectedItem in NewSelection)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (SelectedItem != null)
                        {
                            // Open the selected object for write
                            Entity ItemReadyToWrite = SelectPolyLines.GetObject(SelectedItem.ObjectId,
                                                             OpenMode.ForWrite) as Entity;
                            EntityList.Add(ItemReadyToWrite);
                        }
                    }
                }
                return EntityList;
            }
        }
        public static List<string> GetListOfIDElementsFromSelection(List<Entity> EntityListFromSelection)
        {
            List<string> IDList = new List<string>();
            using (Transaction BlocksName = database.TransactionManager.StartTransaction())
            {
                docAP.LockDocument();
                foreach (var entity in EntityListFromSelection)
                {
                    IDList.Add(entity.Id.ToString());
                }
                return IDList;
            }
        }
        public static DataTable GeDataTableOfLineDataFromSelection(List<Entity> EntityListFromSelection)
        {
            DataTable itemsdataTable = new DataTable();
            using (Transaction GetSelectedItemsInfo = database.TransactionManager.StartTransaction())
            {
                int count = 1;
                docAP.LockDocument();
                itemsdataTable.Columns.Add("No");
                itemsdataTable.Columns.Add("Line Type");
                itemsdataTable.Columns.Add("Line Scale");
                itemsdataTable.Columns.Add("Line Weight");
                itemsdataTable.Columns.Add("Line Value");
                itemsdataTable.Columns.Add("Line Color");
                foreach (var entity in EntityListFromSelection)
                {
                    itemsdataTable.Rows.Add(count, entity.Linetype
                        , entity.LinetypeScale.ToString(), entity.LineWeight.ToString()
                        , entity.Bounds.Value, entity.Color.ColorNameForDisplay);
                    count++;
                }
                GetSelectedItemsInfo.Commit();
            }
            return itemsdataTable;
        }
        public static DataTable GetPolyLineFromSelection()
        {
            var CommandMassege = new PromptEntityOptions("\nSelect Polyline: ");
            CommandMassege.SetRejectMessage("\nThere is No Polyline Selected.");
            CommandMassege.AddAllowedClass(typeof(Polyline), true);
            DataTable plinedataTable = new DataTable();
            var result = editor.GetEntity(CommandMassege);
            if (result.Status == PromptStatus.OK)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction GetPolyLineInfo = database.TransactionManager.StartTransaction())
                {
                    int count = 1;
                    docAP.LockDocument();
                    var pline = (Polyline)GetPolyLineInfo.GetObject(result.ObjectId, OpenMode.ForWrite);
                    plinedataTable.Columns.Add("No");
                    plinedataTable.Columns.Add("PolyLine Type");
                    plinedataTable.Columns.Add("PolyLine Constant Width");
                    plinedataTable.Columns.Add("PolyLine Length");
                    plinedataTable.Columns.Add("PolyLine NumberOfVertices");
                    plinedataTable.Columns.Add("PolyLine Closed");
                    plinedataTable.Columns.Add("PolyLine Area");

                    plinedataTable.Rows.Add(count
                        , pline.Linetype
                        , pline.ConstantWidth
                        , pline.Length
                        , pline.NumberOfVertices.ToString()
                        , pline.Closed.ToString()
                        , pline.Area);

                    GetPolyLineInfo.Commit();
                }
                var DataOfRowsinDataTable = plinedataTable.Rows;
            }
            return plinedataTable;
        }
        public static (DataTable dataTable, List<ObjectId> objectIdList) GetDataTableOfPolyLinesFromSelectionByListOfColumn(List<Entity> EntityListFromSelection, TableType tableType)
        {
            List<ObjectId> objectIds = new List<ObjectId>();
            DataTable plinedataTable = new DataTable();
            int count = 1;
            var ColumnsNameList = new List<string>();
            switch (tableType)
            {
                case TableType.WallsTable:
                    ColumnsNameList = WallsTable.Values.ToList();
                    if (ColumnsNameList != null)
                    {
                        foreach (var colName in ColumnsNameList)
                        {
                            plinedataTable.Columns.Add(colName);
                        }
                    }
                    break;
                case TableType.PlasterTable:
                    ColumnsNameList = PlasterTable.Values.ToList();
                    if (ColumnsNameList != null)
                    {
                        foreach (var colName in ColumnsNameList)
                        {
                            plinedataTable.Columns.Add(colName);
                        }
                    }
                    break;
                case TableType.CeramicTable:
                    ColumnsNameList = CeramicTable.Values.ToList();
                    if (ColumnsNameList != null)
                    {
                        foreach (var colName in ColumnsNameList)
                        {
                            plinedataTable.Columns.Add(colName);
                        }
                    }
                    break;
                default:
                    break;
            }
            if (EntityListFromSelection != null)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction GetPolyLinesInfo = database.TransactionManager.StartTransaction())
                {
                    docAP.LockDocument();
                    foreach (var Element in EntityListFromSelection)
                    {
                        if (Element.GetType() == typeof(Polyline))
                        {
                            var pline = (Polyline)Element;
                            switch (tableType)
                            {
                                case TableType.WallsTable:
                                    plinedataTable.Rows.Add(count.ToString()
                                    , pline.Id.ToString().Replace("(", "").Replace(")", "").Trim()
                                    , ""
                                    , "1"
                                    , pline.ConstantWidth.ToString()
                                    , pline.Length.ToString()
                                    , pline.NumberOfVertices.ToString()
                                    , "3"
                                    , ""
                                    , "");
                                    objectIds.Add(pline.Id);
                                    break;
                                case TableType.PlasterTable:
                                    plinedataTable.Rows.Add(count.ToString()
                                    , pline.Id.ToString().Replace("(", "").Replace(")", "").Trim()
                                    , ""
                                    , "1"
                                    , pline.Length.ToString()
                                    , "3"
                                    , pline.NumberOfVertices.ToString()
                                    , pline.Closed.ToString()
                                    , "");
                                    objectIds.Add(pline.Id);
                                    break;
                                case TableType.CeramicTable:
                                    double AreaOfPolyClosedLine = 0;
                                    if (pline.Closed)
                                    {
                                        AreaOfPolyClosedLine = pline.Area;
                                    }
                                    plinedataTable.Rows.Add(count.ToString()
                                    , pline.Id.ToString().Replace("(", "").Replace(")", "").Trim()
                                    , ""
                                    , "1"
                                    , pline.Length.ToString()
                                    , ".7"
                                    , pline.NumberOfVertices.ToString()
                                    , pline.Closed.ToString()
                                    , AreaOfPolyClosedLine.ToString()
                                    , ""
                                    , "3"
                                    , "");
                                    objectIds.Add(pline.Id);
                                    break;
                                default:
                                    break;
                            }
                            count++;
                        }
                    }
                    switch (tableType)
                    {
                        case TableType.WallsTable:
                            FillVolumeColumnInDataTableByValues(plinedataTable);
                            break;
                        case TableType.PlasterTable:

                            break;
                        case TableType.CeramicTable:

                            break;
                        default:
                            break;
                    }
                    GetPolyLinesInfo.Commit();
                }
            }
            return (plinedataTable, objectIds);
        }
        /// <summary>
        /// Get The Layers Name
        /// </summary>
        /// <returns>List Of Layers Name</returns>
        public static List<string> GetTheListOfLayerName()
        {
            List<string> LayerNamesList = new List<string>();
            using (Transaction GetTheListOfLayer = database.TransactionManager.StartTransaction())
            {
                LayerTable LayTable = GetTheListOfLayer.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (var Lay in LayTable)
                {
                    docAP.LockDocument();
                    LayerTableRecord layerTableRecord = GetTheListOfLayer.GetObject(Lay, OpenMode.ForRead) as LayerTableRecord;
                    LayerNamesList.Add(layerTableRecord.Name);
                }
            }
            return LayerNamesList;
        }
        /// <summary>
        /// Need To Check
        /// </summary>
        public static void ClearTheAllListOfLayers()
        {
            using (Transaction GetTheListOfLayer = database.TransactionManager.StartTransaction())
            {
                LayerTable LayTable = GetTheListOfLayer.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (var Lay in LayTable)
                {
                    docAP.LockDocument();
                    LayerTableRecord layerTableRecord = GetTheListOfLayer.GetObject(Lay, OpenMode.ForRead) as LayerTableRecord;
                    LayTable.UpgradeOpen();
                    var LayerName = layerTableRecord.Name;
                    if (LayerName != "0")
                    {
                        layerTableRecord.UpgradeOpen();
                        layerTableRecord.Erase(true);
                    }
                }
            }
        }
        public static void SetTheListOfLayerName(List<string> List)
        {
            List.Add("GridLines");
            using (Transaction SetTheListOfLayers = database.TransactionManager.StartTransaction())
            {
                LayerTable LayTable = SetTheListOfLayers.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (var item in List)
                {
                    if (!LayTable.Has(item))
                    {
                        docAP.LockDocument();
                        LayTable.UpgradeOpen();
                        LayerTableRecord layerTableRecord = new LayerTableRecord();
                        layerTableRecord.Name = item;
                        layerTableRecord.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                        LayTable.Add(layerTableRecord);
                        SetTheListOfLayers.AddNewlyCreatedDBObject(layerTableRecord, true);
                    }
                }
                SetTheListOfLayers.Commit();
            }
        }
        public static void ChangePolylineConstantWidth(ObjectId ID, double constantWidth)
        {
            using (Transaction SelectPolyLine = database.TransactionManager.StartOpenCloseTransaction())
            {
                docAP.LockDocument();
                var entity = SelectPolyLine.GetObject(ID, OpenMode.ForWrite) as Entity;
                var polyline = (Polyline)entity;
                polyline.ConstantWidth = constantWidth;
                SelectPolyLine.Commit();
            }
        }
        public static ObjectId GetObjectId(string idAsString, List<ObjectId> objectIdList)
        {
            ObjectId objectId = ObjectId.Null;
            foreach (var ObjId in objectIdList)
            {
                if (ObjId.ToString().Replace("(", "").Replace(")", "").Trim() == idAsString)
                {
                    objectId = ObjId;
                }
            }
            return objectId;
        }
        public static (List<List<string>> RowsDataList, List<ObjectId> objectIdList) GetRowsDataListOfPolyLinesFromSelectionByListOfColumn(List<Entity> EntityListFromSelection, string PageName)
        {
            List<ObjectId> objectIds = new List<ObjectId>();
            var RowsDataList = new List<List<string>>();
            int count = 1;
            if (EntityListFromSelection != null)
            {
                // at this point we know an entity have been selected and it is a Polyline
                using (Transaction GetPolyLinesInfo = database.TransactionManager.StartTransaction())
                {
                    docAP.LockDocument();
                    foreach (var Element in EntityListFromSelection)
                    {
                        if (Element.GetType() == typeof(Polyline))
                        {
                            var pline = (Polyline)Element;
                            var dataRow = new List<string>();
                            switch (PageName)
                            {
                                case "Walls":
                                    dataRow.Add(count.ToString());
                                    dataRow.Add(pline.Id.ToString().Replace("(", "").Replace(")", "").Trim());
                                    dataRow.Add("");
                                    dataRow.Add("1");
                                    dataRow.Add(pline.ConstantWidth.ToString());
                                    dataRow.Add(pline.Length.ToString());
                                    dataRow.Add(pline.NumberOfVertices.ToString());
                                    dataRow.Add("3");
                                    dataRow.Add("0");
                                    dataRow.Add("0");
                                    objectIds.Add(pline.Id);
                                    RowsDataList.Add(dataRow);
                                    break;
                                case "Plaster":
                                    dataRow.Add(count.ToString());
                                    dataRow.Add(pline.Id.ToString().Replace("(", "").Replace(")", "").Trim());
                                    dataRow.Add("");
                                    dataRow.Add("1");
                                    dataRow.Add(pline.Length.ToString());
                                    dataRow.Add("3");
                                    dataRow.Add(pline.NumberOfVertices.ToString());
                                    dataRow.Add(pline.Closed.ToString());
                                    dataRow.Add("");
                                    objectIds.Add(pline.Id);
                                    RowsDataList.Add(dataRow);  
                                    break;
                                case "Ceramic":
                                    double AreaOfPolyClosedLine = 0;
                                    if (pline.Closed)
                                    {
                                        AreaOfPolyClosedLine = pline.Area;
                                    }
                                    dataRow.Add(count.ToString());
                                    dataRow.Add(pline.Id.ToString().Replace("(", "").Replace(")", "").Trim());
                                    dataRow.Add("");
                                    dataRow.Add("1");
                                    dataRow.Add(pline.Length.ToString());
                                    dataRow.Add(".7");
                                    dataRow.Add(pline.NumberOfVertices.ToString());
                                    dataRow.Add(pline.Closed.ToString());
                                    dataRow.Add(AreaOfPolyClosedLine.ToString());
                                    dataRow.Add("");
                                    dataRow.Add("3");
                                    dataRow.Add("");
                                    objectIds.Add(pline.Id);
                                    RowsDataList.Add(dataRow);
                                    break;
                                default:
                                    break;
                            }
                            count++;
                        }
                    }
                    GetPolyLinesInfo.Commit();
                }
            }
            int WallAreaIndex = 8;
            int VolumnIndex = 9;
            int CountIndex = 3;
            int WallThicknessIndex = 4;
            int WallHeightIndex = 7;
            int WallLengthIndex = 5;
            foreach (var row in RowsDataList)
            {
                row[WallAreaIndex].Replace("0", (StringMethods.IsDoubleNumber(row[CountIndex]) *
                   StringMethods.IsDoubleNumber(row[WallLengthIndex]) *
                   StringMethods.IsDoubleNumber(row[WallHeightIndex])).ToString());

                row[VolumnIndex].Replace("0", (StringMethods.IsDoubleNumber(row[WallAreaIndex]) *
                    StringMethods.IsDoubleNumber(row[WallThicknessIndex])).ToString());
            }
            return (RowsDataList, objectIds);
        }
        public static void HighlightPolyLine(ObjectId ID,bool HighLight = true)
        {
            using (Transaction HighlightPolyLine = database.TransactionManager.StartOpenCloseTransaction())
            {
                docAP.LockDocument();
                var entity = HighlightPolyLine.GetObject(ID, OpenMode.ForWrite) as Entity;
                var polyline = (Polyline)entity;
                if (HighLight)
                {
                    polyline.Highlight();
                }
                else
                {
                    polyline.Unhighlight();
                }

                HighlightPolyLine.Commit();
            }
        }
        public static void CreateTextAlongPolyLine(ObjectId PolyLineID,string Text)
        {
            Point3d StartPlPoint;
            Point3d EndPlPoint;
            using (Transaction GetCoord = database.TransactionManager.StartOpenCloseTransaction())
            {
                docAP.LockDocument();
                var entity = GetCoord.GetObject(PolyLineID, OpenMode.ForWrite) as Entity;
                var polyline = (Polyline)entity;
                StartPlPoint =polyline.StartPoint;
                EndPlPoint = polyline.EndPoint;
                GetCoord.Commit();
            }
            double StartPlXPoint = StartPlPoint.X;
            double StartPlYPoint = StartPlPoint.Y;
;
            double EndPlXPoint = EndPlPoint.X;
            double EndPlYPoint = EndPlPoint.Y;

            double MidPlXPoint = ((EndPlXPoint - StartPlXPoint) / 2) + StartPlXPoint;
            double MidPlYPoint = ((EndPlYPoint - StartPlYPoint) / 2) + StartPlYPoint;
            using (Transaction CreateText = database.TransactionManager.StartOpenCloseTransaction())
            {
                docAP.LockDocument();
                BlockTable blockTable;
                blockTable = CreateText.GetObject(database.BlockTableId,OpenMode.ForRead) as BlockTable;
                // Open the Block table record Model space for write
                BlockTableRecord blockTableRecord;
                blockTableRecord = CreateText.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                DBText dbText = new DBText();
                dbText.SetDatabaseDefaults();
                dbText.Position = new Point3d(MidPlXPoint, MidPlYPoint, 0);
                dbText.Height = 100;
                dbText.TextString = $"{Text}";
                blockTableRecord.AppendEntity(dbText);
                CreateText.AddNewlyCreatedDBObject(dbText, true);
                CreateText.Commit();
            }
        }
        public static void JoinPolyLinesByThereIds(List<ObjectId> objectIdList)
        {
            using (Transaction JoinPl = database.TransactionManager.StartTransaction())
            {
                var polylines = new List<Polyline>();
                docAP.LockDocument();
                foreach (var id in objectIdList)
                {
                    var entity = JoinPl.GetObject(id, OpenMode.ForWrite) as Entity;
                    polylines.Add((Polyline)entity);
                }
                int i = 1;
                foreach (var pl in polylines)
                {
                    if (i == polylines.Count)
                    {
                        break;
                    }
                    pl.UpgradeOpen();
                    if (pl.StartPoint.X == polylines[i].StartPoint.X ||
                        pl.StartPoint.Y == polylines[i].StartPoint.Y ||
                        pl.EndPoint.X == polylines[i].EndPoint.X ||
                        pl.EndPoint.Y == polylines[i].EndPoint.Y ||
                        pl.EndPoint.Y == polylines[i].StartPoint.Y ||
                        pl.StartPoint.Y == polylines[i].EndPoint.Y ||
                        pl.EndPoint.X == polylines[i].StartPoint.X ||
                        pl.StartPoint.X == polylines[i].EndPoint.X)
                    {
                        pl.JoinEntity(polylines[i]);
                    }
                    i++;
                }
                JoinPl.Commit();
            }
        }
    }
}
