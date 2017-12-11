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
    /// The Huge Status Effect.
    /// The entity grows and has its Attack raised by 2 until it ends.
    /// </summary>
    public sealed class HugeStatus : POWUpStatus
    {
        private const int AttackBoost = 2;

        public HugeStatus(int duration) : base(AttackBoost, duration)
        {
            StatusType = Enumerations.StatusTypes.Huge;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(658, 204, 38, 46));

            AfflictedMessage = "Huge! Attack power is\nnow boosted!";
        }

        public override StatusEffect Copy()
        {
            return new HugeStatus(Duration);
        }
    }
}
