﻿using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// Demonstrates creating an ArcGISDynamicMapServiceLayer which references a LocalMapService.
    /// </summary>
    /// <title>ArcGIS Local Dynamic Map Service Layer</title>
	/// <category>Layers</category>
	/// <subcategory>Dynamic Service Layers</subcategory>
	public partial class ArcGISDynamicMapServiceLayerLocalSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcGISDynamicMapServiceLayerLocalSample"/> class.
        /// </summary>
        public ArcGISDynamicMapServiceLayerLocalSample()
        {
            DataContext = this;

            InitializeComponent();

            var _ = CreateLocalServiceAndDynamicLayer();
        }

        public async Task CreateLocalServiceAndDynamicLayer() 
        {
            LocalMapService localMapService = new LocalMapService(@"..\..\..\..\..\samples-data\maps\water-distribution-network.mpk");

            try
            {
                await localMapService.StartAsync();

                ArcGISDynamicMapServiceLayer arcGISDynamicMapServiceLayer = new ArcGISDynamicMapServiceLayer() 
                {
                    ID = "arcGISDynamicMapServiceLayer",
                    ServiceUri = localMapService.UrlMapService,
                };

				MyMapView.Map.Layers.Add(arcGISDynamicMapServiceLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
