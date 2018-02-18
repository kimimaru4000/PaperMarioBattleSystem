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
    /// The Sequence for a Duplighost's Disguise move.
    /// </summary>
    public sealed class DisguiseSequence : Sequence
    {
        private Duplighost DuplighostRef = null;
        private double DisguiseWaitDur = 800d;

        public DisguiseSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            DuplighostRef = User as Duplighost;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.DuplighostBattleAnimations.DisguiseStartName);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.DuplighostBattleAnimations.DisguiseStartName);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(AnimationGlobals.DuplighostBattleAnimations.DisguiseName);

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
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());

                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //If, for whatever reason, a Duplighost isn't using this move, then just continue
                    if (DuplighostRef == null)
                    {
                        Debug.LogWarning($"{User.Name} is not a Duplighost, so nothing will happen!");
                        ChangeSequenceBranch(SequenceBranch.End);
                        break;
                    }

                    BattleEntity entityDisguised = EntitiesAffected[0];

                    //Copy the animations of the entity
                    DuplighostRef.CopyEntityAnimations(entityDisguised.AnimManager);

                    //See which Partner the Duplighost is disguised as
                    BattlePartner partnerDisguised = entityDisguised as BattlePartner;
                    if (partnerDisguised != null)
                    {
                        DuplighostRef.PartnerTypeDisguise = partnerDisguised.PartnerType;

                        //If disguising as Watt, add Electrified, Watt's light source, and Watt's Payback
                        if (DuplighostRef.PartnerTypeDisguise == Enumerations.PartnerTypes.Watt)
                        {
                            DuplighostRef.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
                            DuplighostRef.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.LightSource,
                                partnerDisguised.EntityProperties.GetAdditionalProperty<double>(Enumerations.AdditionalProperty.LightSource));

                            //Copy Watt's Payback
                            Watt watt = partnerDisguised as Watt;

                            StatusGlobals.PaybackHolder wattPayback = watt.ElectricPayback;
                            DuplighostRef.PaybackCopied = wattPayback;
                            DuplighostRef.EntityProperties.AddPayback(wattPayback);
                        }
                    }

                    //If copying a Flippable BattleEntity, copy its Flippable behavior
                    IFlippableEntity flippableEnt = entityDisguised as IFlippableEntity;
                    if (flippableEnt != null)
                    {
                        DuplighostRef.FlippableBehavior = flippableEnt.FlippedBehavior.CopyBehavior(DuplighostRef);
                    }

                    //Copy its Defense and height state
                    DuplighostRef.BattleStats.BaseDefense = entityDisguised.BattleStats.BaseDefense;

                    //Handle marking that it's airborne
                    if (entityDisguised.HeightState == Enumerations.HeightStates.Airborne)
                    {
                        DuplighostRef.ChangeHeightState(entityDisguised.HeightState);
                        DuplighostRef.SetBattlePosition(DuplighostRef.BattlePosition - new Vector2(0f, BattleManager.Instance.AirborneY));
                        DuplighostRef.Position = DuplighostRef.BattlePosition;
                    }

                    CurSequenceAction = new WaitSeqAction(DisguiseWaitDur);
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
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
