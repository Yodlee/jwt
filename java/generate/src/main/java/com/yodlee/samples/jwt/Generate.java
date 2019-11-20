// Copyright (c) 2019 Yodlee, Inc. All Rights Reserved.
// Licensed under the MIT License. See `LICENSE` for details.

package com.yodlee.samples.jwt;

import io.fusionauth.jwt.Signer;
import io.fusionauth.jwt.domain.JWT;
import io.fusionauth.jwt.rsa.RSASigner;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.time.ZoneOffset;
import java.time.ZonedDateTime;

import joptsimple.OptionException;
import joptsimple.OptionParser;
import joptsimple.OptionSet;

import static java.util.Arrays.asList;

public class Generate {
	private static String issuerId;
	private static String privateKey;
	private static String username;

	private static String getKey(String file) throws IOException {
		return new String(Files.readAllBytes(Paths.get(file)));
	}

	public static void main(String[] args) throws IOException {
		OptionSet options = getOptions(args);
 
		privateKey = getKey((String)options.valueOf("key"));
		issuerId = (String)options.valueOf("issuer-id");

		username = null;

		if (options.has("username")) {
			username = (String)options.valueOf("username");
		}

		Signer signer = RSASigner.newSHA512Signer(privateKey);

		JWT jwt = new JWT().setIssuer(issuerId)
				.setSubject(username)
				.setIssuedAt(ZonedDateTime.now(ZoneOffset.UTC))
				.setExpiration(ZonedDateTime.now(ZoneOffset.UTC).plusMinutes(30));

		String encodedJWT = JWT.getEncoder().encode(jwt, signer);

		System.out.println(encodedJWT);
	}

	public static OptionSet getOptions(String[] arguments) throws IOException {
		OptionParser optionParser = new OptionParser();
		OptionSet optionSet;

		optionParser.acceptsAll(asList("k", "key")).withRequiredArg().required();
		optionParser.acceptsAll(asList("i", "issuer-id")).withRequiredArg().required();
		optionParser.acceptsAll(asList("u", "username")).withRequiredArg();
		optionParser.acceptsAll(asList("h", "help")).forHelp();	

		try {
			optionSet = optionParser.parse(arguments);
		} catch(OptionException ex) {
			System.out.println("Option parsing error: " + ex.getMessage());
			optionParser.printHelpOn(System.out);
			throw new RuntimeException();
		}

		if (optionSet.has("help")) {
			optionParser.printHelpOn(System.out);
		}

		return optionSet;
	}
}
