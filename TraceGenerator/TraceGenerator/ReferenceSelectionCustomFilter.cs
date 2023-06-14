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
{
   public  class ReferenceSelectionCustomFilter: SelectionCustomFilter<IEntity>
    {

        protected override bool Filter(IPropertyManagerPageControlEx selBox, IEntity selection, swSelectType_e selType, ref string itemText)
        {
            if (selType == swSelectType_e.swSelFACES) //f a face is selected
            {

                var face = selection as IFace2;

                if (face.IGetSurface().IsPlane())
                {

                    return true;

                }

            }

            return false;

        }
    
    }
}
