﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StockMarketDesktopClient.Scripts;
using Pomelo.Data.MySql;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StockMarketDesktopClient.Pages.User {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class PriceHis {
        public string Time { get; set; }
        public double Price { get; set; }
    }
    public sealed partial class StockPage : Page {
        string StockName;
        public StockPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            StockName = (string)e.Parameter;
            LoadChart();
            LoadPrices();
        }

        private void LoadPrices() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT FullName, CurrentPrice, OpeningPriceToday, VolumeTraded FROM Stock WHERE StockName = '" + StockName + "'");
            while (reader.Read()) {
                string FullName = (string)reader["FullName"];
                double Price = (double)reader["CurrentPrice"];
                double OpeningPrice = (double)reader["OpeningPriceToday"];
                double RealChangeInPrice = Price - OpeningPrice;
                double PercentageChange = RealChangeInPrice / OpeningPrice;
                int VolumeTraded = (int)reader["VolumeTraded"];
                StockNameTitle.Text = FullName;
                ChangeInPrice.Text = "Change In Price: " + RealChangeInPrice.ToString();
                ChangeInPricePercentage.Text = "Change In Price: " + PercentageChange.ToString() + "%";
                CurrentPriceBlock.Text =  "Current Price: " + Price.ToString().Split('.')[0] + Price.ToString().Split('.')[1].Substring(0,4);
                VolumeTradedToday.Text = "Volume Traded: " + VolumeTraded.ToString();
            }
        }

        private void LoadChart() {
            MySqlDataReader reader = DataBaseHandler.GetData("SELECT Time, Price FROM PricingHistory WHERE StockName = '" + StockName + "' LIMIT 20");
            List<PriceHis> Prices = new List<PriceHis>();
            double MaxPrice = 0;
            double MinPrice = double.PositiveInfinity;
            while (reader.Read()) {
                Prices.Add(new PriceHis() { Time = ((DateTime)reader["Time"]).ToString().Split(' ')[1], Price = (double)reader["Price"] });
                if ((double)reader["Price"] < MinPrice) {
                    MinPrice = (double)reader["Price"];
                }
                if ((double)reader["Price"] > MaxPrice) {
                    MaxPrice = (double)reader["Price"];
                }
            }
            ((LineSeries)LineChart.Series[0]).DependentRangeAxis = new LinearAxis {
                Minimum = MinPrice - 1,
                Maximum = MaxPrice + 1,
                Orientation = AxisOrientation.Y,
            };
            (LineChart.Series[0] as LineSeries).ItemsSource = Prices;
        }

        private void SellButton(object sender, RoutedEventArgs e) {

        }

        private void BuyButton(object sender, RoutedEventArgs e) {

        }
    }
}