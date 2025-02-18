﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates how to spatially search your data using a QueryTask with its geometry attribute set.
    /// </summary>
    /// <title>Spatial Query</title>
    /// <category>Query Tasks</category>
    public sealed partial class SpatialQuery : Page
    {
        public SpatialQuery()
        {
            this.InitializeComponent();

            mapView.Map.InitialExtent = new Envelope(-9270434.248, 5246977.326, -9269261.417, 5247569.712);
            var _ = InitializePMS();
        }

        private async Task InitializePMS()
        {
            try
            {
                var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/RedStickpin.png"));
                var imageSource = await imageFile.OpenReadAsync();
                var pms = LayoutRoot.Resources["DefaultMarkerSymbol"] as PictureMarkerSymbol;
                await pms.SetSourceAsync(imageSource);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void mapView_Tapped(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            try
            {
                var graphicsLayer = mapView.Map.Layers["GraphicsLayer"] as GraphicsLayer;
                graphicsLayer.Graphics.Add(new Graphic() { Geometry = e.Location });

                var bufferLayer = mapView.Map.Layers["BufferLayer"] as GraphicsLayer;
                var bufferResult = GeometryEngine.Buffer(e.Location, 100);
                bufferLayer.Graphics.Add(new Graphic() { Geometry = bufferResult });

                var queryTask = new QueryTask(
                    new Uri("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/BloomfieldHillsMichigan/Parcels/MapServer/2"));
                var query = new Query("1=1")
                {
                    ReturnGeometry = true,
                    OutSpatialReference = mapView.SpatialReference,
                    Geometry = bufferResult
                };
                query.OutFields.Add("OWNERNME1");

                var queryResult = await queryTask.ExecuteAsync(query);
                if (queryResult != null && queryResult.FeatureSet != null)
                {
                    var resultLayer = mapView.Map.Layers["ResultsGraphicsLayer"] as GraphicsLayer;
                    resultLayer.Graphics.AddRange(queryResult.FeatureSet.Features);
                }
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
        }
    }
}
