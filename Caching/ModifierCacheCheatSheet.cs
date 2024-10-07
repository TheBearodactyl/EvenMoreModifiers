using System.Linq;
using CheatSheet;
using Loot.Api.Core;

namespace Loot.Caching
{
	public sealed partial class ModifierCachePlayer
	{
		// This needs to be separate because of CheatSheetInterface static reference
		// to not freak out JIT
		private void UpdateCheatSheetCache()
		{
			// get cheat sheet slots
			var curEquips = CheatSheetInterface.GetEnabledExtraAccessories(Player).Take(_oldCheatSheetEquips.Length).ToArray();

			// go over enabled slots
			for (int i = 0; i < curEquips.Length; i++)
			{
				var oldEquip = _oldCheatSheetEquips[i];
				var newEquip = curEquips[i];

				// update delegations
				if (oldEquip != null && newEquip == oldEquip)
				{
					continue;
				}

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

				_oldCheatSheetEquips[i] = newEquip;
			}

			// current enabled is smaller than total
			if (curEquips.Length >= _oldCheatSheetEquips.Count(x => x != null))
			{
				return;
			}

			var outOfDateEquips = _oldCheatSheetEquips.Skip(curEquips.Length);
			// for all disabled slots but still had a registered item, detach it
			foreach (var item in outOfDateEquips.Where(x => x != null && !x.IsAir))
			{
				Ready = false;
				foreach (Modifier m in LootModItem.GetActivePool(item))
				{
					AddDetachItem(item, m);
				}
			}
		}
	}
}
