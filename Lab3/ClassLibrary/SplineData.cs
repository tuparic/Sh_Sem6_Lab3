using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClassLibrary
{
    public class SplineData
    {
        public int nGrid { get; set; }
        public RawData rawData { get; set; }
        public double [] dif2 { get; set; }
        public double integral { get; set; }
        public List<SplineDataItem> data { get; set; }

        public SplineData (RawData rawData, double lefty2, double righty2, int nGrid)
        {
            if (nGrid < 2)
                throw new Exception("Not enough points in SplineData");
            this.rawData = rawData;
            this.nGrid = nGrid;
            this.dif2 = new double[2] {lefty2, righty2 };
            data = new List<SplineDataItem>();
        }

        [DllImport("..\\..\\..\\..\\..\\Lab3\\x64\\Debug\\Dll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int spline_dll(int n, int n_1, double[] init_NU_grid, double[] val_init_NU_grid,
                                              double[] dif_2, double[] st_end, double[] val_res,
                                              double[] spline_coef, double[] integral, ref int code);

        public bool spline()
        {
            double[] spline_coef = new double[1 * 4 * (rawData.nRawNodes - 1)];
            double[] integral_1 = new double[1];

            double[] x = new double[2];
            x[0] = rawData.leftEnd;
            x[1] = rawData.rightEnd;
            double[] y = new double[3 * nGrid];


            int code = 0;
            int res = spline_dll(rawData.nRawNodes, nGrid, rawData.x, rawData.y,
                                    dif2, x, y, spline_coef, integral_1, ref code);

            if (res == 0)
            {
                for (int i = 0; i < nGrid; i++)
                    data.Add(new SplineDataItem(rawData.leftEnd + i * (rawData.rightEnd - rawData.leftEnd) / (nGrid - 1),
                                y[3 * i], y[3 * i + 1], y[3 * i + 2]));
                integral = integral_1[0];
            }
            else
            {
                throw new Exception("Error in spline interpolation\n");
            }
            return true;
        }
    }
}
