using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base Sequence for Items. The Start for each item sequence is the same (the entity moves up then holds the item over their head).
    /// <para>Items do not have Action Commands in PM or TTYD, so the default behavior is to not consider them.
    /// However, they can be added if desired in derived classes.</para>
    /// </summary>
    public class ItemSequence : Sequence
    {
        protected double WalkDuration = 500d;
        protected double WaitDuration = 1500d;

        protected ItemAction itemAction { get; private set; } = null;
        protected UICroppedTexture2D ItemShown = null;

        public ItemSequence(ItemAction moveAction) : base(moveAction)
        {
            itemAction = moveAction;
        }

        protected override void OnStart()
        {
            base.OnStart();

            //Ensure that we have a usable ItemAction
            //This can be null at this point if the Sequence's MoveAction
            //was passed in as null in the constructor and set later on
            if (itemAction == null)
            {
                itemAction = (ItemAction)Action;
            }

            if (itemAction.ItemUsed.Icon != null || itemAction.ItemUsed.Icon.Tex != null)
            {
                ItemShown = new UICroppedTexture2D(itemAction.ItemUsed.Icon.Copy());
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (ItemShown != null)
            {
                BattleUIManager.Instance.RemoveUIElement(ItemShown);
            }
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName, true);
                    CurSequenceAction = new MoveToSeqAction(BattleManager.Instance.GetPositionInFront(BattleManager.Instance.GetFrontPlayer(), User.EntityType != Enumerations.EntityTypes.Player), WalkDuration);
                    break;
                case 1:
                    if (ItemShown != null)
                    {
                        ItemShown.Position = Camera.Instance.SpriteToUIPos(User.Position + new Vector2(0, -20));
                        BattleUIManager.Instance.AddUIElement(ItemShown);
                    }

                    User.AnimManager.PlayAnimation(AnimationGlobals.GetItemName, false);
                    CurSequenceAction = new WaitSeqAction(WaitDuration);
                    break;
                case 2:
                    if (ItemShown != null)
                    {
                        BattleUIManager.Instance.RemoveUIElement(ItemShown);
                    }

                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());

                    //Remove the item from the Inventory when it's finished
                    //NOTE: I'm not sure if this is the best place to do this yet
                    Inventory.Instance.RemoveItem(itemAction.ItemUsed);

                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            //Base behavior:
            //1. Heal (if the item heals)
            //2. Deal Damage (if the item deals damage)
            //3. End
            switch (SequenceStep)
            {
                case 0:
                    if (Action.Heals == true)
                    {
                        PerformHeal(Action.HealingInfo.Value, EntitiesAffected);
                        CurSequenceAction = new WaitSeqAction(WaitDuration);
                    }
                    break;
                case 1:
                    if (Action.DealsDamage == true)
                    {
                        AttemptDamage(BaseDamage, EntitiesAffected, Action.DamageProperties, true);
                        CurSequenceAction = new WaitSeqAction(WaitDuration);
                    }
                    break;
                case 2:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
