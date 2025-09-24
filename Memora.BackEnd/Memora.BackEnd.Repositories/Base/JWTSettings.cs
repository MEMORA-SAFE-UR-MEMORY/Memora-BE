namespace Memora.BackEnd.Repositories.Base
{
    public class JWTSettings
    {
        public string? SecretKey { get; set; }

        /// <summary>
        /// thông tin về nhà phát hành
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Đối tượng mà token cấp phát
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// thời gian tồn tại của accessToken
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; }

        /// <summary>
        /// thòi gian tồn tại của refreshToken
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; }
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(SecretKey))
            {
                throw new ArgumentException("SecretKey cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(Issuer))
            {
                throw new ArgumentException("Issuer cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(Audience))
            {
                throw new ArgumentException("Audience cannot be null or empty.");
            }

            if (AccessTokenExpirationMinutes <= 0)
            {
                throw new ArgumentException("AccessTokenExpirationMinutes must be greater than 0.");
            }

            if (RefreshTokenExpirationDays <= 0)
            {
                throw new ArgumentException("RefreshTokenExpirationDays must be greater than 0.");
            }

            return true;
        }
    }
}

