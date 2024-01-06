namespace TwoFactorAuthenticatorDemo.Utility
{
    public static class Messages
    {
        // User controller
        public const string EXISTING_EMAIL = "User with this email: {0} already exists.";
        public const string EXISTING_USERNAME = "User with this username: {0} already exists.";

        // Account controller
        public const string CODE_VALIDATION = "Error validating code. Reach out to the admin if this persists.";
        public const string CODE_INVALID = "Invalid code";
        public const string INVALID_LOGIN = "Invalid login attempt.";

        // Two factor attribute
        public const string TWO_FACTOR_REQUIRED = "Two-factor login verification required.";

        //general
        public const string ACCESS_DENIED = "Access denied.";
        public const string CONTACT_ADMIN = "An error occured. Reach out to the admin if this persists.";
    }
}
