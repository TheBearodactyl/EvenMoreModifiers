using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Loot.Api.Attributes;
using Loot.Api.Content;
using Loot.Api.Delegators;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Api.Core
{
	/// <summary>
	/// A ModifierEffect signifies the effect of a modifier on a player
	/// It should house the implementation, and delegation of such an effect
	/// Methods on effects can be delegated from ModPlayer
	/// </summary>
	public abstract class ModifierEffect : ModPlayer, ILoadableContent, ILoadableContentSetter, ICloneable
	{
		private new Mod mod => Mod;
		public Mod Mod { get; internal set; }

		Mod ILoadableContentSetter.Mod
		{
			set => Mod = value;
		}

		public uint Type { get; internal set; }

		uint ILoadableContentSetter.Type
		{
			set => Type = value;
		}

		public new string Name => GetType().Name;

		public ModifierDelegatorPlayer DelegatorPlayer { get; internal set; }

		public new Player player => DelegatorPlayer.Player;

		/// <summary>
		/// Keeps track of if this particular modifier is being delegated or not
		/// This is used to check if this effect's automatic delegation needs to be performed
		/// </summary>
		public bool IsBeingDelegated { get; internal set; }

		/// <summary>
		/// Called when the ModPlayer initializes the effect
		/// </summary>
		public virtual void OnInitialize()
		{
		}

		/// <summary>
		/// Automatically called when the ModPlayer does its ResetEffects
		/// Also automatically called when the delegations of the effect detach
		/// </summary>
		public virtual void ResetEffects()
		{
		}

		/// <summary>
		/// Allows modders to perform various delegations when this modifier is detected to become active
		/// This method will be invoked one time when the modifier becomes 'active'
		/// Alternatively automatic delegation can be used, using the AutoDelegation attribute
		/// </summary>
		public virtual void AttachDelegations(ModifierDelegatorPlayer delegatorPlayer)
		{
		}

		internal void _DetachDelegations(ModifierDelegatorPlayer delegatorPlayer)
		{
			// We need to reset effects first, as they will no longer run
			// after the effect is detached

			// Regular reset
			ResetEffects();

			// This part is actually kind of unneeded anymore
			// But im keeping it in in case someone, for some reason,
			// wants to split their ResetEffects in multiple methods
			// without each other calling them.
			// Performance is no issue since automatic attach and detach
			// is handled by ModifierCachePlayer

			// Look for ResetEffects and manually invoke it
			var resetEffects = GetType()
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(x => x.GetCustomAttributes().OfType<AutoDelegation>().Any())
				.ToDictionary(x => x, y => y.GetCustomAttribute<AutoDelegation>());

			foreach (var kvp in resetEffects.Where(x => x.Value.DelegationTypes.Contains("OnResetEffects")))
			{
				try
				{
					kvp.Key.Invoke(this, new object[] { delegatorPlayer.Player });
				}
				catch (Exception e)
				{
					Loot.Logger.Error("Error in ModifierEffect._DetachDelegations", e);
				}
			}

			// Now detach
			DetachDelegations(delegatorPlayer);
		}

		/// <summary>
		/// Allows modders to undo their performed delegations in <see cref="AttachDelegations"/>
		/// </summary>
		public virtual void DetachDelegations(ModifierDelegatorPlayer delegatorPlayer)
		{
		}

		/// <summary>
		/// A modder can provide custom cloning of effects in this hook
		/// </summary>
		public virtual void Clone(ref ModifierEffect clone)
		{
		}

		public object Clone()
		{
			ModifierEffect clone = (ModifierEffect)MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			Clone(ref clone);
			return clone;
		}

		public sealed override bool IsLoadingEnabled(Mod mod)/* tModPorter Suggestion: If you return false for the purposes of manual loading, use the [Autoload(false)] attribute on your class instead */
			=> false;
		protected override bool CloneNewInstances
			=> false;
		public sealed override void LoadData(TagCompound tag)
		{
		}
		public sealed override void LoadLegacy(BinaryReader reader)
		{
		}
		public sealed override void PreSaveCustomData()
		{
		}
		public sealed override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)/* tModPorter Suggestion: Return an Item array to add to the players starting items. Use ModifyStartingInventory for modifying them if needed */
		{
		}
		[Obsolete("SetupStartInventory now has an overload with a mediumcoreDeath bool argument, please use that.")]
		public sealed override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)/* tModPorter Suggestion: Return an Item array to add to the players starting items. Use ModifyStartingInventory for modifying them if needed */
		{
		}
		public override void PreSavePlayer()
		{
		}
		public override void PostSavePlayer()
		{
		}
		public override void UpdateBiomes()
		{
		}
		public override void UpdateBiomeVisuals()
		{
		}
		public sealed override bool CustomBiomesMatch(Player other)
		{
			return base.CustomBiomesMatch(other);
		}
		public sealed override void CopyCustomBiomesTo(Player other)
		{
		}
		public sealed override void SendCustomBiomes(BinaryWriter writer)
		{
		}
		public sealed override void ReceiveCustomBiomes(BinaryReader reader)
		{
		}
		public sealed override void CopyClientState(ModPlayer clientClone)/* tModPorter Suggestion: Replace Item.Clone usages with Item.CopyNetStateTo */
		{
		}
		public sealed override Texture2D GetMapBackgroundImage()
		{
			return base.GetMapBackgroundImage();
		}
		public sealed override void PlayerConnect()
		{
		}
		public sealed override void PlayerDisconnect()
		{
		}
		public sealed override void OnEnterWorld()
		{
		}
		public sealed override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			return base.ShiftClickSlot(inventory, context, slot);
		}
		public sealed override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
		{
		}
		public sealed override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
		{
			return base.CanSellItem(vendor, shopInventory, item);
		}
		public sealed override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
		{
		}
		public sealed override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item)
		{
			return base.CanBuyItem(vendor, shopInventory, item);
		}
	}
}
