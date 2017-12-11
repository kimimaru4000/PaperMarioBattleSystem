using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The W Emblem Badge - Changes Mario's clothes to Wario's.
    /// <para>When combined with the <see cref="LEmblemBadge"/>, Mario's clothes turn into Waluigi's.</para>
    /// </summary>
    public sealed class WEmblemBadge : Badge
    {
        public WEmblemBadge()
        {
            Name = "W Emblem";
            Description = "Change Mario's clothes into Wario's clothes.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.WEmblem;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            //NOTE: Think of a way to make this more flexible to easily add more SpriteSheet swaps with different Badges
            //Building a system to allow for more combinations would be great as well and add more possibilities
            //Examples include Partner SpriteSheet swaps (especially Yoshi)

            BattleMario mario = EntityEquipped as BattleMario;
            if (mario == null)
            {
                Debug.LogWarning($"W Emblem will not work on {EntityEquipped.Name}; it currently works only for Mario");
                return;
            }

            string sheetPath = $"{ContentGlobals.SpriteRoot}/Characters/Mario";

            //Check if L Emblem is equipped and change to Waluigi's clothes
            if (EntityEquipped.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.LEmblem) > 0)
            {
                sheetPath += ContentGlobals.WaluigiPaletteExtension;
            }
            //Otherwise change to Wario's clothes
            else
            {
                sheetPath += ContentGlobals.WarioPaletteExtension;
            }

            EntityEquipped.AnimManager.SetSpriteSheet(AssetManager.Instance.LoadRawTexture2D(sheetPath + ".png"), ObjAnimManager.SetSpriteSheetOptions.ReplaceSame);
        }

        protected override void OnUnequip()
        {
            BattleMario mario = EntityEquipped as BattleMario;
            //Don't do anything, as the badge made no changes when equipped
            if (mario == null)
            {
                return;
            }

            string sheetPath = $"{ContentGlobals.SpriteRoot}/Characters/Mario";

            //Check if L Emblem is equipped and change to Luigi's clothes
            if (EntityEquipped.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.LEmblem) > 0)
            {
                sheetPath += ContentGlobals.LuigiPaletteExtension;
            }

            EntityEquipped.AnimManager.SetSpriteSheet(AssetManager.Instance.LoadRawTexture2D(sheetPath + ".png"), ObjAnimManager.SetSpriteSheetOptions.ReplaceSame);
        }
    }
}
