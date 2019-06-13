using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Models
{
    public enum Platforms { Steam, Epic, Origin, Bethesda, UPlay, None }

    public class Platform
    {
        public string Name { get; set; }
        public Platforms PlatformType { get; set; }
        public string InstallationPath { get; set; }
    }
}
