using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
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
using System.Windows.Shapes;

namespace AwsS3WpfLab1
{
    /// <summary>
    /// Interaction logic for createbucket.xaml
    /// </summary>
    public partial class createbucket : Window
    {
        List<bucket> bucketlist = new List<bucket>();

        public createbucket()
        {
            InitializeComponent();
            ListBucket();
        }

        public async void ListBucket()
        {
            bucketlist = new List<bucket>();

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

            var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

            var s3Client = new AmazonS3Client(accessKeyID, secretKey);
            var buckets = await s3Client.ListBucketsAsync();
            //this.lbInfo.Content = "bucket name must not be uppercase and white spaces";

            foreach (var bucket in buckets.Buckets)
            {
                string bucketna = bucket.BucketName;
                DateTime date = bucket.CreationDate;
                bucketlist.Add(new bucket(bucketna, date));
            }
            dataGridListBuckets.ItemsSource = bucketlist;
        }

        // -------------------------------to create bucket----------------------
        private void createBucketClicked(object sender, RoutedEventArgs e)
        {
            int numberOfBucketBefore = bucketlist.Count;
            int numberOfBucketAfter;
            string newName = inputBucketName.Text;
            this.lbInfo.Content = newName;
            if (newName == "")
            {
                this.lbInfo.Content = "Please enter the bucket name...";
                return;
            }
            else
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);

                var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
                var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

                var s3Client = new AmazonS3Client(accessKeyID, secretKey);
                var response = s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = newName
                });
                this.lbInfo.Content = $"adding {newName} bucket";

                inputBucketName.Text = "";
            }

            int mil = 3000;
            
            Task.Run(async () =>
            {
                await Task.Delay(mil);

                // it only works in WPF
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Do something on the UI thread.
                    ListBucket();
                    this.lbInfo.Content = "bucket added successfully...";
                });
            });

        }

        private void goBack(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
