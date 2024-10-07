using Loot.Api.Core;
using Loot.Api.Ext;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers.Base
{
	/// <summary>
	/// Defines a modifier that can roll on a weapon item
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class WeaponModifier : Modifier
	{
		public static bool HasVanillaDamage(Item item)
			=> item.CountsAsClass(DamageClass.Magic) || item.CountsAsClass(DamageClass.Melee) || item.CountsAsClass(DamageClass.Ranged) || item.CountsAsClass(DamageClass.Summon) || item.CountsAsClass(DamageClass.Throwing);

		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsWeapon();
	}
}
