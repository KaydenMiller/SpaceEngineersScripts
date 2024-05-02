using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript.HangerDoors
{

    public class Program : MyGridProgram
    {
        private const int TICKS_PER_SECOND = 60;
        private const int UPDATE_TICK_FREQUENCY = 10;
        private const int SECONDS_TO_WAIT = 1;
        private bool _opening = false;
        private bool LightStateAlert = false;
        private const int MAX_BLINKS = 5;
        private int _currentBlinks = 0;
        private int DELAY_BETWEEN_BLINKS_MAX_COUNT = (SECONDS_TO_WAIT * TICKS_PER_SECOND) / UPDATE_TICK_FREQUENCY;
        private int _blinkDelayCount = 0;
        private Color InternalWhite = new Color(255, 240, 230);
        
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Once | UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Trigger)
            {
                var portHangarDoorGroup = GridTerminalSystem.GetBlockGroupWithName("Port Hangar Door");
                var portBayDoorRotor = GridTerminalSystem.GetBlockWithName("Port Bay Door Rotor") as IMyMotorStator;
                List<IMyAirtightHangarDoor> portDoors = new List<IMyAirtightHangarDoor>();
            
                var starboardHangarDoorGroup = GridTerminalSystem.GetBlockGroupWithName("Starboard Hangar Door");
                var starboardBayDoorRotor = GridTerminalSystem.GetBlockWithName("Starboard Bay Door Rotor") as IMyMotorStator;
                List<IMyAirtightHangarDoor> starboardDoors = new List<IMyAirtightHangarDoor>();
            
                if (portHangarDoorGroup != null && portDoors != null)
                {
                    portHangarDoorGroup.GetBlocksOfType(portDoors);
                    starboardHangarDoorGroup.GetBlocksOfType(starboardDoors);
                    Echo($"There are: {portDoors.Count + starboardDoors.Count}");
                }

                _opening = true;
                BlinkLights();
                OpenClose(argument, starboardDoors, portDoors, starboardBayDoorRotor, portBayDoorRotor); 
            }
            else if (updateSource == UpdateType.Update10)
            {
                BlinkLights();
                if (_opening == false)
                {
                    
                }
            }
        }

        public void BlinkLights()
        {
            if (_opening == false)
            {
                SetLightToColor(InternalWhite);
                return;
            }
            if (_blinkDelayCount++ < DELAY_BETWEEN_BLINKS_MAX_COUNT) return;
            _blinkDelayCount = 0;
            
            if (_currentBlinks >= MAX_BLINKS)
            {
                _opening = false;
                _currentBlinks = 0;
            }
            
            _currentBlinks++;

            if (!LightStateAlert)
            {
                LightStateAlert = true;
                SetLightToColor(Color.Red);
            }
            else
            {
                LightStateAlert = false;
                SetLightToColor(InternalWhite);
            }
        }

        private void SetLightToColor(Color color)
        {
            var portLightsGroup = GridTerminalSystem.GetBlockGroupWithName("Port Bay Door Corner Lights");
            var starboardLightsGroup = GridTerminalSystem.GetBlockGroupWithName("Starboard Bay Door Corner Lights");

            List<IMyLightingBlock> portLights = new List<IMyLightingBlock>();
            List<IMyLightingBlock> starboardLights = new List<IMyLightingBlock>();

            portLightsGroup.GetBlocksOfType<IMyLightingBlock>(portLights);
            starboardLightsGroup.GetBlocksOfType<IMyLightingBlock>(starboardLights);

            foreach (var light in portLights)
                light.Color = color;
            foreach (var light in starboardLights)
                light.Color = color;
        }

        public void OpenClose(string argument, List<IMyAirtightHangarDoor> starboardDoors, List<IMyAirtightHangarDoor> portDoors, IMyMotorStator starboardRotor, IMyMotorStator portRotor)
        {
            var args = argument.Split(':');
            var side = args[0]; // starboard, port
            var action = args[1]; // open, close
            var hanger = args[2]; // top, bottom
            var portVelocityOpen = 1f;
            var starboardVelocityOpen = -1f;
            var velocity = 0f;

            List<IMyAirtightHangarDoor> doorsToAffect = new List<IMyAirtightHangarDoor>();
            IMyMotorStator rotorToAffect = null;
            if (side == "port")
            {
                doorsToAffect = portDoors;
                rotorToAffect = portRotor;
                if (action == "open")
                    velocity = portVelocityOpen;
                else
                    velocity = -portVelocityOpen;
            }
            else if (side == "starboard")
            {
                doorsToAffect = starboardDoors;
                rotorToAffect = starboardRotor;

                if (action == "open")
                    velocity = starboardVelocityOpen;
                else
                    velocity = -starboardVelocityOpen;
            }

            if (hanger == "top")
            {
                if (action == "open")
                {
                    Echo("Opening Top!"); 
                    foreach (var door in doorsToAffect)
                    {
                        door.OpenDoor();
                    }  
                }
                else
                {
                    Echo("Closing Top!");
                    foreach (var door in doorsToAffect)
                    {
                        door.CloseDoor();
                    }    
                }
            }
            else if (hanger == "bottom")
            {
                if (action == "open")
                {
                    Echo("Opening Bottom!");
                    rotorToAffect.TargetVelocityRPM = velocity;
                }
                else
                {
                    Echo("Closing Bottom!");
                    rotorToAffect.TargetVelocityRPM = velocity;
                }
            }
        }
    }
}