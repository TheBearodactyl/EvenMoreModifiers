using Loot.Api.Core;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers.WeaponModifiers
{
	public class DamagePlus : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% damage");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
		{
			base.ModifyWeaponDamage(item, player, ref add, ref mult, ref flat);
			add += Properties.RoundedPower / 100f;
		}
	}
}
