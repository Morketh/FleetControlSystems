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

        bool _outputNow;
        string _textToOutput;
        string _DroneGroupName_;
        string _outputName;
        IMyTextPanel _outputPanel;

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

            // Parse PB Ini File for drones
            MyIniParseResult result;
            if (!_ini.TryParse(Me.CustomData, out result))
                throw new Exception(result.ToString());

            // Get the value of the "outputNow" key under the "demo" section.
            // Then, by calling ToBoolean(), we try to convert it into a
            // bool value.
            _outputNow = _ini.Get("demo", "outputNow").ToBoolean();

            // Get the value of the "output" key. This time we just want the
            // string.
            _outputName = _ini.Get("demo", "output").ToString();

            // Then the final value
            _textToOutput = _ini.Get("demo", "textToOutput").ToString();

            // If the configuration says that the text should be added immediately, so we
            // tell the programmable block to run itself every game tick automatically.
            if (_outputNow)
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update1;
            }

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

            // Block List
            List<IMyTerminalBlock> DroneCMD = new List<IMyTerminalBlock>();
            //Get All Blocks on grid with Type <IMyTextPanel> and filter only [drone] ini style configuration panels
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(DroneCMD, lcd => MyIni.HasSection(lcd.CustomData, "drone"));
            int count = DroneCMD.Count;

            // Loop through Drone Pannels and display data fields on screen
            for(int i = 0; i < count; i++)
            {
                _outputPanel = GridTerminalSystem.GetBlockWithName(DroneCMD[i].CustomName) as IMyTextPanel;
                MyIniParseResult result;
                if (!_ini.TryParse(_outputPanel.CustomData, out result))
                    throw new Exception(result.ToString());
                _DroneGroupName_ = _ini.Get("drone", "name").ToString();
                _outputPanel.ShowPublicTextOnScreen(); //turn text display on
                _outputPanel.CustomName = "[" + _DroneGroupName_ + "] Commands";
                _outputPanel.WritePublicText("Drone Group: " + _DroneGroupName_, false);
                _outputPanel.WritePublicText("\n", true);
            }
        }

    }
}