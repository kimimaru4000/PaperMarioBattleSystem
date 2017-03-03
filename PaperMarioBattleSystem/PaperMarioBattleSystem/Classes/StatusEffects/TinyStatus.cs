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
    /// The Tiny Status Effect.
    /// The entity shrinks and has its Attack reduced by 2 until it ends.
    /// </summary>
    public sealed class TinyStatus : POWDownStatus
    {
        private const int AttackReduction = 2;

        public TinyStatus(int duration) : base(AttackReduction, duration)
        {
            StatusType = Enumerations.StatusTypes.Tiny;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(708, 204, 38, 46));

            AfflictedMessage = "Tiny! Attack power has\nnow dropped!";
        }

        public override StatusEffect Copy()
        {
            return new TinyStatus(Duration);
        }
    }
}
