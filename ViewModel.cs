using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SubmarineMotionVisualization
{
    public class ViewModel : ObservableObject
    {
        private SeriesCollection _xAndTCollection;
        private SeriesCollection _yAndTCollection;
        private SeriesCollection _yAndXCollection;
        private SeriesCollection _uAndTCollection;
        private SeriesCollection _vAndTCollection;
        private ChartValues<ObservablePoint> _xAndTValues;
        private ChartValues<ObservablePoint> _yAndTValues;
        private ChartValues<ObservablePoint> _yAndXValues;
        private ChartValues<ObservablePoint> _uAndTValues;
        private ChartValues<ObservablePoint> _vAndTValues;
        private DispatcherTimer _timer;
        private SubmarineMotion _submarineMotionProcess;
        private List<double> _xValues;
        private List<double> _yValues;
        private List<double> _uValues;
        private List<double> _vValues;
        private double _t;

        public ViewModel()
        {
            StartVisualizationCommand = new RelayCommand(StartVisualization, CanStartVisualization);
            StopVisualizationCommand = new RelayCommand(StopVisualization);
            SubmarineMotionProcess = new SubmarineMotion(StartVisualizationCommand);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += DrawGraphs;

            _xAndTValues = new ChartValues<ObservablePoint>();
            _yAndTValues = new ChartValues<ObservablePoint>();
            _yAndXValues = new ChartValues<ObservablePoint>();
            _uAndTValues = new ChartValues<ObservablePoint>();
            _vAndTValues = new ChartValues<ObservablePoint>();

            XAndTCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = _xAndTValues,
                    Title = "X(T)",
                    PointGeometry = null,
                    LabelPoint = point => $"t:{point.X:#.###}, x:{point.Y:#.###}"
                }
            };
            YAndTCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = _yAndTValues,
                    Title = "Y(T)",
                    PointGeometry = null,
                    LabelPoint = point => $"t:{point.X:#.###}, y:{point.Y:#.###}"
                }
            };
            YAndXCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = _yAndXValues,
                    Title = "Y(X)",
                    PointGeometry = null,
                    LabelPoint = point => $"x:{point.X:#.###}, y:{point.Y:#.###}"
                }
            };

            UAndTCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = _uAndTValues,
                    Title = "U(T)",
                    PointGeometry = null,
                    LabelPoint = point => $"t:{point.X:#.###}, u:{point.Y:#.###}"
                }
            };

            VAndTCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = _vAndTValues,
                    Title = "V(T)",
                    PointGeometry = null,
                    LabelPoint = point => $"t:{point.X:#.###}, v:{point.Y:#.###}"
                }
            };
        }


        #region Properties
        public RelayCommand StartVisualizationCommand { get; set; }
        public RelayCommand StopVisualizationCommand { get; set; }
        public SeriesCollection XAndTCollection
        {
            get { return _xAndTCollection; }
            set { _xAndTCollection = value; }
        }
        public SeriesCollection YAndTCollection
        {
            get { return _yAndTCollection; }
            set { _yAndTCollection = value; }
        }
        public SeriesCollection YAndXCollection
        {
            get { return _yAndXCollection; }
            set { _yAndXCollection = value; }
        }
        public SeriesCollection UAndTCollection
        {
            get { return _uAndTCollection; }
            set { _uAndTCollection = value; }
        }
        public SeriesCollection VAndTCollection
        {
            get { return _vAndTCollection; }
            set { _vAndTCollection = value; }
        }
        public SubmarineMotion SubmarineMotionProcess
        {
            get { return _submarineMotionProcess; }
            set { _submarineMotionProcess = value; }
        }

        #endregion

        #region Methods
        private void StartVisualization()
        {
            _t = 0;
            SubmarineMotionProcess.CalculateAllPoints();
            _uValues = SubmarineMotionProcess.GetAllValuesU();
            _vValues = SubmarineMotionProcess.GetAllValuesV();
            _xValues = SubmarineMotionProcess.GetAllValuesX();
            _yValues = SubmarineMotionProcess.GetAllValuesY();
            _timer.Start();
        }

        private bool CanStartVisualization()
        {
            SubmarineMotion sm = SubmarineMotionProcess;

            if (sm.Mass > 0 && sm.Volume > 0 && sm.U0 > 0 && sm.DragCoefficient > 0)
            {
                return true;
            }

            return false;
        }

        private void StopVisualization()
        {
            _timer.Stop();
            _xAndTValues.Clear();
            _yAndTValues.Clear();
            _yAndXValues.Clear();
            _uAndTValues.Clear();
            _vAndTValues.Clear();
            _uValues.Clear();
            _vValues.Clear();
            _xValues.Clear();
            _yValues.Clear();
        }

        private void DrawGraphs(object sender, EventArgs args)
        {
            int t = (int)_t;
            _xAndTValues.Add(new ObservablePoint(_t, _xValues[t]));
            _yAndTValues.Add(new ObservablePoint(_t, _yValues[t]));
            _uAndTValues.Add(new ObservablePoint(_t, _uValues[t]));
            _vAndTValues.Add(new ObservablePoint(_t, _vValues[t]));
            _yAndXValues.Add(new ObservablePoint(_xValues[t], _yValues[t]));
            
            _t++;
            if(_t == _xValues.Count)
            {
                _timer.Stop();
            }
        }
        
        #endregion
    }
}
