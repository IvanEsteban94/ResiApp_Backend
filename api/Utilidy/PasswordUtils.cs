namespace api.Utilidy
{
    public class PasswordUtils
    {
        public static string Hash(string input)
        {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        public static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }

        public static bool VerifySecurityWord(string inputSecurityWord, string hashedSecurityWord)
        {
            return BCrypt.Net.BCrypt.Verify(inputSecurityWord, hashedSecurityWord);
        }
    }
}
