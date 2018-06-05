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
    /// Sushie's Tidal Wave attack
    /// </summary>
    public class TidalWaveAction : MoveAction
    {
        public TidalWaveAction(BattleEntity user) : base(user)
        {
            Name = "Tidal Wave";
            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 75, 22, 22)),
                "A surge of water hits all enemies.", Enumerations.MoveResourceTypes.FP, 6,
                Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Other, Enumerations.EntitySelectionType.All,
                true, null, User.GetOpposingEntityType());

            DamageInfo = new DamageData(0, Enumerations.Elements.Water, false, Enumerations.ContactTypes.None, Enumerations.ContactProperties.Ranged, null, 
                Enumerations.DamageEffects.None);

            SetMoveSequence(new TidalWaveSequence(this));
            actionCommand = new TidalWaveCommand(MoveSequence);
        }
    }
}
