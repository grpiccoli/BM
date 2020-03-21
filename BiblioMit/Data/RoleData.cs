using System.Collections.Generic;

namespace BiblioMit.Data
{
    public static class RoleData
    {
        public static List<string> AppRoles { get; } = new List<string>
                                                            {
                                                                "Administrador",
                                                                "Editor",
                                                                "Invitado"
                                                            };
    }
}
