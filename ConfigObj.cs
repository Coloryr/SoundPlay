using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundPlay
{
    public class SoundItem 
    {
        public int Index { get; set; }
        public string Local { get; set; }
        public bool AutoNext { get; set; }
        public bool Have { get; set; }
    }
    public class ConfigObj
    {
        public List<SoundItem> Sounds { get; set; }
    }
}
