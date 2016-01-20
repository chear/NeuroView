using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WIN_FFT
{
    class complex
    {
        public double real;
        public double imag;

        public complex EE(complex b1, complex b2)   //旋转因子
        {
            complex b3 = new complex();
            b3.real = b1.real * b2.real - b1.imag * b2.imag;
            b3.imag = b1.real * b2.imag + b1.imag * b2.real;
            return (b3);
        }

        public double[] amplitude(complex[] x, int n)
        {

            double temp;
            double[] amp = new double[n];
            for (int i = 0; i < n; i++)
            {
                temp = x[i].real * x[i].real + x[i].imag * x[i].imag;
                amp[i] = Math.Sqrt(temp) / (n / 2);
            }
            return amp;
        }

        public void FFT(complex[] xin, int N)      //*xin为需要FFT变换的数组，N为转换的点数
        {
            int f, m, LH, nm, i, k, j, L;
            double p, ps;
            int le, B, ip;
            float pi;
            complex w = new complex();
            complex t = new complex();

            LH = N >> 1;
            f = N;
            for (m = 1; (f = f >> 1) != 1; m++) { ;}
            nm = N - 2;
            j = N >> 1;

            for (i = 1; i <= nm; i++)        //  变址运算
            {
                if (i < j) { t = xin[j]; xin[j] = xin[i]; xin[i] = t; }
                k = LH;
                while (j >= k) { j = j - k; k = k / 2; }
                j = j + k;
            }

            for (L = 1; L <= m; L++)       //实现蝶形运算
            {
                le = (int)Math.Pow(2, L);
                B = le >> 1;
                pi = 3.14159f;
                for (j = 0; j <= B - 1; j++)
                {
                    p = Math.Pow(2, m - L) * j;
                    ps = 2 * pi / N * p;
                    w.real = Math.Cos(ps);
                    w.imag = -Math.Sin(ps);
                    for (i = j; i <= N - 1; i = i + le)
                    {
                        ip = i + B;
                        t = EE(xin[ip], w);
                        xin[ip].real = xin[i].real - t.real;
                        xin[ip].imag = xin[i].imag - t.imag;
                        xin[i].real = xin[i].real + t.real;
                        xin[i].imag = xin[i].imag + t.imag;
                    }
                }
            }
        }

    }
}
