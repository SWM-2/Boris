# Boris Proxy

Simple proxy with simple config

Create your config:

{
  "run": {
    "sources": [
      <Your Sources>
    ],
    "sinks": [
      <Your Sinks>
    ],
    "routers": [
      <Your Routers>
    ]
  }
}

## Sources
### Struct
{
  "name": <Name>,
  "type": <Type>,
  "conn": <Sink or router conected to>
}
### Types
- HTTPS = Makes a https server
- HTTP = Makes a http server
### HTTPS
Params:
- addr: Address
- port: Port
- cert_path: Private pfx path
- cert_pass: pfx password
### HTTP
- addr: Address
- port: Port

# Sinks
Idk

### Example Config

{
    "run": {
        "sources": [
            {
                "name": "Main ingress HTTPS",
                "type": "HTTPS",
                "addr": "0.0.0.0",
                "port": 5001,
                "cert_path": "cert/localhost.pfx",
                "cert_pass": "1234",
                "conn": "Header Select 1"
            },
            {
                "name": "HTTP block",
                "type": "HTTP",
                "addr": "0.0.0.0",
                "port": 5002,
                "conn": "Use HTTPS"
            },
            {
                "name": "Anubis source",
                "type": "HTTP",
                "addr": "0.0.0.0",
                "port": 5124,
                "conn": "Test HTTPS"
            }
        ],
        "sinks": [
            {
                "name": "Default HTTPS",
                "type": "HTTP",
                "addr": "localhost",
                "port": 5123
            },
            {
                "name": "Test HTTPS",
                "type": "HTTP",
                "addr": "localhost",
                "port": 5500
            },
            {
                "name": "Use HTTPS",
                "type": "STATIC",
                "path": "https.http"
            }
        ],
        "routers": [
            {
                "name": "Header Select 1",
                "type": "HDRSEL",
                "key": "host",
                "def": "Default HTTPS",
                "choices": [
                    {
                        "key": "test",
                        "value": "Test HTTPS"
                    }
                ]
            }
        ]
    },
    "exec": [
        {
            "file": "anubis",
            "args": "--bind localhost:5123 --target http://localhost:5124"
        }
    ]
}
