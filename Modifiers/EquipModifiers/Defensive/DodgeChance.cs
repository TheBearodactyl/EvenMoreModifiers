using Loot.Api.Attributes;
using Loot.Api.Delegators;
using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class DodgeEffect : ModifierEffect
	{
		public float DodgeChance; // Dodge chance

		public override void ResetEffects(ModifierDelegatorPlayer delegatorPlayer)
		{
			DodgeChance = 0f;
		}

		[AutoDelegation("OnPreHurt")]
		[DelegationPrioritization(DelegationPrioritization.Early, 99)]
		private bool TryDodge(ModifierDelegatorPlayer delegatorPlayer, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Main.rand.NextFloat() < DodgeChance)
			{
				delegatorPlayer.player.NinjaDodge();
				return false;
			}

			return true;
		}
	}

	[UsesEffect(typeof(DodgeEffect))]
	public class DodgeChance : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% dodge chance");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f)
				.WithRollChance(0.333f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.Player(player).GetEffect<DodgeEffect>().DodgeChance += Properties.RoundedPower / 100f;
		}
	}
}
