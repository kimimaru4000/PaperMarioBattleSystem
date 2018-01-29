using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class ParatroopaWingedBehavior : ParagoombaWingedBehavior
    {
        public ParatroopaWingedBehavior(BattleEntity entity, int groundedTurns, Enumerations.DamageEffects groundedOnEffects, BattleEntity groundedEntity)
            : base(entity, groundedTurns, groundedOnEffects, groundedEntity)
        {
            WingOffset = new Vector2(-1, 2);
            WingRectRegion = new Rectangle(66, 190, 45, 26);
        }
    }
}
