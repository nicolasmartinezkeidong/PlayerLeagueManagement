namespace PlayerManagement.ViewModels
{
    /// <summary>
    /// Class to be used as a 'checkbox' to select roles
    /// </summary>
    public class RoleVM
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public bool Assigned { get; set; }
    }
}
