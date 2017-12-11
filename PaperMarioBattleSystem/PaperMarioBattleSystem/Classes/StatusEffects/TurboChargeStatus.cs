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
    /// The Turbo Charge Status Effect.
    /// The entity's Attack is raised by 1.
    /// <para>This Status Effect is inflicted with Watt's Turbo Charge move.</para>
    /// </summary>
    public sealed class TurboChargeStatus : POWUpStatus
    {
        /// <summary>
        /// The amount Turbo Charge increases the entity's Attack by.
        /// </summary>
        private const int DamageBoost = 1;

        public TurboChargeStatus(int duration) : base(DamageBoost, duration)
        {
            StatusType = Enumerations.StatusTypes.TurboCharge;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(461, 423, 29, 29));

            //ArrowRect = new Rectangle(506, 422, 30, 30);

            AfflictedMessage = "Your attack power will go up for a short time!";
            RemovedMessage = "Your attack power has returned to normal!";
        }

        public override StatusEffect Copy()
        {
            return new TurboChargeStatus(Duration);
        }
    }
}
