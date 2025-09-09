using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDA.Identity;

namespace BDA.Entities
{
    public class BankDraftAction 
    {

        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        //public virtual BankDraft BankDraft { get; set; }
        public string ApplicationType { get; set; }
        public string ActionRole { get; set; }
        public DateTime On { get; set; }
        public string ById { get; set; }
        public virtual ApplicationUser By { get; set; }
        public string ActionType { get; set; }
        public string Comment { get; set; }

        public BankDraftAction()
        {
        }

        public BankDraftAction(Guid bankDraftId, string actionRole, DateTime on, string byId, string actionType, string comment = null)
        {
            this.ParentId = bankDraftId;
            this.ActionRole = actionRole;
            this.On = on;
            this.ById = byId;
            this.ActionType = actionType;
            this.Comment = comment;
        }
    }

   
}