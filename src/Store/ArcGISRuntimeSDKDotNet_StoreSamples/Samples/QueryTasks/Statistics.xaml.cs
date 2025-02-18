﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates how to use a QueryTask to get statistics from a map service.
    /// </summary>
    /// <title>Statistics</title>
    /// <category>Query Tasks</category>
	public sealed partial class Statistics : Windows.UI.Xaml.Controls.Page
    {
        private const string LAYER_URL = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/2";
        
        private GraphicsLayer _graphicsLayer;

        /// <summary>Construct Statistics sample control</summary>
        public Statistics()
        {
            InitializeComponent();

            mapView.Map.InitialExtent = new Envelope(-15000000, 2000000, -7000000, 8000000);

            _graphicsLayer = mapView.Map.Layers["GraphicsLayer"] as GraphicsLayer;

            var taskRenderer = SetUniqueRenderer();
            var taskQuery = RunQuery();
        }

        // Create a unique value renderer by state sub-region name
        private async Task SetUniqueRenderer()
        {
            try
            {
                // Generate a unique value renderer on the server
                GenerateRendererTask generateRendererTask = new GenerateRendererTask(new Uri(LAYER_URL));

                UniqueValueDefinition uvDef = new UniqueValueDefinition() { Fields = new string[] { "sub_region" } };
                uvDef.ColorRamps.Add(new ColorRamp() { From = Colors.Purple, To = Colors.Yellow, Algorithm = Algorithm.LabLch });
                GenerateRendererParameter rendererParams = new GenerateRendererParameter() { ClassificationDefinition = uvDef };

                var rendererResult = await generateRendererTask.GenerateRendererAsync(rendererParams);
                _graphicsLayer.Renderer = rendererResult.Renderer;
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
        }

        // Query states the for graphics and statistics
        private async Task RunQuery()
        {
            try
            {
                progress.Visibility = Visibility.Visible;
                _graphicsLayer.Graphics.Clear();
                resultsGrid.ItemsSource = null;

                QueryTask queryTask = new QueryTask(new Uri(LAYER_URL));
                Query query = new Query("1=1")
                {
                    GroupByFieldsForStatistics = new List<string> { "sub_region" },
                    OutStatistics = new List<OutStatistic> 
                    { 
                        new OutStatistic() 
                        {
                            OnStatisticField = "pop2000",
                            OutStatisticFieldName = "SubRegionPopulation",
                            StatisticType = StatisticType.Sum
                        },
                        new OutStatistic()
                        {
                            OnStatisticField = "sub_region",
                            OutStatisticFieldName = "NumberOfStates",
                            StatisticType = StatisticType.Count
                        }
                    }
                };

                var result = await queryTask.ExecuteAsync(query);
                if (result.FeatureSet.Features != null && result.FeatureSet.Features.Count > 0)
                {
                    await CreateSubRegionLayerGraphics(result.FeatureSet.Features);
                    resultsGrid.ItemsSource = _graphicsLayer.Graphics;
                }
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
            finally
            {
                progress.Visibility = Visibility.Collapsed;
            }
        }

        private async Task CreateSubRegionLayerGraphics(IEnumerable<Graphic> statistics)
        {
            QueryTask queryTask = new QueryTask(new Uri(LAYER_URL));
            Query query = new Query("1=1")
            {
                ReturnGeometry = true,
                OutSpatialReference = mapView.SpatialReference,
                OutFields = new OutFields(new List<string> { "sub_region" })
            };

            var states = await queryTask.ExecuteAsync(query);

            // Create unioned graphics from state geometries for each region
            var regions = states.FeatureSet.Features
                .GroupBy(g => g.Attributes["sub_region"], g => g.Geometry)
                .Select(grp => new Graphic(GeometryEngine.Union(grp), statistics.First(stat => grp.Key.Equals(stat.Attributes["sub_region"])).Attributes));

            _graphicsLayer.Graphics.Clear();
            _graphicsLayer.Graphics.AddRange(regions);
        }

        private void resultsGrid_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            _graphicsLayer.ClearSelection();

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var graphic = e.AddedItems[0] as Graphic;
                if (graphic != null)
                    graphic.IsSelected = true;
            }
        }
    }

    /// <summary>Double to formatted string value converter</summary>
    internal class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double)
            {
                return ((double)value).ToString("#,#");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
