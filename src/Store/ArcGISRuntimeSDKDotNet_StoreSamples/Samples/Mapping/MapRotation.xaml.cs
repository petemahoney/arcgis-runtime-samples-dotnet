﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// This sample shows how to rotate a map using the MapView.Rotation property.
    /// </summary>
    /// <title>Map Rotation</title>
    /// <category>Mapping</category>
	public sealed partial class MapRotation : Page
    {
        public MapRotation()
        {
            this.InitializeComponent();
        }

        private void EnableTouchRotateButton_Click(object sender, RoutedEventArgs e)
        {
            var toggle = e.OriginalSource as AppBarToggleButton;
            if (toggle == null)
                return;

            if (toggle.IsChecked == true)
                mapView.ManipulationMode = ManipulationModes.All;
            else
                mapView.ClearValue(UIElement.ManipulationModeProperty);
        }
    }
}
