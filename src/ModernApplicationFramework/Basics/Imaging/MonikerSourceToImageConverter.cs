﻿using System.ComponentModel;
using System.Globalization;
using ModernApplicationFramework.Core.Converters.General;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Basics.Imaging
{
    public class MonikerSourceToImageConverter : MultiValueConverter<ImageMoniker, bool, CrispImage>
    {
        protected override CrispImage Convert(ImageMoniker moniker, bool enabled, object parameter, CultureInfo culture)
        {
            var image = new CrispImage
            {
                Moniker = moniker,
                Grayscale = enabled
            };
            return image;
        }
    }

    public class EmptyMonikerToBoolConverter : ToBooleanValueConverter<ImageMoniker>
    {
        protected override bool Convert(ImageMoniker value, object parameter, CultureInfo culture)
        {
            return value == ImageLibrary.EmptyMoniker;
        }
    }
}
