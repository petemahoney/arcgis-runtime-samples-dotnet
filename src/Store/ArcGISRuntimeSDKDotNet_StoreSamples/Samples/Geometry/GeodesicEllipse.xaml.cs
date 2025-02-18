﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates use of the GeometryEngine.GeodesicEllipse to calculate a geodesic ellipse. 
    /// Also shows how to calculate a geodesic sector using GeometryEngine.GeodesicSector to create a gedesic sector emanating from point.
    /// </summary>
    /// <title>Geodesic Ellipse</title>
	/// <category>Geometry</category>
	public partial class GeodesicEllipse : Windows.UI.Xaml.Controls.Page
    {
        private Symbol _pinSymbol;
        private Symbol _sectorSymbol;
        private GraphicsLayer _graphicsLayer;

        /// <summary>Construct Geodesic Ellipse sample control</summary>
        public GeodesicEllipse()
        {
            InitializeComponent();

            _pinSymbol = LayoutRoot.Resources["PointSymbol"] as Symbol;
            _sectorSymbol = LayoutRoot.Resources["SectorSymbol"] as Symbol;
            _graphicsLayer = mapView.Map.Layers["GraphicsLayer"] as GraphicsLayer;
        }

        private async void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                while (mapView.Extent != null)
                {
                    // Accept user point
                    var point = await mapView.Editor.RequestPointAsync();

                    // create the geodesic ellipse
                    var radius1 = (double)comboRadius1.SelectedItem;
                    var radius2 = (double)comboRadius2.SelectedItem;
                    var axis = sliderAxis.Value;
                    var maxLength = (double)comboSegmentLength.SelectedItem;
                    var param = new GeodesicEllipseParameters(point, radius1, radius2, LinearUnits.Miles)
                    { 
                        AxisDirection = axis, 
                        MaxPointCount = 10000, 
                        MaxSegmentLength = maxLength
                    };
                    var ellipse = GeometryEngine.GeodesicEllipse(param);

                    //show geometries on map
                    _graphicsLayer.Graphics.Clear();
                    _graphicsLayer.Graphics.Add(new Graphic(point, _pinSymbol));
                    _graphicsLayer.Graphics.Add(new Graphic(ellipse));

                    // geodesic sector
                    if ((bool)chkSector.IsChecked)
                    {
                        var sectorParams = new GeodesicSectorParameters(point, radius1, radius2, LinearUnits.Miles)
                        {
                            AxisDirection = axis,
                            MaxPointCount = 10000,
                            MaxSegmentLength = maxLength,
                            SectorAngle = sliderSectorAxis.Value,
                            StartDirection = sliderSectorStart.Value
                        };
                        var sector = GeometryEngine.GeodesicSector(sectorParams);

                        _graphicsLayer.Graphics.Add(new Graphic(sector, _sectorSymbol));
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
        }
    }
}
