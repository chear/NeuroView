using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Text.RegularExpressions;

using NeuroSky.ThinkGear;
//using NeuroSky.ThinkGear.Parser;

using NeuroSky.ThinkGear.Algorithms;

namespace NeuroSky.MindView {
    public partial class Launcher : Form {
        
        private Connector connector;
        RespiratoryRate respRate;
        MainForm mainForm;
        Device device;

        private TGHrv tgHRV;                //the RR interval detection algorithm
        
        private int tgHRVresult;            //output of the TGHrv algorithm
        private int tgHRVresultInMS;     //output of the TGHrv algorithm in MSec

        private byte[] bytesToSend;     //bytes to send for EGO
        private int rawCounter;         //counter for delay of EGO output
        private int delay;              //delay for lead on/lead off

        private int bufferSize_hp = 1009;       //length of the low pass filter
        
        //bandpass filter, 0.5 to 2Hz, 35 to 36.5Hz, equiripple
        private double[] hp_coeff = new double[1009] { 4.97842e-005, -1.6073e-005, -4.38342e-005, -9.0004e-005, -0.000153298, -0.000230318, -0.000314971, -0.000398489, -0.000469973, -0.000517472, -0.000529423, -0.000496331, -0.000412407, -0.000276935, -9.50954e-005, 0.000121915, 0.000357604, 0.000591691, 0.000802295, 0.000968525, 0.00107317, 0.00110508, 0.00106089, 0.000945738, 0.000772936, 0.000562427, 0.000338361, 0.000125981, -5.16857e-005, -0.000176926, -0.00023969, -0.000238881, -0.000182311, -8.54013e-005, 3.12379e-005, 0.000144963, 0.00023462, 0.000283763, 0.000283099, 0.000231661, 0.00013688, 1.33839e-005, -0.000120031, -0.000242959, -0.000337238, -0.000389688, -0.000394337, -0.000353272, -0.000276203, -0.000178694, -7.94472e-005, 2.9522e-006, 5.32201e-005, 6.19903e-005, 2.73595e-005, -4.48451e-005, -0.000142171, -0.000247778, -0.000343455, -0.000412854, -0.000444393, -0.000433287, -0.000382386, -0.000301629, -0.000206258, -0.000114059, -4.21593e-005, -3.92132e-006, -6.54742e-006, -4.97674e-005, -0.000125889, -0.00022117, -0.00031833, -0.000399692, -0.000450426, -0.000461245, -0.000430149, -0.000362831, -0.000271676, -0.000173472, -8.64117e-005, -2.67399e-005, -5.66692e-006, -2.74003e-005, -8.81832e-005, -0.000177021, -0.0002776, -0.000371292, -0.000440564, -0.000472258, -0.000460059, -0.000405741, -0.000318915, -0.000215329, -0.000114006, -3.37416e-005, 1.03968e-005, 9.91564e-006, -3.54469e-005, -0.000117579, -0.0002214, -0.000327588, -0.000416124, -0.000470027, -0.000478549, -0.000439269, -0.000358643, -0.000250928, -0.000135622, -3.38883e-005, 3.53741e-005, 5.90895e-005, 3.25324e-005, -3.96701e-005, -0.000144263, -0.000261735, -0.000369865, -0.000447836, -0.000480153, -0.000459636, -0.000388857, -0.000279818, -0.000151863, -2.81216e-005, 6.87087e-005, 0.000120668, 0.000117933, 6.08078e-005, -4.01972e-005, -0.000166163, -0.000293185, -0.000396764, -0.000456395, -0.000459465, -0.000403747, -0.000297973, -0.000160372, -1.54059e-005, 0.000110719, 0.000194937, 0.000221695, 0.000185993, 9.44788e-005, -3.5604e-005, -0.000179484, -0.000309383, -0.000399686, -0.000431767, -0.000397602, -0.00030144, -0.000159206, 4.30025e-006, 0.000159918, 0.000279517, 0.000341293, 0.000333965, 0.000259055, 0.000130806, -2.62808e-005, -0.000181874, -0.000305429, -0.00037189, -0.000366495, -0.000287715, -0.000147738, 2.95778e-005, 0.000213015, 0.000369718, 0.000471346, 0.000499459, 0.000449139, 0.000330132, 0.000165249, -1.37043e-005, -0.000171755, -0.000277278, -0.000307988, -0.000255277, -0.000126043, 5.8352e-005, 0.000265863, 0.000459606, 0.000604713, 0.000674904, 0.000657563, 0.000556324, 0.000390649, 0.00019242, 1.50962e-007, -0.000148142, -0.00022209, -0.000204796, -9.62312e-005, 8.64968e-005, 0.000312086, 0.000540686, 0.000731258, 0.000849261, 0.000873244, 0.000799081, 0.000641031, 0.000429332, 0.000204786, 1.12534e-005, -0.000112549, -0.000140389, -6.36667e-005, 0.000106707, 0.000342042, 0.000600983, 0.000837082, 0.00100745, 0.00108085, 0.00104381, 0.000903343, 0.00068604, 0.000433322, 0.000193907, 1.47609e-005, -6.77566e-005, -3.48957e-005, 0.000110684, 0.000345075, 0.000627282, 0.000906666, 0.00113231, 0.00126258, 0.00127311, 0.00116164, 0.000948794, 0.00067454, 0.000390999, 0.00015282, 6.93962e-006, -1.63582e-005, 9.06015e-005, 0.000311087, 0.000606724, 0.000924384, 0.00120594, 0.00139911, 0.00146741, 0.00139722, 0.00120072, 0.000914022, 0.00059069, 0.000291984, 7.54329e-005, -1.60388e-005, 3.75807e-005, 0.000229277, 0.000525903, 0.000874017, 0.00120961, 0.00147, 0.00160562, 0.00158958, 0.00142303, 0.00113543, 0.000779412, 0.000421231, 0.00012853, -4.23778e-005, -5.72837e-005, 8.91401e-005, 0.000371887, 0.00073985, 0.00112504, 0.0014551, 0.00166687, 0.00171843, 0.00159726, 0.00132311, 0.00094477, 0.000531317, 0.000159446, -0.000100848, -0.000199552, -0.00011636, 0.000135044, 0.000509035, 0.000936128, 0.0013357, 0.00163096, 0.00176352, 0.00170461, 0.00146099, 0.00107421, 0.000613365, 0.00016248, -0.000195056, -0.000392206, -0.000391161, -0.000190913, 0.00017197, 0.000629648, 0.00109528, 0.0014791, 0.00170522, 0.00172621, 0.00153243, 0.00115445, 0.000657917, 0.000131795, -0.000328076, -0.000636886, -0.000737016, -0.000609315, -0.000277325, 0.000196479, 0.000721825, 0.00119721, 0.00152882, 0.00164837, 0.00152629, 0.00117802, 0.000661752, 6.8175e-005, -0.000496062, -0.0009282, -0.00114878, -0.00111701, -0.000839032, -0.000367587, 0.000207202, 0.000774182, 0.00122177, 0.00145895, 0.00143281, 0.00113906, 0.00062371, -2.47923e-005, -0.000691529, -0.00125632, -0.00161627, -0.00170545, -0.00150797, -0.00106174, -0.0004523, 0.000202544, 0.000774355, 0.0011481, 0.00124405, 0.00103341, 0.000544639, -0.000140981, -0.000903717, -0.00160731, -0.00212435, -0.0023605, -0.00227276, -0.00187856, -0.00125346, -0.000517976, 0.000184397, 0.000713794, 0.000960428, 0.000865822, 0.000434742, -0.00026456, -0.00111209, -0.00195777, -0.00264925, -0.00306051, -0.00311619, -0.00280663, -0.00219097, -0.00138724, -0.000551165, 0.000152457, 0.000580703, 0.000639875, 0.000304097, -0.000378518, -0.00129354, -0.00228053, -0.00316244, -0.00377866, -0.00401565, -0.00382924, -0.00325438, -0.00240002, -0.00143006, -0.00053352, 0.00011063, 0.000367443, 0.000172336, -0.000455299, -0.00141404, -0.00253787, -0.00362621, -0.00448102, -0.00494422, -0.00492835, -0.0044346, -0.00355442, -0.00245424, -0.00134562, -0.000446322, 6.06444e-005, 6.23512e-005, -0.000461193, -0.0014312, -0.00268275, -0.00399399, -0.00512671, -0.00587093, -0.0060852, -0.00572491, -0.00485295, -0.0036304, -0.00228845, -0.0010864, -0.000263448, 7.05746e-006, -0.000348085, -0.00128559, -0.00265096, -0.0042048, -0.00566592, -0.00676332, -0.00728765, -0.00713238, -0.00631662, -0.00498492, -0.00338331, -0.00181561, -0.000587673, 5.03948e-005, -4.69781e-005, -0.000890329, -0.00234927, -0.00417302, -0.00603419, -0.00758839, -0.00853877, -0.00869352, -0.00800543, -0.00658532, -0.00468611, -0.00265964, -0.000893409, 0.000261468, 0.000557815, -9.7344e-005, -0.0016199, -0.00375832, -0.00613523, -0.00831414, -0.00987974, -0.0105172, -0.0100752, -0.00860142, -0.00634057, -0.00369663, -0.00116275, 0.000768088, 0.0016962, 0.00139464, -0.000140363, -0.00267874, -0.00578933, -0.00891179, -0.0114562, -0.0129129, -0.0129523, -0.0114965, -0.00874675, -0.00516073, -0.00138027, 0.00187893, 0.00395757, 0.00437993, 0.00295426, -0.000173413, -0.00452491, -0.00935723, -0.0137831, -0.0169265, -0.018086, -0.0168787, -0.0133363, -0.0079323, -0.00153294, 0.00472761, 0.00962952, 0.0120805, 0.011325, 0.00711448, -0.000194145, -0.00963208, -0.0197124, -0.0286155, -0.0344426, -0.0355015, -0.0305815, -0.0191755, -0.00161144, 0.0209306, 0.0465231, 0.0727066, 0.0967898, 0.116191, 0.128774, 0.133132, 0.128774, 0.116191, 0.0967898, 0.0727066, 0.0465231, 0.0209306, -0.00161144, -0.0191755, -0.0305815, -0.0355015, -0.0344426, -0.0286155, -0.0197124, -0.00963208, -0.000194145, 0.00711448, 0.011325, 0.0120805, 0.00962952, 0.00472761, -0.00153294, -0.0079323, -0.0133363, -0.0168787, -0.018086, -0.0169265, -0.0137831, -0.00935723, -0.00452491, -0.000173413, 0.00295426, 0.00437993, 0.00395757, 0.00187893, -0.00138027, -0.00516073, -0.00874675, -0.0114965, -0.0129523, -0.0129129, -0.0114562, -0.00891179, -0.00578933, -0.00267874, -0.000140363, 0.00139464, 0.0016962, 0.000768088, -0.00116275, -0.00369663, -0.00634057, -0.00860142, -0.0100752, -0.0105172, -0.00987974, -0.00831414, -0.00613523, -0.00375832, -0.0016199, -9.7344e-005, 0.000557815, 0.000261468, -0.000893409, -0.00265964, -0.00468611, -0.00658532, -0.00800543, -0.00869352, -0.00853877, -0.00758839, -0.00603419, -0.00417302, -0.00234927, -0.000890329, -4.69781e-005, 5.03948e-005, -0.000587673, -0.00181561, -0.00338331, -0.00498492, -0.00631662, -0.00713238, -0.00728765, -0.00676332, -0.00566592, -0.0042048, -0.00265096, -0.00128559, -0.000348085, 7.05746e-006, -0.000263448, -0.0010864, -0.00228845, -0.0036304, -0.00485295, -0.00572491, -0.0060852, -0.00587093, -0.00512671, -0.00399399, -0.00268275, -0.0014312, -0.000461193, 6.23512e-005, 6.06444e-005, -0.000446322, -0.00134562, -0.00245424, -0.00355442, -0.0044346, -0.00492835, -0.00494422, -0.00448102, -0.00362621, -0.00253787, -0.00141404, -0.000455299, 0.000172336, 0.000367443, 0.00011063, -0.00053352, -0.00143006, -0.00240002, -0.00325438, -0.00382924, -0.00401565, -0.00377866, -0.00316244, -0.00228053, -0.00129354, -0.000378518, 0.000304097, 0.000639875, 0.000580703, 0.000152457, -0.000551165, -0.00138724, -0.00219097, -0.00280663, -0.00311619, -0.00306051, -0.00264925, -0.00195777, -0.00111209, -0.00026456, 0.000434742, 0.000865822, 0.000960428, 0.000713794, 0.000184397, -0.000517976, -0.00125346, -0.00187856, -0.00227276, -0.0023605, -0.00212435, -0.00160731, -0.000903717, -0.000140981, 0.000544639, 0.00103341, 0.00124405, 0.0011481, 0.000774355, 0.000202544, -0.0004523, -0.00106174, -0.00150797, -0.00170545, -0.00161627, -0.00125632, -0.000691529, -2.47923e-005, 0.00062371, 0.00113906, 0.00143281, 0.00145895, 0.00122177, 0.000774182, 0.000207202, -0.000367587, -0.000839032, -0.00111701, -0.00114878, -0.0009282, -0.000496062, 6.8175e-005, 0.000661752, 0.00117802, 0.00152629, 0.00164837, 0.00152882, 0.00119721, 0.000721825, 0.000196479, -0.000277325, -0.000609315, -0.000737016, -0.000636886, -0.000328076, 0.000131795, 0.000657917, 0.00115445, 0.00153243, 0.00172621, 0.00170522, 0.0014791, 0.00109528, 0.000629648, 0.00017197, -0.000190913, -0.000391161, -0.000392206, -0.000195056, 0.00016248, 0.000613365, 0.00107421, 0.00146099, 0.00170461, 0.00176352, 0.00163096, 0.0013357, 0.000936128, 0.000509035, 0.000135044, -0.00011636, -0.000199552, -0.000100848, 0.000159446, 0.000531317, 0.00094477, 0.00132311, 0.00159726, 0.00171843, 0.00166687, 0.0014551, 0.00112504, 0.00073985, 0.000371887, 8.91401e-005, -5.72837e-005, -4.23778e-005, 0.00012853, 0.000421231, 0.000779412, 0.00113543, 0.00142303, 0.00158958, 0.00160562, 0.00147, 0.00120961, 0.000874017, 0.000525903, 0.000229277, 3.75807e-005, -1.60388e-005, 7.54329e-005, 0.000291984, 0.00059069, 0.000914022, 0.00120072, 0.00139722, 0.00146741, 0.00139911, 0.00120594, 0.000924384, 0.000606724, 0.000311087, 9.06015e-005, -1.63582e-005, 6.93962e-006, 0.00015282, 0.000390999, 0.00067454, 0.000948794, 0.00116164, 0.00127311, 0.00126258, 0.00113231, 0.000906666, 0.000627282, 0.000345075, 0.000110684, -3.48957e-005, -6.77566e-005, 1.47609e-005, 0.000193907, 0.000433322, 0.00068604, 0.000903343, 0.00104381, 0.00108085, 0.00100745, 0.000837082, 0.000600983, 0.000342042, 0.000106707, -6.36667e-005, -0.000140389, -0.000112549, 1.12534e-005, 0.000204786, 0.000429332, 0.000641031, 0.000799081, 0.000873244, 0.000849261, 0.000731258, 0.000540686, 0.000312086, 8.64968e-005, -9.62312e-005, -0.000204796, -0.00022209, -0.000148142, 1.50962e-007, 0.00019242, 0.000390649, 0.000556324, 0.000657563, 0.000674904, 0.000604713, 0.000459606, 0.000265863, 5.8352e-005, -0.000126043, -0.000255277, -0.000307988, -0.000277278, -0.000171755, -1.37043e-005, 0.000165249, 0.000330132, 0.000449139, 0.000499459, 0.000471346, 0.000369718, 0.000213015, 2.95778e-005, -0.000147738, -0.000287715, -0.000366495, -0.00037189, -0.000305429, -0.000181874, -2.62808e-005, 0.000130806, 0.000259055, 0.000333965, 0.000341293, 0.000279517, 0.000159918, 4.30025e-006, -0.000159206, -0.00030144, -0.000397602, -0.000431767, -0.000399686, -0.000309383, -0.000179484, -3.5604e-005, 9.44788e-005, 0.000185993, 0.000221695, 0.000194937, 0.000110719, -1.54059e-005, -0.000160372, -0.000297973, -0.000403747, -0.000459465, -0.000456395, -0.000396764, -0.000293185, -0.000166163, -4.01972e-005, 6.08078e-005, 0.000117933, 0.000120668, 6.87087e-005, -2.81216e-005, -0.000151863, -0.000279818, -0.000388857, -0.000459636, -0.000480153, -0.000447836, -0.000369865, -0.000261735, -0.000144263, -3.96701e-005, 3.25324e-005, 5.90895e-005, 3.53741e-005, -3.38883e-005, -0.000135622, -0.000250928, -0.000358643, -0.000439269, -0.000478549, -0.000470027, -0.000416124, -0.000327588, -0.0002214, -0.000117579, -3.54469e-005, 9.91564e-006, 1.03968e-005, -3.37416e-005, -0.000114006, -0.000215329, -0.000318915, -0.000405741, -0.000460059, -0.000472258, -0.000440564, -0.000371292, -0.0002776, -0.000177021, -8.81832e-005, -2.74003e-005, -5.66692e-006, -2.67399e-005, -8.64117e-005, -0.000173472, -0.000271676, -0.000362831, -0.000430149, -0.000461245, -0.000450426, -0.000399692, -0.00031833, -0.00022117, -0.000125889, -4.97674e-005, -6.54742e-006, -3.92132e-006, -4.21593e-005, -0.000114059, -0.000206258, -0.000301629, -0.000382386, -0.000433287, -0.000444393, -0.000412854, -0.000343455, -0.000247778, -0.000142171, -4.48451e-005, 2.73595e-005, 6.19903e-005, 5.32201e-005, 2.9522e-006, -7.94472e-005, -0.000178694, -0.000276203, -0.000353272, -0.000394337, -0.000389688, -0.000337238, -0.000242959, -0.000120031, 1.33839e-005, 0.00013688, 0.000231661, 0.000283099, 0.000283763, 0.00023462, 0.000144963, 3.12379e-005, -8.54013e-005, -0.000182311, -0.000238881, -0.00023969, -0.000176926, -5.16857e-005, 0.000125981, 0.000338361, 0.000562427, 0.000772936, 0.000945738, 0.00106089, 0.00110508, 0.00107317, 0.000968525, 0.000802295, 0.000591691, 0.000357604, 0.000121915, -9.50954e-005, -0.000276935, -0.000412407, -0.000496331, -0.000529423, -0.000517472, -0.000469973, -0.000398489, -0.000314971, -0.000230318, -0.000153298, -9.0004e-005, -4.38342e-005, -1.6073e-005, 4.97842e-005 };

