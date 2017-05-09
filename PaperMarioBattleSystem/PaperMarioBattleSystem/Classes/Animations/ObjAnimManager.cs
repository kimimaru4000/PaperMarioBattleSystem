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
        /// Options when setting the <see cref="ObjAnimManager"/>'s SpriteSheet.
        /// <para>
        /// None does not replace any Animation SpriteSheets.
        /// ReplaceSame replaces all Animation SpriteSheets with the current SpriteSheet reference to the new one.
        /// ReplaceAll replaces all Animation SpriteSheets with the new one.
        /// </para>
        /// </summary>
        public enum SetSpriteSheetOptions
        {
            None, ReplaceSame, ReplaceAll
        }

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

        protected virtual string GetName => (NamedObj == null) ? "N/A" : NamedObj.Name;

        protected ObjAnimManager()
        {

        }

        public ObjAnimManager(INameable nameableObj)
        {
            NamedObj = nameableObj;
        }

        public ObjAnimManager(Texture2D spriteSheet, INameable nameableObj) : this(nameableObj)
        {
            SetSpriteSheet(spriteSheet);
        }

        /// <summary>
        /// Set the SpriteSheet of the animation manager.
        /// </summary>
        /// <param name="spriteSheet">The new SpriteSheet.</param>
        public void SetSpriteSheet(Texture2D spriteSheet)
        {
            SetSpriteSheet(spriteSheet, SetSpriteSheetOptions.None);
        }

        /// <summary>
        /// Set the SpriteSheet and optionally change the SpriteSheet the animations reference.
        /// </summary>
        /// <param name="newSheet">The new SpriteSheet.</param>
        /// <param name="setSpriteSheetOptions">The option for replacing the SpriteSheet the animations reference.</param>
        public void SetSpriteSheet(Texture2D newSheet, SetSpriteSheetOptions setSpriteSheetOptions)
        {
            //Store the current SpriteSheet
            Texture2D previousSheet = SpriteSheet;

            if (newSheet == null)
            {
                Debug.LogError($"{nameof(newSheet)} is being set to null for {GetName}'s {nameof(ObjAnimManager)}. Not setting spriteSheet as this will cause errors.");
                return;
            }

            //Set the new SpriteSheet
            SpriteSheet = newSheet;

            //Don't replace any Animation SpriteSheets
            if (setSpriteSheetOptions == SetSpriteSheetOptions.None)
            {
                return;
            }
            //Replace some
            else if (setSpriteSheetOptions == SetSpriteSheetOptions.ReplaceSame)
            {
                //Get all animations
                Animation[] animations = GetAllAnimations();
                for (int i = 0; i < animations.Length; i++)
                {
                    //Replace all Animation SpriteSheets with the new one if they're the same as the previous
                    if (animations[i].SpriteSheet == previousSheet)
                    {
                        animations[i].SpriteSheet = SpriteSheet;
                    }
                }
            }
            //Replace all
            else if (setSpriteSheetOptions == SetSpriteSheetOptions.ReplaceAll)
            {
                //Get all animations
                Animation[] animations = GetAllAnimations();

                //Replace all Animation SpriteSheets with the new one
                for (int i = 0; i < animations.Length; i++)
                {
                    animations[i].SpriteSheet = newSheet;
                }
            }
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
                Debug.LogError($"Trying to add null animation called \"{animName}\" to {GetName}, so it won't be added");
                return;
            }

            if (Animations.ContainsKey(animName) == true)
            {
                Debug.LogWarning($"{GetName} already has an animation called \"{animName}\" and will be replaced");

                //Clear the current animation reference if it is the animation being removed
                Animation prevAnim = Animations[animName];
                if (CurrentAnim == prevAnim)
                {
                    CurrentAnim = null;
                }

                Animations.Remove(animName);
            }

            anim.SetKey(animName);

            //Set the Animation's SpriteSheet to this one if it's not defined
            //If an Animation needs to use a different SpriteSheet for its animations, this won't stop it
            if (anim.SpriteSheet == null)
                anim.SpriteSheet = SpriteSheet;

            Animations.Add(animName, anim);

            //Play the first animation that gets added by default
            //This allows us to safely have a valid animation reference at all times, provided at least one is added
            if (CurrentAnim == null)
            {
                PlayAnimation(animName);
            }
        }

        /// <summary>
        /// Gets an animation by name.
        /// </summary>
        /// <param name="animName">The name of the animation.</param>
        /// <returns>An animation if found, otherwise null.</returns>
        public Animation GetAnimation(string animName)
        {
            //If animation cannot be found
            if (Animations.ContainsKey(animName) == false)
            {
                Debug.LogError($"Cannot find animation called \"{animName}\" for {GetName} to play");
                return null;
            }

            return Animations[animName];
        }

        /// <summary>
        /// Gets a set of animations by their names.
        /// </summary>
        /// <param name="animNames">The names of the animations.</param>
        /// <returns>An array of animations. If none were found, an empty array.</returns>
        public Animation[] GetAnimations(params string[] animNames)
        {
            List<Animation> animations = new List<Animation>();

            for (int i = 0; i < animNames.Length; i++)
            {
                Animation anim = GetAnimation(animNames[i]);
                if (anim != null) animations.Add(anim);
            }

            return animations.ToArray();
        }

        /// <summary>
        /// Gets all animations.
        /// </summary>
        /// <returns>An array of all the animations.</returns>
        public Animation[] GetAllAnimations()
        {
            return GetAnimations(Animations.Keys.ToArray());
        }

        /// <summary>
        /// Sets the child frames for an animation to render.
        /// </summary>
        /// <param name="animName">The name of the animation.</param>
        /// <param name="frames">The Frames to set as the Animation's child frames.</param>
        public void AddAnimationChildFrames(string animName, params Animation.Frame[] frames)
        {
            Animation anim = GetAnimation(animName);

            //The error message is in the previous method so simply return
            if (anim == null) return;

            anim.SetChildFrames(frames);
        }

        /// <summary>
        /// Gets the length of all frames in an animation.
        /// </summary>
        /// <param name="animName">The name of the animation.</param>
        /// <returns>A double with the total length of all frames in the animation. 0 if the animation doesn't exist.</returns>
        public double GetAnimationFrameLength(string animName)
        {
            Animation anim = GetAnimation(animName);
            if (anim == null) return 0d;

            return anim.GetTotalFrameLength();
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
