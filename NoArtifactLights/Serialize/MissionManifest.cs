// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using System;

namespace NoArtifactLights.Serialize
{
	[Serializable]
	[Obsolete]
	internal struct MissionManifest
	{
		public int Chapter { get; set; }
		public int Version { get; set; }
		public string Name { get; set; }
	}
}
