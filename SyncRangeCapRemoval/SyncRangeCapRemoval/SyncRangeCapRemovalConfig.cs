using System;
using System.Collections.Generic;
using Torch;

namespace SyncRangeCapRemoval
{
    public class SyncRangeCapRemovalConfig : ViewModel
    {

        private int _syncMaxRange = 50000;

        public int SyncMaxRange
        {
            get => _syncMaxRange;
            set => _syncMaxRange = value;
        }
    }
}
