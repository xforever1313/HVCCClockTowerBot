//
// HvccClock - A Twitter bot that chimes the time every hour.
// Copyright (C) 2022 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HvccClock
{
    /// <remarks>
    /// Taken from this stack overflow: 
    /// https://stackoverflow.com/a/70902049/5076087
    /// </remarks>
    internal class Pkce
    {
        // ---------------- Constructor ----------------

        /// <summary>
        /// Initializes a new instance of the Pkce class.
        /// </summary>
        /// <param name="size">The size of the code verifier (43 - 128 charters).</param>
        public Pkce( uint size = 128 )
        {
            CodeVerifier = GenerateCodeVerifier( size );
            CodeChallenge = GenerateCodeChallenge( CodeVerifier );
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The randomly generating PKCE code verifier.
        /// </summary>
        public string CodeVerifier { get; private set; }

        /// <summary>
        /// Corresponding PKCE code challenge.
        /// </summary>
        public string CodeChallenge { get; private set; }

        // ---------------- Functions ----------------

        /// <summary>
        /// Generates a code_verifier based on rfc-7636.
        /// </summary>
        /// <param name="size">The size of the code verifier (43 - 128 charters).</param>
        /// <returns>A code verifier.</returns>
        /// <remarks> 
        /// code_verifier = high-entropy cryptographic random STRING using the 
        /// unreserved characters[A - Z] / [a-z] / [0-9] / "-" / "." / "_" / "~"
        /// from Section 2.3 of[RFC3986], with a minimum length of 43 characters
        /// and a maximum length of 128 characters.
        ///    
        /// ABNF for "code_verifier" is as follows.
        ///    
        /// code-verifier = 43*128unreserved
        /// unreserved = ALPHA / DIGIT / "-" / "." / "_" / "~"
        /// ALPHA = %x41-5A / %x61-7A
        /// DIGIT = % x30 - 39 
        ///    
        /// Reference: rfc-7636 https://datatracker.ietf.org/doc/html/rfc7636#section-4.1     
        ///</remarks>
        private static string GenerateCodeVerifier( uint size = 128 )
        {
            if( size < 43 || size > 128 )
                size = 128;

            const string unreservedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
            Random random = new Random();
            char[] highEntropyCryptograph = new char[size];

            for( int i = 0; i < highEntropyCryptograph.Length; i++ )
            {
                highEntropyCryptograph[i] = unreservedCharacters[random.Next( unreservedCharacters.Length )];
            }

            return new string( highEntropyCryptograph );
        }

        /// <summary>
        /// Generates a code_challenge based on rfc-7636.
        /// </summary>
        /// <param name="codeVerifier">The code verifier.</param>
        /// <returns>A code challenge.</returns>
        /// <remarks> 
        /// plain
        ///    code_challenge = code_verifier
        ///    
        /// S256
        ///    code_challenge = BASE64URL-ENCODE(SHA256(ASCII(code_verifier)))
        ///    
        /// If the client is capable of using "S256", it MUST use "S256", as
        /// "S256" is Mandatory To Implement(MTI) on the server.Clients are
        /// permitted to use "plain" only if they cannot support "S256" for some
        /// technical reason and know via out-of-band configuration that the
        /// server supports "plain".
        /// 
        /// The plain transformation is for compatibility with existing
        /// deployments and for constrained environments that can't use the S256
        /// transformation.
        ///    
        /// ABNF for "code_challenge" is as follows.
        ///    
        /// code-challenge = 43 * 128unreserved
        /// unreserved = ALPHA / DIGIT / "-" / "." / "_" / "~"
        /// ALPHA = % x41 - 5A / %x61-7A
        /// DIGIT = % x30 - 39
        /// 
        /// Reference: rfc-7636 https://datatracker.ietf.org/doc/html/rfc7636#section-4.2
        /// </remarks>
        private static string GenerateCodeChallenge( string codeVerifier )
        {
            using( var sha256 = SHA256.Create() )
            {
                var challengeBytes = sha256.ComputeHash( Encoding.UTF8.GetBytes( codeVerifier ) );
                return Base64UrlEncoder.Encode( challengeBytes );
            }
        }
    }
}
