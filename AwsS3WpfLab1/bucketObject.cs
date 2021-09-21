using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3WpfLab1
{
    
    class bucketObject
    {
        public string key { get; set; }
        public string size { get; set; }
        public bucketObject(string key, string size)
        {
            this.key = key;
            this.size = size;
        }
    }
}
