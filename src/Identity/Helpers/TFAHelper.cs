using Google.Authenticator;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Helpers
{
    public class TFAHelper
    {
        private readonly string _issuer;
        public TFAHelper(string issuer)
        {
            this._issuer = issuer;
        }

        /// <summary>
        /// TFA details creation method
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public (string manualKey, string QrCode, string authenticationKey) CreateTFADetails(string email)
        {
            string authenticationKey = string.Concat((new Guid()).ToString(), email);
            TwoFactorAuthenticator TFA = new TwoFactorAuthenticator();
            var TFAInfo = TFA.GenerateSetupCode(_issuer, email, ConvertSecretToBytes(authenticationKey, false), 300);

            return
                (
                manualKey: TFAInfo.ManualEntryKey,
                QrCode: TFAInfo.QrCodeSetupImageUrl,
                authenticationKey
                );
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

        /// <summary>
        /// Two factor check method
        /// </summary>
        /// <param name="gaCode"></param>
        /// <param name="authenticationKey"></param>
        /// <returns></returns>
        public bool TFAValidation(string gaCode,
                                  string authenticationKey) =>
            new TwoFactorAuthenticator().ValidateTwoFactorPIN(authenticationKey, gaCode);
    }
}
