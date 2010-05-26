using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace NeuroSky.ThinkGear.Algorithms {
    class BlinkDetector {
        private const int BLINK_DETECT_UPPER_THRESHOLD = 400;
        private const int BLINK_DETECT_LOWER_THRESHOLD = -200;

        private const int UPPER_BLINK_MIN_TIME = 5;
        private const int UPPER_BLINK_MAX_TIME = 75;

        private const int MIDDLE_BLINK_MIN_TIME = 0;
        private const int MIDDLE_BLINK_MAX_TIME = 40;
        
        private const int LOWER_BLINK_MIN_TIME = 10;
        private const int LOWER_BLINK_MAX_TIME = 110;

        private const int PQ_THRESHOLD = 27;

        private const int SHIFTING_TERM = 4;

        private enum BlinkState {
            NoBlink,
            UpperBlink,
            MiddleBlink,
            LowerBlink
        }

        private BlinkState state = BlinkState.NoBlink;

        private int aboveCount = 0;
        private int blinkCount0 = 0;
        private int blinkCount1 = 0;
        private int blinkCount2 = 0;
        private int blinkMin = 0;
        private int blinkMax = 0;

        /**
         * This method returns a 0 if no blink was detected, and a non-zero value (1 to 255)
         * indicating the blink strength otherwise.
         */
        public byte Detect(byte poorSignalValue, short eegValue) {
            if(poorSignalValue < PQ_THRESHOLD){
                switch(state) {
                    case (BlinkState.NoBlink):
                        if(eegValue > BLINK_DETECT_UPPER_THRESHOLD) {
                            aboveCount = 1;
                            state = BlinkState.UpperBlink;
                            blinkMax = eegValue;
                        }

                        break;

                    case (BlinkState.UpperBlink):
                        if(eegValue > BLINK_DETECT_UPPER_THRESHOLD) {
                            aboveCount = aboveCount + 1;
                            if(eegValue > blinkMax) blinkMax = eegValue;

                            /* Else this sample is no longer above the blink threshold...*/
                        }
                        else {
                            if(aboveCount > UPPER_BLINK_MIN_TIME && aboveCount < UPPER_BLINK_MAX_TIME) {
                                state = BlinkState.MiddleBlink;
                                blinkCount0 = aboveCount;
                                aboveCount = 1;
                            }
                            else {
                                state = BlinkState.NoBlink;
                                aboveCount = 0;
                            }
                        }
                        break;

                    case (BlinkState.MiddleBlink):
                        if(eegValue < BLINK_DETECT_UPPER_THRESHOLD && eegValue > BLINK_DETECT_LOWER_THRESHOLD) {
                            aboveCount = aboveCount + 1;
                        }
                        else {
                            if(aboveCount > MIDDLE_BLINK_MIN_TIME && aboveCount < MIDDLE_BLINK_MAX_TIME) {
                                state = BlinkState.LowerBlink;
                                blinkCount1 = aboveCount;
                                aboveCount = 1;
                                blinkMin = eegValue;
                            }
                            else {
                                state = BlinkState.NoBlink;
                                aboveCount = 0;
                            }
                        }
                        break;

                    case (BlinkState.LowerBlink):
                        if(eegValue < BLINK_DETECT_LOWER_THRESHOLD) {
                            aboveCount = aboveCount + 1;
                            if(eegValue < blinkMin) blinkMin = eegValue;

                            /* Else this sample is no longer above the blink threshold...*/
                        }
                        else {
                            if(aboveCount > LOWER_BLINK_MIN_TIME && aboveCount < LOWER_BLINK_MAX_TIME) {
                                byte blinkSize = 0;
                                blinkCount2 = aboveCount;

                                if((((blinkMax - blinkMin) >> SHIFTING_TERM) & 0xFF) < 0xFF)
                                    blinkSize = (byte)(((blinkMax - blinkMin) >> SHIFTING_TERM) & 0xFF);
                                else
                                    blinkSize = 0xFF;

                                /*Initializes the state and counter for next blink*/
                                state = BlinkState.NoBlink;
                                aboveCount = 0;
                                blinkMax = 0;
                                blinkMin = 0;

                                /* Initializes the blinkCounter user for debug only */
                                blinkCount0 = 0;
                                blinkCount1 = 0;
                                blinkCount2 = 0;

                                return blinkSize;
                            }

                            /*Initializes the state and counter for next blink*/
                            state = BlinkState.NoBlink;
                            aboveCount = 0;
                        }
                        break;

                    default:
                        state = BlinkState.NoBlink;
                        aboveCount = 0;
                        break;
                }
            }
            else {
                state = BlinkState.NoBlink;
                aboveCount = 0;
            }

            return 0;
        }
    }
}