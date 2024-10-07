using Loot.Api.Core;
using Loot.Hacks;
using Terraria;

namespace Loot.Caching
{
	public sealed partial class ModifierCachePlayer
	{
		private void UpdateVanityCache()
		{
			// vanity
			for (int i = 13; i < 18 + Player.extraAccessorySlots; i++)
			{
				var oldEquip = _oldVanityEquips[i - 13];
				var newEquip = Player.armor[i];

				// If equip slot needs an update
				if (!_forceEquipUpdate && oldEquip != null && newEquip == oldEquip)
					continue;

				Ready = false;

				// detach old first
				if (oldEquip != null && !oldEquip.IsAir && CheatedItemHackGlobalItem.GetInfo(oldEquip).IsCheated)
				{
					foreach (Modifier m in LootModItem.GetActivePool(oldEquip))
					{
						AddDetachItem(oldEquip, m);
					}
				}

				// attach new
				if (newEquip != null && !newEquip.IsAir && CheatedItemHackGlobalItem.GetInfo(newEquip).IsCheated)
				{
					foreach (Modifier m in LootModItem.GetActivePool(newEquip))
					{
						AddAttachItem(newEquip, m);
					}
				}

				_oldVanityEquips[i - 13] = newEquip;
			}
		}

		private void UpdateHeldItemCache()
		{
			// If held item needs an update
			if (_oldSelectedItem == Player.selectedItem)
				return;

			Ready = false;

			// detach old held item
			Item oldSelectedItem = Player.inventory[_oldSelectedItem];
			if (oldSelectedItem != null && !oldSelectedItem.IsAir && IsMouseUsable(oldSelectedItem))
			{
				foreach (Modifier m in LootModItem.GetActivePool(oldSelectedItem))
				{
					AddDetachItem(oldSelectedItem, m);
				}
			}

			// attach new held item
			if (Player.HeldItem != null && !Player.HeldItem.IsAir && IsMouseUsable(Player.HeldItem))
			{
				foreach (Modifier m in LootModItem.GetActivePool(Player.HeldItem))
				{
					AddAttachItem(Player.HeldItem, m);
				}
			}

			_oldSelectedItem = Player.selectedItem;
		}

		private bool UpdateMouseItemCache()
		{
			// If held item needs an update
			if (_oldMouseItem != null && _oldMouseItem == Main.mouseItem)
				return false;

			Ready = false;

			// detach old mouse item
			if (_oldMouseItem != null && !_oldMouseItem.IsAir && IsMouseUsable(_oldMouseItem))
			{
				foreach (Modifier m in LootModItem.GetActivePool(_oldMouseItem))
				{
					AddDetachItem(_oldMouseItem, m);
				}
			}

			// attach new held item
			if (Main.mouseItem != null && !Main.mouseItem.IsAir && IsMouseUsable(Main.mouseItem))
			{
				foreach (Modifier m in LootModItem.GetActivePool(Player.HeldItem))
				{
					AddAttachItem(Main.mouseItem, m);
				}
			}

			_oldMouseItem = Main.mouseItem;
			return Main.mouseItem != null && !Main.mouseItem.IsAir;
		}

		private void UpdateEquipsCache()
		{
			for (int i = 0; i < 8 + Player.extraAccessorySlots; i++)
			{
				var oldEquip = _oldEquips[i];
				var newEquip = Player.armor[i];

				// If equip slot needs an update
				if (!_forceEquipUpdate && oldEquip != null && newEquip == oldEquip)
					continue;

				Ready = false;

				// detach old first
				if (oldEquip != null && !oldEquip.IsAir)
				{
					foreach (Modifier m in LootModItem.GetActivePool(oldEquip))
					{
						AddDetachItem(oldEquip, m);
					}
				}

				// attach new
				if (newEquip != null && !newEquip.IsAir)
				{
					foreach (Modifier m in LootModItem.GetActivePool(newEquip))
					{
						AddAttachItem(newEquip, m);
					}
				}

				_oldEquips[i] = newEquip;
			}
		}
	}
}
