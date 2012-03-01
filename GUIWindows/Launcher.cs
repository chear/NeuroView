using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Text.RegularExpressions;

using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Parser;

using NeuroSky.ThinkGear.Algorithms;

namespace NeuroSky.MindView {
    public partial class Launcher : Form {


        private string debugOutFile;
        private System.IO.StreamWriter debugStream;
        
        
        
        
        private Connector connector;

        MainForm mainForm;
        Device device;

        private byte[] bytesToSend;     //bytes to send for EGO
        private int rawCounter;     //counter for delay of EGO output
        private int delay;          //delay for lead on/lead off



        private double[] hp_coeff = new double[511] { 2.428591e-002, 4.635880e-004, 4.677350e-004, 4.717070e-004, 4.754360e-004, 4.788830e-004, 4.821770e-004, 4.854570e-004, 4.881980e-004, 4.909140e-004, 4.933230e-004, 4.954860e-004, 4.973410e-004, 4.989530e-004, 5.003200e-004, 5.013070e-004, 5.020490e-004, 5.024410e-004, 5.025060e-004, 5.021660e-004, 5.014960e-004, 5.004510e-004, 4.989730e-004, 4.971480e-004, 4.949210e-004, 4.923420e-004, 4.893030e-004, 4.859840e-004, 4.822950e-004, 4.782490e-004, 4.739710e-004, 4.694800e-004, 4.649680e-004, 4.601440e-004, 4.557930e-004, 4.515240e-004, 4.475860e-004, 4.443480e-004, 4.420830e-004, 4.410460e-004, 3.982030e-004, 4.101420e-004, 4.018530e-004, 3.931610e-004, 3.841220e-004, 3.748150e-004, 3.651320e-004, 3.548480e-004, 3.444640e-004, 3.335430e-004, 3.222810e-004, 3.106260e-004, 2.986430e-004, 2.862540e-004, 2.734180e-004, 2.602610e-004, 2.466290e-004, 2.325890e-004, 2.180890e-004, 2.032080e-004, 1.878540e-004, 1.720480e-004, 1.558560e-004, 1.391980e-004, 1.221260e-004, 1.045860e-004, 8.670000e-005, 6.840000e-005, 4.980000e-005, 3.090000e-005, 1.160000e-005, -7.860000e-006, -2.780000e-005, -4.760000e-005, -6.830000e-005, -8.930000e-005, -1.110570e-004, -1.339560e-004, -1.586870e-004, -1.860750e-004, -2.004270e-004, -2.279140e-004, -2.521790e-004, -2.767900e-004, -3.017850e-004, -3.272720e-004, -3.531910e-004, -3.792640e-004, -4.059430e-004, -4.328740e-004, -4.601910e-004, -4.878540e-004, -5.159470e-004, -5.444180e-004, -5.732150e-004, -6.024800e-004, -6.320860e-004, -6.620850e-004, -6.924220e-004, -7.231860e-004, -7.542850e-004, -7.857120e-004, -8.175390e-004, -8.496740e-004, -8.821430e-004, -9.148400e-004, -9.479260e-004, -9.811890e-004, -1.014694e-003, -1.048515e-003, -1.082605e-003, -1.117066e-003, -1.151639e-003, -1.187199e-003, -1.222757e-003, -1.258828e-003, -1.295415e-003, -1.332056e-003, -1.367941e-003, -1.401734e-003, -1.442446e-003, -1.478561e-003, -1.516042e-003, -1.553803e-003, -1.591820e-003, -1.629957e-003, -1.668196e-003, -1.706834e-003, -1.745460e-003, -1.784350e-003, -1.823414e-003, -1.862701e-003, -1.902120e-003, -1.941701e-003, -1.981510e-003, -2.021394e-003, -2.061450e-003, -2.101621e-003, -2.141961e-003, -2.182356e-003, -2.222871e-003, -2.263508e-003, -2.304163e-003, -2.344885e-003, -2.385615e-003, -2.426462e-003, -2.467214e-003, -2.508079e-003, -2.549036e-003, -2.590021e-003, -2.631107e-003, -2.672192e-003, -2.713755e-003, -2.754587e-003, -2.795814e-003, -2.836877e-003, -2.877492e-003, -2.917844e-003, -2.958647e-003, -3.001544e-003, -3.040566e-003, -3.081754e-003, -3.122419e-003, -3.162930e-003, -3.203249e-003, -3.243494e-003, -3.283730e-003, -3.323647e-003, -3.363599e-003, -3.403372e-003, -3.443015e-003, -3.482456e-003, -3.521765e-003, -3.560914e-003, -3.599803e-003, -3.638557e-003, -3.677086e-003, -3.715422e-003, -3.753475e-003, -3.791341e-003, -3.828939e-003, -3.866219e-003, -3.903261e-003, -3.940005e-003, -3.976515e-003, -4.012643e-003, -4.048646e-003, -4.084331e-003, -4.119696e-003, -4.154807e-003, -4.189574e-003, -4.224135e-003, -4.257633e-003, -4.291632e-003, -4.324758e-003, -4.357421e-003, -4.390056e-003, -4.422754e-003, -4.455022e-003, -4.485054e-003, -4.516987e-003, -4.547320e-003, -4.577376e-003, -4.607020e-003, -4.636322e-003, -4.665188e-003, -4.693499e-003, -4.721546e-003, -4.749010e-003, -4.776029e-003, -4.802545e-003, -4.828624e-003, -4.854186e-003, -4.879224e-003, -4.903839e-003, -4.927899e-003, -4.951458e-003, -4.974458e-003, -4.996981e-003, -5.018904e-003, -5.040274e-003, -5.061151e-003, -5.081456e-003, -5.101243e-003, -5.120435e-003, -5.139222e-003, -5.157312e-003, -5.174854e-003, -5.191855e-003, -5.208208e-003, -5.223963e-003, -5.238793e-003, -5.254048e-003, -5.267602e-003, -5.281052e-003, -5.294231e-003, -5.306668e-003, -5.318017e-003, -5.328440e-003, -5.339699e-003, -5.348857e-003, -5.358050e-003, -5.366607e-003, -5.374579e-003, -5.381859e-003, -5.388494e-003, -5.394617e-003, -5.399933e-003, -5.404698e-003, -5.408814e-003, -5.412334e-003, -5.415181e-003, -5.417421e-003, -5.419073e-003, -5.420014e-003, 9.945797e-001, -5.420014e-003, -5.419073e-003, -5.417421e-003, -5.415181e-003, -5.412334e-003, -5.408814e-003, -5.404698e-003, -5.399933e-003, -5.394617e-003, -5.388494e-003, -5.381859e-003, -5.374579e-003, -5.366607e-003, -5.358050e-003, -5.348857e-003, -5.339699e-003, -5.328440e-003, -5.318017e-003, -5.306668e-003, -5.294231e-003, -5.281052e-003, -5.267602e-003, -5.254048e-003, -5.238793e-003, -5.223963e-003, -5.208208e-003, -5.191855e-003, -5.174854e-003, -5.157312e-003, -5.139222e-003, -5.120435e-003, -5.101243e-003, -5.081456e-003, -5.061151e-003, -5.040274e-003, -5.018904e-003, -4.996981e-003, -4.974458e-003, -4.951458e-003, -4.927899e-003, -4.903839e-003, -4.879224e-003, -4.854186e-003, -4.828624e-003, -4.802545e-003, -4.776029e-003, -4.749010e-003, -4.721546e-003, -4.693499e-003, -4.665188e-003, -4.636322e-003, -4.607020e-003, -4.577376e-003, -4.547320e-003, -4.516987e-003, -4.485054e-003, -4.455022e-003, -4.422754e-003, -4.390056e-003, -4.357421e-003, -4.324758e-003, -4.291632e-003, -4.257633e-003, -4.224135e-003, -4.189574e-003, -4.154807e-003, -4.119696e-003, -4.084331e-003, -4.048646e-003, -4.012643e-003, -3.976515e-003, -3.940005e-003, -3.903261e-003, -3.866219e-003, -3.828939e-003, -3.791341e-003, -3.753475e-003, -3.715422e-003, -3.677086e-003, -3.638557e-003, -3.599803e-003, -3.560914e-003, -3.521765e-003, -3.482456e-003, -3.443015e-003, -3.403372e-003, -3.363599e-003, -3.323647e-003, -3.283730e-003, -3.243494e-003, -3.203249e-003, -3.162930e-003, -3.122419e-003, -3.081754e-003, -3.040566e-003, -3.001544e-003, -2.958647e-003, -2.917844e-003, -2.877492e-003, -2.836877e-003, -2.795814e-003, -2.754587e-003, -2.713755e-003, -2.672192e-003, -2.631107e-003, -2.590021e-003, -2.549036e-003, -2.508079e-003, -2.467214e-003, -2.426462e-003, -2.385615e-003, -2.344885e-003, -2.304163e-003, -2.263508e-003, -2.222871e-003, -2.182356e-003, -2.141961e-003, -2.101621e-003, -2.061450e-003, -2.021394e-003, -1.981510e-003, -1.941701e-003, -1.902120e-003, -1.862701e-003, -1.823414e-003, -1.784350e-003, -1.745460e-003, -1.706834e-003, -1.668196e-003, -1.629957e-003, -1.591820e-003, -1.553803e-003, -1.516042e-003, -1.478561e-003, -1.442446e-003, -1.401734e-003, -1.367941e-003, -1.332056e-003, -1.295415e-003, -1.258828e-003, -1.222757e-003, -1.187199e-003, -1.151639e-003, -1.117066e-003, -1.082605e-003, -1.048515e-003, -1.014694e-003, -9.811890e-004, -9.479260e-004, -9.148400e-004, -8.821430e-004, -8.496740e-004, -8.175390e-004, -7.857120e-004, -7.542850e-004, -7.231860e-004, -6.924220e-004, -6.620850e-004, -6.320860e-004, -6.024800e-004, -5.732150e-004, -5.444180e-004, -5.159470e-004, -4.878540e-004, -4.601910e-004, -4.328740e-004, -4.059430e-004, -3.792640e-004, -3.531910e-004, -3.272720e-004, -3.017850e-004, -2.767900e-004, -2.521790e-004, -2.279140e-004, -2.004270e-004, -1.860750e-004, -1.586870e-004, -1.339560e-004, -1.110570e-004, -8.930000e-005, -6.830000e-005, -4.760000e-005, -2.780000e-005, -7.860000e-006, 1.160000e-005, 3.090000e-005, 4.980000e-005, 6.840000e-005, 8.670000e-005, 1.045860e-004, 1.221260e-004, 1.391980e-004, 1.558560e-004, 1.720480e-004, 1.878540e-004, 2.032080e-004, 2.180890e-004, 2.325890e-004, 2.466290e-004, 2.602610e-004, 2.734180e-004, 2.862540e-004, 2.986430e-004, 3.106260e-004, 3.222810e-004, 3.335430e-004, 3.444640e-004, 3.548480e-004, 3.651320e-004, 3.748150e-004, 3.841220e-004, 3.931610e-004, 4.018530e-004, 4.101420e-004, 3.982030e-004, 4.410460e-004, 4.420830e-004, 4.443480e-004, 4.475860e-004, 4.515240e-004, 4.557930e-004, 4.601440e-004, 4.649680e-004, 4.694800e-004, 4.739710e-004, 4.782490e-004, 4.822950e-004, 4.859840e-004, 4.893030e-004, 4.923420e-004, 4.949210e-004, 4.971480e-004, 4.989730e-004, 5.004510e-004, 5.014960e-004, 5.021660e-004, 5.025060e-004, 5.024410e-004, 5.020490e-004, 5.013070e-004, 5.003200e-004, 4.989530e-004, 4.973410e-004, 4.954860e-004, 4.933230e-004, 4.909140e-004, 4.881980e-004, 4.854570e-004, 4.821770e-004, 4.788830e-004, 4.754360e-004, 4.717070e-004, 4.677350e-004, 4.635880e-004, 2.428591e-002 };

