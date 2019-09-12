using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using Matrox.MatroxImagingLibrary;

namespace _2_MatroxImageAcquisition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int DEFAULT_IMAGE_SIZE_X = 640;
        private const int DEFAULT_IMAGE_SIZE_Y = 480;
        public MIL_ID MilApplication = MIL.M_NULL;
        public MIL_ID MilSystem = MIL.M_NULL;
        public MIL_ID MilDisplay = MIL.M_NULL;
        public MIL_ID MilImage = MIL.M_NULL;

        private void mil(IntPtr UserWindowHandle)
        {
            MIL_ID MilDigitizer = MIL.M_NULL;

            MIL_INT BufSizeX = DEFAULT_IMAGE_SIZE_X;
            MIL_INT BufSizeY = DEFAULT_IMAGE_SIZE_Y;


            MIL_INT bufsx = DEFAULT_IMAGE_SIZE_X;
            MIL_INT bufsy = DEFAULT_IMAGE_SIZE_Y;

            MIL.MappAllocDefault(MIL.M_DEFAULT, ref MilApplication, ref MilSystem, ref MilDisplay, ref MilDigitizer, ref MilImage);

            MIL.MdispSelectWindow(MilDisplay, MilImage, UserWindowHandle);

            //MIL.MdispSelect(MilDisplay, MilImage);

            ////MIL.MdigAlloc(MilSystem, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref MilDigitizer);
            MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_X, ref BufSizeX);
            MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_Y, ref BufSizeY);

            //MIL.MdispControl(MilDisplay, MIL.M_SIZE_BIT, 8);
            // Resize the display window
            //if ((BufSizeX > DEFAULT_IMAGE_SIZE_X) || (BufSizeY > DEFAULT_IMAGE_SIZE_Y))
            //{
            //    FromHandle(UserWindowHandle).Size = new Size((int)BufSizeX, (int)BufSizeY);
            //}


            // MIL.MdispZoom(MilDisplay, -2.0, -2.0);

            //MIL.MdispControl(MilDisplay, MIL.M_VIEW_BIT_SHIFT, 8);
            MIL.MdispControl(MilDisplay, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE);
            MIL.MdispControl(MilDisplay, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE);  

            ////MIL.MdispInquire(MilDisplay, MIL.M_SIZE_X, ref DEFAULT_IMAGE_SIZE_X);
            //MIL_INT imageWidth, imageHeight;
            //imageWidth = MIL.MbufInquire(MilImage, MIL.M_SIZE_X, MIL.M_NULL);
            //imageHeight = MIL.MbufInquire(MilImage, MIL.M_SIZE_Y, MIL.M_NULL);

            //MIL.MbufInquire(MilImage, MIL.M_SIZE_X, bufsx);

            //MIL.MbufInquire(MilImage, MIL.M_SIZE_X, bufsy);

            if (MilDigitizer != MIL.M_NULL)
            {
                MIL.MdigGrabContinuous(MilDigitizer, MilImage);
                //MessageBox.Show("Image Acquisition", "Acquisition");
                //MIL.MdigHalt(MilDigitizer);
            }
            
            //MIL.MappFreeDefault(MilApplication, MilSystem, MilDisplay, MilDigitizer, MilImage);

        }


        private void btn_acquisition_Click(object sender, EventArgs e)
        {
            mil(Form1.ActiveForm.Handle);
        }
    }
}
