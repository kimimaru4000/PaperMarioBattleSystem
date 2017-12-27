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
        /// <summary>
        /// Types of animation settings for after-images.
        /// </summary>
        public enum AfterImageAnimSetting
        {
            /// <summary>
            /// After-images render the current animation state of the BattleEntity.
            /// </summary>
            Current,
            /// <summary>
            /// After-images render the previous animation state of the BattleEntity recorded at the time.
            /// </summary>
            Previous
        }

        /// <summary>
        /// Types of alpha settings for after-images.
        /// </summary>
        public enum AfterImageAlphaSetting
        {
            /// <summary>
            /// After-images are more transparent the further they are from the current position.
            /// </summary>
            FadeOff,
            /// <summary>
            /// All after-images have the same alpha value.
            /// </summary>
            Constant
        }

        /// <summary>
        /// The BattleEntity.
        /// </summary>
        public BattleEntity Entity = null;

        /// <summary>
        /// The max number of after-images to have.
        /// </summary>
        public int MaxAfterImages = 3;

        /// <summary>
        /// How many frames behind the BattleEntity's position each successive after-image is.
        /// Lower values result in after-images closer to the BattleEntity.
        /// <para>For example, if 2, the closest after-image would be 2 frames behind the BattleEntity's current position.
        /// The next closest after-image would be 4 frames behind, and so on.</para>
        /// </summary>
        public int FramesBehind = 1;

        /// <summary>
        /// The amount to modify the alpha of each after-image by.
        /// <para>If <see cref="AfterImageAlphaSetting.FadeOff"/>, closer after-images are rendered more opaque than further ones.
        /// If <see cref="AfterImageAlphaSetting.Constant"/>, all after-images have the same alpha value.</para>
        /// </summary>
        public float AlphaValue = .3f;

        /// <summary>
        /// The alpha setting of the after-images.
        /// </summary>
        public AfterImageAlphaSetting AlphaSetting = AfterImageAlphaSetting.FadeOff;

        /// <summary>
        /// The animation setting of the after-images.
        /// </summary>
        public AfterImageAnimSetting AnimSetting = AfterImageAnimSetting.Current;

        /// <summary>
        /// The total duration to display after-images.
        /// If less than 0, it will display after-images until stopped manually.
        /// </summary>
        public double TotalDuration = -1;

        /// <summary>
        /// The previous positions and animation frames of the BattleEntity. After-images are rendered at certain points of these states.
        /// </summary>
        private readonly List<AfterImageState> PrevEntityStates = null;

        /// <summary>
        /// The amount of time elapsed to track when the after-images should end if TotalDuration is 0 or greater.
        /// </summary>
        private double ElapsedTime = 0d;

        public AfterImageVFX(BattleEntity entity, int maxAfterImages, int framesBehind, float alphaValue, AfterImageAlphaSetting alphaSetting,
            AfterImageAnimSetting animSetting)
        {
            Entity = entity;
            MaxAfterImages = maxAfterImages;
            FramesBehind = framesBehind;
            AlphaValue = alphaValue;
            AlphaSetting = alphaSetting;
            AnimSetting = animSetting;

            //Set the capacity of the list
            PrevEntityStates = new List<AfterImageState>(MaxAfterImages * FramesBehind);
        }

        public AfterImageVFX(BattleEntity entity, int maxAfterImages, int framesBehind, float alphaValue, AfterImageAlphaSetting alphaSetting,
            AfterImageAnimSetting animSetting, double totalDuration)
            : this(entity, maxAfterImages, framesBehind, alphaValue, alphaSetting, animSetting)
        {
            TotalDuration = totalDuration;
        }

        public override void Update()
        {
            if (Entity == null)
                return;

            //If we're past the position capacity, remove the last one
            if (PrevEntityStates.Count >= PrevEntityStates.Capacity)
            {
                PrevEntityStates.RemoveAt(PrevEntityStates.Count - 1);
            }

            //Add the most recent position and animation frame at the front of the list
            PrevEntityStates.Insert(0, new AfterImageState(Entity.Position, Entity.AnimManager.CurrentAnim.CurFrame));

            if (TotalDuration >= 0)
            {
                //If after-images last a certain amount of time, increment the elapsed time and check if they should end
                ElapsedTime += Time.ElapsedMilliseconds;

                if (ElapsedTime >= TotalDuration)
                {
                    ReadyForRemoval = true;
                }
            }
        }

        public override void Draw()
        {
            if (Entity == null)
                return;

            //Go through all after-images
            for (int i = 0; i < MaxAfterImages; i++)
            {
                //Find the index of the state list to render this after-image
                int posIndex = ((i + 1) * FramesBehind) - 1;

                //If we don't have the state available yet, don't render this one or the rest
                if (posIndex >= PrevEntityStates.Count)
                {
                    break;
                }

                //If the after-image's position is the same as the entity's position, don't render it
                //If the anim setting is Previous, don't render if the draw region also is the same (indicating it's the same animation frame)
                if (PrevEntityStates[posIndex].Position == Entity.Position &&
                    (AnimSetting == AfterImageAnimSetting.Current || (AnimSetting == AfterImageAnimSetting.Previous
                    && PrevEntityStates[posIndex].AnimFrame.DrawRegion == Entity.AnimManager.CurrentAnim.CurFrame.DrawRegion)))
                {
                     continue;
                }

                //Get the color
                Color color = GetAfterImageColor(i);

                //Render based on the animation setting
                if (AnimSetting == AfterImageAnimSetting.Current)
                {
                    Entity.AnimManager.CurrentAnim.Draw(PrevEntityStates[posIndex].Position, color, Vector2.Zero, Entity.Scale,
                        Entity.EntityType == Enumerations.EntityTypes.Player, .09f);
                }
                else if (AnimSetting == AfterImageAnimSetting.Previous)
                {
                    PrevEntityStates[posIndex].AnimFrame.Draw(Entity.AnimManager.SpriteSheet, PrevEntityStates[posIndex].Position,
                        color, Vector2.Zero, Entity.Scale, Entity.EntityType == Enumerations.EntityTypes.Player, .09f, false);
                }
            }
        }

        private Color GetAfterImageColor(int index)
        {
            //Modify the alpha of the Entity's TintColor by the AlphaValue based on the alpha setting
            Color color = Entity.TintColor;// * (1 - ((i + 1) * AlphaValue));
            if (AlphaSetting == AfterImageAlphaSetting.FadeOff)
            {
                color *= (1 - ((index + 1) * AlphaValue));
            }
            else if (AlphaSetting == AfterImageAlphaSetting.Constant)
            {
                color *= AlphaValue;
            }

            return color;
        }

        private struct AfterImageState
        {
            public Vector2 Position;
            public Animation.Frame AnimFrame;

            public AfterImageState(Vector2 position, Animation.Frame animFrame)
            {
                Position = position;
                AnimFrame = animFrame;
            }
        }
    }
}