        private double[] lp_coeff = new double[260] { -9.930000e-005, -9.600000e-005, -1.358070e-004, -1.801960e-004, -2.265160e-004, -2.711770e-004, -3.096190e-004, -3.364770e-004, -3.455650e-004, -3.307330e-004, -2.854140e-004, -2.038980e-004, -8.130000e-005, 8.570000e-005, 2.984710e-004, 5.560150e-004, 8.545590e-004, 1.187433e-003, 1.544988e-003, 1.914834e-003, 2.282215e-003, 2.630493e-003, 2.942019e-003, 3.198943e-003, 3.384406e-003, 3.483449e-003, 3.484316e-003, 3.379222e-003, 3.165405e-003, 2.845570e-003, 2.428372e-003, 1.928244e-003, 1.365157e-003, 7.637350e-004, 1.522290e-004, -4.390040e-004, -9.790910e-004, -1.438574e-003, -1.791166e-003, -2.015672e-003, -2.097461e-003, -2.029896e-003, -1.815018e-003, -1.464052e-003, -9.970050e-004, -4.420030e-004, 1.662540e-004, 7.882490e-004, 1.382130e-003, 1.906188e-003, 2.321735e-003, 2.595689e-003, 2.703167e-003, 2.629410e-003, 2.371398e-003, 1.938479e-003, 1.352477e-003, 6.466750e-004, -1.357370e-004, -9.442820e-004, -1.724144e-003, -2.419861e-003, -2.978974e-003, -3.355931e-003, -3.515455e-003, -3.435638e-003, -3.110028e-003, -2.548995e-003, -1.779770e-003, -8.455660e-004, 1.967020e-004, 1.279523e-003, 2.329036e-003, 3.269698e-003, 4.029575e-003, 4.545400e-003, 4.767585e-003, 4.664279e-003, 4.224689e-003, 3.460874e-003, 2.408302e-003, 1.124594e-003, -3.131100e-004, -1.812484e-003, -3.271431e-003, -4.584688e-003, -5.650810e-003, -6.379504e-003, -6.698420e-003, -6.559325e-003, -5.942780e-003, -4.861362e-003, -3.360701e-003, -1.518559e-003, 5.585060e-004, 2.740409e-003, 4.880896e-003, 6.826232e-003, 8.425123e-003, 9.538923e-003, 1.005183e-002, 9.880123e-003, 8.980045e-003, 7.353569e-003, 5.051755e-003, 2.175158e-003, -1.128639e-003, -4.670638e-003, -8.228711e-003, -1.155847e-002, -1.440592e-002, -1.652146e-002, -1.767428e-002, -1.766646e-002, -1.634584e-002, -1.361702e-002, -9.449484e-003, -3.882758e-003, 2.972298e-003, 1.093585e-002, 1.976533e-002, 2.916516e-002, 3.879963e-002, 4.830815e-002, 5.732224e-002, 6.548326e-002, 7.245992e-002, 7.796474e-002, 8.176836e-002, 8.371112e-002, 8.371112e-002, 8.176836e-002, 7.796474e-002, 7.245992e-002, 6.548326e-002, 5.732224e-002, 4.830815e-002, 3.879963e-002, 2.916516e-002, 1.976533e-002, 1.093585e-002, 2.972298e-003, -3.882758e-003, -9.449484e-003, -1.361702e-002, -1.634584e-002, -1.766646e-002, -1.767428e-002, -1.652146e-002, -1.440592e-002, -1.155847e-002, -8.228711e-003, -4.670638e-003, -1.128639e-003, 2.175158e-003, 5.051755e-003, 7.353569e-003, 8.980045e-003, 9.880123e-003, 1.005183e-002, 9.538923e-003, 8.425123e-003, 6.826232e-003, 4.880896e-003, 2.740409e-003, 5.585060e-004, -1.518559e-003, -3.360701e-003, -4.861362e-003, -5.942780e-003, -6.559325e-003, -6.698420e-003, -6.379504e-003, -5.650810e-003, -4.584688e-003, -3.271431e-003, -1.812484e-003, -3.131100e-004, 1.124594e-003, 2.408302e-003, 3.460874e-003, 4.224689e-003, 4.664279e-003, 4.767585e-003, 4.545400e-003, 4.029575e-003, 3.269698e-003, 2.329036e-003, 1.279523e-003, 1.967020e-004, -8.455660e-004, -1.779770e-003, -2.548995e-003, -3.110028e-003, -3.435638e-003, -3.515455e-003, -3.355931e-003, -2.978974e-003, -2.419861e-003, -1.724144e-003, -9.442820e-004, -1.357370e-004, 6.466750e-004, 1.352477e-003, 1.938479e-003, 2.371398e-003, 2.629410e-003, 2.703167e-003, 2.595689e-003, 2.321735e-003, 1.906188e-003, 1.382130e-003, 7.882490e-004, 1.662540e-004, -4.420030e-004, -9.970050e-004, -1.464052e-003, -1.815018e-003, -2.029896e-003, -2.097461e-003, -2.015672e-003, -1.791166e-003, -1.438574e-003, -9.790910e-004, -4.390040e-004, 1.522290e-004, 7.637350e-004, 1.365157e-003, 1.928244e-003, 2.428372e-003, 2.845570e-003, 3.165405e-003, 3.379222e-003, 3.484316e-003, 3.483449e-003, 3.384406e-003, 3.198943e-003, 2.942019e-003, 2.630493e-003, 2.282215e-003, 1.914834e-003, 1.544988e-003, 1.187433e-003, 8.545590e-004, 5.560150e-004, 2.984710e-004, 8.570000e-005, -8.130000e-005, -2.038980e-004, -2.854140e-004, -3.307330e-004, -3.455650e-004, -3.364770e-004, -3.096190e-004, -2.711770e-004, -2.265160e-004, -1.801960e-004, -1.358070e-004, -9.600000e-005, -9.930000e-005 };



