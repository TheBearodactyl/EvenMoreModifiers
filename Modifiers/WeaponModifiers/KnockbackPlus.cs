using Loot.Api.Core;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers.WeaponModifiers
{
	public class KnockbackPlus : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% knockback");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override bool CanRoll(ModifierContext ctx)
			=> base.CanRoll(ctx) && ctx.Item.knockBack > 0;

		public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
		{
			base.ModifyWeaponKnockback(item, player, ref knockback);
			knockback *= Properties.RoundedPower / 100f + 1;
		}
	}
}
