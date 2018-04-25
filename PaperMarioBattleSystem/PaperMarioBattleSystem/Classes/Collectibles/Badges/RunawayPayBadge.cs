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
    /// The Ruanway Pay Badge - Mario gains any Star Points he earned in battle when running away.
    /// </summary>
    public sealed class RunawayPayBadge : Badge
    {
        public RunawayPayBadge()
        {
            Name = "Runaway Pay";
            Description = "Lets Mario earn Star Points even if he escapes from battle.";

            BPCost = 2;

            BadgeType = BadgeGlobals.BadgeTypes.RunawayPay;
            AffectedType = BadgeGlobals.AffectedTypes.Self;

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(758, 61, 25, 25));
        }

        protected override void OnEquip()
        {
            
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