        //hold the filtered data
        private double filtered;

        //hold the raw data in a continuously updating buffer
        private double[] eegBuffer;
        private double[] tempeegBuffer;

        //counter to track when we should plot
        private int bufferCounter_raw;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.Run(new Launcher());
        }
        
        public Launcher() {
            mainForm = new MainForm();
            tgHRV = new TGHrv();
            respRate = new RespiratoryRate();

            connector = new Connector();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceFound += new EventHandler(OnDeviceFound);
            connector.DeviceNotFound += new EventHandler(OnDeviceNotFound);
            connector.DeviceConnectFail += new EventHandler(OnDeviceNotFound);
            connector.DeviceDisconnected += new EventHandler(OnDeviceDisconnected);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);

            mainForm.ConnectButtonClicked += new EventHandler(OnConnectButtonClicked);
            mainForm.DisconnectButtonClicked += new EventHandler(OnDisconnectButtonClicked);
            mainForm.ConfirmHeartAgeButtonClicked += new EventHandler(OnConfirmHeartAgeButtonClicked);
            mainForm.Disposed += new EventHandler(OnMainFormDisposed);

            InitializeComponent();

            rawCounter = 0;     //initially zero
            //delay = 512 * 5;    //5 seconds delay
            delay = 0;  //no delay (although there is still a delay due to the filtering of data)


