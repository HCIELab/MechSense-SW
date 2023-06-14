using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Enums;
using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.PMPage.Attributes;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraceGenerator.Properties;

namespace TraceGenerator
{

    [PageOptions(swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton | swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton)] //creates the green tick and red cross
    [Title(typeof(Resources), nameof(Resources.CommanTitleCreateTrace))]
    [Description("Generates trace")]
    [Icon(typeof(Resources), nameof(Resources.UI_trace_01))]

    class TraceData
    {


        //add Ientity for position

        [Description("Sketch")]

        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectProfile)]
        [SelectionBox(
            swSelectType_e.swSelSKETCHES)] //this allows the whole sketch to be selected
        public IEntity Sketch
        { get; set; }


        [Description("Diameter")]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 100, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_Thumbwheel)]

        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Diameter)]
        //   [Icon(typeof(Resources), nameof(Resources.height))]
        public double Diameter { get; set; }

        //I can probably ask for position of circle and allow it to be concentric to a circle if picked, or a point, or a line and if none picked, and then they choose an offset




        //     [SelectionBox(swSelectType_e.swSelPOINTREFS)]
        //     public double PointA { get; set; }


    }
}
