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

        }

        public override void RemoveWings()
        {
            Animation[] animations = Entity.AnimManager.GetAnimations(AnimationGlobals.HurtName);

            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetChildFrames(null);
            }

            //Add VFX for the wings disappearing
            Texture2D spriteSheet = Entity.AnimManager.SpriteSheet;
            CroppedTexture2D wingSprite = new CroppedTexture2D(spriteSheet, new Rectangle(66, 190, 45, 26));

            //Put the wings in the same spot as they were in the Paratroopa's last animation
            WingsDisappearVFX wingsDisappear = new WingsDisappearVFX(wingSprite, Entity.BattlePosition + new Vector2(-1, 2),
                Entity.EntityType != Enumerations.EntityTypes.Enemy, .1f - .01f, 500d, 500d, (1d / 30d) * Time.MsPerS);

            BattleObjManager.Instance.AddBattleObject(wingsDisappear);
        }
    }
}
