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
    /// The Sleep Status Effect.
    /// Entities afflicted with this cannot move until it ends.
    /// There is a chance that the entity will wake up and end this status when it is attacked
    /// </summary>
    public sealed class SleepStatus : ImmobilizedStatus
    {
        public SleepStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Sleep;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(555, 9, 38, 46));

            AfflictedMessage = "Sleepy! It'll take time for\nthe sleepiness to wear off!";
        }
        
        public sealed override StatusEffect Copy()
        {
            return new SleepStatus(Duration);
        }
    }
}
