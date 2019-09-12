using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Matrox.MatroxImagingLibrary;

namespace _5_MatroxMetrology
{
    class Program
    {

        static string appPath = AppDomain.CurrentDomain.BaseDirectory;

        static string MODEL_FILE = appPath + "barcodeModel1.mmf";

        static string IMAGE_FILE = appPath + "barcode.jpg";


        static void Main(string[] args)
        {
            MIL_ID MilApplication = MIL.M_NULL;
            MIL_ID MilSystem = MIL.M_NULL;
            MIL_ID MilDisplay = MIL.M_NULL;

            MIL.MappAllocDefault(MIL.M_DEFAULT, ref MilApplication, ref MilSystem, ref MilDisplay, MIL.M_NULL, MIL.M_NULL);

            //Metrology(MilSystem, MilDisplay);

            //FindModel(MilSystem, MilDisplay);
            //Metro(MilSystem, MilDisplay);

            CompleteImageExample(MilSystem, MilDisplay);

            //Console.ReadKey();

            MIL.MappFreeDefault(MilApplication, MilSystem, MilDisplay, MIL.M_NULL, MIL.M_NULL);
        }

        static readonly double FAIL_COLOR = MIL.M_RGB888(255, 0, 0);
        static readonly double PASS_COLOR = MIL.M_RGB888(0, 255, 0);
        static readonly double REGION_COLOR = MIL.M_RGB888(0, 100, 255);
        static readonly double FEATURE_COLOR = MIL.M_RGB888(255, 0, 255);


        private const string METROL_COMPLETE_IMAGE_FILE = MIL.M_IMAGE_PATH + "barcode.jpg";
        private const string METROL_MODEL_FINDER_FILE = MIL.M_IMAGE_PATH + "barcodeModel.mmf";

        // Region parameters

        private const int SEGMENT1_LABEL = 1;
        private const double RECT1_POSITION_X = 33.00;
        private const double RECT1_POSITION_Y = 12.00;
        private const double RECT1_WIDTH = 121.04;
        private const double RECT1_HEIGHT = 13.90;
        private const double RECT1_ANGLE = 268.58;
        

        // Tolerance parameters
        private const int LENGTH_LABEL = 1;

        private const double LENGTH_VALUE_MIN = 90.00;
        private const double LENGTH_VALUE_MAX = 110.00;

        static void CompleteImageExample(MIL_ID MilSystem, MIL_ID MilDisplay)
        {
            MIL_ID MilImage = MIL.M_NULL;                        // Image buffer identifier.
            MIL_ID GraphicList = MIL.M_NULL;                     // Graphic list identifier.
            MIL_ID MilCalibration = MIL.M_NULL;                  // Calibration context
            MIL_ID MilMetrolContext = MIL.M_NULL;                // Metrology Context
            MIL_ID MilMetrolResult = MIL.M_NULL;                 // Metrology Result
            MIL_ID MilModelFinderContext = MIL.M_NULL;           // Model Finder Context
            MIL_ID MilModelFinderResult = MIL.M_NULL;            // Model Finder Result

            double Status = 0.0;
            double Value = 0.0;

            MIL_INT[] LengthLabels = new MIL_INT[1];

            LengthLabels[0] = SEGMENT1_LABEL;

            // Restore and display the source image.
            MIL.MbufRestore(METROL_COMPLETE_IMAGE_FILE, MilSystem, ref MilImage);
            MIL.MdispSelect(MilDisplay, MilImage);

            // Allocate a graphic list to hold the subpixel annotations to draw.
            MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref GraphicList);

            // Associate the graphic list to the display for annotations.
            MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, GraphicList);

            // Allocate metrology context and result.
            MIL.MmetAlloc(MilSystem, MIL.M_DEFAULT, ref MilMetrolContext);
            MIL.MmetAllocResult(MilSystem, MIL.M_DEFAULT, ref MilMetrolResult);

            // Add a first measured circle feature to context and set its search region
            
            // Add a first measured segment feature to context and set its search region
            MIL.MmetAddFeature(MilMetrolContext, MIL.M_MEASURED, MIL.M_SEGMENT, SEGMENT1_LABEL,
                           MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL, 0, MIL.M_DEFAULT);

