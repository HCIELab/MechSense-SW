using CodeStack.SwEx.PMPage.Base;
using CodeStack.SwEx.PMPage.Controls;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceGenerator
{ public   class diameterInner: SelectionCustomFilter<IEntity>
    {


        protected override bool Filter(IPropertyManagerPageControlEx selBox, IEntity selection, swSelectType_e selType, ref string itemText)
        {

      //    swCurveTypes_e.CIRCLE_TYPE
            if (selType == swSelectType_e.swSelEDGES)
            {

                var edge =
                    selection as IEdge;


                    if (edge.IGetCurve()!=null)
                    {
                    Curve curve = edge.IGetCurve();

                    if (curve.IsCircle())
                    {

                        return true;

                    }

                    }

            }

            return false;

        }

    }

}
