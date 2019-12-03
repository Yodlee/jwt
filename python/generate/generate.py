# Copyright (c) 2019 Yodlee, Inc. All Rights Reserved.
# Licensed under the MIT License. See `LICENSE` for details.

"""Usage:  generate.py [--key=<str>] [--issuer-id=<str>] [--username=<str>]
           generate.py (-h | --help)

Options:
  --key=<str>       path to private key
  --issuer-id=<str>    issuer-id id from api connsole
  --username=<str>  username if generating user token
  -h --help         Show this screen.
"""

import jwt
import time
from docopt import docopt

def generate(arguments):
    del arguments['--help']

    proceed = True
    for k, v in arguments.items():
        if k == '--username':
            continue

        if v is None:
            print(f"parameter {k} is required")
            proceed = False

    if proceed is False:
        return 'sorry'

    privateKeyPath = arguments['--key']

    with open(privateKeyPath) as f:
        privateKey = f.read()

    currentTime = int(time.time())

    payload = {}
    payload['iat'] = currentTime
    payload['exp'] = currentTime + 1800
    payload['iss'] = arguments['--issuer-id']

    if '--username' in arguments and arguments['--username'] is not None:
        payload['sub'] = arguments['--username']

    # RS512 requires $ pip install cryptography
    encoded_jwt = jwt.encode(payload, privateKey, algorithm='RS512')
    return encoded_jwt

if __name__ == '__main__':
    arguments = docopt(__doc__)

    print(generate(arguments).decode("utf-8"))
