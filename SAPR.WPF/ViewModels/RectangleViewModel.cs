using SAPR.WPF.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SAPR.WPF.ViewModels
{
    public class RectangleViewModel : INotifyPropertyChanged
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<RectangleViewModel>();
        public ObservableCollection<Rectangle> InputRectangles { get; set; }
        public ObservableCollection<RectangleViewModel> DisplayRectangles { get; set; }
        public ObservableCollection<Color> FilterColors { get; set; }
        public ICommand AddMainRectangleCommand { get; private set; }
        public ICommand AddSecondRectanglesCommand { get; private set; }
        public ICommand ResizeMainRectangleCommand { get; private set; }
        public ICommand ClearRectanglesCommand { get; private set; }
        public ICommand AddColorCommand { get; private set; }
        public ICommand RemoveColorCommand { get; private set; }

        private double _xTopLeft;
        private double _yTopLeft;
        private double _xTopRight;
        private double _yTopRight;
        private double _xBottomLeft;
        private double _yBottomLeft;
        private double _xBottomRight;
        private double _yBottomRight;
        private double _left;
        private double _top;
        private double _width;
        private double _height;
        private int _zIndex;
        private bool _isRegularBehaviorChecked;
        private bool _isExcludeOutsideMainChecked;
        private bool _isByColorChecked;

        public bool IsRegularBehaviorChecked
        {
            get => _isRegularBehaviorChecked;
            set
            {
                if (SetProperty(ref _isRegularBehaviorChecked, value))
                {
                    OnPropertyChanged(nameof(IsExcludeOutsideMainEnabled));
                }
            }
        }

        public bool IsExcludeOutsideMainChecked
        {
            get => _isExcludeOutsideMainChecked;
            set
            {
                if (SetProperty(ref _isExcludeOutsideMainChecked, value))
                {
                    OnPropertyChanged(nameof(IsRegularBehaviorEnabled));
                }
            }
        }

        public bool IsByColorChecked
        {
            get => _isByColorChecked;
            set => SetProperty(ref _isByColorChecked, value);
        }

        public bool IsRegularBehaviorEnabled => !IsExcludeOutsideMainChecked;
        public bool IsExcludeOutsideMainEnabled => !IsRegularBehaviorChecked;

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double Left
        {
            get => _left;
            set => SetProperty(ref _left, value);
        }

        public double Top
        {
            get => _top;
            set => SetProperty(ref _top, value);
        }

        public double TopLeftX
        {
            get => _xTopLeft;
            set => SetProperty(ref _xTopLeft, value);
        }
        public double TopLeftY
        {
            get => _yTopLeft;
            set => SetProperty(ref _yTopLeft, value);
        }
        public double TopRightX
        {
            get => _xTopRight;
            set => SetProperty(ref _xTopRight, value);
        }
        public double TopRightY
        {
            get => _yTopRight;
            set => SetProperty(ref _yTopRight, value);
        }
        public double BottomRightX
        {
            get => _xBottomRight;
            set => SetProperty(ref _xBottomRight, value);
        }
        public double BottomRightY
        {
            get => _yBottomRight;
            set => SetProperty(ref _yBottomRight, value);
        }
        public double BottomLeftX
        {
            get => _xBottomLeft;
            set => SetProperty(ref _xBottomLeft, value);
        }
        public double BottomLeftY
        {
            get => _yBottomLeft;
            set => SetProperty(ref _yBottomLeft, value);
        }

        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
        }

        private Color _color;
     

        public Color ColorInput
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(ColorInput));
            }
        }
        private Color _colorSelect;

        public Color ColorSelect
        {
            get => _colorSelect;
            set
            {
                if (_colorSelect != value)
                {
                    _colorSelect = value;
                    OnPropertyChanged(nameof(ColorSelect));
                }
            }
        }

        public Brush Color { get; set; }
        public Point BottomLeft { get; set; }
        public bool IsMain {  get; set; }

        public RectangleViewModel()
        {
            InputRectangles = new ObservableCollection<Rectangle>();
            DisplayRectangles = new ObservableCollection<RectangleViewModel>();
            FilterColors = new ObservableCollection<Color>();
            AddSecondRectanglesCommand = new RelayCommand(param => AddSecondRectangles());
            AddMainRectangleCommand = new RelayCommand(param => AddMainRectangles());
            ResizeMainRectangleCommand = new RelayCommand(param =>  ResizeMainRectangle());
            ClearRectanglesCommand = new RelayCommand(param => ClearRectangles());
            AddColorCommand = new RelayCommand(param => AddColor());
            RemoveColorCommand = new RelayCommand(param => RemoveColor());
        }

        private void AddSecondRectangles()
        {
            AddRectangle(false);           
        }

        private void AddMainRectangles()
        {
            AddRectangle(true);
        }

        int count = 0;
        private void CastRect(Rectangle rectangle, bool isMain)
        {
            var rectViewModel = new RectangleViewModel
            {
                Color = new SolidColorBrush(rectangle.Color),
                Width = Math.Abs(rectangle.TopRight.X - rectangle.TopLeft.X),
                Height = Math.Abs(rectangle.BottomLeft.Y - rectangle.TopLeft.Y),
                BottomLeft = rectangle.BottomLeft,
                IsMain = isMain,
                ZIndex= count+1,
            };

            rectViewModel.Left = rectangle.TopLeft.X;
            rectViewModel.Top = rectangle.TopLeft.Y;

            DisplayRectangles.Add(rectViewModel);
        }
        private void AddRectangle(bool isMain)
        {
            IsMain = isMain;
            if (!DisplayRectangles.Any(r => r.IsMain) || isMain == false)
            {
                if (TopRightX == 0 && TopRightY == 0 && BottomLeftX == 0 && BottomLeftY == 0)
                {
                    var newRectangle = new Rectangle(
                        new Point(TopLeftX,TopLeftY), 
                        new Point(BottomRightX, BottomRightY),
                        ColorInput, isMain);

                    InputRectangles.Add(newRectangle);
                    CastRect(newRectangle, IsMain);
                }
                else
                {
                    var newRectangle = new Rectangle(
                        new Point(TopLeftX, TopLeftY), 
                        new Point(BottomRightX, BottomRightY),
                        new Point(TopRightX, TopRightY),
                        new Point(BottomLeftX, BottomLeftY),
                        ColorInput, isMain);
                    InputRectangles.Add(newRectangle);
                    CastRect(newRectangle, IsMain);
                }
            }
        }

        private void ResizeMainRectangle()
        {
            if (IsRegularBehaviorChecked && !IsByColorChecked)
            {
                RegularResize();
            }
            else if (IsExcludeOutsideMainChecked && !IsByColorChecked)
            {
                ResizeExcludingOutside();
            }
            else if (IsByColorChecked && (IsRegularBehaviorChecked && IsByColorChecked) && !IsExcludeOutsideMainChecked)
            {
                ColorResize();
            } 
            else if(IsByColorChecked && IsExcludeOutsideMainChecked)
            {
                ColorInsideResize();
            }
        }

        private void ResizeExcludingOutside()
        {
            var mainRectInput = InputRectangles.FirstOrDefault(r => r.IsMain);

            if (mainRectInput == null) return;

            var pointsInside = new List<Point>();

            foreach (var rect in InputRectangles.Where(r => !r.IsMain))
            {
                if (IsPointInside(mainRectInput, rect.TopLeft))
                    pointsInside.Add(rect.TopLeft);
                if (IsPointInside(mainRectInput, rect.TopRight))
                    pointsInside.Add(rect.TopRight);
                if (IsPointInside(mainRectInput, rect.BottomLeft))
                    pointsInside.Add(rect.BottomLeft);
                if (IsPointInside(mainRectInput, rect.BottomRight))
                    pointsInside.Add(rect.BottomRight);
            }

            if (pointsInside.Count == 0) return;

            var minX = pointsInside.Min(p => p.X);
            var minY = pointsInside.Min(p => p.Y);
            var maxX = pointsInside.Max(p => p.X);
            var maxY = pointsInside.Max(p => p.Y);

            var newRectangle = new Rectangle(
                new Point(minX, minY),
                new Point(maxX, maxY),
                new Point(maxX, minY),
                new Point(minX, maxY),
                ColorInput, true);

            var mainRect = DisplayRectangles.FirstOrDefault(r => r.IsMain);
            if (mainRect != null)
            {
                DisplayRectangles.Remove(mainRect);
            }
            CastRect(newRectangle, true);
            OnPropertyChanged(nameof(DisplayRectangles));
        }

        private void RegularResize()
        {
            if (InputRectangles.Count>0)
            {
                var minX = InputRectangles.Where(r => !r.IsMain).Min(r => r.TopLeft.X);
                var minY = InputRectangles.Where(r => !r.IsMain).Min(r => r.TopLeft.Y);
                var maxX = InputRectangles.Where(r => !r.IsMain).Max(r => r.BottomRight.X);
                var maxY = InputRectangles.Where(r => !r.IsMain).Max(r => r.BottomRight.Y);

                var mainRect = DisplayRectangles.FirstOrDefault(r => r.IsMain);
                var secondaryRects = DisplayRectangles.Where(r =>  !r.IsMain);

                Logger.Information("До изменения:");
                LogRectangles(mainRect, secondaryRects);

                if (mainRect != null)
                {
                    var newRectangle = new Rectangle(
                        new Point(minX, minY),
                        new Point(maxX, maxY), 
                        new Point(maxX, minY),
                        new Point(minX, maxY),
                        ColorInput, true);
                    CastRect(newRectangle, true);
                    DisplayRectangles.Remove(mainRect);
                    var newMainRect = DisplayRectangles.FirstOrDefault(mainRect => mainRect.IsMain);
                    Logger.Information("После изменения:");
                    LogRectangles(newMainRect, secondaryRects);

                    OnPropertyChanged(nameof(DisplayRectangles));
                }
            }        
        }

        private bool IsPointInside(Rectangle rect, Point point)
        {
            return point.X >= rect.BottomLeft.X && point.X <= rect.TopRight.X &&
                   point.Y >= rect.TopRight.Y && point.Y <= rect.BottomLeft.Y;
        }

        private void ColorResize()
        {
            if(InputRectangles.Count > 0)
            {
                var filteredRectangles = InputRectangles
                                    .Where(r => !r.IsMain && FilterColors.Contains(r.Color))
                                    .ToList();

                if (filteredRectangles.Any())
                {
                    var minX = filteredRectangles.Min(r => r.TopLeft.X);
                    var minY = filteredRectangles.Min(r => r.TopLeft.Y);
                    var maxX = filteredRectangles.Max(r => r.BottomRight.X);
                    var maxY = filteredRectangles.Max(r => r.BottomRight.Y);

                    var mainRect = DisplayRectangles.FirstOrDefault(r => r.IsMain);
                    var secondaryRects = DisplayRectangles.Where(r => !r.IsMain);
                    Logger.Information("До изменения:");
                    LogRectangles(mainRect, secondaryRects);

                    if (mainRect != null)
                    {
                        var newRectangle = new Rectangle(
                            new Point(minX, minY),
                            new Point(maxX, maxY),
                            new Point(maxX, minY),
                            new Point(minX, maxY),
                            ColorInput, true);

                        CastRect(newRectangle, true);
                        DisplayRectangles.Remove(mainRect);
                        var newMainRect = DisplayRectangles.FirstOrDefault(r => r.IsMain);
                        Logger.Information("После изменения:");
                        LogRectangles(newMainRect, secondaryRects);

                        OnPropertyChanged(nameof(DisplayRectangles));
                    }
                }
            }          
        }

        private void ColorInsideResize()
        {
            if(InputRectangles.Count > 0)
            {
                var mainRectInput = InputRectangles.FirstOrDefault(r => r.IsMain);
                if (mainRectInput == null) return;

                var pointsInside = new List<Point>();
                var filteredRects = InputRectangles.
                    Where(r => !r.IsMain && FilterColors.
                    Contains(r.Color)).ToList();

                foreach (var rect in filteredRects)
                {
                    if (IsPointInside(mainRectInput, rect.TopLeft))
                        pointsInside.Add(rect.TopLeft);
                    if (IsPointInside(mainRectInput, rect.TopRight))
                        pointsInside.Add(rect.TopRight);
                    if (IsPointInside(mainRectInput, rect.BottomLeft))
                        pointsInside.Add(rect.BottomLeft);
                    if (IsPointInside(mainRectInput, rect.BottomRight))
                        pointsInside.Add(rect.BottomRight);
                }

                if (pointsInside.Count == 0) return;

                var minX = pointsInside.Min(p => p.X);
                var minY = pointsInside.Min(p => p.Y);
                var maxX = pointsInside.Max(p => p.X);
                var maxY = pointsInside.Max(p => p.Y);

                var newRectangle = new Rectangle(
                    new Point(minX, minY),
                    new Point(maxX, maxY),
                    new Point(maxX, minY),
                    new Point(minX, maxY),
                    mainRectInput.Color, true);

                var mainRect = DisplayRectangles.FirstOrDefault(r => r.IsMain);
                var secondaryRects = DisplayRectangles.Where(r => !r.IsMain);

                Logger.Information("До изменения:");
                LogRectangles(mainRect, secondaryRects);

                if (mainRect != null)
                {
                    DisplayRectangles.Remove(mainRect);
                }

                CastRect(newRectangle, true);
                var newMainRect = DisplayRectangles.FirstOrDefault(r => r.IsMain);

                Logger.Information("После изменения:");
                LogRectangles(newMainRect, secondaryRects);

                OnPropertyChanged(nameof(DisplayRectangles));
            }        
        }

        private void AddColor()
        {
            if (ColorSelect != null && !FilterColors.Contains(ColorSelect))
            {
                FilterColors.Add(ColorSelect);
            }         
        }
        private void RemoveColor()
        {
            if (FilterColors.Count > 0 && FilterColors.Contains(ColorSelect))
            {
                FilterColors.Remove(ColorSelect);
            }
        }

        private void ClearRectangles()
        {
            DisplayRectangles.Clear();
            InputRectangles.Clear();
            OnPropertyChanged(nameof(DisplayRectangles));
        }

        private void LogRectangles(RectangleViewModel mainRect, IEnumerable<RectangleViewModel> secondaryRects)
        {          
            Logger.Information("Главный прямоугольник:");
            Logger.Information($"Координаты: {mainRect.Top}, {mainRect.Left}");
            Logger.Information($"Ширина: {mainRect.Width}, Высота: {mainRect.Height}");
            Logger.Information($"Цвет: {mainRect.Color}");

            Logger.Information("Второстепенные прямоугольники:");
            foreach (var rect in secondaryRects)
            {
                Logger.Information($"Прямоугольник:");
                Logger.Information($"Координаты: {rect.Top}, {rect.Left}");
                Logger.Information($"Ширина: {rect.Width}, Высота: {rect.Height}");
                Logger.Information($"Цвет: {rect.Color}");
            }
        }


        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
