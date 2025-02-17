using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Api.Graphics;
using Loot.Modifiers.EquipModifiers.Utility;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Utilities;

namespace Loot.Api.Strategy
{
	public abstract class RollingStrategy
	{
		// Forces the next roll to succeed
		protected bool _forceNextRoll;

		protected RollingStrategyProperties properties;
		protected LootModItem modifierItem;

		private void InitFields(Item item, RollingStrategyProperties properties)
		{
			this.properties = properties;
			modifierItem = LootModItem.GetInfo(item);
		}

		private WeightedRandom<Modifier> GetRandomList(ModifierContext context, ModifierPool pool)
		{
			WeightedRandom<Modifier> wr = new WeightedRandom<Modifier>();
			foreach (var e in pool.GetRollableModifiers(context))
			{
				wr.Add(e, e.Properties.RollChance);
			}
			return wr;
		}

		internal List<Modifier> _Roll(ModifierPool pool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			InitFields(modifierContext.Item, properties);
			var list = PreRoll(pool, modifierContext, properties);
			list = Roll(list, pool, modifierContext, properties);
			PostRoll(ref list, pool, modifierContext, properties);
			GraphicsGlobalItem.UpdateGraphicsEntities(list, modifierContext.Item);
			return list;
		}

		public virtual void PlaySoundEffect(Item item)
		{
			SoundEngine.PlaySound(SoundID.Item37);
		}

		public virtual List<Modifier> Roll(List<Modifier> currentModifiers, ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			if (currentModifiers.Count >= properties.MaxRollableLines)
			{
				return currentModifiers;
			}

			WeightedRandom<Modifier> wr = new WeightedRandom<Modifier>();
			if (modifierContext.Pool != null)
			{
				foreach (var modifier in modifierContext.Pool.GetRollableModifiers(modifierContext))
				{
					wr.Add(modifier, modifier.Properties.RollChance);
				}
			}
			else if (modifierContext.PoolSet != null && modifierContext.PoolSet.Any())
			{
				var randomPool = modifierContext.PoolSet[Main.rand.Next(modifierContext.PoolSet.Count - 1)];
				foreach (var modifier in randomPool.GetRollableModifiers(modifierContext))
				{
					wr.Add(modifier, modifier.Properties.RollChance);
				}
			}
			else
			{
				wr = GetRandomList(modifierContext, drawPool);
			}

			var list = currentModifiers;

			if (properties.MinRollableLines > properties.MaxRollableLines)
				properties.MaxRollableLines = properties.MinRollableLines;

			// Up to n times, try rolling a mod
			for (int i = 0; i < properties.MaxRollableLines; ++i)
			{
				int z = currentModifiers.Count;
				Modifier line = null;
				int tries = 0;

				while (tries < 4 && currentModifiers.Count < properties.MaxRollableLines)
				{
					line = PreRollLine(ref currentModifiers, drawPool, modifierContext, properties) ?? RollLine(currentModifiers, drawPool, modifierContext, properties, wr);
					if (line == null) continue;
					currentModifiers.Add(line);
					PostRollLine(ref currentModifiers, line, drawPool, modifierContext, properties);
					if (currentModifiers.Count > z) break;
				}

				// If it is a unique modifier, remove it from the list to be rolled
				if (line != null && line.Properties.IsUnique)
				{
					wr.elements.RemoveAll(x => x.Item1.Type == line.Type);
					wr.needsRefresh = true;
				}
			}

			return list;
		}

		public virtual List<Modifier> PreRoll(ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			var preset = properties?.PresetLines?.Invoke() ?? new List<Modifier>();
			preset.ForEach(x => RollProperties(x, preset, modifierContext, properties));
			preset.ForEach(x => x.Roll(modifierContext, preset));
			return preset;
		}

		public virtual void PostRoll(ref List<Modifier> modifiers, ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{

		}

		public virtual Modifier PreRollLine(ref List<Modifier> currentModifiers, ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			return null;
		}

		public virtual Modifier RollLine(List<Modifier> currentModifiers, ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties, WeightedRandom<Modifier> weightedRandom)
		{
			// If there are no mods left, or we fail the roll, break.
			if (weightedRandom.elements.Count <= 0
				|| !_forceNextRoll
				&& currentModifiers.Count >= properties.MinRollableLines
				&& Main.rand.NextFloat() > properties.RollNextChance)
			{
				return null;
			}

			_forceNextRoll = false;

			// Get a next weighted random mod
			// Clone the mod (new instance) and roll its properties, then roll it
			Modifier rolledModifier = (Modifier)weightedRandom.Get().Clone();
			RollProperties(rolledModifier, currentModifiers, modifierContext, properties);

			rolledModifier.Roll(modifierContext, currentModifiers);

			// If the mod deemed to be unable to be added,
			// Force that the next roll is successful
			// (no RNG on top of RNG)
			if (!rolledModifier.PostRoll(modifierContext, currentModifiers))
			{
				_forceNextRoll = true;
				return null;
			}

			return rolledModifier;
		}

		public virtual void PostRollLine(ref List<Modifier> modifiers, Modifier line, ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
		}

		private void RollProperties(Modifier modifier, List<Modifier> modifiers, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			float luck = properties.ExtraLuck;
			float magnitudePower = properties.MagnitudePower;

			if (modifierContext.Player != null)
			{
				luck += ModifierDelegatorPlayer.GetPlayer(modifierContext.Player).GetEffect<LuckEffect>().Luck;
			}
			if (modifierContext.Rarity != null)
			{
				luck += modifierContext.Rarity.ExtraLuck;
				magnitudePower += modifierContext.Rarity.ExtraMagnitudePower;
			}

			modifier.Properties =
				modifier.GetModifierProperties(modifierContext.Item)
					.Build()
					.RollMagnitudeAndPower(magnitudePower, luck);
		}
	}

}
