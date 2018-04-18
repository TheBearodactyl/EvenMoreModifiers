﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Loot
{
	/// <summary>
	/// Defines an item that may be modified by modifiers from mods
	/// </summary>
	public sealed class EMMItem : GlobalItem
	{
		// Helpers
		public static EMMItem GetItemInfo(Item item) => item.GetGlobalItem<EMMItem>();
		public static IEnumerable<Modifier> GetActivePool(Item item) => GetItemInfo(item)?.ModifierPool?.ActiveModifiers ?? Enumerable.Empty<Modifier>();

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public ModifierPool ModifierPool;
		public bool HasRolled;

		/// <summary>
		/// Attempts to roll new modifiers
		/// Has a set chance to hit a predefined pool of modifiers
		/// </summary>
		internal ModifierPool RollNewPool(ModifierContext ctx)
		{
			// Try getting a weighted pool, or roll all modifiers at random
			bool rollPredefinedPool = Main.rand.NextFloat() <= 0.25f;
			bool canRollRandom = !rollPredefinedPool;
			if (rollPredefinedPool)
			{
				ModifierPool = EMMLoader.GetWeightedPool(ctx);
				if (ModifierPool == null)
					canRollRandom = true;
			}

			if (canRollRandom)
				ModifierPool = Loot.Instance.GetModifierPool<AllModifiersPool>();

			// Now we have actually rolled
			HasRolled = true;

			// Attempt rolling modifiers
			if (!ModifierPool.RollModifiers(ctx))
				ModifierPool = null; // reset (didn't roll anything)
			else
				ModifierPool.UpdateRarity();

			return ModifierPool;
		}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			EMMItem clone = (EMMItem)base.Clone(item, itemClone);
			clone.ModifierPool = (ModifierPool)ModifierPool?.Clone();
			// there is no need to apply here, we already cloned the item which stats are already modified by its pool
			//clone.ModifierPool?.ApplyModifiers(itemClone);
			return clone;
		}

		public override void Load(Item item, TagCompound tag)
		{
			if (tag.ContainsKey("Type"))
				ModifierPool = ModifierPool._Load(item, tag);

			HasRolled = tag.GetBool("HasRolled");

			ModifierPool?.ApplyModifiers(item);
		}

		public override TagCompound Save(Item item)
		{
			TagCompound tag = ModifierPool != null
				? ModifierPool.Save(ModifierPool)
				: new TagCompound();

			tag.Add("HasRolled", HasRolled);

			return tag;
		}

		public override bool NeedsSaving(Item item)
			=> ModifierPool != null || HasRolled;

		public override void NetReceive(Item item, BinaryReader reader)
		{
			if (reader.ReadBoolean())
				ModifierPool = ModifierPool._NetReceive(item, reader);

			HasRolled = reader.ReadBoolean();

			ModifierPool?.ApplyModifiers(item);
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			bool hasPool = ModifierPool != null;
			writer.Write(hasPool);
			if (hasPool)
				ModifierPool._NetSend(ModifierPool, item, writer);

			writer.Write(HasRolled);
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnCraft,
				Item = item,
				Player = Main.LocalPlayer,
				Recipe = recipe
			};

			ModifierPool pool = GetItemInfo(item).ModifierPool;
			if (!HasRolled && pool == null)
			{
				pool = RollNewPool(ctx);
				pool?.ApplyModifiers(item);
			}

			base.OnCraft(item, recipe);
		}

		public override bool OnPickup(Item item, Player player)
		{
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnPickup,
				Item = item,
				Player = player
			};

			ModifierPool pool = GetItemInfo(item).ModifierPool;
			if (!HasRolled && pool == null)
			{
				pool = RollNewPool(ctx);
				pool?.ApplyModifiers(item);
			}

			return base.OnPickup(item, player);
		}

		public override void PostReforge(Item item)
		{
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnReforge,
				Item = item,
				Player = Main.LocalPlayer
			};

			ModifierPool pool = RollNewPool(ctx);
			pool?.ApplyModifiers(item);
		}

		/// <summary>
		/// Will modify vanilla tooltips to add additional information for the affected item's modifiers
		/// </summary>
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			var pool = GetItemInfo(item).ModifierPool;
			if (pool != null && pool.ActiveModifiers.Length > 0)
			{
				int i = tooltips.FindIndex(x => x.mod == "Terraria" && x.Name == "ItemName");
				if (i != -1)
				{
					var namelayer = tooltips[i];
					if (pool.Rarity.ItemPrefix != null)
						namelayer.text = $"{pool.Rarity.ItemPrefix} {namelayer.text}";
					if (pool.Rarity.ItemSuffix != null)
						namelayer.text += $" {pool.Rarity.ItemSuffix}";
					if (pool.Rarity.OverrideNameColor != null)
						namelayer.overrideColor = pool.Rarity.OverrideNameColor;
					tooltips[i] = namelayer;
				}

				i = tooltips.Count;
				tooltips.Insert(i, new TooltipLine(mod, "Modifier:Name", $"[{pool.Rarity.Name}]") { overrideColor = pool.Rarity.Color * Main.inventoryScale });

				foreach (var ttcol in pool.Description)
					foreach (var tt in ttcol)
						tooltips.Insert(++i, new TooltipLine(mod, $"Modifier:Description:{i}", tt.Text) { overrideColor = (tt.Color ?? Color.White) * Main.inventoryScale });

				foreach (var e in pool.ActiveModifiers)
					e.ModifyTooltips(item, tooltips);
			}
		}
	}

}
