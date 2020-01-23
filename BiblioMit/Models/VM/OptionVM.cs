using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models.VM
{
    public class OptionVM
    {
        public string Value { set; get; }

        public string Token { get; set; }

        public string Icon { get; set; }

        public string Subtxt { get; set; }

        public string Text { get; set; }
    }
}
