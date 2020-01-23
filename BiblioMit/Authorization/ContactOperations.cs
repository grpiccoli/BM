using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BiblioMit.Authorization
{
    public static class ContactOperations
    {
        public static OperationAuthorizationRequirement Create =   
          new OperationAuthorizationRequirement {Name=Constants.CreateOperationName};
        public static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name=Constants.ReadOperationName};  
        public static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name=Constants.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name=Constants.DeleteOperationName};
        public static OperationAuthorizationRequirement Approve = 
          new OperationAuthorizationRequirement {Name=Constants.ApproveOperationName};
        public static OperationAuthorizationRequirement Reject = 
          new OperationAuthorizationRequirement {Name=Constants.RejectOperationName};
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Crear";
        public static readonly string ReadOperationName = "Leer";
        public static readonly string UpdateOperationName = "Actualizar";
        public static readonly string DeleteOperationName = "Eliminar";
        public static readonly string ApproveOperationName = "Aprobar";
        public static readonly string RejectOperationName = "Rechazar";

        public static readonly string ContactAdministratorsRole = "ContactAdministrators";
        public static readonly string ContactManagersRole = "ContactManagers";
    }
}