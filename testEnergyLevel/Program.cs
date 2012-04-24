using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;


namespace testEnergyLevel {
    class Program {


        int[] rrInMSecs = new int[110] { 853, 867, 855, 832, 828, 873, 892, 923, 937, 921, 873, 849, 873, 855, 894, 968, 962, 906, 904, 931, 845, 822, 761, 841, 771, 859, 832, 871, 777, 664, 1058, 914, 880, 748, 814, 933, 822, 775, 865, 857, 847, 880, 857, 775, 755, 832, 808, 810, 923, 951, 851, 996, 929, 890, 916, 896, 751, 783, 837, 794, 777, 759, 800, 888, 785, 791, 835, 732, 701, 757, 833, 857, 822, 826, 796, 748, 753, 755, 806, 824, 843, 792, 841, 826, 791, 787, 808, 812, 789, 771, 783, 769, 755, 814, 800, 812, 773, 826, 835, 814, 785, 800, 769, 748, 722, 720, 716, 724, 712, 763 };

        EnergyLevel energy;

        private string energyOutFile;
        private System.IO.StreamWriter energyStream;

        public Program() {
            energy = new EnergyLevel();

            energyOutFile = "energy_CS.txt";
            this.energyStream = new System.IO.StreamWriter(energyOutFile, true);
        }

        static void Main(string[] args) {

            Program p = new Program();
            p.simulateData();
        }


        public void simulateData() {

            energy.calculateEnergyLevel(rrInMSecs, 110);


        }











    }
}
