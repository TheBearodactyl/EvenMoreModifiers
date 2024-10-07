using Loot.Api.Core;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class MeleeCritPlus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% melee crit chance");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.GetCritChance(DamageClass.Melee) += (int) Properties.RoundedPower;
		}
	}
}
