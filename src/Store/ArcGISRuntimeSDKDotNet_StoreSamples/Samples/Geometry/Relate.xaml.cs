﻿using Windows.UI.Xaml;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Esri.ArcGISRuntime.Geometry;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Sample shows how to use the GeometryEngine.Relate method to test the spatial relationship of two geometries.
    /// </summary>
    /// <title>Relate</title>
	/// <category>Geometry</category>
	public partial class Relate : Windows.UI.Xaml.Controls.Page
    {
        private List<Symbol> _symbols;
        private GraphicsLayer _graphicsLayer;

        /// <summary>Construct Relationship sample control</summary>
        public Relate()
        {
            InitializeComponent();

            _symbols = new List<Symbol>();
            _symbols.Add(LayoutRoot.Resources["PointSymbol"] as Symbol);
            _symbols.Add(LayoutRoot.Resources["LineSymbol"] as Symbol);
            _symbols.Add(LayoutRoot.Resources["FillSymbol"] as Symbol);

            _graphicsLayer = mapView.Map.Layers["GraphicsLayer"] as GraphicsLayer;

            mapView.ExtentChanged += mapView_ExtentChanged;
        }

        // Start map interaction
        private void mapView_ExtentChanged(object sender, EventArgs e)
        {
            try
            {
                mapView.ExtentChanged -= mapView_ExtentChanged;
                btnDraw.IsEnabled = true;
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
        }

        // Accepts two user shapes and adds them to the graphics layer
        private async void StartDrawingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnDraw.IsEnabled = false;
                btnTest.IsEnabled = false;
                resultPanel.Visibility = Visibility.Collapsed;

                _graphicsLayer.Graphics.Clear();

                // Shape One
                Esri.ArcGISRuntime.Geometry.Geometry shapeOne = await mapView.Editor.RequestShapeAsync(
                    (DrawShape)comboShapeOne.SelectedValue, _symbols[comboShapeOne.SelectedIndex]);

                _graphicsLayer.Graphics.Add(new Graphic(shapeOne, _symbols[comboShapeOne.SelectedIndex]));

                // Shape Two
                Esri.ArcGISRuntime.Geometry.Geometry shapeTwo = await mapView.Editor.RequestShapeAsync(
                    (DrawShape)comboShapeTwo.SelectedValue, _symbols[comboShapeTwo.SelectedIndex]);

                _graphicsLayer.Graphics.Add(new Graphic(shapeTwo, _symbols[comboShapeTwo.SelectedIndex]));
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
            finally
            {
                btnDraw.IsEnabled = true;
                btnTest.IsEnabled = (_graphicsLayer.Graphics.Count >= 2);
            }
        }

        // Checks the specified relationship of the two shapes
        private void RelateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_graphicsLayer.Graphics.Count < 2)
                    throw new Exception("No shapes abailable for relationship test");

                var shape1 = _graphicsLayer.Graphics[0].Geometry;
                var shape2 = _graphicsLayer.Graphics[1].Geometry;

                string relate = txtRelation.Text;
                if (relate.Length < 9)
                    throw new Exception("DE-9IM relate string must be 9 characters");

                relate = relate.Substring(0, 9);

                bool isRelated = GeometryEngine.Relate(shape1, shape2, relate);

                resultPanel.Visibility = Visibility.Visible;
                resultPanel.Background = new SolidColorBrush((isRelated) ? Color.FromArgb(0x66, 0, 0xFF, 0) : Color.FromArgb(0x66, 0xFF, 0, 0));
                resultPanel.DataContext = string.Format("Relationship: '{0}' is {1}", relate, isRelated.ToString());
            }
            catch (Exception ex)
            {
                resultPanel.Visibility = Visibility.Collapsed;
                var _ = new MessageDialog(ex.Message, "Sample Error").ShowAsync();
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as Windows.UI.Xaml.Controls.MenuFlyoutItem;
            txtRelation.Text = menuItem.Text;
        }
    }
}
