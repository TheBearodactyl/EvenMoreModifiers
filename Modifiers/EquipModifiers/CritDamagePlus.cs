﻿using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class CritDamagePlusEffect : ModifierEffect
	{
		public float Multiplier;

		public override void OnInitialize(ModifierPlayer player)
		{
			Multiplier = 1f;
		}

		public override void ResetEffects(ModifierPlayer player)
		{
			Multiplier = 1f;
		}

		// @todo must be prioritized after healthy foes

		[AutoDelegation("OnModifyHitNPC")]
		public void ModifyHitNPC(ModifierPlayer player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (crit) CritBonus(ref damage);
		}

		[AutoDelegation("OnModifyHitPvp")]
		private void ModifyHitPvp(ModifierPlayer player, Item item, Player target, ref int damage, ref bool crit)
		{
			if (crit) CritBonus(ref damage);
		}

		private void CritBonus(ref int damage)
		{
			damage = (int) Math.Ceiling(damage * Multiplier);
		}
	}

	[UsesEffect(typeof(CritDamagePlusEffect))]
	public class CritDamagePlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {Text = $"+{Properties.RoundedPower}% crit multiplier", Color = Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 15f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).GetEffect<CritDamagePlusEffect>().Multiplier += Properties.RoundedPower / 100f;
		}
	}
}