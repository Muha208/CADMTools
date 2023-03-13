using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogApp
{
    public static class GetAndSetData
    {
        private static string DataAssignedLogFile = @".\Logs\DataAssignedLogFile.txt";
        private static string ListOfClassPropertiesLogFile = @".\Logs\ListOfClassPropertiesLogFile.txt";
        private static string ListOfPropNamesLogFile = @".\Logs\ListOfPropNamesLogFile.txt";
        public static T GetDataFromTextBoxInsideContainerIntoProperties<T>(Form form, T castToClass,bool AllProperties = false) where T : class
        {
            var DataAssigned = new StringBuilder("The Data Assugned As:\n");
            List<MemberInfo> PropList = new List<MemberInfo>();
            if (AllProperties == true)
            {
                PropList = GetListOfAllPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            else
            {
                PropList = GetListOfPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            var activeControl = form.ActiveControl;
            int MemberCount = 1;
            int ControlCount = 1;
            if (activeControl.GetType() == typeof(SplitContainer))
            {
                SplitContainer SplitContainer = (SplitContainer)activeControl;
                var MembersPanel1 = SplitContainer.Panel1.Controls;
                var MembersPanel2 = SplitContainer.Panel2.Controls;
                DataAssigned.Append($"\n-------- The Active Control At Panel(1) Name = {SplitContainer.Name}\n" +
                    $"Members Included:");
                foreach (var Member in MembersPanel1)
                {
                    DataAssigned.Append($"\n{MemberCount}- {Member}");
                    MemberCount++;
                    if (Member.GetType() == typeof(TextBox))
                    {
                        var TextControl = (TextBox)Member;
                        SetDataFromTextBoxIProperties(TextControl, castToClass);
                    }
                    if (Member.GetType() == typeof(TabControl))
                    {
                        TabControl tabControl = (TabControl)Member;
                        DataAssigned.Append($" - Is {tabControl.Name}");
                        for (int i = 0; i < tabControl.TabPages.Count; i++)
                        {
                            var CTabPage = tabControl.TabPages[i].Controls;
                            DataAssigned.Append($" - In TabPage At index[{i}] The Controls Included:");
                            foreach (var control in CTabPage)
                            {
                                DataAssigned.Append($"\n----[{ControlCount}]-----{control} - It's Property = {control.GetType()}");
                                ControlCount++;
                                if (control.GetType() == typeof(TextBox))
                                {
                                    var TextControl = (TextBox)control;
                                    SetDataFromTextBoxIProperties(TextControl, castToClass, AllProperties);
                                }
                                else if (control.GetType() == typeof(ComboBox))
                                {
                                    var CombControl = (ComboBox)control;
                                    SetDataFromComboBoxIProperties(CombControl, castToClass, AllProperties);
                                }
                                else if (control.GetType() == typeof(DateTimePicker))
                                {
                                    var DTControl = (DateTimePicker)control;
                                    SetDataFromDateTimePickerIProperties(DTControl, castToClass, AllProperties);
                                }
                                else if (control.GetType() == typeof(RichTextBox))
                                {
                                    var TextControl = (RichTextBox)control;
                                    SetDataFromRichTextBoxIProperties(TextControl, castToClass, AllProperties);
                                }
                            }
                        }
                    }
                }
                DataAssigned.Append($"\n-------- The Active Control At Panel(2) Name = {SplitContainer.Name}\n" +
                    $"Members Included:");
                foreach (var Member in MembersPanel2)
                {
                    DataAssigned.Append($"\n{MemberCount}- {Member}");
                    MemberCount++;
                    if (Member.GetType() == typeof(TextBox))
                    {
                        var TextControl = (TextBox)Member;
                        SetDataFromTextBoxIProperties(TextControl, castToClass , AllProperties);
                    }
                    if (Member.GetType() == typeof(TabControl))
                    {
                        TabControl tabControl = (TabControl)Member;
                        DataAssigned.Append($" - Is {tabControl.Name}");
                        for (int i = 0; i < tabControl.TabPages.Count; i++)
                        {
                            var CTabPage = tabControl.TabPages[i].Controls;
                            DataAssigned.Append($" - In TabPage At index[{i}] The Controls Included:");
                            foreach (var control in CTabPage)
                            {
                                DataAssigned.Append($"\n----[{ControlCount}]-----{control} - It's Property = {control.GetType()}");
                                ControlCount++;
                                if (control.GetType() == typeof(TextBox))
                                {
                                    var TextControl = (TextBox)control;
                                    SetDataFromTextBoxIProperties(TextControl, castToClass, AllProperties);
                                }
                                else if (control.GetType() == typeof(ComboBox))
                                {
                                    var CombControl = (ComboBox)control;
                                    SetDataFromComboBoxIProperties(CombControl, castToClass, AllProperties);
                                }
                                else if (control.GetType() == typeof(DateTimePicker))
                                {
                                    var DTControl = (DateTimePicker)control;
                                    SetDataFromDateTimePickerIProperties(DTControl, castToClass, AllProperties);
                                }
                                else if (control.GetType() == typeof(RichTextBox))
                                {
                                    var TextControl = (RichTextBox)control;
                                    SetDataFromRichTextBoxIProperties(TextControl, castToClass, AllProperties);
                                }
                            }
                        }
                    }
                }
            }
            DataAssigned.Append("\n----- Add Sucsseded ------");
            File.WriteAllText(DataAssignedLogFile, DataAssigned.ToString());
            return castToClass;
        }
        public static void SetDataFromTextBoxIProperties<T>(TextBox control, T castToClass, bool AllProperties = false) where T : class
        {
            var DataAssigned = new StringBuilder("\nThe Data Assinged As:\n");
            List<MemberInfo> PropList = new List<MemberInfo>();
            if (AllProperties == true)
            {
                PropList = GetListOfAllPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            else
            {
                PropList = GetListOfPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            var ControlText = control;
            var ControlName = ControlText.Name.ToLower().Replace("tx_", "").Trim();
            var PropName = PropList.Where(x => x.Name.ToLower().Trim() == ControlName);
            var PropNewList = PropName.ToList();
            foreach (var Prop in PropNewList)
            {
                var PropertyType = castToClass.GetType().GetProperty(Prop.Name).PropertyType;
                DataAssigned.Append($" -[{PropertyType}]- ");
                if (PropertyType.IsPublic)
                {
                    if (PropertyType == typeof(string))
                    {
                        if (ControlName == Prop.Name.ToLower().Trim())
                        {
                            var ControlValue = control.Text;
                            var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                            PropToGetValue.SetValue(castToClass, $@"{ControlValue}");
                            var Result = PropToGetValue.GetValue(castToClass);
                            DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                                $@"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                        }
                    }
                    else if (PropertyType == typeof(int))
                    {
                        var ControlValue = StringMethods.IsNumber(control.Text);
                        var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                        PropToGetValue.SetValue(castToClass, ControlValue);
                        var Result = PropToGetValue.GetValue(castToClass);
                        DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                            $"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                    }
                }
            }
        }
        public static void SetDataFromRichTextBoxIProperties<T>(RichTextBox control, T castToClass, bool AllProperties = false) where T : class
        {
            var DataAssigned = new StringBuilder("\nThe Data Assinged As:\n");
            List<MemberInfo> PropList = new List<MemberInfo>();
            if (AllProperties == true)
            {
                PropList = GetListOfAllPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            else
            {
                PropList = GetListOfPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            var ControlText = control;
            var ControlName = ControlText.Name.ToLower().Replace("rtx_", "").Trim();
            var PropName = PropList.Where(x => x.Name.ToLower().Trim() == ControlName);
            var PropNewList = PropName.ToList();
            foreach (var Prop in PropNewList)
            {
                var PropertyType = castToClass.GetType().GetProperty(Prop.Name).PropertyType;
                DataAssigned.Append($" -[{PropertyType}]- ");
                if (PropertyType.IsPublic)
                {
                    if (PropertyType == typeof(string))
                    {
                        if (ControlName == Prop.Name.ToLower().Trim())
                        {
                            var ControlValue = control.Text;
                            var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                            PropToGetValue.SetValue(castToClass, $@"{ControlValue}");
                            var Result = PropToGetValue.GetValue(castToClass);
                            DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                                $@"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                        }
                    }
                    else if (PropertyType == typeof(int))
                    {
                        var ControlValue = StringMethods.IsNumber(control.Text);
                        var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                        PropToGetValue.SetValue(castToClass, ControlValue);
                        var Result = PropToGetValue.GetValue(castToClass);
                        DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                            $"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                    }
                }
            }
        }
        public static void SetDataFromComboBoxIProperties<T>(ComboBox control, T castToClass, bool AllProperties = false) where T : class
        {
            var DataAssigned = new StringBuilder("\nThe Data Assinged As:\n");
            List<MemberInfo> PropList = new List<MemberInfo>();
            if (AllProperties == true)
            {
                PropList = GetListOfAllPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            else
            {
                PropList = GetListOfPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            var ControlText = control;
            var ControlName = ControlText.Name.ToLower().Replace("cb_", "").Trim();
            var PropName = PropList.Where(x => x.Name.ToLower().Trim() == ControlName);
            var PropNewList = PropName.ToList();
            foreach (var Prop in PropNewList)
            {
                if (ControlName == Prop.Name.ToLower().Trim())
                {
                    var PropertyType = castToClass.GetType().GetProperty(Prop.Name).PropertyType;
                    DataAssigned.Append($" -[{PropertyType}]- ");
                    if (PropertyType.IsPublic)
                    {
                        if (PropertyType == typeof(string))
                        {
                            var ControlValue = control.Text;
                            var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                            PropToGetValue.SetValue(castToClass, ControlValue.ToString());
                            var Result = PropToGetValue.GetValue(castToClass);
                            DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                                $"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                        }
                        else if (PropertyType == typeof(int))
                        {
                            int ControlValue;
                            if (control.SelectedIndex == -1)
                            {
                                ControlValue = control.SelectedIndex + 2;
                            }
                            else
                            {
                                ControlValue = control.SelectedIndex + 1;
                            }
                            var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                            PropToGetValue.SetValue(castToClass, ControlValue);
                            var Result = PropToGetValue.GetValue(castToClass);
                            DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                                $"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                        }
                    }
                }
            }
        }
        public static void SetDataFromDateTimePickerIProperties<T>(DateTimePicker control, T castToClass, bool AllProperties = false) where T : class
        {
            var DataAssigned = new StringBuilder("\nThe Data Assinged As:\n");
            List<MemberInfo> PropList = new List<MemberInfo>();
            if (AllProperties == true)
            {
                PropList = GetListOfAllPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            else
            {
                PropList = GetListOfPropertiesNamesFromClass(castToClass).MemberInfoList;
            }
            var ControlText = control;
            var ControlName = ControlText.Name.ToLower().Replace("dt_", "").Trim();
            var PropName = PropList.Where(x => x.Name.ToLower().Trim() == ControlName);
            var PropNewList = PropName.ToList();
            foreach (var Prop in PropNewList)
            {
                var PropertyType = castToClass.GetType().GetProperty(Prop.Name).PropertyType;
                DataAssigned.Append($" -[{PropertyType}]- ");
                if (PropertyType.IsPublic)
                {
                    if (PropertyType == typeof(DateTime))
                    {
                        if (ControlName == Prop.Name.ToLower().Trim())
                        {
                            var ControlValue = control.Value;
                            var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                            PropToGetValue.SetValue(castToClass, ControlValue);
                            var Result = PropToGetValue.GetValue(castToClass);
                            DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                                $"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                        }
                    }
                    else if (PropertyType == typeof(string))
                    {
                        var ControlValue = control.Value.ToString();
                        var PropToGetValue = castToClass.GetType().GetProperty(Prop.Name);
                        PropToGetValue.SetValue(castToClass, ControlValue);
                        var Result = PropToGetValue.GetValue(castToClass);
                        DataAssigned.Append($"[ Property Name: {Prop.Name} - Property Value: {Result} ] => " +
                            $"[ Control Name: {ControlText.Name} - Control Result: {ControlValue} ]\n");
                    }
                }
            }
        }
        public static List<string> GetListOfPropertiesValueAtIndexFromListOfClass<T>(List<T> values, int indexNumber, List<string> ColumnsName) where T : class
        {
            int count = 1;
            var DataAssigned = new StringBuilder("The Data Assinged As:\n");
            var ListOfValues = new List<string>();
            var ElementValues = values.ElementAtOrDefault(indexNumber);
            var PropList = ElementValues.GetType().GetMembers().ToList();
            for (int i = 0; i < ColumnsName.Count; i++)
            {
                foreach (var Prop in PropList)
                {
                    if (Prop.Name == ColumnsName[i])
                    {
                        var PropToGetValue = ElementValues.GetType().GetProperty(Prop.Name);
                        var IsNullValues = PropToGetValue.GetValue(ElementValues);
                        if (IsNullValues == null)
                        {
                            ListOfValues.Add("NULL");
                        }
                        else
                        {
                            ListOfValues.Add(IsNullValues.ToString());
                        }
                        DataAssigned.Append($"{count} - {PropToGetValue} = {PropToGetValue.GetValue(ElementValues)}\n");
                        count++;
                    }
                }
            }
            DataAssigned.Append("\n----- Add Sucsseded ------");
            File.WriteAllText(ListOfClassPropertiesLogFile, DataAssigned.ToString());
            return ListOfValues;
        }
        public static (List<string> StringList,List<MemberInfo> MemberInfoList) GetListOfPropertiesNamesFromClass<T>(T castToClass, bool IsTableView = false) where T:class
        {
            var DataAssigned = new StringBuilder("The Data Assugned As:\n");
            var PropList = castToClass.GetType().GetMembers().Where(x => x.DeclaringType.FullName == castToClass.ToString()).ToList();
            List<MemberInfo> NewPropList = new List<MemberInfo>();  
            List<string> PropNameList = new List<string>();
            int count = 1;
            string NameOfProp;
            foreach (var Prop in PropList)
            {
                NameOfProp = Prop.Name;   
                if (NameOfProp.StartsWith("get_", StringComparison.OrdinalIgnoreCase) == true || NameOfProp.StartsWith("set_", StringComparison.OrdinalIgnoreCase) == true || NameOfProp == "Equals" || NameOfProp == "GetHashCode" || NameOfProp == "GetType" || NameOfProp == "ToString" || NameOfProp == ".ctor")
                {

                }
                else
                {
                    if (IsTableView)
                    {
                        var NewName = NameOfProp.ToString().Replace("View_", "");
                        NewPropList.Add(Prop);
                        PropNameList.Add(NewName);
                        DataAssigned.AppendLine($"{count}- " + NewName);
                        count++;
                    }
                    else
                    {
                        NewPropList.Add(Prop);
                        PropNameList.Add(NameOfProp);
                        DataAssigned.AppendLine($"{count}- " + NameOfProp);
                        count++;
                    }
                }
                
            }
            DataAssigned.Append("\n----- Add Sucsseded ------");
            File.WriteAllText(ListOfPropNamesLogFile, DataAssigned.ToString());
            return (PropNameList, NewPropList);    
        }
        public static (List<string> StringList, List<MemberInfo> MemberInfoList) GetListOfAllPropertiesNamesFromClass<T>(T castToClass, bool IsTableView = false) where T : class
        {
            var DataAssigned = new StringBuilder("The Data Assugned As:\n");
            var PropList = castToClass.GetType().GetMembers().ToList();
            List<MemberInfo> NewPropList = new List<MemberInfo>();
            List<string> PropNameList = new List<string>();
            int count = 1;
            string NameOfProp;
            foreach (var Prop in PropList)
            {
                NameOfProp = Prop.Name;
                if (NameOfProp.StartsWith("get_", StringComparison.OrdinalIgnoreCase) == true || NameOfProp.StartsWith("set_", StringComparison.OrdinalIgnoreCase) == true || NameOfProp == "Equals" || NameOfProp == "GetHashCode" || NameOfProp == "GetType" || NameOfProp == "ToString" || NameOfProp == ".ctor")
                {

                }
                else
                {
                    if (IsTableView)
                    {
                        var NewName = NameOfProp.ToString().Replace("View_", "");
                        NewPropList.Add(Prop);
                        PropNameList.Add(NewName);
                        DataAssigned.AppendLine($"{count}- " + NewName);
                        count++;
                    }
                    else
                    {
                        NewPropList.Add(Prop);
                        PropNameList.Add(NameOfProp);
                        DataAssigned.AppendLine($"{count}- " + NameOfProp);
                        count++;
                    }
                }

            }
            DataAssigned.Append("\n----- Add Sucsseded ------");
            File.WriteAllText(ListOfPropNamesLogFile, DataAssigned.ToString());
            return (PropNameList, NewPropList);
        }
    }
}
