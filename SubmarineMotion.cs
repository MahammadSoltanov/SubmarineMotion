using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineMotionVisualization
{
    public class SubmarineMotion : ObservableObject
    {
        private double? _x0;
        private double? _y0;
        private double? _u0;
        private double? _mass;
        private double? _volume;
        private double? _dragCoefficient;
        private double? _v0;
        private const double _gravity = 9.8;
        private const double _waterDensity = 997;
        private List<double> _xValues;
        private List<double> _yValues;
        private List<double> _uValues;
        private List<double> _vValues;
        private double _h = 0.01;
        private RelayCommand _startVisualizationCommand;
        public SubmarineMotion(RelayCommand startVisualizationCommand)
        {
            _startVisualizationCommand = startVisualizationCommand;
            _xValues = new List<double>();
            _yValues = new List<double>();
            _uValues = new List<double>();
            _vValues = new List<double>();
        }

        #region Properties
        public double? X0
        {
            get { return _x0; }
            set
            {
                _x0 = value;
                OnPropertyChanged();
                _startVisualizationCommand.RaiseCanExecuteChanged();
            }
        }
        public double? Y0
        {
            get { return _y0; }
            set
            {
                _y0 = value;
                OnPropertyChanged();
                _startVisualizationCommand.RaiseCanExecuteChanged();
            }
        }
        public double? Mass
        {
            get { return _mass; }
            set
            {
                if (IsPositive(value))
                {
                    _mass = value;
                    OnPropertyChanged();
                    _startVisualizationCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    throw new ArgumentException("Mass of submarine should be a positive number", nameof(Mass));
                }
            }
        }
        public double? Volume
        {
            get { return _volume; }
            set
            {
                if (IsPositive(value))
                {
                    _volume = value;
                    OnPropertyChanged();
                    _startVisualizationCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    throw new ArgumentException("Volume of submarine should be a positive number", nameof(Volume));
                }
            }
        }
        public double? U0
        {
            get { return _u0; }
            set
            {
                if (IsPositive(value))
                {
                    _u0 = value;
                    OnPropertyChanged();
                    _startVisualizationCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    throw new ArgumentException("Speed of submarine should be a positive number", nameof(U0));
                }
            }
        }

        public double? V0
        {
            get { return _v0; }
            set
            {
                if (IsPositive(value))
                {
                    _v0 = value;
                    OnPropertyChanged();
                    _startVisualizationCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    throw new ArgumentException("Speed of submarine should be a positive number", nameof(V0));
                }
            }
        }
        public double? DragCoefficient
        {
            get { return _dragCoefficient; }
            set
            {
                if (IsPositive(value))
                {
                    _dragCoefficient = value;
                    OnPropertyChanged();
                    _startVisualizationCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    throw new ArgumentException("Drag coefficient of water should be a positive number", nameof(DragCoefficient));
                }
            }
        }
        #endregion
        public List<double> GetAllValuesU()
        {
            return _uValues;
        }
        public List<double> GetAllValuesV()
        {
            return _vValues;
        }
        public List<double> GetAllValuesX()
        {
            return _xValues;
        }
        public List<double> GetAllValuesY()
        {
            return _yValues;
        }

        public void CalculateAllPoints()
        {
            _uValues.Add((double)_u0);
            _vValues.Add((double)_v0);
            _xValues.Add((double)_x0);
            _yValues.Add((double)-_y0);
            int i = 0;
            while (_yValues[i] <= 0)
            {
                double k1u = _h * f1(_uValues[i], _vValues[i]);
                double k1v = _h * f2(_uValues[i], _vValues[i]);
                double k1x = _h * f3(_uValues[i], _vValues[i]);
                double k1y = _h * f4(_uValues[i], _vValues[i]);

                double k2u = _h * f1(_uValues[i] + _h / 2, _vValues[i] + k1u / 2);
                double k2v = _h * f2(_uValues[i] + _h / 2, _vValues[i] + k1v / 2);
                double k2x = _h * f3(_uValues[i] + _h / 2, _vValues[i] + k1x / 2);
                double k2y = _h * f4(_uValues[i] + _h / 2, _vValues[i] + k1y / 2);

                double k3u = _h * f1(_uValues[i] + _h / 2, _vValues[i] + k2u / 2);
                double k3v = _h * f2(_uValues[i] + _h / 2, _vValues[i] + k2v / 2);
                double k3x = _h * f3(_uValues[i] + _h / 2, _vValues[i] + k2x / 2);
                double k3y = _h * f4(_uValues[i] + _h / 2, _vValues[i] + k2y / 2);

                double k4u = _h * f1(_uValues[i] + _h, _vValues[i] + k3u);
                double k4v = _h * f2(_uValues[i] + _h, _vValues[i] + k3v);
                double k4x = _h * f3(_uValues[i] + _h, _vValues[i] + k3x);
                double k4y = _h * f4(_uValues[i] + _h, _vValues[i] + k3y);

                
                _uValues.Add(_uValues[i] + (k1u + 2 * k2u + 2 * k3u + k4u) / 6);
                _vValues.Add(_vValues[i] + (k1v + 2 * k2v + 2 * k3v + k4v) / 6);
                _xValues.Add(_xValues[i] + (k1x + 2 * k2x + 2 * k3x + k4x) / 6);
                _yValues.Add(_yValues[i] + (k1y + 2 * k2y + 2 * k3y + k4y) / 6);
                i++;
            }

        }

        public void ClearAllPoints()
        {
            _uValues.Clear();
            _vValues.Clear();
            _xValues.Clear();
            _yValues.Clear();
        }


        private double f1(double u, double v)
        {
            double m = (double)Mass;
            double k = (double)DragCoefficient;
            return (1 / m) * (-k * (double)Math.Sqrt(u * u + v * v));
        }

        private double f2(double u, double v)
        {
            double m = (double)Mass;
            double k = (double)DragCoefficient;
            double V = (double)Volume;
            return (1 / m) * (_waterDensity * V * _gravity - k * (double)Math.Sqrt(u * u + v * v) - m * _gravity);
        }

        private double f3(double u, double v)
        {
            return u;
        }

        private double f4(double u, double v)
        {
            return v;
        }

        private bool IsPositive(double? num)
        {
            return num >= 0;
        }

    }
}
