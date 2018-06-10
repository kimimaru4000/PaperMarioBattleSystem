using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A static class for watching GC activity within any range of code.
    /// </summary>
    public static class GCWatcher
    {
        private static int NumberOfPasses = 0;
        private static long StartMemory = 0L;
        private static long EndMemory = 0L;
        private static long MemoryDifference = 0L;

        /// <summary>
        /// How much memory was allocated in the block. This value is updated after calling <see cref="Stop"/>.
        /// </summary>
        public static long TotalMemoryAllocatedInTheBlock { get; private set; } = 0L;

        /// <summary>
        /// How much memory was freed in the block. This value is updated after calling <see cref="Stop"/>.
        /// </summary>
        public static long TotalMemoryFreedInTheBlock { get; private set; } = 0L;

        /// <summary>
        /// Starts the watcher.
        /// </summary>
        public static void Start()
        {
            StartMemory = GC.GetTotalMemory(false);
        }

        /// <summary>
        /// Stops the watcher.
        /// </summary>
        public static void Stop()
        {
            EndMemory = GC.GetTotalMemory(false);
            NumberOfPasses++;

            MemoryDifference = EndMemory - StartMemory;

            //If less than 0, we freed up more memory
            if (MemoryDifference < 0)
            {
                TotalMemoryFreedInTheBlock += MemoryDifference;
            }
            //If greater than 0, we allocated more memory
            //This is indicative of a region that generated garbage
            else if (MemoryDifference > 0)
            {
                TotalMemoryAllocatedInTheBlock += MemoryDifference;
            }
        }
        
        /// <summary>
        /// Resets all values.
        /// </summary>
        public static void Reset()
        {
            NumberOfPasses = 0;
            StartMemory = 0;
            EndMemory = 0;
            MemoryDifference = 0;
            TotalMemoryAllocatedInTheBlock = 0;
            TotalMemoryFreedInTheBlock = 0;
        }
    }
}
