using CodeStack.SwEx.AddIn;
using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Enums;
using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.PMPage;
using CodeStack.SwEx.PMPage.Base;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TraceGenerator.Properties; //make sure access modified is set to internal in resources in project properties, otherwise this won't work 

namespace TraceGenerator
{
    [ComVisible(true), Guid("AA9E4013-7970-49FB-BC79-4B570EF49E4E")] //GUI ID 
    [AutoRegister("Conductive Trace Generator", "generates conductive traces for multi material 3D printing")]

    public class AddIn : SwAddInEx
    {

        [Title("MechSense")]
        [Description("Augments conductive features to a 3D model for self-sensing applications")]
        [Icon(typeof(Resources), nameof(Resources.sensor_test))]

        private enum Commands_e



        {


            //[Title("Linear")]
            //[Description("Creates conductive chunks based on motion type")]
            //[Icon(typeof(Resources), nameof(Resources.UI_Linear_01))]
            //[CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            //Linear,


            [Title("Rotational Motion Sensors")]
            [Description("Generates Rotational Conductive Sensing Electrodes for a rotational mechanism")]
            [Icon(typeof(Resources), nameof(Resources.UI_rotational))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            Rotational,

            //[Title("Angular")]
            //[Description("Creates conductive chunks based on motion type")]
            //[Icon(typeof(Resources), nameof(Resources.UI_Angular_01))]
            //[CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            //Angular,


            //[Title("Radial")]
            //[Description("Creates conductive chunks based on motion type")]
            //[Icon(typeof(Resources), nameof(Resources.UI_Radial_01))]
            //[CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            //Radial,


            //[Title(typeof(Resources), nameof(Resources.CommanTitleCreateTrace))]
            [Title("Conductive Routing Path ")]
            [Description("Creates conductive routing path based on user-inputted routing profile")]
            [Icon(typeof(Resources), nameof(Resources.UI_trace_01))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            GenerateTrace,

            [Title("Export Models")]
            [Description("Exports 3d models into separate files")]
            [Icon(typeof(Resources), nameof(Resources.ExportObjects))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            ExportModels,
        }

        private PropertyManagerPageEx<PropertyPageHandler, TraceData> m_TracePmPage;
        private PropertyManagerPageEx<PropertyPageHandler, Rotational> m_RotPmPage;

        private TraceData m_TraceData;
        private Rotational m_Rotational;

        public override bool OnConnect()
        {

            AddCommandGroup<Commands_e>(OnButtonClick);
            m_TracePmPage = new PropertyManagerPageEx<PropertyPageHandler, TraceData>(App);
            m_TracePmPage.Handler.Closing += onTracePageClosing;
            m_TracePmPage.Handler.Closed += onTracePageClosed;

            m_TraceData = new TraceData()
            {
                Diameter = 0.05,      //default value, but can change in editor          

            };

            m_RotPmPage = new PropertyManagerPageEx<PropertyPageHandler, Rotational>(App);
            m_RotPmPage.Handler.Closing += onRotPageClosing;
            m_RotPmPage.Handler.Closed += onRotPageClosed;
            m_Rotational = new Rotational()
            {


            };
            return true;

        }

        private void onTracePageClosing(swPropertyManagerPageCloseReasons_e reason, ClosingArg arg)
        {

            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {

                //if (m_TraceData.Reference == null)
                //{

                //    arg.ErrorMessage = "Select Reference";
                //    arg.Cancel = true;
                //}



                if (m_TraceData.Sketch == null)
                {

                    arg.ErrorMessage = "Select Sketch";
                    arg.Cancel = true;
                }
            }
        }


        private void onTracePageClosed(swPropertyManagerPageCloseReasons_e reason)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay) //if Ok is pressed
            {
                try
                {
                    GenerateTrace(m_TraceData.Sketch, m_TraceData.Diameter);
                }

                catch (Exception ex)
                {

                    App.SendMsgToUser2(ex.Message,
               (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);

                }
            }

        }



        private void onRotPageClosing(swPropertyManagerPageCloseReasons_e reason, ClosingArg arg)
        {

        }


        private void onRotPageClosed(swPropertyManagerPageCloseReasons_e reason)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay) //if Ok is pressed
            {
                try
                {
                    conductiveRotation();
                }

                catch (Exception ex)
                {

                    App.SendMsgToUser2(ex.Message,
               (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);

                }
            }

        }




