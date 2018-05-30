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
        /// Which BattleEntities the Item affects.
        /// </summary>
        public MoveAffectionTypes MoveAffectionType { get; protected set; } = MoveAffectionTypes.None;

        /// <summary>
        /// The types of BattleEntities to target if MoveAffectionType has Other.
        /// </summary>
        public EntityTypes[] OtherEntTypes {get; protected set; } = null;

        /// <summary>
        /// Whether to replace <see cref="EntityTypes.Enemy"/> with the opposing EntityType if it is found in <see cref="OtherEntTypes"/> or not.
        /// </summary>
        public bool GetOpposingIfEnemy { get; protected set; } = true;

        /// <summary>
        /// The HeightStates of the BattleEntities the Item can affect.
        /// </summary>
        public HeightStates[] HeightsAffected { get; protected set; } = null;

        /// <summary>
        /// The Sequence that this Item performs when used in battle.
        /// By default, it uses the base <see cref="ItemSequence"/>.
        /// <para>This returns a new instance of the sequence.</para>
        /// </summary>
        public virtual Sequence SequencePerformed => new ItemSequence(null);

        public BattleItem()
        {
            
        }

        /// <summary>
        /// Gets the ItemAction associated with the item.
        /// By default, it is the base <see cref="ItemAction"/>.
        /// <para>This returns a new instance of the action.</para>
        /// </summary>
        public virtual ItemAction GetActionAssociated(BattleEntity user)
        {
            return new ItemAction(user, this);
        }
    }
}
