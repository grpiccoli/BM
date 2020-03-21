using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BiblioMit.Authorization
{
    public static class ContactOperations
    {
        public readonly static OperationAuthorizationRequirement Create =   
          new OperationAuthorizationRequirement {Name=Constants.CreateOperationName};
        public readonly static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name=Constants.ReadOperationName};  
        public readonly static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name=Constants.UpdateOperationName}; 
        public readonly static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name=Constants.DeleteOperationName};
        public readonly static OperationAuthorizationRequirement Approve = 
          new OperationAuthorizationRequirement {Name=Constants.ApproveOperationName};
        public readonly static OperationAuthorizationRequirement Reject = 
          new OperationAuthorizationRequirement {Name=Constants.RejectOperationName};
    }

    public static class Constants
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