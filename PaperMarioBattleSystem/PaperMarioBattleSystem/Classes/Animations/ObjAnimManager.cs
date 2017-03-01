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
    /// An animation manager for objects.
    /// </summary>
    public class ObjAnimManager
    {
        /// <summary>
        /// The SpriteSheet to use for the animations.
        /// </summary>
        public Texture2D SpriteSheet { get; private set; } = null;

        /// <summary>
        /// The named object the animations are for.
        /// </summary>
        public INameable NamedObj = null;

        /// <summary>
        /// The animations, referred to by string.
        /// </summary>
        protected readonly Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        /// <summary>
        /// The previous animation that has been played.
        /// </summary>
        public Animation PreviousAnim { get; private set; } = null;

        /// <summary>
        /// The current animation being played.
        /// </summary>
        public Animation CurrentAnim { get; private set; } = null;

        public ObjAnimManager(INameable nameableObj)
        {
            NamedObj = nameableObj;
        }

        public ObjAnimManager(Texture2D spriteSheet, INameable nameableObj) : this(nameableObj)
        {
            SetSpriteSheet(spriteSheet);
        }

        public void SetSpriteSheet(Texture2D spriteSheet)
        {
            SpriteSheet = spriteSheet;
        }

        /// <summary>
        /// Adds an animation.
        /// If an animation already exists, it will be replaced.
        /// </summary>
        /// <param name="animName">The name of the animation.</param>
        /// <param name="anim">The animation reference.</param>
        public void AddAnimation(string animName, Animation anim)
        {
            //Return if trying to add null animation
            if (anim == null)
            {
                Debug.LogError($"Trying to add null animation called \"{animName}\" to {NamedObj.Name}, so it won't be added");
                return;
            }

            if (Animations.ContainsKey(animName) == true)
            {
                Debug.LogWarning($"{NamedObj.Name} already has an animation called \"{animName}\" and will be replaced");

                //Clear the current animation reference if it is the animation being removed
                Animation prevAnim = Animations[animName];
                if (CurrentAnim == prevAnim)
                {
                    CurrentAnim = null;
                }

                Animations.Remove(animName);
            }

            anim.SetKey(animName);
            Animations.Add(animName, anim);

            //Play the first animation that gets added by default
            //This allows us to safely have a valid animation reference at all times, provided at least one is added
            if (CurrentAnim == null)
            {
                PlayAnimation(animName);
            }
        }

        public Animation GetAnimation(string animName)
        {
            //If animation cannot be found
            if (Animations.ContainsKey(animName) == false)
            {
                Debug.LogError($"Cannot find animation called \"{animName}\" for {NamedObj.Name} to play");
                return null;
            }

            return Animations[animName];
        }

        /// <summary>
        /// Plays an animation, specified by name. If the animation does not have the specified animation, nothing happens.
        /// </summary>
        /// <param name="animName">The name of the animation to play</param>
        /// <param name="resetPrevious">If true, resets the previous animation that was playing, if any.
        /// This will also reset its speed</param>
        public void PlayAnimation(string animName, bool resetPrevious = false, Animation.AnimFinish onFinish = null)
        {
            Animation animToPlay = GetAnimation(animName);

            //If animation cannot be found, return
            if (animToPlay == null)
            {
                //Call the delegate
                onFinish?.Invoke();
                return;
            }

            //Reset the previous animation if specified
            if (resetPrevious == true)
            {
                CurrentAnim?.Reset(true);
            }

            //Set previous animation
            PreviousAnim = CurrentAnim;

            //Play animation
            CurrentAnim = animToPlay;
            CurrentAnim.Play(onFinish);
        }
    }
}
