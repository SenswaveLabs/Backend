namespace Senswave.Abstractions.Entities;

public static class AllowedLengths
{
    public static class Names
    {
        public const int MinLength = 3;
        public const int MaxLength = 64;
    }

    public static class Icons
    {
        public const int MaxLength = 128;
    }

    public static class Descriptions
    {
        public const int MaxLength = 512;
    }

    public static class Logging
    {
        public const int MaxLength = 512;
    }

    public static class Configuration
    {
        public const int MaxLenght = 4096;
    }
}
