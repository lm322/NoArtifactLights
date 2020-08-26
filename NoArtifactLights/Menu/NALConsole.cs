// Sources from SHVDN core ASI
// Copyright (C) crosire and contributors 2015-2020

// NAL Commands & Registers
// Copyright (C) RelaperCrystal 2020

using GTA;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Menu
{
	public class NALConsole //: Script
	{
		int cursorPos = 0;
		int commandPos = -1;
		int currentPage = 1;
		bool isOpen = false;
		string input = string.Empty;
		List<string> lineHistory = new List<string>();
		List<string> commandHistory = new List<string>();
		ConcurrentQueue<string[]> outputQueue = new ConcurrentQueue<string[]>();
		Dictionary<string, List<ConsoleCommand>> commands = new Dictionary<string, List<ConsoleCommand>>();
		DateTime lastClosed;
		const int BASE_WIDTH = 1280;
		const int BASE_HEIGHT = 720;
		const int CONSOLE_WIDTH = BASE_WIDTH;
		const int CONSOLE_HEIGHT = BASE_HEIGHT / 3;
		const int INPUT_HEIGHT = 20;
		const int LINES_PER_PAGE = 16;

		static readonly Color InputColor = Color.White;
		static readonly Color InputColorBusy = Color.DarkGray;
		static readonly Color OutputColor = Color.White;
		static readonly Color PrefixColor = Color.FromArgb(255, 52, 152, 219);
		static readonly Color BackgroundColor = Color.FromArgb(200, Color.Black);
		static readonly Color AltBackgroundColor = Color.FromArgb(200, 52, 73, 94);

		[DllImport("user32.dll")]
		static extern int ToUnicode(
		uint virtualKeyCode, uint scanCode, byte[] keyboardState,
		[Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] StringBuilder receivingBuffer, int bufferSize, uint flags);

		
	}
}
