using System;
using System.Collections.Generic;
using Loot.Api.Cubes;
using Loot.UI;
using Loot.UI.Tabs.CraftingTab;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Loot.ModSupport
{
	internal class WingSlotSupport : ModSupport
	{
		public override string ModName => "WingSlot";

		public bool IsInvalid;

		// "Adds" this check for the item to be equipped into the wing slot
		// If it returns true, it stops the item being equipped
		private static bool WingSlotHandler()
			=> Loot.Instance.GuiInterface?.CurrentState is GuiTabWindow tabUi
			   && tabUi.Visible;

		public override bool CheckValidity(Mod mod)
		{
			IsInvalid = mod.Version < new Version(1, 6, 1);
			return !IsInvalid;
		}

		public override void AddClientSupport(Mod mod)
		{
			mod.Call("add", (Func<bool>)WingSlotHandler);
		}

		private static bool RightClickFunctionalityRequirements(Item item)
		{
			if (ModSupportTunneler.GetModSupport<WingSlotSupport>().IsInvalid && item.wingSlot > 0)
				return false;

			return !PlayerInput.WritingText
				   && Main.hasFocus
				   && Main.keyState.IsKeyDown(Keys.LeftControl)
				   && Loot.Instance.GuiInterface.CurrentState != null;
		}

		private static void SwapItems(ICraftingTab craftingTab, Item item)
		{
			craftingTab.GiveBackItems();
			craftingTab.OverrideSlottedItem(item);
		}

		public class WingSlotSupportGlobalItem : GlobalItem
		{
			// Needed to enable right click for possible items
			public override bool CanRightClick(Item item)
			{
				if (!RightClickFunctionalityRequirements(item))
					return false;

				if (!(Loot.Instance.GuiInterface.CurrentState is GuiTabWindow ui)
					|| !(ui.GetCurrentTab() is ICraftingTab tab))
					return false;

				return ui.Visible && tab.AcceptsItem(item);
			}

			// Auto slot item in UI if possible
			public override void RightClick(Item item, Player player)
			{
				// We need to rerun checks here because our forced right click above
				// becomes meaningless to items that can already be right clicked
				// meaning that items such as goodie bags will end up in this hook
				// regardless of our forced right click functionality in CanRightClick
				// by which we need to assume any possible item can be passed into this hook
				if (!(Loot.Instance.GuiInterface.CurrentState is GuiTabWindow ui)
					|| !(ui.GetCurrentTab() is ICraftingTab tab))
					return;

				if (RightClickFunctionalityRequirements(item) && !(item.ModItem is MagicalCube) && ui.Visible && tab.AcceptsItem(item))
				{
					SwapItems(tab, item);
					// else // item has innate right click or mod allows it, do nothing
				}
			}

			// Give notice how to slot
			public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
			{
				if (!(Loot.Instance.GuiInterface.CurrentState is GuiTabWindow ui)
				    || !(ui.GetCurrentTab() is ICraftingTab tab))
					return;

				if ((ModSupportTunneler.GetModSupport<WingSlotSupport>().IsInvalid && item.wingSlot > 0) // block wings if low version if wingslot
				    || !tab.AcceptsItem(item)
				    || LootModItem.GetInfo(item).SlottedInUI)
					return;

				var i = tooltips.FindIndex(x => x.Mod.Equals("Terraria") && x.Name.Equals("ItemName"));
				if (i != -1) tooltips[i].Text += " (control right click to slot into UI)";
			}
		}
	}
}