        private int bufferSize_lp = 260;     //length of the low pass filter
        private int bufferSize_hp = 511;     //length of the high pass filter

        //hold the filtered data
        private double filtered;

        //hold the raw data in a continuously updating buffer
        private double[] eegBuffer;
        private double[] tempeegBuffer;

        private double[] lpBuffer;
        private double[] templpBuffer;

        //counter to track when we should plot
        private int bufferCounter_raw;
        private int bufferCounter_lp;

        public Launcher() {


            debugOutFile = "debug.txt";
            this.debugStream = new System.IO.StreamWriter(debugOutFile, true);

            
            
            
            
            
            mainForm = new MainForm();

            connector = new Connector();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceFound += new EventHandler(OnDeviceFound);
            connector.DeviceNotFound += new EventHandler(OnDeviceNotFound);
            connector.DeviceConnectFail += new EventHandler(OnDeviceNotFound);
            connector.DeviceDisconnected += new EventHandler(OnDeviceDisconnected);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);

            mainForm.ConnectButtonClicked += new EventHandler(OnConnectButtonClicked);
            mainForm.DisconnectButtonClicked += new EventHandler(OnDisconnectButtonClicked);
            mainForm.Disposed += new EventHandler(OnMainFormDisposed);

            InitializeComponent();

