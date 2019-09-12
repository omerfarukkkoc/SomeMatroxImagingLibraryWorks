using System;
using System.Collections.Generic;

using Matrox.DesignAssistant.Core;
using Matrox.DesignAssistant.Core.CommonTypes;
using Matrox.DesignAssistant.Core.Expressions;
using Matrox.DesignAssistant.Core.Steps;
using Matrox.DesignAssistant.Core.Steps.Attributes;
using Matrox.DesignAssistant.Core.Steps.Annotations;

using Matrox.MatroxImagingLibrary;

using IronOcr;

namespace IronOCRCustomStep1
{
    //Add additional attributes as required:
    //[ResponsibleOfMILAllocations]
    [Description("inovakomerfaruk")]
    [UIEditor("IronOCRCustomStep1.IronOCRCustomStep1UIEditor")]
    public class IronOCRCustomStep1 : Step
    {
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
        public Image Image
        {
             get { return (Image)GetInputValue("Image"); }
        }

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

            Image img = Image;
            _numericOutput = NumericInput * img.SizeX * img.SizeY;

            AutoOcr OCR = new AutoOcr() { ReadBarCodes = false };
            OcrResult Results = OCR.Read("C:\\Users\\inovakomerfaruk\\Desktop\\Image.bmp");
            _OcrResult = Results.Text;
        }

        protected override void CreateAnnotations(StepAnnotationCollection annotations)
        {
            //Create annotations here if needed
            //annotations.AddAnnotation(...)
        }
    }
}