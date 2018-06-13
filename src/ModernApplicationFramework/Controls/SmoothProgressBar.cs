using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls
{
    public class SmoothProgressBar : ProgressBar
    {
        public static readonly DependencyProperty TargetValueProperty = DependencyProperty.Register(
            "TargetValue", typeof(double), typeof(SmoothProgressBar),
            new FrameworkPropertyMetadata(Boxes.DoubleZero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTargetValueChanged));

        public static readonly DependencyProperty AnimationMaximumDurationProperty = DependencyProperty.Register(
            "AnimationMaximumDuration", typeof(int), typeof(SmoothProgressBar), new FrameworkPropertyMetadata(1000));

        private DateTime _progressStartTime = DateTime.UtcNow;
        private readonly DoubleAnimation _valueAnimation;
        private readonly Storyboard _progressStoryboard;


        public int AnimationMaximumDuration
        {
            get => (int) GetValue(AnimationMaximumDurationProperty);
            set => SetValue(AnimationMaximumDurationProperty, value);
        }

        public double TargetValue
        {
            get => (double) GetValue(TargetValueProperty);
            set => SetValue(TargetValueProperty, value);
        }
           
        public SmoothProgressBar()
        {
            _valueAnimation = new DoubleAnimation();
            Storyboard.SetTarget(_valueAnimation, this);
            Storyboard.SetTargetProperty(_valueAnimation, new PropertyPath(ValueProperty));
            _progressStoryboard = new Storyboard();
            _progressStoryboard.Children.Add(_valueAnimation);
            IsVisibleChanged += UpdateAnaimation;
        }

        public void InitializeProgress()
        {
            TargetValue = Minimum;
            _progressStartTime = DateTime.UtcNow;
            SetValueOverridingAnimation();
        }

        private void UpdateAnaimation(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                return;
            SetValueOverridingAnimation();
        }

        private void SetValueOverridingAnimation()
        {
            Value = TargetValue;
            BeginAnimation(ValueProperty, null);
        }

        private static void OnTargetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var smoothProgressBar = (SmoothProgressBar)d;
            var newValue = (double)e.NewValue;
            var oldValue = (double)e.OldValue;
            var val2 = Math.Max(Math.Min(newValue, smoothProgressBar.Maximum), smoothProgressBar.Minimum);
            if (val2 == smoothProgressBar.Minimum)
                smoothProgressBar._progressStartTime = DateTime.UtcNow;
            if (val2 == smoothProgressBar.Minimum || val2 < oldValue || (smoothProgressBar.IsIndeterminate || !smoothProgressBar.IsVisible))
            {
                smoothProgressBar.SetValueOverridingAnimation();
            }
            else
            {
                var num1 = (int)((DateTime.UtcNow - smoothProgressBar._progressStartTime).TotalMilliseconds / (val2 - smoothProgressBar.Minimum) * (smoothProgressBar.Maximum - val2));
                if (smoothProgressBar.Maximum != val2)
                {
                    var val1 = (int)(num1 * (val2 - smoothProgressBar.Value) / (smoothProgressBar.Maximum - smoothProgressBar.Value));
                    smoothProgressBar._valueAnimation.From = smoothProgressBar.Value;
                    smoothProgressBar._valueAnimation.To = val2;
                    var num2 = Math.Min(val1, smoothProgressBar.AnimationMaximumDuration);
                    smoothProgressBar._valueAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(num2));
                    smoothProgressBar._progressStoryboard.Begin(smoothProgressBar);
                }
                else
                {
                    var num2 = (uint)smoothProgressBar.Value + 1U;
                    var num3 = (uint)val2;
                    var num4 = Math.Max(1U, (num3 - num2) / 10U);
                    smoothProgressBar.Value = Math.Min(num2, val2);
                    smoothProgressBar.BeginAnimation(ValueProperty, null);
                    var num5 = num2;
                    while (num5 < num3)
                    {
                        smoothProgressBar.Value = num5;
                        num5 += num4;
                    }
                    smoothProgressBar.Value = val2;
                }
            }
        }
    }
}
