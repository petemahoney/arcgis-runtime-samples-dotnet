﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates the GeometryEngine.Boundary method to calculate the geometric boundary of a given geometry.
    /// </summary>
    /// <title>Boundary</title>
	/// <category>Geometry</category>
	public partial class Boundary : Page
    {
        /// <summary>Construct Boundary sample control</summary>
        public Boundary()
        {
            InitializeComponent();

            var task = CreateGraphics();
        }

        // Setup graphic layers with test graphics and calculated boundaries of each
        private async Task CreateGraphics()
        {
            await mapView.LayersLoadedAsync();

            CreateTestGraphics();
            CalculateBoundaries();
        }

        // Creates a two-part polygon and a four-part polyline to use as test graphics for the Boundary method
        private void CreateTestGraphics()
        {
            var center = mapView.Extent.GetCenter();
            var width = mapView.Extent.Width / 4;
            var left = new MapPoint(center.X - width, center.Y, mapView.SpatialReference);
            var right = new MapPoint(center.X + width, center.Y, mapView.SpatialReference);

            var fillSymbol = new SimpleFillSymbol() { Color = Colors.Red, Style = SimpleFillStyle.Solid };
            var lineSymbol = new SimpleLineSymbol() { Color = Colors.Red, Style = SimpleLineStyle.Solid, Width = 2 };

            var testGraphics = mapView.Map.Layers["TestGraphics"] as GraphicsLayer;
            testGraphics.Graphics.Add(new Graphic() { Geometry = CreatePolygonBox(left, width), Symbol = fillSymbol });
            testGraphics.Graphics.Add(new Graphic() { Geometry = CreatePolylineBox(right, width), Symbol = lineSymbol });
        }

        // Calculates the geometric boundaries for each test graphic
        private void CalculateBoundaries()
        {
            var testGraphics = mapView.Map.Layers["TestGraphics"] as GraphicsLayer;
            var boundaryGraphics = mapView.Map.Layers["BoundaryGraphics"] as GraphicsLayer;

            var lineSymbol = (Esri.ArcGISRuntime.Symbology.Symbol)new SimpleLineSymbol() { Color = Colors.Blue, Style = SimpleLineStyle.Solid, Width = 2 };
            var pointSymbol = (Esri.ArcGISRuntime.Symbology.Symbol)new SimpleMarkerSymbol() { Color = Colors.Blue, Style = SimpleMarkerStyle.Circle, Size = 12 };

            foreach (var testGraphic in testGraphics.Graphics)
            {
                var boundary = GeometryEngine.Boundary(testGraphic.Geometry);
                var graphic = new Graphic(boundary, (boundary.GeometryType == GeometryType.Polyline) ? lineSymbol : pointSymbol);
                boundaryGraphics.Graphics.Add(graphic);
            }
        }

        // Creates a square polygon with a hole centered at the given point
        private Polygon CreatePolygonBox(MapPoint center, double length)
        {
            var halfLen = length / 2.0;

            CoordinateCollection coords = new CoordinateCollection();
            coords.Add(new Coordinate(center.X - halfLen, center.Y + halfLen));
            coords.Add(new Coordinate(center.X + halfLen, center.Y + halfLen));
            coords.Add(new Coordinate(center.X + halfLen, center.Y - halfLen));
            coords.Add(new Coordinate(center.X - halfLen, center.Y - halfLen));
            coords.Add(new Coordinate(center.X - halfLen, center.Y + halfLen));

            halfLen /= 3;
            CoordinateCollection coordsHole = new CoordinateCollection();
            coordsHole.Add(new Coordinate(center.X - halfLen, center.Y + halfLen));
            coordsHole.Add(new Coordinate(center.X - halfLen, center.Y - halfLen));
            coordsHole.Add(new Coordinate(center.X + halfLen, center.Y - halfLen));
            coordsHole.Add(new Coordinate(center.X + halfLen, center.Y + halfLen));
            coordsHole.Add(new Coordinate(center.X - halfLen, center.Y + halfLen));

            return new Polygon(new List<CoordinateCollection> { coords, coordsHole }, mapView.SpatialReference);
        }

        // Creates a polyline with four paths in the shape of a box centered at the given point
        private Polyline CreatePolylineBox(MapPoint center, double length)
        {
            var halfLen = length / 2.0;
            var spacer = length / 10.0;

            List<CoordinateCollection> coords = new List<CoordinateCollection>()
            {
                new CoordinateCollection() 
                { 
                    new Coordinate(center.X - halfLen + spacer, center.Y + halfLen),
                    new Coordinate(center.X + halfLen - spacer, center.Y + halfLen)
                },
                new CoordinateCollection() 
                { 
                    new Coordinate(center.X + halfLen, center.Y + halfLen - spacer),
                    new Coordinate(center.X + halfLen, center.Y - halfLen + spacer)
                },
                new CoordinateCollection() 
                { 
                    new Coordinate(center.X + halfLen - spacer, center.Y - halfLen),
                    new Coordinate(center.X - halfLen + spacer, center.Y - halfLen)
                },
                new CoordinateCollection() 
                { 
                    new Coordinate(center.X - halfLen, center.Y - halfLen + spacer),
                    new Coordinate(center.X - halfLen, center.Y + halfLen - spacer)
                }
            };

            return new Polyline(coords, mapView.SpatialReference);
        }
    }
}
