using System;
using System.Collections.Generic;

namespace BiblioMit.Models.ViewModels
{
    public class UserInitializerVM
    {
        public string Name { get; set; }
        public List<string> Roles { get; } = new List<string>();
        public List<string> Claims { get; } = new List<string>();
        public string Email { get; set; }
        public string Key { get; set; }
        public List<Plataforma> Plataforma { get; } = new List<Plataforma>();
        public Uri ImageUri { get; set; }
        public int? Rating { get; set; }
    }
}
