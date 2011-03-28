/*
 * Dyana Blink Algorithm
 * update 2
 * 11.8.10
 * 
 * cs code implemented by Neraj Bobra
 */



using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

using AForge.Math;

namespace NeuroSky.ThinkGear.Algorithms {

    public class AlphaAttenuation {

        public double OpenDivideByClose;
        public double CloseMinusOpen;

        public event EventHandler CalculationError = delegate { };
        public event EventHandler CalculationDone = delegate { };

        private Thread CalculationThread;
        private bool FFTThreadEnabled;
        private double[] EyesOpenData;
        private double[] EyesCloseData;
        private double[] FilteredOpenData;
        private double[] FilteredCloseData;
        private Complex[] FFTEyesOpenData = new Complex[512];
        private Complex[] FFTEyesCloseData = new Complex[512];
        private double[] EyesOpenAlpha;
        private double[] EyesCloseAlpha;

        public int StartCalculate(double[] eyesOpenData, double[] eyesCloseData) {
            EyesOpenData = new double[eyesOpenData.Length];
            EyesCloseData = new double[eyesCloseData.Length];

            //Copies the data.
            Array.Copy(eyesOpenData, EyesOpenData, eyesOpenData.Length);
            Array.Copy(eyesCloseData, EyesCloseData, eyesCloseData.Length);

            //If there is not enough data points, return an error.
            if (EyesOpenData.Length < 512 || EyesCloseData.Length < 512) {
                Console.WriteLine("Not enough data.");
                return -1;
            }

            //Run the calculation thread
            CalculationThread = new Thread(AlphaCalculation);
            CalculationThread.Start();

            return 0;
        }

        private void AlphaCalculation() {
            //Check the average and subtract the average from the data.
            double eyesOpenDataAverage = Average(EyesOpenData);
            double eyesCloseDataAverage = Average(EyesCloseData);

            for (int i = 0; i < EyesOpenData.Length; i++) {
                EyesOpenData[i] -= eyesOpenDataAverage;
            }

            for (int i = 0; i < EyesCloseData.Length; i++) {
                EyesCloseData[i] -= eyesCloseDataAverage;
            }

            Console.WriteLine("Calculating EyeOpenAlpha");
            EyesOpenAlpha = Calculate(EyesOpenData);
            Console.WriteLine("Calculating EyeCloseAlpha");
            EyesCloseAlpha = Calculate(EyesCloseData);

            //Get the average value for the alpha.
            double eyesOpenAlphaAverage = Average(EyesOpenAlpha);
            double eyesCloseAlphaAverage = Average(EyesCloseAlpha);

            if (Double.IsNaN(eyesCloseAlphaAverage) || Double.IsNaN(eyesOpenAlphaAverage)) {
                CalculationError(this, EventArgs.Empty);
                return;
            }

            Console.WriteLine("EyesOpenAlphaAverage: " + eyesOpenAlphaAverage);
            Console.WriteLine("EyesCloseAlphaAverage: " + eyesCloseAlphaAverage);

            OpenDivideByClose = eyesOpenAlphaAverage / eyesCloseAlphaAverage;
            CloseMinusOpen = eyesCloseAlphaAverage - eyesOpenAlphaAverage;

            CalculationDone(this, EventArgs.Empty);

            Console.WriteLine("Exiting Calculation Thread");
        }

        private double Average(double[] data) {
            double sum = 0;
            double average = 0;
            foreach (double d in data) {
                sum += d;
            }

            average = sum / data.Length;
            
            return average;
        }

        private double[] Calculate( double[] data) {
            List<double> alphaDataList = new List<double>();
            int totalNoisyData= 0;

            int i = 0;
            bool isClean;
            while (i+512 < data.Length) {
                double[] tempArray = new double[512];

                isClean = true;
                for (int j = 0; j < 512; j++) {

                    if (data[i + j] < 200 && data[i + j] > -200) {
                        tempArray[j] = data[i + j];
                    }
                    else {
                        isClean = false;
                        totalNoisyData++;
                        break;
                    }
                }

                Complex[] FFTArray = new Complex[512];

                //If there is no noise add it into the FFT Array.
                if (isClean) {
                    for(int k = 0; k < 512; k++ ) {
                        FFTArray[k].Re = tempArray[k];
                    }

                    FourierTransform.FFT(FFTArray, FourierTransform.Direction.Forward);

                    double sum = 0;
                    for (int l = 9; l < 11; l++) {
                        sum += FFTArray[l].Magnitude;
                    }

                    alphaDataList.Add(sum);

                }

                i += 512;
            }

            Console.WriteLine("TotalNoisyData was " + totalNoisyData + "s");
            return alphaDataList.ToArray();
        }

        
    }

    public class BlinkDetector
    {

