using System;
using System.Collections.Generic;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Indicates that an exception supports properties that should be displayed in detailed error messages, in addition
    /// to any values stored in the <see cref="Exception.Data"/> dictionary.
    /// </summary>
    public interface ISupportsMessageData
    {
        /// <summary>
        /// Populates the given dictionary with data to be added to a detailed error message.
        /// </summary>
        /// <param name="data">The <see cref="IDictionary{TKey, TValue}"/> to add data to.</param>
        void GetMessageData(IDictionary<string, string> data);
    }
}
