using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.ViewModel
{
   
    public class BankDraftActionViewModel
    {
        public Guid BdId { get; set; }
        public string ActionRole { get; set; }
        public string Comment { get; set; }
        public string ById { get; set; }
        public string ActionType{ get; set; }
       
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public string On { get; set; }
    }

}
