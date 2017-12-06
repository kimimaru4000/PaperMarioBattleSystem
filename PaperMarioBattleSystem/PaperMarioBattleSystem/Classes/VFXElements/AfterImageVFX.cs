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
    /// Leaves after-images behind a BattleEntity.
    /// </summary>
    public class AfterImageVFX : VFXElement
    {
        /* TO FINISH:
         * -Make a struct to hold the time and animation frame.
         *   -This way, we can have after-images disappear after some time and show previous animation frames; right now if the animation changes, so do the after-images
         * -Add duration feature; how long after-image effects should go on for
         */

        /// <summary>
        /// The BattleEntity.
        /// </summary>
        private BattleEntity Entity = null;

        /// <summary>
        /// The max number of after-images to have.
        /// </summary>
        private int MaxAfterImages = 3;

        /// <summary>
        /// How many frames to wait poll the BattleEntity's position. Lower values result in after-images closer to the BattleEntity.
        /// </summary>
        private int FrameSampleRate = 1;

        /// <summary>
        /// The amount to modify the alpha of each after-image by.
        /// Recent after-image positions are rendered more opaque than older ones.
        /// </summary>
        private float AlphaFadeoff = .3f;

        /// <summary>
        /// The positions to render the after-images.
        /// </summary>
        private readonly List<Vector2> AfterImagePositions = new List<Vector2>();

        /// <summary>
        /// The number of frames elapsed since the last after-image position was recorded.
        /// </summary>
        private int CurFrames = 0;

        public AfterImageVFX(BattleEntity entity, int maxAfterImages, int frameSampleRate, float alphaFadeoff)
        {
            Entity = entity;
            MaxAfterImages = maxAfterImages;
            FrameSampleRate = frameSampleRate;
            AlphaFadeoff = alphaFadeoff;
        }

        public override void Update()
        {
            if (Entity == null)
                return;

            //Increment frames
            CurFrames++;

            //If we're at or past the number of frames to get the position, get it
            if (CurFrames >= FrameSampleRate)
            {
                //If we're past the last 
                if (AfterImagePositions.Count >= MaxAfterImages)
                {
                    AfterImagePositions.RemoveAt(AfterImagePositions.Count - 1);
                }
                
                //Add the most recent position to the front of the list
                AfterImagePositions.Insert(0, Entity.Position);

                CurFrames = 0;
            }
        }

        public override void Draw()
        {
            if (Entity == null)
                return;

            for (int i = 0; i < AfterImagePositions.Count; i++)
            {
                //Modify the alpha of the Entity's TintColor by the AlphaFadeoff
                //More recent positions are rendered more opaque than older ones
                Color color = Entity.TintColor * (1 - ((i + 1) * AlphaFadeoff));

                Entity.AnimManager.CurrentAnim.Draw(AfterImagePositions[i], color, Vector2.Zero, Entity.Scale,
                    Entity.EntityType == Enumerations.EntityTypes.Player, .09f);
            }
        }
    }
}
