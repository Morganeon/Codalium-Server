using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.NPCs
{
    public class PlayerConditions
    {
        public enum Reason
        {
            PlayerLevelGreaterThan,
            PlayerLevelLowerThan,
            QuestDone,
            ObjectRequired,
            ObjectUsable,
        }

        public Reason reason;

        public int objectQuantityGreaterThan;
        public int objectQuantityLowerThan;
        public bool consumeItems;
        public bool triggerCooldown;

    }

    public class DialogChoice
    {
        public string choice;
        public int jumpTo;
        public bool endDialog;
    }

    public class DialogLine
    {
        public string dialog;
        public List<DialogChoice> choices;
    }

    public class Dialog
    {
        public List<PlayerConditions> conditions;

        // dialogue, options, pages de l'option
        public List<DialogLine> dialogLines;
    }

    class DialogComponent
    {
        public List<Dialog> dialogs;
    }
}
