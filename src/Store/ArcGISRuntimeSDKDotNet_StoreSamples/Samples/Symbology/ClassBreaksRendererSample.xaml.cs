﻿using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Shows how to create a ClassBreaksRenderer for a graphics layer.
    /// Earthquake data points are pulled from an online source and rendered using the GraphicsLayer ClassBreaksRenderer.
    /// </summary>
    /// <title>Class Breaks Renderer</title>
	/// <category>Symbology</category>
	public partial class ClassBreaksRendererSample : Windows.UI.Xaml.Controls.Page
    {
        private Random _random = new Random();
        private GraphicsLayer _earthquakes;

        /// <summary>Construct Class Breaks Renderer sample control</summary>
        public ClassBreaksRendererSample()
        {
            InitializeComponent();

            _earthquakes = mapView.Map.Layers["Earthquakes"] as GraphicsLayer;
                
            mapView.ExtentChanged += mapView_ExtentChanged;
        }

        // Load earthquake data
        private async void mapView_ExtentChanged(object sender, EventArgs e)
        {
            try
            {
                mapView.ExtentChanged -= mapView_ExtentChanged;
                await LoadEarthquakesAsync();
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog("Error loading earthquake data: " + ex.Message, "Sample Error").ShowAsync();
            }
        }

        // Change the graphics layer renderer to a new ClassBreaksRenderer
        private void ChangeRendererButton_Click(object sender, RoutedEventArgs e)
        {
            SimpleMarkerStyle style = (SimpleMarkerStyle)_random.Next(0, 6);

            _earthquakes.Renderer = new ClassBreaksRenderer()
            {
                Field = "magnitude",
                Infos = new ClassBreakInfoCollection() 
                { 
                    new ClassBreakInfo() { Minimum = 2, Maximum = 3, Symbol = GetRandomSymbol(style) },
                    new ClassBreakInfo() { Minimum = 3, Maximum = 4, Symbol = GetRandomSymbol(style) },
                    new ClassBreakInfo() { Minimum = 4, Maximum = 5, Symbol = GetRandomSymbol(style) },
                    new ClassBreakInfo() { Minimum = 5, Maximum = 6, Symbol = GetRandomSymbol(style) },
                    new ClassBreakInfo() { Minimum = 6, Maximum = 7, Symbol = GetRandomSymbol(style) },
                    new ClassBreakInfo() { Minimum = 7, Maximum = 8, Symbol = GetRandomSymbol(style) },
                }
            };
        }

        // Load earthquakes from map service
        private async Task LoadEarthquakesAsync()
        {
            var queryTask = new QueryTask(
                new Uri("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Earthquakes/EarthquakesFromLastSevenDays/MapServer/0"));
            var query = new Query(mapView.Extent)
            {
                ReturnGeometry = true,
                OutSpatialReference = mapView.SpatialReference,
                Where = "magnitude > 2.0",
                OutFields = new OutFields(new string[] { "magnitude" })
            };
            var result = await queryTask.ExecuteAsync(query);

            _earthquakes.Graphics.Clear();
            _earthquakes.Graphics.AddRange(result.FeatureSet.Features);
        }

        // Utility: Generate a random simple marker symbol
        private SimpleMarkerSymbol GetRandomSymbol(SimpleMarkerStyle style)
        {
            return new SimpleMarkerSymbol()
            {
                Size = 12,
                Color = GetRandomColor(),
                Style = style
            };
        }

        // Utility function: Generate a random System.Windows.Media.Color
        private Color GetRandomColor()
        {
            var colorBytes = new byte[3];
            _random.NextBytes(colorBytes);
            return Color.FromArgb(0xFF, colorBytes[0], colorBytes[1], colorBytes[2]);
        }
    }
}
