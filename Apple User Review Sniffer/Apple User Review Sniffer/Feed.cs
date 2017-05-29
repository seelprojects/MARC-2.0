using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apple_User_Review_Sniffer
{
    public class Feed
    {
        public Title2 title { get; set; }
        public Icon icon { get; set; }
        public List<Entry> entry { get; set; }
    }
}
