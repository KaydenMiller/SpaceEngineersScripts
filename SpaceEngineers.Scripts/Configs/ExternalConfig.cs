using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class ExternalConfig : Serializable
    {
        private const string SECTION = "External";
        public string DataSource { get; set; }
        
        public ExternalConfig() : base(SECTION)
        {
        }

        public override void SaveToFields()
        {
            Fields[$"{SECTION}:{nameof(DataSource)}"] = new Field(DataSource);
        }

        public override void LoadFields(MyIni ini)
        {
            DataSource = ini.Get(SECTION, nameof(DataSource)).ToString();
        }
    }
}