using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Hacks
{
	// This is here so old (alpha) classes convert to the new classes
	internal sealed class EMMItem : GlobalItem
	{
		public sealed override bool NeedsSaving(Item item) => false;

		public sealed override void LoadData(Item item, TagCompound tag)
		{
			LootModItem.GetInfo(item).LoadData(item, tag);
		}

		public sealed override void SaveData(Item item, TagCompound tag)/* tModPorter Suggestion: Edit tag parameter instead of returning new TagCompound */
		{
			return null;
		}
	}

	internal sealed class EMMWorld : ModSystem
	{
		public sealed override void SaveWorldData(TagCompound tag)/* tModPorter Suggestion: Edit tag parameter instead of returning new TagCompound */
		{
			return null;
		}

		public sealed override void LoadWorldData(TagCompound tag)
		{
			ModContent.GetInstance<LootModWorld>().LoadWorldData(tag);
		}
	}
}
