using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ModernApplicationFramework.Imaging.Converters;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Imaging
{
    public sealed class CrispImage : Image
    {
        public static readonly DependencyProperty MonikerProperty =
            DependencyProperty.Register(nameof(Moniker), typeof(ImageMoniker), typeof(CrispImage));

        public static readonly DependencyProperty ConverterTaskProperty =
            DependencyProperty.Register(nameof(ConverterTask), typeof(ObservableTask<ImageSource>), typeof(CrispImage));
        public static readonly DependencyProperty GrayscaleProperty = DependencyProperty.Register(nameof(Grayscale), typeof(bool), typeof(CrispImage), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty ActualGrayscaleBiasColorProperty = DependencyProperty.Register(nameof(ActualGrayscaleBiasColor), typeof(Color), typeof(CrispImage));

        public static readonly DependencyProperty ActualDpiProperty = DependencyProperty.Register(nameof(ActualDpi), typeof(double), typeof(CrispImage), new FrameworkPropertyMetadata(Boxes.DoubleZero));

        public static readonly DependencyProperty DpiProperty = DependencyProperty.RegisterAttached("Dpi", typeof(double), typeof(CrispImage), new FrameworkPropertyMetadata(Boxes.DoubleZero, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty GrayscaleBiasColorProperty = DependencyProperty.RegisterAttached("GrayscaleBiasColor", typeof(Color?), typeof(CrispImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ActualHighContrastProperty = DependencyProperty.Register(nameof(ActualHighContrast), typeof(bool), typeof(CrispImage));

        public static readonly DependencyProperty SystemHighContrastProperty = DependencyProperty.Register(nameof(SystemHighContrast), typeof(bool), typeof(CrispImage));

        public static readonly DependencyProperty HighContrastProperty = DependencyProperty.RegisterAttached("HighContrast", typeof(bool?), typeof(CrispImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.RegisterAttached("ScaleFactor", typeof(double), typeof(CrispImage), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));

        public Color ActualGrayscaleBiasColor => (Color)GetValue(ActualGrayscaleBiasColorProperty);


        public double ActualDpi => (double)GetValue(ActualDpiProperty);

        public ImageMoniker Moniker
        {
            get => (ImageMoniker) GetValue(MonikerProperty);
            set => SetValue(MonikerProperty, value);
        }

        public bool SystemHighContrast => (bool)GetValue(SystemHighContrastProperty);


        public static double DefaultDpi => DpiHelper.Default.DeviceDpiX;

        public ObservableTask<ImageSource> ConverterTask
        {
            get => (ObservableTask<ImageSource>)GetValue(ConverterTaskProperty);
            internal set => SetValue(ConverterTaskProperty, value);
        }

        public bool Grayscale
        {
            get => (bool)GetValue(GrayscaleProperty);
            set => SetValue(GrayscaleProperty, value);
        }

        public bool ActualHighContrast => (bool)GetValue(ActualHighContrastProperty);

        static CrispImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CrispImage), new FrameworkPropertyMetadata(typeof(CrispImage)));
        }

        public CrispImage()
        {
            InitializeBindings();
        }

        public static double GetScaleFactor(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (double)element.GetValue(ScaleFactorProperty);
        }

        public static void SetScaleFactor(DependencyObject element, double value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(ScaleFactorProperty, value);
        }

        private void InitializeBindings()
        {
            InitializeDpiBindings();
            InitializeGrayscaleBiasColorBindings();
            InitializeHighContrastBindings();
        }

        private void InitializeDpiBindings()
        {
            BindingOperations.SetBinding(this, ActualDpiProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(DpiProperty),
                Converter = ActualDpiConverter.Instance
            });
        }

        private void InitializeGrayscaleBiasColorBindings()
        {
            BindingOperations.SetBinding(this, ActualGrayscaleBiasColorProperty, new MultiBinding()
            {
                Bindings = {
                    new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(GrayscaleBiasColorProperty)
                    },
                    new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(ActualHighContrastProperty)
                    }
                },
                Converter = ActualGrayscaleBiasColorConverter.Instance
            });
        }

        private void InitializeHighContrastBindings()
        {
            SetResourceReference(SystemHighContrastProperty, SystemParameters.HighContrastKey);
            BindingOperations.SetBinding(this, ActualHighContrastProperty, new MultiBinding()
            {
                Bindings = {
                    new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(HighContrastProperty)
                    },
                    new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(SystemHighContrastProperty)
                    }
                },
                Converter = ActualHighContrastConverter.Instance
            });
        }
    }
}
