using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MyHttpListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpListener listener;
        public ObservableCollection<string> requests = new ObservableCollection<string>();
        public object locker = new object();

        public MainWindow()
        {
            InitializeComponent();

            //Biding
            BindingOperations.EnableCollectionSynchronization(requests, locker);
            Requests.ItemsSource = requests;

            //Listener
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/usd/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Start();

            //Requests
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    HttpListenerContext httpListenerContext = listener.GetContext();
                    ThreadPool.QueueUserWorkItem((_) => ProcessRequest(httpListenerContext));
                }
            });

            //Queue
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    ProcessNextRequest();
                }
            });
        }

        private void ProcessRequest(HttpListenerContext httpListenerContext)
        {
            HttpListenerRequest request = httpListenerContext.Request;
            if ("POST".Equals(request.HttpMethod, StringComparison.InvariantCulture))
            {
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var reader = new System.IO.StreamReader(body, encoding);
                string json = reader.ReadToEnd();

                requests.Add(json);

                httpListenerContext.Response.StatusCode = 202;
                httpListenerContext.Response.StatusDescription = "ACK";
                httpListenerContext.Response.Close();
            }
        }

        private void ProcessNextRequest()
        {
            if (requests.Count > 0)
            {
                var next = requests.FirstOrDefault();
                Thread.Sleep(5000);
                requests.Remove(next);
                //Execute...
            }
        }
    }
}
