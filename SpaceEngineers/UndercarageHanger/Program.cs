using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript.UndercarageHanger
{
    public class Program : MyGridProgram
    {
        // OPENED
        // 1. close sliding doors
        // 2. turn off the interior light
        // 3. turn on alert light
        // 4. start depressurizing
        // 5. after depressurizing complete open hanger door
        
        // CLOSED
        // 1. bay door closed
        // 2. close sliding doors
        // 3. start pressurizing
        // 4. after pressurizing
        // 5. turn off alert light
        // 6. turn on the interior light
        
        // DISPLAY
        // place each current step on the display panel
        
        private enum HangerStatus
        {
            Open,
            Opening,
            Closing,
            Closed,
            Unknown,
        }

        private const string INI_LIGHTS_GROUP = "Lights";
        private const string INI_HANGER_GROUP = "Hanger";

        private HangerStatus Status = HangerStatus.Unknown;

        private Dictionary<string, IMyTextSurface> _displays = new Dictionary<string, IMyTextSurface>();
        private Dictionary<string, List<IMyDoor>> _doors = new Dictionary<string, List<IMyDoor>>();
        
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            var ini = new MyIni();
            if (ini.TryParse(Me.CustomData))
            {
                Echo("Data Load Successful");

                if (ini.ContainsSection("External"))
                {
                    Echo("Contains External DataSource");
                    var externalDataSource = ini.Get("External", "DataSource").ToString();
                    if (!string.IsNullOrWhiteSpace(externalDataSource))
                    {
                        var externalData = GridTerminalSystem.GetBlockWithName(externalDataSource);
                        if (ini.TryParse(externalData.CustomData))
                            Echo("External Data Load Successful");
                        else
                            Echo("External Data Load Failed");
                        Echo(externalData.CustomData);
                    }
                }
                else
                {
                    Echo("No External DataSource");
                }
                
                RegisterDisplays(ini);
                RegisterDoors(ini);
            }
            else
            {
                Echo("Data Failed To Load");
                Echo(Me.CustomData);
            }
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            var airlockDoors = _doors.GetValueOrDefault("Airlock") ?? new List<IMyDoor>();
            var main = _displays.GetValueOrDefault("MainDisplay");
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

        private void RegisterDisplays(MyIni ini)
        {
            const string INI_OUTPUT_MAIN = "MainDisplay";
            const string INI_OUTPUT_DEBUG = "DebugDisplay";
            
            _displays.Add(INI_OUTPUT_MAIN, RegisterDisplay(ini, INI_OUTPUT_MAIN));
            _displays.Add(INI_OUTPUT_DEBUG, RegisterDisplay(ini, INI_OUTPUT_DEBUG));
        }

        private void RegisterDoors(MyIni ini)
        {
            const string INI_DOORS = "Doors";
            _doors.Add("Airlock", RegisterDoorGroup(ini, INI_DOORS, "Airlock"));
        }

        private List<IMyDoor> RegisterDoorGroup(MyIni ini, string doorSectionName, string iniRoot)
        {
            var doors = new List<IMyDoor>();

            var groupName = ini.Get(doorSectionName, GetPropertyName(iniRoot, "group")).ToString();
            var group = GridTerminalSystem.GetBlockGroupWithName(groupName);
            group.GetBlocksOfType(doors);
            
            return doors;
        }

        private IMyTextSurface RegisterDisplay(MyIni ini, string iniRoot)
        {
            const string INI_OUTPUT = "Output";

            var displayBlock = ini.Get(INI_OUTPUT, iniRoot).ToString();
            var displayIsTextPanel = ini.Get(INI_OUTPUT, GetPropertyName(iniRoot, "isPanel")).ToBoolean();
            var displayId = ini.Get(INI_OUTPUT, GetPropertyName(iniRoot, "id")).ToInt32();
            IMyTextSurfaceProvider surfaceProvider = null;

            // The display is the current block
            if (string.IsNullOrWhiteSpace(displayBlock))
            {
                surfaceProvider = Me;
                Echo("Loading 'Me' as surface provider");
            }
            
            // Is the display a text panel, since text panels are surfaces by default we can just return
            if (displayIsTextPanel)
            {
                Echo($"Display '{iniRoot}' is a TextPanel");
                return GridTerminalSystem.GetBlockWithName(displayBlock) as IMyTextPanel;
            }

            if (surfaceProvider == null)
            {
                Echo($"Loading '{displayBlock}' as surface provider");
                surfaceProvider = GridTerminalSystem.GetBlockWithName(displayBlock) as IMyTextSurfaceProvider;
            }

            if (surfaceProvider == null) throw new ArgumentNullException($"No Surface Provided: '{iniRoot}.{displayBlock}'");
            if (surfaceProvider.SurfaceCount <= 0) throw new Exception("Surface Provider has no displays");
            if (displayId > surfaceProvider.SurfaceCount - 1) throw new Exception($"Surface Provider does not have '{displayId+1}' display(s)");

            Echo($"Successfully loaded display '{displayId}' from '{displayBlock}");
            return surfaceProvider.GetSurface(displayId);
        }

        private string GetPropertyName(string iniRoot, string iniProperty) => $"{iniRoot}.{iniProperty}";
    }
}
