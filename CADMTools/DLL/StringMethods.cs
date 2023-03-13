using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADMTools.DLL
{
    public class StringMethods
    {
        private static string GetStringWithoutSymbol(string Text,string ReplaceSymbol = "")
        {
            List<string> ListOfSymbols = new List<string>()
            {":",";","~","`","\\","@","\'","\"","$",",",">","<","=","+",".","{","}","[","]","/","`","#","%","^","*","&","(",")","_","?"};
            foreach (var Symbol in ListOfSymbols)
            {
                Text.Replace(Symbol, ReplaceSymbol);
            }
            return Text;
        }
        public static int IsIntNumber(string Number)
        {
            int NewNumber;
            if (int.TryParse(Number,out NewNumber))
            {
                return NewNumber;
            }
            else
            {
                return 0;
            }
        }
        public static double IsDoubleNumber(string Number)
        {
            double NewNumber;
            if (Number.ToLower() != "null")
            {
                if (double.TryParse(Number, out NewNumber))
                {
                    return NewNumber;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public enum TotalNumbers
        {
            One = 1,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight
        }
        public static string GetTimeStringFormatFromString(string Time)
        {
            if (Time == "" || Time == "00:00:00")
            {
                return "00:00:00";
            }
            var NewTime = new StringBuilder();
            string FirstNumber = null;
            string FirstNumber2 = null;
            string FirstNumber3 = null;
            for (int i = 0; i < Time.Length; i++)
            {
                if (i == 0 || i == 1 || i == 3 || i == 4 || i == 6 || i == 7)
                {
                    var NewNumber = IsIntNumber(Time[i].ToString());    
                    if (i == 0)
                    {
                        if (NewNumber > 2)
                        {
                            NewTime.Append("2");
                            FirstNumber = "2";
                        }
                        else
                        {
                            NewTime.Append($"{NewNumber}");
                            FirstNumber = NewNumber.ToString();    
                        }
                    }
                    else if (i == 1)
                    {
                        if (NewNumber > 4)
                        {
                            if (FirstNumber == "2")
                            {
                                NewTime.Append("4");
                            }
                            else
                            {
                                NewTime.Append($"{NewNumber}");
                            }
                        }
                        else
                        {
                            NewTime.Append($"{NewNumber}");
                        }
                    }
                    else if (i == 3)
                    {
                        if (NewNumber > 6)
                        {
                            NewTime.Append("6");
                            FirstNumber2 = "6";
                        }
                        else
                        {
                            NewTime.Append($"{NewNumber}");
                            FirstNumber2 = NewNumber.ToString();
                        }
                    }
                    else if (i == 4)
                    {
                        if (int.Parse(FirstNumber2) == 6)
                        {
                            NewTime.Append("0");
                        }
                        else
                        {
                            NewTime.Append($"{NewNumber}");
                        }
                    }
                    else if (i == 6)
                    {
                        if (NewNumber > 6)
                        {
                            NewTime.Append("6");
                            FirstNumber3 = "6";
                        }
                        else
                        {
                            NewTime.Append($"{NewNumber}");
                            FirstNumber3 = NewNumber.ToString();
                        }
                    }
                    else if (i == 7)
                    {
                        if (int.Parse(FirstNumber3) == 6)
                        {
                            NewTime.Append("0");
                        }
                        else
                        {
                            NewTime.Append($"{NewNumber}");
                        }
                    }
                }
                else
                {
                    NewTime.Append(Time[i].ToString().Replace(Time[i].ToString(), ":"));
                }
            }
            if (Time.Length == 1) {NewTime.Append("0:00:00");}
            else if (Time.Length == 2) { NewTime.Append(":00:00"); }
            else if (Time.Length == 3) { NewTime.Append("00:00"); }
            else if (Time.Length == 4) { NewTime.Append("0:00"); }
            else if (Time.Length == 5) { NewTime.Append(":00"); }
            else if (Time.Length == 6) { NewTime.Append("00"); }
            else if (Time.Length == 7) { NewTime.Append("0"); }
            else if (Time.Length == 8) { return NewTime.ToString(); }
            else { NewTime.Clear().Append("00:00:00");
        }
            return NewTime.ToString();  
        }
        public static string GetTheRequestCode(string NumberOfRecquest, string Department,string RequestRevisionNo)
        {
            return  $"{NumberOfRecquest.Trim()}-{Department.Trim()}-{RequestRevisionNo.Trim()}";
        }

    }
}
