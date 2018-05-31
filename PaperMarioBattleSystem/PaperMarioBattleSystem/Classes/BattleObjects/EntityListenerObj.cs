using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleObject that listens to BattleEntities being added or removed from battle.
    /// </summary>
    public abstract class EntityListenerObj : BattleObject
    {
        protected BattleManager BManager = null;

        protected EntityListenerObj(BattleManager bManager)
        {
            BManager = bManager;
        }

        /// <summary>
        /// Listens to the events for adding and removing BattleEntities.
        /// </summary>
        protected void ListenToEntityEvents()
        {
            BManager.EntityAddedEvent -= EntityAdded;
            BManager.EntityAddedEvent += EntityAdded;

            BManager.EntityRemovedEvent -= EntityRemoved;
            BManager.EntityRemovedEvent += EntityRemoved;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            BManager.EntityAddedEvent -= EntityAdded;
            BManager.EntityRemovedEvent -= EntityRemoved;
        }

        /// <summary>
        /// What happens when the BattleEntity is added to battle.
        /// </summary>
        /// <param name="entity">The BattleEntity added to battle.</param>
        protected abstract void EntityAdded(BattleEntity entity);

        /// <summary>
        /// What happens when the BattleEntity is removed from battle.
        /// </summary>
        /// <param name="entity">The BattleEntity removed from battle.</param>
        protected abstract void EntityRemoved(BattleEntity entity);
    }
}
