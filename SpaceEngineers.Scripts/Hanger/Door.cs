using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DoorDto : Serializable
    {
        private const string SECTION = "Door";
        public string Id { get; set; }

        public DoorDto() : base(SECTION)
        {
        }
        
        public override void SaveToFields()
        {
            Fields[$"{SECTION}:{nameof(Id)}"] = new Field(Id);
        }

        public override void LoadFields(MyIni ini)
        {
            Id = ini.Get(SECTION, nameof(Id)).ToString();
        }
    }
}