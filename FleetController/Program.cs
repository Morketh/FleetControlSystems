using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.

        // Instantiate a shared instance of the parser
        MyIni _ini = new MyIni();
        public bool FirstRun = true;

        //List of <DroneBay> Objects
        List<DroneBay> MyDroneBays = new List<DroneBay>();

        //Terminal Block Lists by type
        List<IMyTerminalBlock> DroneCMD = new List<IMyTerminalBlock>();
        List<IMyAirtightHangarDoor> DroneDoors = new List<IMyAirtightHangarDoor>();
        List<IMyTerminalBlock> BlockCache = new List<IMyTerminalBlock>();

        //Whip's Profiler Graph Code
        int count = 1;
        int maxSeconds = 30;
        StringBuilder profile = new StringBuilder();
        bool hasWritten = false;
        void ProfilerGraph()
        {
            if (count <= maxSeconds * 60)
            {
                double timeToRunCode = Runtime.LastRunTimeMs;

                profile.Append(timeToRunCode.ToString()).Append("\n");
                count++;
            }
            else if (!hasWritten)
            {
                var screen = GridTerminalSystem.GetBlockWithName("DEBUG") as IMyTextPanel;
                screen?.WritePublicText(profile.ToString());
                screen?.ShowPublicTextOnScreen();
                if (screen != null)
                    hasWritten = true;
            }
        }

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //
            // The constructor is optional and can be removed if not
            // needed.
            //
            // It's recommended to set RuntimeInfo.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        //Get blocks -> foreach (var block in blocks) {moduleX.CheckBlock(block);}

        // MAIN PROGRAM LOOP //
        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.

            ProfilerGraph();
            
            if(FirstRun == true || argument == (string)"refresh")
            {
                FirstRun = false;
                GridTerminalSystem.GetBlocks(BlockCache);
            }
            
            MyIniParseResult result;
            
            int TotalBlocks = BlockCache.Count;
            //MyIniParseResult result;
            
            for (int i = 0; i < TotalBlocks; i++)
            {
                var hangDoor = BlockCache[i] as IMyAirtightHangarDoor;

                // its a door and its open so close it.
                if (hangDoor != null && hangDoor.OpenRatio > 0.9)
                {
                    // if door is grouped under miner-1 close it and nothing else
                    MyIni.HasSection(hangDoor.CustomData, "drone");
                    if (_ini.TryParse(hangDoor.CustomData, out result) && _ini.Get("drone", "name").ToString().ToLower() == (string)"miner-1")
                    {
                        hangDoor.CloseDoor();
                    }
                } else
                {

                }
            }

            // Loop through Drone Pannels and display data fields on screen
            Echo("Program init");
        }

    }
}