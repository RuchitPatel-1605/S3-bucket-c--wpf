using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3WpfLab1
{
    class bucket
    {
        public string name { get; set; }
        public DateTime date { get; set; }
        public bucket(string name, DateTime date)
        {
            this.name = name;
            this.date = date;
        }
    }
}