        private void OnButtonClick(Commands_e cmd) //when button for each UI element from MechSense menu is clicked
        {
            try
            {
                switch (cmd)
                {

                    case Commands_e.GenerateTrace: //show the trace generation window
                        m_TracePmPage.Show(m_TraceData);
                        break;


                    case Commands_e.Rotational: // show the mechsense rotation patch window/page
                        m_RotPmPage.Show(m_Rotational);
                        break;

                    case Commands_e.ExportModels:
                        ExportModels();
                        break;
                }
            }

            catch (Exception ex)
            {

                App.SendMsgToUser2(ex.Message,
           (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);

            }

        }





        private void GenerateTrace(IEntity sketch,
            double diameter)

        //curve was set to double diam
        {
            // we want to sweep cut an existing model which we will add as a var in the function (selected by user) 
            // we want to sweep create the other model but not combine 
            // we want to color it orange



            App.IActiveDoc2.IActiveView.EnableGraphicsUpdate = false;
            App.IActiveDoc2.FeatureManager.EnableFeatureTree = false;

            try
            {



                if (sketch.Select2(false, 4))
                {
                    //prev IFeature, if you add it- it actually doesn't work so keep like this! 

                    sketch.SelectByMark(false, 4); // mark 4 indicates that the sweep profile is circular

                    var featMgr = App.IActiveDoc2.FeatureManager;


                    Debug.Write("A Sketch has been selected" + sketch.GetType());




                    #region SweepCut


                    var swSweepFeatureData_cut = (SweepFeatureData)featMgr.CreateDefinition((int)swFeatureNameID_e.swFmSweepCut);


                    swSweepFeatureData_cut.TangentPropagation = false;
                    swSweepFeatureData_cut.AlignWithEndFaces = false;
                    swSweepFeatureData_cut.TwistControlType = (int)swTwistControlType_e.swTwistControlFollowPath;
                    swSweepFeatureData_cut.MaintainTangency = false;
                    swSweepFeatureData_cut.AdvancedSmoothing = false;
                    swSweepFeatureData_cut.PathAlignmentType = (int)swTangencyType_e.swTangencyDirectionVector;
                    swSweepFeatureData_cut.FeatureScope = true;
                    swSweepFeatureData_cut.AutoSelect = true;
                    swSweepFeatureData_cut.MergeSmoothFaces = false;
                    swSweepFeatureData_cut.CircularProfile = true;
                    swSweepFeatureData_cut.CircularProfileDiameter = diameter;
                    swSweepFeatureData_cut.Direction = 0;





                    var feat_cut = featMgr.CreateFeature(swSweepFeatureData_cut);

                    #endregion

                    #region SweepExtrude

                    sketch.SelectByMark(false, 4); //have to add this to reselect

                    var swSweepFeatureData = (SweepFeatureData)featMgr.CreateDefinition((int)swFeatureNameID_e.swFmSweep);



                    swSweepFeatureData.TangentPropagation = false;
                    swSweepFeatureData.AlignWithEndFaces = false;
                    swSweepFeatureData.TwistControlType = (int)swTwistControlType_e.swTwistControlFollowPath;
                    swSweepFeatureData.MaintainTangency = false;
                    swSweepFeatureData.AdvancedSmoothing = false;
                    swSweepFeatureData.StartTangencyType = 0;
                    swSweepFeatureData.EndTangencyType = 0;
                    swSweepFeatureData.ThinFeature = false;
                    swSweepFeatureData.ThinWallType = 0;
                    swSweepFeatureData.PathAlignmentType = (int)swTangencyType_e.swTangencyDirectionVector;
                    swSweepFeatureData.Merge = false; //was false
                    swSweepFeatureData.FeatureScope = true;
                    swSweepFeatureData.AutoSelect = true;
                    swSweepFeatureData.MergeSmoothFaces = false;
                    swSweepFeatureData.CircularProfile = true;
                    swSweepFeatureData.CircularProfileDiameter = diameter;
                    swSweepFeatureData.Direction = 0;




                    if (swSweepFeatureData != null)
                    {

                        Debug.Write("a definition has been created" + swSweepFeatureData.CircularProfileDiameter);


                    }


                    var feat = featMgr.CreateFeature(swSweepFeatureData);
                    #endregion


                    #region Extrude

                    //var feat=
                    //    App.IActiveDoc2.FeatureManager.FeatureExtrusion3(true, false, false,
                    //          (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind,
                    //          height, 0, false, false, false, false, 0, 0, false, false, false, false, false,
                    //          false, false, (int)swStartConditions_e.swStartSketchPlane, 0, false);   
                    #endregion


                    #region Change Color

                    double[] matProps = { 1.0, 0.5, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.0 }; //values can either be 1 or 0
                    // must pass as object type
                    // doubles in following format: [ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]

                    var matPropsObj = (object)matProps;

                    var model = App.IActiveDoc2;

                    var configNames = model.GetConfigurationNames();

                    feat.SetMaterialPropertyValues2(matPropsObj, 1, configNames); //supposedly changes material properties like color


                    #endregion


                    if (feat_cut == null)
                    {

                        throw new NullReferenceException("Failed to Cut");

                    }



                    if (feat == null)
                    {

                        throw new NullReferenceException("Failed to Extrude");

                    }


                }
                else
                {
                    throw new Exception("Failed to create Sketch");

                }

            }

            finally
            {


                App.IActiveDoc2.IActiveView.EnableGraphicsUpdate = true; //neeed to set back to true so users can use sw
                App.IActiveDoc2.FeatureManager.EnableFeatureTree = true;
            }
        }
        //**////////////////////////////////////////////////////***/////////////////////

        private void conductiveRotation()
        {

            App.IActiveDoc2.IActiveView.EnableGraphicsUpdate = false;
            App.IActiveDoc2.FeatureManager.EnableFeatureTree = false;

            try
            {
                ExtrudeRegion();
            }

            finally
            {
                App.IActiveDoc2.IActiveView.EnableGraphicsUpdate = true; //neeed to set back to true so users can use sw
                App.IActiveDoc2.FeatureManager.EnableFeatureTree = true;

            }
        }


        private ISketch CreateCircle(IEntity reference, IEntity circle, int sensorCount)
        {
            if (!reference.SelectByMark(false, 0))
            {

                throw new Exception("Failed to select");

            }

            var swEdge = circle as IEdge;
            var swCurveIn = (ICurve)
                  swEdge.GetCurve(); //call the GetCurve Method

            var swCircleArray = swCurveIn.CircleParams; //use CircleParams instead of ICircleParams to return object of type double[] array


            double[] swParaDataCircle = (double[])swCircleArray;

            //var rotPot = (SketchPoint)rotationCenter;
            var radius = swParaDataCircle[6];


            sensorCount = 2 * sensorCount; //04/10/2022

            // int sensorCount = 6;
            var model = App.IActiveDoc2;

            var swSelMgr = model.ISelectionManager;

            var skMgr = model.SketchManager;

            skMgr.InsertSketch(true);

            skMgr.AddToDB = true;

            var xPos = swParaDataCircle[0];
            var yPos = swParaDataCircle[1];
            var zPos = swParaDataCircle[2];

            // var norm = (IFace2)m_Rotational.Stationary.Face;
            var norm = (IFace2)m_Rotational.Face;

            double[] norm_vals = (double[])norm.Normal;



            Debug.Write("Norm is: " + norm_vals[1]); //seems like each element here is composed of 3; 

            // List<double> norm_list = norm_vals.ToString.Split(',').Select(r => Convert.ToDouble(r)).ToArray();


            if (swParaDataCircle[3] == 1) //if x-axis is the normal
            {



                if (norm_vals[0] < 0)
                {

                    xPos = zPos;
                }
                else
                {
                    xPos = -zPos;
                }
            }

            if (swParaDataCircle[4] == 1)
            {


                if (norm_vals[1] < 0)
                {

                    yPos = zPos;
                }
                else
                {
                    yPos = -zPos;
                }



            }





            if (swParaDataCircle[5] == 1)
            {



            }


            Debug.Print(xPos + "," + yPos + "," + zPos);

            // var boolstat = skMgr.SketchUseEdge3(false, true);

            //model.ClearSelection2(true); 

            //boolstat = model.Extension.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 1, null, 0);

            //Debug.Print(boolstat.ToString());

            float offset_rad = 0.007f; //offset so that circle is not at shaft completely; can change this value

            var arc = skMgr.CreateCircleByRadius(xPos, yPos, 0, radius + offset_rad); //this location depends on axis, so we need to retreieve this info first,
                                                                                      //that's why we weren't able to get correct vals before!! it depends on the axis 

            // Debug.Write("(" + swParaDataCircle[3] + swParaDataCircle[4] + swParaDataCircle[5] + ")");

            var offset = skMgr.SketchOffset2(1.5 * radius,
            false, true, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            var boolstatus = model.Extension.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0); //change this position based on where circle is made; btw this is first circle created, 

            //ie the bigger one

            var sketch = skMgr.ActiveSketch;


            //segment 1
            var SkSeg = (SketchSegment)((SelectionMgr)model.SelectionManager).GetSelectedObject6(1, -1);

            SkSeg.EqualSegment((int)swSketchSegmentType_e.swSketchSegmentType_sketchpoints, sensorCount); //creates 6 segment points, we can have this customized if we wish
            var sketchPoints_arc_1 = (object[])sketch.GetSketchPoints2();


            if (arc == null)
            {

                throw new NullReferenceException("Failed to create sketch");
            }




            //segment 2

            boolstatus = model.Extension.SelectByID2("Arc2", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0); //change this position based on where circle is made; btw this is first circle created, 
            //ie the bigger one

            var SkSeg2 = (SketchSegment)((SelectionMgr)model.SelectionManager).GetSelectedObject6(1, -1);

            SkSeg2.EqualSegment((int)swSketchSegmentType_e.swSketchSegmentType_sketchpoints, sensorCount); //creates 6 segment points, we can have this customized if we wish

            var sketchPoints_arc_2 = (object[])sketch.GetSketchPoints2();


            var point_arc_inner = default(SketchPoint);
            var point_arc_outer = default(SketchPoint);
            int counter = 0;


            //create a line between the two point pairs 
            // For static use: 2 * sensorCount - 1 for the for loop below for regions






            for (int i = sensorCount; i <= 2 * sensorCount - 1; i++)
            {

                // Debug.Write(sketchPoints.Length); //returns 8 points (6 for segments and 2 at center)
                point_arc_outer = (SketchPoint)sketchPoints_arc_1[2 + counter]; //first two will be the centers of the first circle and its conecntric counterpart, so choose the third element in array

                //  Debug.Write("(" + point_arc_outer.X + "," + point_arc_outer.Y + "," + point_arc_outer.Z + ")");
                point_arc_inner = (SketchPoint)sketchPoints_arc_2[2 + i]; //first two will be the centers of the first circle and its conecntric counterpart, so choose the third element in array
                // Debug.Write("(" + point_arc_inner.X + "," + point_arc_inner.Y + "," + point_arc_inner.Z + ")");

                skMgr.CreateLine(point_arc_inner.X, point_arc_inner.Y, point_arc_inner.Z, point_arc_outer.X, point_arc_outer.Y, point_arc_outer.Z);



                counter++;


            }

            skMgr.InsertSketch(true);

            // to false if you want sketch to be hidden, but didn't seem to affect it... 

            skMgr.AddToDB = true;




            return sketch;


        }


        private void ExtrudeRegion() //will change name later and will have to include a reference face 
                                     //the ref face will have to be for the sketch though and I guess based on that, this'll be fine ish
        {

            int sensorNum = 3;







            //var sketch = CreateCircle(m_Rotational.Stationary.Face, m_Rotational.Stationary.COR_circle, sensorNum);


            var sketch = CreateCircle(m_Rotational.Face, m_Rotational.COR_circle, sensorNum);
            var model = App.IActiveDoc2;
            var swSelectionMgr = model.ISelectionManager;



            if ((sketch as IFeature).Select2(false, 0))
            {

                var mySelectData = (SelectData)swSelectionMgr.CreateSelectData();
                var contours = (object[])sketch.GetSketchRegions();

                Debug.Write("(" + sketch.GetSketchContourCount() + "," + contours + ")"); //this returns 8 regions or countours within the sketch; they are just lines and arcs that make up the model

                var height = 0.01;


                //Check if rotational or stationary part has been selected:
                //Option (1) is stationary 
                // Option (2) is rotary

                var val_i = 1;

                if (m_Rotational.Optionss.ToString() == "Option1")
                {

                    val_i = 1;

                }



                if (m_Rotational.Optionss.ToString() == "Option2")
                {

                    val_i = contours.Length - 4;

                }

                Debug.Write(contours.Length - 2);

                for (int i = val_i; i < contours.Length - 2; i += 2) // was previously 1 for i
                {

                    var boolstatus = ((SketchRegion)contours[i]).Select2(false, mySelectData);

                    // For stationary base set offset to 0 on both cut extrude and cut 

                    var myFeature = model.FeatureManager.FeatureCut4(true, false, false, (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, height, height, false, false, false, false, 1.74532925199433E-02, 1.74532925199433E-02, false, false, false, false, false, true, true, true, true, false, 3, 0, true, false);

                    model.ClearSelection2(true);

                    boolstatus = ((SketchRegion)contours[i]).Select2(false, mySelectData);

                    var feat =
                    App.IActiveDoc2.FeatureManager.FeatureExtrusion3(true, false, true,
                         (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind,
                       height, 0, false, false, false, false, 0, 0, true, false, false, false, false,
                     false, false, (int)swStartConditions_e.swStartOffset, 0, false);

                    double[] matProps = { 1.0, 0.5, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.0 }; //values can either be 1 or 0
                    // must pass as object type
                    // doubles in following format: [ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]

                    var matPropsObj = (object)matProps;

                    var configNames = model.GetConfigurationNames();

                    feat.SetMaterialPropertyValues2(matPropsObj, 1, configNames); //supposedly changes material properties like color






                }





            }




        }







        public void ExportModels()
        {
            //Debug.Write("Hey, it's me. I'm the problem it's me");

            int modelType;
            string modelTitle;
            BodyFolder swBodyFolder;
            long longStatus;
            var swModel = (ModelDoc2)App.ActiveDoc;
            var swFeat = (Feature)swModel.FirstFeature();
            string filePath = "C:/Users/Owner/Downloads/assem_MechSense";
            var swFeatMgr = swModel.FeatureManager;
            var swSelMgr = swModel.SelectionManager;
            var fileNames = new string [2];
            var fileNameVar = new string[2];
            Boolean contLoop = true;
            Object[] bodyList = null;
           




        void GetVariantofBody(Feature swFeature)
            {


                //Debug.Write("functionCalled");
                int count;

                swBodyFolder = (BodyFolder)swFeat.GetSpecificFeature2();

                count = swBodyFolder.GetBodyCount();
                Debug.Write(count); //print number of bodies in project

                if (count < 1)
                {
                    Debug.Write("No bodies. Please create a body");

                }


                else
                {
                    bodyList = (Object[]) swBodyFolder.GetBodies();
                }


            }




            while (swFeat != null && contLoop)
            {

               
                string Name;
                Name = swFeat.GetTypeName2();

                if (Name == "SolidBodyFolder")
                {
                    //Debug.Write("foundSB");
                    GetVariantofBody(swFeat);
                    contLoop = false;

                }

                if (contLoop == true)
                {

                    swFeat = (Feature)swFeat.GetNextFeature();
                }

            }



            if (bodyList != null)
            {

                for (int i=0; i< bodyList.Count(); i++) {


                    Object COSMOSWORKSObj;
                    Object CWAddinCallBackObj;
                    CWAddinCallBackObj = App.GetAddInObject("CosmosWorks.CosmosWorks");



                    var path = "C:/Users/Owner/Downloads/"; // change this based on path of your choice;
                       var MechSenseObj=   (IBody2)bodyList[i];

                    //      var type = bodyList[0].GetType();

                    var obj_ID = MechSenseObj.Name;

                    var MechSenseObj_type = MechSenseObj.GetType();

                    Debug.Write(obj_ID);


      Boolean     boolstat = swModel.Extension.SelectByID2(obj_ID, "SOLIDBODY", 0, 0, 0, false, 0, null, 0);


             //    Boolean boolstat= App.IActiveDoc2.Extension.SelectByID2(obj_ID, "SOLIDBODY", 0, 0, 0, false, 0, null, 0);


                    //      Boolean boolstat= App.IActiveDoc2.Extension.SelectByID2("Solid Bodies", "BDYFOLDER", 0, 0, 0, false, 0, null, 0); //export all as one body (ie folder)



                    if (boolstat) {



                           var longstatus = App.IActiveDoc2.SaveAs3(path+ "MechSense_"   + i.ToString() + ".STL", 0, 2);



                    }





                }
            }
        }
    }


    }



