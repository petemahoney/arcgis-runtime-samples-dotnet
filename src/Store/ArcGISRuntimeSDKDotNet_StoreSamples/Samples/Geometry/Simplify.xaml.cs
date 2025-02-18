﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates use of the GeometryEngine.Simplify method to simplify a polygon, and demonstrates the importance of simplification.
    /// </summary>
    /// <title>Simplify</title>
	/// <category>Geometry</category>
	public partial class Simplify : Windows.UI.Xaml.Controls.Page
    {
        private Polygon _unsimplifiedPolygon;
        private GraphicsLayer _parcelLayer;
        private GraphicsLayer _polygonLayer;

        /// <summary>Construct Geodesic Move sample control</summary>
        public Simplify()
        {
            InitializeComponent();

            _parcelLayer = mapView.Map.Layers["ParcelLayer"] as GraphicsLayer;
            _polygonLayer = mapView.Map.Layers["PolygonLayer"] as GraphicsLayer;

            mapView.ExtentChanged += mapView_ExtentChanged;
        }

        // Start map interaction once the mapview extent is set
        private void mapView_ExtentChanged(object sender, EventArgs e)
        {
            mapView.ExtentChanged -= mapView_ExtentChanged;
            DrawPolygon();
        }

        // Query without simplifying original geometry
        private async void QueryOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            await ParcelQuery(_unsimplifiedPolygon);
        }

        // Simplify and then query
        private async void SimplifyAndQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var simplifiedPolygon = GeometryEngine.Simplify(_unsimplifiedPolygon);
            await ParcelQuery(simplifiedPolygon);
        }

        // Draw the unsimplified polygon
        private void DrawPolygon()
        {
            MapPoint center = mapView.Extent.GetCenter();
            double lat = center.Y;
            double lon = center.X + 300;
            double latOffset = 300;
            double lonOffset = 300;

            var points = new CoordinateCollection()
            {
                new Coordinate(lon - lonOffset, lat),
                new Coordinate(lon, lat + latOffset),
                new Coordinate(lon + lonOffset, lat),
                new Coordinate(lon, lat - latOffset),
                new Coordinate(lon - lonOffset, lat),
                new Coordinate(lon - 2 * lonOffset, lat + latOffset),
                new Coordinate(lon - 3 * lonOffset, lat),
                new Coordinate(lon - 2 * lonOffset, lat - latOffset),
                new Coordinate(lon - 1.5 * lonOffset, lat + latOffset),
                new Coordinate(lon - lonOffset, lat)
            };
            _unsimplifiedPolygon = new Polygon(points, mapView.SpatialReference);

            _polygonLayer.Graphics.Clear();
            _polygonLayer.Graphics.Add(new Graphic(_unsimplifiedPolygon));
        }

        // Query the parcel service with the given geometry (Contains)
        private async Task ParcelQuery(Geometry geometry)
        {
            try
            {
                QueryTask queryTask = new QueryTask(
                    new Uri("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/BloomfieldHillsMichigan/Parcels/MapServer/2"));
                var query = new Query(geometry)
                {
                    ReturnGeometry = true,
                    OutSpatialReference = mapView.SpatialReference,
                    SpatialRelationship = SpatialRelationship.Contains,
                    OutFields = OutFields.All
                };
                var result = await queryTask.ExecuteAsync(query);

                _parcelLayer.Graphics.Clear();
                _parcelLayer.Graphics.AddRange(result.FeatureSet.Features);
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
        }
    }
}
