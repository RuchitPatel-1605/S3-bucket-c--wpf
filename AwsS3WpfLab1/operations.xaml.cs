using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AwsS3WpfLab1
{
    /// <summary>
    /// Interaction logic for operations.xaml
    /// </summary>
    public partial class operations : Window
    {

        string filePath = "";
        //bucket class is used here
        List<bucket> bucketlist = new List<bucket>();

        //object class is used here
        List<bucketObject> objectss;
        public operations()
        {
            InitializeComponent();
            ListBucket();
        }

        //to show the list of bucket in combo box
        public async void ListBucket()
        {
            bucketlist = new List<bucket>();

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

            var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

            var s3Client = new AmazonS3Client(accessKeyID, secretKey);
            var buckets = await s3Client.ListBucketsAsync();

            foreach (var bucket in buckets.Buckets)
            {
                string bucketna = bucket.BucketName;
                DateTime date = bucket.CreationDate;
                bucketlist.Add(new bucket(bucketna, date));
                cbCountry.Items.Add(bucketna);
            }
        }


        private void goBack(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }


        private void cbCountry_DropDownClosed(object sender, EventArgs e)
        {
            refreshGrid();
        }

        //when the browse button is clicked
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog1 = new OpenFileDialog();
            if (openfiledialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = openfiledialog1.FileName;
                lbPath.Content = filePath;
            }

        }

        //to refresh the grid - can be used many times
        private async void refreshGrid()
        {
            string cbSelected = cbCountry.SelectionBoxItem.ToString();

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

            var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

            var s3Client = new AmazonS3Client(accessKeyID, secretKey);
            var buckets = await s3Client.ListBucketsAsync();
            objectss = new List<bucketObject>();
            //Objects in buckets...
            foreach (var bucket in buckets.Buckets)
            {
                if (bucket.BucketName == cbSelected)
                {
                    var objects = await s3Client.ListObjectsAsync(bucket.BucketName);
                    if (objects != null)
                    {
                        foreach (var eachObject in objects.S3Objects)
                        {
                            objectss.Add(new bucketObject($"{eachObject.Key}", $"{eachObject.Size}"));
                        }
                    }
                }

            }
            dataGridListObjects.ItemsSource = objectss;
        }

        //when the upload btn is clicked
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            //check if the path is selected
            if (filePath == "") {
                lbInfo.Content = "Please select a path...";
                return;
            } 
            else if(cbCountry.SelectionBoxItem.ToString() == "" || cbCountry.SelectionBoxItem.ToString()==null)  // check if the combo box is selected
            {
                lbInfo.Content = "Please select a Bucket...";
            }
            else
            {
                try
                {
                    FileInfo fi = new FileInfo(filePath);
                    string justFileName = fi.Name;

                    string cbSelected = cbCountry.SelectionBoxItem.ToString();

                    PutObjectRequest putRequest = new PutObjectRequest
                    {
                        BucketName = cbSelected,
                        Key = justFileName,
                        FilePath = filePath,
                        ContentType = "text/plain"
                    };

                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

                    var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
                    var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

                    var s3Client = new AmazonS3Client(accessKeyID, secretKey);
                    s3Client.PutObject(putRequest);
                    lbPath.Content = "";
                    refreshGrid();
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    Console.WriteLine(amazonS3Exception);
                }
            }
        }
    }
}