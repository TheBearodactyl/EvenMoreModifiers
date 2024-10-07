using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Loot.Sounds
{
	/// <summary>
	/// An internal helper class to play custom sounds
	/// </summary>
	internal static class SoundHelper
	{
		internal enum SoundType
		{
			CloseUI,
			Decline,
			Notif,
			OpenUI,
			Receive,
			Redeem,
			GainSeal,
			LoseSeal
		}

		internal static void PlayCustomSound(SoundType type)
		{
			SoundEngine.PlaySound(SoundLoader.customSoundType, -1, -1,
				Loot.Instance.GetSoundSlot(
					Terraria.ModLoader.SoundType.Custom,
					"Sounds/Custom/" + Enum.GetName(typeof(SoundType), type)));
		}
	}
}
