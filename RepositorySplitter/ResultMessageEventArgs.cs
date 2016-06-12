using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositorySplitter
{
    /// <summary>
    /// Represents new result message event arguments.
    /// </summary>
    public class ResultMessageEventArgs : EventArgs
    {
        private readonly string _ResultMessage;
        /// <summary>
        /// Gets the result message.
        /// </summary>
        public string ResultMessage
        {
            get { return _ResultMessage; }
        }

        /// <summary>
        /// Initializes a new instance of the RepositorySplitter.ResultMessageEventArgs class with the specified result message.
        /// </summary>
        /// <param name="message">The result message associated with the event.</param>
        public ResultMessageEventArgs(string message)
        {
            _ResultMessage = message;
        }
    }
}