            MIL.MmetSetRegion(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_DEFAULT, MIL.M_RECTANGLE,
                          RECT1_POSITION_X, RECT1_POSITION_Y, RECT1_WIDTH, RECT1_HEIGHT,
                          RECT1_ANGLE, MIL.M_NULL);

            MIL.MmetControl(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_EDGEL_ANGLE_RANGE, 10);
            
            
            // Add minimum distance tolerance
            MIL.MmetAddTolerance(MilMetrolContext, MIL.M_LENGTH, LENGTH_LABEL,
                             LENGTH_VALUE_MIN, LENGTH_VALUE_MAX, LengthLabels,
                             MIL.M_NULL, 1, MIL.M_DEFAULT);
            

            // Restore the model finder context and calibrate it
            MIL.MmodRestore(METROL_MODEL_FINDER_FILE, MilSystem, MIL.M_DEFAULT, ref MilModelFinderContext);
            //MIL.MmodControl(MilModelFinderContext, 0, MIL.M_ASSOCIATED_CALIBRATION, MilCalibration);

            // Allocate a result buffer
            MIL.MmodAllocResult(MilSystem, MIL.M_DEFAULT, ref MilModelFinderResult);

            // Find object occurrence
            MIL.MmodPreprocess(MilModelFinderContext, MIL.M_DEFAULT);
            MIL.MmodFind(MilModelFinderContext, MilImage, MilModelFinderResult);

            // Get number of found occurrences
            MIL.MmodGetResult(MilModelFinderResult, MIL.M_GENERAL, MIL.M_NUMBER, ref Value);

            if (Value == 1)
            {
                MIL.MmodDraw(MIL.M_DEFAULT, MilModelFinderResult, GraphicList, MIL.M_DRAW_POSITION + MIL.M_DRAW_BOX, MIL.M_DEFAULT, MIL.M_DEFAULT);
                Console.Write("Found occurrence using MIL Model Finder.\n");
                Console.Write("Press <Enter> to continue.\n\n");
                Console.ReadKey();

                // Clear annotations.
                MIL.MgraClear(MIL.M_DEFAULT, GraphicList);

                // Set the new context position
                MIL.MmetSetPosition(MilMetrolContext, MIL.M_DEFAULT, MIL.M_RESULT, MilModelFinderResult, 0, MIL.M_NULL, MIL.M_NULL, MIL.M_DEFAULT);

                // Calculate
                MIL.MmetCalculate(MilMetrolContext, MilImage, MilMetrolResult, MIL.M_DEFAULT);

                // Draw features
                MIL.MgraColor(MIL.M_DEFAULT, REGION_COLOR);
                MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_REGION, MIL.M_DEFAULT, MIL.M_DEFAULT);
                Console.Write("Regions used to calculate measured features at the new location.\n");
                Console.Write("Press <Enter> to continue.\n\n");
                Console.ReadKey();

                // Clear annotations.
                MIL.MgraClear(MIL.M_DEFAULT, GraphicList);

