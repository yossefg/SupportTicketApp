using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSuppTicket.Common.Intefaces
{
    public interface ISummarizerService
    {
            /// <summary>
            /// Summarizes the given fault description into a concise text.
            /// </summary>
            /// <param name="faultDescription">The full fault description.</param>
            /// <returns>A concise summary of the fault.</returns>
            Task<string> SummarizeFaultDescriptionAsync(string faultDescription);
        
    }
}
