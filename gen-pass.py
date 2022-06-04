# You can use this function in order to generate the bcrypt hashed password.
# This password is later used in the Prometheus configuration to set the
# authentication credentials.
#
# How to use it?
# python3 gen-pass.py

import getpass
import bcrypt

password = getpass.getpass("password: ")
hashed_password = bcrypt.hashpw(password.encode("utf-8"), bcrypt.gensalt())
print(hashed_password.decode())
