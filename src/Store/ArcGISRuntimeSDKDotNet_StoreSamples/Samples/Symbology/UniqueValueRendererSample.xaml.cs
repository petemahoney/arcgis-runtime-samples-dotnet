﻿using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Shows how to create a UniqueValueRenderer for a graphics layer.
    /// US state polygons are pulled from an online source and rendered using the GraphicsLayer UniqueValueRenderer.
    /// </summary>
    /// <title>Unique Value Renderer</title>
	/// <category>Symbology</category>
	public partial class UniqueValueRendererSample : Windows.UI.Xaml.Controls.Page
    {
        private Random _random = new Random();
        private GraphicsLayer _states;

        /// <summary>Construct Unique Value Renderer sample control</summary>
        public UniqueValueRendererSample()
        {
            InitializeComponent();

            _states = mapView.Map.Layers["States"] as GraphicsLayer;
                
            mapView.ExtentChanged += mapView_ExtentChanged;
        }

        // Load state data - set initial renderer
        private async void mapView_ExtentChanged(object sender, EventArgs e)
        {
            try
            {
                mapView.ExtentChanged -= mapView_ExtentChanged;
                await LoadStatesAsync();

                ChangeRenderer();
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog("Error loading states data: " + ex.Message, "Sample Error").ShowAsync();
            }
        }

        // Change the graphics layer renderer to a new UniqueValueRenderer
        private void ChangeRendererButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeRenderer();
        }

        private void ChangeRenderer()
        {
            var renderer = new UniqueValueRenderer() { Fields = new ObservableCollection<string>(new string[] { "sub_region" }) };

            renderer.Infos = new UniqueValueInfoCollection(_states.Graphics
                .Select(g => g.Attributes["sub_region"])
                .Distinct()
                .Select(obj => new UniqueValueInfo { Values = new ObservableCollection<object>(new object[] { obj }), Symbol = GetRandomSymbol() }));

            _states.Renderer = renderer;
        }

        // Load US state data from map service
        private async Task LoadStatesAsync()
        {
            var queryTask = new QueryTask(
                new Uri("http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/2"));
            var query = new Query(mapView.Extent)
            {
                ReturnGeometry = true,
                OutSpatialReference = mapView.SpatialReference,
                OutFields = new OutFields(new string[] { "sub_region" })
            };
            var result = await queryTask.ExecuteAsync(query);

            _states.Graphics.Clear();
            _states.Graphics.AddRange(result.FeatureSet.Features);
        }

        // Utility: Generate a random simple fill symbol
        private SimpleFillSymbol GetRandomSymbol()
        {
            var color = GetRandomColor();

            return new SimpleFillSymbol()
            {
                Color = Color.FromArgb(0x77, color.R, color.G, color.B),
                Outline = new SimpleLineSymbol() { Width = 2, Style = SimpleLineStyle.Solid, Color = color },
                Style = SimpleFillStyle.Solid
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
