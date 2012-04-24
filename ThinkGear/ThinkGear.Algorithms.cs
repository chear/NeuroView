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
    //blink detection algorithm
    public class BlinkDetector {

        /* Defines the Blink States*/
        private enum BlinkState {
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

        private short i = 0;
        private short bufferCounter = 0;
        private short[] buffer = new short[BUFFER_SIZE];  /* initialize an array of size BUFFER_SIZE*/

        private const int DATA_MEAN = 33;
        private const int POS_VOLT_THRESHOLD = 230;  /* DATA_MEAN+265*/
        private const int NEG_VOLT_THRESHOLD = -200; /* DATA_MEAN-265*/
        private const int DISTANCE_THRESHOLD = 120;

        private const int INNER_DISTANCE_THRESHOLD = 45;

        private const int MAX_LEFT_RIGHT = 25;

        private const int MEAN_VARIABILITY = 200;
        private const int BLINK_LENGTH = 50;
        private const int MIN_MAX_DIFF = 500;

        private const int POORSIGNAL_THRESHOLD = 51;

        private BlinkState state = BlinkState.NO_BLINK;    /* initialize the variable "state" to NO_BLINK*/

        /* initialize various variables*/
        private short blinkStart = -1;
        private short outerLow = -1;
        private short innerLow = -1;
        private short innerHigh = -1;
        private short outerHigh = -1;
        private short blinkEnd = -1;

        private short maxValue = 0;
        private short minValue = 0;

        private short blinkStrength = 0;

        private double meanVariablityThreshold = 0;
        private double average = 0;




        /* This method returns a 0 if no blink was detected, and a non-zero value (1 to 255)
         * indicating the blink strength otherwise.
         */

        public byte Detect(byte poorSignalQualityValue, short eegValue) {
            if(poorSignalQualityValue < POORSIGNAL_THRESHOLD)    /*if poorSignal is less than 51, continue with algorithm*/ {
                /* update the buffer with the latest eegValue*/
                for(i = 0; i < BUFFER_SIZE - 1; i++) {
                    buffer[i] = buffer[i + 1];
                }
                buffer[BUFFER_SIZE - 1] = (short)eegValue;

                /* Counting the number of points in the buffer to make sure you have 512*/
                if(bufferCounter < 512) {
                    bufferCounter++;
                }

                if(bufferCounter > (BUFFER_SIZE - 1))    /* if the buffer is full (it has BUFFER_SIZE number of points)*/ {
                    switch(state) {
                        case BlinkState.NO_BLINK:

                            if(eegValue > POS_VOLT_THRESHOLD) {
                                blinkStart = -1;
                                innerLow = -1;
                                innerHigh = -1;
                                outerHigh = -1;
                                blinkEnd = -1;

                                outerLow = BUFFER_SIZE - 1;
                                maxValue = eegValue;
                                state = BlinkState.NORMAL_BLINK_UPPER;
                            }


                            if(eegValue < NEG_VOLT_THRESHOLD) {
                                blinkStart = -1;
                                innerLow = -1;
                                innerHigh = -1;
                                outerHigh = -1;
                                blinkEnd = -1;

                                outerLow = BUFFER_SIZE - 1;
                                minValue = eegValue;
                                state = BlinkState.INVERTED_BLINK_LOWER;
                            }

                            break;

                        case BlinkState.NORMAL_BLINK_UPPER:
                            /* Monitors the DISTANCE_THRESHOLD*/
                            if(((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD || outerLow < 1) {
                                state = BlinkState.NO_BLINK;
                            }

                            outerLow--;		//decrement the index of outerlow to account for shifting of the buffer

                            //Monitors the innerLow value.
                            if(eegValue <= POS_VOLT_THRESHOLD && buffer[BUFFER_SIZE - 2] > POS_VOLT_THRESHOLD)	//if the current value is less than POS_VOLT_THRESH and the previous value is greater than POS_VOLT_THRESH
                            {
                                innerLow = BUFFER_SIZE - 2;		//then innerLow is defined to be the previous value
                            } else {
                                innerLow--;
                            }

                            //Monitors the maximum value
                            if(eegValue > maxValue) maxValue = eegValue;

                            //When it hits the negative threshold, set that to be the innerHigh and set the state to NORMAL_BLINK_LOWER.
                            if(eegValue < NEG_VOLT_THRESHOLD)	//if we are below the NEG_VOLT_THRESH
                            {
                                innerHigh = BUFFER_SIZE - 1;	//innerHigh is the current value
                                minValue = eegValue;

                                //Verify the INNER_DISTANCE_THRESHOLD
                                if((innerHigh - innerLow) < INNER_DISTANCE_THRESHOLD)	//if the distance btwn innerHigh and innerLow isn't too long
                                {
                                    state = BlinkState.NORMAL_BLINK_LOWER;
                                } else		//otherwise the distance btwn innerHigh and innerLow is too much and it wasn't actually a blink
                                {
                                    state = BlinkState.NO_BLINK;
                                }
                            }

                            break;

                        case BlinkState.INVERTED_BLINK_LOWER:
                            /* Monitors the DISTANCE_THRESHOLD*/
                            if(((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD || outerLow < 1) {
                                state = BlinkState.NO_BLINK;
                                return 0;
                            }

                            outerLow--;

                            //Monitors the innerLow value.
                            if(eegValue >= NEG_VOLT_THRESHOLD && buffer[BUFFER_SIZE - 2] < NEG_VOLT_THRESHOLD) {
                                innerLow = BUFFER_SIZE - 2;
                            } else {
                                innerLow--;
                            }

                            //Monitors the minimum value
                            if(eegValue < minValue) minValue = eegValue;

                            //When it hits the positive threshold, set that to be innerHigh and set the state to INVERTED_BLINK_UPPER.
                            if(eegValue > POS_VOLT_THRESHOLD) {
                                innerHigh = BUFFER_SIZE - 1;
                                maxValue = eegValue;

                                //Verify the INNER_DISTANCE_THRESHOLD
                                if(innerHigh - innerLow < INNER_DISTANCE_THRESHOLD) {
                                    state = BlinkState.INVERTED_BLINK_UPPER;
                                } else {
                                    state = BlinkState.NO_BLINK;
                                }
                            }

                            break;

                        case BlinkState.NORMAL_BLINK_LOWER:
                            outerLow--;
                            innerLow--;
                            innerHigh--;

                            /* Monitors the outerHigh value*/
                            if(eegValue >= NEG_VOLT_THRESHOLD && buffer[BUFFER_SIZE - 2] < NEG_VOLT_THRESHOLD)	/* if the current value is greater than NEG_VOLT_THRESH and the previous value is less than NEG_VOLT_THRESH*/ {
                                outerHigh = BUFFER_SIZE - 2;		/* then the previous value is defined to be outerHigh*/
                                state = BlinkState.NORMAL_BLINK_VERIFY;
                            } else {
                                outerHigh--;
                            }

                            /* Monitors the minimum value*/
                            if(eegValue < minValue) minValue = eegValue;

                            /* Monitors the DISTANCE_THRESHOLD*/
                            if(((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD)    /* if the distance from the current point to outerLow is greater than DIST_THRESH*/ {
                                outerHigh = BUFFER_SIZE - 1;
                                state = BlinkState.NORMAL_BLINK_VERIFY;
                            }

                            break;

                        case BlinkState.INVERTED_BLINK_UPPER:
                            outerLow--;
                            innerLow--;
                            innerHigh--;

                            //Monitors the outerHigh value.
                            if((eegValue <= POS_VOLT_THRESHOLD) && (buffer[BUFFER_SIZE - 2] > POS_VOLT_THRESHOLD))		//if the current value is less than POS_VOLT_THRESH and the previous value is greater than POS_VOLT_THRESH
                            {
                                outerHigh = BUFFER_SIZE - 2;			//then the previous value is defined as outerHigh
                                state = BlinkState.INVERTED_BLINK_VERIFY;
                            } else {
                                outerHigh--;
                            }

                            //Monitors the maximum value
                            if(eegValue > maxValue) maxValue = eegValue;

                            //Monitors the DISTANCE_THRESHOLD
                            if(((BUFFER_SIZE - 1) - outerLow) > DISTANCE_THRESHOLD) {
                                outerHigh = BUFFER_SIZE - 1;
                                state = BlinkState.INVERTED_BLINK_VERIFY;
                            }
                            break;

                        case BlinkState.NORMAL_BLINK_VERIFY:
                            outerLow--;
                            innerLow--;
                            innerHigh--;

                            if(eegValue < NEG_VOLT_THRESHOLD) //if the current value is less than NEG_VOLT_THRES
                            {
                                state = BlinkState.NORMAL_BLINK_LOWER;
                            } else {
                                outerHigh--;
                            }

                            //Set the endBlink to when it hits the mean or it hits MAX_LEFT_RIGHT.
                            if(((BUFFER_SIZE - 1) - outerHigh > MAX_LEFT_RIGHT) || (eegValue > DATA_MEAN)) {
                                blinkEnd = BUFFER_SIZE - 1;
                            }

                            //Checks if the value is back at the DATA_MEAN
                            if(blinkEnd > 0) {
                                //Verifies the Blink
                                //Sets the blinkStart to when it hits the mean or it hits MAX_LEFT_RIGHT.
                                for(i = 0; i < MAX_LEFT_RIGHT; i++) {

                                    blinkStart = (short)(outerLow - i);

                                    if(buffer[outerLow - i] < DATA_MEAN) {
                                        break;
                                    }
                                }

                                //Verify the MIN_MAX_DIFF
                                blinkStrength = (short)(maxValue - minValue);
                                if(blinkStrength < MIN_MAX_DIFF) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }


                                //Verify the MEAN_VARIABILITY
                                meanVariablityThreshold = blinkStrength / 993 * MEAN_VARIABILITY;
                                average = 0;
                                for(i = blinkStart; i < blinkEnd + 1; i++) {
                                    average += buffer[i];
                                }
                                average /= (blinkEnd - blinkStart + 1);
                                /*take abs value of average*/
                                if(average < 0) {
                                    average = average * -1;
                                }

                                if(average > MEAN_VARIABILITY) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //Verify the BLINK_LENGTH
                                if(blinkEnd - blinkStart < BLINK_LENGTH) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkStart is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if((buffer[blinkStart] > POS_VOLT_THRESHOLD) || (buffer[blinkStart] < NEG_VOLT_THRESHOLD)) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkEnd is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if((buffer[blinkEnd] > POS_VOLT_THRESHOLD) || (buffer[blinkEnd] < NEG_VOLT_THRESHOLD)) {
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

                            if(eegValue > POS_VOLT_THRESHOLD) {
                                state = BlinkState.INVERTED_BLINK_UPPER;
                            } else {
                                outerHigh--;
                            }

                            //Set the endBlink to when it hits the mean or it hits MAX_LEFT_RIGHT.
                            if(((BUFFER_SIZE - 1) - outerHigh > MAX_LEFT_RIGHT) || (eegValue < DATA_MEAN)) {
                                blinkEnd = BUFFER_SIZE - 1;
                            }

                            //Checks if the value is back at the DATA_MEAN
                            if(blinkEnd > 0) {
                                //Verifies the Blink
                                //Sets the blinkStart to when it hits the mean or it hits MAX_LEFT_RIGHT.
                                for(i = 0; i < MAX_LEFT_RIGHT; i++) {
                                    blinkStart = (short)(outerLow - i);

                                    if(buffer[outerLow - i] > DATA_MEAN) {
                                        break;
                                    }
                                }

                                //Verify the MIN_MAX_DIFF
                                blinkStrength = (short)(maxValue - minValue);
                                if(blinkStrength < MIN_MAX_DIFF) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }



                                //Verify the MEAN_VARIABILITY
                                meanVariablityThreshold = blinkStrength / 993 * MEAN_VARIABILITY;
                                average = 0;
                                for(i = blinkStart; i < blinkEnd + 1; i++) {
                                    average += buffer[i];
                                }
                                average /= (blinkEnd - blinkStart + 1);
                                /*take abs value of average*/
                                if(average < 0) {
                                    average = average * -1;
                                }

                                if(average > MEAN_VARIABILITY) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //Verify the BLINK_LENGTH
                                if(blinkEnd - blinkStart < BLINK_LENGTH) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkStart is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if((buffer[blinkStart] > POS_VOLT_THRESHOLD) || (buffer[blinkStart] < NEG_VOLT_THRESHOLD)) {
                                    state = BlinkState.NO_BLINK;
                                    return 0;
                                }

                                //verify that blinkEnd is between POS_VOLT_THRESHOLD and NEG_VOLT_THRESHOLD
                                if((buffer[blinkEnd] > POS_VOLT_THRESHOLD) || (buffer[blinkEnd] < NEG_VOLT_THRESHOLD)) {
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
            } else	/* poorsignal is greater than 51 and do not evaluate the algorithm */ {
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

    //generalized FFT (equivalent to matlab)
    public class FFT {
        private double[] x;
        private double[] y;

        public FFTResult calculateFFT(double[] input1, double[] input2, int dir, int fftLength) {
            /* dir: the direction. if dir = 1, this is an FFT. if dir = -1, this is an IFFT.
            * 
            * fftLength: the desired length of the FFT/IFFT. If fftLength is longer than the length of the input signal, then extra zeroes are added
            * to the end of the signal. If fftLength is shorter than the length of the input signal, the first fftLength number of points of the input
            * signal are used for the calculation. This is exactly how the matlab FFT function works
            * 
            * when using this as a forward FFT, pass the time domain data in the input1 variable, and pass the input2 variable as zeros (and equal 
            * in length to input1).
            *
            * when using this as a reverse FFT, pass the real component in the input1 variable, and pass the imaginary component in the input2 variable. 
            * 
            * example implementation:
            FFT fft = new FFT();
            FFTResult fftResult = new FFTResult();
            double[] x = new double[512];
            Array.Copy(eegdata, x, 512);     //eegdata contains raw EEG data
            double[] y = new double[512];    //initialized to zeros
            
            fftResult = fft.calculateFFT(x, y, 1, 1024);
            double[] real = fftResult.getReal();
            double[] imag = fftResult.getImag();
            double[] power = fftResult.getPower();
            
            * 
            */

            int n, i, i1, j, k, i2, l, l1, l2, m;
            double c1, c2, tx, ty, t1, t2, u1, u2, z;

            x = new double[fftLength];
            y = new double[fftLength];

            //adjust the length of the input data, according to the fftLength
            Array.Copy(input1, 0, x, 0, Math.Min(input1.Length, fftLength));
            Array.Copy(input2, 0, y, 0, Math.Min(input2.Length, fftLength));

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

            return new FFTResult(x, y);

        }
    }

    //this handles the return object of the FFT class
    public class FFTResult {
        protected double[] real, imaginary, power;

        public FFTResult(double[] re, double[] im) {
            real = re;
            imaginary = im;
            power = calculatePower();
        }

        //return the real component
        public double[] getReal() {
            return real;
        }

        //return the imaginary component
        public double[] getImaginary() {
            return imaginary;
        }

        //return the power
        public double[] getPower() {
            return power;
        }

        //calculate the power (absolute value of real and imaginary)
        private double[] calculatePower() {
            power = new double[real.Length];

            for(int i = 0; i < power.Length; i++) {
                power[i] = (double)Math.Pow(Math.Pow(Math.Abs(real[i]), 2) + Math.Pow(Math.Abs(imaginary[i]), 2), 0.5);
            }

            return power;
        }
    }

    //hanning window
    public class HanningWindow {

        double[] coeffs;        //this holds the coefficients
        double[] postHanning;   //this holds the hanning windowed data

        //constructor
        public HanningWindow(int length) {
            coeffs = generateCoeffs(length);
        }

        //initialize the coefficients
        public double[] generateCoeffs(int length) {
            double[] hanncoeffs = new double[length];

            for(int i = 0; i < length; i++) {
                hanncoeffs[i] = 0.5 - 0.5 * Math.Cos(2 * Math.PI * i / (length - 1));
            }

            return hanncoeffs;
        }

        //apply coefficients to the data
        public double[] applyCoeffs(double[] data) {

            //re-initialize the array to clear it
            postHanning = new double[data.Length];

            for(int i = 0; i < data.Length; i++) {
                postHanning[i] = data[i] * coeffs[i];   
            }

            return postHanning;
        }
    }

    //algorithm to detect RR interval
    public class TGHrv {

        private short x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15;

        private int dataCount;		//count how many data points been processed within a second
        private int secondCount;	//count how many seconds of data been processed
        private bool rWaveUp;		//=1 indicates ECG wave is at around R wave peak
        private bool oldPeakFound;	//=1 indicates just found the previous peak
        private bool newPeakFound;	//=1 indicatess
        private int sampleRate; 	// sample rate
        private int rrInt;			// the calculated RR interval value 
        private int diffX;
        private int diffXNew;
        private int diffXOld;
        private int peakThresholdUp;
        private int peakThresholdDown;

        private int ThPcntU;		// threshold for comparing the Rpeak to previous Rpeak
        private int ThPcntD;
        private int pointsCount;
        private int minDistance; 		// closest time an R wave can be detected
        private int ibi;
        private int nbeats;
        private int realibi;
        private int HB0;

        private double HBUpL;
        private double HBDnL;

        public TGHrv() {
            // initialization
            Reset();
        }

        public void Reset() {
            x0 = x1 = x2 = x3 = x4 = x5 = x6 = x7 = x8 = x9 = x10 = x11 = x12 = x13 = x14 = x15 = 0;
            dataCount = 0;
            secondCount = 0;
            rWaveUp = false;
            oldPeakFound = false;
            newPeakFound = false;
            sampleRate = 512;
            rrInt = 0;
            diffX = 0;
            diffXNew = 3000;
            diffXOld = 3000;
            peakThresholdUp = 0;
            peakThresholdDown = 0;

            pointsCount = 0;
            ThPcntU = 55;
            ThPcntD = 20;
            minDistance = 153;
            HBUpL = 1.4;
            HBDnL = 0.6;
            HB0 = 0;
            ibi = 0;
            nbeats = 0;
            realibi = 153;
        }

        public int AddData(short data) {
            int returnval = -1;

            // when a new raw data is available
            x0 = x1;
            x1 = x2;
            x2 = x3;
            x3 = x4;
            x4 = x5;
            x5 = x6;
            x6 = x7;
            x7 = x8;
            x8 = x9;
            x9 = x10;
            x10 = x11;
            x11 = x12;
            x12 = x13;
            x13 = x14;
            x14 = x15;
            x15 = data;
            diffX = x0 - x15;

            if(dataCount == sampleRate * 2) {

                secondCount += 2;
                dataCount = 0;
                if(diffXOld > (3 * diffXNew) / 2) {
                    diffXNew = (3 * diffXNew) / 2;
                } else {
                    diffXNew = diffXOld;
                }

                diffXNew = Math.Min(diffXNew, 6000);
                diffXNew = Math.Max(diffXNew, 600);
                diffXOld = 0;
            }
            dataCount++;

            if(diffX >= diffXOld)
                diffXOld = diffX; // find out the Diff Max within 2 seconds;

            if(secondCount < 2) {
                diffXNew = diffXOld;
            }

            if(secondCount >= 0) {

                peakThresholdUp = diffXNew * ThPcntU / 100;
                peakThresholdDown = diffXNew * ThPcntD / 100;

                if(!rWaveUp && diffX >= peakThresholdUp) { // going up the slope
                    rWaveUp = true;
                    
                }

                if(rWaveUp && diffX <= peakThresholdDown && !newPeakFound) { // going down
                    oldPeakFound = true;
                }

                if(oldPeakFound && (diffX >= peakThresholdUp || -diffX >= (3 * peakThresholdUp) / 2 + 1)) { // looking for new peak
                    newPeakFound = true;

                    if(pointsCount < realibi) {
                        oldPeakFound = false;
                        if(nbeats < 2) pointsCount = 0;
                        rrInt = 0;
                    }

                    if(pointsCount >= realibi) { // checking if there is enough distance

                        oldPeakFound = false;
                        rrInt = pointsCount;
                        pointsCount = 0;

                    }
                }

                if((pointsCount == 0) && (rrInt != 0)) {
                    if(HB0 == 0) HB0 = rrInt;
                    else returnval = rrInt;

                    nbeats = nbeats + 1;
                    if(rrInt > 1500) {
                        nbeats = 0;
                        ibi = 0;
                        realibi = 153;
                    }
                    if(nbeats >= 3) {
                        if(ibi == 0) {
                            if(rrInt > 153 && rrInt < 1000)
                                ibi = rrInt;
                        } else {
                            if(rrInt > 153 && rrInt < (5 * ibi) / 2)
                                ibi = (rrInt + (4 * ibi)) / 5;
                        }
                    }
                    if(nbeats > 6 || ((ibi > 300) && (ibi < 400))) {
                        realibi = (3 * ibi) / 4;
                    }
                }

                if(newPeakFound) {
                    pointsCount++;
                    if(diffX <= peakThresholdDown) {
                        oldPeakFound = true;
                    }
                }
            }

            // return value
            return returnval;
        }
    }

    //energy level from Masa's MindWaveMac
    public class EnergyLevel {

        int[] t;    //this hold the timestamps for each RR interval
        int numSamples = 128;  //Specifies the number of FFT.
        int[] reSampledTime;                //this holds the timestamps of the interpolated data (at 500 msec intervals, effectively 2Hz sampling rate)
        int[] reSampledData;    //this holds the data resampled at 2Hz
        double[] fftInput;   //this holds the data to be FFT'd
        double[] real;      //this holds the real FFT result
        double[] imag;      //this holds the imaginary fft result
        double[] psd;           //this holds the power of the FFT result
        double[] zeros;         //this is always zeros, used as the fft Input2

        int sum = 0;
        int average = 0;

        int samplingFrequency = 2;  //the sampling fate 

        double fIndex;          //this is used to hold the current frequency bin
        double highBand;    //this holds the sum of the power from 0.15 to 0.4 hz  
        double lowBand;     //this holds the sum of the power from 0.04 to 0.15
        double energyIndex; //this holds the energy index, intermediate step
        int energyLevel;    //this holds the final output

        FFT fft;
        FFTResult fftResult;
        HanningWindow hanningWindow;

        //initialize stuff in the constructor
        public EnergyLevel() {
            reSampledTime = new int[numSamples];
            reSampledData = new int[numSamples];
            fftInput = new double[numSamples];
            psd = new double[numSamples];
            zeros = new double[numSamples];

            fft = new FFT();
            hanningWindow = new HanningWindow(numSamples);

        }

        //pass in an array of RR intervals, in milliseconds. and also pass in the length of the array
        public int calculateEnergyLevel(int[] rrIntervalInMS, int length) {
            t = new int[length];

            //Recreates the timestamps from the rrInterval Data.  Assumes that rrInterval are consecutive.
            t[0] = 0;
            for(int i = 1; i < length; i++) {
                t[i] = t[i - 1] + rrIntervalInMS[i];
            }

            
            //Checks to see if there is enough points to do a 128 point FFT.
            if(t[length - 1] < 63500) {
                Console.WriteLine("Need 64 seconds of data. Total ms = %d", t[length - 1]);
                return -1;
            }
            

            //Time index used for resampling
            for(int i = 0; i < numSamples; i++) {
                reSampledTime[i] = 1000 * i / 2;
            }

            //Resamples the data at 2Hz, which is 500ms
            reSampledData[0] = rrIntervalInMS[0];
            Console.WriteLine(reSampledData[0]);

            
            int n = 1;
            sum = reSampledData[0];     //TODO: this is missing in the mac version

            for(int k = 1; k < numSamples; k++) {

                while(t[n] <= reSampledTime[k]) {
                    n++;
                }

                reSampledData[k] = (int)(rrIntervalInMS[n - 1] + (float)((rrIntervalInMS[n] - rrIntervalInMS[n - 1]) * (reSampledTime[k] - t[n - 1])) / (float)rrIntervalInMS[n]);
                
            
                //Calculate the sum of all the points.
                sum += reSampledData[k];
        
            }

            //Calculate the average of the interpolated data.
            average = (int)(sum / numSamples);

            //Subtract the average from the data before doing the FFT
            for(int i = 0; i < numSamples; i++) {
                fftInput[i] = reSampledData[i] - average;
            }

            //apply the hanning window
            fftInput = hanningWindow.applyCoeffs(fftInput);

            //finally calcuate the power spectrum of the FFT. this is a 128 point FFT
            fftResult = fft.calculateFFT(fftInput, zeros, 1, numSamples);
            real = fftResult.getReal();
            imag = fftResult.getImaginary();

            //get the power
            for(int i = 0; i < numSamples; i++) {
                psd[i] = 2 * (Math.Pow(real[i], 2) + Math.Pow(imag[i], 2));
            }
            
            //now gather the frequency bands (high and low)
            highBand = 0;
            lowBand = 0;
            for(int i = 0; i < numSamples; i++) {
                fIndex = (double)(((double)samplingFrequency / (double)numSamples) * i);

                if((fIndex >= 0.15) && (fIndex <= 0.4)) {
                    highBand += psd[i];     //high band
                }

                if((fIndex >= 0.04) && (fIndex <= 0.15)) {
                    lowBand += psd[i];   //Low band
                }

            }

            //energyIndex = (3.0 - MIN(8.0, lowBand/highBand)) / 2.24;  //Original Equation
            energyIndex = (5.005 - lowBand / highBand) / 3.33;  //New Equation by Calculated with assuming range of LF/HF of 0.01 to 10.

            energyIndex = Math.Max(-1.5, energyIndex);
            energyIndex = Math.Min(1.5, energyIndex);

            energyLevel = (int)(Math.Round(50.0 + 33.0 * energyIndex));

            return energyLevel;
        }

    }
}