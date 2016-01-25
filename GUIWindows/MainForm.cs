using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Media;
using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;
using System.Text;


//comment here

namespace NeuroSky.MindView
{
    public class MainForm : System.Windows.Forms.Form
    {
        private SaveFileGUI saveFileGUI;        //for saving the EEG data
        private UserAgeInputGUI heartAgeInputGUI;  //for inputing age and file name
        private RelaxationLevel relaxationLevel;
        private AddNewUserGUI addNewUerGUI;
        public RecognitionRecordingGUI identificationRecordingGUI;//display trained steps
        public RecognitionGUI identificationGUI;//display the identification result

        public bool runFatigueMeter = false;   //fatigue meter is off by default
        private int fatigueResult;              //output of the EnergyLevel algorithm
        private int fatigueTime;                //holds a record of how many seconds have passed since the RR recording began

        public GraphPanel rawGraphPanel;
        public GraphPanel graphPanel1;// = new NeuroSky.MindView.GraphPanel(PlotType.Bar);
        public TextBox portText;
        private Label statusLabel;
        private Label fileLabel;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button stopButton;

        private System.Windows.Forms.Button inputAgeAndFileNameButton;
        private System.ComponentModel.Container components = null;
        public int timeStampIndex = 0;
        public bool recordFlag;
        public DateTime RecordStartTime;
        public DateTime RecordStopTime;
        public Label averageHeartRateLabel;
        public double poorQuality;
        public byte TrimByte;
        private int ADCValue;
        private double tempADCValue;
        private int tempIntADCValue;
        private String[] newHighLowByte;

        public double ASICHBValue;     //actual value coming out of the chip
        public double realTimeHBValue;
        private int realTimeHBBufferLength;
        private int realTimeHBCounter;
        private double[] realTimeHBValueBuffer;
        public double avgHBValue;
        private int avgHBBufferLength;
        private int avgHBCounter;
        private double[] avgHBValueBuffer;

        private string dataLogOutFile;
        private System.IO.StreamWriter dataLogStream;
        private string ECGLogOutFile;
        private System.IO.StreamWriter ECGLogStream;
        private string currentPath;
        public event EventHandler ConnectButtonClicked = delegate { };
        public event EventHandler DisconnectButtonClicked = delegate { };
        public event EventHandler ConfirmHeartAgeButtonClicked = delegate { };
        public event EventHandler IdentificationButtonClicked = delegate { };
        public event EventHandler NewUserButtonClicked = delegate { };
        public event EventHandler ReplyButtonClicked = delegate { };
        public event EventHandler StopReplayButtonClicked = delegate { };

        public CheckBox soundCheckBox;
        private Label fatigueLabelIndicator;
        public Label fatigueLabel;
        private Button startFatigueButton;
        private Button stopFatigueButton;
        private PictureBox energyPictureBox;
        public SoundPlayer player;
        private Bitmap emptyImage;
        private Bitmap lowImage;
        private Bitmap mediumImage;
        private Label HRVLabelIndicator;
        public Label HRVLabel;
        private Bitmap fullImage;
        private Label respirationRateLabel;
        public Label respirationRateIndicator;
        private Label heartAgeLabel;
        public Label heartAgeIndicator;
        public Button Replay;
        public string loadedFileData;
        public Button stopReplay;
        private Button UART_Close_Button;
        private Button UART_Open_Button;
        private Button SPI_Close_Button;
        private Button SPI_Open_Button;
        private Button I2C_Close_Button;
        private Button I2C_Open_Button;
        private Button button7;
        private Label label1;
        private Label label2;
        public TextBox textBox1;
        public TextBox textBox2;
        public TextBox textBox3;
        public TextBox textBox4;
        public TextBox textBox5;
        public TextBox textBox6;
        public TextBox textBox7;
        public TextBox textBox8;
        private TextBox textBox9;
        private TextBox textBox10;
        private TextBox textBox11;
        private TextBox textBox12;
        private TextBox textBox13;
        private TextBox textBox14;
        private TextBox textBox15;
        private TextBox textBox16;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label20;
        private TextBox textBox17;
        private TextBox textBox18;
        private TextBox Write_Byte0_Box;
        private TextBox Write_Byte11_Box;
        private TextBox Write_Byte12_Box;
        private TextBox textBox23;
        private TextBox textBox24;
        private TextBox Write_Byte13_Box;
        private TextBox Write_Byte2_Box;
        private TextBox textBox27;
        private TextBox textBox28;
        private TextBox Write_Byte14_Box;
        private TextBox Write_Byte3_Box;
        private TextBox textBox31;
        private TextBox textBox32;
        private TextBox Write_Byte15_Box;
        private TextBox Write_Byte4_Box;
        private TextBox textBox35;
        private TextBox textBox36;
        private TextBox Write_Byte16_Box;
        private TextBox Write_Byte5_Box;
        private TextBox textBox39;
        private TextBox textBox40;
        private TextBox Write_Byte17_Box;
        private TextBox Write_Byte6_Box;
        private TextBox textBox43;
        private TextBox textBox44;
        private TextBox Write_Byte18_Box;
        private TextBox Write_Byte7_Box;
        private TextBox textBox47;
        private TextBox textBox48;
        private TextBox Write_Byte19_Box;
        private TextBox Write_Byte8_Box;
        private TextBox textBox51;
        private TextBox textBox52;
        private TextBox Write_Byte20_Box;
        private TextBox Write_Byte9_Box;
        private TextBox textBox55;
        private TextBox textBox56;
        private TextBox Write_Byte10_Box;
        private TextBox textBox60;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label29;
        private Label label30;
        private Label label31;
        private Label label32;
        private Label label33;
        private Label label34;
        private Label label35;
        private Label label36;
        private Label label37;
        private Label label38;
        private Label label39;
        private Label label40;
        private Label label41;
        private Label label42;
        private Label label43;
        private Label label44;
        private Label label45;
        private Label label46;
        private Label label47;
        private Label label48;
        private Label label49;
        private Label label50;
        private Label label51;
        private Label label52;
        private Label label53;
        private Label label54;
        private Label label55;
        private Label label56;
        private Label label57;
        private Label label58;
        private Label label59;
        private Label label60;
        private Label label61;
        private Label label62;
        private Label averageHeartRateLabelIndicator;
        public Label realtimeHeartRateLabel;
        private Label realtimeHeartRateLabelIndicator;
        private Label label63;
        private TextBox HBR_Box;
        private TextBox Interval_textBox;
        private Label label64;
        private Label label65;
        private TextBox Signal_QualityBox;
        private Label label66;
        private TextBox Trim_ByteBox;
        private Button button9;
        private Button button10;
        private Button Sample_Rate300;
        private Button Sample_Rate600;
        private Button button1;
        private Button button2;
        private TextBox Write_Byte1_Box;
        private Button button3;
        public Boolean replayEnable;
        public bool UART_ENABLED = false;
        public bool SPI_ENABLED = false;
        private Button button4;
        private Label label72;
        private Label label73;
        private Label label74;
        private Label label75;
        private Label label76;
        private Label label77;
        private Label label78;
        private Label label79;
        private Label label80;
        private Label label81;
        private Label label82;
        private Label label83;
        private Label label84;
        private Label label85;
        private Label label86;
        private Label label87;
        private TextBox textBox26;
        private TextBox textBox29;
        private TextBox textBox30;
        private TextBox textBox33;
        private TextBox textBox34;
        private TextBox textBox37;
        private TextBox textBox38;
        private TextBox textBox41;
        private TextBox textBox42;
        private TextBox textBox45;
        private TextBox textBox46;
        private TextBox textBox49;
        private TextBox textBox50;
        private TextBox textBox53;
        private TextBox textBox54;
        private TextBox textBox57;
        private Label label67;
        private Label label68;
        private TextBox textBox19;
        private Label label69;
        private Label label70;
        private Label label71;
        private Button button5;
        private Button button6;
        private Label label88;
        private Label label89;
        private Button button14;
        private Button button8;
        private Button button11;
        private Button button13;
        private Button button12;
        private Button button15;
        private Button button16;
        private Label label90;
        public bool I2C_ENABLED = false;
        private Button identificationButton;
        private Button newUserButton;


        public MainForm()
        {
            saveFileGUI = new SaveFileGUI();
            saveFileGUI.SaveButtonClicked += new EventHandler(OnSaveButtonClicked);
            saveFileGUI.DiscardButtonClicked += new EventHandler(OnDiscardButtonClicked);
            saveFileGUI.BrowseButtonClicked += new EventHandler(OnBrowseButtonClicked);
            saveFileGUI.StartPosition = FormStartPosition.Manual;

            heartAgeInputGUI = new UserAgeInputGUI();
            heartAgeInputGUI.ConfirmButtonClicked += new EventHandler(OnConfirmButtonClicked);

            addNewUerGUI = new AddNewUserGUI();
            addNewUerGUI.OnSaveNewUserNameButtonClicked += new EventHandler(OnSaveNewUserNameButtonClicked);

            identificationRecordingGUI = new RecognitionRecordingGUI();
            identificationGUI = new RecognitionGUI();

            relaxationLevel = new RelaxationLevel();
            InitializeComponent();

            recordFlag = false;
            replayEnable = false;
            rawGraphPanel.samplingRate = 600;
            rawGraphPanel.xAxisMax = 5;
            rawGraphPanel.xAxisMin = 0;
            rawGraphPanel.yAxisMax = 2.5;
            rawGraphPanel.yAxisMin = -2.5;

            rawGraphPanel.Text = "ECG";
            rawGraphPanel.EnableValueDisplay();
            rawGraphPanel.OptimizeScrollBar();
            rawGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);
            if (rawGraphPanel.PlotType == PlotType.Line)
            {
                rawGraphPanel.LineGraph.DCRemovalEnabled = false;
            }


            disconnectButton.Visible = false;
            disconnectButton.Enabled = false;
            stopButton.Visible = false;
            stopButton.Enabled = false;
            ASICHBValue = 0;

            avgHBValue = 0;
            avgHBValue = 0;
            avgHBCounter = 0;
            avgHBBufferLength = 30;
            avgHBValueBuffer = new double[avgHBBufferLength];

            realTimeHBValue = 0;
            realTimeHBCounter = 0;
            realTimeHBBufferLength = 1;
            realTimeHBValueBuffer = new double[realTimeHBBufferLength];

            currentPath = Directory.GetCurrentDirectory();

            this.MinimumSize = new System.Drawing.Size(711, 322);

            //grab the embedded audio file to play. note that player.Play() is a nonblocking function, so you can plot
            //and play audio and record data at the same time
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream s = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.heartbeep.wav");
            player = new SoundPlayer(s);

            System.IO.Stream emptyStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.empty.gif");
            emptyImage = new Bitmap(emptyStream);

            System.IO.Stream lowStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.low.gif");
            lowImage = new Bitmap(lowStream);

            System.IO.Stream mediumStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.medium.gif");
            mediumImage = new Bitmap(mediumStream);

            System.IO.Stream fullStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.full.gif");
            fullImage = new Bitmap(fullStream);

            #region Initialize FFT Drawing
            graphPanel1.samplingRate = 512;
            graphPanel1.xAxisMax = 512;
            graphPanel1.xAxisMin = 0;
            graphPanel1.yAxisMax = 2000;
            graphPanel1.yAxisMin = 0;
            graphPanel1.Text = "FFT";
            graphPanel1.EnableValueDisplay();
            graphPanel1.OptimizeScrollBar();
            //graphPanel1.DataSavingFinished += new EventHandler(OnDataSavingFinished);
            graphPanel1.BarGraph.BarReadType = ReadType.FFTArray;
            if (graphPanel1.PlotType == PlotType.Bar)
            {
                graphPanel1.BarGraph.pwrSpecWindow = 1;
                //graphPanel1.BarGraph.BarReadType = ReadType.RawArray;
            }
            #endregion
        }

        //transfer user input parameters
        void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            string filename = heartAgeInputGUI.getFilename();
            int age = heartAgeInputGUI.getAge();

            HeartAgeEventArgs heartAgeEventArgs = new HeartAgeEventArgs(age, filename);     //cast this as a HeartAgeEvent

