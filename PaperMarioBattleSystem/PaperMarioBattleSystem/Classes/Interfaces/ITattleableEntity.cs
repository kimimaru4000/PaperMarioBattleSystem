using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for any BattleEntity that can be tattled.
    /// </summary>
    public interface ITattleableEntity
    {
        /// <summary>
        /// Tells whether the BattleEntity can currently be tattled.
        /// <para>This is useful if the BattleEntity did something to prevent itself from being tattled.</para>
        /// </summary>
        bool CanBeTattled { get; set; }

        /// <summary>
        /// The log entry description of the BattleEntity. This is shown when viewing the Tattle Log outside of battle.
        /// <para>Each entry in the array correlates to a different set of text in the text box.</para>
        /// </summary>
        /// <returns>A string[] array whose length correlates to the number of entries in the text box.</returns>
        string[] GetTattleLogEntry();

        /// <summary>
        /// The in-battle description of the BattleEntity. This is shown when using Tattle on the BattleEntity.
        /// <para>Each entry in the array correlates to a different set of text in the dialogue box (not yet implemented - will be changed when it is).</para>
        /// </summary>
        /// <returns>A string[] array whose length correlates to the number of entries in the dialogue box.</returns>
        string[] GetTattleDescription();
    }
}
