namespace asp_net_po_schedule_management_server.Utils
{
    public class GlobalConfigurer
    {
        public static string JwtKey { get; set; }
        public static byte JwtExpiredMinutes { get; set; }
        public static int AngularPort { get; set; }
        public static string AngularProductionUrl { get; set; }
        public static string DbDriverVersion { get; set; }
    }
}