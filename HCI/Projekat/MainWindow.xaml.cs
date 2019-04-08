using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using WPFTextBoxAutoComplete;
using System.Text.RegularExpressions;

namespace Projekat
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<City> listaGradova;
        public MainWindow()
        {
            listaGradova = LoadCities();
            InitializeComponent();
            

            Image refresh = new Image();
            refresh.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@"..\..\..\refresh.png")));

            refreshButton.Content = refresh;
            minTempDay1.Background = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.GetFullPath(@"..\..\..\background.png"))));
            try
            {
                string city = GetCurrentCity();
                foreach (City c in listaGradova)
                {
                    if (c.ToString().Equals(city))
                    {
                        cities.Items.Add(c);
                        cities.SelectedIndex = 0;
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                City temp = null;
                string city = textBoxNew.Text;
                foreach (City c in listaGradova)
                {
                    if (c.ToString().Equals(city))
                    {
                        temp = c;
                    }
                }
                if (temp != null)
                {
                    bool ind = false;
                    foreach (City c in cities.Items)
                    {
                        if (c.id.Equals(temp.id))
                        {
                            ind = true;
                        }
                    }
                    if (ind == false)
                    {
                        cities.Items.Add(temp);
                    }
                    textBoxNew.Text = "";
                    borderBox.Visibility = Visibility.Collapsed;
                }


            }
            catch (Exception)
            {

            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Cities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadData();
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }


        private static List<City> LoadCities()
        {
            List<City> cities;
            using (StreamReader r = new StreamReader(@"..\..\..\cities.json"))
            {
                string json = r.ReadToEnd();
                cities = JsonConvert.DeserializeObject<List<City>>(json);
            }

            return cities;
        }
        private static Forecast LoadForecast(int id)
        {
            string URL = "http://api.openweathermap.org/data/2.5/";
            string urlParameters = $"forecast?id={id}&units=metric&APPID=62212602955dbb01708352795a897453";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var forecast = response.Content.ReadAsAsync<Forecast>().Result;
                client.Dispose();
                return forecast;
            }
            client.Dispose();
            return null;
        }

        private static string GetCurrentCity()
        {
            string city = new WebClient().DownloadString("http://ipinfo.io/city");
            string country = new WebClient().DownloadString("http://ipinfo.io/country");
            string rez = city + ", " + country;
            string replacement = Regex.Replace(rez, @"\t|\n|\r", "");
            return replacement;
        }

        private static List<double> getMinMax(Forecast f, int index)
        {
            List<double> temp = new List<double>();
            double min = f.list[index].main.temp_min;
            double max = f.list[index].main.temp_max;

            for (int i = index + 1; i < index + 8; i++)
            {
                if (f.list[i].main.temp_max > max)
                {
                    max = f.list[i].main.temp_max;
                }
                if (f.list[i].main.temp_min < min)
                {
                    min = f.list[i].main.temp_min;
                }
            }
            temp.Add(min);
            temp.Add(max);

            min = f.list[index + 8].main.temp_min;
            max = f.list[index + 8].main.temp_max;

            for (int i = index + 9; i < index + 16; i++)
            {
                if (f.list[i].main.temp_max > max)
                {
                    max = f.list[i].main.temp_max;
                }
                if (f.list[i].main.temp_min < min)
                {
                    min = f.list[i].main.temp_min;
                }
            }
            temp.Add(min);
            temp.Add(max);

            min = f.list[index + 16].main.temp_min;
            max = f.list[index + 16].main.temp_max;

            for (int i = index + 17; i < index + 24; i++)
            {
                if (f.list[i].main.temp_max > max)
                {
                    max = f.list[i].main.temp_max;
                }
                if (f.list[i].main.temp_min < min)
                {
                    min = f.list[i].main.temp_min;
                }
            }
            temp.Add(min);
            temp.Add(max);

            min = f.list[index + 24].main.temp_min;
            max = f.list[index + 24].main.temp_max;

            for (int i = index + 25; i < index + 32; i++)
            {
                if (f.list[i].main.temp_max > max)
                {
                    max = f.list[i].main.temp_max;
                }
                if (f.list[i].main.temp_min < min)
                {
                    min = f.list[i].main.temp_min;
                }
            }
            temp.Add(min);
            temp.Add(max);

            return temp;
        }

        private static int getIndex(string date)
        {
            string sat = date.Substring(11, 2);
            int indeks;
            switch (sat)
            {
                case "00":
                    indeks = 12;
                    break;
                case "03":
                    indeks = 11;
                    break;
                case "06":
                    indeks = 10;
                    break;
                case "09":
                    indeks = 9;
                    break;
                case "12":
                    indeks = 8;
                    break;
                case "15":
                    indeks = 7;
                    break;
                case "18":
                    indeks = 6;
                    break;
                case "21":
                    indeks = 5;
                    break;
                default:
                    indeks = 0;
                    break;
            }
            return indeks;
        }

        private void TextBoxNew_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Equals("Return"))
            {
                try
                {
                    City temp = null;
                    string city = textBoxNew.Text;
                    foreach (City c in listaGradova)
                    {
                        if (c.ToString().Equals(city))
                        {
                            temp = c;
                        }
                    }
                    if (temp != null)
                    {
                        bool ind = false;
                        foreach (City c in cities.Items)
                        {
                            if (c.id.Equals(temp.id))
                            {
                                ind = true;
                            }
                        }
                        if (ind == false)
                        {
                            cities.Items.Add(temp);
                        }
                        textBoxNew.Text = "";
                        borderBox.Visibility = Visibility.Collapsed;
                        return;
                    }


                }
                catch (Exception)
                {

                }
            }
            if (e.Key.ToString().Equals("Up"))
            {
                
            }
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = listaGradova;
            string query = (sender as TextBox).Text;
            if (query.Length < 3)
            {
                // Clear
                resultStack.Children.Clear();
                border.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                border.Visibility = System.Windows.Visibility.Visible;
            }

            // Clear the list
            resultStack.Children.Clear();

            // Add the result
            if (query.Length < 3) { return; }
            foreach (City c in data)
            {
                if (c.ToString().ToLower().StartsWith(query.ToLower()))
                {
                    // The word starts with this... Autocomplete must work

                    addItem(c.ToString());
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void addItem(string text)
        {
            TextBlock block = new TextBlock();

            // Add the text
            block.Text = text;

            // A little style...
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events
            block.MouseLeftButtonUp += (sender, e) =>
            {
                textBoxNew.Text = (sender as TextBlock).Text;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.SkyBlue;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel
            resultStack.Children.Add(block);
        }
        

        private void Cities_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cities.Items.Remove(cities.SelectedItem);
            cities.Items.Remove(cities.SelectedItem);
            cityName.Visibility = Visibility.Hidden;
            countryName.Visibility = Visibility.Hidden;
            conditionLabel.Visibility = Visibility.Hidden;
            currentTemp.Visibility = Visibility.Hidden;
            conditionType.Visibility = Visibility.Hidden;
            conditionImage.Visibility = Visibility.Hidden;
            lastUpdateTime.Visibility = Visibility.Hidden;
            lastUpdateLabel.Visibility = Visibility.Hidden;
            conditionImage1.Visibility = Visibility.Hidden;
            conditionImage2.Visibility = Visibility.Hidden;
            conditionImage3.Visibility = Visibility.Hidden;
            conditionImage4.Visibility = Visibility.Hidden;
            day1Label.Visibility = Visibility.Hidden;
            day2Label.Visibility = Visibility.Hidden;
            day3Label.Visibility = Visibility.Hidden;
            day4Label.Visibility = Visibility.Hidden;
            day1ContditionLabel.Visibility = Visibility.Hidden;
            day2ContditionLabel.Visibility = Visibility.Hidden;
            day3ContditionLabel.Visibility = Visibility.Hidden;
            day4ContditionLabel.Visibility = Visibility.Hidden;
            minTempDay.Visibility = Visibility.Hidden;
            maxTempDay1.Visibility = Visibility.Hidden;
            minTempDay2.Visibility = Visibility.Hidden;
            maxTempDay2.Visibility = Visibility.Hidden;
            minTempDay3.Visibility = Visibility.Hidden;
            maxTempDay3.Visibility = Visibility.Hidden;
            minTempDay4.Visibility = Visibility.Hidden;
            maxTempDay4.Visibility = Visibility.Hidden;
            hour1Label.Visibility = Visibility.Hidden;
            hour1Temp.Visibility = Visibility.Hidden;
            hour2Label.Visibility = Visibility.Hidden;
            hour2Temp.Visibility = Visibility.Hidden;
            hour3Label.Visibility = Visibility.Hidden;
            hour3Temp.Visibility = Visibility.Hidden;
            hour4Label.Visibility = Visibility.Hidden;
            hour4Temp.Visibility = Visibility.Hidden;
            hour5Label.Visibility = Visibility.Hidden;
            hour5Temp.Visibility = Visibility.Hidden;
            hour6Label.Visibility = Visibility.Hidden;
            hour6Temp.Visibility = Visibility.Hidden;
            hour7Label.Visibility = Visibility.Hidden;
            hour7Temp.Visibility = Visibility.Hidden;
            hour8Label.Visibility = Visibility.Hidden;
            hour8Temp.Visibility = Visibility.Hidden;
            refreshButton.Visibility = Visibility.Hidden;
        }

        private void Cities_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key.ToString().Equals("Delete"))
            {
                cities.Items.Remove(cities.SelectedItem);
                cityName.Visibility = Visibility.Hidden;
                countryName.Visibility = Visibility.Hidden;
                conditionLabel.Visibility = Visibility.Hidden;
                currentTemp.Visibility = Visibility.Hidden;
                conditionType.Visibility = Visibility.Hidden;
                conditionImage.Visibility = Visibility.Hidden;
                lastUpdateTime.Visibility = Visibility.Hidden;
                lastUpdateLabel.Visibility = Visibility.Hidden;
                conditionImage1.Visibility = Visibility.Hidden;
                conditionImage2.Visibility = Visibility.Hidden;
                conditionImage3.Visibility = Visibility.Hidden;
                conditionImage4.Visibility = Visibility.Hidden;
                day1Label.Visibility = Visibility.Hidden;
                day2Label.Visibility = Visibility.Hidden;
                day3Label.Visibility = Visibility.Hidden;
                day4Label.Visibility = Visibility.Hidden;
                day1ContditionLabel.Visibility = Visibility.Hidden;
                day2ContditionLabel.Visibility = Visibility.Hidden;
                day3ContditionLabel.Visibility = Visibility.Hidden;
                day4ContditionLabel.Visibility = Visibility.Hidden;
                minTempDay.Visibility = Visibility.Hidden;
                maxTempDay1.Visibility = Visibility.Hidden;
                minTempDay2.Visibility = Visibility.Hidden;
                maxTempDay2.Visibility = Visibility.Hidden;
                minTempDay3.Visibility = Visibility.Hidden;
                maxTempDay3.Visibility = Visibility.Hidden;
                minTempDay4.Visibility = Visibility.Hidden;
                maxTempDay4.Visibility = Visibility.Hidden;
                hour1Label.Visibility = Visibility.Hidden;
                hour1Temp.Visibility = Visibility.Hidden;
                hour2Label.Visibility = Visibility.Hidden;
                hour2Temp.Visibility = Visibility.Hidden;
                hour3Label.Visibility = Visibility.Hidden;
                hour3Temp.Visibility = Visibility.Hidden;
                hour4Label.Visibility = Visibility.Hidden;
                hour4Temp.Visibility = Visibility.Hidden;
                hour5Label.Visibility = Visibility.Hidden;
                hour5Temp.Visibility = Visibility.Hidden;
                hour6Label.Visibility = Visibility.Hidden;
                hour6Temp.Visibility = Visibility.Hidden;
                hour7Label.Visibility = Visibility.Hidden;
                hour7Temp.Visibility = Visibility.Hidden;
                hour8Label.Visibility = Visibility.Hidden;
                hour8Temp.Visibility = Visibility.Hidden;
                refreshButton.Visibility = Visibility.Hidden;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (cities.SelectedItem != null)
            {
                loadData();
            }
        }

        private void loadData()
        {
            try
            {
                City city = (City)cities.SelectedValue;
                cityName.Content = city.name;
                countryName.Content = city.country;
                Forecast f = LoadForecast(city.id);

                currentTemp.Content = Math.Round(f.list[0].main.temp) + "°C";
                conditionType.Content = f.list[0].weather[0].description;
                string iconID = f.list[0].weather[0].icon;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(@"..\..\..\WeatherIcons\") + iconID + ".png");
                bitmap.EndInit();
                conditionImage.Source = bitmap;

                lastUpdateTime.Content = System.DateTime.Now.ToString();

                int index = getIndex(f.list[0].dt_txt);

                bitmap = new BitmapImage();
                string iconID1 = f.list[index].weather[0].icon;
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(@"..\..\..\WeatherIcons\") + iconID1 + ".png");
                bitmap.EndInit();
                conditionImage1.Source = bitmap;

                bitmap = new BitmapImage();
                string iconID2 = f.list[index + 8].weather[0].icon;
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(@"..\..\..\WeatherIcons\") + iconID2 + ".png");
                bitmap.EndInit();
                conditionImage2.Source = bitmap;

                bitmap = new BitmapImage();
                string iconID3 = f.list[index + 16].weather[0].icon;
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(@"..\..\..\WeatherIcons\") + iconID3 + ".png");
                bitmap.EndInit();
                conditionImage3.Source = bitmap;

                bitmap = new BitmapImage();
                string iconID4 = f.list[index + 24].weather[0].icon;
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(@"..\..\..\WeatherIcons\") + iconID4 + ".png");
                bitmap.EndInit();
                conditionImage4.Source = bitmap;



                day1Label.Content = DateTime.Parse(f.list[index].dt_txt).DayOfWeek;
                day2Label.Content = DateTime.Parse(f.list[index + 8].dt_txt).DayOfWeek;
                day3Label.Content = DateTime.Parse(f.list[index + 16].dt_txt).DayOfWeek;
                day4Label.Content = DateTime.Parse(f.list[index + 24].dt_txt).DayOfWeek;

                day1ContditionLabel.Content = f.list[index].weather[0].description;
                day2ContditionLabel.Content = f.list[index + 8].weather[0].description;
                day3ContditionLabel.Content = f.list[index + 16].weather[0].description;
                day4ContditionLabel.Content = f.list[index + 24].weather[0].description;

                List<double> minmax = getMinMax(f, index - 4);

                minTempDay.Content = Math.Round(minmax[0]) + "°C";
                maxTempDay1.Content = Math.Round(minmax[1]) + "°C";

                minTempDay2.Content = Math.Round(minmax[2]) + "°C";
                maxTempDay2.Content = Math.Round(minmax[3]) + "°C";

                minTempDay3.Content = Math.Round(minmax[4]) + "°C";
                maxTempDay3.Content = Math.Round(minmax[5]) + "°C";

                minTempDay4.Content = Math.Round(minmax[6]) + "°C";
                maxTempDay4.Content = Math.Round(minmax[7]) + "°C";

                hour1Label.Content = f.list[1].dt_txt.Substring(11, 5);
                hour2Label.Content = f.list[2].dt_txt.Substring(11, 5);
                hour3Label.Content = f.list[3].dt_txt.Substring(11, 5);
                hour4Label.Content = f.list[4].dt_txt.Substring(11, 5);
                hour5Label.Content = f.list[5].dt_txt.Substring(11, 5);
                hour6Label.Content = f.list[6].dt_txt.Substring(11, 5);
                hour7Label.Content = f.list[7].dt_txt.Substring(11, 5);
                hour8Label.Content = f.list[8].dt_txt.Substring(11, 5);

                hour1Temp.Content = Math.Round(f.list[1].main.temp) + "°C";
                hour2Temp.Content = Math.Round(f.list[2].main.temp) + "°C";
                hour3Temp.Content = Math.Round(f.list[3].main.temp) + "°C";
                hour4Temp.Content = Math.Round(f.list[4].main.temp) + "°C";
                hour5Temp.Content = Math.Round(f.list[5].main.temp) + "°C";
                hour6Temp.Content = Math.Round(f.list[6].main.temp) + "°C";
                hour7Temp.Content = Math.Round(f.list[7].main.temp) + "°C";
                hour8Temp.Content = Math.Round(f.list[8].main.temp) + "°C";

                cityName.Visibility = Visibility.Visible;
                countryName.Visibility = Visibility.Visible;
                conditionLabel.Visibility = Visibility.Visible;
                currentTemp.Visibility = Visibility.Visible;
                conditionType.Visibility = Visibility.Visible;
                conditionImage.Visibility = Visibility.Visible;
                lastUpdateTime.Visibility = Visibility.Visible;
                lastUpdateLabel.Visibility = Visibility.Visible;
                conditionImage1.Visibility = Visibility.Visible;
                conditionImage2.Visibility = Visibility.Visible;
                conditionImage3.Visibility = Visibility.Visible;
                conditionImage4.Visibility = Visibility.Visible;
                day1Label.Visibility = Visibility.Visible;
                day2Label.Visibility = Visibility.Visible;
                day3Label.Visibility = Visibility.Visible;
                day4Label.Visibility = Visibility.Visible;
                day1ContditionLabel.Visibility = Visibility.Visible;
                day2ContditionLabel.Visibility = Visibility.Visible;
                day3ContditionLabel.Visibility = Visibility.Visible;
                day4ContditionLabel.Visibility = Visibility.Visible;
                minTempDay.Visibility = Visibility.Visible;
                maxTempDay1.Visibility = Visibility.Visible;
                minTempDay2.Visibility = Visibility.Visible;
                maxTempDay2.Visibility = Visibility.Visible;
                minTempDay3.Visibility = Visibility.Visible;
                maxTempDay3.Visibility = Visibility.Visible;
                minTempDay4.Visibility = Visibility.Visible;
                maxTempDay4.Visibility = Visibility.Visible;
                hour1Label.Visibility = Visibility.Visible;
                hour1Temp.Visibility = Visibility.Visible;
                hour2Label.Visibility = Visibility.Visible;
                hour2Temp.Visibility = Visibility.Visible;
                hour3Label.Visibility = Visibility.Visible;
                hour3Temp.Visibility = Visibility.Visible;
                hour4Label.Visibility = Visibility.Visible;
                hour4Temp.Visibility = Visibility.Visible;
                hour5Label.Visibility = Visibility.Visible;
                hour5Temp.Visibility = Visibility.Visible;
                hour6Label.Visibility = Visibility.Visible;
                hour6Temp.Visibility = Visibility.Visible;
                hour7Label.Visibility = Visibility.Visible;
                hour7Temp.Visibility = Visibility.Visible;
                hour8Label.Visibility = Visibility.Visible;
                hour8Temp.Visibility = Visibility.Visible;
                refreshButton.Visibility = Visibility.Visible;
            }
            catch
            {

            }
        }
        
    }
}