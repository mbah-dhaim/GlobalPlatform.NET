namespace GlobalPlatform.NET.Reference
{
    public static class ApduInstruction
    {
        public static byte Delete = 0xE4;
        public static byte GetData = 0xCA;
        public static byte GetStatus = 0xF2;
        public static byte Install = 0xE6;
        public static byte Load = 0xE8;
        public static byte ManageChannel = 0x70;
        public static byte PutKey = 0xD8;
        public static byte Select = 0xA4;
        public static byte SetStatus = 0XF0;
        public static byte StoreData = 0xE2;
        public static byte InitializeUpdate = 0x50;
        public static byte ExternalAuthenticate = 0x82;
    }
}
