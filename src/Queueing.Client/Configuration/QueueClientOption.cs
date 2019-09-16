using System.ComponentModel.DataAnnotations;

namespace Lib.Queueing.Client
{
    public class QueueClientOption
    {
        public QueueClientOption()
        {
            NetworkRecoveryInterval = 10; //second
            SystemRecoveryInterval = 60; //second
            DeadLetterPostFix = ".dead";
            MessageControlExchange = "MessageControl";
        }
        [Required(ErrorMessage = "System Identifier is required")]
        public string SystemIdentifier { get; set; }
        [Required(ErrorMessage = "The Host is required")]
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int SystemRecoveryInterval { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public int NetworkRecoveryInterval { get; set; }
        public bool DisableDeadLettering { get; set; }
        public string DeadLetterPostFix { get; set; }
        public bool DisableMessageControl { get; set; }

        [Required(ErrorMessage = "The Exchange is required")]
        public string MessageControlExchange { get; set; }

    }
}