            //hold the raw data in a continuously updating buffer
            eegBuffer = new double[bufferSize_hp];
            tempeegBuffer = new double[bufferSize_hp];

            //counter to track when we should plot
            bufferCounter_raw = 0;
 
        }
        //accept heart age input parameters from mainForm then calculate
        void OnConfirmHeartAgeButtonClicked(object sender, EventArgs e)
        {

            HeartAgeEventArgs heartAgeEventArgs = (HeartAgeEventArgs)e;   //cast this as a HeartAgeEventArgs
            string heartAgeFileName = heartAgeEventArgs.parametersFileName;     //get the filename
            int heartAge = heartAgeEventArgs.parametersAge;  //get the heart age
            connector.GetHeartAge(heartAge, heartAgeFileName);
        }

        private void button1_Click(object sender, EventArgs e) {
            //UpdateConnectButton(true);
            //UpdateStatusLabel("Searching for MindSet...");

            //mainForm.updateConnectButton(true);
            //mainForm.updateStatusLabel("Searching for MindSet...");

            //Comment this line out if you want the splash screen to wait for good connection.
            UpdateVisibility(false);

            //connector.RefreshAvailableConnections();

        }

        void OnDeviceNotFound(object sender, EventArgs e) {
            UpdateConnectButton(false);
            mainForm.updateConnectButton(false);
            mainForm.updateStatusLabel("Unable to connect.");
        }

        void OnDeviceFound(object sender, EventArgs e) {
            Connector.PortEventArgs de = (Connector.PortEventArgs)e;

            string tempPortName = de.PortName;
            mainForm.updateStatusLabel("Device found on " + tempPortName + ". Connecting...");

            connector.Connect(tempPortName);

        }

        void OnDeviceValidating(object sender, EventArgs e) {
            Connector.ConnectionEventArgs ce = (Connector.ConnectionEventArgs)e;

            mainForm.updateStatusLabel("Validating " + ce.Connection.PortName + ".");
        }

        void OnDeviceConnected(object sender, EventArgs e) {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            //save the device
            device = de.Device;

            mainForm.updateStatusLabel("Connected to a device on " + de.Device.PortName + ".");

            de.Device.DataReceived += new EventHandler(OnDataReceived);
            mainForm.updateConnectButton(true);

            UpdateVisibility(false);
            Console.WriteLine("Done");
        }

        void OnDeviceDisconnected(object sender, EventArgs e) {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            mainForm.updateStatusLabel("Disconnected from a device on " + de.Device.PortName + ".");

            mainForm.updateConnectButton(false);

        }


        void OnDataReceived(object sender, EventArgs e) {
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            ThinkGear.DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser thinkGearParser = new TGParser();
            thinkGearParser.Read(de.DataRowArray);

            // Pass off data for recording
            if(mainForm.recordFlag == true) {
                //save the datalog.txt
                mainForm.recordData(de.DataRowArray);
            }


            /* Loop through new parsed data */
            for(int i = 0; i < thinkGearParser.ParsedData.Length; i++) {
                
                //save the poorsignal value. this is always updated
                if(thinkGearParser.ParsedData[i].ContainsKey("PoorSignal")) {
                    mainForm.poorQuality = thinkGearParser.ParsedData[i]["PoorSignal"];
                    
                }
                //update heart age
                if (thinkGearParser.ParsedData[i].ContainsKey("HeartAge"))
                {
                    mainForm.updateHeartAgeIndicator((thinkGearParser.ParsedData[i]["HeartAge"]).ToString());
                    Console.WriteLine("HeartAge = " + thinkGearParser.ParsedData[i]["HeartAge"]);
                }

                if(thinkGearParser.ParsedData[i].ContainsKey("Raw")) {

                    

                    //if signal is good
                    if(mainForm.poorQuality == 200) {
                        rawCounter++;

                        double rr = respRate.calculateRespiratoryRate((short)thinkGearParser.ParsedData[i]["Raw"], (byte)mainForm.poorQuality);
                        if (rr > 0)
                        {
                            //display this
                            mainForm.updateRespirationRateIndicator(rr.ToString());
                            Console.WriteLine("rr = " + rr);
                        }

                        //update the buffer with the latest eeg value
                        Array.Copy(eegBuffer, 1, tempeegBuffer, 0, bufferSize_hp - 1);
                        tempeegBuffer[bufferSize_hp - 1] = (double)thinkGearParser.ParsedData[i]["Raw"];
                        Array.Copy(tempeegBuffer, eegBuffer, bufferSize_hp);
                        bufferCounter_raw++;

                        //if the eeg buffer is full, calculate the filtered data
                        if(bufferCounter_raw >= bufferSize_hp) {

                            //filter the data
                            filtered = applyFilter(eegBuffer, hp_coeff);

                            //pass filtered data to the TGHRV algorithm 
                            tgHRVresult = tgHRV.AddData((short)filtered);

                            //calulate the fatigue level
                            mainForm.calculateFatigue(tgHRVresult);
                            
                            //update the label and play the beep (only if "delay" seconds have passed)
                            if((tgHRVresult > 150) && (tgHRVresult < 800) && (rawCounter >= delay)) {
                                tgHRVresultInMS = (int)(tgHRVresult * 1000.0 / 512.0);
                                mainForm.updateHRVLabel(tgHRVresultInMS.ToString() + " msec");
                                mainForm.playBeep();
                            }

                            //if "delay" seconds have passed, start plotting the data
                            if(rawCounter >= delay) {
                                mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), filtered));
                                mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                            }

                            //clear the graph when it's full
                            if(mainForm.rawGraphPanel.LineGraph.timeStampIndex >= mainForm.rawGraphPanel.LineGraph.numberOfPoints) {
                                mainForm.rawGraphPanel.LineGraph.Clear();
                            }

                        } else {
                            //raw buffer is not full yet. plot zero
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                            mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                        }

                    } else {
                        //otherwise signal is bad, plot zero. reset counter. reset HRV
                        rawCounter = 0;
                        bufferCounter_raw = 0;

                        Array.Clear(eegBuffer, 0, eegBuffer.Length);

                        respRate.calculateRespiratoryRate(0, 0);    //reset the respiration buffer

                        mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                        mainForm.rawGraphPanel.LineGraph.timeStampIndex++;

                        mainForm.updateHRVLabel("0");
                        mainForm.updateAverageHeartRateLabel("0");
                        mainForm.updateRealTimeHeartRateLabel("0");
                        //mainForm.updateHeartAgeIndicator("0");
                        //mainForm.updateRespirationRateIndicator("0");

                        tgHRV.Reset();
                    }
                }


                if(thinkGearParser.ParsedData[i].ContainsKey("HeartRate")) {
                    
                    //if the "delay" number of seconds have passed, pass the heartrate value
                    if(rawCounter >= delay) {
                        mainForm.ASICHBValue = thinkGearParser.ParsedData[i]["HeartRate"];
                        mainForm.updateAverageHeartBeatValue(thinkGearParser.ParsedData[i]["HeartRate"]);
                        mainForm.updateRealTimeHeartBeatValue(thinkGearParser.ParsedData[i]["HeartRate"]);
                    }
                        //otherwise just pass a value of 0 to make it think its poor signal
                    else {
                        mainForm.updateAverageHeartBeatValue(0);
                        mainForm.updateRealTimeHeartBeatValue(0);

                        //but still pass the correct heartbeat value for ecglog.txt
                        mainForm.ASICHBValue = thinkGearParser.ParsedData[i]["HeartRate"];
                    }
                }

                /* End "Check for the data flag for each panel..." */
            }
        }





        void OnConnectButtonClicked(object sender, EventArgs e) {
            string portName = mainForm.portText.Text.ToUpper();
            portName = portName.Trim();

            if(portName == "AUTO") {
                connector.RefreshAvailableConnections();
                mainForm.updateStatusLabel("Searching for MindSet...");
                return;
            }

            int portNumber = 0;

            try {
                portNumber = Convert.ToInt16(portName);
            } catch(FormatException fe) {
                Console.WriteLine(fe.Message);
            }

            if(portNumber > 0) {
                portName = "COM" + portNumber;
            }

            Regex r = new Regex("COM[1-9][0-9]*");
            portName = r.Match(portName).ToString();
            Console.WriteLine("Connecting to xx" + portName + "xx");

            if(portName != "") {
                connector.Connect(portName);
                mainForm.updateStatusLabel("Connecting to " + portName);
                return;
            }

            MessageBox.Show("You must enter a valid COM port name. (Ex. COM1 or 1)\nYou may also type in 'Auto' for auto-connection.", "Invalid Input Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //mainForm.updateConnectButton(false);
            mainForm.updateConnectButton(false);
            return;

        }

        void OnDisconnectButtonClicked(object sender, EventArgs e) {
            connector.Disconnect();
            mainForm.updateConnectButton(false);

            //make the byteToSend null so it will be resent when pressing connect
            bytesToSend = null;
        }



        delegate void UpdateVisibilityDelegate(bool enable);
        public void UpdateVisibility(bool enable) {
            if(this.InvokeRequired) {
                UpdateVisibilityDelegate del = new UpdateVisibilityDelegate(UpdateVisibility);
                this.Invoke(del, new object[] { enable });
            } else {
                if(enable) {
                    if(!this.Visible) {
                        this.Show();
                        mainForm.Hide();
                    }
                } else {
                    if(this.Visible) {
                        this.Hide();
                        mainForm.Show();
                    }
                }
            }
        }


        delegate void UpdateConnectButtonDelegate(bool connected);
        public void UpdateConnectButton(bool connected) {
            if(this.InvokeRequired) {
                UpdateConnectButtonDelegate del = new UpdateConnectButtonDelegate(UpdateConnectButton);
                this.Invoke(del, new object[] { connected });
            } else {
                if(connected) {
                    this.button1.Enabled = false;
                } else {

                    this.button1.Enabled = true;
                }

            }
        }

        void OnMainFormDisposed(object sender, EventArgs e) {
            this.Dispose();
        }

        private void Launcher_Load(object sender, EventArgs e) {

        }


        //apply filter based on multiply add technique
        private double applyFilter(double[] data, double[] coeffs) {
            int length = data.Length;
            double result = new double();

            for(int i = 0; i < length; i++) {
                result += data[i] * coeffs[i];
            }
            return result;
        }
    }
}
