using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript.InventoryManager
{
    public class Program : MyGridProgram
    {
        private Dictionary<string, ItemInformation> items = new Dictionary<string, ItemInformation>();
        
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var blocksWithInventory = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocksWithInventory, block => block.InventoryCount > 0);

            Clear();
            Count(blocksWithInventory);
            Draw();
        }

        public void Clear()
        {
            items.Clear();
        }

        public void Count(IEnumerable<IMyTerminalBlock> inventoryBlocks)
        {
            var itemsList = new List<MyInventoryItem>();
            foreach (var block in inventoryBlocks)
            {
                var itemList = new List<MyInventoryItem>();
                block.GetInventory(0).GetItems(itemList, item => item.Type.TypeId == "MyObjectBuilder_Ingot");
                itemsList.AddRange(itemList);
                
                if (block.InventoryCount <= 1) continue;
                block.GetInventory(1).GetItems(itemList, item => item.Type.TypeId == "MyObjectBuilder_Ingot");
                itemsList.AddRange(itemList);
            }
            
            foreach (var item in itemsList)
            {
                var info = GetItemInfo(item);

                if (items.ContainsKey(info.ItemSubType))
                {
                    var value = items.GetValueOrDefault(info.ItemSubType);
                    value.ItemCount += info.ItemCount;
                }
                else
                {
                    items.Add(info.ItemSubType, info);
                }
            }
        }

        public void Draw()
        {
            var display = GridTerminalSystem.GetBlockWithName("InventoryDisplay") as IMyTextPanel;
            var builder = new StringBuilder();


            foreach (var item in items.Values.OrderBy(i => i.ItemCount))
            {
                builder.AppendLine(item.ToString());
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

        private class ItemInformation
        {
            public string ItemType { get; set; }
            public string ItemSubType { get; set; }
            public double ItemCount { get; set; }

            public override string ToString()
            {
                return $"{ItemSubType}: {ItemCount.ToString("N2")}";
            }
        }
    }
}