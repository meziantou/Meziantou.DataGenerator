using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Meziantou.DataGenerator.Design
{
    /// <summary>
    ///        
    /// </summary>
    /// <example>
    ///     <Viewbox Width="200" Height="200" HorizontalAlignment="Center" VerticalAlignment="Center">
    ///         <local:CircularProgressBar />
    ///     </Viewbox>
    /// </example>
    public partial class CircularProgressBar : UserControl
    {
        private readonly DispatcherTimer _animationTimer;
        private int _bulletCount = 8;
        private List<Ellipse> _bullets = new List<Ellipse>();

        public CircularProgressBar()
        {
            InitializeComponent();

            /*
             * <Ellipse Width="20" 
             *          Height="20"
             *          Stretch="Fill"
             *          Fill="Black" 
             *          Opacity="1.0"/> 
             */

            var opacityDiff = (1.0 - 0.2) / _bulletCount;

            for (int i = 0; i < _bulletCount; i++)
            {
                var ellipse = new Ellipse();
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Stretch = Stretch.Fill;
                ellipse.Fill = Brushes.Black;
                ellipse.Opacity = 1.0 - (opacityDiff * (double)i);

                _bullets.Add(ellipse);
                BulletCanvas.Children.Add(ellipse);
            }

            _animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher);
            _animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 75);
        }
        
        private void Start()
        {
            _animationTimer.Tick += OnAnimationTick;
            _animationTimer.Start();
        }

        private void Stop()
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= OnAnimationTick;
        }

        private void OnAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            for (int i = 0; i < _bulletCount; i++)
            {
                SetPosition(_bullets[i], offset, i, step);
            }
        }

        private static void SetPosition(DependencyObject ellipse, double offset, double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0 + Math.Sin(offset + posOffSet * step) * 50.0);
            ellipse.SetValue(Canvas.TopProperty, 50 + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            if (isVisible)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }
    }
}

