﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates using the GeometryEngine.Generalize method to take a polyline with numerous vertices and return a generalized polyline with less vertices.
    /// </summary>
    /// <title>Generalize</title>
    /// <category>Geometry</category>
    public partial class Generalize : Windows.UI.Xaml.Controls.Page
    {
        private GraphicsLayer _originalGraphicsLayer;
        private GraphicsLayer _generalizedGraphicsLayer;
        private SimpleMarkerSymbol _defaultMarkerSymbol;
        private SimpleLineSymbol _defaultLineSymbol;
        private SimpleLineSymbol _generalizedLineSymbol;
        private SimpleMarkerSymbol _generalizedMarkerSymbol;

        /// <summary>Construct Generalize sample control</summary>
        public Generalize()
        {
            InitializeComponent();

            mapView.Map.InitialExtent = new Envelope(-12000000, 3000000, -7000000, 7000000, SpatialReferences.WebMercator);
            mapView.Loaded += mapView_Loaded;

            _originalGraphicsLayer = mapView.Map.Layers["OriginalLineGraphicsLayer"] as GraphicsLayer;
            _generalizedGraphicsLayer = mapView.Map.Layers["GeneralizedLineGraphicsLayer"] as GraphicsLayer;

            _defaultMarkerSymbol = LayoutRoot.Resources["DefaultMarkerSymbol"] as SimpleMarkerSymbol;
            _defaultLineSymbol = LayoutRoot.Resources["DefaultLineSymbol"] as SimpleLineSymbol;
            _generalizedLineSymbol = LayoutRoot.Resources["GeneralizedLineSymbol"] as SimpleLineSymbol;
            _generalizedMarkerSymbol = LayoutRoot.Resources["GeneralizedMarkerSymbol"] as SimpleMarkerSymbol;
        }

        // Adds the original river graphic to the map (from an online service)
        async void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_originalGraphicsLayer != null && _originalGraphicsLayer.Graphics.Count == 0)
                {
                    QueryTask queryTask = new QueryTask(
                        new Uri("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StatesCitiesRivers_USA/MapServer/1"));

                    Query query = new Query("NAME = 'Mississippi'");
                    query.ReturnGeometry = true;
                    query.OutSpatialReference = mapView.SpatialReference;

                    var results = await queryTask.ExecuteAsync(query);

                    var river = results.FeatureSet.Features
                        .Select(f => f.Geometry)
                        .OfType<Polyline>()
                        .FirstOrDefault();

                    _originalGraphicsLayer.Graphics.Add(new Graphic(river, _defaultLineSymbol));

                    foreach (var path in river)
                    {
                        foreach (var coord in path)
                        {
                            var vertex = new Graphic(new MapPoint(coord, river.SpatialReference), _defaultMarkerSymbol);
                            _originalGraphicsLayer.Graphics.Add(vertex);
                        }
                    }

                    GeneralizeButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog("Error loading test line: " + ex.Message, "Sample Error").ShowAsync();
            }
        }

        // Generalizes the original line graphic
        private void GeneralizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _generalizedGraphicsLayer.Graphics.Clear();

                var offset = DistanceSlider.Value * 1000;
                var generalizedPolyline = GeometryEngine.Generalize(_originalGraphicsLayer.Graphics[0].Geometry, offset, false) as Polyline;

                if (generalizedPolyline != null)
                {
                    var graphic = new Graphic(generalizedPolyline, _generalizedLineSymbol);
                    _generalizedGraphicsLayer.Graphics.Add(graphic);

                    foreach (var path in generalizedPolyline)
                    {
                        foreach (var coord in path)
                        {
                            var vertex = new Graphic(new MapPoint(coord, mapView.SpatialReference), _generalizedMarkerSymbol);
                            _generalizedGraphicsLayer.Graphics.Add(vertex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog("Error generalizing line: " + ex.Message, "Sample Error").ShowAsync();
            }
        }
    }
}
