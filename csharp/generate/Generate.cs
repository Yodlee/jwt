// Copyright (c) 2019 Yodlee, Inc. All Rights Reserved.
// Licensed under the MIT License. See `LICENSE` for details.

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Builder;
using System.Threading.Tasks;

namespace jwt_csharp_netcore
{
    class Generate
    {
        /*
         * 
         * generate.
         *
         * Usage:
         * generate.exe --key=<str> --issuer=<str> [--username=<str>]
         * generate.exe (-h | --help)
         *
         * Options:
         *  -k, --key=<str>          Path to private key file
         *  -i, --issuer-id=<str>    Issuer id from api connsole
         *  -u, --username=<str>     Username if generating user token         
         *  -h, --help               Show this screen.
         *
         */

        static async Task Main(string[] args)
        {
            Command command = prepareOptions(args);

            command.Handler = CommandHandler.Create<FileInfo, string, string>(CreateToken);

            await command.InvokeAsync(args);
        }

        private static void CreateToken(FileInfo key, string issuerId, string username)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            var payload = new Dictionary<string, object>();

            payload["iss"] = issuerId;
            payload["iat"] = currentTime;
            payload["exp"] = currentTime + 1800;

            if (username != null)
            {
                payload["sub"] = username;
            }

            string token = CreateToken(payload, key);

            Console.WriteLine(token);
        }

        public static string CreateToken(Dictionary<string, object> payload, FileInfo privateKey)
        {
            RSAParameters rsaParams;

            using (var tr = privateKey.OpenText())
            {
                var pemReader = new PemReader(tr);

                RsaPrivateCrtKeyParameters privkey = null;
                Object obj = pemReader.ReadObject();

                if (obj != null)
                {
                    privkey = (RsaPrivateCrtKeyParameters)obj;
                }

                rsaParams = DotNetUtilities.ToRSAParameters(privkey);
            }

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);

                return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS512);
            }
        }

        public static Command prepareOptions(params string[] args)
        {
            Argument parserHook = new Argument { IsHidden = true };

            parserHook.AddValidator(command => {
                if (!command.Children.Contains("--key"))
                {
                    return "Option '--key' is required.";
                }

                if (!command.Children.Contains("--issuer-id"))
                {
                    return "Option '--issuer-id' is required.";
                }

                return null; // no error
            });

            RootCommand rootCommand = new RootCommand
            {
                new Option(new[] { "--key", "-k" }, "Path to your private key file.")
                {
                    Argument = new Argument<FileInfo>().ExistingOnly()
                },
                new Option(new[] { "--issuer-id", "-i" }, "Your issuer id.")
                {
                    Argument = new Argument<string>()
                },
                new Option(new[] { "--username", "-u" }, "A user id.")
                {
                    Argument = new Argument<string>()
                },
                parserHook
            };

            rootCommand.Description = "An application to generate JWTs";

            rootCommand.TreatUnmatchedTokensAsErrors = true;

            return rootCommand;
        }
    }
}