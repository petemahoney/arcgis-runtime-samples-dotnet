﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstartes the GraphicsLayer.GraphicsSouce property.
    /// </summary>
    /// <title>Graphics Source</title>
    /// <category>Graphics Layers</category>
    public sealed partial class GraphicsSourceSample : Page
    {
        private Random _random = new Random();
        private GraphicsLayer _graphicsLayer;
        private List<List<Graphic>> _grahicsSources;
        private int _graphicSourceIndex;

        public GraphicsSourceSample()
        {
            this.InitializeComponent();

            _graphicsLayer = mapView.Map.Layers["GraphicsLayer"] as GraphicsLayer;
            CreateGraphics();
        }

        // Switch between pre-created graphics lists
        private void SwitchGraphicSourceButton_Click(object sender, RoutedEventArgs e)
        {
            ++_graphicSourceIndex;
            if (_graphicSourceIndex == _grahicsSources.Count)
                _graphicSourceIndex = 0;

            _graphicsLayer.GraphicsSource = _grahicsSources[_graphicSourceIndex];
        }

        // Create three List<Graphic> objects with random graphics to serve as layer GraphicsSources
        private async void CreateGraphics()
        {
            await mapView.LayersLoadedAsync();

            _grahicsSources = new List<List<Graphic>>()
            {
                new List<Graphic>(),
                new List<Graphic>(),
                new List<Graphic>()
            };

            foreach (var graphicList in _grahicsSources)
            {
                for (int n = 0; n < 10; ++n)
                {
                    graphicList.Add(CreateRandomGraphic());
                }
            }

            _graphicSourceIndex = 0;
            _graphicsLayer.GraphicsSource = _grahicsSources[_graphicSourceIndex];
        }

        // Create a random graphic
        private Graphic CreateRandomGraphic()
        {
            return new Graphic()
            {
                Geometry = GetRandomMapPoint(),
                Symbol = new SimpleMarkerSymbol() { Color = GetRandomColor(), Size = 15, Style = GetRandomMarkerStyle() }
            };
        }

        // Utility: Generate a random MapPoint within the current extent
        private MapPoint GetRandomMapPoint()
        {
            double x = mapView.Extent.XMin + (_random.NextDouble() * mapView.Extent.Width);
            double y = mapView.Extent.YMin + (_random.NextDouble() * mapView.Extent.Height);
            return new MapPoint(x, y, mapView.SpatialReference);
        }

        // Utility: Generate a random System.Windows.Media.Color
        private Color GetRandomColor()
        {
            var colorBytes = new byte[3];
            _random.NextBytes(colorBytes);
            return Color.FromArgb(0xFF, colorBytes[0], colorBytes[1], colorBytes[2]);
        }

        // Utility: Generate a random marker sytle
        private SimpleMarkerStyle GetRandomMarkerStyle()
        {
            return (SimpleMarkerStyle)_random.Next(0, 6);
        }
    }
}