            ConfirmHeartAgeButtonClicked(this, heartAgeEventArgs);
        }
        //transfer new user name
        void OnSaveNewUserNameButtonClicked(object sender, EventArgs e)
        {
            string newUserName = addNewUerGUI.getNewUserName();
            NewUserNameEventArgs newAddUserNameEventArgs = new NewUserNameEventArgs(newUserName);
            NewUserButtonClicked(this, newAddUserNameEventArgs);
            identificationRecordingGUI.Show();//open window to display the trained steps
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Disposing MainForm.");
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rawGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.connectButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.portText = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.fileLabel = new System.Windows.Forms.Label();
            this.averageHeartRateLabel = new System.Windows.Forms.Label();
            this.soundCheckBox = new System.Windows.Forms.CheckBox();
            this.fatigueLabelIndicator = new System.Windows.Forms.Label();
            this.fatigueLabel = new System.Windows.Forms.Label();
            this.energyPictureBox = new System.Windows.Forms.PictureBox();
            this.HRVLabelIndicator = new System.Windows.Forms.Label();
            this.HRVLabel = new System.Windows.Forms.Label();
            this.respirationRateLabel = new System.Windows.Forms.Label();
            this.respirationRateIndicator = new System.Windows.Forms.Label();
            this.heartAgeLabel = new System.Windows.Forms.Label();
            this.heartAgeIndicator = new System.Windows.Forms.Label();
            this.Replay = new System.Windows.Forms.Button();
            this.stopReplay = new System.Windows.Forms.Button();
            this.UART_Close_Button = new System.Windows.Forms.Button();
            this.UART_Open_Button = new System.Windows.Forms.Button();
            this.SPI_Close_Button = new System.Windows.Forms.Button();
            this.SPI_Open_Button = new System.Windows.Forms.Button();
            this.I2C_Close_Button = new System.Windows.Forms.Button();
            this.I2C_Open_Button = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox17 = new System.Windows.Forms.TextBox();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.Write_Byte0_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte11_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte12_Box = new System.Windows.Forms.TextBox();
            this.textBox23 = new System.Windows.Forms.TextBox();
            this.textBox24 = new System.Windows.Forms.TextBox();
            this.Write_Byte13_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte2_Box = new System.Windows.Forms.TextBox();
            this.textBox27 = new System.Windows.Forms.TextBox();
            this.textBox28 = new System.Windows.Forms.TextBox();
            this.Write_Byte14_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte3_Box = new System.Windows.Forms.TextBox();
            this.textBox31 = new System.Windows.Forms.TextBox();
            this.textBox32 = new System.Windows.Forms.TextBox();
            this.Write_Byte15_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte4_Box = new System.Windows.Forms.TextBox();
            this.textBox35 = new System.Windows.Forms.TextBox();
            this.textBox36 = new System.Windows.Forms.TextBox();
            this.Write_Byte16_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte5_Box = new System.Windows.Forms.TextBox();
            this.textBox39 = new System.Windows.Forms.TextBox();
            this.textBox40 = new System.Windows.Forms.TextBox();
            this.Write_Byte17_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte6_Box = new System.Windows.Forms.TextBox();
            this.textBox43 = new System.Windows.Forms.TextBox();
            this.textBox44 = new System.Windows.Forms.TextBox();
            this.Write_Byte18_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte7_Box = new System.Windows.Forms.TextBox();
            this.textBox47 = new System.Windows.Forms.TextBox();
            this.textBox48 = new System.Windows.Forms.TextBox();
            this.Write_Byte19_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte8_Box = new System.Windows.Forms.TextBox();
            this.textBox51 = new System.Windows.Forms.TextBox();
            this.textBox52 = new System.Windows.Forms.TextBox();
            this.Write_Byte20_Box = new System.Windows.Forms.TextBox();
            this.Write_Byte9_Box = new System.Windows.Forms.TextBox();
            this.textBox55 = new System.Windows.Forms.TextBox();
            this.textBox56 = new System.Windows.Forms.TextBox();
            this.Write_Byte10_Box = new System.Windows.Forms.TextBox();
            this.textBox60 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.averageHeartRateLabelIndicator = new System.Windows.Forms.Label();
            this.realtimeHeartRateLabel = new System.Windows.Forms.Label();
            this.realtimeHeartRateLabelIndicator = new System.Windows.Forms.Label();
            this.label63 = new System.Windows.Forms.Label();
            this.HBR_Box = new System.Windows.Forms.TextBox();
            this.Interval_textBox = new System.Windows.Forms.TextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.Signal_QualityBox = new System.Windows.Forms.TextBox();
            this.label66 = new System.Windows.Forms.Label();
            this.Trim_ByteBox = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.Sample_Rate300 = new System.Windows.Forms.Button();
            this.Sample_Rate600 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Write_Byte1_Box = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label72 = new System.Windows.Forms.Label();
            this.label73 = new System.Windows.Forms.Label();
            this.label74 = new System.Windows.Forms.Label();
            this.label75 = new System.Windows.Forms.Label();
            this.label76 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.label78 = new System.Windows.Forms.Label();
            this.label79 = new System.Windows.Forms.Label();
            this.label80 = new System.Windows.Forms.Label();
            this.label81 = new System.Windows.Forms.Label();
            this.label82 = new System.Windows.Forms.Label();
            this.label83 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.label87 = new System.Windows.Forms.Label();
            this.textBox26 = new System.Windows.Forms.TextBox();
            this.textBox29 = new System.Windows.Forms.TextBox();
            this.textBox30 = new System.Windows.Forms.TextBox();
            this.textBox33 = new System.Windows.Forms.TextBox();
            this.textBox34 = new System.Windows.Forms.TextBox();
            this.textBox37 = new System.Windows.Forms.TextBox();
            this.textBox38 = new System.Windows.Forms.TextBox();
            this.textBox41 = new System.Windows.Forms.TextBox();
            this.textBox42 = new System.Windows.Forms.TextBox();
            this.textBox45 = new System.Windows.Forms.TextBox();
            this.textBox46 = new System.Windows.Forms.TextBox();
            this.textBox49 = new System.Windows.Forms.TextBox();
            this.textBox50 = new System.Windows.Forms.TextBox();
            this.textBox53 = new System.Windows.Forms.TextBox();
            this.textBox54 = new System.Windows.Forms.TextBox();
            this.textBox57 = new System.Windows.Forms.TextBox();
            this.label67 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.label69 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label88 = new System.Windows.Forms.Label();
            this.label89 = new System.Windows.Forms.Label();
            this.button14 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.label90 = new System.Windows.Forms.Label();
            this.graphPanel1 = new NeuroSky.MindView.GraphPanel(PlotType.Bar);
            ((System.ComponentModel.ISupportInitialize)(this.energyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rawGraphPanel.Location = new System.Drawing.Point(1, 95);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.samplingRate = 10;
            this.rawGraphPanel.Size = new System.Drawing.Size(1040, 227);
            this.rawGraphPanel.TabIndex = 0;
            this.rawGraphPanel.xAxisMax = 0D;
            this.rawGraphPanel.xAxisMin = 0D;
            this.rawGraphPanel.yAxisMax = 0D;
            this.rawGraphPanel.yAxisMin = 0D;
            // 
            // connectButton
            // 
            this.connectButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectButton.Location = new System.Drawing.Point(102, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(80, 24);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearButton.Location = new System.Drawing.Point(948, 326);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(95, 36);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear Graph";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recordButton.Location = new System.Drawing.Point(963, 368);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(80, 35);
            this.recordButton.TabIndex = 1;
            this.recordButton.Text = "Record";
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disconnectButton.Location = new System.Drawing.Point(102, 12);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(80, 24);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.Click += new System.EventHandler(this.disconnect_Click);
            // 
            // stopButton
            // 
            this.stopButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopButton.Location = new System.Drawing.Point(963, 369);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 32);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // portText
            // 
            this.portText.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portText.Location = new System.Drawing.Point(10, 14);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(80, 21);
            this.portText.TabIndex = 2;
            this.portText.Text = "Auto";
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.Color.Transparent;
            this.statusLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.Crimson;
            this.statusLabel.Location = new System.Drawing.Point(9, 400);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(286, 21);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Type COM Port To Connect And Press Connect!!";
            // 
            // fileLabel
            // 
            this.fileLabel.Location = new System.Drawing.Point(0, 15);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(800, 24);
            this.fileLabel.TabIndex = 3;
            this.fileLabel.Text = "None";
            // 
            // averageHeartRateLabel
            // 
            this.averageHeartRateLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.averageHeartRateLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.averageHeartRateLabel.Location = new System.Drawing.Point(1207, 7);
            this.averageHeartRateLabel.Name = "averageHeartRateLabel";
            this.averageHeartRateLabel.Size = new System.Drawing.Size(92, 24);
            this.averageHeartRateLabel.TabIndex = 14;
            this.averageHeartRateLabel.Text = "0";
            this.averageHeartRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.averageHeartRateLabel.Visible = false;
            // 
            // soundCheckBox
            // 
            this.soundCheckBox.AutoSize = true;
            this.soundCheckBox.Font = new System.Drawing.Font("Arial", 9F);
            this.soundCheckBox.Location = new System.Drawing.Point(189, 12);
            this.soundCheckBox.Name = "soundCheckBox";
            this.soundCheckBox.Size = new System.Drawing.Size(104, 19);
            this.soundCheckBox.TabIndex = 17;
            this.soundCheckBox.Text = "Enable Sound";
            this.soundCheckBox.UseVisualStyleBackColor = true;
            // 
            // fatigueLabelIndicator
            // 
            this.fatigueLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fatigueLabelIndicator.Location = new System.Drawing.Point(507, 8);
            this.fatigueLabelIndicator.Name = "fatigueLabelIndicator";
            this.fatigueLabelIndicator.Size = new System.Drawing.Size(107, 19);
            this.fatigueLabelIndicator.TabIndex = 20;
            this.fatigueLabelIndicator.Text = "Relaxation Level:";
            this.fatigueLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fatigueLabelIndicator.Visible = false;
            // 
            // fatigueLabel
            // 
            this.fatigueLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fatigueLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fatigueLabel.Location = new System.Drawing.Point(741, 9);
            this.fatigueLabel.Name = "fatigueLabel";
            this.fatigueLabel.Size = new System.Drawing.Size(83, 24);
            this.fatigueLabel.TabIndex = 19;
            this.fatigueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fatigueLabel.Visible = false;
            // 
            // energyPictureBox
            // 
            this.energyPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.energyPictureBox.ErrorImage = null;
            this.energyPictureBox.InitialImage = global::NeuroSky.ThinkGear.Properties.Resources.full;
            this.energyPictureBox.Location = new System.Drawing.Point(620, 7);
            this.energyPictureBox.Name = "energyPictureBox";
            this.energyPictureBox.Size = new System.Drawing.Size(115, 29);
            this.energyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.energyPictureBox.TabIndex = 22;
            this.energyPictureBox.TabStop = false;
            this.energyPictureBox.Visible = false;
            // 
            // HRVLabelIndicator
            // 
            this.HRVLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HRVLabelIndicator.Location = new System.Drawing.Point(1273, 40);
            this.HRVLabelIndicator.Name = "HRVLabelIndicator";
            this.HRVLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.HRVLabelIndicator.TabIndex = 24;
            this.HRVLabelIndicator.Text = "R-R interval (SDK):";
            this.HRVLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HRVLabelIndicator.Visible = false;
            // 
            // HRVLabel
            // 
            this.HRVLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HRVLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HRVLabel.Location = new System.Drawing.Point(1423, 36);
            this.HRVLabel.Name = "HRVLabel";
            this.HRVLabel.Size = new System.Drawing.Size(96, 24);
            this.HRVLabel.TabIndex = 23;
            this.HRVLabel.Text = "0";
            this.HRVLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HRVLabel.Visible = false;
            // 
            // respirationRateLabel
            // 
            this.respirationRateLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.respirationRateLabel.Location = new System.Drawing.Point(343, 28);
            this.respirationRateLabel.Name = "respirationRateLabel";
            this.respirationRateLabel.Size = new System.Drawing.Size(132, 19);
            this.respirationRateLabel.TabIndex = 16;
            this.respirationRateLabel.Text = "Respiration Rate:";
            this.respirationRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.respirationRateLabel.Visible = false;
            // 
            // respirationRateIndicator
            // 
            this.respirationRateIndicator.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.respirationRateIndicator.Location = new System.Drawing.Point(443, 26);
            this.respirationRateIndicator.Name = "respirationRateIndicator";
            this.respirationRateIndicator.Size = new System.Drawing.Size(132, 19);
            this.respirationRateIndicator.TabIndex = 16;
            this.respirationRateIndicator.Text = "0";
            this.respirationRateIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.respirationRateIndicator.Visible = false;
            // 
            // heartAgeLabel
            // 
            this.heartAgeLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heartAgeLabel.Location = new System.Drawing.Point(343, 9);
            this.heartAgeLabel.Name = "heartAgeLabel";
            this.heartAgeLabel.Size = new System.Drawing.Size(132, 19);
            this.heartAgeLabel.TabIndex = 16;
            this.heartAgeLabel.Text = "Heart Age:";
            this.heartAgeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.heartAgeLabel.Visible = false;
            // 
            // heartAgeIndicator
            // 
            this.heartAgeIndicator.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.heartAgeIndicator.Location = new System.Drawing.Point(426, 7);
            this.heartAgeIndicator.Name = "heartAgeIndicator";
            this.heartAgeIndicator.Size = new System.Drawing.Size(132, 19);
            this.heartAgeIndicator.TabIndex = 16;
            this.heartAgeIndicator.Text = "0";
            this.heartAgeIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.heartAgeIndicator.Visible = false;
            // 
            // Replay
            // 
            this.Replay.Location = new System.Drawing.Point(10, 57);
            this.Replay.Name = "Replay";
            this.Replay.Size = new System.Drawing.Size(75, 23);
            this.Replay.TabIndex = 25;
            this.Replay.Text = "Replay";
            this.Replay.UseVisualStyleBackColor = true;
            this.Replay.Visible = false;
            this.Replay.Click += new System.EventHandler(this.Replay_Click);
            // 
            // stopReplay
            // 
            this.stopReplay.Location = new System.Drawing.Point(10, 57);
            this.stopReplay.Name = "stopReplay";
            this.stopReplay.Size = new System.Drawing.Size(75, 23);
            this.stopReplay.TabIndex = 26;
            this.stopReplay.Text = "Stop";
            this.stopReplay.UseVisualStyleBackColor = true;
            this.stopReplay.Visible = false;
            this.stopReplay.Click += new System.EventHandler(this.stopReplay_Click);
            // 
            // UART_Close_Button
            // 
            this.UART_Close_Button.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UART_Close_Button.Location = new System.Drawing.Point(522, 324);
            this.UART_Close_Button.Name = "UART_Close_Button";
            this.UART_Close_Button.Size = new System.Drawing.Size(75, 33);
            this.UART_Close_Button.TabIndex = 27;
            this.UART_Close_Button.Text = "UART Mode Disable";
            this.UART_Close_Button.UseVisualStyleBackColor = true;
            this.UART_Close_Button.Click += new System.EventHandler(this.button1_Click);
            // 
            // UART_Open_Button
            // 
            this.UART_Open_Button.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UART_Open_Button.Location = new System.Drawing.Point(522, 324);
            this.UART_Open_Button.Name = "UART_Open_Button";
            this.UART_Open_Button.Size = new System.Drawing.Size(75, 33);
            this.UART_Open_Button.TabIndex = 28;
            this.UART_Open_Button.Text = "UART Mode Enable";
            this.UART_Open_Button.UseVisualStyleBackColor = true;
            this.UART_Open_Button.Click += new System.EventHandler(this.button2_Click);
            // 
            // SPI_Close_Button
            // 
            this.SPI_Close_Button.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SPI_Close_Button.Location = new System.Drawing.Point(1062, 75);
            this.SPI_Close_Button.Name = "SPI_Close_Button";
            this.SPI_Close_Button.Size = new System.Drawing.Size(75, 33);
            this.SPI_Close_Button.TabIndex = 29;
            this.SPI_Close_Button.Text = "SPI Mode Disable";
            this.SPI_Close_Button.UseVisualStyleBackColor = true;
            this.SPI_Close_Button.Click += new System.EventHandler(this.button3_Click);
            // 
            // SPI_Open_Button
            // 
            this.SPI_Open_Button.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SPI_Open_Button.Location = new System.Drawing.Point(1061, 75);
            this.SPI_Open_Button.Name = "SPI_Open_Button";
            this.SPI_Open_Button.Size = new System.Drawing.Size(75, 33);
            this.SPI_Open_Button.TabIndex = 30;
            this.SPI_Open_Button.Text = "SPI Mode Enable";
            this.SPI_Open_Button.UseVisualStyleBackColor = true;
            this.SPI_Open_Button.Click += new System.EventHandler(this.button4_Click);
            // 
            // I2C_Close_Button
            // 
            this.I2C_Close_Button.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.I2C_Close_Button.Location = new System.Drawing.Point(1062, 36);
            this.I2C_Close_Button.Name = "I2C_Close_Button";
            this.I2C_Close_Button.Size = new System.Drawing.Size(75, 33);
            this.I2C_Close_Button.TabIndex = 31;
            this.I2C_Close_Button.Text = "I2C Mode Disable";
            this.I2C_Close_Button.UseVisualStyleBackColor = true;
            this.I2C_Close_Button.Click += new System.EventHandler(this.button5_Click);
            // 
            // I2C_Open_Button
            // 
            this.I2C_Open_Button.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.I2C_Open_Button.Location = new System.Drawing.Point(1061, 36);
            this.I2C_Open_Button.Name = "I2C_Open_Button";
            this.I2C_Open_Button.Size = new System.Drawing.Size(75, 33);
            this.I2C_Open_Button.TabIndex = 32;
            this.I2C_Open_Button.Text = "I2C Mode Enable";
            this.I2C_Open_Button.UseVisualStyleBackColor = true;
            this.I2C_Open_Button.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(479, 56);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 33);
            this.button7.TabIndex = 33;
            this.button7.Text = "Set Clock Freq";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(224, 325);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 15);
            this.label1.TabIndex = 39;
            this.label1.Text = "UART Register Table(Read)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(694, 325);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 15);
            this.label2.TabIndex = 40;
            this.label2.Text = "UART Register Table(Write)";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(109, 363);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(43, 21);
            this.textBox1.TabIndex = 41;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(158, 363);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(42, 21);
            this.textBox2.TabIndex = 42;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(206, 363);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(40, 21);
            this.textBox3.TabIndex = 43;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox4
            // 
            this.textBox4.Enabled = false;
            this.textBox4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(252, 363);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(43, 21);
            this.textBox4.TabIndex = 44;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox5
            // 
            this.textBox5.Enabled = false;
            this.textBox5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(302, 363);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(46, 21);
            this.textBox5.TabIndex = 45;
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox6
            // 
            this.textBox6.Enabled = false;
            this.textBox6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(355, 363);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(42, 21);
            this.textBox6.TabIndex = 46;
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox7
            // 
            this.textBox7.Enabled = false;
            this.textBox7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox7.Location = new System.Drawing.Point(403, 362);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(46, 21);
            this.textBox7.TabIndex = 47;
            this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox8
            // 
            this.textBox8.Enabled = false;
            this.textBox8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(455, 363);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(47, 21);
            this.textBox8.TabIndex = 48;
            this.textBox8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox9
            // 
            this.textBox9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(613, 363);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(35, 21);
            this.textBox9.TabIndex = 49;
            this.textBox9.Text = "06";
            this.textBox9.TextChanged += new System.EventHandler(this.textBox9_TextChanged);
            // 
            // textBox10
            // 
            this.textBox10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox10.Location = new System.Drawing.Point(654, 363);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(35, 21);
            this.textBox10.TabIndex = 50;
            // 
            // textBox11
            // 
            this.textBox11.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox11.Location = new System.Drawing.Point(695, 363);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(35, 21);
            this.textBox11.TabIndex = 51;
            this.textBox11.Text = "94";
            this.textBox11.TextChanged += new System.EventHandler(this.textBox11_TextChanged);
            // 
            // textBox12
            // 
            this.textBox12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(736, 363);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(35, 21);
            this.textBox12.TabIndex = 52;
            this.textBox12.Text = "F8";
            this.textBox12.TextChanged += new System.EventHandler(this.textBox12_TextChanged);
            // 
            // textBox13
            // 
            this.textBox13.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox13.Location = new System.Drawing.Point(777, 363);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(35, 21);
            this.textBox13.TabIndex = 53;
            this.textBox13.Text = "60";
            this.textBox13.TextChanged += new System.EventHandler(this.textBox13_TextChanged);
            // 
            // textBox14
            // 
            this.textBox14.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox14.Location = new System.Drawing.Point(818, 363);
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new System.Drawing.Size(35, 21);
            this.textBox14.TabIndex = 54;
            this.textBox14.TextChanged += new System.EventHandler(this.textBox14_TextChanged);
            // 
            // textBox15
            // 
            this.textBox15.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox15.Location = new System.Drawing.Point(859, 363);
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new System.Drawing.Size(35, 21);
            this.textBox15.TabIndex = 55;
            this.textBox15.Text = "42";
            this.textBox15.TextChanged += new System.EventHandler(this.textBox15_TextChanged);
            // 
            // textBox16
            // 
            this.textBox16.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox16.Location = new System.Drawing.Point(900, 363);
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new System.Drawing.Size(35, 21);
            this.textBox16.TabIndex = 56;
            this.textBox16.Text = "C1";
            this.textBox16.TextChanged += new System.EventHandler(this.textBox16_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(109, 343);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 14);
            this.label3.TabIndex = 57;
            this.label3.Text = "conOut0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(156, 343);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 14);
            this.label4.TabIndex = 58;
            this.label4.Text = "conOut1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(203, 343);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 14);
            this.label5.TabIndex = 59;
            this.label5.Text = "conOut2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(251, 343);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 14);
            this.label6.TabIndex = 60;
            this.label6.Text = "conOut3";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(303, 343);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 14);
            this.label7.TabIndex = 61;
            this.label7.Text = "conOut4";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(351, 343);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 14);
            this.label8.TabIndex = 62;
            this.label8.Text = "conOut5";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(402, 343);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 14);
            this.label9.TabIndex = 63;
            this.label9.Text = "conOut6";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(455, 344);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 14);
            this.label10.TabIndex = 64;
            this.label10.Text = "conOut7";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(654, 344);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 14);
            this.label11.TabIndex = 65;
            this.label11.Text = "Byte 1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(695, 344);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 14);
            this.label12.TabIndex = 66;
            this.label12.Text = "Byte 2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(613, 344);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(38, 14);
            this.label13.TabIndex = 67;
            this.label13.Text = "Byte 0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(736, 344);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(38, 14);
            this.label14.TabIndex = 68;
            this.label14.Text = "Byte 3";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(777, 343);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(38, 14);
            this.label15.TabIndex = 69;
            this.label15.Text = "Byte 4";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(818, 343);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(38, 14);
            this.label16.TabIndex = 70;
            this.label16.Text = "Byte 5";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(859, 344);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(38, 14);
            this.label17.TabIndex = 71;
            this.label17.Text = "Byte 6";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(898, 344);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 14);
            this.label18.TabIndex = 72;
            this.label18.Text = "Byte 7";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.Black;
            this.label19.Location = new System.Drawing.Point(1058, 117);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(136, 15);
            this.label19.TabIndex = 73;
            this.label19.Text = "SPI/I2C Register(Read)";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.Black;
            this.label20.Location = new System.Drawing.Point(1213, 117);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(134, 15);
            this.label20.TabIndex = 74;
            this.label20.Text = "SPI/I2C Register(Write)";
            // 
            // textBox17
            // 
            this.textBox17.Enabled = false;
            this.textBox17.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox17.Location = new System.Drawing.Point(1062, 156);
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new System.Drawing.Size(54, 21);
            this.textBox17.TabIndex = 75;
            this.textBox17.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox18
            // 
            this.textBox18.Enabled = false;
            this.textBox18.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox18.Location = new System.Drawing.Point(1132, 156);
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new System.Drawing.Size(54, 21);
            this.textBox18.TabIndex = 76;
            this.textBox18.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte0_Box
            // 
            this.Write_Byte0_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte0_Box.Location = new System.Drawing.Point(1219, 156);
            this.Write_Byte0_Box.Name = "Write_Byte0_Box";
            this.Write_Byte0_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte0_Box.TabIndex = 77;
            this.Write_Byte0_Box.Text = "C6";
            this.Write_Byte0_Box.TextChanged += new System.EventHandler(this.Write_Byte0_Box_TextChanged);
            // 
            // Write_Byte11_Box
            // 
            this.Write_Byte11_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte11_Box.Location = new System.Drawing.Point(1299, 156);
            this.Write_Byte11_Box.Name = "Write_Byte11_Box";
            this.Write_Byte11_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte11_Box.TabIndex = 78;
            this.Write_Byte11_Box.Text = "01";
            // 
            // Write_Byte12_Box
            // 
            this.Write_Byte12_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte12_Box.Location = new System.Drawing.Point(1299, 198);
            this.Write_Byte12_Box.Name = "Write_Byte12_Box";
            this.Write_Byte12_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte12_Box.TabIndex = 82;
            this.Write_Byte12_Box.Text = "E0";
            // 
            // textBox23
            // 
            this.textBox23.Enabled = false;
            this.textBox23.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox23.Location = new System.Drawing.Point(1132, 198);
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new System.Drawing.Size(54, 21);
            this.textBox23.TabIndex = 80;
            this.textBox23.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox24
            // 
            this.textBox24.Enabled = false;
            this.textBox24.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox24.Location = new System.Drawing.Point(1062, 198);
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new System.Drawing.Size(54, 21);
            this.textBox24.TabIndex = 79;
            this.textBox24.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte13_Box
            // 
            this.Write_Byte13_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte13_Box.Location = new System.Drawing.Point(1299, 241);
            this.Write_Byte13_Box.Name = "Write_Byte13_Box";
            this.Write_Byte13_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte13_Box.TabIndex = 86;
            this.Write_Byte13_Box.Text = "43";
            // 
            // Write_Byte2_Box
            // 
            this.Write_Byte2_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte2_Box.Location = new System.Drawing.Point(1219, 241);
            this.Write_Byte2_Box.Name = "Write_Byte2_Box";
            this.Write_Byte2_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte2_Box.TabIndex = 85;
            this.Write_Byte2_Box.Text = "80";
            // 
            // textBox27
            // 
            this.textBox27.Enabled = false;
            this.textBox27.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox27.Location = new System.Drawing.Point(1132, 241);
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new System.Drawing.Size(54, 21);
            this.textBox27.TabIndex = 84;
            this.textBox27.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox28
            // 
            this.textBox28.Enabled = false;
            this.textBox28.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox28.Location = new System.Drawing.Point(1062, 241);
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new System.Drawing.Size(54, 21);
            this.textBox28.TabIndex = 83;
            this.textBox28.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte14_Box
            // 
            this.Write_Byte14_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte14_Box.Location = new System.Drawing.Point(1299, 283);
            this.Write_Byte14_Box.Name = "Write_Byte14_Box";
            this.Write_Byte14_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte14_Box.TabIndex = 90;
            this.Write_Byte14_Box.Text = "00";
            // 
            // Write_Byte3_Box
            // 
            this.Write_Byte3_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte3_Box.Location = new System.Drawing.Point(1219, 283);
            this.Write_Byte3_Box.Name = "Write_Byte3_Box";
            this.Write_Byte3_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte3_Box.TabIndex = 89;
            this.Write_Byte3_Box.Text = "F8";
            // 
            // textBox31
            // 
            this.textBox31.Enabled = false;
            this.textBox31.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox31.Location = new System.Drawing.Point(1132, 283);
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new System.Drawing.Size(54, 21);
            this.textBox31.TabIndex = 88;
            this.textBox31.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox32
            // 
            this.textBox32.Enabled = false;
            this.textBox32.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox32.Location = new System.Drawing.Point(1062, 283);
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new System.Drawing.Size(54, 21);
            this.textBox32.TabIndex = 87;
            this.textBox32.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte15_Box
            // 
            this.Write_Byte15_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte15_Box.Location = new System.Drawing.Point(1299, 324);
            this.Write_Byte15_Box.Name = "Write_Byte15_Box";
            this.Write_Byte15_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte15_Box.TabIndex = 94;
            this.Write_Byte15_Box.Text = "02";
            // 
            // Write_Byte4_Box
            // 
            this.Write_Byte4_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte4_Box.Location = new System.Drawing.Point(1219, 324);
            this.Write_Byte4_Box.Name = "Write_Byte4_Box";
            this.Write_Byte4_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte4_Box.TabIndex = 93;
            this.Write_Byte4_Box.Text = "E0";
            // 
            // textBox35
            // 
            this.textBox35.Enabled = false;
            this.textBox35.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox35.Location = new System.Drawing.Point(1132, 324);
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new System.Drawing.Size(54, 21);
            this.textBox35.TabIndex = 92;
            this.textBox35.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox36
            // 
            this.textBox36.Enabled = false;
            this.textBox36.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox36.Location = new System.Drawing.Point(1062, 324);
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new System.Drawing.Size(54, 21);
            this.textBox36.TabIndex = 91;
            this.textBox36.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte16_Box
            // 
            this.Write_Byte16_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte16_Box.Location = new System.Drawing.Point(1299, 367);
            this.Write_Byte16_Box.Name = "Write_Byte16_Box";
            this.Write_Byte16_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte16_Box.TabIndex = 98;
            this.Write_Byte16_Box.Text = "C1";
            // 
            // Write_Byte5_Box
            // 
            this.Write_Byte5_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte5_Box.Location = new System.Drawing.Point(1219, 367);
            this.Write_Byte5_Box.Name = "Write_Byte5_Box";
            this.Write_Byte5_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte5_Box.TabIndex = 97;
            this.Write_Byte5_Box.Text = "80";
            // 
            // textBox39
            // 
            this.textBox39.Enabled = false;
            this.textBox39.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox39.Location = new System.Drawing.Point(1132, 367);
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new System.Drawing.Size(54, 21);
            this.textBox39.TabIndex = 96;
            this.textBox39.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox40
            // 
            this.textBox40.Enabled = false;
            this.textBox40.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox40.Location = new System.Drawing.Point(1062, 367);
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new System.Drawing.Size(54, 21);
            this.textBox40.TabIndex = 95;
            this.textBox40.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte17_Box
            // 
            this.Write_Byte17_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte17_Box.Location = new System.Drawing.Point(1299, 412);
            this.Write_Byte17_Box.Name = "Write_Byte17_Box";
            this.Write_Byte17_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte17_Box.TabIndex = 102;
            this.Write_Byte17_Box.Text = "C8";
            // 
            // Write_Byte6_Box
            // 
            this.Write_Byte6_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte6_Box.Location = new System.Drawing.Point(1219, 412);
            this.Write_Byte6_Box.Name = "Write_Byte6_Box";
            this.Write_Byte6_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte6_Box.TabIndex = 101;
            this.Write_Byte6_Box.Text = "00";
            // 
            // textBox43
            // 
            this.textBox43.Enabled = false;
            this.textBox43.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox43.Location = new System.Drawing.Point(1132, 412);
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new System.Drawing.Size(54, 21);
            this.textBox43.TabIndex = 100;
            this.textBox43.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox44
            // 
            this.textBox44.Enabled = false;
            this.textBox44.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox44.Location = new System.Drawing.Point(1062, 412);
            this.textBox44.Name = "textBox44";
            this.textBox44.Size = new System.Drawing.Size(54, 21);
            this.textBox44.TabIndex = 99;
            this.textBox44.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte18_Box
            // 
            this.Write_Byte18_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte18_Box.Location = new System.Drawing.Point(1299, 454);
            this.Write_Byte18_Box.Name = "Write_Byte18_Box";
            this.Write_Byte18_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte18_Box.TabIndex = 106;
            this.Write_Byte18_Box.Text = "24";
            // 
            // Write_Byte7_Box
            // 
            this.Write_Byte7_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte7_Box.Location = new System.Drawing.Point(1219, 454);
            this.Write_Byte7_Box.Name = "Write_Byte7_Box";
            this.Write_Byte7_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte7_Box.TabIndex = 105;
            this.Write_Byte7_Box.Text = "00";
            // 
            // textBox47
            // 
            this.textBox47.Enabled = false;
            this.textBox47.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox47.Location = new System.Drawing.Point(1132, 454);
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new System.Drawing.Size(54, 21);
            this.textBox47.TabIndex = 104;
            this.textBox47.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox48
            // 
            this.textBox48.Enabled = false;
            this.textBox48.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox48.Location = new System.Drawing.Point(1062, 454);
            this.textBox48.Name = "textBox48";
            this.textBox48.Size = new System.Drawing.Size(54, 21);
            this.textBox48.TabIndex = 103;
            this.textBox48.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte19_Box
            // 
            this.Write_Byte19_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte19_Box.Location = new System.Drawing.Point(1299, 497);
            this.Write_Byte19_Box.Name = "Write_Byte19_Box";
            this.Write_Byte19_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte19_Box.TabIndex = 110;
            this.Write_Byte19_Box.Text = "0C";
            // 
            // Write_Byte8_Box
            // 
            this.Write_Byte8_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte8_Box.Location = new System.Drawing.Point(1219, 497);
            this.Write_Byte8_Box.Name = "Write_Byte8_Box";
            this.Write_Byte8_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte8_Box.TabIndex = 109;
            this.Write_Byte8_Box.Text = "00";
            // 
            // textBox51
            // 
            this.textBox51.Enabled = false;
            this.textBox51.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox51.Location = new System.Drawing.Point(1132, 497);
            this.textBox51.Name = "textBox51";
            this.textBox51.Size = new System.Drawing.Size(54, 21);
            this.textBox51.TabIndex = 108;
            this.textBox51.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox52
            // 
            this.textBox52.Enabled = false;
            this.textBox52.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox52.Location = new System.Drawing.Point(1062, 497);
            this.textBox52.Name = "textBox52";
            this.textBox52.Size = new System.Drawing.Size(54, 21);
            this.textBox52.TabIndex = 107;
            this.textBox52.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte20_Box
            // 
            this.Write_Byte20_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte20_Box.Location = new System.Drawing.Point(1299, 538);
            this.Write_Byte20_Box.Name = "Write_Byte20_Box";
            this.Write_Byte20_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte20_Box.TabIndex = 114;
            this.Write_Byte20_Box.Text = "24";
            // 
            // Write_Byte9_Box
            // 
            this.Write_Byte9_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte9_Box.Location = new System.Drawing.Point(1219, 538);
            this.Write_Byte9_Box.Name = "Write_Byte9_Box";
            this.Write_Byte9_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte9_Box.TabIndex = 113;
            this.Write_Byte9_Box.Text = "00";
            // 
            // textBox55
            // 
            this.textBox55.Enabled = false;
            this.textBox55.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox55.Location = new System.Drawing.Point(1132, 538);
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new System.Drawing.Size(54, 21);
            this.textBox55.TabIndex = 112;
            this.textBox55.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox56
            // 
            this.textBox56.Enabled = false;
            this.textBox56.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox56.Location = new System.Drawing.Point(1062, 538);
            this.textBox56.Name = "textBox56";
            this.textBox56.Size = new System.Drawing.Size(54, 21);
            this.textBox56.TabIndex = 111;
            this.textBox56.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Write_Byte10_Box
            // 
            this.Write_Byte10_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte10_Box.Location = new System.Drawing.Point(1219, 580);
            this.Write_Byte10_Box.Name = "Write_Byte10_Box";
            this.Write_Byte10_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte10_Box.TabIndex = 117;
            this.Write_Byte10_Box.Text = "00";
            // 
            // textBox60
            // 
            this.textBox60.Enabled = false;
            this.textBox60.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox60.Location = new System.Drawing.Point(1062, 580);
            this.textBox60.Name = "textBox60";
            this.textBox60.Size = new System.Drawing.Size(54, 21);
            this.textBox60.TabIndex = 115;
            this.textBox60.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(1062, 139);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(40, 15);
            this.label21.TabIndex = 118;
            this.label21.Text = "Byte 0";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(1130, 139);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(46, 15);
            this.label22.TabIndex = 119;
            this.label22.Text = "Byte 11";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(1217, 138);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(40, 15);
            this.label23.TabIndex = 120;
            this.label23.Text = "Byte 0";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(1297, 138);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(46, 15);
            this.label24.TabIndex = 121;
            this.label24.Text = "Byte 11";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(1062, 181);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(40, 15);
            this.label25.TabIndex = 122;
            this.label25.Text = "Byte 1";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(1130, 181);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(47, 15);
            this.label26.TabIndex = 123;
            this.label26.Text = "Byte 12";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(1217, 181);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(40, 15);
            this.label27.TabIndex = 124;
            this.label27.Text = "Byte 1";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(1297, 180);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(47, 15);
            this.label28.TabIndex = 125;
            this.label28.Text = "Byte 12";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(1060, 223);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(40, 15);
            this.label29.TabIndex = 126;
            this.label29.Text = "Byte 2";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(1130, 223);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(47, 15);
            this.label30.TabIndex = 127;
            this.label30.Text = "Byte 13";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(1217, 223);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(40, 15);
            this.label31.TabIndex = 128;
            this.label31.Text = "Byte 2";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(1297, 223);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(47, 15);
            this.label32.TabIndex = 129;
            this.label32.Text = "Byte 13";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(1060, 265);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(40, 15);
            this.label33.TabIndex = 130;
            this.label33.Text = "Byte 3";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(1130, 266);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(47, 15);
            this.label34.TabIndex = 131;
            this.label34.Text = "Byte 14";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(1217, 266);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(40, 15);
            this.label35.TabIndex = 132;
            this.label35.Text = "Byte 3";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(1297, 265);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(47, 15);
            this.label36.TabIndex = 133;
            this.label36.Text = "Byte 14";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(1060, 306);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(40, 15);
            this.label37.TabIndex = 134;
            this.label37.Text = "Byte 4";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(1130, 307);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(39, 15);
            this.label38.TabIndex = 135;
            this.label38.Text = "Byte A";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(1217, 306);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(40, 15);
            this.label39.TabIndex = 136;
            this.label39.Text = "Byte 4";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(1297, 306);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(39, 15);
            this.label40.TabIndex = 137;
            this.label40.Text = "Byte A";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(1060, 349);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(40, 15);
            this.label41.TabIndex = 138;
            this.label41.Text = "Byte 5";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(1130, 349);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(41, 15);
            this.label42.TabIndex = 139;
            this.label42.Text = "Byte B";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(1217, 349);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(40, 15);
            this.label43.TabIndex = 140;
            this.label43.Text = "Byte 5";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(1297, 349);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(41, 15);
            this.label44.TabIndex = 141;
            this.label44.Text = "Byte B";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(1062, 394);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(40, 15);
            this.label45.TabIndex = 142;
            this.label45.Text = "Byte 6";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.Location = new System.Drawing.Point(1130, 394);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(42, 15);
            this.label46.TabIndex = 143;
            this.label46.Text = "Byte C";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(1217, 394);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(40, 15);
            this.label47.TabIndex = 144;
            this.label47.Text = "Byte 6";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(1297, 394);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(42, 15);
            this.label48.TabIndex = 145;
            this.label48.Text = "Byte C";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label49.Location = new System.Drawing.Point(1060, 436);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(40, 15);
            this.label49.TabIndex = 146;
            this.label49.Text = "Byte 7";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.Location = new System.Drawing.Point(1130, 437);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(42, 15);
            this.label50.TabIndex = 147;
            this.label50.Text = "Byte D";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(1217, 436);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(40, 15);
            this.label51.TabIndex = 148;
            this.label51.Text = "Byte 7";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(1297, 436);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(42, 15);
            this.label52.TabIndex = 149;
            this.label52.Text = "Byte D";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.Location = new System.Drawing.Point(1062, 479);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(40, 15);
            this.label53.TabIndex = 150;
            this.label53.Text = "Byte 8";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label54.Location = new System.Drawing.Point(1130, 479);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(41, 15);
            this.label54.TabIndex = 151;
            this.label54.Text = "Byte E";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(1217, 479);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(40, 15);
            this.label55.TabIndex = 152;
            this.label55.Text = "Byte 8";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(1297, 479);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(41, 15);
            this.label56.TabIndex = 153;
            this.label56.Text = "Byte E";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label57.Location = new System.Drawing.Point(1065, 521);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(40, 15);
            this.label57.TabIndex = 154;
            this.label57.Text = "Byte 9";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label58.Location = new System.Drawing.Point(1130, 521);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(40, 15);
            this.label58.TabIndex = 155;
            this.label58.Text = "Byte F";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(1217, 521);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(40, 15);
            this.label59.TabIndex = 156;
            this.label59.Text = "Byte 9";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(1297, 521);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(40, 15);
            this.label60.TabIndex = 157;
            this.label60.Text = "Byte F";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(1217, 562);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(47, 15);
            this.label61.TabIndex = 158;
            this.label61.Text = "Byte 10";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label62.Location = new System.Drawing.Point(1060, 562);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(47, 15);
            this.label62.TabIndex = 159;
            this.label62.Text = "Byte 10";
            // 
            // averageHeartRateLabelIndicator
            // 
            this.averageHeartRateLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.averageHeartRateLabelIndicator.Location = new System.Drawing.Point(1067, 9);
            this.averageHeartRateLabelIndicator.Name = "averageHeartRateLabelIndicator";
            this.averageHeartRateLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.averageHeartRateLabelIndicator.TabIndex = 16;
            this.averageHeartRateLabelIndicator.Text = "Average Heart Rate:";
            this.averageHeartRateLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.averageHeartRateLabelIndicator.Visible = false;
            // 
            // realtimeHeartRateLabel
            // 
            this.realtimeHeartRateLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.realtimeHeartRateLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.realtimeHeartRateLabel.Location = new System.Drawing.Point(1450, 7);
            this.realtimeHeartRateLabel.Name = "realtimeHeartRateLabel";
            this.realtimeHeartRateLabel.Size = new System.Drawing.Size(92, 24);
            this.realtimeHeartRateLabel.TabIndex = 5;
            this.realtimeHeartRateLabel.Text = "0";
            this.realtimeHeartRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.realtimeHeartRateLabel.Visible = false;
            // 
            // realtimeHeartRateLabelIndicator
            // 
            this.realtimeHeartRateLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(1273, 8);
            this.realtimeHeartRateLabelIndicator.Name = "realtimeHeartRateLabelIndicator";
            this.realtimeHeartRateLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.realtimeHeartRateLabelIndicator.TabIndex = 15;
            this.realtimeHeartRateLabelIndicator.Text = "Real Time HR (SDK):";
            this.realtimeHeartRateLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.realtimeHeartRateLabelIndicator.Visible = false;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label63.Location = new System.Drawing.Point(695, 47);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(69, 15);
            this.label63.TabIndex = 160;
            this.label63.Text = "HBR (bpm)";
            // 
            // HBR_Box
            // 
            this.HBR_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HBR_Box.Location = new System.Drawing.Point(683, 65);
            this.HBR_Box.Multiline = true;
            this.HBR_Box.Name = "HBR_Box";
            this.HBR_Box.Size = new System.Drawing.Size(85, 23);
            this.HBR_Box.TabIndex = 161;
            this.HBR_Box.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Interval_textBox
            // 
            this.Interval_textBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Interval_textBox.Location = new System.Drawing.Point(805, 66);
            this.Interval_textBox.Multiline = true;
            this.Interval_textBox.Name = "Interval_textBox";
            this.Interval_textBox.Size = new System.Drawing.Size(97, 23);
            this.Interval_textBox.TabIndex = 162;
            this.Interval_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label64.Location = new System.Drawing.Point(802, 46);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(103, 15);
            this.label64.TabIndex = 163;
            this.label64.Text = " R-R Interval (ms)";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label65.Location = new System.Drawing.Point(941, 46);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(91, 15);
            this.label65.TabIndex = 164;
            this.label65.Text = "   Singal Quality";
            // 
            // Signal_QualityBox
            // 
            this.Signal_QualityBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Signal_QualityBox.Location = new System.Drawing.Point(940, 67);
            this.Signal_QualityBox.Multiline = true;
            this.Signal_QualityBox.Name = "Signal_QualityBox";
            this.Signal_QualityBox.Size = new System.Drawing.Size(101, 23);
            this.Signal_QualityBox.TabIndex = 165;
            this.Signal_QualityBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label66.Location = new System.Drawing.Point(1474, 98);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(79, 14);
            this.label66.TabIndex = 166;
            this.label66.Text = "eFuse1 [39:32]";
            // 
            // Trim_ByteBox
            // 
            this.Trim_ByteBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Trim_ByteBox.Location = new System.Drawing.Point(1485, 115);
            this.Trim_ByteBox.Multiline = true;
            this.Trim_ByteBox.Name = "Trim_ByteBox";
            this.Trim_ByteBox.Size = new System.Drawing.Size(55, 23);
            this.Trim_ByteBox.TabIndex = 167;
            this.Trim_ByteBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button9.ForeColor = System.Drawing.Color.Black;
            this.button9.Location = new System.Drawing.Point(1461, 55);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(91, 27);
            this.button9.TabIndex = 168;
            this.button9.Text = "SPI/I2C Read ";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Visible = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Enabled = false;
            this.button10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button10.ForeColor = System.Drawing.Color.Black;
            this.button10.Location = new System.Drawing.Point(1240, 85);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(98, 27);
            this.button10.TabIndex = 169;
            this.button10.Text = "SPI/I2C Write";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // Sample_Rate300
            // 
            this.Sample_Rate300.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sample_Rate300.Location = new System.Drawing.Point(560, 55);
            this.Sample_Rate300.Name = "Sample_Rate300";
            this.Sample_Rate300.Size = new System.Drawing.Size(113, 33);
            this.Sample_Rate300.TabIndex = 170;
            this.Sample_Rate300.Text = "Sampling Rate 300";
            this.Sample_Rate300.UseVisualStyleBackColor = true;
            this.Sample_Rate300.Visible = false;
            this.Sample_Rate300.Click += new System.EventHandler(this.button8_Click);
            // 
            // Sample_Rate600
            // 
            this.Sample_Rate600.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sample_Rate600.Location = new System.Drawing.Point(560, 56);
            this.Sample_Rate600.Name = "Sample_Rate600";
            this.Sample_Rate600.Size = new System.Drawing.Size(113, 32);
            this.Sample_Rate600.TabIndex = 171;
            this.Sample_Rate600.Text = "Sampling Rate  600";
            this.Sample_Rate600.UseVisualStyleBackColor = true;
            this.Sample_Rate600.Visible = false;
            this.Sample_Rate600.Click += new System.EventHandler(this.button11_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(22, 358);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 28);
            this.button1.TabIndex = 172;
            this.button1.Text = "UART Read";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(522, 358);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 28);
            this.button2.TabIndex = 173;
            this.button2.Text = "UART Write";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // Write_Byte1_Box
            // 
            this.Write_Byte1_Box.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Write_Byte1_Box.Location = new System.Drawing.Point(1219, 198);
            this.Write_Byte1_Box.Name = "Write_Byte1_Box";
            this.Write_Byte1_Box.Size = new System.Drawing.Size(54, 21);
            this.Write_Byte1_Box.TabIndex = 81;
            this.Write_Byte1_Box.Text = "00";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(1048, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(16, 25);
            this.button3.TabIndex = 174;
            this.button3.Text = "R";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.TrimByte_Read_button_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.Red;
            this.button4.Location = new System.Drawing.Point(1405, 156);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(134, 27);
            this.button4.TabIndex = 218;
            this.button4.Text = "Program eFuse";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.ForeColor = System.Drawing.Color.DarkCyan;
            this.label72.Location = new System.Drawing.Point(1483, 507);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(44, 15);
            this.label72.TabIndex = 212;
            this.label72.Text = "[63:56]";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.ForeColor = System.Drawing.Color.Navy;
            this.label73.Location = new System.Drawing.Point(1403, 507);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(44, 15);
            this.label73.TabIndex = 211;
            this.label73.Text = "[63:56]";
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.ForeColor = System.Drawing.Color.DarkCyan;
            this.label74.Location = new System.Drawing.Point(1483, 465);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(44, 15);
            this.label74.TabIndex = 210;
            this.label74.Text = "[55:48]";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.ForeColor = System.Drawing.Color.Navy;
            this.label75.Location = new System.Drawing.Point(1403, 465);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(44, 15);
            this.label75.TabIndex = 209;
            this.label75.Text = "[55:48]";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.ForeColor = System.Drawing.Color.DarkCyan;
            this.label76.Location = new System.Drawing.Point(1483, 420);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(44, 15);
            this.label76.TabIndex = 208;
            this.label76.Text = "[47:40]";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.ForeColor = System.Drawing.Color.Navy;
            this.label77.Location = new System.Drawing.Point(1403, 420);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(44, 15);
            this.label77.TabIndex = 207;
            this.label77.Text = "[47:40]";
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.ForeColor = System.Drawing.Color.DarkCyan;
            this.label78.Location = new System.Drawing.Point(1483, 377);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(44, 15);
            this.label78.TabIndex = 206;
            this.label78.Text = "[39:32]";
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.ForeColor = System.Drawing.Color.Navy;
            this.label79.Location = new System.Drawing.Point(1403, 377);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(44, 15);
            this.label79.TabIndex = 205;
            this.label79.Text = "[39:32]";
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.ForeColor = System.Drawing.Color.DarkCyan;
            this.label80.Location = new System.Drawing.Point(1483, 336);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(44, 15);
            this.label80.TabIndex = 204;
            this.label80.Text = "[31:24]";
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.ForeColor = System.Drawing.Color.Navy;
            this.label81.Location = new System.Drawing.Point(1403, 337);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(44, 15);
            this.label81.TabIndex = 203;
            this.label81.Text = "[31:24]";
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.ForeColor = System.Drawing.Color.DarkCyan;
            this.label82.Location = new System.Drawing.Point(1483, 294);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(44, 15);
            this.label82.TabIndex = 202;
            this.label82.Text = "[23:16]";
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.ForeColor = System.Drawing.Color.Navy;
            this.label83.Location = new System.Drawing.Point(1403, 294);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(44, 15);
            this.label83.TabIndex = 201;
            this.label83.Text = "[23:16]";
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.ForeColor = System.Drawing.Color.DarkCyan;
            this.label84.Location = new System.Drawing.Point(1483, 251);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(37, 15);
            this.label84.TabIndex = 200;
            this.label84.Text = "[15:8]";
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.ForeColor = System.Drawing.Color.Navy;
            this.label85.Location = new System.Drawing.Point(1403, 252);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(37, 15);
            this.label85.TabIndex = 199;
            this.label85.Text = "[15:8]";
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.ForeColor = System.Drawing.Color.DarkCyan;
            this.label86.Location = new System.Drawing.Point(1483, 209);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(30, 15);
            this.label86.TabIndex = 198;
            this.label86.Text = "[7:0]";
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.ForeColor = System.Drawing.Color.Navy;
            this.label87.Location = new System.Drawing.Point(1403, 209);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(30, 15);
            this.label87.TabIndex = 197;
            this.label87.Text = "[7:0]";
            // 
            // textBox26
            // 
            this.textBox26.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox26.ForeColor = System.Drawing.Color.Black;
            this.textBox26.Location = new System.Drawing.Point(1485, 525);
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new System.Drawing.Size(54, 21);
            this.textBox26.TabIndex = 191;
            this.textBox26.Text = "E4";
            // 
            // textBox29
            // 
            this.textBox29.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox29.ForeColor = System.Drawing.Color.Black;
            this.textBox29.Location = new System.Drawing.Point(1405, 525);
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new System.Drawing.Size(54, 21);
            this.textBox29.TabIndex = 190;
            this.textBox29.Text = "00";
            // 
            // textBox30
            // 
            this.textBox30.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox30.ForeColor = System.Drawing.Color.Black;
            this.textBox30.Location = new System.Drawing.Point(1485, 483);
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new System.Drawing.Size(54, 21);
            this.textBox30.TabIndex = 189;
            this.textBox30.Text = "90";
            // 
            // textBox33
            // 
            this.textBox33.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox33.ForeColor = System.Drawing.Color.Black;
            this.textBox33.Location = new System.Drawing.Point(1405, 483);
            this.textBox33.Name = "textBox33";
            this.textBox33.Size = new System.Drawing.Size(54, 21);
            this.textBox33.TabIndex = 188;
            this.textBox33.Text = "E0";
            // 
            // textBox34
            // 
            this.textBox34.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox34.ForeColor = System.Drawing.Color.Black;
            this.textBox34.Location = new System.Drawing.Point(1485, 438);
            this.textBox34.Name = "textBox34";
            this.textBox34.Size = new System.Drawing.Size(54, 21);
            this.textBox34.TabIndex = 187;
            this.textBox34.Text = "24";
            // 
            // textBox37
            // 
            this.textBox37.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox37.ForeColor = System.Drawing.Color.Black;
            this.textBox37.Location = new System.Drawing.Point(1405, 438);
            this.textBox37.Name = "textBox37";
            this.textBox37.Size = new System.Drawing.Size(54, 21);
            this.textBox37.TabIndex = 186;
            this.textBox37.Text = "43";
            // 
            // textBox38
            // 
            this.textBox38.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox38.ForeColor = System.Drawing.Color.Black;
            this.textBox38.Location = new System.Drawing.Point(1485, 395);
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new System.Drawing.Size(54, 21);
            this.textBox38.TabIndex = 185;
            this.textBox38.Text = "C0";
            // 
            // textBox41
            // 
            this.textBox41.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox41.ForeColor = System.Drawing.Color.Black;
            this.textBox41.Location = new System.Drawing.Point(1405, 395);
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new System.Drawing.Size(54, 21);
            this.textBox41.TabIndex = 184;
            // 
            // textBox42
            // 
            this.textBox42.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox42.ForeColor = System.Drawing.Color.Black;
            this.textBox42.Location = new System.Drawing.Point(1485, 354);
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new System.Drawing.Size(54, 21);
            this.textBox42.TabIndex = 183;
            this.textBox42.Text = "D2";
            // 
            // textBox45
            // 
            this.textBox45.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox45.ForeColor = System.Drawing.Color.Black;
            this.textBox45.Location = new System.Drawing.Point(1405, 354);
            this.textBox45.Name = "textBox45";
            this.textBox45.Size = new System.Drawing.Size(54, 21);
            this.textBox45.TabIndex = 182;
            this.textBox45.Text = "0E";
            // 
            // textBox46
            // 
            this.textBox46.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox46.ForeColor = System.Drawing.Color.Black;
            this.textBox46.Location = new System.Drawing.Point(1485, 312);
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new System.Drawing.Size(54, 21);
            this.textBox46.TabIndex = 181;
            this.textBox46.Text = "80";
            // 
            // textBox49
            // 
            this.textBox49.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox49.ForeColor = System.Drawing.Color.Black;
            this.textBox49.Location = new System.Drawing.Point(1405, 312);
            this.textBox49.Name = "textBox49";
            this.textBox49.Size = new System.Drawing.Size(54, 21);
            this.textBox49.TabIndex = 180;
            // 
            // textBox50
            // 
            this.textBox50.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox50.ForeColor = System.Drawing.Color.Black;
            this.textBox50.Location = new System.Drawing.Point(1485, 269);
            this.textBox50.Name = "textBox50";
            this.textBox50.Size = new System.Drawing.Size(54, 21);
            this.textBox50.TabIndex = 179;
            this.textBox50.Text = "93";
            // 
            // textBox53
            // 
            this.textBox53.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox53.ForeColor = System.Drawing.Color.Black;
            this.textBox53.Location = new System.Drawing.Point(1405, 269);
            this.textBox53.Name = "textBox53";
            this.textBox53.Size = new System.Drawing.Size(54, 21);
            this.textBox53.TabIndex = 178;
            this.textBox53.Text = "FF";
            // 
            // textBox54
            // 
            this.textBox54.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox54.ForeColor = System.Drawing.Color.Black;
            this.textBox54.Location = new System.Drawing.Point(1485, 227);
            this.textBox54.Name = "textBox54";
            this.textBox54.Size = new System.Drawing.Size(54, 21);
            this.textBox54.TabIndex = 177;
            this.textBox54.Text = "FF";
            // 
            // textBox57
            // 
            this.textBox57.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox57.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox57.ForeColor = System.Drawing.Color.Black;
            this.textBox57.Location = new System.Drawing.Point(1405, 227);
            this.textBox57.Name = "textBox57";
            this.textBox57.Size = new System.Drawing.Size(54, 21);
            this.textBox57.TabIndex = 176;
            this.textBox57.Text = "FF";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label67.ForeColor = System.Drawing.Color.Navy;
            this.label67.Location = new System.Drawing.Point(1406, 188);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(52, 15);
            this.label67.TabIndex = 219;
            this.label67.Text = "eFuse 1";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label68.ForeColor = System.Drawing.Color.DarkCyan;
            this.label68.Location = new System.Drawing.Point(1486, 188);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(52, 15);
            this.label68.TabIndex = 220;
            this.label68.Text = "eFuse 2";
            // 
            // textBox19
            // 
            this.textBox19.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox19.Location = new System.Drawing.Point(1401, 115);
            this.textBox19.Multiline = true;
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(55, 23);
            this.textBox19.TabIndex = 222;
            this.textBox19.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label69.Location = new System.Drawing.Point(1389, 98);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(79, 14);
            this.label69.TabIndex = 221;
            this.label69.Text = "eFuse1 [21:17]";
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label70.ForeColor = System.Drawing.Color.Navy;
            this.label70.Location = new System.Drawing.Point(613, 397);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(209, 14);
            this.label70.TabIndex = 223;
            this.label70.Text = "Note: conOut3 / Byte5 ==>eFuse1 [39:32] ";
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label71.ForeColor = System.Drawing.Color.Navy;
            this.label71.Location = new System.Drawing.Point(643, 415);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(223, 14);
            this.label71.TabIndex = 224;
            this.label71.Text = "conOut4[4:0] / Byte1[4:0] ==>eFuse1 [21:17] ";
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(422, 389);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(80, 36);
            this.button5.TabIndex = 225;
            this.button5.Text = "Clear \r\nUART Read";
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(1122, 569);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(89, 36);
            this.button6.TabIndex = 226;
            this.button6.Text = "Clear \r\nSPI/I2C Read";
            this.button6.Click += new System.EventHandler(this.button6_Click_1);
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label88.Location = new System.Drawing.Point(29, 162);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(61, 15);
            this.label88.TabIndex = 227;
            this.label88.Text = "(Unit: mV)";
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label89.Location = new System.Drawing.Point(29, 249);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(50, 15);
            this.label89.TabIndex = 228;
            this.label89.Text = "(Unit: s)";
            // 
            // button14
            // 
            this.button14.Enabled = false;
            this.button14.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button14.ForeColor = System.Drawing.Color.Black;
            this.button14.Location = new System.Drawing.Point(1233, 63);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(40, 21);
            this.button14.TabIndex = 233;
            this.button14.Text = "125K";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button8
            // 
            this.button8.Enabled = false;
            this.button8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.ForeColor = System.Drawing.Color.Black;
            this.button8.Location = new System.Drawing.Point(1279, 63);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(40, 21);
            this.button8.TabIndex = 234;
            this.button8.Text = "250K";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click_1);
            // 
            // button11
            // 
            this.button11.Enabled = false;
            this.button11.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button11.ForeColor = System.Drawing.Color.Black;
            this.button11.Location = new System.Drawing.Point(1325, 63);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(40, 21);
            this.button11.TabIndex = 235;
            this.button11.Text = "500K";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click_1);
            // 
            // button13
            // 
            this.button13.Enabled = false;
            this.button13.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button13.ForeColor = System.Drawing.Color.Black;
            this.button13.Location = new System.Drawing.Point(1371, 63);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(40, 21);
            this.button13.TabIndex = 237;
            this.button13.Text = "1M";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button12
            // 
            this.button12.Enabled = false;
            this.button12.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button12.ForeColor = System.Drawing.Color.Black;
            this.button12.Location = new System.Drawing.Point(1178, 63);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(49, 21);
            this.button12.TabIndex = 238;
            this.button12.Text = "62.5K";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button15
            // 
            this.button15.Enabled = false;
            this.button15.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button15.ForeColor = System.Drawing.Color.Black;
            this.button15.Location = new System.Drawing.Point(1250, 38);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(49, 21);
            this.button15.TabIndex = 239;
            this.button15.Text = "100K";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button16
            // 
            this.button16.Enabled = false;
            this.button16.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button16.ForeColor = System.Drawing.Color.Black;
            this.button16.Location = new System.Drawing.Point(1305, 38);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(49, 21);
            this.button16.TabIndex = 240;
            this.button16.Text = "400K";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.ForeColor = System.Drawing.Color.Navy;
            this.label90.Location = new System.Drawing.Point(1403, 561);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(110, 30);
            this.label90.TabIndex = 241;
            this.label90.Text = "Note: [23:22] == 10\r\n          [16] == 1";
            // 
            // graphPanel1
            // 
            this.graphPanel1.Location = new System.Drawing.Point(1, 431);
            this.graphPanel1.Name = "graphPanel1";
            this.graphPanel1.samplingRate = 10;
            this.graphPanel1.Size = new System.Drawing.Size(1040, 227);
            this.graphPanel1.TabIndex = 242;
            this.graphPanel1.xAxisMax = 0D;
            this.graphPanel1.xAxisMin = 0D;
            this.graphPanel1.yAxisMax = 0D;
            this.graphPanel1.yAxisMin = 0D;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1362, 669);
            this.Controls.Add(this.label90);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.label89);
            this.Controls.Add(this.label88);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label71);
            this.Controls.Add(this.label70);
            this.Controls.Add(this.textBox19);
            this.Controls.Add(this.label69);
            this.Controls.Add(this.label68);
            this.Controls.Add(this.label67);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label72);
            this.Controls.Add(this.label73);
            this.Controls.Add(this.label74);
            this.Controls.Add(this.label75);
            this.Controls.Add(this.label76);
            this.Controls.Add(this.label77);
            this.Controls.Add(this.label78);
            this.Controls.Add(this.label79);
            this.Controls.Add(this.label80);
            this.Controls.Add(this.label81);
            this.Controls.Add(this.label82);
            this.Controls.Add(this.label83);
            this.Controls.Add(this.label84);
            this.Controls.Add(this.label85);
            this.Controls.Add(this.label86);
            this.Controls.Add(this.label87);
            this.Controls.Add(this.textBox26);
            this.Controls.Add(this.textBox29);
            this.Controls.Add(this.textBox30);
            this.Controls.Add(this.textBox33);
            this.Controls.Add(this.textBox34);
            this.Controls.Add(this.textBox37);
            this.Controls.Add(this.textBox38);
            this.Controls.Add(this.textBox41);
            this.Controls.Add(this.textBox42);
            this.Controls.Add(this.textBox45);
            this.Controls.Add(this.textBox46);
            this.Controls.Add(this.textBox49);
            this.Controls.Add(this.textBox50);
            this.Controls.Add(this.textBox53);
            this.Controls.Add(this.textBox54);
            this.Controls.Add(this.textBox57);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Sample_Rate600);
            this.Controls.Add(this.Sample_Rate300);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.Trim_ByteBox);
            this.Controls.Add(this.label66);
            this.Controls.Add(this.Signal_QualityBox);
            this.Controls.Add(this.label65);
            this.Controls.Add(this.label64);
            this.Controls.Add(this.Interval_textBox);
            this.Controls.Add(this.HBR_Box);
            this.Controls.Add(this.label63);
            this.Controls.Add(this.label62);
            this.Controls.Add(this.label61);
            this.Controls.Add(this.label60);
            this.Controls.Add(this.label59);
            this.Controls.Add(this.label58);
            this.Controls.Add(this.label57);
            this.Controls.Add(this.label56);
            this.Controls.Add(this.label55);
            this.Controls.Add(this.label54);
            this.Controls.Add(this.label53);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.label51);
            this.Controls.Add(this.label50);
            this.Controls.Add(this.label49);
            this.Controls.Add(this.label48);
            this.Controls.Add(this.label47);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.label45);
            this.Controls.Add(this.label44);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.Write_Byte10_Box);
            this.Controls.Add(this.textBox60);
            this.Controls.Add(this.Write_Byte20_Box);
            this.Controls.Add(this.Write_Byte9_Box);
            this.Controls.Add(this.textBox55);
            this.Controls.Add(this.textBox56);
            this.Controls.Add(this.Write_Byte19_Box);
            this.Controls.Add(this.Write_Byte8_Box);
            this.Controls.Add(this.textBox51);
            this.Controls.Add(this.textBox52);
            this.Controls.Add(this.Write_Byte18_Box);
            this.Controls.Add(this.Write_Byte7_Box);
            this.Controls.Add(this.textBox47);
            this.Controls.Add(this.textBox48);
            this.Controls.Add(this.Write_Byte17_Box);
            this.Controls.Add(this.Write_Byte6_Box);
            this.Controls.Add(this.textBox43);
            this.Controls.Add(this.textBox44);
            this.Controls.Add(this.Write_Byte16_Box);
            this.Controls.Add(this.Write_Byte5_Box);
            this.Controls.Add(this.textBox39);
            this.Controls.Add(this.textBox40);
            this.Controls.Add(this.Write_Byte15_Box);
            this.Controls.Add(this.Write_Byte4_Box);
            this.Controls.Add(this.textBox35);
            this.Controls.Add(this.textBox36);
            this.Controls.Add(this.Write_Byte14_Box);
            this.Controls.Add(this.Write_Byte3_Box);
            this.Controls.Add(this.textBox31);
            this.Controls.Add(this.textBox32);
            this.Controls.Add(this.Write_Byte13_Box);
            this.Controls.Add(this.Write_Byte2_Box);
            this.Controls.Add(this.textBox27);
            this.Controls.Add(this.textBox28);
            this.Controls.Add(this.Write_Byte12_Box);
            this.Controls.Add(this.Write_Byte1_Box);
            this.Controls.Add(this.textBox23);
            this.Controls.Add(this.textBox24);
            this.Controls.Add(this.Write_Byte11_Box);
            this.Controls.Add(this.Write_Byte0_Box);
            this.Controls.Add(this.textBox18);
            this.Controls.Add(this.textBox17);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox16);
            this.Controls.Add(this.textBox15);
            this.Controls.Add(this.textBox14);
            this.Controls.Add(this.textBox13);
            this.Controls.Add(this.textBox12);
            this.Controls.Add(this.textBox11);
            this.Controls.Add(this.textBox10);
            this.Controls.Add(this.textBox9);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.I2C_Open_Button);
            this.Controls.Add(this.I2C_Close_Button);
            this.Controls.Add(this.SPI_Open_Button);
            this.Controls.Add(this.SPI_Close_Button);
            this.Controls.Add(this.UART_Open_Button);
            this.Controls.Add(this.UART_Close_Button);
            this.Controls.Add(this.stopReplay);
            this.Controls.Add(this.Replay);
            this.Controls.Add(this.HRVLabelIndicator);
            this.Controls.Add(this.HRVLabel);
            this.Controls.Add(this.energyPictureBox);
            this.Controls.Add(this.fatigueLabelIndicator);
            this.Controls.Add(this.fatigueLabel);
            this.Controls.Add(this.soundCheckBox);
            this.Controls.Add(this.averageHeartRateLabelIndicator);
            this.Controls.Add(this.realtimeHeartRateLabelIndicator);
            this.Controls.Add(this.averageHeartRateLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.recordButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.realtimeHeartRateLabel);
            this.Controls.Add(this.rawGraphPanel);
            this.Controls.Add(this.respirationRateLabel);
            this.Controls.Add(this.respirationRateIndicator);
            this.Controls.Add(this.heartAgeLabel);
            this.Controls.Add(this.heartAgeIndicator);
            this.Controls.Add(this.graphPanel1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BMD200 Test Software x64";
            ((System.ComponentModel.ISupportInitialize)(this.energyPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void portText_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the key pressed was "Enter"
            if (e.KeyChar == (char)13)
            {
                //suppress the beep sounds by setting e.Handled = true
                e.Handled = true;
                connectButton_Click(sender, e);
            }
        }

        /*Connect Button Clicked*/
        private void connectButton_Click(object sender, System.EventArgs e)
        {
#if true
            this.connectButton.Enabled = false;
            this.portText.Enabled = false;

            rawGraphPanel.LineGraph.Clear();

            ConnectButtonClicked(this, EventArgs.Empty);
#else
            double tempData = 0;

            tempData = Math.Sin(2*Math.PI*1*i/rawGraphPanel.LineGraph.samplingRate);
            DataPair tempDataPair;
            tempDataPair.timeStamp = (float)i/rawGraphPanel.LineGraph.samplingRate;
            tempDataPair.data      = (float)tempData;


            rawGraphPanel.LineGraph.Add( tempDataPair );

            i++;

            rawGraphPanel.LineGraph.Invalidate();
#endif
        }

        //disconnect button clicked
        private void disconnect_Click(object sender, System.EventArgs e)
        {
            this.respirationRateIndicator.Text = "0";
            this.heartAgeIndicator.Text = "0";
            this.fatigueLabelIndicator.Visible = false;
            this.fatigueLabel.Text = "";
            DisconnectButtonClicked(this, EventArgs.Empty);

            rawGraphPanel.LineGraph.Clear();            
            timeStampIndex = 0;
            rawGraphPanel.LineGraph.Invalidate();

            ///Clear the FFT drawing
            graphPanel1.BarGraph.Clear();
            graphPanel1.BarGraph.Invalidate();

            HBR_Box.Text = "";
            Interval_textBox.Text = "";
            Signal_QualityBox.Text = "";
        }

        //if there has been an R peak, play a "beep"
        public void playBeep()
        {
            if (soundCheckBox.Checked)
            {
                player.Play();
            }
        }

        /*Clear Button Clicked*/
        private void clearButton_Click(object sender, System.EventArgs e)
        {
            rawGraphPanel.LineGraph.Clear();

            timeStampIndex = 0;

            rawGraphPanel.LineGraph.Invalidate();

            HBR_Box.Text = "";
            Interval_textBox.Text = "";
            Signal_QualityBox.Text = "";
        }

        //fatigue button clicked. set up stuff
        private void fatigueButton_Click(object sender, EventArgs e)
        {
            updateFatigueLevelLabel("Calculating...");
            toggleFatigueLevelLabel(true);
            toggleFatigueLevelLabelIndicator(true);
            toggleRecordButton(false);
            toggleEnergyPictureBox(false);

            fatigueResult = 0;

            //reset the meter
            fatigueResult = relaxationLevel.addInterval(0, 0);

            //disable the button
            toggleFatigueStartButton(false);
            toggleFatigueStopButton(true);

            //update the status bar
            updateStatusLabel("Recording...please hold position for approximately 1 minute...");

            //start the fatigue meter
            runFatigueMeter = true;
        }
        
        //stop fatigue button clicked
        private void stopFatigueMeter_Click(object sender, EventArgs e)
        {
            runFatigueMeter = false;
            //reset the meter
            fatigueResult = relaxationLevel.addInterval(0, 0);
            outputFatigueResults(fatigueResult);
        }

        //calculate the fatigue value based on RR interval
        public void calculateFatigue(int RRvalue)
        {
            //if the Fatigue Meter button has been pressed
            if (runFatigueMeter)
            {
                //if the energy level is less than 1
                if (fatigueResult < 1)
                {
                    fatigueResult = relaxationLevel.addInterval((int)((RRvalue * 1000.0) / 512.0), (byte)poorQuality);
                }
                else
                {
                    //else energy level is now greater than 0. or, the user has pressed the stop button. write it out to a text file
                    runFatigueMeter = false;
                    outputFatigueResults(fatigueResult);
                }

            }
        }

        //save the output of the fatigue meter
        public void outputFatigueResults(int fatigueLevel)
        {
            if (fatigueLevel > 0)
            {    //if we actually got a fatigue level
                updateFatigueLevelLabel(fatigueLevel.ToString());
                updateStatusLabel("Energy recording complete.");

                if (fatigueLevel < 25)
                {
                    setEnergyPictureBox(emptyImage);
                }
                else if (fatigueLevel < 50)
                {
                    setEnergyPictureBox(lowImage);
                }
                else if (fatigueLevel < 75)
                {
                    setEnergyPictureBox(mediumImage);
                }
                else if (fatigueLevel <= 100)
                {
                    setEnergyPictureBox(fullImage);
                }
                toggleEnergyPictureBox(false);

            }
            else
            {    //fatigue result = -1, because the user stopped it early
                updateFatigueLevelLabel("");
                toggleFatigueLevelLabelIndicator(true);
                toggleFatigueLevelLabel(true);
                updateStatusLabel("Data recording ended early. Please try recording again.");
            }

            //stop the fatigue meter
            runFatigueMeter = false;

            //re-enable the button
            toggleFatigueStartButton(true);
            toggleFatigueStopButton(false);
            toggleRecordButton(true);

        }

        /*Record Button Clicked*/
        private void recordButton_Click(object sender, System.EventArgs e)
        {
            recordButton.Enabled = false;
            recordButton.Visible = false;
            fatigueStartButton(false);

            updateStatusLabel("Recording...");

            //save linegraph data in a seperate file
            //rawGraphPanel.LineGraph.SaveDataFlag = false;
            //rawGraphPanel.LineGraph.RecordDataFlag = true;

            /*Clear Block*/
            //rawGraphPanel.LineGraph.Clear();
            //rawGraphPanel.LineGraph.timeStampIndex = 0;

            stopButton.Enabled = true;
            stopButton.Visible = true;

            // Create new file and save to the "Data" folder
            dataLogOutFile = "dataLog-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
            + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt";
            dataLogOutFile = Path.Combine(currentPath, dataLogOutFile);

            ECGLogOutFile = "ECGLog-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
            + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt";
            ECGLogOutFile = Path.Combine(currentPath, ECGLogOutFile);

            // Create new filestream, appendable. write the headers
            this.dataLogStream = new System.IO.StreamWriter(dataLogOutFile, true);
            this.ECGLogStream = new System.IO.StreamWriter(ECGLogOutFile, true);

            if (dataLogStream != null)
            {
                this.dataLogStream.WriteLine("timestamp: CODE VALUEBYTE(S)");
            }

            if (ECGLogStream != null)
            {
                this.ECGLogStream.WriteLine("timestamp: ADC");
            }

            recordFlag = true;
        }

        //stop button clicked
        private void stopButton_Click(object sender, System.EventArgs e)
        {
            rawGraphPanel.LineGraph.SaveDataFlag = true;
            rawGraphPanel.LineGraph.RecordDataFlag = false;
            updateStatusLabel("Recording complete.");
            recordFlag = false;
            stopButton.Enabled = false;
            stopButton.Visible = false;
            fatigueStartButton(true);
            recordButton.Enabled = true;
            recordButton.Visible = true;
            RecordStopTime = DateTime.Now;
            dataLogStream.Close();
            ECGLogStream.Close();
            saveFileGUI.updateDataLogTextBox(Path.GetFileName(dataLogOutFile));
            saveFileGUI.updateECGLogTextBox(Path.GetFileName(ECGLogOutFile));
            saveFileGUI.updatefolderPathTextBox(Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString());
            saveFileGUI.Location = new Point((this.Width - saveFileGUI.Width) / 2 + this.Location.X, (this.Height - saveFileGUI.Height) / 2 + this.Location.Y);
            saveFileGUI.Show();
        }

        //when the save button is clicked in the save dialog
        void OnSaveButtonClicked(object sender, EventArgs e)
        {

            try
            {
                if (!System.IO.Directory.Exists(saveFileGUI.folderPathTextBox.Text))
                {
                    System.IO.Directory.CreateDirectory(saveFileGUI.folderPathTextBox.Text);
                }

                System.IO.File.Copy(dataLogOutFile, System.IO.Path.Combine(saveFileGUI.folderPathTextBox.Text, saveFileGUI.dataLogTextBox.Text), true);
                System.IO.File.Copy(ECGLogOutFile, System.IO.Path.Combine(saveFileGUI.folderPathTextBox.Text, saveFileGUI.sleepLogTextBox.Text), true);

                System.IO.File.Delete(dataLogOutFile);
                System.IO.File.Delete(ECGLogOutFile);

                saveFileGUI.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("To save data in this directory, please exit the application and run as Administrator.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

        }

        //when the browse button is clicked in the save dialog
        void OnBrowseButtonClicked(object sender, EventArgs e)
        {

            //show the folder browser box
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                saveFileGUI.updatefolderPathTextBox(folderBrowserDialog.SelectedPath);
            }
        }

        //when the browse button is clicked in the save dialog
        void OnDiscardButtonClicked(object sender, EventArgs e)
        {

            System.IO.File.Delete(System.IO.Path.Combine(currentPath, dataLogOutFile));
            System.IO.File.Delete(System.IO.Path.Combine(currentPath, ECGLogOutFile));

            saveFileGUI.Hide();
        }

        public void updateAverageHeartBeatValue(double heartBeatValue)
        {
            //if it's a good signal
            if (poorQuality == 200)
            {
                //if the buffer isnt full yet, add to the buffer. return the average heartbeat value based on however
                //much data is in the buffer currently
                if (avgHBCounter < avgHBBufferLength)
                {
                    avgHBValueBuffer[avgHBCounter] = heartBeatValue;
                    avgHBCounter++;

                    avgHBValue = 0;
                    for (int i = 0; i < avgHBCounter; i++)
                    {
                        avgHBValue += avgHBValueBuffer[i];
                    }
                    avgHBValue = avgHBValue / avgHBCounter;

                    //update the label
                    updateAverageHeartRateLabel(Math.Round(avgHBValue).ToString() + " bpm");
                }
                //else shift the buffer and add the most recent value to the end, calculate the average, and return the average value
                else
                {
                    //set the counter to the length of the index, to make sure that it doesn't become huge after looping so many times
                    avgHBCounter = avgHBBufferLength;

                    //shift the buffer
                    for (int i = 0; i < avgHBBufferLength - 1; i++)
                    {
                        avgHBValueBuffer[i] = avgHBValueBuffer[i + 1];
                    }
                    avgHBValueBuffer[avgHBBufferLength - 1] = heartBeatValue;

                    //calculate the average
                    //reset
                    avgHBValue = 0;
                    for (int i = 0; i < avgHBBufferLength; i++)
                    {
                        avgHBValue += (avgHBValueBuffer[i]);
                    }
                    avgHBValue = (avgHBValue / avgHBBufferLength);

                    //display the average heartbeat (rounded to nearest int)
                    updateAverageHeartRateLabel(Math.Round(avgHBValue).ToString() + " bpm");
                }
            }
            //else poor signal
            else
            {
                //set the counter back to zero. when its good signal again, it will refill the 30 seconds of new data before outputting average value
                avgHBCounter = 0;

                //for now, set the avgHBValue to zero
                avgHBValue = 0;

                //since poor signal, display 0 as average heartbeat
                updateAverageHeartRateLabel("0");
            }
        }

        public void updateRealTimeHeartBeatValue(double heartBeatValue)
        {
            //if it's a good signal
            if (poorQuality == 0)
            {
                //if the buffer isnt full yet, add to the buffer. return the average heartbeat value based on however
                //much data is in the buffer currently
                if (realTimeHBCounter < realTimeHBBufferLength)
                {
                    realTimeHBValueBuffer[realTimeHBCounter] = heartBeatValue;
                    realTimeHBCounter++;

                    realTimeHBValue = 0;
                    for (int i = 0; i < realTimeHBCounter; i++)
                    {
                        realTimeHBValue += realTimeHBValueBuffer[i];
                    }
                    realTimeHBValue = realTimeHBValue / realTimeHBCounter;

                    //update the label
                    updateRealTimeHeartRateLabel(Math.Round(realTimeHBValue).ToString() + " bpm");
                }
                //else shift the buffer and add the most recent value to the end, calculate the average, and return the average value
                else
                {
                    //set the counter to the length of the index, to make sure that it doesn't become huge after looping so many times
                    realTimeHBCounter = realTimeHBBufferLength;

                    //shift the buffer
                    for (int i = 0; i < realTimeHBBufferLength - 1; i++)
                    {
                        realTimeHBValueBuffer[i] = realTimeHBValueBuffer[i + 1];
                    }
                    realTimeHBValueBuffer[realTimeHBBufferLength - 1] = heartBeatValue;

                    //calculate the average
                    //reset
                    realTimeHBValue = 0;
                    for (int i = 0; i < realTimeHBBufferLength; i++)
                    {
                        realTimeHBValue += (realTimeHBValueBuffer[i]);
                    }
                    realTimeHBValue = (realTimeHBValue / realTimeHBBufferLength);

                    //display the average heartbeat (rounded to nearest int)
                    updateRealTimeHeartRateLabel(Math.Round(realTimeHBValue).ToString() + " bpm");
                }
            }
            //else poor signal
            else
            {
                //set the counter back to zero. when its good signal again, it will refill the 30 seconds of new data before outputting average value
                realTimeHBCounter = 0;

                //for now, set the avgHBValue to zero
                realTimeHBValue = 0;

                //since poor signal, display 0 as average heartbeat
                updateRealTimeHeartRateLabel("0");

            }
        }

        /**
        * Record out the received data to a dataLog file
        * Record the RAW ECG value and heartbeat to an ECGLog file
        */
        public void recordData(ThinkGear.DataRow[] dataRowArray)
        {
            if ((dataLogStream != null) && (ECGLogStream != null))
            {
                foreach (DataRow dr in dataRowArray)
                {
                    //suppress the EGODebug1, EGODebug2, and EGODebug3 outputs
                    if ((dr.Type.GetHashCode() != 0x84) && (dr.Type.GetHashCode() != 0x08) && (dr.Type.GetHashCode() != 0x85))
                    {
                        //write the timestamp
                        dataLogStream.Write(dr.Time.ToString("#0.000") + ": " + (dr.Type.GetHashCode().ToString("X").PadLeft(2, '0')) + " ");

                        //print out the hex values into datalog
                        foreach (byte b in dr.Data)
                        {
                            dataLogStream.Write(b.ToString("X").PadLeft(2, '0') + " ");
                        }
                        dataLogStream.Write(dataLogStream.NewLine);
                    }

                    //also print to the ECGLog
                    if (dr.Type.GetHashCode() == 0x95)
                    {
                        //write the timestamp
                        ECGLogStream.Write(dr.Time.ToString("#0.000") + ": ");

                        //print out the ECG waveform, ASIC heartbeat value, and "real time" heartbeat value into EGCLog
                        if (dr.Type.GetHashCode() == 0x95)
                        {
                            ADCValue = (short)((dr.Data[0] << 8) + dr.Data[1]);
                            ECGLogStream.Write(ADCValue.ToString().PadLeft(6, ' '));
                            ECGLogStream.Write(ECGLogStream.NewLine);
                        }
                    }
                }
            }
        }

        //when data saving is done
        void OnDataSavingFinished(object sender, EventArgs e)
        {
            //no need for this currently
        }


        //update the connect button status
        delegate void updateConnectButtonDelegate(bool connected);
        public void updateConnectButton(bool connected)
        {
            if (this.InvokeRequired)
            {
                updateConnectButtonDelegate del = new updateConnectButtonDelegate(updateConnectButton);
                this.Invoke(del, new object[] { connected });
            }
            else
            {
                if (connected)
                {
                    this.connectButton.Enabled = false;
                    this.connectButton.Visible = false;

                    this.portText.Enabled = false;

                    this.disconnectButton.Enabled = true;
                    this.disconnectButton.Visible = true;

                    //when connected, enable identification and add new user
                    //   this.identificationButton.Enabled = true;
                    //   this.identificationButton.Visible = true;

                    //  this.newUserButton.Enabled = true;
                    //    this.newUserButton.Visible = true;

                    this.Replay.Enabled = true;
                    this.Replay.Visible = true;

                }
                else
                {
                    this.disconnectButton.Enabled = false;
                    this.disconnectButton.Visible = false;

                    this.portText.Enabled = true;

                    this.connectButton.Enabled = true;
                    this.connectButton.Visible = true;

                    //when disconnected, disable identification and add new user
                    //       this.identificationButton.Enabled = false;
                    //    this.identificationButton.Visible = false;

                    // this.newUserButton.Enabled = false;
                    //  this.newUserButton.Visible = false;

                    this.Replay.Enabled = false;
                    this.Replay.Visible = false;

                }

            }
        }

        //show/hide the engergy picture box
        delegate void ToggleEnergyPictureBoxDelegate(bool visible);
        public void toggleEnergyPictureBox(bool visible)
        {
            if (this.InvokeRequired)
            {
                ToggleEnergyPictureBoxDelegate del = new ToggleEnergyPictureBoxDelegate(toggleEnergyPictureBox);
                this.Invoke(del, new object[] { visible });
            }
            else
            {
                this.energyPictureBox.Visible = visible;
            }
        }

        //show a certain image on the picture box
        delegate void SetEnergyPictureBoxDelegate(Bitmap image);
        public void setEnergyPictureBox(Bitmap image)
        {
            if (this.InvokeRequired)
            {
                SetEnergyPictureBoxDelegate del = new SetEnergyPictureBoxDelegate(setEnergyPictureBox);
                this.Invoke(del, new object[] { image });
            }
            else
            {
                this.energyPictureBox.Image = image;
            }
        }

        //update the fatigue start button
        delegate void ToggleFatigueStartButtonDelegate(bool visible);
        public void toggleFatigueStartButton(bool visible)
        {
            if (this.InvokeRequired)
            {
                ToggleFatigueStartButtonDelegate del = new ToggleFatigueStartButtonDelegate(toggleFatigueStartButton);
                this.Invoke(del, new object[] { visible });
            }
            else
            {
                //    this.startFatigueButton.Visible = visible;
            }
        }

        //disable/enable the fatigue start button
        delegate void FatigueStartButtonDelegate(bool enabled);
        public void fatigueStartButton(bool enabled)
        {
            if (this.InvokeRequired)
            {
                FatigueStartButtonDelegate del = new FatigueStartButtonDelegate(fatigueStartButton);
                this.Invoke(del, new object[] { enabled });
            }
            else
            {
                //      this.startFatigueButton.Enabled = enabled;
            }
        }

        //update the fatigue stop button
        delegate void ToggleFatigueStopButtonDelegate(bool visible);
        public void toggleFatigueStopButton(bool visible)
        {
            if (this.InvokeRequired)
            {
                ToggleFatigueStopButtonDelegate del = new ToggleFatigueStopButtonDelegate(toggleFatigueStopButton);
                this.Invoke(del, new object[] { visible });
            }
            else
            {
                // this.stopFatigueButton.Visible = visible;
            }
        }

        //update the fatigue level
        delegate void UpdateFatigueLabelDelegate(string tempString);
        public void updateFatigueLevelLabel(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateFatigueLabelDelegate del = new UpdateFatigueLabelDelegate(updateFatigueLevelLabel);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at updateFatigueLevelLabel: " + e.Message);
                    }

                }
                else
                {

                    //if the status is "calculating...", change the font to something smaller
                    if (String.Equals("Calculating...", tempString))
                    {
                        fatigueLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    }
                    else
                    {
                        fatigueLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    }

                    this.fatigueLabel.Text = tempString;
                }
            }
        }

        //make the fatigue label visible/invisible
        delegate void ToggleFatigueLabelDelegate(bool visible);
        public void toggleFatigueLevelLabel(bool visible)
        {
            if (this.InvokeRequired)
            {
                //try catch necessary for handling case when form is disposing
                try
                {
                    ToggleFatigueLabelDelegate del = new ToggleFatigueLabelDelegate(toggleFatigueLevelLabel);
                    this.Invoke(del, new object[] { visible });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught exception at toggleFatigueLevelLabel: " + e.Message);
                }

            }
            else
            {
                this.fatigueLabel.Visible = visible;
            }
        }

        //make the fatigue label indicator visible/invisible
        delegate void ToggleFatigueLabelIndicatorDelegate(bool visible);
        public void toggleFatigueLevelLabelIndicator(bool visible)
        {
            if (this.InvokeRequired)
            {
                //try catch necessary for handling case when form is disposing
                try
                {
                    ToggleFatigueLabelIndicatorDelegate del = new ToggleFatigueLabelIndicatorDelegate(toggleFatigueLevelLabelIndicator);
                    this.Invoke(del, new object[] { visible });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught exception at toggleFatigueLevelLabelIndicator: " + e.Message);
                }

            }
            else
            {
                this.fatigueLabelIndicator.Visible = visible;
            }
        }

        //make the record button enabled/disabled
        delegate void ToggleRecordButtonDelegate(bool enabled);
        public void toggleRecordButton(bool enabled)
        {
            if (this.InvokeRequired)
            {
                //try catch necessary for handling case when form is disposing
                try
                {
                    ToggleRecordButtonDelegate del = new ToggleRecordButtonDelegate(toggleRecordButton);
                    this.Invoke(del, new object[] { enabled });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught exception at toggleRecordButton: " + e.Message);
                }

            }
            else
            {
                this.recordButton.Enabled = enabled;
            }
        }

        //update the realtime heart rate label status
        delegate void UpdateRealTimeHeartRateLabelDelegate(string tempString);
        public void updateRealTimeHeartRateLabel(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateRealTimeHeartRateLabelDelegate del = new UpdateRealTimeHeartRateLabelDelegate(updateRealTimeHeartRateLabel);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at updateRealTimeHeartRateLabel: " + e.Message);
                    }

                }
                else
                {
                    this.realtimeHeartRateLabel.Text = tempString;
                }
            }
        }

        //update the HBR box
        delegate void UpdateHBRBoxDelegate(string tempString);
        public void updateHBRBox(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateHBRBoxDelegate del = new UpdateHBRBoxDelegate(updateHBRBox);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at HBR Box: " + e.Message);
                    }

                }
                else
                {
                    this.HBR_Box.Text = tempString;
                }
            }
        }

        //update the Interval box
        delegate void UpdateIntervalBoxDelegate(string tempString);
        public void updateIntervalBox(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateIntervalBoxDelegate del = new UpdateIntervalBoxDelegate(updateIntervalBox);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at IntervalBox Box: " + e.Message);
                    }

                }
                else
                {
                    this.Interval_textBox.Text = tempString;
                }
            }
        }


        //update the Signal_QualityBox 
        delegate void UpdateSignal_QualityBoxDelegate(string tempString);
        public void updateSignal_QualityBox(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSignal_QualityBoxDelegate del = new UpdateSignal_QualityBoxDelegate(updateSignal_QualityBox);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at Signal_QualityBox  Box: " + e.Message);
                    }

                }
                else
                {
                    this.Signal_QualityBox.Text = tempString;
                }
            }
        }

        //update RC_CSEL[8:1] 
        delegate void UpdateTrim_ByteDelegate(string tempString);
        public void updateTrim_ByteBox(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateTrim_ByteDelegate del = new UpdateTrim_ByteDelegate(updateTrim_ByteBox);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at TrimByte Box: " + e.Message);
                    }

                }
                else
                {
                    this.Trim_ByteBox.Text = tempString;
                }
            }
        }

        //update RC_CSEL_IN[0] and RC_RSEL_IN[3:0] 
        delegate void UpdateTrim_Byte_0Delegate(string tempString);
        public void updateTrim_Byte_0Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateTrim_Byte_0Delegate del = new UpdateTrim_Byte_0Delegate(updateTrim_Byte_0Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at TrimByte_0 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox19.Text = tempString;
                }
            }
        }

        //update UART conOut 
        delegate void UpdateconOut7_ByteDelegate(string tempString);
        public void updateconOut7Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut7_ByteDelegate del = new UpdateconOut7_ByteDelegate(updateconOut7Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut7 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox8.Text = tempString;
                }
            }
        }

        delegate void UpdateconOut6_ByteDelegate(string tempString);
        public void updateconOut6Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut6_ByteDelegate del = new UpdateconOut6_ByteDelegate(updateconOut6Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut6 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox7.Text = tempString;
                }
            }
        }

        delegate void UpdateconOut5_ByteDelegate(string tempString);
        public void updateconOut5Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut5_ByteDelegate del = new UpdateconOut5_ByteDelegate(updateconOut5Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut5 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox6.Text = tempString;
                }
            }
        }

        delegate void UpdateconOut4_ByteDelegate(string tempString);
        public void updateconOut4Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut4_ByteDelegate del = new UpdateconOut4_ByteDelegate(updateconOut4Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut4 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox5.Text = tempString;
                }
            }
        }

        delegate void UpdateconOut3_ByteDelegate(string tempString);
        public void updateconOut3Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut3_ByteDelegate del = new UpdateconOut3_ByteDelegate(updateconOut3Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut3 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox4.Text = tempString;
                }
            }
        }

        delegate void UpdateconOut2_ByteDelegate(string tempString);
        public void updateconOut2Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut2_ByteDelegate del = new UpdateconOut2_ByteDelegate(updateconOut2Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut2 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox3.Text = tempString;
                }
            }
        }

        delegate void UpdateconOut1_ByteDelegate(string tempString);
        public void updateconOut1Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut1_ByteDelegate del = new UpdateconOut1_ByteDelegate(updateconOut1Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut1 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox2.Text = tempString;
                }
            }
        }

        delegate void UpdateconOut0_ByteDelegate(string tempString);
        public void updateconOut0Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateconOut0_ByteDelegate del = new UpdateconOut0_ByteDelegate(updateconOut0Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at conOut0 Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox1.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate(string tempString);
        public void updateSPI_I2CRegisterBox0(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate del = new UpdateSPI_I2CRegisterDelegate(updateSPI_I2CRegisterBox0);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox17.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate1(string tempString);
        public void updateSPI_I2CRegisterBox1(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate1 del = new UpdateSPI_I2CRegisterDelegate1(updateSPI_I2CRegisterBox1);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox24.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate2(string tempString);
        public void updateSPI_I2CRegisterBox2(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate2 del = new UpdateSPI_I2CRegisterDelegate2(updateSPI_I2CRegisterBox2);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox28.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate3(string tempString);
        public void updateSPI_I2CRegisterBox3(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate3 del = new UpdateSPI_I2CRegisterDelegate3(updateSPI_I2CRegisterBox3);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox32.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate4(string tempString);
        public void updateSPI_I2CRegisterBox4(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate4 del = new UpdateSPI_I2CRegisterDelegate4(updateSPI_I2CRegisterBox4);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox36.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate5(string tempString);
        public void updateSPI_I2CRegisterBox5(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate5 del = new UpdateSPI_I2CRegisterDelegate5(updateSPI_I2CRegisterBox5);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox40.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate6(string tempString);
        public void updateSPI_I2CRegisterBox6(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate6 del = new UpdateSPI_I2CRegisterDelegate6(updateSPI_I2CRegisterBox6);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox44.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate7(string tempString);
        public void updateSPI_I2CRegisterBox7(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate7 del = new UpdateSPI_I2CRegisterDelegate7(updateSPI_I2CRegisterBox7);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox48.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate8(string tempString);
        public void updateSPI_I2CRegisterBox8(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate8 del = new UpdateSPI_I2CRegisterDelegate8(updateSPI_I2CRegisterBox8);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox52.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate9(string tempString);
        public void updateSPI_I2CRegisterBox9(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate9 del = new UpdateSPI_I2CRegisterDelegate9(updateSPI_I2CRegisterBox9);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox56.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate10(string tempString);
        public void updateSPI_I2CRegisterBox10(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate10 del = new UpdateSPI_I2CRegisterDelegate10(updateSPI_I2CRegisterBox10);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox60.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate11(string tempString);
        public void updateSPI_I2CRegisterBox11(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate11 del = new UpdateSPI_I2CRegisterDelegate11(updateSPI_I2CRegisterBox11);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox18.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate12(string tempString);
        public void updateSPI_I2CRegisterBox12(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate12 del = new UpdateSPI_I2CRegisterDelegate12(updateSPI_I2CRegisterBox12);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox23.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate13(string tempString);
        public void updateSPI_I2CRegisterBox13(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate13 del = new UpdateSPI_I2CRegisterDelegate13(updateSPI_I2CRegisterBox13);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox27.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate14(string tempString);
        public void updateSPI_I2CRegisterBox14(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate14 del = new UpdateSPI_I2CRegisterDelegate14(updateSPI_I2CRegisterBox14);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox31.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate15(string tempString);
        public void updateSPI_I2CRegisterBox15(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate15 del = new UpdateSPI_I2CRegisterDelegate15(updateSPI_I2CRegisterBox15);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox35.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate16(string tempString);
        public void updateSPI_I2CRegisterBox16(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate16 del = new UpdateSPI_I2CRegisterDelegate16(updateSPI_I2CRegisterBox16);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox39.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate17(string tempString);
        public void updateSPI_I2CRegisterBox17(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate17 del = new UpdateSPI_I2CRegisterDelegate17(updateSPI_I2CRegisterBox17);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox43.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate18(string tempString);
        public void updateSPI_I2CRegisterBox18(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate18 del = new UpdateSPI_I2CRegisterDelegate18(updateSPI_I2CRegisterBox18);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox47.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate19(string tempString);
        public void updateSPI_I2CRegisterBox19(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate19 del = new UpdateSPI_I2CRegisterDelegate19(updateSPI_I2CRegisterBox19);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox51.Text = tempString;
                }
            }
        }

        delegate void UpdateSPI_I2CRegisterDelegate20(string tempString);
        public void updateSPI_I2CRegisterBox20(string tempString)
        {

            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateSPI_I2CRegisterDelegate20 del = new UpdateSPI_I2CRegisterDelegate20(updateSPI_I2CRegisterBox20);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at SPI/I2C Register Box: " + e.Message);
                    }

                }
                else
                {
                    this.textBox55.Text = tempString;
                }
            }
        }

        delegate void UpdateWrite_Byte0_BoxDelegate(string tempString);
        public void updateWrite_Byte0_F_Box(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateWrite_Byte0_BoxDelegate del = new UpdateWrite_Byte0_BoxDelegate(updateWrite_Byte0_F_Box);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at HBR Box: " + e.Message);
                    }

                }
                else
                {
                    this.Write_Byte0_Box.Text = tempString;

                }
            }
        }

        //update the average heart rate label status
        delegate void UpdateAverageHeartRateLabelDelegate(string tempString);
        public void updateAverageHeartRateLabel(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateAverageHeartRateLabelDelegate del = new UpdateAverageHeartRateLabelDelegate(updateAverageHeartRateLabel);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at updateAverageHeartRateLabel: " + e.Message);
                    }

                }
                else
                {
                    this.averageHeartRateLabel.Text = tempString;
                }
            }
        }


        //update the HRV label
        delegate void UpdateHRVLabelDelegate(string tempString);
        public void updateHRVLabel(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        UpdateHRVLabelDelegate del = new UpdateHRVLabelDelegate(updateHRVLabel);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("caught exception at UpdateHRVLabel: " + e.Message);
                    }
                }
                else
                {
                    this.HRVLabel.Text = tempString;
                }
            }
        }


        delegate void UpdateDataForGraphicThreadDelegate(object sender);
        public void UpdateDataForGraphicThread(object sender)
        {
            if (this.InvokeRequired)
            {
                UpdateDataForGraphicThreadDelegate del = new UpdateDataForGraphicThreadDelegate(UpdateDataForGraphicThread);
                this.Invoke(del, new object[] { sender });
            }
            else
            {
                double[] raw = sender as double[];
                int length = raw.Length;
                double[] result = new double[length];
                Array.Copy(raw, result, length);
                for (int i = 0; i < length; i++)
                {
                    double r = result[i];
                    if (graphPanel1.BarGraph.BarReadType == ReadType.RawArray)
                    {
                        Console.WriteLine("update BarGraph data:" + raw);

                        /// TODO: Input 'Raw' from ThinkGear SDK or text ,then BarGraph.cs should be caculate the FFT with 
                        /// AForge.Math.FourierTransform.FFT(Complex[] , AForge.Math.FourierTransform.Direction.Forward).
                        AForge.Math.Complex d = new AForge.Math.Complex(r, 0);
                        graphPanel1.BarGraph.Add(d);
                    }
                    else if (graphPanel1.BarGraph.BarReadType == ReadType.FFTArray)
                    {

                        float f = Convert.ToSingle(r);
                        /// TODO: Input data for FFT array,
                        graphPanel1.BarGraph.AddFFT(f);
                        Console.WriteLine("update float data:" + f + ",raw:" + raw);
                    }
                }
            }
        }
        
        /// <summary>
        /// call for delegate 'UpdateDataForGraphicDelegate' 
        /// </summary>
        /// <param name="raw"></param>
        public void UpdateDataForGraphic(double raw)
        {

            if (this.InvokeRequired)
            {
                UpdateDataForGraphicDelegate del = new UpdateDataForGraphicDelegate(UpdateDataForGraphic);
                this.Invoke(del, new object[] { raw });
            }
            else
            {
                if (graphPanel1.BarGraph.BarReadType == ReadType.RawArray)
                {
                    Console.WriteLine("update BarGraph data:" + raw);

                    /// TODO: Input 'Raw' from ThinkGear SDK or text ,then BarGraph.cs should be caculate the FFT with 
                    /// AForge.Math.FourierTransform.FFT(Complex[] , AForge.Math.FourierTransform.Direction.Forward).
                    AForge.Math.Complex d = new AForge.Math.Complex(raw, 0);
                    graphPanel1.BarGraph.Add(d);
                }
                else if (graphPanel1.BarGraph.BarReadType == ReadType.FFTArray)
                {

                    float f = Convert.ToSingle(raw);
                    /// TODO: Input data for FFT array,
                    graphPanel1.BarGraph.AddFFT(f);
                    //Console.WriteLine("update float data:" + f + ",raw:" + raw);
                }
            }
        }

        delegate void UpdateStatusLabelDelegate(string tempText);
        public void updateStatusLabel(string tempText)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    UpdateStatusLabelDelegate del = new UpdateStatusLabelDelegate(updateStatusLabel);
                    this.Invoke(del, new object[] { tempText });
                }
                else
                {
                    this.statusLabel.Text = tempText;
                }
            }
        }

        //update the respiration rate
        delegate void UpdateRespirationRateIndicatorDelegate(string tempString);
        public void updateRespirationRateIndicator(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    //try catch necessary for handling case when form is disposing
                    try
                    {
                        UpdateRespirationRateIndicatorDelegate del = new UpdateRespirationRateIndicatorDelegate(updateRespirationRateIndicator);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Caught exception at UpdateRespirationRateIndicator: " + e.Message);
                    }

                }
                else
                {
                    this.respirationRateIndicator.Text = tempString;
                }
            }
        }

        //update heart age
        delegate void UpdateHeartAgeDelegate(string tempString);
        public void updateHeartAgeIndicator(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        UpdateHeartAgeDelegate del = new UpdateHeartAgeDelegate(updateHeartAgeIndicator);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("caught exception at UpdateHeartAgeIndicator: " + e.Message);
                    }
                }
                else
                {
                    this.heartAgeIndicator.Text = tempString;
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {

            realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 262, realtimeHeartRateLabelIndicator.Location.Y);
            realtimeHeartRateLabel.Location = new System.Drawing.Point(this.Width - 112, realtimeHeartRateLabel.Location.Y);

            averageHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 262, realtimeHeartRateLabelIndicator.Location.Y + 30);
            averageHeartRateLabel.Location = new System.Drawing.Point(this.Width - 112, realtimeHeartRateLabel.Location.Y + 30);

            HRVLabelIndicator.Location = new System.Drawing.Point(this.Width - 262, realtimeHeartRateLabelIndicator.Location.Y + 30);
            HRVLabel.Location = new System.Drawing.Point(this.Width - 112, realtimeHeartRateLabel.Location.Y + 30);

            energyPictureBox.Location = new System.Drawing.Point(this.Width - 492, energyPictureBox.Location.Y);

            fatigueLabelIndicator.Location = new System.Drawing.Point(this.Width - 501, energyPictureBox.Location.Y + energyPictureBox.Height + 8);
            fatigueLabel.Location = new System.Drawing.Point(this.Width - 399, energyPictureBox.Location.Y + energyPictureBox.Height + 6);

            // statusLabel.Location = new System.Drawing.Point(statusLabel.Location.X, this.Height - 54);

            //recordButton.Location = new System.Drawing.Point(this.Width - 240, this.Height - 73);
            //stopButton.Location = new System.Drawing.Point(this.Width - 240, this.Height - 73);

            //clearButton.Location = new System.Drawing.Point(this.Width - 140, this.Height - 73);

            //      startFatigueButton.Location = new System.Drawing.Point(this.Width - 360, this.Height - 73);
            //   stopFatigueButton.Location = new System.Drawing.Point(this.Width - 360, this.Height - 73);

            //rawGraphPanel.Location = new Point(rawGraphPanel.Location.X, HRVLabelIndicator.Location.Y + HRVLabelIndicator.Height + 9);
            //rawGraphPanel.Height = (int)(recordButton.Location.Y - rawGraphPanel.Location.Y - 15);
            //rawGraphPanel.Width = this.Width - 10;

            //  inputAgeAndFileNameButton.Location = new System.Drawing.Point(this.Width - 480, this.Height - 73);

            // identificationButton.Location = new System.Drawing.Point(this.Width - 600, this.Height - 73);
            //    newUserButton.Location = new System.Drawing.Point(this.Width - 720, this.Height - 73);
            base.OnSizeChanged(e);
        }
 
        private void inputAgeAndFileNameButton_Click(object sender, EventArgs e)
        {
            heartAgeInputGUI.Show();
            heartAgeInputGUI.TopMost = true;
            if (heartAgeInputGUI.WindowState != FormWindowState.Normal)
            {
                heartAgeInputGUI.WindowState = FormWindowState.Normal;
            }
            if (heartAgeInputGUI.Visible == false)
            {
                heartAgeInputGUI.Visible = true;
            }

        }

        private void identificationButton_Click(object sender, EventArgs e)
        {
            //tell launcher to start identification
            identificationGUI.Show();
            IdentificationButtonClicked(this, EventArgs.Empty);


        }

        /* private void newUserButton_Click(object sender, EventArgs e)
         {
             addNewUerGUI.Show();
             addNewUerGUI.TopMost = true;
             if (addNewUerGUI.WindowState != FormWindowState.Normal)
             {
                 addNewUerGUI.WindowState = FormWindowState.Normal;
             }
             if (addNewUerGUI.Visible == false)
             {
                 addNewUerGUI.Visible = true;
             }
         }*/

        private void Replay_Click(object sender, EventArgs e)
        {

            rawGraphPanel.LineGraph.Clear();
            stopButton.Visible = true;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                try
                {
                    loadedFileData = File.ReadAllText(file);
                    replayEnable = true;
                }
                catch (IOException)
                {
                }

                ReplyButtonClicked(this, EventArgs.Empty);
                //Console.WriteLine(text);
            }
        }

        private void stopReplay_Click(object sender, EventArgs e)
        {
            StopReplayButtonClicked(this, EventArgs.Empty);
        }

        public event deleupdateClockFrequencyTOBMD sendClockFrequencyWriteDateEvent;

        private void button7_Click(object sender, EventArgs e)
        {
            /* send trim byte to BMD101*/
            byte[] commandToSend = { };
            sendClockFrequencyWriteDateEvent(commandToSend);

        }

        public event deleupdateUARTCmdToBMD sendUARTOpenCmdByteEvent;

        private void button2_Click(object sender, EventArgs e)
        {
            //byte[] commandToSend = { 0x77, 0x01, 0x30, 0xCE };
            //sendUARTOpenCmdByteEvent(commandToSend);

            button2.Enabled = true;

            UART_ENABLED = true;

            UART_Close_Button.Visible = true;
            UART_Close_Button.Enabled = true;

            UART_Open_Button.Visible = false;
            UART_Open_Button.Enabled = false;

            I2C_Open_Button.Enabled = false;
            SPI_Open_Button.Enabled = false;

        }

        public event deleupdateUARTCmdToBMD sendUARTCloseCmdByteEvent;
        private void button1_Click(object sender, EventArgs e)
        {

            //byte[] commandToSend = { };
            //sendUARTCloseCmdByteEvent(commandToSend);

            button2.Enabled = false;

            UART_ENABLED = false;

            UART_Close_Button.Visible = false;
            UART_Close_Button.Enabled = false;

            UART_Open_Button.Visible = true;
            UART_Open_Button.Enabled = true;

            I2C_Open_Button.Enabled = true;
            SPI_Open_Button.Enabled = true;

        }

        //public event deleupdateSPICmdToBMD sendSPIOpenCmdEvent;
        private void button4_Click(object sender, EventArgs e)
        {

            //byte[] commandToSend = { 0x77, 0x01, 0x31, 0xCD };
            //sendSPIOpenCmdEvent(commandToSend);

            button10.Enabled = true;
            //button10.Visible = true;

            button12.Enabled = true;
            button14.Enabled = true;
            button11.Enabled = true;
            button13.Enabled = true;
            button8.Enabled = true;

            SPI_ENABLED = true;

            SPI_Open_Button.Visible = false;
            SPI_Open_Button.Enabled = false;

            SPI_Close_Button.Visible = true;
            SPI_Close_Button.Enabled = true;

            UART_Open_Button.Enabled = false;
            I2C_Open_Button.Enabled = false;
        }

        public event deleupdateSPICmdToBMD sendSPICloseCmdEvent;
        private void button3_Click(object sender, EventArgs e)
        {
            //byte[] commandToSend = { };
            //sendSPICloseCmdEvent(commandToSend);

            button10.Enabled = false;
            //button10.Visible = false;

            button12.Enabled = false;
            button14.Enabled = false;
            button11.Enabled = false;
            button13.Enabled = false;
            button8.Enabled = false;

            SPI_ENABLED = false;

            SPI_Close_Button.Visible = false;
            SPI_Close_Button.Enabled = false;

            SPI_Open_Button.Visible = true;
            SPI_Open_Button.Enabled = true;

            UART_Open_Button.Enabled = true;
            I2C_Open_Button.Enabled = true;

        }

        //public event deleupdateI2CCmdToBMD sendI2COpenCmdEvent;
        private void button6_Click(object sender, EventArgs e)
        {
            //byte[] commandToSend = { 0x77, 0x01, 0x32, 0xCC };
            //sendI2COpenCmdEvent(commandToSend);

            button10.Enabled = true;
            //button10.Visible = true;

            button15.Enabled = true;
            button16.Enabled = true;

            I2C_ENABLED = true;

            I2C_Open_Button.Visible = false;
            I2C_Open_Button.Enabled = false;

            I2C_Close_Button.Visible = true;
            I2C_Close_Button.Enabled = true;

            UART_Open_Button.Enabled = false;
            SPI_Open_Button.Enabled = false;
        }

        public event deleupdateI2CCmdToBMD sendI2CCloseCmdEvent;
        private void button5_Click(object sender, EventArgs e)
        {
            //byte[] commandToSend = { };
            //sendI2CCloseCmdEvent(commandToSend);

            button10.Enabled = false;
            //button10.Visible = false;

            button15.Enabled = false;
            button16.Enabled = false;

            I2C_ENABLED = false;

            I2C_Close_Button.Visible = false;
            I2C_Close_Button.Enabled = false;

            I2C_Open_Button.Visible = true;
            I2C_Open_Button.Enabled = true;

            UART_Open_Button.Enabled = true;
            SPI_Open_Button.Enabled = true;
        }

        public event deleupdateSamplingRateToBMD sendSamplingRate300CmdEvent;
        private void button11_Click(object sender, EventArgs e)
        {
            byte[] commandToSend = { };
            sendSamplingRate300CmdEvent(commandToSend);

            Sample_Rate600.Visible = false;
            Sample_Rate600.Enabled = false;
            Sample_Rate300.Visible = true;
            Sample_Rate300.Enabled = true;

        }

        public event deleupdateSamplingRateToBMD sendSamplingRate600CmdEvent;
        private void button8_Click(object sender, EventArgs e)
        {
            byte[] commandToSend = { };
            sendSamplingRate600CmdEvent(commandToSend);

            Sample_Rate600.Visible = true;
            Sample_Rate600.Enabled = true;
            Sample_Rate300.Visible = false;
            Sample_Rate300.Enabled = false;

        }

        private void Write_Byte0_Box_TextChanged(object sender, EventArgs e)
        {
            string Value;
            Value = Write_Byte0_Box.Text;
        }

        public event deleupdateSPI_I2CWriteDateTOBMD sendSPI_I2CWriteDateEvent;
        private void button10_Click(object sender, EventArgs e)
        {
            string[] str = new string[21];

            str[0] = Write_Byte0_Box.Text.ToString();
            str[1] = Write_Byte1_Box.Text.ToString();
            str[2] = Write_Byte2_Box.Text.ToString();
            str[3] = Write_Byte3_Box.Text.ToString();
            str[4] = Write_Byte4_Box.Text.ToString();
            str[5] = Write_Byte5_Box.Text.ToString();
            str[6] = Write_Byte6_Box.Text.ToString();
            str[7] = Write_Byte7_Box.Text.ToString();
            str[8] = Write_Byte8_Box.Text.ToString();
            str[9] = Write_Byte9_Box.Text.ToString();
            str[10] = Write_Byte15_Box.Text.ToString();
            str[11] = Write_Byte16_Box.Text.ToString();
            str[12] = Write_Byte17_Box.Text.ToString();
            str[13] = Write_Byte18_Box.Text.ToString();
            str[14] = Write_Byte19_Box.Text.ToString();
            str[15] = Write_Byte20_Box.Text.ToString();
            str[16] = Write_Byte10_Box.Text.ToString();
            str[17] = Write_Byte11_Box.Text.ToString();
            str[18] = Write_Byte12_Box.Text.ToString();
            str[19] = Write_Byte13_Box.Text.ToString();
            str[20] = Write_Byte14_Box.Text.ToString();

            byte[] Value = StringToByteArray(str[0] + str[1] + str[2] + str[3] + str[4] + str[5] + str[6] + str[7]
                               + str[8] + str[9] + str[10] + str[11] + str[12] + str[13] + str[14] + str[15]
                               + str[16] + str[17] + str[18] + str[19] + str[20]);

            string[] header = new string[3];
            string[] cs = new string[1];

            byte[] checkSum = new byte[1];
            int i;

            checkSum[0] = 0x00;

            if (SPI_ENABLED)
            {
                checkSum[0] += 0x16;
                checkSum[0] += 0x21;

                header[0] = "77";
                header[1] = "16";
                header[2] = "21";

                for (i = 0; i < 21; i++)
                {
                    checkSum[0] += Value[i];
                }
                checkSum[0] = (byte)(~checkSum[0]);

                cs[0] = BitConverter.ToString(checkSum);

                byte[] commandToSend = StringToByteArray(header[0] + header[1] + header[2] + str[0] + str[1] + str[2] + str[3] + str[4] + str[5] + str[6] + str[7]
                                           + str[8] + str[9] + str[10] + str[11] + str[12] + str[13] + str[14] + str[15]
                                           + str[16] + str[17] + str[18] + str[19] + str[20] + cs[0]);
                sendSPI_I2CWriteDateEvent(commandToSend);

            }
            else if (I2C_ENABLED)
            {
                checkSum[0] += 0x16;
                checkSum[0] += 0x22;

                header[0] = "77";
                header[1] = "16";
                header[2] = "22";

                for (i = 0; i < 21; i++)
                {
                    checkSum[0] += Value[i];
                }
                checkSum[0] = (byte)(~checkSum[0]);

                cs[0] = BitConverter.ToString(checkSum);

                byte[] commandToSend = StringToByteArray(header[0] + header[1] + header[2] + str[0] + str[1] + str[2] + str[3] + str[4] + str[5] + str[6] + str[7]
                                           + str[8] + str[9] + str[10] + str[11] + str[12] + str[13] + str[14] + str[15]
                                           + str[16] + str[17] + str[18] + str[19] + str[20] + cs[0]);
                sendSPI_I2CWriteDateEvent(commandToSend);

            }
            else
            {
                byte[] commandToSend = { };
                sendSPI_I2CWriteDateEvent(commandToSend);
            }
        }


        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            // string Value;
            // Value = textBox9.Text;
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            // string Value;
            // Value = textBox11.Text;
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            // string Value;
            //Value = textBox12.Text;
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            // string Value;
            // Value = textBox13.Text;
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            // string Value;
            // Value = textBox14.Text;
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            //  string Value;
            //  Value = textBox15.Text;
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            // string Value;
            // Value = textBox16.Text;
        }

        public event deleupdateUARTWriteDateTOBMD sendUARTWriteDateEvent;
        private void button2_Click_1(object sender, EventArgs e)
        {
            string[] str = new string[8];

            str[0] = textBox9.Text.ToString();
            str[1] = textBox10.Text.ToString();
            str[2] = textBox11.Text.ToString();
            str[3] = textBox12.Text.ToString();
            str[4] = textBox13.Text.ToString();
            str[5] = textBox14.Text.ToString();
            str[6] = textBox15.Text.ToString();
            str[7] = textBox16.Text.ToString();

            string[] sync_byte = new string[2];

            sync_byte[0] = "AA";
            sync_byte[1] = "AA";

            byte[] Value = StringToByteArray(sync_byte[0] + sync_byte[1] + str[0] + str[1] + str[2] + str[3] + str[4] + str[5] + str[6] + str[7]);

            //byte[] commandToSend = { 0xAA, 0xAA };
            //sendUARTWriteDateEvent(commandToSend);

            sendUARTWriteDateEvent(Value);
        }

        public static byte[] StringToByteArray(String hex)
        {
            //Console.WriteLine("hex: " + hex);
            int NumberChars = hex.Length;
            //Console.WriteLine("hex len: " + NumberChars);

            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                //Console.WriteLine("i =  " + i);
                //Console.WriteLine("single string: " + hex.Substring(i, 2));
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                //Console.WriteLine("single byte: " + bytes[i / 2]);
            }

            return bytes;
        }       

        public event deleupdateReadSPICmdFromBMD SPIReadFromBMDEVENT;
        private void button9_Click(object sender, EventArgs e)
        {
            /*
            if (SPI_ENABLED)
            {
                byte[] commandToSend = { 0x77, 0x01, 0x11, 0xED };
                SPIReadFromBMDEVENT(commandToSend);
            }
            else if (I2C_ENABLED)
            {
                byte[] commandToSend = { 0x77, 0x01, 0x12, 0xEC };
                SPIReadFromBMDEVENT(commandToSend);
            }
            else
            {
                byte[] commandToSend = { };
                SPIReadFromBMDEVENT(commandToSend);
            }
            */

            /*
            string textbox17_hex = String.Format("{0:X}", 0);//返回的值。
            string textbox24_hex = String.Format("{0:X}", 1);//返回的值。
            string textbox28_hex = String.Format("{0:X}", 2);//返回的值。
            string textbox32_hex = String.Format("{0:X}", 3);//返回的值。
            string textbox36_hex = String.Format("{0:X}", 4);//返回的值。
            string textbox40_hex = String.Format("{0:X}", 5);//返回的值。
            string textbox44_hex = String.Format("{0:X}", 6);//返回的值。
            string textbox48_hex = String.Format("{0:X}", 7);//返回的值。
            string textbox52_hex = String.Format("{0:X}", 8);//返回的值。
            string textbox56_hex = String.Format("{0:X}", 9);//返回的值。
            string textbox60_hex = String.Format("{0:X}", 10);//返回的值。
            string textbox18_hex = String.Format("{0:X}", 11);//返回的值。
            string textbox23_hex = String.Format("{0:X}", 12);//返回的值。
            string textbox27_hex = String.Format("{0:X}", 13);//返回的值。
            string textbox31_hex = String.Format("{0:X}", 14);//返回的值。
            string textbox35_hex = String.Format("{0:X}", 15);//返回的值。
            string textbox39_hex = String.Format("{0:X}", 16);//返回的值。
            string textbox43_hex = String.Format("{0:X}", 17);//返回的值。
            string textbox47_hex = String.Format("{0:X}", 18);//返回的值。
            string textbox51_hex = String.Format("{0:X}", 19);//返回的值。
            string textbox55_hex = String.Format("{0:X}", 20);//返回的值。

           
            textbox17_hex= "0x"+textbox17_hex;
            textBox17.Text =textbox17_hex;

            textbox18_hex = "0x"+textbox18_hex;
            textBox18.Text =textbox18_hex;

            textbox24_hex = "0x"+textbox24_hex;
            textBox24.Text =textbox24_hex;

            textbox23_hex = "0x"+textbox23_hex;
            textBox23.Text= textbox23_hex;

            textbox27_hex = "0x"+textbox27_hex;
            textBox27.Text =textbox27_hex;

            textbox28_hex ="0x"+textbox28_hex;
            textBox28.Text =textbox28_hex;

            textbox31_hex = "0x"+textbox31_hex;
            textBox31.Text =textbox31_hex;

            textbox32_hex = "0x"+textbox32_hex;
            textBox32.Text=textbox32_hex;

            textbox35_hex = "0x"+textbox35_hex;
            textBox35.Text=textbox35_hex;

            textbox36_hex ="0x"+textbox36_hex;
            textBox36.Text=textbox36_hex;
            
            textbox39_hex = "0x"+textbox39_hex;
            textBox39.Text=textbox39_hex;
            
            textbox40_hex ="0x"+textbox40_hex;
            textBox40.Text=textbox40_hex;

            textbox43_hex = "0x"+textbox43_hex;
            textBox43.Text=textbox43_hex;

            textbox44_hex = "0x"+textbox44_hex;
            textBox44.Text=textbox44_hex;
            
            textbox47_hex = "0x"+textbox47_hex;
            textBox47.Text=textbox47_hex;

            textbox48_hex = "0x"+textbox48_hex;
            textBox48.Text=textbox48_hex;

            textbox51_hex = "0x"+textbox51_hex;;
            textBox51.Text= textbox51_hex;;
            
            textbox52_hex = "0x"+textbox52_hex;
            textBox52.Text=textbox52_hex;

            textbox55_hex = "0x"+textbox55_hex;
            textBox55.Text=textbox55_hex;

            textbox56_hex = "0x" + textbox56_hex;
            textBox56.Text=textbox56_hex;

            textbox60_hex = "0x" + textbox60_hex;
            textBox60.Text = textbox60_hex; 
            */
        }

        public event deleupdateUARTReadFromBMD UARTReadFromBMDEVENT;
        private void button1_Click_1(object sender, EventArgs e)
        {
            //byte[] commandToSend = { 0x77, 0x01, 0x10, 0xEE };
            //UARTReadFromBMDEVENT(commandToSend);

            /*
            string textbox1_hex = String.Format("{0:X}", 0);//返回的值。
            string textbox2_hex = String.Format("{0:X}", 1);//返回的值。
            string textbox3_hex = String.Format("{0:X}", 2);//返回的值。
            string textbox4_hex = String.Format("{0:X}", 3);//返回的值。
            string textbox5_hex = String.Format("{0:X}", 4);//返回的值。
            string textbox6_hex = String.Format("{0:X}", 5);//返回的值。
            string textbox7_hex = String.Format("{0:X}", 6);//返回的值。
            string textbox8_hex = String.Format("{0:X}", 7);//返回的值。

            textbox1_hex = "0x" + textbox1_hex;
            textBox1.Text = textbox1_hex;

            textbox2_hex = "0x" + textbox2_hex;
            textBox2.Text = textbox2_hex;

            textbox3_hex = "0x" + textbox3_hex;
            textBox3.Text = textbox3_hex;

            textbox4_hex = "0x" + textbox4_hex;
            textBox4.Text = textbox4_hex;

            textbox5_hex = "0x" + textbox5_hex;
            textBox5.Text = textbox5_hex;

            textbox6_hex = "0x" + textbox6_hex;
            textBox6.Text = textbox6_hex;

            textbox7_hex = "0x" + textbox7_hex;
            textBox7.Text = textbox7_hex;

            textbox8_hex = "0x" + textbox8_hex;
            textBox8.Text = textbox8_hex;           
            */
        }      

        public event deleupdateReadTrimByteFromBMD TrimByteReadFromBMDEVENT;
        private void TrimByte_Read_button_Click(object sender, EventArgs e)
        {
            //byte[] commandToSend = { 0x77, 0x01, 0x13, 0xEB };
            //TrimByteReadFromBMDEVENT(commandToSend);
        }


        public event deleefuseProgram efuseProgramEvent;
        private void button4_Click_1(object sender, EventArgs e)
        {
            string[] str = new string[16];

            str[0] = textBox57.Text.ToString();
            str[1] = textBox53.Text.ToString();
            str[2] = textBox49.Text.ToString();
            str[3] = textBox45.Text.ToString();
            str[4] = textBox41.Text.ToString();
            str[5] = textBox37.Text.ToString();
            str[6] = textBox33.Text.ToString();
            str[7] = textBox29.Text.ToString();
            str[8] = textBox54.Text.ToString();
            str[9] = textBox50.Text.ToString();
            str[10] = textBox46.Text.ToString();
            str[11] = textBox42.Text.ToString();
            str[12] = textBox38.Text.ToString();
            str[13] = textBox34.Text.ToString();
            str[14] = textBox30.Text.ToString();
            str[15] = textBox26.Text.ToString();

            byte[] Value = StringToByteArray(str[0] + str[1] + str[2] + str[3] + str[4] + str[5] + str[6] + str[7]
                               + str[8] + str[9] + str[10] + str[11] + str[12] + str[13] + str[14] + str[15]);

            string[] header = new string[3];
            string[] cs = new string[1];

            byte[] checkSum = new byte[1];
            int i;

            checkSum[0] = 0x00;

            checkSum[0] += 0x11;
            checkSum[0] += 0x50;

            header[0] = "77";
            header[1] = "11";
            header[2] = "50";

            for (i = 0; i < 16; i++)
            {
                checkSum[0] += Value[i];
            }
            checkSum[0] = (byte)(~checkSum[0]);

            cs[0] = BitConverter.ToString(checkSum);

            byte[] commandToSend = StringToByteArray(header[0] + header[1] + header[2] + str[0] + str[1] + str[2] + str[3] + str[4] + str[5] + str[6] + str[7]
                                        + str[8] + str[9] + str[10] + str[11] + str[12] + str[13] + str[14] + str[15] + cs[0]);
            efuseProgramEvent(commandToSend);

        }      

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox4.Text = "";
            this.textBox5.Text = "";
            this.textBox6.Text = "";
            this.textBox7.Text = "";
            this.textBox8.Text = "";
            this.textBox19.Text = "";
            this.Trim_ByteBox.Text = "";
        }
        
        private void button6_Click_1(object sender, EventArgs e)
        {
            this.textBox17.Text = "";
            this.textBox24.Text = "";
            this.textBox28.Text = "";
            this.textBox32.Text = "";
            this.textBox36.Text = "";
            this.textBox40.Text = "";
            this.textBox44.Text = "";
            this.textBox48.Text = "";
            this.textBox52.Text = "";
            this.textBox56.Text = "";
            this.textBox60.Text = "";
            this.textBox18.Text = "";
            this.textBox23.Text = "";
            this.textBox27.Text = "";
            this.textBox31.Text = "";
            this.textBox35.Text = "";
            this.textBox39.Text = "";
            this.textBox43.Text = "";
            this.textBox47.Text = "";
            this.textBox51.Text = "";
            this.textBox55.Text = "";
            this.textBox19.Text = "";
            this.Trim_ByteBox.Text = "";
        }

        public event deleupdateSPICmdToBMD sendSPIOpenCmdEvent;
        private void button12_Click(object sender, EventArgs e)
        {
            byte[] commandToSend = { 0x77, 0x02, 0x60, 0x00, 0x9D };
            sendSPIOpenCmdEvent(commandToSend);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            byte[] commandToSend = { 0x77, 0x02, 0x60, 0x01, 0x9C };
            sendSPIOpenCmdEvent(commandToSend);
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            byte[] commandToSend = { 0x77, 0x02, 0x60, 0x02, 0x9B };
            sendSPIOpenCmdEvent(commandToSend);
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            byte[] commandToSend = { 0x77, 0x02, 0x60, 0x03, 0x9A };
            sendSPIOpenCmdEvent(commandToSend);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            byte[] commandToSend = { 0x77, 0x02, 0x60, 0x04, 0x99 };
            sendSPIOpenCmdEvent(commandToSend);
        }

        public event deleupdateI2CCmdToBMD sendI2COpenCmdEvent;
        private void button15_Click(object sender, EventArgs e)
        {
            byte[] commandToSend = { 0x77, 0x02, 0x61, 0x00, 0x9C };
            sendI2COpenCmdEvent(commandToSend);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            byte[] commandToSend = { 0x77, 0x02, 0x61, 0x01, 0x9B };
            sendI2COpenCmdEvent(commandToSend);
        }      
    }
    //  public event deleupdateUARTCmdToBMD sendUARTCloseCmdByteEvent;
    public delegate void deleupdateUARTCmdToBMD(byte[] tempString);
    public delegate void deleupdateSPICmdToBMD(byte[] tempString);
    public delegate void deleupdateI2CCmdToBMD(byte[] tempString);
    public delegate void deleupdateUARTWriteDateTOBMD(byte[] tempString);
    public delegate void deleupdateSPI_I2CWriteDateTOBMD(byte[] tempString);
    public delegate void deleupdateClockFrequencyTOBMD(byte[] tempString);
    public delegate void deleupdateSamplingRateToBMD(byte[] tempString);
    public delegate void deleupdateUARTReadFromBMD(byte[] tempString);
    public delegate void deleupdateReadSPICmdFromBMD(byte[] tempString);
    public delegate void deleupdateReadTrimByteFromBMD(byte[] tempString);
    public delegate void deleefuseProgram(byte[] tempString);
    public delegate void UpdateDataForGraphicDelegate(double raw);

    /*End of MainForm*/
    public class HeartAgeEventArgs : EventArgs
    {
        string filename;
        int age;

        public HeartAgeEventArgs(int age, string filename)
        {
            this.filename = filename;
            this.age = age;
        }

        public string parametersFileName
        {
            get { return filename; }
            set { this.filename = value; }
        }

        public int parametersAge
        {
            get { return age; }
            set { this.age = value; }
        }
    }
    //class for new user name
    public class NewUserNameEventArgs : EventArgs
    {
        string newUserName;

        public NewUserNameEventArgs(string userName)
        {
            this.newUserName = userName;
        }

        public string parametersUserName
        {
            get { return newUserName; }
            set { this.newUserName = value; }
        }
    }
}
