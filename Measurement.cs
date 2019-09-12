using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Matrox.MatroxImagingLibrary;

namespace _7_MatroxMeasurement
{
    class Program
    {
        static void Main(string[] args)
        {

            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string MODEL_FILE = path + "/" + "constructionModel.mmf";
            string IMAGE_FILE = path + "/OrjinalImaj.jpg";

            MIL_ID MilApplication = MIL.M_NULL;
            MIL_ID MilSystem = MIL.M_NULL;
            MIL_ID MilDisplay = MIL.M_NULL;
            MIL_ID MilImage = MIL.M_NULL;

            MIL.MappAllocDefault(MIL.M_DEFAULT, ref MilApplication, ref MilSystem, ref MilDisplay, MIL.M_NULL, MIL.M_NULL);
            



            Console.Write("StripeMarker.\n");
            StripeMarker(MilSystem, MilDisplay, MilImage, MODEL_FILE, IMAGE_FILE);

            
            
            Console.Write("\n\nEdgeMarker.\n");
            EdgeMarker(MilSystem, MilDisplay, MilImage, MODEL_FILE, IMAGE_FILE);
            
            MIL.MappFreeDefault(MilApplication, MilSystem, MilDisplay, MIL.M_NULL, MIL.M_NULL);

        }

        static void StripeMarker(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID MilImage, string MODEL_FILE, string IMAGE_FILE)
        {
            MIL_ID MilSourceImage = MIL.M_NULL;
            MIL_ID MilGraphicList = MIL.M_NULL;
            MIL_ID MilModContext = MIL.M_NULL;
            MIL_ID MilModResult = MIL.M_NULL;
            MIL_ID MilFixturingOffset = MIL.M_NULL;
            MIL_ID StripeMarker = MIL.M_NULL;


            MIL_INT ImageSizeX = 0;
            MIL_INT ImageSizeY = 0;

            double ModelPosX = 0.0;
            double ModelPosY = 0.0;
            double ModelFindControl = 0.0;

            int StripePosX = 812;
            int StripePosY = 210;
            int StripeWidth = 80;
            int StripeHeight = 16;
            int StripeAngle = 90;
            

            int POLARITY_LEFT = MIL.M_ANY;
            int POLARITY_RIGHT = MIL.M_OPPOSITE;

            MIL_INT FindStripeControl = 0;

            double ResultWidth = 0.0;


            MIL.MbufRestore(IMAGE_FILE, MilSystem, ref MilSourceImage);
            MIL.MbufInquire(MilSourceImage, MIL.M_SIZE_X, ref ImageSizeX);
            MIL.MbufInquire(MilSourceImage, MIL.M_SIZE_Y, ref ImageSizeY);

            MIL.MbufAlloc2d(MilSystem, ImageSizeX, ImageSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilImage);
            MIL.MimConvert(MilSourceImage, MilImage, MIL.M_RGB_TO_L);

            MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref MilGraphicList);

            MIL.MdispSelect(MilDisplay, MilImage);

            MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, MilGraphicList);

            MIL.MmeasAllocMarker(MilSystem, MIL.M_STRIPE, MIL.M_DEFAULT, ref StripeMarker);

            //MIL.MmeasAllocMarker(MilSystem, MIL.M_EDGE, MIL.M_DEFAULT, ref StripeMarker);

            MIL.MmeasSetMarker(StripeMarker, MIL.M_SEARCH_REGION_INPUT_UNITS, MIL.M_WORLD, MIL.M_NULL);

            MIL.MmodAlloc(MilSystem, MIL.M_GEOMETRIC, MIL.M_DEFAULT, ref MilModContext);
            MIL.MmodRestore(MODEL_FILE, MilSystem, MIL.M_DEFAULT, ref MilModContext);
            MIL.MmodAllocResult(MilSystem, MIL.M_DEFAULT, ref MilModResult);

