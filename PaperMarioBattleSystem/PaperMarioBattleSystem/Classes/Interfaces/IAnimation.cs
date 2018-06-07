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
    /// A general interface for animations.
    /// </summary>
    public interface IAnimation : IUpdateable
    {
        /// <summary>
        /// The key for the animation, acting as an identifier.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// The spritesheet containing the sprites to be used in the animation.
        /// </summary>
        Texture2D SpriteSheet { get; set;  }

        /// <summary>
        /// Tells if the animation is playing.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Tells if the animation is finished.
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Plays the animation from the start.
        /// </summary>
        void Play();

        /// <summary>
        /// Resets the animation to the start.
        /// </summary>
        void Reset();

        /// <summary>
        /// Ends the animation.
        /// </summary>
        void End();
    }
}
