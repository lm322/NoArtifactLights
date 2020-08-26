// NoArtifactLights
// (C) RelaperCrystal 2020

using System;

namespace NoArtifactLights.Menu
{
    public delegate void ConsoleCommandEventHandler(string[] paramaters);

    public class ConsoleCommand
    {
        public ConsoleCommand(string name)
        {
            Name = name;
        }

        public string Name { get; internal set; }

        internal void Input(params string[] parameters) => Activated(parameters);
        public event ConsoleCommandEventHandler Activated;
    }
}