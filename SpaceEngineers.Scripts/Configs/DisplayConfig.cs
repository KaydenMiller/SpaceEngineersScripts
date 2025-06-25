using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DisplayConfig : Serializable
    {
        private const string SECTION = "Displays";

        public Display MainDisplay { get; set; }
        public Display DebugDisplay { get; set; }
        
        public DisplayConfig() : base(SECTION)
        {
        }

        public override void SaveToFields()
        {
            Fields[$"{SECTION}:{nameof(MainDisplay)}"] = new Field(MainDisplay.Name);
            Fields[$"{SECTION}:{nameof(MainDisplay)}.id"] = new Field(MainDisplay.PanelId);
            Fields[$"{SECTION}:{nameof(MainDisplay)}.isPanel"] = new Field(MainDisplay.IsTextPanel);
            
            Fields[$"{SECTION}:{nameof(DebugDisplay)}"] = new Field(DebugDisplay.Name);
            Fields[$"{SECTION}:{nameof(DebugDisplay)}.id"] = new Field(DebugDisplay.PanelId);
            Fields[$"{SECTION}:{nameof(DebugDisplay)}.isPanel"] = new Field(DebugDisplay.IsTextPanel);
        }

        public override void LoadFields(MyIni ini)
        {
            MainDisplay.Name = ini.Get(SECTION, nameof(MainDisplay)).ToString();
            MainDisplay.PanelId = ini.Get(SECTION, $"{nameof(MainDisplay)}.id").ToInt32();
            MainDisplay.IsTextPanel = ini.Get(SECTION, $"{nameof(MainDisplay)}.isPanel").ToBoolean();
            
            DebugDisplay.Name = ini.Get(SECTION, $"{nameof(DebugDisplay)}").ToString();
            DebugDisplay.PanelId = ini.Get(SECTION, $"{nameof(DebugDisplay)}.id").ToInt32();
            DebugDisplay.IsTextPanel = ini.Get(SECTION, $"{nameof(DebugDisplay)}.isPanel").ToBoolean();
        }
    }

    public class Display
    {
        public string Name { get; set; }
        public int PanelId { get; set; }
        public bool IsTextPanel { get; set; }
    }
}