// Copyright (c) 2019 Yodlee, Inc. All Rights Reserved.
// Licensed under the MIT License. See `LICENSE` for details.

/**
 * Usage:
 *
 * $ node generate.js --key=/path/to/your/private.key --issuer-id=issuer-id --username=username
 */

const fs = require('fs')
const jwt = require('jsonwebtoken')
const argv = require('yargs')
  .option('key', {
    alias: 'k',
    describe: 'Path to your private key'
  })
  .option('issuer-id', {
    alias: 'i',
    describe: 'Your issuer id'
  })
  .option('username', {
    alias: 'u',
    describe: 'Create a user token'
  })
  .demandOption(['key', 'issuer-id'], 'Please provide both a key file and issuer id')
  .requiresArg(['key', 'issuer-id', 'username'])
  .help()
  .argv

function generate() {
    const keyFile = argv.key

    //Location of the file with your private key
    const privateKey = fs.readFileSync(keyFile, "utf8");

    const currentTime =  Math.floor(Date.now() / 1000);

    const signOptions = {
	    algorithm: "RS512"  //Yodlee requires RS512 algorithm when encoding JWT
    };

    let payload = {};

    // The issuer id from the API Dashboard
    payload.iss = argv['issuer-id']
    payload.iat = currentTime;  //Epoch time when token is issued in seconds
    payload.exp = currentTime + 1800;  //Epoch time when token is set to expire. Must be 1800 seconds

    //generate user token if present in argv
    if (typeof argv.username != 'undefined') {
        payload.sub = argv.username
    }

    return token = jwt.sign(payload, privateKey, signOptions);
}

module.exports = generate

if (require.main === module) {
    console.log(generate())
}
