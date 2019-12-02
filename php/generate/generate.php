<?php
// Copyright (c) 2019 Yodlee, Inc. All Rights Reserved.
// Licensed under the MIT License. See `LICENSE` for details.

error_reporting(E_STRICT);

require __DIR__ . '/vendor/autoload.php';

use \Firebase\JWT\JWT;

function generate() {
    $options = getopt("k:i:u:");

    $requiredOptions = ['k','i'];

    $errors = [];

    foreach ($requiredOptions as $opt) {
        if (!isset($options[$opt])) {
            $errors[] = "Option $opt is required";
        }
    }

    if (count($errors)) {
        throw new InvalidArgumentException(implode(".\n", $errors));
    }

    $time = time();

    $keyfile = $options["k"];

    $key = file_get_contents($keyfile);


    if ($key === false) {
        throw new Exception("Private key file ($keyfile): no such file or directory");
    }

    $payload = array(
        "iss" => $options['i'],
        "iat" => $time,
        "exp" => $time + 1800
    );

    if (isset($options['u'])) {
        $payload['sub'] = $options['u'];
    }

    return $jwt = JWT::encode($payload, $key, 'RS512');
}

if (basename(__FILE__) == $_SERVER['SCRIPT_FILENAME']) {
    try {
        echo generate() . "\n";
    } catch (Exception $e) {
        echo $e->getMessage() . ".\n";
    }
}
