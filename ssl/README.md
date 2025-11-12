# SSL Certificates

This directory should contain SSL certificates for HTTPS in production.

## Development (Self-Signed Certificate)

For local development, you can generate a self-signed certificate:

```bash
# Generate self-signed certificate (valid for 365 days)
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout ssl/key.pem \
  -out ssl/cert.pem \
  -subj "/C=DK/ST=Denmark/L=Copenhagen/O=BookMyHome/CN=localhost"
```

## Production

For production, use a real SSL certificate from:
- Let's Encrypt (free, automated)
- Commercial CA (Comodo, DigiCert, etc.)

Place the certificate files here:
- `cert.pem` - SSL certificate
- `key.pem` - Private key

Then uncomment the HTTPS server block in `nginx.conf`.

