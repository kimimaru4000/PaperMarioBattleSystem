using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Mario's Mega Smash move.
    /// </summary>
    public sealed class MegaSmashSequence : HammerSequence
    {
        private AfterImageVFX AfterImages = null;

        //Mega Smash adds 4 damage
        public MegaSmashSequence(MoveAction moveAction) : base(moveAction, 4)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            //Add after-images to the user
            AfterImages = new AfterImageVFX(User, 4, 4, .2f,
                AfterImageVFX.AfterImageAlphaSetting.FadeOff, AfterImageVFX.AfterImageAnimSetting.Current);
            
            //Update as many times as there are after-images to add several after-images in place
            //This prevents them from suddenly popping in while the BattleEntity is walking towards the target
            for (int i = 0; i < AfterImages.MaxAfterImages; i++)
            {
                AfterImages.Update();
            }

            BattleObjManager.Instance.AddBattleObject(AfterImages);
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //Remove if somehow not removed by now
            RemoveAfterImages();
        }

        protected override void OnInterruption(Enumerations.Elements element)
        {
            //Remove after-images if interrupted
            RemoveAfterImages();

            base.OnInterruption(element);
        }

        protected override void CommandSuccess()
        {
            RemoveAfterImages();

            base.CommandSuccess();
        }

        protected override void CommandFailed()
        {
            RemoveAfterImages();

            base.CommandFailed();
        }

        private void RemoveAfterImages()
        {
            if (AfterImages != null)
            {
                BattleObjManager.Instance.RemoveBattleObject(AfterImages);
                AfterImages = null;
            }
        }
    }
}
