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
    [Icon(typeof(Resources), nameof(Resources.cylinder))]

    class ExportData
    {


        [SelectionBox( swSelectType_e.swSelBODYFEATURES)]



        // [SelectionBox(typeof(ReferenceSelectionCustomFilter),swSelectType_e.swSelDATUMPLANES, swSelectType_e.swSelFACES)]



        // swSelectType_e.swSelFACES)]

        [Title("Select Bodies to export")]
        [Description("Base Face or Plane")]
        [ControlOptions(height: 12)]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)] //use standard solidworks icons
        public IEntity Bodies { get; set; }



    }
}
