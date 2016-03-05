using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicDownloader.Models
{
    public class Music
    {
        public string Name { get; set; }

        public string Extension { get; set; }

        public int BitRate { get; set; }

        public string dfsId { get; set; }
    }
}