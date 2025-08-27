namespace TuneStore.Infrastructure.Security
{
    internal class SymmetricSecurityKey
    {
        private byte[] bytes;

        public SymmetricSecurityKey(byte[] bytes)
        {
            this.bytes = bytes;
        }
    }
}