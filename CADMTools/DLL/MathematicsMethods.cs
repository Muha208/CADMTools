using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADMTools.DLL
{
    public static class MathematicsMethods
    {
        private const string Format = "{0:0.00}";

        public static (List<string> Volume , List<string> Area) GetVolumeListFromDataBase(string TableName,string CountColumnName, string HeightColumnName, string LengthColumnName, string ThicknessColumnName)
        {
            CURD SELECTCountColumn = new CURD();
            SELECTCountColumn.TableName = TableName;
            var CountList = ControlsMethods.GetListFromQueryFromDataBase(SELECTCountColumn.SelectColumnFromOneTable(CountColumnName));  
            CURD SELECTHeighttColumn = new CURD();
            SELECTHeighttColumn.TableName = TableName;
            var HeightList = ControlsMethods.GetListFromQueryFromDataBase(SELECTHeighttColumn.SelectColumnFromOneTable(HeightColumnName));
            CURD SELECTLengthColumn = new CURD();
            SELECTLengthColumn.TableName = TableName;
            var LengthList = ControlsMethods.GetListFromQueryFromDataBase(SELECTLengthColumn.SelectColumnFromOneTable(LengthColumnName));
            CURD SELECTThicknessColumn = new CURD();
            SELECTThicknessColumn.TableName = TableName;
            var ThicknesstList = ControlsMethods.GetListFromQueryFromDataBase(SELECTThicknessColumn.SelectColumnFromOneTable(ThicknessColumnName));
            var VolumeList= new List<string>();
            var AreaList = new List<string>();
            if (CountList != null && HeightList != null && LengthList != null && ThicknesstList != null )
            {
                for (int i = 0; i < LengthList.Count(); i++)
                {
                    double Area = StringMethods.IsDoubleNumber(CountList[i].ToString()) *
                        StringMethods.IsDoubleNumber(HeightList[i].ToString()) *
                        StringMethods.IsDoubleNumber(LengthList[i].ToString());
                    AreaList.Add(Area.ToString(String.Format(Format,0)));
                    double Volume = StringMethods.IsDoubleNumber(CountList[i].ToString()) *
                        StringMethods.IsDoubleNumber(HeightList[i].ToString()) *
                        StringMethods.IsDoubleNumber(LengthList[i].ToString()) *
                        StringMethods.IsDoubleNumber(ThicknesstList[i].ToString());
                    VolumeList.Add(Volume.ToString(String.Format(Format,Volume.ToString().Length)));
                }
            }
            return (VolumeList,AreaList);
        }
    }
}
