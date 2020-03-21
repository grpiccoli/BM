using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models.ManageViewModels
{
    public class GenerateRecoveryCodesViewModel
    {
        public List<string> RecoveryCodes { get; } = new List<string>();
    }
}
