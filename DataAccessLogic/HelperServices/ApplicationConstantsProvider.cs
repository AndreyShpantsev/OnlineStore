namespace DataAccessLogic.HelperServices
{
    public static class ApplicationConstantsProvider
    {
        public static int GetPageSize()
        {
            return 6;
        }

        public static int GetShortRedirectionTime()
        {
            return 5;
        }

        public static int GetLongRedirectionTime()
        {
            return 10;
        }

        public static int GetMaxRedirectionTime()
        {
            return 15;
        }

        public static string AvoidValidationCode()
        {
            return "8951d9e2-e41b-47d5-a8b4-f26d9065fc2d";
        }
    }
}