        /* Defines the Blink States*/
        private enum BlinkState
        {
            NO_BLINK,
            NORMAL_BLINK_UPPER,
            NORMAL_BLINK_LOWER,
            NORMAL_BLINK_VERIFY,
            INVERTED_BLINK_LOWER,
            INVERTED_BLINK_UPPER,
            INVERTED_BLINK_VERIFY
        }

        private const int SHIFTING_TERM = 4;

        private const int BUFFER_SIZE = 512;

        private short i             = 0;
        private short bufferCounter = 0;
        private short[] buffer      = new short[BUFFER_SIZE];  /* initialize an array of size BUFFER_SIZE*/

        private const int DATA_MEAN          = 33;
        private const int POS_VOLT_THRESHOLD = 230;  /* DATA_MEAN+265*/
        private const int NEG_VOLT_THRESHOLD = -200; /* DATA_MEAN-265*/
        private const int DISTANCE_THRESHOLD = 120;

        private const int INNER_DISTANCE_THRESHOLD = 45;

        private const int MAX_LEFT_RIGHT = 25;

        private const int MEAN_VARIABILITY = 200;
        private const int BLINK_LENGTH     = 50;
        private const int MIN_MAX_DIFF     = 500;

        private const int POORSIGNAL_THRESHOLD = 51;   

        private BlinkState state = BlinkState.NO_BLINK;    /* initialize the variable "state" to NO_BLINK*/

        /* initialize various variables*/
        private short blinkStart = -1;
        private short outerLow   = -1;
        private short innerLow   = -1;
        private short innerHigh  = -1;
        private short outerHigh  = -1;
        private short blinkEnd   = -1;

        private short maxValue = 0;
        private short minValue = 0;

        private short blinkStrength = 0;

        private double meanVariablityThreshold = 0;
        private double average                 = 0;




        /* This method returns a 0 if no blink was detected, and a non-zero value (1 to 255)
         * indicating the blink strength otherwise.
         */

