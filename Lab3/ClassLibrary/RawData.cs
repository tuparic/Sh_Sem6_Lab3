using System;
using System.Text.Json;
using System.IO;

namespace ClassLibrary
{

    public class RawData
    {
        public double leftEnd { get; set; }
        public double rightEnd { get; set; }
        public int nRawNodes { get; set; }
        public bool uni { get; set; }

        public FRaw fRaw { get; set; }

        public double[] x { get; set; }
        public double[] y { get; set; }


        public RawData (double leftEnd, double rightEnd, int nRawNodes, bool uni, FRaw fRaw, double[] x = null, double[] y = null)
        {
            this.leftEnd = leftEnd;
            this.rightEnd = rightEnd;
            if (nRawNodes < 2)
                throw new Exception("Not enough points in RawData");
            if (leftEnd >= rightEnd)
                throw new Exception("leftEnd must be less than rightEnd");
            this.nRawNodes = nRawNodes;
            this.uni = uni;
            this.fRaw = fRaw;
            if (x == null)
            {
                this.x = new double[nRawNodes];
                if (uni)
                {
                    for (int i = 0; i < nRawNodes; i++)
                        this.x[i] = leftEnd + i * (rightEnd - leftEnd) / (nRawNodes - 1);
                }
                else
                {
                    this.x[0] = leftEnd;
                    this.x[nRawNodes-1] = rightEnd;
                    Random rnd = new Random();
                    for (int i = 1; i < nRawNodes - 1; i++)
                        this.x[i] = rnd.NextDouble() * (rightEnd - leftEnd) + leftEnd;
                    for (int i=1; i < nRawNodes - 1; i++)
                        for (int j = i + 1; j < nRawNodes - 1; j++)
                            if (this.x[i] > this.x[j])
                            {
                                double t = this.x[i];
                                this.x[i] = this.x[j];
                                this.x[j] = t;
                            }
                }
            }
            else
                this.x = x;

            if (y == null)
            {
                this.y = new double[nRawNodes];
                for (int i = 0; i < nRawNodes; i++)
                    this.y[i] = fRaw(this.x[i]);
            }
            else
                this.y = y;


        }
        public RawData(string filename)
        {
            using (StreamReader fs = new StreamReader(filename))
            {
                string s = fs.ReadLine();
                leftEnd = JsonSerializer.Deserialize<float>(s);
                s = fs.ReadLine();
                rightEnd = JsonSerializer.Deserialize<float>(s);
                s = fs.ReadLine();
                nRawNodes = JsonSerializer.Deserialize<int>(s);
                s = fs.ReadLine();
                uni = JsonSerializer.Deserialize<bool>(s);
                s = fs.ReadLine();
                FRawEnum t = JsonSerializer.Deserialize<FRawEnum>(s);
                switch (t)
                {
                    case FRawEnum.linear:
                        fRaw = FRawFunctions.linear;
                        break;
                    case FRawEnum.cube:
                        fRaw = FRawFunctions.cube;
                        break;
                    case FRawEnum.rand:
                        fRaw = FRawFunctions.rand;
                        break;
                }
                s = fs.ReadLine();
                x = JsonSerializer.Deserialize<double[]>(s);
                s = fs.ReadLine();
                y = JsonSerializer.Deserialize<double[]>(s);
                if (nRawNodes < 2)
                    throw new Exception("Not enough points in RawData");
                if (leftEnd >= rightEnd)
                    throw new Exception("leftEnd must be less than rightEnd");
            }
        }



        public void Save(string filename)
        {
            FRawEnum t = FRawEnum.linear;
            switch (fRaw.Method.Name)
            {
                case "linear":
                    t = FRawEnum.linear;
                    break;
                case "cube":
                    t = FRawEnum.cube;
                    break;
                case "rand":
                    t = FRawEnum.rand;
                    break;
            }
            using (StreamWriter fs = new StreamWriter(filename))
            {
                string s = JsonSerializer.Serialize(leftEnd);
                fs.WriteLine(s);
                s = JsonSerializer.Serialize(rightEnd);
                fs.WriteLine(s);
                s = JsonSerializer.Serialize(nRawNodes);
                fs.WriteLine(s);
                s = JsonSerializer.Serialize(uni);
                fs.WriteLine(s);
                s = JsonSerializer.Serialize(t);
                fs.WriteLine(s);
                s = JsonSerializer.Serialize(x);
                fs.WriteLine(s);
                s = JsonSerializer.Serialize(y);
                fs.WriteLine(s);
            }
        }

        public void Load(string filename, out RawData rawData)
        {
            rawData = new RawData(filename);
        }

        public override string ToString()
        {
            return $"leftEnd = {leftEnd} rightEnd = {rightEnd} nNodes = {nRawNodes} fRaw = {fRaw.Method.Name}";
        }
    }
}