            MIL.MmodControl(MilModContext, MIL.M_DEFAULT, MIL.M_NUMBER, MIL.M_ALL);
            MIL.MmodControl(MilModContext, MIL.M_CONTEXT, MIL.M_SPEED, MIL.M_VERY_HIGH);

            MIL.MmodPreprocess(MilModContext, MIL.M_DEFAULT);

            MIL.McalAlloc(MilSystem, MIL.M_FIXTURING_OFFSET, MIL.M_DEFAULT, ref MilFixturingOffset);
            MIL.McalFixture(MIL.M_NULL, MilFixturingOffset, MIL.M_LEARN_OFFSET, MIL.M_MODEL_MOD,
               MilModContext, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT);

            MIL.MmodFind(MilModContext, MilImage, MilModResult);
            MIL.MmodGetResult(MilModResult, MIL.M_GENERAL, MIL.M_NUMBER, ref ModelFindControl);

            if (ModelFindControl > 0)
            {
                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_GREEN);
                MIL.MmodDraw(MIL.M_DEFAULT, MilModResult, MilGraphicList, MIL.M_DRAW_POSITION, MIL.M_DEFAULT, MIL.M_DEFAULT);

                MIL.MmodGetResult(MilModResult, 0, MIL.M_POSITION_X + MIL.M_TYPE_MIL_DOUBLE, ref ModelPosX);
                MIL.MmodGetResult(MilModResult, 0, MIL.M_POSITION_Y + MIL.M_TYPE_MIL_DOUBLE, ref ModelPosY);

                MIL.MgraText(MIL.M_DEFAULT, MilGraphicList, ModelPosX - 20, ModelPosY, "Occurence Found");

                MIL.McalFixture(MilImage, MilFixturingOffset, MIL.M_MOVE_RELATIVE, MIL.M_RESULT_MOD,
                   MilModResult, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT);

                MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ORIGIN, StripePosX, StripePosY);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_POLARITY, POLARITY_LEFT, POLARITY_RIGHT);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_SEARCH_REGION_CLIPPING, MIL.M_ENABLE, MIL.M_NULL);

                MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_SIZE, StripeWidth, StripeHeight);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ANGLE, StripeAngle, MIL.M_NULL);

                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_BLUE);
                MIL.MgraControl(MIL.M_DEFAULT, MIL.M_INPUT_UNITS, MIL.M_WORLD);
                MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_SEARCH_REGION, MIL.M_DEFAULT, MIL.M_MARKER);
                MIL.MgraControl(MIL.M_DEFAULT, MIL.M_INPUT_UNITS, MIL.M_PIXEL);

                MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_POSITION + MIL.M_STRIPE_WIDTH);

                //MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_POSITION + MIL.M_LENGTH);

                MIL.MmeasGetResult(StripeMarker, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref FindStripeControl, MIL.M_NULL);

                if (FindStripeControl == 1)
                {
                    //MIL.MmeasGetResult(StripeMarker, MIL.M_LENGTH, ref ResultWidth, MIL.M_NULL);

                    MIL.MmeasGetResult(StripeMarker, MIL.M_STRIPE_WIDTH, ref ResultWidth, MIL.M_NULL);

                    ResultWidth = Math.Round(ResultWidth, 4);

                    Console.Write("Width: {0} pixels.\n", ResultWidth);

                    MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_GREEN);
                    MIL.MgraText(MIL.M_DEFAULT, MilGraphicList, StripePosX + 40, StripePosY - 40, ResultWidth.ToString()+" px");

                    MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_RED);

                    //MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_EDGES, MIL.M_DEFAULT, MIL.M_RESULT);

                    MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_WIDTH, MIL.M_DEFAULT, MIL.M_RESULT);

                }
                Console.Write("Press <Enter> to continue.\n");
                Console.ReadKey();
            }
            else
            {
                Console.Write("Occurence Not Found");
            }
            
            MIL.MgraFree(MilGraphicList);
            MIL.MmeasFree(StripeMarker);
            MIL.MmodFree(MilModContext);
            MIL.MmodFree(MilModResult);
            MIL.McalFree(MilFixturingOffset);
            MIL.MbufFree(MilSourceImage);
        }


        static void EdgeMarker(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID MilImage, string MODEL_FILE, string IMAGE_FILE)
        {
            MIL_ID MilSourceImage = MIL.M_NULL;
            MIL_ID MilGraphicList = MIL.M_NULL;
            MIL_ID MilModContext = MIL.M_NULL;
            MIL_ID MilModResult = MIL.M_NULL;
            MIL_ID MilFixturingOffset = MIL.M_NULL;
            MIL_ID EdgeMarker = MIL.M_NULL;
            
            MIL_INT ImageSizeX = 0;
            MIL_INT ImageSizeY = 0;

            double ModelPosX = 0.0;
            double ModelPosY = 0.0;
            double ModelFindControl = 0.0;

            //int EdgePosX = 833;
            //int EdgePosY = 235;
            //int EdgeWidth = 7;
            //int EdgeHeight = 35;
            //int EdgeAngle = 270;

            int EdgePosX = 812;
            int EdgePosY = 210;
            int EdgeWidth = 80;
            int EdgeHeight = 16;
            int EdgeAngle = 90;

            int POLARITY_LEFT = MIL.M_ANY;
            int POLARITY_RIGHT = MIL.M_OPPOSITE;

            MIL_INT FindEdgeControl = 0;

            double ResultWidth = 0.0;


            MIL.MbufRestore(IMAGE_FILE, MilSystem, ref MilSourceImage);
            MIL.MbufInquire(MilSourceImage, MIL.M_SIZE_X, ref ImageSizeX);
            MIL.MbufInquire(MilSourceImage, MIL.M_SIZE_Y, ref ImageSizeY);

            MIL.MbufAlloc2d(MilSystem, ImageSizeX, ImageSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref MilImage);
            MIL.MimConvert(MilSourceImage, MilImage, MIL.M_RGB_TO_L);

            MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref MilGraphicList);

            MIL.MdispSelect(MilDisplay, MilImage);

            MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, MilGraphicList);

            //MIL.MmeasAllocMarker(MilSystem, MIL.M_STRIPE, MIL.M_DEFAULT, ref StripeMarker);

            MIL.MmeasAllocMarker(MilSystem, MIL.M_EDGE, MIL.M_DEFAULT, ref EdgeMarker);

            MIL.MmeasSetMarker(EdgeMarker, MIL.M_SEARCH_REGION_INPUT_UNITS, MIL.M_WORLD, MIL.M_NULL);

            MIL.MmodAlloc(MilSystem, MIL.M_GEOMETRIC, MIL.M_DEFAULT, ref MilModContext);
            MIL.MmodRestore(MODEL_FILE, MilSystem, MIL.M_DEFAULT, ref MilModContext);
            MIL.MmodAllocResult(MilSystem, MIL.M_DEFAULT, ref MilModResult);

            MIL.MmodControl(MilModContext, MIL.M_DEFAULT, MIL.M_NUMBER, MIL.M_ALL);
            MIL.MmodControl(MilModContext, MIL.M_CONTEXT, MIL.M_SPEED, MIL.M_VERY_HIGH);

            MIL.MmodPreprocess(MilModContext, MIL.M_DEFAULT);

            MIL.McalAlloc(MilSystem, MIL.M_FIXTURING_OFFSET, MIL.M_DEFAULT, ref MilFixturingOffset);
            MIL.McalFixture(MIL.M_NULL, MilFixturingOffset, MIL.M_LEARN_OFFSET, MIL.M_MODEL_MOD,
               MilModContext, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT);

            MIL.MmodFind(MilModContext, MilImage, MilModResult);
            MIL.MmodGetResult(MilModResult, MIL.M_GENERAL, MIL.M_NUMBER, ref ModelFindControl);

            if (ModelFindControl > 0)
            {
                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_GREEN);
                MIL.MmodDraw(MIL.M_DEFAULT, MilModResult, MilGraphicList, MIL.M_DRAW_POSITION, MIL.M_DEFAULT, MIL.M_DEFAULT);

                MIL.MmodGetResult(MilModResult, 0, MIL.M_POSITION_X + MIL.M_TYPE_MIL_DOUBLE, ref ModelPosX);
                MIL.MmodGetResult(MilModResult, 0, MIL.M_POSITION_Y + MIL.M_TYPE_MIL_DOUBLE, ref ModelPosY);

                MIL.MgraText(MIL.M_DEFAULT, MilGraphicList, ModelPosX - 20, ModelPosY, "Occurence Found");

                MIL.McalFixture(MilImage, MilFixturingOffset, MIL.M_MOVE_RELATIVE, MIL.M_RESULT_MOD,
                   MilModResult, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT);

                MIL.MmeasSetMarker(EdgeMarker, MIL.M_BOX_ORIGIN, EdgePosX, EdgePosY);
                MIL.MmeasSetMarker(EdgeMarker, MIL.M_POLARITY, POLARITY_LEFT, POLARITY_RIGHT);
                MIL.MmeasSetMarker(EdgeMarker, MIL.M_SEARCH_REGION_CLIPPING, MIL.M_ENABLE, MIL.M_NULL);

                MIL.MmeasSetMarker(EdgeMarker, MIL.M_BOX_SIZE, EdgeWidth, EdgeHeight);
                MIL.MmeasSetMarker(EdgeMarker, MIL.M_BOX_ANGLE, EdgeAngle, MIL.M_NULL);

                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_BLUE);
                MIL.MgraControl(MIL.M_DEFAULT, MIL.M_INPUT_UNITS, MIL.M_WORLD);
                MIL.MmeasDraw(MIL.M_DEFAULT, EdgeMarker, MilGraphicList, MIL.M_DRAW_SEARCH_REGION, MIL.M_DEFAULT, MIL.M_MARKER);
                MIL.MgraControl(MIL.M_DEFAULT, MIL.M_INPUT_UNITS, MIL.M_PIXEL);

                //MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_POSITION + MIL.M_STRIPE_WIDTH);

                MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, EdgeMarker, MIL.M_POSITION + MIL.M_LENGTH);

                MIL.MmeasGetResult(EdgeMarker, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref FindEdgeControl, MIL.M_NULL);

                if (FindEdgeControl > 0)
                {
                    MIL.MmeasGetResult(EdgeMarker, MIL.M_LENGTH, ref ResultWidth, MIL.M_NULL);

                    //MIL.MmeasGetResult(StripeMarker, MIL.M_STRIPE_WIDTH, ref ResultWidth, MIL.M_NULL);

                    //ResultWidth = Math.Round(ResultWidth, 4);

                    Console.Write("Width: {0} pixels.\n", ResultWidth);

                    MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_GREEN);
                    MIL.MgraText(MIL.M_DEFAULT, MilGraphicList, EdgePosX + 40, EdgePosY - 40, ResultWidth.ToString());

                    MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_RED);

                    MIL.MmeasDraw(MIL.M_DEFAULT, EdgeMarker, MilGraphicList, MIL.M_DRAW_EDGES, MIL.M_DEFAULT, MIL.M_RESULT);

                    //MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_WIDTH, MIL.M_DEFAULT, MIL.M_RESULT);

                }
                Console.Write("Press <Enter> to end.\n");
                Console.ReadKey();
            }
            else
            {
                Console.Write("Occurence Not Found");
            }
            
            MIL.MgraFree(MilGraphicList);
            MIL.MmeasFree(EdgeMarker);
            MIL.MmodFree(MilModContext);
            MIL.MmodFree(MilModResult);
            MIL.McalFree(MilFixturingOffset);
            MIL.MbufFree(MilSourceImage);
        }
    }
}
