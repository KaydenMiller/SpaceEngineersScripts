using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DoorConfig : Serializable
    {
        private const string SECTION = "Door";
        
        private List<Door> Doors = new List<Door>();
        
        public DoorConfig() : base(SECTION)
        {
        }

        public override void SaveToFields()
        {
            foreach (var door in Doors)
            {
                Fields[$"{SECTION}:{door.Name}"] = new Field(door.Name);
            }
        }

        public override void LoadFields(MyIni ini)
        {
            Doors.Add();
        }
    }
    
    public class Door
    {
        public string Name { get; set; }
        public string[] DoorIds { get; set; }
    }
}