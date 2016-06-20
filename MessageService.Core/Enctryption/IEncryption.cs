namespace MessageService.Core.Enctryption
{
    public interface IEncryption
    {
        string Encrypt(string value);
        string Decrypt(string value);
    }
}
