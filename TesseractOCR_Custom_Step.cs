using System;
using System.Collections.Generic;

using Matrox.DesignAssistant.Core;
using Matrox.DesignAssistant.Core.CommonTypes;
using Matrox.DesignAssistant.Core.Expressions;
using Matrox.DesignAssistant.Core.Steps;
using Matrox.DesignAssistant.Core.Steps.Attributes;
using Matrox.DesignAssistant.Core.Steps.Annotations;

using Matrox.MatroxImagingLibrary;

using Tesseract;
using System.Drawing;
using System.Diagnostics;


namespace OCRCustomStep1
{
    //Add additional attributes as required:
    //[ResponsibleOfMILAllocations]
    [Description("inovakomerfaruk")]
    [UIEditor("OCRCustomStep1.OCRCustomStep1UIEditor")]
    public class OCRCustomStep1 : Step
    {
        //[Input]
        //[Linkable]
        ////Add additional attributes as required:
        ////[ConstantValue]
        ////[InitialValue]
        ////[DynamicInitialValue]
        ////[DynamicValueNames]
        ////[AvailabilityConstraint]
        ////[Category]
        ////[MIL]
        ////[UIEditor]
        //public Image Image
        //{
        //     get { return (Image)GetInputValue("Image"); }
        //}

        [Input]
        [Linkable]
        //Add additional attributes as required:
        //[ConstantValue]
        //[InitialValue]
        //[DynamicInitialValue]
        //[DynamicValueNames]
        //[AvailabilityConstraint]
        //[Category]
        //[MIL]
        //[UIEditor]
        public double NumericInput
        {
            get { return (double)GetInputValue("NumericInput"); }
        }

        [Output]
        //Add additional attributes as required:
        //[Category]        
        public double NumericOutput
        {
            get
            {
                ValidateOutputAvailability("NumericOutput");
                return _numericOutput;
            }
        }
        private double _numericOutput;


        [Output]
        public String OcrResult
        {
            get
            {
                ValidateOutputAvailability("OcrResult");
                return _OcrResult;
            }
        }
        private String _OcrResult;


        protected override void Run()
        {
            //Insert Step logic here

            //Image img = Image;
            //_numericOutput = NumericInput * img.SizeX * img.SizeY;

            try
            {
                Bitmap bmp = new Bitmap("C:\\Users\\inovakomerfaruk\\Desktop\\Image.bmp");
                
                var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractAndCube);

                var page = ocr.Process(bmp);

                _OcrResult = page.GetText();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.InnerException.Message.ToString());
                Debug.Write(e.InnerException.Message.ToString());
                _OcrResult = e.InnerException.Message.ToString();
            }
        }

        protected override void CreateAnnotations(StepAnnotationCollection annotations)
        {
            //Create annotations here if needed
            //annotations.AddAnnotation(...)
        }
    }
}