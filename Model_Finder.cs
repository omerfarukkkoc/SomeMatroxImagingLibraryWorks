using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Matrox.MatroxImagingLibrary;

namespace _4_MatroxModelFinderContinuous
{
    class Program
    {
        static void Main(string[] args)
        {
            Acquisition();
        }


        static string appPath = AppDomain.CurrentDomain.BaseDirectory;

        static string MODEL_FILE = appPath + "barcodeModel.mmf";
        


        static void Acquisition()
        {
            MIL_ID MilApplication = MIL.M_NULL;
            MIL_ID MilSystem = MIL.M_NULL;
            MIL_ID MilDisplay = MIL.M_NULL;
            MIL_ID MilDigitizer = MIL.M_NULL;
            MIL_ID MilImage = MIL.M_NULL;
                        
            MIL.MappAllocDefault(MIL.M_DEFAULT, ref MilApplication, ref MilSystem, ref MilDisplay, ref MilDigitizer, ref MilImage);

            MIL.MdispSelect(MilDisplay, MilImage);

            //if (MilDigitizer != MIL.M_NULL)
            //{
            //    MIL.MdigGrabContinuous(MilDigitizer, MilImage);
            //    //FindModel(MilSystem, MilDisplay, MilImage);
            //    //FindModel(MilSystem, MilDisplay, MilDigitizer, MilImage);
                FindModel(MilSystem, MilDisplay, MilDigitizer, MilImage);
            //    Console.ReadKey();
            //    MIL.MdigHalt(MilDigitizer);
            //}

            MIL.MappFreeDefault(MilApplication, MilSystem, MilDisplay, MilDigitizer, MilImage);
        }

        

        static void FindModel(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID MilDigitizer, MIL_ID MilImage)
        {
            MIL_ID GraphicList = MIL.M_NULL;
            MIL_ID MilModelImage = MIL.M_NULL;
            MIL_ID MilModelFinderResult = MIL.M_NULL;

            double Value = 0.0;

            MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref GraphicList);

            MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_DARK_RED);

            MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, GraphicList);

            MIL.MmodAlloc(MilSystem, MIL.M_GEOMETRIC, MIL.M_DEFAULT, ref MilModelImage);

            //MIL.MmodDefineFromFile(MIL.M_GEOMETRIC, MIL.M_IMAGE, MODEL_FILE, MIL.M_DEFAULT);

            MIL.MmodRestore(MODEL_FILE, MilSystem, MIL.M_DEFAULT, ref MilModelImage);

            MIL.MmodAllocResult(MilSystem, MIL.M_DEFAULT, ref MilModelFinderResult);

            MIL.MmodPreprocess(MilModelImage, MIL.M_DEFAULT);

            MIL.MdigGrabContinuous(MilDigitizer, MilImage);

            if (MilDigitizer != MIL.M_NULL)
            {
                //MIL.MdigGrab(MilDigitizer, MilImage);
                do
                {
                    MIL.MmodFind(MilModelImage, MilImage, MilModelFinderResult);

                    MIL.MmodGetResult(MilModelFinderResult, MIL.M_GENERAL, MIL.M_NUMBER, ref Value);

                    if (Value == 1)
                    {
                        MIL.MmodDraw(MIL.M_DEFAULT, MilModelFinderResult, GraphicList, MIL.M_DRAW_POSITION + MIL.M_DRAW_BOX, MIL.M_DEFAULT, MIL.M_DEFAULT);
                        Console.Write("Found occurrence using MIL Model Finder.\n");
                        //Console.ReadKey();
                    }
                    else
                    {
                        Console.Write("Occurrence not found.\n");
                        //Console.ReadKey();
                    }
                    MIL.MgraClear(MIL.M_DEFAULT, GraphicList);
                }
                while (!Console.KeyAvailable);
            }
            MIL.MgraFree(GraphicList);
            MIL.MbufFree(MilModelImage);
            MIL.MmodFree(MilModelFinderResult);
            MIL.MbufFree(MilImage);
        }
        
    }
}