        public byte Detect(byte poorSignalQualityValue, short eegValue)
        {
            if (poorSignalQualityValue < POORSIGNAL_THRESHOLD)    /*if poorSignal is less than 51, continue with algorithm*/
            {
                /* update the buffer with the latest eegValue*/
                for (i = 0; i < BUFFER_SIZE - 1; i++)
                {
                    buffer[i] = buffer[i + 1];
                }
                buffer[BUFFER_SIZE - 1] = (short)eegValue;

                /* Counting the number of points in the buffer to make sure you have 512*/
                if (bufferCounter < 512)
                {
                    bufferCounter++;
                }

                if ( bufferCounter > (BUFFER_SIZE - 1) )    /* if the buffer is full (it has BUFFER_SIZE number of points)*/
                {
                    switch (state)
                    {
                        case BlinkState.NO_BLINK:

                            if (eegValue > POS_VOLT_THRESHOLD)
                            {
                                blinkStart = -1;
                                innerLow   = -1;
                                innerHigh  = -1;
                                outerHigh  = -1;
                                blinkEnd   = -1;

                                outerLow = BUFFER_SIZE - 1;
                                maxValue = eegValue;
                                state    = BlinkState.NORMAL_BLINK_UPPER;
                            }

                            
                            if (eegValue < NEG_VOLT_THRESHOLD)
                            {
                                blinkStart = -1;
                                innerLow   = -1;
                                innerHigh  = -1;
                                outerHigh  = -1;
                                blinkEnd   = -1;

                                outerLow = BUFFER_SIZE - 1;
                                minValue = eegValue;
                                state    = BlinkState.INVERTED_BLINK_LOWER;
                            }

                            break;

                        case BlinkState.NORMAL_BLINK_UPPER:
                            /* Monitors the DISTANCE_THRESHOLD*/
                            if (((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD || outerLow < 1)
                            {
                                state = BlinkState.NO_BLINK;
                            }

                            outerLow--;		//decrement the index of outerlow to account for shifting of the buffer

                            //Monitors the innerLow value.
                            if (eegValue <= POS_VOLT_THRESHOLD && buffer[BUFFER_SIZE - 2] > POS_VOLT_THRESHOLD)	//if the current value is less than POS_VOLT_THRESH and the previous value is greater than POS_VOLT_THRESH
                            {
                                innerLow = BUFFER_SIZE - 2;		//then innerLow is defined to be the previous value
                            }
                            else
                            {
                                innerLow--;
                            }

                            //Monitors the maximum value
                            if (eegValue > maxValue) maxValue = eegValue;

                            //When it hits the negative threshold, set that to be the innerHigh and set the state to NORMAL_BLINK_LOWER.
                            if (eegValue < NEG_VOLT_THRESHOLD)	//if we are below the NEG_VOLT_THRESH
                            {
                                innerHigh = BUFFER_SIZE - 1;	//innerHigh is the current value
                                minValue = eegValue;

                                //Verify the INNER_DISTANCE_THRESHOLD
                                if ((innerHigh - innerLow) < INNER_DISTANCE_THRESHOLD)	//if the distance btwn innerHigh and innerLow isn't too long
                                {
                                    state = BlinkState.NORMAL_BLINK_LOWER;
                                }
                                else		//otherwise the distance btwn innerHigh and innerLow is too much and it wasn't actually a blink
                                {
                                    state = BlinkState.NO_BLINK;
                                }
                            }

                            break;

                        case BlinkState.INVERTED_BLINK_LOWER:
                            /* Monitors the DISTANCE_THRESHOLD*/
                            if (((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD || outerLow < 1)
                            {
                                state = BlinkState.NO_BLINK;
                                return 0;
                            }

                            outerLow--;

                            //Monitors the innerLow value.
                            if (eegValue >= NEG_VOLT_THRESHOLD && buffer[BUFFER_SIZE - 2] < NEG_VOLT_THRESHOLD)
                            {
                                innerLow = BUFFER_SIZE - 2;
                            }
                            else
                            {
                                innerLow--;
                            }

                            //Monitors the minimum value
                            if (eegValue < minValue) minValue = eegValue;

                            //When it hits the positive threshold, set that to be innerHigh and set the state to INVERTED_BLINK_UPPER.
                            if (eegValue > POS_VOLT_THRESHOLD)
                            {
                                innerHigh = BUFFER_SIZE - 1;
                                maxValue = eegValue;

                                //Verify the INNER_DISTANCE_THRESHOLD
                                if (innerHigh - innerLow < INNER_DISTANCE_THRESHOLD)
                                {
                                    state = BlinkState.INVERTED_BLINK_UPPER;
                                }
                                else
                                {
                                    state = BlinkState.NO_BLINK;
                                }
                            }

                            break;

                        case BlinkState.NORMAL_BLINK_LOWER:
                            outerLow--;
                            innerLow--;
                            innerHigh--;

                            /* Monitors the outerHigh value*/
                            if (eegValue >= NEG_VOLT_THRESHOLD && buffer[BUFFER_SIZE - 2] < NEG_VOLT_THRESHOLD)	/* if the current value is greater than NEG_VOLT_THRESH and the previous value is less than NEG_VOLT_THRESH*/
                            {
                                outerHigh = BUFFER_SIZE - 2;		/* then the previous value is defined to be outerHigh*/
                                state = BlinkState.NORMAL_BLINK_VERIFY;
                            }
                            else
                            {
                                outerHigh--;
                            }

                            /* Monitors the minimum value*/
                            if (eegValue < minValue) minValue = eegValue;

                            /* Monitors the DISTANCE_THRESHOLD*/
                            if (((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD)    /* if the distance from the current point to outerLow is greater than DIST_THRESH*/
                            {
                                outerHigh = BUFFER_SIZE - 1;
                                state = BlinkState.NORMAL_BLINK_VERIFY;
                            }

                            break;

                        case BlinkState.INVERTED_BLINK_UPPER:
                            outerLow--;
                            innerLow--;
                            innerHigh--;

                            //Monitors the outerHigh value.
                            if ((eegValue <= POS_VOLT_THRESHOLD) && (buffer[BUFFER_SIZE - 2] > POS_VOLT_THRESHOLD))		//if the current value is less than POS_VOLT_THRESH and the previous value is greater than POS_VOLT_THRESH
                            {
                                outerHigh = BUFFER_SIZE - 2;			//then the previous value is defined as outerHigh
                                state = BlinkState.INVERTED_BLINK_VERIFY;
                            }
                            else
                            {
                                outerHigh--;
                            }

                            //Monitors the maximum value
                            if (eegValue > maxValue) maxValue = eegValue;

                            //Monitors the DISTANCE_THRESHOLD
                            if (((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD)
                            {
                                outerHigh = BUFFER_SIZE - 1;
                                state = BlinkState.INVERTED_BLINK_VERIFY;
                            }
                            break;

                        case BlinkState.NORMAL_BLINK_VERIFY:
                            outerLow--;
                            innerLow--;
                            innerHigh--;

                            if (eegValue < NEG_VOLT_THRESHOLD) //if the current value is less than NEG_VOLT_THRES
                            {
                                state = BlinkState.NORMAL_BLINK_LOWER;
                            }
                            else
                            {
                                outerHigh--;
                            }

                            //Set the endBlink to when it hits the mean or it hits MAX_LEFT_RIGHT.
                            if (((BUFFER_SIZE - 1) - outerHigh > MAX_LEFT_RIGHT) || (eegValue > DATA_MEAN))
                            {
                                blinkEnd = BUFFER_SIZE - 1;
                            }

                            //Checks if the value is back at the DATA_MEAN
                            if (blinkEnd > 0)
                            {
                                //Verifies the Blink
                                //Sets the blinkStart to when it hits the mean or it hits MAX_LEFT_RIGHT.
                                for (i = 0; i < MAX_LEFT_RIGHT; i++)
                                {

                                    blinkStart = (short)(outerLow - i);

                                    if (buffer[outerLow - i] < DATA_MEAN)
                                    {
                                        break;
                                    }
                                }

                                //Verify the MIN_MAX_DIFF
                                blinkStrength = (short)(maxValue - minValue);
                                if (blinkStrength < MIN_MAX_DIFF)
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }


                                //Verify the MEAN_VARIABILITY
                                meanVariablityThreshold = blinkStrength / 993 * MEAN_VARIABILITY;
                                average = 0;
                                for (i = blinkStart; i < blinkEnd + 1; i++)
                                {
                                    average += buffer[i];
                                }
                                average /= (blinkEnd - blinkStart + 1);
                                /*take abs value of average*/
                                if (average < 0)
                                {
                                    average = average * -1;
                                }
                                                                
                                if (average > MEAN_VARIABILITY)
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //Verify the BLINK_LENGTH
                                if (blinkEnd - blinkStart < BLINK_LENGTH)
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkStart is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if ((buffer[blinkStart] > POS_VOLT_THRESHOLD) || (buffer[blinkStart] < NEG_VOLT_THRESHOLD))
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkEnd is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if ((buffer[blinkEnd] > POS_VOLT_THRESHOLD) || (buffer[blinkEnd] < NEG_VOLT_THRESHOLD))
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                state = BlinkState.NO_BLINK;
                                return (byte)(blinkStrength >> SHIFTING_TERM);
                            }

                            break;

                        case BlinkState.INVERTED_BLINK_VERIFY:
                            outerLow--;
                            innerLow--;
                            innerHigh--;

                            if (eegValue > POS_VOLT_THRESHOLD)
                            {
                                state = BlinkState.INVERTED_BLINK_UPPER;
                            }
                            else
                            {
                                outerHigh--;
                            }

                            //Set the endBlink to when it hits the mean or it hits MAX_LEFT_RIGHT.
                            if (((BUFFER_SIZE - 1) - outerHigh > MAX_LEFT_RIGHT) || (eegValue < DATA_MEAN))
                            {
                                blinkEnd = BUFFER_SIZE - 1;
                            }

                            //Checks if the value is back at the DATA_MEAN
                            if (blinkEnd > 0)
                            {
                                //Verifies the Blink
                                //Sets the blinkStart to when it hits the mean or it hits MAX_LEFT_RIGHT.
                                for (i = 0; i < MAX_LEFT_RIGHT; i++)
                                {
                                    blinkStart = (short)(outerLow - i);

                                    if (buffer[outerLow - i] > DATA_MEAN)
                                    {
                                        break;
                                    }
                                }

                                //Verify the MIN_MAX_DIFF
                                blinkStrength = (short)(maxValue - minValue);
                                if (blinkStrength < MIN_MAX_DIFF)
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }



                                //Verify the MEAN_VARIABILITY
                                meanVariablityThreshold = blinkStrength / 993 * MEAN_VARIABILITY;
                                average = 0;
                                for (i = blinkStart; i < blinkEnd + 1; i++)
                                {
                                    average += buffer[i];
                                }
                                average /= (blinkEnd - blinkStart + 1);
                                /*take abs value of average*/
                                if (average < 0)
                                {
                                    average = average * -1;
                                }

                                if (average > MEAN_VARIABILITY)
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //Verify the BLINK_LENGTH
                                if (blinkEnd - blinkStart < BLINK_LENGTH)
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkStart is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if ((buffer[blinkStart] > POS_VOLT_THRESHOLD) || (buffer[blinkStart] < NEG_VOLT_THRESHOLD))
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkEnd is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if ((buffer[blinkEnd] > POS_VOLT_THRESHOLD) || (buffer[blinkEnd] < NEG_VOLT_THRESHOLD))
                                {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                state = BlinkState.NO_BLINK;
                                return (byte)(blinkStrength >> SHIFTING_TERM);
                            }

                            break;

                        default:
                            state = BlinkState.NO_BLINK;

                            break;
                    }
                }
            }
            else	/* poorsignal is greater than 51 and do not evaluate the algorithm */
            {
                bufferCounter = 0;
                outerLow = -1;
                innerLow = -1;
                innerHigh = -1;
                outerHigh = -1;

                state = BlinkState.NO_BLINK;
            }

            return 0;
        }
    }
}