﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates how to create point geometries, attach them to graphics and display them on the map.
    /// MapPoint geometry objects are used to store geographic points.
    /// </summary>
    /// <title>Create Points</title>
	/// <category>Geometry</category>
	public partial class CreatePoints : Page
    {
        private GraphicsLayer graphicsLayer;

        /// <summary>Construct Create Points sample control</summary>
        public CreatePoints()
        {
            InitializeComponent();

            graphicsLayer = mapView.Map.Layers["GraphicsLayer"] as GraphicsLayer;
            var task = CreatePointGraphics();
        }

        // Create four point graphics on the map in the center of four equal quadrants
        private async Task CreatePointGraphics()
        {
            await mapView.LayersLoadedAsync();

            var height = mapView.Extent.Height / 4;
            var width = mapView.Extent.Width / 4;
            var center = mapView.Extent.GetCenter();

            var topLeft = new MapPoint(center.X - width, center.Y + height, mapView.SpatialReference);
            var topRight = new MapPoint(center.X + width, center.Y + height, mapView.SpatialReference);
            var bottomLeft = new MapPoint(center.X - width, center.Y - height, mapView.SpatialReference);
            var bottomRight = new MapPoint(center.X + width, center.Y - height, mapView.SpatialReference);

            var symbol = new SimpleMarkerSymbol() { Color = Colors.Red, Size = 15, Style = SimpleMarkerStyle.Diamond };

            graphicsLayer.Graphics.Add(new Graphic() { Geometry = topLeft, Symbol = symbol });
            graphicsLayer.Graphics.Add(new Graphic() { Geometry = topRight, Symbol = symbol });
            graphicsLayer.Graphics.Add(new Graphic() { Geometry = bottomLeft, Symbol = symbol });
            graphicsLayer.Graphics.Add(new Graphic() { Geometry = bottomRight, Symbol = symbol });
        }
    }
}
