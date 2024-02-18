using System;
using System.Collections.Generic;

namespace ComplexAssetAdministrationShellScenario.Serializers
{
    public class Usecase2data
    {
        public string ConversationID { get; set; }
        public string MessagID { get; set; }
        public List<Interrup> Interruptions { get; set; } 
    }
    public class Interrup
    {
        public string Interruption { get; set; }
        public string MachineName { get; set; }
        public DateTime StartInterruptionDateTime { get; set; }
        public DateTime EndInterruptionDateTime { get; set; }
    }
}