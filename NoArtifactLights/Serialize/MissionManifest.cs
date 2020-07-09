using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Serialize
{
    [Serializable]
    internal struct MissionManifest
    {
        public int Chapter { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
    }
}
