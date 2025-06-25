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
    public class DisplayManager
    {
        private readonly MyGridProgram _program;
        private readonly Dictionary<string, IMyTextSurface> _displays = new Dictionary<string, IMyTextSurface>();

        public DisplayManager(MyGridProgram program)
        {
            _program = program;
        }

        public IMyTextSurface GetDisplay(string displayKey)
        {
            return _displays.GetValueOrDefault(displayKey);
        }
        
        public void RegisterDisplays(DisplayConfig displayConfig)
        {
            _displays.Add(nameof(displayConfig.MainDisplay), RegisterDisplay(displayConfig.MainDisplay));
            _displays.Add(nameof(displayConfig.DebugDisplay), RegisterDisplay(displayConfig.DebugDisplay));
        }
        
        private IMyTextSurface RegisterDisplay(Display display)
        {
            IMyTextSurfaceProvider surfaceProvider = null;

            // The display is the current block
            if (string.IsNullOrWhiteSpace(display.Name))
            {
                surfaceProvider = _program.Me;
                _program.Echo("Loading 'Me' as surface provider");
            }
            
            // Is the display a text panel, since text panels are surfaces by default we can just return
            if (display.IsTextPanel)
            {
                return _program.GridTerminalSystem.GetBlockWithName(display.Name) as IMyTextPanel;
            }

            if (surfaceProvider == null)
            {
                surfaceProvider = _program.GridTerminalSystem.GetBlockWithName(display.Name) as IMyTextSurfaceProvider;
            }

            if (surfaceProvider == null) throw new ArgumentNullException($"No Surface Provided: '{display.Name}'");
            if (surfaceProvider.SurfaceCount <= 0) throw new Exception("Surface Provider has no displays");
            if (display.PanelId > surfaceProvider.SurfaceCount - 1) throw new Exception($"Surface Provider does not have '{display.PanelId+1}' display(s)");

            _program.Echo($"Successfully loaded display '{display.PanelId}' from '{display.Name}");
            return surfaceProvider.GetSurface(display.PanelId);
        } 
    }
}