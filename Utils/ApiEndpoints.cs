namespace asp_net_po_schedule_management_server.Utils
{
    public static class ApiEndpoints
    {
        // kontroler AuthController
        public const string LOGIN = "login";
        public const string REGISTER = "register";
        public const string CHANGE_PASSWORD = "change-password";
        public const string REFRESH_TOKEN = "refresh-token";
        public const string SEND_RESET_PASSWORD_TOKEN = "reset-password-email";
        public const string CONFIRM_RESET_TOKEN = "confirm-reset-password";
        public const string RESET_PASSWORD = "reset-password";
        public const string SEND_EMAIL_REGISTER = "send-emails-register";
     
        //--------------------------------------------------------------------------------------------------------------
        
        // kontroler FileController
        public const string GET_AVATAR = "get-avatar";
        public const string ADD_AVATAR = "add-avatar";
        public const string GET_ALL_AVATARS = "user-avatars";
        
        //--------------------------------------------------------------------------------------------------------------
        
        // kontroler UsersController
        public const string GET_ALL_USERS = "all-users";
        public const string GET_AVAILABLE_PAGINATIONS = "available-paginations";
        public const string DELETE_MASSIVE_USERS = "delete-massive";
        public const string DELETE_ALL_USERS = "delete";
    }
}