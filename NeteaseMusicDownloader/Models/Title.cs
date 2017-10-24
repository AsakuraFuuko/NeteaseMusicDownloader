using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicDownloader.Models
{
    public class Title
    {
        public Title()
        {

        }

        public Title(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Url { get { return string.Format("http://music.163.com/song?id={0}", Id); } }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}