            this.MaximumSize = new Size(391, 361);
            this.MinimumSize = this.MaximumSize;

            rawCounter = 0;     //initially zero
            delay = 512 * 5;    //5 seconds delay

            
            //hold the raw data in a continuously updating buffer
            eegBuffer = new double[bufferSize_lp];
            tempeegBuffer = new double[bufferSize_lp];

            lpBuffer = new double[bufferSize_hp];
            templpBuffer = new double[bufferSize_hp];

            //counter to track when we should plot
            bufferCounter_raw = 0;
            bufferCounter_lp = 0;
            
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.Run(new Launcher());

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
                //send the configuration bytes to the chip. this happens immediately and only once
                if(thinkGearParser.ParsedData[i].ContainsKey("BMDConfig")) {
                    if(bytesToSend == null) {
                        bytesToSend = new byte[8] { 0xAA, 0xAA, 0x04, 0x03, 0x40, 0xF9, 0x00, (byte)thinkGearParser.ParsedData[i]["BMDConfig"] };
                        connector.Send(device.PortName, bytesToSend);
                    }
                }

                //save the poorsignal value. this is always updated
                if(thinkGearParser.ParsedData[i].ContainsKey("PoorSignal")) {
                    mainForm.poorQuality = thinkGearParser.ParsedData[i]["PoorSignal"];
                }


