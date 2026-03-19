using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircularSliderControl
{
    public class CircularSlider : Control
    {
        static CircularSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularSlider),
                new FrameworkPropertyMetadata(typeof(CircularSlider)));
        }

        #region 路由事件
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>), typeof(CircularSlider));

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion

        #region 依赖属性
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(CircularSlider),
                new PropertyMetadata(0.0, OnVisualChanged, CoerceAngle));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CircularSlider),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValuePropertyChanged, CoerceValue));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(CircularSlider),
                new PropertyMetadata(0.0, OnRangeChanged, CoerceMinimum));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(CircularSlider),
                new PropertyMetadata(360.0, OnRangeChanged, CoerceMaximum));

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularSlider),
                new PropertyMetadata(0.0, OnAngleChanged, CoerceAngle));

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(CircularSlider),
                new PropertyMetadata(360.0, OnAngleChanged, CoerceAngle));

        public static readonly DependencyProperty TrackThicknessProperty =
            DependencyProperty.Register("TrackThickness", typeof(double), typeof(CircularSlider),
                new PropertyMetadata(8.0, OnVisualChanged));

        public static readonly DependencyProperty ThumbSizeProperty =
            DependencyProperty.Register("ThumbSize", typeof(double), typeof(CircularSlider),
                new PropertyMetadata(20.0, OnVisualChanged));

        public static readonly DependencyProperty TrackBrushProperty =
            DependencyProperty.Register("TrackBrush", typeof(Brush), typeof(CircularSlider),
                new PropertyMetadata(Brushes.LightGray, OnVisualChanged));

        public static readonly DependencyProperty ProgressBrushProperty =
            DependencyProperty.Register("ProgressBrush", typeof(Brush), typeof(CircularSlider),
                new PropertyMetadata(Brushes.DodgerBlue, OnVisualChanged));

        public static readonly DependencyProperty ThumbBrushProperty =
            DependencyProperty.Register("ThumbBrush", typeof(Brush), typeof(CircularSlider),
                new PropertyMetadata(Brushes.White, OnVisualChanged));
        #endregion

        #region 属性包装器
        public double RotationAngle
        {
            get => (double)GetValue(RotationAngleProperty);
            set => SetValue(RotationAngleProperty, value);
        }
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }
        public double EndAngle
        {
            get => (double)GetValue(EndAngleProperty);
            set => SetValue(EndAngleProperty, value);
        }
        public double TrackThickness
        {
            get => (double)GetValue(TrackThicknessProperty);
            set => SetValue(TrackThicknessProperty, value);
        }
        public double ThumbSize
        {
            get => (double)GetValue(ThumbSizeProperty);
            set => SetValue(ThumbSizeProperty, value);
        }
        public Brush TrackBrush
        {
            get => (Brush)GetValue(TrackBrushProperty);
            set => SetValue(TrackBrushProperty, value);
        }
        public Brush ProgressBrush
        {
            get => (Brush)GetValue(ProgressBrushProperty);
            set => SetValue(ProgressBrushProperty, value);
        }
        public Brush ThumbBrush
        {
            get => (Brush)GetValue(ThumbBrushProperty);
            set => SetValue(ThumbBrushProperty, value);
        }
        #endregion

        #region 回调方法
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (CircularSlider)d;
            var args = new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue)
            {
                RoutedEvent = ValueChangedEvent
            };
            slider.RaiseEvent(args);
            slider.UpdateVisual();
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (CircularSlider)d;
            CoerceValue(d, slider.Value);
            slider.UpdateVisual();
        }

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (CircularSlider)d;
            slider.UpdateVisual();
        }

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (CircularSlider)d;
            slider.UpdateVisual();
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var slider = (CircularSlider)d;
            var value = (double)baseValue;
            return Math.Max(slider.Minimum, Math.Min(slider.Maximum, value));
        }

        private static object CoerceMinimum(DependencyObject d, object baseValue)
        {
            var slider = (CircularSlider)d;
            var minimum = (double)baseValue;
            return Math.Min(minimum, slider.Maximum);
        }

        private static object CoerceMaximum(DependencyObject d, object baseValue)
        {
            var slider = (CircularSlider)d;
            var maximum = (double)baseValue;
            return Math.Max(maximum, slider.Minimum);
        }

        private static object CoerceAngle(DependencyObject d, object baseValue)
        {
            var angle = (double)baseValue;
            angle = angle % 360;
            if (angle < 0) angle += 360;
            return angle;
        }
        #endregion

        #region 控件部件
        private Path? _trackPath;
        private Path? _progressPath;
        private Ellipse? _thumb;
        private bool _isDragging;
        private Point _center;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _trackPath = GetTemplateChild("PART_Track") as Path;
            _progressPath = GetTemplateChild("PART_Progress") as Path;
            _thumb = GetTemplateChild("PART_Thumb") as Ellipse;

            if (_thumb != null)
            {
                _thumb.MouseLeftButtonDown -= OnThumbMouseDown;
                _thumb.MouseMove -= OnThumbMouseMove;
                _thumb.MouseLeftButtonUp -= OnThumbMouseUp;

                _thumb.MouseLeftButtonDown += OnThumbMouseDown;
                _thumb.MouseMove += OnThumbMouseMove;
                _thumb.MouseLeftButtonUp += OnThumbMouseUp;
            }

            this.MouseLeftButtonDown -= OnSliderMouseDown;
            this.MouseMove -= OnSliderMouseMove;
            this.MouseLeftButtonUp -= OnSliderMouseUp;

            this.MouseLeftButtonDown += OnSliderMouseDown;
            this.MouseMove += OnSliderMouseMove;
            this.MouseLeftButtonUp += OnSliderMouseUp;

            UpdateVisual();
        }
        #endregion

        #region 鼠标事件处理
        private void OnThumbMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _thumb?.CaptureMouse();
            e.Handled = true;
        }

        private void OnThumbMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                UpdateValueFromMousePosition(e.GetPosition(this));
            }
        }

        private void OnThumbMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _thumb?.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void OnSliderMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            this.CaptureMouse();
            UpdateValueFromMousePosition(e.GetPosition(this));
            e.Handled = true;
        }

        private void OnSliderMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && !(_thumb?.IsMouseCaptured == true))
            {
                UpdateValueFromMousePosition(e.GetPosition(this));
            }
        }

        private void OnSliderMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            this.ReleaseMouseCapture();
            e.Handled = true;
        }

        /// <summary>
        /// 根据鼠标位置更新Value（修正边界处理，超出范围时停在边界）
        /// </summary>
        private void UpdateValueFromMousePosition(Point mousePos)
        {
            var radius = Math.Min(ActualWidth, ActualHeight) / 2 - TrackThickness / 2;
            if (radius <= 0) return;

            _center = new Point(ActualWidth / 2, ActualHeight / 2);

            // 计算鼠标相对于圆心的向量
            var vector = new Point(mousePos.X - _center.X, mousePos.Y - _center.Y);

            // 计算WPF坐标系角度（0度在3点钟，顺时针）
            var angleWpf = Math.Atan2(vector.Y, vector.X) * 180 / Math.PI;
            if (angleWpf < 0) angleWpf += 360;

            // 转换为我们的坐标系：12点钟为0度，顺时针
            double ourAngle = (angleWpf + 90) % 360;

            // 应用旋转角度（减去旋转角，得到在未旋转坐标系中的角度）
            double rawAngle = (ourAngle - RotationAngle) % 360;
            if (rawAngle < 0) rawAngle += 360;

            // 定义有效角度范围（未旋转时）
            double start = StartAngle;
            double end = EndAngle;

            // 确保范围连续（end在start之后）
            if (end < start) end += 360;
            double sweep = end - start;
            if (sweep <= 0) sweep = 360; // 完整圆

            // 计算归一化值
            double normalized;

            if (sweep >= 360)
            {
                // 完整圆：角度直接映射到值
                normalized = rawAngle / 360.0;
            }
            else
            {
                // 处理非完整圆的情况
                // 将rawAngle调整到[start, end]范围内进行比较
                double adjustedAngle = rawAngle;

                // 判断鼠标是否在有效范围内
                bool isInRange;

                if (start <= 360 && end <= 360)
                {
                    // 正常范围（不跨0度）
                    isInRange = adjustedAngle >= start && adjustedAngle <= end;
                }
                else
                {
                    // 跨0度范围（如 350° 到 30°）
                    double startMod = start % 360;
                    double endMod = end % 360;

                    if (adjustedAngle >= startMod || adjustedAngle <= endMod)
                    {
                        isInRange = true;
                        // 调整角度以便计算归一化值
                        if (adjustedAngle < startMod)
                            adjustedAngle += 360;
                    }
                    else
                    {
                        isInRange = false;
                    }
                }

                if (isInRange)
                {
                    // 鼠标在有效范围内，计算归一化值
                    normalized = (adjustedAngle - start) / sweep;
                }
                else
                {
                    // 鼠标超出有效范围，保持当前值不变
                    return;
                }
            }

            // 确保normalized在[0,1]内
            normalized = Math.Max(0, Math.Min(1, normalized));

            // 计算新值
            double newValue = Minimum + normalized * (Maximum - Minimum);

            // 应用值（通过SetCurrentValue避免破坏绑定）
            SetCurrentValue(ValueProperty, newValue);
        }
        #endregion

        #region 绘制方法
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            if (_trackPath == null || _progressPath == null || _thumb == null)
                return;
            if (ActualWidth <= 0 || ActualHeight <= 0)
                return;

            double radius = Math.Max(Math.Min(ActualWidth, ActualHeight) / 2 - TrackThickness / 2, 1);
            _center = new Point(ActualWidth / 2, ActualHeight / 2);

            UpdateTrack(radius);
            UpdateProgress(radius);
            UpdateThumbPosition(radius);
        }

        private void UpdateTrack(double radius)
        {
            if (_trackPath == null) return;

            double start = (StartAngle + RotationAngle) % 360;
            double end = (EndAngle + RotationAngle) % 360;
            if (end < start) end += 360;
            double sweep = end - start;
            if (sweep <= 0) sweep = 360;

            double startRad = (start - 90) * Math.PI / 180;
            double endRad = (end - 90) * Math.PI / 180;

            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                var startPt = new Point(_center.X + radius * Math.Cos(startRad), _center.Y + radius * Math.Sin(startRad));
                ctx.BeginFigure(startPt, false, false);
                ctx.ArcTo(
                    new Point(_center.X + radius * Math.Cos(endRad), _center.Y + radius * Math.Sin(endRad)),
                    new Size(radius, radius), 0, sweep > 180, SweepDirection.Clockwise, true, false);
            }
            _trackPath.Data = geo;
        }

        private void UpdateProgress(double radius)
        {
            if (_progressPath == null) return;

            double norm = Math.Max(0, Math.Min(1, (Value - Minimum) / (Maximum - Minimum)));

            double start = (StartAngle + RotationAngle) % 360;
            double end = (EndAngle + RotationAngle) % 360;
            if (end < start) end += 360;
            double sweep = end - start;
            if (sweep <= 0) sweep = 360;

            double current = start + norm * sweep;
            double startRad = (start - 90) * Math.PI / 180;
            double currRad = (current - 90) * Math.PI / 180;
            double currSweep = current - start;

            if (norm < 0.001)
            {
                _progressPath.Data = null;
                return;
            }

            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                var startPt = new Point(_center.X + radius * Math.Cos(startRad), _center.Y + radius * Math.Sin(startRad));
                ctx.BeginFigure(startPt, false, false);
                ctx.ArcTo(
                    new Point(_center.X + radius * Math.Cos(currRad), _center.Y + radius * Math.Sin(currRad)),
                    new Size(radius, radius), 0, currSweep > 180, SweepDirection.Clockwise, true, false);
            }
            _progressPath.Data = geo;
        }

        private void UpdateThumbPosition(double radius)
        {
            if (_thumb == null) return;

            double norm = Math.Max(0, Math.Min(1, (Value - Minimum) / (Maximum - Minimum)));

            double start = (StartAngle + RotationAngle) % 360;
            double end = (EndAngle + RotationAngle) % 360;
            if (end < start) end += 360;
            double sweep = end - start;
            if (sweep <= 0) sweep = 360;

            double current = start + norm * sweep;
            double rad = (current - 90) * Math.PI / 180;

            var pos = new Point(_center.X + radius * Math.Cos(rad), _center.Y + radius * Math.Sin(rad));
            Canvas.SetLeft(_thumb, pos.X - ThumbSize / 2);
            Canvas.SetTop(_thumb, pos.Y - ThumbSize / 2);
        }
        #endregion
    }
}