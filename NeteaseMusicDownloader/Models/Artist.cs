using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicDownloader.Models
{
    public class Artists
    {
        List<Artist> _artists = new List<Artist>();

        public List<Artist> Data
        {
            set { _artists = value; }
            get { return _artists; }
        }

        public Artists()
        {

        }

        public Artists(string[] names)
        {
            _artists.Clear();

            foreach (var name in names)
            {
                _artists.Add(new Artist(name));
            }
        }

        public void Add(Artist artist)
        {
            _artists.Add(artist);
        }

        public override string ToString()
        {
            return string.Join("&", Data.Select(ar => ar.Name));
        }
    }

    public class Artist
    {
        public Artist()
        {

        }

        public Artist(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Url { get { return string.Format("http://music.163.com/artist?id={0}", Id); } }
    }
}
