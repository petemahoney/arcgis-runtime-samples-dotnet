﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates Feature Layer hit testing
    /// </summary>
    /// <title>Hit Testing</title>
    /// <category>Feature Layers</category>
    public sealed partial class FeatureLayerHitTesting : Page
	{
        private FeatureLayer _featureLayer;

        public FeatureLayerHitTesting()
		{
			this.InitializeComponent();

            mapView.MapViewTapped += mapView_MapViewTapped;

            mapView.Map.InitialExtent = new Envelope(-14675766.3566695, 2695407.73380258, -6733121.86117095, 6583994.1013904);

            _featureLayer = mapView.Map.Layers["FeatureLayer"] as FeatureLayer;
            ((GeodatabaseFeatureServiceTable)_featureLayer.FeatureTable).OutFields = OutFields.All;
        }

        /// <summary>
        /// On each mouse click:
        /// - HitTest the feature layer
        /// - Query the feature table for the returned row
        /// - Set the result feature for the UI
        /// </summary>
        private async void mapView_MapViewTapped(object sender, MapViewInputEventArgs e)
        {
            try
            {
                var rows = await _featureLayer.HitTestAsync(mapView, e.Position);
                if (rows != null && rows.Length > 0)
                {
                    var features = await _featureLayer.FeatureTable.QueryAsync(rows);
                    var feature = features.FirstOrDefault();
                    if (feature != null)
                        listHitFeature.ItemsSource = feature.Attributes.Select(attr => new Tuple<string, string>(attr.Key, attr.Value.ToString()));
                    else
                        listHitFeature.ItemsSource = null;
                }
                else
                    listHitFeature.ItemsSource = null;
            }
            catch (Exception ex)
            {
                listHitFeature.ItemsSource = null;
                var _ = new MessageDialog("HitTest Error: " + ex.Message, "Feature Layer Hit Testing Sample").ShowAsync();
            }
        }
    }
}
