using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebSockets;

namespace CADMTools.DLL
{
    public static class ClassMethods
    {
        const string ColumnsNamePath = @"./ColumnsNamePath.txt";
        const string TableNAmePath = @"./TableNAmePath.txt";
        const string ColumnsDataTypePath = @"./ColumnsDataTypePath.txt";
        public static (List<string> ColumnsName, List<string> ColumnsNameForDataBase) GetListOfColumnNamesFromClass<T>(T castClass) where T : class
        {
            StringBuilder ColumnsNameFormClass = new StringBuilder();
            var ColumnsNameList = new List<string>();
            var ColumnsNameListForDataBase = new List<string>();
            var ListOfProp =  castClass.GetType().GetProperties().ToList();
            foreach (var propertyInfo in ListOfProp)
            {
                var ColumnsName = propertyInfo.Name;
                if (propertyInfo.Name.Contains("_"))
                {
                    ColumnsName = propertyInfo.Name.Replace("_", " ");
                }
                ColumnsNameList.Add(ColumnsName);
                ColumnsNameListForDataBase.Add(propertyInfo.Name);
                ColumnsNameFormClass.AppendLine(ColumnsName + "-----" + propertyInfo.Name);
            }
            File.AppendAllText(ColumnsNamePath, ColumnsNameFormClass.ToString());
            return (ColumnsNameList, ColumnsNameListForDataBase);
        }
        public static string GetTableNameFromClassForDataBase<T>(T castClass) where T : class
        {
            StringBuilder ClassName = new StringBuilder();
            int ClassNameLength = castClass.GetType().Name.ToString().Length;
            ClassName.Append(castClass.GetType().Name.ToString().Remove(ClassNameLength - 5));
            File.AppendAllText(TableNAmePath, ClassName.ToString());
            return ClassName.ToString();
        }
        public static (List<string> ColumnsDataTypeList,List<PropertyInfo> ListOfProperties) GetListOfDataColumnTypeFromClass<T>(T castClass, int varchar = 50) where T : class
        {
            StringBuilder ColumnsDataTypeFormClass = new StringBuilder();
            var ColumnsDataTypeList = new List<string>();
            string PropNewName;
            List<PropertyInfo> ListOfProperties = castClass.GetType().GetProperties().ToList(); 
            foreach (var propertyInfo in ListOfProperties)
            {
                var propName = propertyInfo.PropertyType.FullName.Replace("System.", "");
                PropNewName = propName;
                if (propName == "String")
                {
                    PropNewName = $"varchar({varchar})";
                }
                else if (propName == "Int23" || propName == "Int64")
                {
                    PropNewName = "Int";
                }
                ColumnsDataTypeList.Add(PropNewName);
                ColumnsDataTypeFormClass.AppendLine(PropNewName);
            }
            File.AppendAllText(ColumnsDataTypePath, ColumnsDataTypeFormClass.ToString());
            return (ColumnsDataTypeList, ListOfProperties);
        }
        public static List<object> FillClassPropertiesWithValue(object ClassAsRow, List<List<string>> RowsValuesList)
        {   
            var ListOfClassWithValues = new List<object>();
            if (RowsValuesList != null)
            {
                var RowCount = 0;    
                foreach (var Row in RowsValuesList)
                {
                    var ListOfClass = ClassAsRow;
                    var ProprtiesList = GetListOfDataColumnTypeFromClass(ListOfClass).ListOfProperties;
                    if (Row.Count == ProprtiesList.Count)
                    {
                        for (int i = 0; i < Row.Count; i++)
                        {
                            var ProprtyOfClass = ListOfClass.GetType().GetProperty(ProprtiesList[i].Name);
                            ProprtyOfClass.SetValue(ListOfClass, Row[i]);    
                        }
                    }
                    ListOfClassWithValues.Add(ListOfClass);
                    RowCount++;
                }
            }
            return ListOfClassWithValues;
        }
    }
}
