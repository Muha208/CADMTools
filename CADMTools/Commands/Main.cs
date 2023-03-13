namespace CADMTools
{
    using System;
    using System.Reflection;
    using Autodesk.AutoCAD.Runtime;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Internal;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.GraphicsInterface;
    using Autodesk.AutoCAD.Windows;
    using Autodesk.AutoCAD.Internal.Windows;
    using Autodesk.Windows;
    using CADMTools.WPF_Pages;

    public static class Main
    {
        [CommandMethod("MTools",CommandFlags.Transparent)]
        #region external application public methods
        public static void Initialize()
        {
            RibbonControl ribbonControl = new RibbonControl();
            ribbonControl = ComponentManager.Ribbon;
            ribbonControl.ApplicationMenuButtonToolTip = "CADMTools By: Eng.Muhammad Osama";
            if (ribbonControl != null)
            {
                RibbonTab rtab = ribbonControl.FindTab("CADMTools");
                if (rtab != null)
                {
                    ribbonControl.Tabs.Remove(rtab);
                }
                rtab = new RibbonTab();
                rtab.Title = "CADMTools";
                rtab.Id = "CADMTools";
                //Add the Tab
                ribbonControl.Tabs.Add(rtab);
                addContent(rtab);
                ribbonControl.Visibility = System.Windows.Visibility.Visible;
            }
        }
        public static void addContent(RibbonTab rtab)
        {
            rtab.Panels.Add(AddOnePanel());
        }
        public static RibbonPanel AddOnePanel()
        {
            RibbonButton Rb_RoomsArea;
            RibbonPanelSource rps = new RibbonPanelSource();
            rps.Title = "CADMTools";
            RibbonPanel rp = new RibbonPanel();
            rp.Source = rps;

            //Create a Command Item that the Dialog Launcher can use,
            // for this test it is just a place holder.
            RibbonButton rci = new RibbonButton();
            rci.Name = "CADMTools";

            //assign the Command Item to the DialgLauncher which auto-enables
            // the little button at the lower right of a Panel
            rps.DialogLauncher = rci;

            Rb_RoomsArea = new RibbonButton();
            Rb_RoomsArea.Name = "CADMTools";
            Rb_RoomsArea.ShowText = true;
            Rb_RoomsArea.Text = "CADMTools";
            //Add the Button to the Tab
            rps.Items.Add(Rb_RoomsArea);
            Rb_RoomsArea.MouseLeft += Rb_RoomsArea_MouseLeft;
            return rp;
            #endregion
        }

        private static void Rb_RoomsArea_MouseLeft(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            mainPage.Show();
        }
    }
}
