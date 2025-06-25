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
    public class DoorManager
    {
        private readonly MyGridProgram _program;
        private readonly Dictionary<string, Door> _doors = new Dictionary<string, Door>();

        public DoorManager(MyGridProgram program)
        {
            _program = program;
        }
        
        public void RegisterDoors(DoorConfig config)
        {
            _doors.Add("Airlock", RegisterDoorGroup(ini, INI_DOORS, "Airlock"));
        }

        private List<IMyDoor> RegisterDoorGroup(MyIni ini, string doorSectionName, string iniRoot)
        {
            var doors = new List<IMyDoor>();

            var groupName = ini.Get(doorSectionName, GetPropertyName(iniRoot, "group")).ToString();
            var group = _program.GridTerminalSystem.GetBlockGroupWithName(groupName);
            group.GetBlocksOfType(doors);
            
            return doors;
        } 
    }
}