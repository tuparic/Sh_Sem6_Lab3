using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ClassLibrary;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;
using System.Linq;

namespace ViewModel
{
    public interface IUIServices
    {
        void ReportError(string message);
    }
    public partial class ViewData : ViewModelBase, IDataErrorInfo
    {
        public double leftEnd { get; set; }
        public double rightEnd { get; set; }
        public int nRawNodes { get; set; }
        public bool uni { get; set; }
        public int nGrid { get; set; }
        public double leftdif { get; set; }
        public double rightdif { get; set; }

        public List<FRaw> listFRaw { get; set; }
        public FRaw fRaw { get; set; }
        public RawData rawData { get; set; }
        public SplineData splineData { get; set; }

        private readonly IUIServices uiServices;

        public ICommand RunCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public IEnumerable<string> Table_listbox_rawData
        {
            get => rawData != null ? (rawData.x).Zip((rawData.y), (first, second) => to_str(first, second, "##00.000")) : null;
        }

        public string integral_text
        {
            get => splineData != null ? splineData.integral.ToString("##00.000") : null;
        }
            

        public string Error { get { return "Error Text"; } }
        public string this[string property]
        {
            get
            {
                string msg = null;
                switch (property)
                {
                    case "nRawNodes":
                        if (nRawNodes < 2) msg = "Number of elements in RawData must be greater than 2";
                        break;
                    case "nGrid":
                        if (nGrid < 2) msg = "Number of elements in nGrid must be greater than 2";
                        break;
                    case "leftEnd":
                        if (leftEnd >= rightEnd) msg = "leftEnd must be less than rightEnd";
                        break;
                    case "rightEnd":
                        if (leftEnd >= rightEnd) msg = "leftEnd must be less than rightEnd";
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }

        public ViewData(IUIServices uiServices)
        {
            leftEnd = 1;
            rightEnd = 10;
            nRawNodes = 10;
            uni = true;
            nGrid = 10;
            leftdif = 0;
            rightdif = 1;
            listFRaw = new List<FRaw>();
            listFRaw.Add(FRawFunctions.linear);
            listFRaw.Add(FRawFunctions.cube);
            listFRaw.Add(FRawFunctions.rand);
            fRaw = listFRaw[0];
            this.uiServices = uiServices;
            RunCommand = new RelayCommand(ExecuteSplines, Correct_Info);
            LoadCommand = new RelayCommand(Load, Correct_Data);
            SaveCommand = new RelayCommand(Save, Correct_Data);
            init();
        }
        public void ExecuteSplines(object arg)
        {
            try
            {
                init();
                splineData = new SplineData(rawData, leftdif, rightdif, nGrid);
                splineData.spline();
                RaisePropertyChanged(nameof(Table_listbox_rawData));
                RaisePropertyChanged(nameof(splineData));
                RaisePropertyChanged(nameof(integral_text));
                RaisePropertyChanged(nameof(ChartData));
            }
            catch (Exception e)
            {
                uiServices.ReportError(e.Message);
            }
        }


        public void Save(object arg)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    rawData.Save(filename);
                }
            }
            catch (Exception e)
            {
                uiServices.ReportError(e.Message);
            }
        }

        public void Load(object arg)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    rawData = new RawData(filename);
                    leftEnd = rawData.leftEnd;
                    rightEnd = rawData.rightEnd;
                    nRawNodes = rawData.nRawNodes;
                    uni = rawData.uni;
                    fRaw = rawData.fRaw;
                    splineData = null;
                    RaisePropertyChanged(nameof(Table_listbox_rawData));
                    RaisePropertyChanged(nameof(splineData));
                    RaisePropertyChanged(nameof(ChartData));
                    RaisePropertyChanged(nameof(leftEnd));
                    RaisePropertyChanged(nameof(rightEnd));
                    RaisePropertyChanged(nameof(nRawNodes));
                    RaisePropertyChanged(nameof(uni));
                    RaisePropertyChanged(nameof(fRaw));
                }
            }
            catch (Exception e)
            {
                uiServices.ReportError(e.Message);
            }
        }

        public void init()
        {
            try
            {
                rawData = new RawData(leftEnd, rightEnd, nRawNodes, uni, fRaw);
            }
            catch (Exception e)
            {
                uiServices.ReportError(e.Message);
            }
        }

        public string to_str(double x, double y, string format)
        {
            return $"x = {x.ToString(format)}, y = {y.ToString(format)}";
        }

        public PlotModel ChartData
        {
            get
            {
                PlotModel pm = new PlotModel { Title = "Data Chart" };
                pm.Series.Clear();

                pm.Axes.Add(new OxyPlot.Axes.LinearAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                    MaximumPadding = 0.1,
                    MinimumPadding = 0.1,
                    StringFormat = "##0.00",
                    Title = "x"
                });
                pm.Axes.Add(new OxyPlot.Axes.LinearAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    MaximumPadding = 0.1,
                    MinimumPadding = 0.1,
                    StringFormat = "##0.00",
                    Title = "y"
                });

                if (splineData == null)
                    return pm;

                for (int js = 0; js < 2; js++)
                {
                    OxyColor color = (js == 0) ? OxyColors.Transparent : OxyColors.Green;
                    LineSeries lineSeries = new LineSeries();
                    if (js == 0)
                    {
                        for (int j = 0; j < nRawNodes; j++) lineSeries.Points.Add(new DataPoint(rawData.x[j], rawData.y[j]));
                        lineSeries.MarkerType = MarkerType.Circle;
                        lineSeries.MarkerSize = 4;
                        lineSeries.MarkerStroke = OxyColors.Blue;
                        lineSeries.MarkerFill = OxyColors.Blue;
                        lineSeries.Title = "Raw Data";
                    }
                    else
                    {
                        for (int j = 0; j < nGrid; j++) 
                            lineSeries.Points.Add(new DataPoint(splineData.data[j].x, splineData.data[j].y));
                        lineSeries.Title = "Spline Data";
                    }
                    lineSeries.Color = color;
                    Legend legend = new Legend() { LegendPosition = LegendPosition.LeftTop };
                    pm.Legends.Add(legend);
                    pm.Series.Add(lineSeries);
                }
                return pm;
            }
        }

        public bool Correct_Info(object sender)
        {
            if ((this["leftEnd"] != null) || (this["rightEnd"] != null) || (this["nRawNodes"] != null) || 
                (this["nGrid"] != null) || (this["leftdif"] != null) || (this["rightdif"] != null))
                return false;
            else
                return true;
        }

        public bool Correct_Data(object sender)
        {
            string s = CheckData();
            if (s == "")
                return true;
            else
                return false;
        }

        public string CheckData()
        {
            string msg = "";
            if (rawData == null)
                return "RawData is null";

            if (rawData.nRawNodes < 2)
                msg += "\nNumber of elements in RawData must be greater than 2";
            if (rawData.leftEnd >= rawData.rightEnd)
                msg += "\nleftEnd must be less than rightEnd";
            return msg;
        }


        public override string ToString()
        {
            return $"leftEnd = {leftEnd}\n" +
                   $"rightEnd = {rightEnd}\n" +
                   $"nRawNodes = {nRawNodes}\n" +
                   $"uni = {uni}\n" +
                   $"leftdif = {leftdif}\n" +
                   $"rightdif = {rightdif}\n" +
                   $"nGrid = {nGrid}\n" +
                   $"fRaw = {fRaw.Method.Name}\n";
        }
    }
}
