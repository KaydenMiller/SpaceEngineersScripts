using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript.AutomaticDoorChecker
{
    public class Program : MyGridProgram 
    {
        private const int TICKS_PER_SECOND = 60;
        private const int UPDATE_TICK_FREQUENCY = 100;
        private const int SECONDS_TO_WAIT = 5;
        private int DELAY_COUNT = (SECONDS_TO_WAIT * TICKS_PER_SECOND) / UPDATE_TICK_FREQUENCY;
        public int _currentCount = 0;
        private bool _ready = false;

        private IMyTextSurface _display;
        
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            var ini = new MyIni();
            ini.TryParse(Me.CustomData);
            var displayId = ini.Get("Output", "display").ToInt32();
            _display = Me.GetSurface(displayId);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var doors = new List<IMyDoor>();
            var airlockDoorGroup = GridTerminalSystem.GetBlockGroupWithName("Airlock Doors");
            airlockDoorGroup.GetBlocksOfType<IMyDoor>(doors);
            Echo($"Airlock Doors Found: {doors.Count}");
            Write(_display, $"Airlock Doors Found: {doors.Count}");
            

            var openDoors = new List<IMyDoor>();
            foreach (var door in doors)
            {
                if (door.Status == DoorStatus.Closed) continue;
                Echo($"The {door.Name} is currently open");
                Write(_display, $"The {door.Name} is currently open");
                openDoors.Add(door);
            }

            Echo($"There are currently {openDoors.Count} open");
            Write(_display, $"There are currently {openDoors.Count} open");
            
            // Are we ready to star the counter?
            if (openDoors.Count > 0 && !_ready)
            {
                Echo("Start the timer!");
                _ready = true;
                _currentCount = 0;
            }
            
            // Are we passed the delay?
            if (!_ready || _currentCount++ < DELAY_COUNT) return;
            _currentCount = 0;
            
            foreach (var door in openDoors)
            {
                door.CloseDoor();
            }

            _ready = false;
        }

        private void Write(IMyTextSurface display, string textToWrite)
        {
            if (display == null) return;
            display.WriteText(textToWrite);
            display.DrawFrame();
        }
    }
}