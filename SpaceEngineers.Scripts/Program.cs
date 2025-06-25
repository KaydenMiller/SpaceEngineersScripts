using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;
using VRageRender.Messages;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private readonly DisplayManager _displayManager;
        private readonly DoorManager _doorManager;
        private HangerStatus Status = HangerStatus.Unknown;
        
        private enum HangerStatus
        {
            Open,
            Opening,
            Closing,
            Closed,
            Unknown,
        }
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            var externalConfig = IniSerializer.Deserialize<ExternalConfig>(Me.CustomData);
            var externalData = GridTerminalSystem.GetBlockWithName(externalConfig.DataSource);
            var displayConfig = IniSerializer.Deserialize<DisplayConfig>(externalData.CustomData);
            var doorConfig = IniSerializer.Deserialize<DoorConfig>(externalData.CustomData);
            
            _displayManager = new DisplayManager(this);
            _displayManager.RegisterDisplays(displayConfig);
            _doorManager = new DoorManager(this);
            _doorManager.RegisterDoors(doorConfig);

            // RegisterDoors();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var airlockDoors = _doors.GetValueOrDefault("Airlock") ?? new List<IMyDoor>();
            var main = _displayManager.GetDisplay("MainDisplay");
            main.ContentType = ContentType.TEXT_AND_IMAGE;
            
            if (updateSource == UpdateType.Trigger)
            {
                if (argument == "open")
                {
                    Status = HangerStatus.Opening;
                    main.WriteText("Hanger Status: Opening");
                }
                if (argument == "close")
                {
                    Status = HangerStatus.Closing;
                    main.WriteText("Hanger Status: Closing");
                }
            }
            else if (Status == HangerStatus.Opening)
            {
                var doors = airlockDoors.Where(d => d.Status == DoorStatus.Open).ToArray();
                if (!doors.Any()) Status = HangerStatus.Open;
                foreach (var airlockDoor in doors)
                {
                    airlockDoor.ToggleDoor();
                }
            }
            else if (Status == HangerStatus.Closing)
            {
                var doors = airlockDoors.Where(d => d.Status == DoorStatus.Open).ToArray();
                if (!doors.Any()) Status = HangerStatus.Closed;
                foreach (var airlockDoor in doors)
                {
                    airlockDoor.ToggleDoor();
                }
            }
            else
            {
                main.WriteText($"Hanger Status: {Status.ToString()}");
            }
        }
    }
}