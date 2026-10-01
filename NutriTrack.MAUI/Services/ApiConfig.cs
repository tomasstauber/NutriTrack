namespace NutriTrack.MAUI.Services
{
    public static class ApiConfig
    {
#if ANDROID
        // El emulador de Android llega a la PC a través de 10.0.2.2
        // ("localhost" dentro del emulador sería el propio emulador).
        public const string BaseUrl = "http://10.0.2.2:5068/";
#else
        public const string BaseUrl = "http://localhost:5068/";
#endif
    }
}