# Use this function to generate the bcrypt hashed password.
# This password is later used in the Prometheus configuration to set the
# authentication credentials.
#
# How to use it?
# python3 gen-pass.py
#
# And to install the bcrypt package, you can use a virtual environment:
# python3 -m venv venv
# source venv/bin/activate
# pip install -r requirements.txt

import getpass
import bcrypt

password = getpass.getpass("password: ")
hashed_password = bcrypt.hashpw(password.encode("utf-8"), bcrypt.gensalt())
print(hashed_password.decode())

# to check a password against the hash, you can use:
# bcrypt.checkpw(b"Pl34s3_d0n7_h4ck_m3", b"$2a$11$v39y2Fb6/jM5p5Oe4yKHgeAmWRrsqs0kncC5NdCL1PjzN0hftOmXy")