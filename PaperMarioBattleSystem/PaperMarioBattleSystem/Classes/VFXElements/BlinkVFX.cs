using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Makes an <see cref="ITintable"/> object blink at certain speed for a certain amount of time.
    /// </summary>
    public class BlinkVFX : VFXElement
    {
        /// <summary>
        /// The tintable object.
        /// </summary>
        public ITintable TintableObj = null;

        /// <summary>
        /// The rate at which the object blinks.
        /// </summary>
        public double BlinkRate = 100d;

        /// <summary>
        /// How long the blink lasts for, in total.
        /// </summary>
        public double TotalBlinkTime = 0d;

        /// <summary>
        /// The alpha value of the blink.
        /// </summary>
        public float BlinkAlpha = 0f;

        private double ElapsedBlinkTime = 0d;
        private double ElapsedTime = 0d;

        private Color PrevColor = Color.White;

        private bool Blinking = false;

        public BlinkVFX(ITintable tintableObj, float blinkAlpha, double blinkRate, double totalBlinkTime)
        {
            TintableObj = tintableObj;

            BlinkAlpha = blinkAlpha;
            BlinkRate = blinkRate;
            TotalBlinkTime = totalBlinkTime;

            PrevColor = TintableObj.TintColor;
        }

        public override void CleanUp()
        {
            //Set the object's color back
            TintableObj.TintColor = PrevColor;

            TintableObj = null;

            PrevColor = Color.White;

            ElapsedBlinkTime = ElapsedTime = 0d;

            Blinking = false;
        }
    
        public override void Update()
        {
            //Mark as ready for removal
            if (ElapsedTime >= TotalBlinkTime)
            {
                ReadyForRemoval = true;
                return;
            }

            ElapsedTime += Time.ElapsedMilliseconds;
            ElapsedBlinkTime += Time.ElapsedMilliseconds;

            //Check if we should switch the blink state
            if (ElapsedBlinkTime >= BlinkRate)
            {
                if (Blinking == false)
                {
                    //Get the most recent color first
                    PrevColor = TintableObj.TintColor;

                    //Make it blink by setting its alpha
                    TintableObj.TintColor = PrevColor * BlinkAlpha;
                }
                else
                {
                    //Set back its previous color
                    TintableObj.TintColor = PrevColor;
                }

                Blinking = !Blinking;
                ElapsedBlinkTime = 0d;
            }
        }
    }
}
