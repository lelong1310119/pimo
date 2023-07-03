using Newtonsoft.Json;
using PismoWebInput.Core.Infrastructure.Common.Extensions;

namespace PismoWebInput.Core
{
    public static class PermissionNames
    {
        //Logs permissions
        public const string LogView = $"{EmployeePermissions.Schema}.{EmployeePermissions.View}";

        //Admin permissions
        public const string Superadmin = "AllowAll";

        public const string ManageUsersView = $"{LeavePermissions.Schema}.{LeavePermissions.View}";
        public const string ManageUsersUpdate = $"{LeavePermissions.Schema}.{LeavePermissions.Update}";
        
        public const string SuperAdminGroupName = "SuperAdmin";

        public static string GetPermissionNames()
        {
            dynamic result = new System.Dynamic.ExpandoObject();

            // Employee
             ((IDictionary<string, dynamic>)result)[EmployeePermissions.Schema] = new System.Dynamic.ExpandoObject();
            var logActions = typeof(EmployeePermissions).ToDictionary();
            foreach (var action in logActions.Where(x => x.Key != "Schema"))
            {
                ((IDictionary<string, dynamic>)((IDictionary<string, dynamic>)result)[EmployeePermissions.Schema])[action.Value.ToString()] = false;
            }

             // Leave
             ((IDictionary<string, dynamic>)result)[LeavePermissions.Schema] = new System.Dynamic.ExpandoObject();
            var manageUserActions = typeof(LeavePermissions).ToDictionary();
            foreach (var action in manageUserActions.Where(x => x.Key != "Schema"))
            {
                ((IDictionary<string, dynamic>)((IDictionary<string, dynamic>)result)[LeavePermissions.Schema])[action.Value.ToString()] = false;
            }

             // Asset
             ((IDictionary<string, dynamic>)result)[AssetPermissions.Schema] = new System.Dynamic.ExpandoObject();
            var assetActions = typeof(AssetPermissions).ToDictionary();
            foreach (var action in manageUserActions.Where(x => x.Key != "Schema"))
            {
                ((IDictionary<string, dynamic>)((IDictionary<string, dynamic>)result)[AssetPermissions.Schema])[action.Value.ToString()] = false;
            }

             // Role
             ((IDictionary<string, dynamic>)result)[RolePermissions.Schema] = new System.Dynamic.ExpandoObject();
            var roleActions = typeof(RolePermissions).ToDictionary();
            foreach (var action in manageUserActions.Where(x => x.Key != "Schema"))
            {
                ((IDictionary<string, dynamic>)((IDictionary<string, dynamic>)result)[RolePermissions.Schema])[action.Value.ToString()] = false;
            }

            return JsonConvert.SerializeObject(result);
        }
    }

    static class EmployeePermissions
    {
        public const string Schema = "Employee";

        public const string View = "View";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }
    
    static class LeavePermissions
    {
        public const string Schema = "Leave";

        public const string View = "View";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }

    static class AssetPermissions
    {
        public const string Schema = "Asset";

        public const string View = "View";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }

    static class RolePermissions
    {
        public const string Schema = "Role";

        public const string View = "View";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }
}
