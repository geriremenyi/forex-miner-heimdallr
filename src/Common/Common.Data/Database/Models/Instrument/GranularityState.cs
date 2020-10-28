using System;
using System.Collections.Generic;
using System.Text;

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument
{
    public enum  GranularityState
    {
        New,
        InDataLoading,
        Tradeable,
        InDelete
    }
}
