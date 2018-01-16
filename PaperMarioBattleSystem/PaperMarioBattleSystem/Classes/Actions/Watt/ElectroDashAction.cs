using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Watt's Electro Dash move.
    /// </summary>
    public class ElectroDashAction : MoveAction
    {
        public ElectroDashAction()
        {
            Name = "Electro Dash";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(216, 845, 22, 22)),
                "Pierce enemy defense, dealing 5 damage.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown, MoveAffectionTypes.Other, 
                TargetSelectionMenu.EntitySelectionType.Single, true, null, new EntityTypes[] { EntityTypes.Neutral, EntityTypes.Enemy });

            DamageInfo = new DamageData(5, Elements.Electric, true, ContactTypes.SideDirect, ContactProperties.None, null, DamageEffects.None);

            ElectroDashSequence electroDashSequence = new ElectroDashSequence(this);
            SetMoveSequence(electroDashSequence);
            actionCommand = new GulpCommand(MoveSequence, electroDashSequence.CommandDur, 500d, 1d, Microsoft.Xna.Framework.Input.Keys.Z);
        }
    }
}
