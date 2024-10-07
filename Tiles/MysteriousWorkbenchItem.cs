using Terraria;
using Terraria.ModLoader;

namespace Loot.Tiles
{
	internal sealed class MysteriousWorkbenchItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("MysteriousWorkbenchItem");
		}

		public override void SetDefaults()
		{
			Item.useStyle = 1;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 99;
			Item.consumable = true;
			Item.width = 40;
			Item.height = 30;
			Item.value = Item.sellPrice(0, 0, 0, 0);
			Item.createTile = ModContent.TileType<MysteriousWorkbench>();
		}
	}
}
