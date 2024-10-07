using System;
using Loot.Api.Core;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CritPlus : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% crit chance");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			return base.CanRoll(ctx) && !ctx.Item.CountsAsClass(DamageClass.Summon);
		}

		public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
		{
			crit = (int) Math.Min(100, crit + Properties.RoundedPower);
		}

		//public override ShaderEntity GetShaderEntity(Item item)
		//{
		//	return new ShaderEntity(item,
		//		GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye),
		//		drawLayer: ShaderDrawLayer.Front,
		//		drawOffsetStyle: ShaderDrawOffsetStyle.Alternate,
		//		shaderDrawColor: Color.IndianRed);
		//}

		//public override GlowmaskEntity GetGlowmaskEntity(Item item)
		//{
		//	return new GlowmaskEntity(item);
		//}
	}
}