                if(thinkGearParser.ParsedData[i].ContainsKey("Raw")) {

                    //if signal is good
                    if(mainForm.poorQuality == 200) {
                        rawCounter++;

                        //update the buffer with the latest eeg value
                        Array.Copy(eegBuffer, 1, tempeegBuffer, 0, bufferSize_lp - 1);
                        tempeegBuffer[bufferSize_lp - 1] = (double)thinkGearParser.ParsedData[i]["Raw"];
                        Array.Copy(tempeegBuffer, eegBuffer, bufferSize_lp);
                        bufferCounter_raw++;

                        //if the eeg buffer is full, and "delay" seconds have already passed
                        if((rawCounter >= delay) && (bufferCounter_raw >= bufferSize_lp)) {

                            //filter the data and update the lp buffer
                            filtered = applyFilter(eegBuffer,lp_coeff);
                            Array.Copy(lpBuffer, 1, templpBuffer, 0, bufferSize_hp - 1);
                            templpBuffer[bufferSize_hp - 1] = filtered;
                            Array.Copy(templpBuffer, lpBuffer, bufferSize_hp);
                            bufferCounter_lp++;

                            //if the lp buffer is full
                            if(bufferCounter_lp >= bufferSize_hp) {

                                //filter the data and plot it (this is now bandpassed data)
                                filtered = applyFilter(lpBuffer, hp_coeff);
                                mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), filtered));
                                mainForm.rawGraphPanel.LineGraph.timeStampIndex++;

                            } else {
                                //else, the lp buffer is not yet full. plot zero
                                mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                                mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                            }
                            
                        } else {
                            //raw buffer is not full yet. plot zero
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                            mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                        }

                    } else {
                        //otherwise signal is bad, plot zero. reset counter
                        rawCounter = 0;
                        bufferCounter_lp = 0;
                        bufferCounter_raw = 0;

                        Array.Clear(eegBuffer, 0, eegBuffer.Length);
                        Array.Clear(lpBuffer, 0, lpBuffer.Length);

                        mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                        mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
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
