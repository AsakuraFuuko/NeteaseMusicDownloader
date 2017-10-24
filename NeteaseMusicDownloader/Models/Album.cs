using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicDownloader.Models
{
    public class Album
    {
        public Album()
        {

        }

        public Album(string name, string image)
        {
            Name = name;
            Image = image;
        }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Id { get; set; }

        public string Url { get { return string.Format("http://music.163.com/album?id={0}", Id); } }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}
