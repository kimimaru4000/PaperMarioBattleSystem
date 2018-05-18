using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that moves a BattleEntity a relative amount over the Duration.
    /// An example would be relative to the entity's current position, such as (50, -12)
    /// </summary>
    public sealed class MoveAmountSeqAction : MoveToSeqAction
    {
        private Vector2 MoveAmount = Vector2.Zero;

        public MoveAmountSeqAction(Vector2 amount, double duration, 
            Interpolation.InterpolationTypes xInterpolation = Interpolation.InterpolationTypes.Linear,
            Interpolation.InterpolationTypes yInterpolation = Interpolation.InterpolationTypes.Linear) : base(duration)
        {
            MoveAmount = amount;

            XInterpolation = xInterpolation;
            YInterpolation = yInterpolation;
        }

        protected override void OnStart()
        {
            base.OnStart();

            MoveEnd = MoveStart + MoveAmount;
        }
    }
}
