using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI;

namespace IngameScript.Airlock
{
    public class Program : MyGridProgram
    {
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Trigger)
            {
                var sensor = GridTerminalSystem.GetBlockWithId(12);
                sensor.
            }
            var airlockDoors = 
            // Vent
            // X Doors
            
            // Can only open 

        }
    }
}