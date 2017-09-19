using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.TargetSelectionMenu;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for items used in battle.
    /// When making an item used in battle, derive from this instead of the Item base.
    /// </summary>
    public abstract class BattleItem : Item
    {
        /// <summary>
        /// The targets to select.
        /// </summary>
        public EntitySelectionType SelectionType { get; protected set; } = EntitySelectionType.Single;

        /// <summary>
        /// The types of BattleEntities to target.
        /// </summary>
        public EntityTypes EntityType { get; protected set; } = EntityTypes.Enemy;

        /// <summary>
        /// The HeightStates of the BattleEntities the Item can affect.
        /// </summary>
        public HeightStates[] HeightsAffected { get; protected set; } = null;

        /// <summary>
        /// Whether the Item can only be used on the BattleEntity using it.
        /// </summary>
        public bool TargetsSelf { get; protected set; } = false;

        /// <summary>
        /// The Sequence that this Item performs when used in battle.
        /// By default, it uses the base ItemSequence.
        /// </summary>
        public virtual Sequence SequencePerformed { get; protected set; } = new ItemSequence(null);

        public BattleItem()
        {
            
        }
    }
}
