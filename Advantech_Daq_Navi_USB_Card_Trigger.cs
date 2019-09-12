using System;
using System.Collections.Generic;

using Matrox.DesignAssistant.Core;
using Matrox.DesignAssistant.Core.CommonTypes;
using Matrox.DesignAssistant.Core.Expressions;
using Matrox.DesignAssistant.Core.Steps;
using Matrox.DesignAssistant.Core.Steps.Attributes;
using Matrox.DesignAssistant.Core.Steps.Annotations;

using Matrox.MatroxImagingLibrary;
using System.Globalization;
using Automation.BDaq;

namespace StrobeTetikAcik
{
    //Add additional attributes as required:
    //[ResponsibleOfMILAllocations]
    [Description("inovakomerfaruk")]
    [UIEditor("StrobeTetikAcik.StrobeTetikAcikUIEditor")]
    public class StrobeTetikAcik : Step
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
        protected override void Run()
        {
            ////Insert Step logic here

            //Image img = Image;
            //_numericOutput = NumericInput * img.SizeX * img.SizeY;
            InstantDoCtrl instantDoCtrl1 = new InstantDoCtrl();
            instantDoCtrl1.SelectedDevice = new DeviceInformation(1);
            if (instantDoCtrl1.Initialized)
            {
                DoBitInformation boxInfo = new DoBitInformation();
                boxInfo.BitNum = 7;
                boxInfo.BitValue = 1;
                boxInfo.PortNum = 0;
                int state = 0;
                instantDoCtrl1.Write(boxInfo.PortNum, (byte)state);
            }
        }

        protected override void CreateAnnotations(StepAnnotationCollection annotations)
        {
            //Create annotations here if needed
            //annotations.AddAnnotation(...)
        }
    }
    public struct DoBitInformation
    {
        #region fields
        private int m_bitValue;
        private int m_portNum;
        private int m_bitNum;
        #endregion

        public DoBitInformation(int bitvalue, int portNum, int bitNum)
        {
            m_bitValue = bitvalue;
            m_portNum = portNum;
            m_bitNum = bitNum;
        }

        #region Properties
        public int BitValue
        {
            get { return m_bitValue; }
            set
            {
                m_bitValue = value & 0x1;
            }
        }
        public int PortNum
        {
            get { return m_portNum; }
            set
            {
                if ((value - ConstVal.StartPort) >= 0
                   && (value - ConstVal.StartPort) <= (ConstVal.PortCountShow - 1))
                {
                    m_portNum = value;
                }
            }
        }
        public int BitNum
        {
            get { return m_bitNum; }
            set
            {
                if (value >= 0 && value <= 7)
                {
                    m_bitNum = value;
                }
            }
        }
        #endregion
    }

    public static class ConstVal
    {
        public const int StartPort = 0;
        public const int PortCountShow = 4;
    }
}