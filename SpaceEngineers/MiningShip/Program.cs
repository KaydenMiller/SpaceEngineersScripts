using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript.InventoryManager.MiningShip
{
    public class Program : MyGridProgram
    {
        private Dictionary<string, ItemInformation> _ore = new Dictionary<string, ItemInformation>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var blocksWithInventory = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocksWithInventory,
                block => block.InventoryCount > 0 && block.IsSameConstructAs(Me));
            blocksWithInventory = blocksWithInventory.Where(block => block.CustomData != "ignore").ToList();

            var cap = GetShipCapacityInformation(blocksWithInventory);

            Clear();
            CountRawResources(blocksWithInventory);
            Draw("InfoDisplayPanel", _ore.Select(x => x.Value), cap, true);
        }

        public void Clear()
        {
            _ore.Clear();
        }

        public CapacityInformation GetShipCapacityInformation(IEnumerable<IMyTerminalBlock> blocksWithInventory)
        {
            var blockInventoryInfo = blocksWithInventory
               .Select(block =>
                {
                    var mainInventory = block.GetInventory(0);
                    return new CapacityInformation((double)mainInventory.MaxVolume,
                        (double)mainInventory.CurrentVolume);
                }).ToArray();
                
            var shipMaxVolume = blockInventoryInfo.Sum(info => info.Max);
            var shipCurrentVolume = blockInventoryInfo.Sum(info => info.Used);

            return new CapacityInformation(shipMaxVolume, shipCurrentVolume);
        }

        public void CountRawResources(IEnumerable<IMyTerminalBlock> inventoryBlocks)
        {
            var itemsList = new List<MyInventoryItem>();
            foreach (var block in inventoryBlocks)
            {
                var itemList = new List<MyInventoryItem>();
                block.GetInventory(0)
                   .GetItems(itemList, item => item.Type.TypeId == "MyObjectBuilder_Ore");
                itemsList.AddRange(itemList);

                if (block.InventoryCount <= 1) continue;
                block.GetInventory(1)
                   .GetItems(itemList, item => item.Type.TypeId == "MyObjectBuilder_Ore");
                itemsList.AddRange(itemList);
            }

            foreach (var item in itemsList)
            {
                var info = GetItemInfo(item);

                if (_ore.ContainsKey(info.ItemSubType))
                {
                    var value = _ore.GetValueOrDefault(info.ItemSubType);
                    value.ItemCount += info.ItemCount;
                }
                else
                {
                    _ore.Add(info.ItemSubType, info);
                }
            }
        }

        private string DrawPercentageBar(CapacityInformation capacityInformation, int displayWidth = 10)
        {
            const int EXTRA_CHARACTERS = 2;
            var maxPercentBars = displayWidth - EXTRA_CHARACTERS;
            var storagePercent = capacityInformation.PercentageUtilized;
            var neededPercentBars = maxPercentBars * storagePercent;
            var neededPadding = maxPercentBars - neededPercentBars;
            
            
            StringBuilder builder = new StringBuilder();
            builder.Append('[');

            for (var currentPercentBar = 0; currentPercentBar < neededPercentBars; currentPercentBar++)
            {
                builder.Append("|");
            }

            for (var padding = 0; padding < neededPadding; padding++)
            {
                builder.Append(" ");
            }
            
            builder.Append(']');
            
            return builder.ToString();
        }

        /// <summary>
        /// We can only draw a max of 10 lines currently
        /// </summary>
        public void Draw(string displayName, IEnumerable<ItemInformation> items, CapacityInformation capacityInformation, bool useOreRatios = false)
        {
            var display = GridTerminalSystem.GetBlockWithName(displayName) as IMyTextPanel;
            var builder = new StringBuilder();
            display.Font = "Monospace";
            display.FontSize = 1f;

            builder.AppendLine($"Capacity: {capacityInformation.PercentageUtilized:P}");
            builder.AppendLine(DrawPercentageBar(capacityInformation, 25));

            foreach (var item in items.OrderBy(i => i.ItemCount))
            {
                builder.AppendLine(item.ToString(useOreRatios));
            }

            display.WriteText(builder.ToString());
            display.DrawFrame();
        }

        private ItemInformation GetItemInfo(MyInventoryItem item)
        {
            return new ItemInformation()
            {
                ItemType = item.Type.TypeId,
                ItemSubType = item.Type.SubtypeId,
                ItemCount = (double)item.Amount
            };
        }

        public class ItemInformation
        {
            public string ItemType { get; set; }
            public string ItemSubType { get; set; }
            public double ItemCount { get; set; }

            public override string ToString()
            {
                return $"{ItemSubType,9}: {ConvertNumberShorthand(ItemCount),4}";
            }

            public string ToString(bool useRatios)
            {
                if (useRatios)
                {
                    var ratio = GetOreRatio(ItemSubType);
                    var refinedValue = ItemCount * ratio;
                    return
                        $"{ItemSubType,9}: {ConvertNumberShorthand(ItemCount),4} => {ConvertNumberShorthand(refinedValue),4}";
                }

                return $"{ItemSubType,9}: {ConvertNumberShorthand(ItemCount),4}";
            }
        }

        public class CapacityInformation
        {
            public double Max { get; }
            public double Used { get; }

            public double PercentageUtilized => Used / Max;

            public CapacityInformation(double max, double used)
            {
                Max = max;
                Used = used;
            }
        }

        private static string ConvertNumberShorthand(double num)
        {
            if (num >= 1000000)
                return string.Concat(Math.Round(num / 1000000), "M");
            if (num >= 1000)
                return string.Concat(Math.Round(num / 1000), "K");
            return Math.Round(num).ToString("N0");
        }

        private static float GetOreRatio(string oreType)
        {
            switch (oreType)
            {
                case "Gold":
                    return 0.01f;
                case "Silver":
                    return 0.10f;
                case "Platinum":
                    return 0.0048f;
                case "Cobalt":
                    return 0.30f;
                case "Iron":
                    return 0.70f;
                case "Magnesium":
                    return 0.007f;
                case "Nickle":
                    return 0.40f;
                case "Silicon":
                    return 0.70f;
                case "Uranium":
                    return 0.01f;
                case "Stone":
                    return 0f;
                default:
                    return 0f;
            }
        }
    }
}