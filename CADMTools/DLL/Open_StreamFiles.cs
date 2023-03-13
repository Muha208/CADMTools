namespace CADMTools.DLL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Threading.Tasks;
    using OfficeOpenXml;
    using System.Windows;

    public static class Open_StreamFiles 
    {
        #region Feilds
        //Log Files
        private const string filepath = @".\Excel Files\Total_View.xlsx";
        private static string logNameFilepath = @".\Logs\LogNameFile.txt";
        private static string logNumberFilepath = @".\Logs\LogNumberFile.txt";
        private static string logExcelSheetsNameFilepath = @".\Logs\LogExcelSheetsNameFile.txt";
        private static string logEncryptFilepath = @".\Logs\LogEncryptFile.txt";
        #endregion
        #region Properties
        private static string LogNameFilepath
        {
            get { return IsDriectoryExists(logNameFilepath); }
        }
        private static string LogNumberFilepath
        {
            get { return IsDriectoryExists(logNumberFilepath); }
        }
        public static string LogExcelSheetsNameFilepath
        {
            get { return IsDriectoryExists(logExcelSheetsNameFilepath); }
        }
        #endregion
        private static void CreatEncryptedLogFile(string FullPath,string NewExtension, StringBuilder LogMessage,string FolderName,bool AddGUID)
        {
            FileInfo logfi = new FileInfo(logEncryptFilepath);
            var logEncryptFile = new StringBuilder();
            try
            {
                string NewFileName;
                switch (AddGUID)
                {
                    case true:
                        NewFileName = $"{Path.GetFileNameWithoutExtension(@FullPath)}-{Guid.NewGuid()}{NewExtension}";
                        break;
                    default:
                        NewFileName = $"{Path.GetFileNameWithoutExtension(@FullPath)}{NewExtension}";
                        break;
                }
                var FullDirctory = new DirectoryInfo(NewFileName).Parent.FullName;
                var FullNewDirctory = $@"{FullDirctory}\Logs\{FolderName}\{NewFileName}";
                IsDriectoryExists(FullNewDirctory);   
                FileInfo fi = new FileInfo(FullNewDirctory);
                using (var WriteLog = fi.CreateText())
                {
                    WriteLog.Write(LogMessage);
                }
                Path.ChangeExtension(FullNewDirctory, NewExtension);
                logEncryptFile.Append($"Given Path:{@FullPath}\n"+
                    $"New File Name:{NewFileName}\n"+
                    $"Parent Path:{FullDirctory}\n"+
                    $"Full New Path:{FullNewDirctory}\n").ToString();
                using (var WriteLog = logfi.CreateText())
                {
                    WriteLog.Write(logEncryptFile);
                }
            }
            catch (Exception ex)
            {
                logEncryptFile.Append($"Failed To Encrypt Data!\n{ex.Message}").ToString();
                using (var WriteLog = logfi.CreateText())
                {
                    WriteLog.Write(logEncryptFile);
                }
            }    
        }
        internal static string IsDriectoryExists(string FullDirectoryPath)
        {
            var DirectoryParentPath = new DirectoryInfo(FullDirectoryPath).Parent.FullName;
            if (!Directory.Exists(DirectoryParentPath))
            {
                Directory.CreateDirectory(DirectoryParentPath);
            }
            return FullDirectoryPath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SheetName">The main sheet that you want to get data from</param>
        /// <param name="Filepath">The path of only Excel File that you want to open</param>
        /// <param name="FixedRows">The Fixed Rows counts from top that you want to ignored, Defult = 1</param>
        /// <param name="FixedColumns">The Fixed Cloumns counts from top that you want to ignored, Defult = 1</param>
        public static IEnumerable<string> GetColumnTextDataFromExcelFile(string SheetName,string Filepath = filepath, int FixedRows = 1,int FixedColumns = 1)
        {
            FileInfo fi = new FileInfo(Filepath);
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                var sheet = excelPackage.Workbook.Worksheets[SheetName];
                List<string> NameOfData = new List<string>();
                var LogMessage = new StringBuilder();
                LogMessage.Append("Log Items: \n");
                int index = 0;

                for (int i = sheet.Dimension.Start.Row + FixedRows; i <= sheet.Dimension.End.Row; i++)
                {
                    NameOfData.Add(sheet.Cells[i,FixedColumns+1].Value.ToString());
                    LogMessage.Append($"{NameOfData[index]} ----- At Row {i}\n");
                    index++;    
                }
                CreatEncryptedLogFile(LogNameFilepath, ".log", LogMessage,"Open_StraemFile",false);
                return NameOfData;  
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SheetName">The main sheet that you want to get data from as string</param>
        /// <param name="Filepath">The path of only Excel File that you want to open</param>
        /// <param name="FixedRows">The Fixed Rows counts from top that you want to ignored, Defult = 1</param>
        /// <param name="FixedColumns">The Fixed Cloumns counts from top that you want to ignored, Defult = 1</param>
        public static IEnumerable<double> GetColumnNumericalDataFromExcelFile(string SheetName, string Filepath = filepath, int FixedRows = 1, int FixedColumns = 1)
        {
            FileInfo fi = new FileInfo(Filepath);
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                var sheet = excelPackage.Workbook.Worksheets[SheetName];
                List<double> NoOfData = new List<double>();
                int i;
                var SuccessedItems = new StringBuilder();
                SuccessedItems.Append("Successfuly Add Items: \n");
                var FaildItems = new StringBuilder();
                FaildItems.Append("Faild Add Items: \n");
                var LogMessage = new StringBuilder();
                LogMessage.Append("Log Items: \n");

                for (i = sheet.Dimension.Start.Row + FixedRows; i <= sheet.Dimension.End.Row; i++)
                {
                    bool IsNumber = double.TryParse(sheet.Cells[i, FixedColumns + 1].Value.ToString(), out double DataAsNumber);
                    if (IsNumber)
                    {
                        NoOfData.Add(DataAsNumber);
                        SuccessedItems.Append($"{DataAsNumber} ----- At Row {i}\n");
                    }
                    else
                    {
                        FaildItems.Append($"{DataAsNumber} ----- At Row {i}\n");  
                        NoOfData.Add(0);        
                    }
                }
                LogMessage.Append($"{SuccessedItems}\n *************** \n{FaildItems}");
                CreatEncryptedLogFile(LogNumberFilepath, ".log", LogMessage, "Open_StraemFile",false);
                return NoOfData;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExcelFilePath">The path of only Excel File that you want to Stream it</param>
        /// <returns></returns>
        public static FileStream OpenStreamExcelFile(string ExcelFilePath = filepath)
        {
            using (var fis = new FileStream(ExcelFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var excelPacksge = new ExcelPackage(fis);
                var excelWorkBook = excelPacksge.Workbook;
                var excelWorkSheets = excelWorkBook.Worksheets;
                var LogMessage = new StringBuilder();
                LogMessage.Append($"successeded to get file {fis.Name}\nIn WorkBook {excelWorkBook.Properties.Title}:\n");
                int index = 1;
                foreach (var worksheet in excelWorkSheets)
                {
                    var WorkSheetsName = worksheet.Name;
                    LogMessage.Append($"{index}- {WorkSheetsName}\n");
                    index++;
                }
                LogMessage.Append("----- Streaming Successeded -----");
                CreatEncryptedLogFile(LogExcelSheetsNameFilepath, ".log", LogMessage, "Open_StraemFile" , false );
                return fis; 
            }
        }
    }
}
