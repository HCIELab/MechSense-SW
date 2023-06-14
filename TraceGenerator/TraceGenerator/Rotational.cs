using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Enums;
using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.PMPage.Attributes;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraceGenerator.Properties;
using Xarial.VPages.Framework.PageElements;

 namespace TraceGenerator

{

    [PageOptions(swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton | swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton | swPropertyManagerPageOptions_e.swPropertyManagerOptions_PreviewButton)] //creates the green tick and red cross
    [Title("Rotational Motion Sensors")]
    [Description("Generates Rotational Conductive Sensing Electrodes for a rotational mechanism")]
    [Icon(typeof(Resources), nameof(Resources.UI_rotational))]








    public class Rotational


    {


        //**********************
        //Copyright(C) 2022 Xarial Pty Limited
        //Reference: https://www.codestack.net/labs/solidworks/swex/pmpage/controls/option-box/
        //License: https://www.codestack.net/license/
        //**********************


        //radio button// 
        public enum Options_ee
        {
            [Title("Static Part")]
            Option1,
            [Title("Moving Part")]
            Option2,

        }

        [OptionBox]
        public Options_ee Optionss { get; set; }



        //drop down menu
        //public enum Options_e
        //{
        //    Stationary,
        //    Rotating,
        //}

        //[ComboBoxOptions(swPropMgrPageComboBoxStyle_e.swPropMgrPageComboBoxStyle_Sorted)]
        //public Options_e Options { get; set; }











        //[Tab]
        //[Icon(typeof(Resources), nameof(Resources.Culture))]
        //public class TabControl1
        //{


        [Description("Object Face")]

        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)]
        [SelectionBox(typeof(ReferenceSelectionCustomFilter),
      swSelectType_e.swSelFACES)] //this allows the whole sketch to be selected
        public IEntity Face
        { get; set; }


        [Description("Shaft (Center of Rotation)")]

            [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectProfile)]
            [SelectionBox(typeof(diameterInner),
           swSelectType_e.swSelEDGES)]
            //  [ControlOptions(swAddControlOptions_e.swControlOptions_Visible)] //makes a selection box enabled or disabled

            public IEntity COR_circle
            { get; set; }


      



            //    [Description("Number of Sensors")]
            //    // public String Title { get; set; } = "Number of Sensors";


            //    [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_UnitlessInteger, 2, 7, 1, false, 1, 1, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_Thumbwheel)]
            //    [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_CircularPattern)]
            //    // [(swPropertyManagerPageControlType_e.swControlType_Checkbox,)]
            //    public int SensorNum { get; set; } //Make 3 be the default value here; 
            

        

        //public TabControl1 Stationary { get; set; }


        //[Tab]
        //[Icon(typeof(Resources), nameof(Resources.Culture))]
        //public class TabControl2
        //{
        //   // Choose face and anc ircle of rotation
        //}

        //public TabControl2 Rotating { get; set; }

        //I can probably ask for position of circle and allow it to be concentric to a circle if picked, or a point, or a line and if none picked, and then they choose an offset

        //  (swPropMgrPageTextBoxStyle_e.swPropMgrPageTextBoxStyle_Multiline)]



    }
}
