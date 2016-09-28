using System;
using System.Collections.Generic;
using System.Text;

namespace Yabe
{
    // Yabe Actions
    enum YabeActionType { ShellCommand };
    
    // Yabe Action Structure
    class YabeAction
    {
        public int id { get; set; }
        public YabeActionType type { get; set; }

        /* Action ShellCommand */
        public string shellcommand { get; set; }
        public List<string> successExitCodes { get; set; }
        public List<string> successRegexPatterns { get; set; }
    }
}