                MIL.MgraColor(MIL.M_DEFAULT, FEATURE_COLOR);
                MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_FEATURE, MIL.M_DEFAULT, MIL.M_DEFAULT);
                Console.Write("Calculated features.\n");

                MIL.MmetGetResult(MilMetrolResult, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_LENGTH, ref Value);
                Console.Write("- first measured segment:  length={0:0.00}mm\n", Value);
                
                Console.Write("- two measured points\n");

                Console.Write("Press <Enter> to continue.\n\n");
                Console.ReadKey();
                

                // Get min distance tolerance status and value
                MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(LENGTH_LABEL), MIL.M_STATUS, ref Status);
                MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(LENGTH_LABEL), MIL.M_TOLERANCE_VALUE, ref Value);

                if (Status == MIL.M_PASS)
                {
                    MIL.MgraColor(MIL.M_DEFAULT, PASS_COLOR);
                    Console.Write("Min distance tolerance value: {0:0.00} mm.\n", Value);
                }
                else
                {
                    MIL.MgraColor(MIL.M_DEFAULT, FAIL_COLOR);
                    Console.Write("Min distance tolerance value - Fail.\n");
                }
                MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_TOLERANCE, MIL.M_TOLERANCE_LABEL(LENGTH_LABEL), MIL.M_DEFAULT);
                

                Console.Write("Press <Enter> to quit.\n\n");
                Console.ReadKey();
            }
            else
            {
                Console.Write("Occurrence not found.\n");
                Console.Write("Press <Enter> to quit.\n\n");
                Console.ReadKey();
            }

            // Free all allocations.
            MIL.MgraFree(GraphicList);
            MIL.MmodFree(MilModelFinderContext);
            MIL.MmodFree(MilModelFinderResult);
            MIL.MmetFree(MilMetrolResult);
            MIL.MmetFree(MilMetrolContext);
            //MIL.McalFree(MilCalibration);
            MIL.MbufFree(MilImage);
        }

        //static readonly double FAIL_COLOR = MIL.M_RGB888(255, 0, 0);
        //static readonly double PASS_COLOR = MIL.M_RGB888(0, 255, 0);
        //static readonly double REGION_COLOR = MIL.M_RGB888(0, 100, 255);
        //static readonly double FEATURE_COLOR = MIL.M_RGB888(255, 0, 255);

        //private const int SEGMENT1_LABEL = 1;
        //private const string rectName1 = "rect1";
        //private const double RECT1_POSITION_X = 15;
        //private const double RECT1_POSITION_Y = 120;
        //private const double RECT1_WIDTH = 109;
        //private const double RECT1_HEIGHT = 13;
        //private const double RECT1_ANGLE = 87.37;

        //private const int SEGMENT2_LABEL = 2;
        //private const string rectName2 = "rect2";
        //private const double RECT2_POSITION_X = 18;
        //private const double RECT2_POSITION_Y = 114.50;
        //private const double RECT2_WIDTH = 120;
        //private const double RECT2_HEIGHT = 12;
        //private const double RECT2_ANGLE = 0.0;

        //private const int tolerenceLabel = 3;
        //private const double valueMin = 50;
        //private const double valueMax = 80;




        //static void Metro(MIL_ID MilSystem, MIL_ID MilDisplay)
        //{
        //    MIL_ID GraphicList = MIL.M_NULL;
        //    MIL_ID MilImage = MIL.M_NULL;
        //    MIL_ID MilModelImage = MIL.M_NULL;
        //    MIL_ID MilModelFinderResult = MIL.M_NULL;

        //    MIL_INT segmentLabel = 1;

        //    MIL_ID MilMetrolContext = MIL.M_NULL;
        //    MIL_ID MilMetrolResult = MIL.M_NULL;


        //    MIL_INT[] tolerenceFeatureLabels = new MIL_INT[1];
        //    tolerenceFeatureLabels[0] = SEGMENT1_LABEL;


        //    double Value = 0.0;
        //    double Status = 0.0;

        //    MIL.MbufRestore(IMAGE_FILE, MilSystem, ref MilImage);
        //    MIL.MdispSelect(MilDisplay, MilImage);

        //    MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref GraphicList);
        //    MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, GraphicList);
        //    MIL.MmodAlloc(MilSystem, MIL.M_GEOMETRIC, MIL.M_DEFAULT, ref MilModelImage);

        //    //MIL.MmodDefine(MilModelImage, MIL.M_IMAGE, MilImage,
        //    //   FIXTURED_MODEL_OFFSET_X, FIXTURED_MODEL_OFFSET_Y,
        //    //   FIXTURED_MODEL_SIZE_X, FIXTURED_MODEL_SIZE_Y);

        //    //MIL.MmodControl(MilModelImage, MIL.M_DEFAULT, MIL.M_NUMBER, MIL.M_ALL);
        //    //MIL.MmodControl(MilModelImage, MIL.M_CONTEXT, MIL.M_SPEED, MIL.M_VERY_HIGH);

        //    MIL.MmetAlloc(MilSystem, MIL.M_DEFAULT, ref MilMetrolContext);
        //    MIL.MmetAllocResult(MilSystem, MIL.M_DEFAULT, ref MilMetrolResult);



        //    MIL.MmetAddFeature(MilMetrolContext, MIL.M_MODIFY, MIL.M_SEGMENT, segmentLabel, MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL, 0, MIL.M_DEFAULT);

        //    MIL.MmetSetRegion(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_DEFAULT, MIL.M_RECTANGLE,
        //                  RECT1_POSITION_X, RECT1_POSITION_Y, RECT1_WIDTH, RECT1_HEIGHT,
        //                  RECT1_ANGLE, MIL.M_NULL);

        //    //MIL.MmetControl(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_EDGEL_ANGLE_RANGE, 10);

        //    //MIL.MmetName(MilMetrolContext, MIL.M_SET_NAME, SEGMENT1_LABEL, rectName1, MIL.M_NULL, MIL.M_NULL);



        //    MIL.MmetAddFeature(MilMetrolContext, MIL.M_MODIFY, MIL.M_SEGMENT, SEGMENT2_LABEL, MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL, 0, MIL.M_DEFAULT);

        //    MIL.MmetSetRegion(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT2_LABEL), MIL.M_DEFAULT, MIL.M_RECTANGLE,
        //                  RECT2_POSITION_X, RECT2_POSITION_Y, RECT2_WIDTH, RECT2_HEIGHT,
        //                  RECT2_ANGLE, MIL.M_NULL);

        //    //MIL.MmetControl(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_EDGEL_ANGLE_RANGE, 10);

        //    //MIL.MmetName(MilMetrolContext, MIL.M_SET_NAME, SEGMENT2_LABEL, rectName2, MIL.M_NULL, MIL.M_NULL);



        //    MIL.MmetAddTolerance(MilMetrolContext, MIL.M_LENGTH, tolerenceLabel,
        //                     valueMin, valueMax, tolerenceFeatureLabels, MIL.M_NULL, 2, MIL.M_DEFAULT);

        //    MIL.MmetCalculate(MilMetrolContext, MilImage, MilMetrolResult, MIL.M_DEFAULT);
        //    MIL.MgraColor(MIL.M_DEFAULT, REGION_COLOR);
        //    MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_REGION, MIL.M_DEFAULT, MIL.M_DEFAULT);
        //    Console.ReadKey();
        //    MIL.MmodRestore(MODEL_FILE, MilSystem, MIL.M_DEFAULT, ref MilModelImage);
        //    MIL.MmodAllocResult(MilSystem, MIL.M_DEFAULT, ref MilModelFinderResult);
        //    MIL.MmodPreprocess(MilModelImage, MIL.M_DEFAULT);
        //    MIL.MmodFind(MilModelImage, MilImage, MilModelFinderResult);
        //    MIL.MmodGetResult(MilModelFinderResult, MIL.M_GENERAL, MIL.M_NUMBER, ref Value);

        //    if (Value == 1)
        //    {
        //        MIL.MgraClear(MIL.M_DEFAULT, GraphicList);
        //        MIL.MmodDraw(MIL.M_DEFAULT, MilModelFinderResult, GraphicList, MIL.M_DRAW_POSITION + MIL.M_DRAW_BOX, MIL.M_DEFAULT, MIL.M_DEFAULT);
        //        Console.Write("Found occurrence using MIL Model Finder.\n");
        //        Console.ReadKey();
        //    }
        //    else
        //    {
        //        Console.Write("Occurrence not found.\n");
        //        Console.ReadKey();
        //    }

        //    MIL.MmodFree(MilModelFinderResult);
        //    MIL.MmodFree(MilModelImage);
        //    MIL.MbufFree(MilImage);
        //    MIL.MgraFree(GraphicList);
        //}









        //static void Metrology(MIL_ID MilSystem, MIL_ID MilDisplay)
        //{
        //    MIL_ID MilImage = MIL.M_NULL;                    // Image buffer identifier.
        //    MIL_ID GraphicList = MIL.M_NULL;                 // Graphic list identifier.
        //    MIL_ID MilMetrolContext = MIL.M_NULL;            // Metrology Context
        //    MIL_ID MilMetrolResult = MIL.M_NULL;             // Metrology Result

        //    MIL_ID MilModelFinderContext = MIL.M_NULL;           // Model Finder Context
        //    MIL_ID MilModelFinderResult = MIL.M_NULL;            // Model Finder Result

        //    double Status = 0.0;
        //    double Value = 0.0;

        //    MIL_INT[] AngularityFeatureLabels = new MIL_INT[2];

        //    MIL.MbufRestore(IMAGE_FILE, MilSystem, ref MilImage);
        //    MIL.MdispSelect(MilDisplay, MilImage);

        //    MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref GraphicList);

        //    MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, GraphicList);
        //    MIL.MmetAlloc(MilSystem, MIL.M_DEFAULT, ref MilMetrolContext);
        //    MIL.MmetAllocResult(MilSystem, MIL.M_DEFAULT, ref MilMetrolResult);

        //    MIL.MmetAddFeature(MilMetrolContext, MIL.M_MEASURED, MIL.M_SEGMENT, SEGMENT1_LABEL,
        //                   MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL, 0, MIL.M_DEFAULT);

        //    MIL.MmetSetRegion(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_DEFAULT, MIL.M_RECTANGLE,
        //                  RECT1_POSITION_X, RECT1_POSITION_Y, RECT1_WIDTH, RECT1_HEIGHT,
        //                  RECT1_ANGLE, MIL.M_NULL);

        //    MIL.MmetControl(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_EDGEL_ANGLE_RANGE, 10);


        //    // Add angularity tolerance
        //    //MIL.MmetAddTolerance(MilMetrolContext, MIL.M_ANGULARITY, ANGULARITY_LABEL,
        //    //                 ANGULARITY_VALUE_MIN, ANGULARITY_VALUE_MAX, AngularityFeatureLabels,
        //    //                 MIL.M_NULL, 2, MIL.M_DEFAULT);

        //    MIL.MmetCalculate(MilMetrolContext, MilImage, MilMetrolResult, MIL.M_DEFAULT);


        //    MIL.MgraColor(MIL.M_DEFAULT, REGION_COLOR);
        //    MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_REGION, MIL.M_DEFAULT, MIL.M_DEFAULT);
        //    Console.Write("Regions used to calculate measured features:\n");

        //    Console.Write("Press <Enter> to continue.\n\n");
        //    Console.ReadKey();


        //    //MIL.MgraClear(MIL.M_DEFAULT, GraphicList);

        //    MIL.MgraColor(MIL.M_DEFAULT, FEATURE_COLOR);
        //    MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_FEATURE, MIL.M_DEFAULT, MIL.M_DEFAULT);
        //    Console.Write("Calculated features:\n");

        //    MIL.MmetGetResult(MilMetrolResult, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_LENGTH, ref Value);
        //    Console.Write("- first measured segment:  length={0:0.00}mm\n", Value);

        //    Console.Write("Press <Enter> to continue.\n\n");
        //    Console.ReadKey();

        //    MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(ANGULARITY_LABEL), MIL.M_STATUS, ref Status);
        //    MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(ANGULARITY_LABEL), MIL.M_TOLERANCE_VALUE, ref Value);

        //    if (Status == MIL.M_PASS)
        //    {
        //        MIL.MgraColor(MIL.M_DEFAULT, PASS_COLOR);
        //        Console.Write("Angularity value: {0:0.00} degrees.\n", Value);
        //    }
        //    else
        //    {
        //        MIL.MgraColor(MIL.M_DEFAULT, FAIL_COLOR);
        //        Console.Write("Angularity value - Fail.\n");
        //    }


        //    MIL.MgraFree(GraphicList);
        //    MIL.MmetFree(MilMetrolResult);
        //    MIL.MmetFree(MilMetrolContext);
        //    MIL.MbufFree(MilImage);

        //}


        //private const int FIXTURED_MODEL_OFFSET_X = 395;
        //private const int FIXTURED_MODEL_OFFSET_Y = 200;
        //private const int FIXTURED_MODEL_SIZE_X = 110;
        //private const int FIXTURED_MODEL_SIZE_Y = 120;

        //static void FindModel(MIL_ID MilSystem, MIL_ID MilDisplay)
        //{
        //    MIL_ID GraphicList = MIL.M_NULL;
        //    MIL_ID MilImage = MIL.M_NULL;
        //    MIL_ID MilModelImage = MIL.M_NULL;
        //    MIL_ID MilModelFinderResult = MIL.M_NULL;


        //    MIL_ID MilMetrolContext = MIL.M_NULL;
        //    MIL_ID MilMetrolResult = MIL.M_NULL;

        //    double Value = 0.0;
        //    double Status = 0.0;

        //    MIL.MbufRestore(IMAGE_FILE, MilSystem, ref MilImage);
        //    MIL.MdispSelect(MilDisplay, MilImage);

        //    MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref GraphicList);
        //    MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, GraphicList);
        //    MIL.MmodAlloc(MilSystem, MIL.M_GEOMETRIC, MIL.M_DEFAULT, ref MilModelImage);

        //    //MIL.MmodDefine(MilModelImage, MIL.M_IMAGE, MilImage,
        //    //   FIXTURED_MODEL_OFFSET_X, FIXTURED_MODEL_OFFSET_Y,
        //    //   FIXTURED_MODEL_SIZE_X, FIXTURED_MODEL_SIZE_Y);

        //    //MIL.MmodControl(MilModelImage, MIL.M_DEFAULT, MIL.M_NUMBER, MIL.M_ALL);
        //    //MIL.MmodControl(MilModelImage, MIL.M_CONTEXT, MIL.M_SPEED, MIL.M_VERY_HIGH);

        //    MIL.MmetAlloc(MilSystem, MIL.M_DEFAULT, ref MilMetrolContext);
        //    MIL.MmetAllocResult(MilSystem, MIL.M_DEFAULT, ref MilMetrolResult);

        //    MIL.MmetAddFeature(MilMetrolContext, MIL.M_MEASURED, MIL.M_SEGMENT, SEGMENT1_LABEL,
        //                   MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL, 0, MIL.M_DEFAULT);

        //    MIL.MmetSetRegion(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_DEFAULT, MIL.M_RECTANGLE,
        //                  RECT1_POSITION_X, RECT1_POSITION_Y, RECT1_WIDTH, RECT1_HEIGHT,
        //                  RECT1_ANGLE, MIL.M_NULL);

        //    MIL.MmetControl(MilMetrolContext, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_EDGEL_ANGLE_RANGE, 10);


        //    MIL.MmodRestore(MODEL_FILE, MilSystem, MIL.M_DEFAULT, ref MilModelImage);
        //    MIL.MmodAllocResult(MilSystem, MIL.M_DEFAULT, ref MilModelFinderResult);
        //    MIL.MmodPreprocess(MilModelImage, MIL.M_DEFAULT);
        //    MIL.MmodFind(MilModelImage, MilImage, MilModelFinderResult);
        //    MIL.MmodGetResult(MilModelFinderResult, MIL.M_GENERAL, MIL.M_NUMBER, ref Value);

        //    if (Value == 1)
        //    {
        //        MIL.MgraClear(MIL.M_DEFAULT, GraphicList);
        //        MIL.MmodDraw(MIL.M_DEFAULT, MilModelFinderResult, GraphicList, MIL.M_DRAW_POSITION + MIL.M_DRAW_BOX, MIL.M_DEFAULT, MIL.M_DEFAULT);
        //        Console.Write("Found occurrence using MIL Model Finder.\n");
        //        Console.ReadKey();


        //        MIL.MmetSetPosition(MilMetrolContext, MIL.M_DEFAULT, MIL.M_RESULT, MilModelFinderResult, 0, MIL.M_NULL, MIL.M_NULL, MIL.M_DEFAULT);
        //        MIL.MmetCalculate(MilMetrolContext, MilImage, MilMetrolResult, MIL.M_DEFAULT);
        //        MIL.MgraColor(MIL.M_DEFAULT, REGION_COLOR);
        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_LENGTH, ref Value);
        //        Console.Write("- first measured segment:  length={0:0.00}mm\n", Value);
        //        MIL.MmetCalculate(MilMetrolContext, MilImage, MilMetrolResult, MIL.M_DEFAULT);
        //        MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_REGION, MIL.M_DEFAULT, MIL.M_DEFAULT);
        //        Console.Write("Regions used to calculate measured features at the new location.\n");
        //        Console.Write("Press <Enter> to continue.\n\n");
        //        Console.ReadKey();
        //        //MIL.MgraClear(MIL.M_DEFAULT, GraphicList);

        //        MIL.MgraColor(MIL.M_DEFAULT, FEATURE_COLOR);
        //        MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_FEATURE, MIL.M_DEFAULT, MIL.M_DEFAULT);
        //        Console.Write("Calculated features.\n");

        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_FEATURE_LABEL(SEGMENT1_LABEL), MIL.M_LENGTH, ref Value);
        //        Console.Write("- first measured segment:  length={0:0.00}mm\n", Value);

        //        Console.Write("- two measured points\n");

        //        Console.Write("Press <Enter> to continue.\n\n");
        //        Console.ReadKey();

        //        // Get angularity tolerance status and value
        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(ANGULARITY_LABEL), MIL.M_STATUS, ref Status);
        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(ANGULARITY_LABEL), MIL.M_TOLERANCE_VALUE, ref Value);

        //        if (Status == MIL.M_PASS)
        //        {
        //            MIL.MgraColor(MIL.M_DEFAULT, PASS_COLOR);
        //            Console.Write("Angularity value: {0:0.00} degrees.\n", Value);
        //        }
        //        else
        //        {
        //            MIL.MgraColor(MIL.M_DEFAULT, FAIL_COLOR);
        //            Console.Write("Angularity value - Fail.\n");
        //        }
        //        MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_TOLERANCE, MIL.M_TOLERANCE_LABEL(ANGULARITY_LABEL), MIL.M_DEFAULT);

        //        // Get min distance tolerance status and value
        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(MIN_DISTANCE_LABEL), MIL.M_STATUS, ref Status);
        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(MIN_DISTANCE_LABEL), MIL.M_TOLERANCE_VALUE, ref Value);

        //        if (Status == MIL.M_PASS)
        //        {
        //            MIL.MgraColor(MIL.M_DEFAULT, PASS_COLOR);
        //            Console.Write("Min distance tolerance value: {0:0.00} mm.\n", Value);
        //        }
        //        else
        //        {
        //            MIL.MgraColor(MIL.M_DEFAULT, FAIL_COLOR);
        //            Console.Write("Min distance tolerance value - Fail.\n");
        //        }
        //        MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_TOLERANCE, MIL.M_TOLERANCE_LABEL(MIN_DISTANCE_LABEL), MIL.M_DEFAULT);

        //        // Get max distance tolerance status and value
        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(MAX_DISTANCE_LABEL), MIL.M_STATUS, ref Status);
        //        MIL.MmetGetResult(MilMetrolResult, MIL.M_TOLERANCE_LABEL(MAX_DISTANCE_LABEL), MIL.M_TOLERANCE_VALUE, ref Value);

        //        if (Status == MIL.M_PASS)
        //        {
        //            MIL.MgraColor(MIL.M_DEFAULT, PASS_COLOR);
        //            Console.Write("Max distance tolerance value: {0:0.00} mm.\n", Value);
        //        }
        //        else
        //        {
        //            MIL.MgraColor(MIL.M_DEFAULT, FAIL_COLOR);
        //            Console.Write("Max distance tolerance value - Fail.\n");
        //        }
        //        MIL.MmetDraw(MIL.M_DEFAULT, MilMetrolResult, GraphicList, MIL.M_DRAW_TOLERANCE, MIL.M_TOLERANCE_LABEL(MAX_DISTANCE_LABEL), MIL.M_DEFAULT);

        //        Console.Write("Press <Enter> to quit.\n\n");
        //        Console.ReadKey();
        //    }
        //    else
        //    {
        //        Console.Write("Occurrence not found.\n");
        //        Console.ReadKey();
        //    }

        //    MIL.MmodFree(MilModelFinderResult);
        //    MIL.MmodFree(MilModelImage);
        //    MIL.MbufFree(MilImage);
        //    MIL.MgraFree(GraphicList);
        //}
    }
}
