using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using Jose;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace MusicKitTokenGenerator {
	class Program {
		static void Main (string [] args)
		{
			Console.WriteLine ("----- GENERATING TOKEN -----");

			var token = GenerateToken ();

			Console.WriteLine ("----- GENERATED TOKEN -----");
			Console.WriteLine ($"{token}\n");
		}

		static string GenerateToken ()
		{
			var algorithm = "ES256";

			/* 
			 * The Key ID of your MusicKit Private key:
			 * https://developer.apple.com/account/ios/authkey/
			 * For more information, go to Apple Music API Reference:
			 * https://developer.apple.com/library/content/documentation/NetworkingInternetWeb/Conceptual/AppleMusicWebServicesReference/SetUpWebServices.html#//apple_ref/doc/uid/TP40017625-CH2-SW1
			 */
			var keyId = "«MusicKit_Private_Key_ID»";
			Console.WriteLine ($"Your MusicKit Private Key ID: {keyId}");

			// Your Team ID from your Apple Developer Account:
			// https://developer.apple.com/account/#/membership/
			var teamId = "«Team_ID»";
			Console.WriteLine ($"Your Apple Team ID: {keyId}");

			var utcNow = DateTime.UtcNow;
			var epoch = new DateTime (1970, 1, 1);
			var epochNow = (int) utcNow.Subtract (epoch).TotalSeconds;
			var utcExpire = utcNow.AddMonths (6);
			var epochExpire = (int) utcExpire.Subtract (epoch).TotalSeconds;

			Console.WriteLine ($"The Token was issued at (UTC Time): {utcNow.ToString ("yyyy/MM/dd")}");
			Console.WriteLine ($"The Token will expire at (UTC Time): {utcExpire.ToString ("yyyy/MM/dd")}\n");

			var headers = new Dictionary<string, object>
			{
				{ "alg", algorithm },
				{ "kid", keyId }
			};
			var payload = new Dictionary<string, object>
			{
				{ "iss", teamId },
				{ "iat", epochNow },
				{ "exp", epochExpire }
			};

			var headersString = string.Join ($",\n{" ",4}", headers.Select (kv => $"{kv.Key}: {kv.Value}"));
			Console.WriteLine ($"Headers to be encoded:");
			Console.WriteLine ($"{{\n{" ",4}{headersString}\n}}\n");

			var payloadString = string.Join ($",\n{" ",4}", payload.Select (kv => $"{kv.Key}: {kv.Value}"));
			Console.WriteLine ($"Payload to be encoded:");
			Console.WriteLine ($"{{\n{" ",4}{payloadString}\n}}\n");

			var parameters = GetPrivateParameters ();
			var secretKey = ECDsa.Create (parameters);
			var token = JWT.Encode (payload, secretKey, JwsAlgorithm.ES256, headers);

			return token;
		}

		static ECParameters GetPrivateParameters ()
		{
			using (var reader = File.OpenText ("/path/to/your/MusicKit_Secret_Key.p8")) {
				var ecPrivateKeyParameters = (ECPrivateKeyParameters) new PemReader (reader).ReadObject ();
				var x = ecPrivateKeyParameters.Parameters.G.AffineXCoord.GetEncoded ();
				var y = ecPrivateKeyParameters.Parameters.G.AffineYCoord.GetEncoded ();
				var d = ecPrivateKeyParameters.D.ToByteArrayUnsigned ();

				var parameters = new ECParameters {
					Curve = ECCurve.NamedCurves.nistP256,
					D = d,
					Q = new ECPoint {
						X = x,
						Y = y
					}
				};

				return parameters;
			}
		}
	}
}
