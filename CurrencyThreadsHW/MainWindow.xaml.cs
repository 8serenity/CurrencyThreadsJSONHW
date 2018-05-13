using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace CurrencyThreadsHW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private delegate IList<CurrencyInfo> MyDelegateGetAsyncList();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefreshClicked(object sender, RoutedEventArgs e)
        {
            MyDelegateGetAsyncList myDelegateGetAsyncList = GetCurrencyList;
            IAsyncResult asyncResult = myDelegateGetAsyncList.BeginInvoke(null, null);
            currencyUI.ItemsSource = myDelegateGetAsyncList.EndInvoke(asyncResult);
        }

        private IList<CurrencyInfo> GetCurrencyList()
        {

            IList<CurrencyInfo> searchResults;
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://api.coinmarketcap.com/v2/ticker/?limit=10");

                JObject currencyResponse = JObject.Parse(json);
                var af = currencyResponse.Property("data");

                IList<JToken> results = currencyResponse["data"].Children().ToList();

                searchResults = new List<CurrencyInfo>();

                foreach (JToken result in results)
                {
                    var rChilds = result.Children();
                    foreach (var cInfo in rChilds)
                    {
                        CurrencyInfo newCurrencyInfo = new CurrencyInfo();
                        newCurrencyInfo.Id = (int)cInfo.SelectToken("id");
                        newCurrencyInfo.Rank = (int)cInfo.SelectToken("rank");
                        newCurrencyInfo.Name = cInfo.SelectToken("name").ToString();
                        newCurrencyInfo.Price = double.Parse(cInfo.SelectToken("quotes").SelectToken("USD").SelectToken("price").ToString());
                        searchResults.Add(newCurrencyInfo);
                    }
                }
                return searchResults;
            }
        }
    }
}
