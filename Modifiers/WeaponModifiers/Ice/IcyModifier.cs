using Loot.Api.Core;
using Terraria;
using Terraria.ID;

namespace Loot.Modifiers.WeaponModifiers.Ice
{
	public class IcyModifier : IceModifier
	{
		public override string UniqueName => "Icy";

		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"Inflict frostburn on hit for {Properties.RoundedPower}s");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithRollChance(0.125f)
				.WithMinMagnitude(0.5f)
				.WithMaxMagnitude(3f)
				.IsUniqueModifier(true);
		}

		public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Frostburn, (int) (Properties.Power * 60));
		}
	}
}
