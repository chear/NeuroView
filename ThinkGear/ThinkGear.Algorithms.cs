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

namespace NeuroSky.ThinkGear.Algorithms {

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

    public class FFT {
        private double[] x;
        private double[] y;

        /* dir: the direction. if dir = 1, this is an FFT. if dir = -1, this is an IFFT.
         * 
         * fftLength: the desired length of the FFT/IFFT. If fftLength is longer than the length of the input signal, then extra zeroes are added
         * to the end of the signal. If fftLength is shorter than the length of the input signal, the first fftLength number of points of the input
         * signal are used for the calculation. This is exactly how the matlab FFT function works
         * 
         * when using this as a forward FFT, pass the time domain data in the input1 variable, and pass the input2 variable as zeros (and equal 
         * in length to input1). The output variable is two single dimension vectors, x and y. x is the real component. y is the imaginary component.
         *
         * when using this as a reverse FFT, pass the real component in the input1 variable, and pass the imaginary component in the input2 variable. 
         * The output variable is two single dimension vectors, x and y. x is the real component. y is the imaginary component (usually on the order of 0)
         * 
         * example implementation:
           FFT fft = new FFT();
           double[] x = new double[512];
           Array.Copy(eegdata, x, 512);     //eegdata contains raw EEG data
           double[] y = new double[512];    //initialized to zeros
          
           double[] real = new double[512];
           double[] imag = new double[512];
         
           fft.calculateFFT(x, y, 1, 1024, out real, out imag);
         * 
         */
        public void calculateFFT(double[] input1, double[] input2, int dir, int fftLength, out double[] x, out double[] y) {
            int n, i, i1, j, k, i2, l, l1, l2, m;
            double c1, c2, tx, ty, t1, t2, u1, u2, z;

            x = new double[fftLength];
            y = new double[fftLength];

            //adjust the length of the input data, according to the fftLength
            Array.Copy(input1, x, Math.Min(input1.Length, fftLength));
            Array.Copy(input2, y, Math.Min(input2.Length, fftLength));

            //2^m = length of x, y
            m = (int)(Math.Log(x.Length) / Math.Log(2));

            /* Calculate the number of points */
            n = 1;
            for(i = 0; i < m; i++) {
                n *= 2;
            }

            /* Do the bit reversal */
            i2 = n >> 1;
            j = 0;
            for(i = 0; i < n - 1; i++) {
                if(i < j) {
                    tx = x[i];
                    ty = y[i];
                    x[i] = x[j];
                    y[i] = y[j];
                    x[j] = tx;
                    y[j] = ty;
                }
                k = i2;
                while(k <= j) {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }

            /* Compute the FFT */
            c1 = -1.0;
            c2 = 0.0;
            l2 = 1;
            for(l = 0; l < m; l++) {
                l1 = l2;
                l2 <<= 1;
                u1 = 1.0;
                u2 = 0.0;
                for(j = 0; j < l1; j++) {
                    for(i = j; i < n; i += l2) {
                        i1 = i + l1;
                        t1 = u1 * x[i1] - u2 * y[i1];
                        t2 = u1 * y[i1] + u2 * x[i1];
                        x[i1] = x[i] - t1;
                        y[i1] = y[i] - t2;
                        x[i] += t1;
                        y[i] += t2;
                    }
                    z = u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = Math.Sqrt((1.0 - c1) / 2.0);
                if(dir == 1)
                    c2 = -c2;
                c1 = Math.Sqrt((1.0 + c1) / 2.0);
            }

            /* Scaling for reverse transform */
            if(dir == -1) {
                for(i = 0; i < n; i++) {
                    x[i] /= n;
                    y[i] /= n;
                }
            }
        }
    }
}