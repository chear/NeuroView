///author: Chear
///Location:China.Wuxi
///Describ: make the replay the record files without connect BT device
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuroSky.MindView
{
    public class ReplayLoadData
    {
        public delegate void DisplayRecordDelegate(List<short> loadedList,Launcher lanucher);
        public delegate void StopDisplayRecordDelegate(object obj);

        private List<short> _localList;
        public List<short> LocalList{
            set { _localList = value; }
        }

        private Launcher _launch;
        public Launcher Sender
        {
            set { _launch = value; }
        }

        private int loadedIndex;
        private List<double> tempLoadedDataBuffer;
        int bufferSize_hp = 1009;
        private double[] loadedDataBuffer;
        private double filtered;
        private int delay;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public ReplayLoadData()
        { 
            delay = 0;
            loadedDataBuffer = new double[bufferSize_hp];
            tempLoadedDataBuffer = new List<double>();

            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);
        }


        public double applyFilter(double[] data, double[] coeffs)
        {
            int length = data.Length;
            double result = new double();

            for (int i = 0; i < length; i++)
            {
                result += data[i] * coeffs[i];
            }
            return result;
        }
         
        private void timer_Tick(object sender, EventArgs e)
        {
            
        }

        public void DisplayRecord(List<short> loadedList, Launcher launch)
        {
            while (loadedIndex < loadedList.Count)
            {
                //loadedDataBuffer[loadedIndex] = loadedList[loadedIndex];
                tempLoadedDataBuffer.Add(loadedList[loadedIndex]);
                loadedIndex++;
                //Console.WriteLine("NeuroSky.MindView loadedIndex=" + loadedIndex);
                System.Threading.Thread.Sleep(1);
                //when buffer is full, plotting
                if (loadedIndex >= bufferSize_hp)
                {
                    //copy data
                    for (int m = 0; m < tempLoadedDataBuffer.Count; m++)
                    {
                        loadedDataBuffer[m] = tempLoadedDataBuffer[m];
                    }

                    //filter the data
                    filtered = applyFilter(loadedDataBuffer, launch.hp_coeff);
                    //if "delay" seconds have passed, start plotting the data

                    lock (launch.mainForm.rawGraphPanel.LineGraph)
                    {
                        if (loadedIndex >= delay)
                        {
                            //Console.WriteLine("NeuroSky.MindView loadedIndex=" + loadedIndex);
                            launch.mainForm.rawGraphPanel.LineGraph.Add(new DataPair((launch.mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)launch.mainForm.rawGraphPanel.LineGraph.samplingRate), filtered));
                            launch.mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                        }

                        //clear the graph when it's full
                        if (launch.mainForm.rawGraphPanel.LineGraph.timeStampIndex >= launch.mainForm.rawGraphPanel.LineGraph.numberOfPoints)
                        {
                            launch.mainForm.rawGraphPanel.LineGraph.Clear();
                        }
                        launch.mainForm.rawGraphPanel.LineGraph.Invalidate();
                        //shift buffer window 
                        tempLoadedDataBuffer.RemoveAt(0);
                    }
                }

            }
            Console.WriteLine("ReplayLoadData Over:loadedIndex = " + loadedIndex);
        }

        bool isStop = false;
        public void DisplayRecord()
        {
            while (loadedIndex < _localList.Count&&!isStop)
            {
                //loadedDataBuffer[loadedIndex] = loadedList[loadedIndex];
                tempLoadedDataBuffer.Add(_localList[loadedIndex]);
                loadedIndex++;
                //Console.WriteLine("NeuroSky.MindView loadedIndex=" + loadedIndex);
                System.Threading.Thread.Sleep(1);
                //when buffer is full, plotting
                if (loadedIndex >= bufferSize_hp)
                {
                    //copy data
                    for (int m = 0; m < tempLoadedDataBuffer.Count; m++)
                    {
                        loadedDataBuffer[m] = tempLoadedDataBuffer[m];
                    }

                    //filter the data
                    filtered = applyFilter(loadedDataBuffer, _launch.hp_coeff);
                    //if "delay" seconds have passed, start plotting the data

                    lock (_launch.mainForm.rawGraphPanel.LineGraph)
                    {
                        if (loadedIndex >= delay)
                        {
                            //Console.WriteLine("NeuroSky.MindView loadedIndex=" + loadedIndex);
                            _launch.mainForm.rawGraphPanel.LineGraph.Add(new DataPair((_launch.mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)_launch.mainForm.rawGraphPanel.LineGraph.samplingRate), filtered));
                            _launch.mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                        }

                        //clear the graph when it's full
                        if (_launch.mainForm.rawGraphPanel.LineGraph.timeStampIndex >= _launch.mainForm.rawGraphPanel.LineGraph.numberOfPoints)
                        {
                            _launch.mainForm.rawGraphPanel.LineGraph.Clear();
                        }
                        _launch.mainForm.rawGraphPanel.LineGraph.Invalidate();
                        //shift buffer window 
                        tempLoadedDataBuffer.RemoveAt(0);
                    }
                }

            }
            Console.WriteLine("ReplayLoadData Over:loadedIndex = " + loadedIndex);
        }

        public void StopDisplay(object obj)
        {
            isStop = true;
        }
    }
}
